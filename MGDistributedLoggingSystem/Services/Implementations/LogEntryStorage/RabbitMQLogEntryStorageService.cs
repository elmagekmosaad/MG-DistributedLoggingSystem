using AutoMapper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System.Collections.Concurrent;
using System.Text;
using MGDistributedLoggingSystem.Configurations;
using MGDistributedLoggingSystem.Data.Entities;
using MGDistributedLoggingSystem.Helpers;
using MGDistributedLoggingSystem.Models;
using MGDistributedLoggingSystem.Services.Interfaces.LogEntryStorage;
using MGDistributedLoggingSystem.Models.Dtos.LogEntryDto;

namespace MGDistributedLoggingSystem.Services.Implementations.LogEntryStorage
{

    public class RabbitMQLogEntryStorageService : IRabbitMQLogEntryStorageService
    {
        private readonly RabbitMQSenderOptions _option;
        private readonly IMapper _mapper;
        private IConnection _connection;
        private IChannel _channel;
        private readonly ConcurrentQueue<LogEntry> _logQueue = new();
        private readonly string queueName = "LogQueue";

        public RabbitMQLogEntryStorageService(IOptions<RabbitMQSenderOptions> options, IMapper mapper)
        {
            _option = options.Value;
            _mapper = mapper;
        }
    
        public async Task<BaseResponse<string>> StoreLogAsync(LogEntryAddDto entity)
        {
            var logMessage = _mapper.Map<LogEntryAddDto, LogEntry>(entity);
            var logMessageJson = JsonConvert.SerializeObject(logMessage);
            var body = Encoding.UTF8.GetBytes(logMessageJson);

            try
            {
                if (_connection == null)
                {
                    var factory = new ConnectionFactory()
                    {
                        UserName = _option.UserName,
                        Password = _option.Password,
                        HostName = _option.HostName,
                    };
                    _connection = await factory.CreateConnectionAsync();
                }

                var channel = await _connection.CreateChannelAsync();
                await channel.QueueDeclareAsync(queueName, false, false, false, null);
                await channel.BasicPublishAsync(exchange: "", routingKey: queueName, body: body);

                Log.Information($"Log [{entity.Service}] saved to {entity.StorageType} successfully");

                return new BaseResponse<string>($"Log [{entity.Service}] saved to {entity.StorageType} successfully", true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error storing log in RabbitMQ.");
                return new BaseResponse<string>($"Failed to store log in RabbitMQ: {ex.Message}", false);
            }
        }
        public async Task<BaseResponse<PaginatedList<LogEntryResponseDto>>> RetrieveLogsAsync(LogEntryFilterDto logEntryFilterDto, int pageIndex, int pageSize)
        {
            try
            {
                if (_connection == null)
                {
                    var factory = new ConnectionFactory()
                    {
                        UserName = _option.UserName,
                        Password = _option.Password,
                        HostName = _option.HostName,
                    };
                    _connection = await factory.CreateConnectionAsync();
                }

                _channel = await _connection.CreateChannelAsync();
                await _channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    try
                    {
                        var logEntry = JsonConvert.DeserializeObject<LogEntry>(message);
                        if (logEntry != null)
                        {
                            _logQueue.Enqueue(logEntry);
                            Log.Information($"Log received: {logEntry.Service} - {logEntry.Message}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Failed to process log: {ex.Message}");
                    }

                    await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                };

                await _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);
                Log.Information($"Consumer initialized and listening to queue: {queueName}");

                var logs = new List<LogEntryResponseDto>();
                while (_logQueue.TryDequeue(out var log))
                {
                    logs.Add(_mapper.Map<LogEntryResponseDto>(log));
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

        public Task<BaseResponse<LogEntryResponseDto>> GetLogById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
