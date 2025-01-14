using AutoMapper;
using System.Security.Cryptography;
using System.Text;
using MGDistributedLoggingSystem.Helpers;
using MGDistributedLoggingSystem.Data.Entities;
using Microsoft.Extensions.Options;
using MGDistributedLoggingSystem.Configurations;
using Serilog;
using MGDistributedLoggingSystem.Data.Entities.Enums;
using MGDistributedLoggingSystem.Models;
using MGDistributedLoggingSystem.Models.Dtos.LogEntryDto;

public class S3LogEntryStorageService : IS3LogEntryStorageService
{
    private readonly S3Config _S3StorageOption;
    private readonly HttpClient _httpClient;
    private readonly IMapper _mapper;

    public S3LogEntryStorageService(IOptions<S3Config> S3StorageOption, HttpClient httpClient, IMapper mapper)
    {
        _S3StorageOption = S3StorageOption.Value;
        _httpClient = httpClient;
        _mapper = mapper;
    }
    public async Task<BaseResponse<PaginatedList<LogEntryResponseDto>>> RetrieveLogsAsync(LogEntryFilterDto logEntryFilterDto, int pageIndex, int pageSize)
    {
        try
        {
            var logs = new List<LogEntry>();

            var allLogObjects = await SendRequestToServerAsync();
            foreach (var logObject in allLogObjects)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_S3StorageOption.Endpoint}/{_S3StorageOption.BucketName}/");
                HandleRequest(request, "GET");

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var logEntry = ParseLogEntryContent(content, logObject.ObjectKey);
                if (logEntry != null)
                {
                    logs.Add(logEntry);
                }
            }

            if (logEntryFilterDto != null)
            {
                var filter = _mapper.Map<LogEntryFilter>(logEntryFilterDto);

                if (!string.IsNullOrEmpty(filter.Service))
                {
                    logs = logs.Where(log => log.Service.Contains(filter.Service, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                if (filter.Level.HasValue)
                {
                    logs = logs.Where(log => log.Level.Equals(filter.Level)).ToList();
                }
                if (filter.StartTime.HasValue)
                {
                    logs = logs.Where(log => log.Timestamp >= filter.StartTime.Value).ToList();
                }
                if (filter.EndTime.HasValue)
                {
                    logs = logs.Where(log => log.Timestamp <= filter.EndTime.Value).ToList();
                }
            }

            var totalLogs = logs.Count;
            var paginatedLogs = logs.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            var result = _mapper.Map<List<LogEntryResponseDto>>(paginatedLogs);

            Log.Information($"Retrieved {result.Count()} log entries on page {pageIndex} of {totalLogs} total logs.");

            return new BaseResponse<PaginatedList<LogEntryResponseDto>>(
                $"Retrieved {result.Count()} log entries on page {pageIndex} of {totalLogs} total logs.",
                true,
                new PaginatedList<LogEntryResponseDto>(result, totalLogs, pageIndex, pageSize)
            );
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while retrieving log entries.");
            return new BaseResponse<PaginatedList<LogEntryResponseDto>>(
                "An error occurred while retrieving log entries.",
                false,
                null
            );
        }
    }
    public async Task<BaseResponse<string>> StoreLogAsync(LogEntryAddDto entity)
    {
        try
        {
            var objectKey = $"{entity.Service}/{entity.Timestamp:yyyy-MM-dd_HH-mm-ss}.txt";
            var content = $"{entity.Timestamp} [{entity.Level}] {entity.Service}: {entity.Message}";
            var request = new HttpRequestMessage(HttpMethod.Put, $"{_S3StorageOption.Endpoint}/{_S3StorageOption.BucketName}/{objectKey}")
            {
                Content = new StringContent(content, Encoding.UTF8, "text/plain")
            };
            HandleRequest(request, "PUT");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Log.Information($"Log [{entity.Service}] saved to {entity.StorageType} successfully");

            return new BaseResponse<string>(
                $"Log [{entity.Service}] saved to {entity.StorageType} successfully",
                true,
                $"{entity.Service}/{entity.Timestamp:yyyy-MM-dd_HH-mm-ss}.txt"
            );
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error storing log in S3.");
            return new BaseResponse<string>(
                "Failed to store log in S3.",
                false,
                null
            );
        }
    }

    private void HandleRequest(HttpRequestMessage request, string method)
    {
        var date = DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ");
        request.Headers.Add("x-amz-date", date);

        var canonicalRequest = $"{method}\n/{_S3StorageOption.BucketName}/{request.RequestUri.AbsolutePath}\n\nhost:{_S3StorageOption.Endpoint}\nx-amz-date:{date}\n\nhost;x-amz-date\nUNSIGNED-PAYLOAD";
        var stringToSign = $"AWS4-HMAC-SHA256\n{date}\n{date.Substring(0, 8)}/us-east-1/s3/aws4_request\n{Hash(canonicalRequest)}";
        var signature = HmacSha256(stringToSign, _S3StorageOption.SecretKey);

        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("AWS4-HMAC-SHA256", $"Credential={_S3StorageOption.AccessKey},SignedHeaders=host;x-amz-date,Signature={signature}");
    }
    private async Task<IEnumerable<S3LogEntryMetadata>> SendRequestToServerAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_S3StorageOption.Endpoint}/{_S3StorageOption.BucketName}?list-type=2");
        HandleRequest(request, "GET");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return HandleResponse(content);
    }
 
    private LogEntry ParseLogEntryContent(string content, string objectKey)
    {
        try
        {
            var parts = content.Split(new[] { '[', ']', ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 4) return null;

            return new LogEntry
            {
                Timestamp = DateTime.Parse(parts[0].Trim()),
                Level = Enum.TryParse(parts[1], out LogEntryLevel result) ? result : LogEntryLevel.Info,
                Service = parts[2].Trim(),
                Message = string.Join(':', parts.Skip(3)).Trim()
            };
        }
        catch
        {
            return null;
        }
    }
 
    private IEnumerable<S3LogEntryMetadata> HandleResponse(string xmlContent)
    {
        var logs = new List<S3LogEntryMetadata>();
        var xmlDoc = new System.Xml.XmlDocument();
        xmlDoc.LoadXml(xmlContent);

        var nodes = xmlDoc.GetElementsByTagName("Contents");
        foreach (System.Xml.XmlNode node in nodes)
        {
            var key = node["Key"]?.InnerText;
            var lastModified = node["LastModified"]?.InnerText;

            if (!string.IsNullOrEmpty(key))
            {
                logs.Add(new S3LogEntryMetadata
                {
                    ObjectKey = key,
                    LastModified = DateTime.Parse(lastModified)
                });
            }
        }

        return logs;
    }
  
    private string Hash(string text)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

    private string HmacSha256(string data, string key)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

    public Task<BaseResponse<LogEntryResponseDto>> GetLogById(int id)
    {
        throw new NotImplementedException();
    }
}
