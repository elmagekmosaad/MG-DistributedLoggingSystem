using MGDistributedLoggingSystem.AutoMapper;
using MGDistributedLoggingSystem.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using MGDistributedLoggingSystem.Authorization.Requirements;
using MGDistributedLoggingSystem.Constants;
using MGDistributedLoggingSystem.Data.Context;
using MGDistributedLoggingSystem.Data.Entities;
using MGDistributedLoggingSystem.Services.Implementations;
using MGDistributedLoggingSystem.Services.Interfaces;
using MGDistributedLoggingSystem.Services.Interfaces.LogEntryStorage;
using MGDistributedLoggingSystem.Services.Implementations.LogEntryStorage;
using MGDistributedLoggingSystem.Configurations;
using System.Reflection;
using MGDistributedLoggingSystem.Core.IRepository;
using MGDistributedLoggingSystem.Core.Repository;
using RabbitMQ.Client;

namespace MGDistributedLoggingSystem.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// For Swagger about and contact info
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSwaggerGenContactInfo(this IServiceCollection services)
        {
            return services.AddSwaggerGen(s => s.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = "Distributed Logging System",
                Version = "v1",
                Description = "Technical Test for Distributed Logging System - Nano Health Suite Company",
                Contact = new OpenApiContact()
                {
                    Name = "Mosaad Ghanem",
                    Email = "mosaadghanem97@gmail.com",
                    Url = new("https://www.linkedin.com/in/elmagekmosaad/"),
                },
            }));
        }
        /// <summary>
        /// For Swagger Authentication | adding Authorization button 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSwaggerGenAuthorizationButton(this IServiceCollection services)
        {
            return services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter the JWT key dont forget to add **Bearer** before the token"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Name = "Bearer",
                In = ParameterLocation.Header,

            },
            Array.Empty<string>()
        }
    });
            });
        }
        /// <summary>
        /// For service registration | AddScoped,AddTransient,AddSingleton 
        /// </summary>
        /// <param name="Services"></param>
        /// <returns></returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection Services, string? connectionStringConfigName)
        {
            #region ConnectedDataBase
            Services.AddDbContext<AppDbContext>(option =>
            option.UseSqlServer(connectionStringConfigName, e => e.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));
            #endregion

            Services.AddAutoMapper(Assembly.GetAssembly(typeof(MappingProfile)));

            #region DI
            Services.AddScoped<IUnitOfWork, UnitOfWork>();
            Services.AddScoped<ILogEntryRepository, LogEntryRepository>();
            Services.AddScoped<IDatabaseLogEntryStorageService, DatabaseLogEntryStorageService>();
            Services.AddScoped<IFileLogEntryStorageService, FileLogEntryStorageService>();
            //Services.AddScoped<IS3LogEntryStorageService, S3LogEntryStorageService>();
            Services.AddSingleton<IRabbitMQLogEntryStorageService, RabbitMQLogEntryStorageService>();
            Services.AddTransient<IAuthService, AuthService>();
            Services.AddTransient<IRoleService, RoleService>();
            Services.AddScoped<ITokenService, TokenService>();
            Services.AddSingleton<IAuthorizationHandler, AdminAuthorizationHandler>();
            Services.AddSingleton<IAuthorizationHandler, UserAuthorizationHandler>();
            Services.AddHttpClient<IS3LogEntryStorageService, S3LogEntryStorageService>();
            #endregion

            return Services;
        }
        /// <summary>
        /// For Add Application Identity 
        /// </summary>
        /// <param name="Services"></param>
        /// <returns></returns>
        public static IdentityBuilder AddApplicationIdentity(this IServiceCollection Services)
        {
            return Services.AddIdentity<AppUser, IdentityRole>()
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();
        }
        /// <summary>
        /// Adding Authentication to show error 401 otherwise error 404
        /// </summary>
        /// <param name="Services"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddApplicationJwtAuth(this IServiceCollection Services, JwtConfig jwt)
        {
            return Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })// Adding jwt Bearer
                .AddJwtBearer(options =>
                {
                    options.Authority = "http://localhost:5011";
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwt.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwt.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key))
                    };
                });
        }

        public static IServiceCollection AddApplicationAuthorization(this IServiceCollection services)
        {
            return services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.Admin, policy =>
                {
                    policy
                    .RequireAuthenticatedUser()
                    .RequireAssertion(context => context.User.IsInRole(Roles.Admin));

                });

                options.AddPolicy(Policies.User, policy =>
                {
                    policy
                    .RequireAuthenticatedUser()
                    .RequireAssertion(context =>
                       context.User.IsInRole(Roles.User));

                });

            });
        }
        public static async Task AddApplicationDataSeedingAsync(this IHost app)
        {
            var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
            var loggerFactory = app.Services.GetService<ILoggerProvider>();
            var logger = loggerFactory.CreateLogger("app");
            using (var scope = scopedFactory?.CreateScope())
            {
                try
                {
                    var dbcontext = scope?.ServiceProvider.GetService<AppDbContext>();
                    dbcontext.Database.EnsureCreated();

                    var userManager = scope?.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                    var roleManager = scope?.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                    var roleService = scope?.ServiceProvider.GetRequiredService<IRoleService>();

                    await new Seeds.SeedData(userManager, roleService).InitializeAsync();
                    logger.LogInformation("Finished Seeding Default Data");
                    logger.LogInformation("Application Starting");
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "An error occured while seeding data");
                }

            }


        }

    }
}
