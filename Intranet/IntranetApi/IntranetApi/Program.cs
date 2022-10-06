using IntranetApi.DbContext;
using IntranetApi.Mapper;
using IntranetApi.Models;
using IntranetApi.Models.Permission;
using IntranetApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MySqlConnector;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// config request handle large file
//builder.WebHost.UseKestrel(options =>
//{
//    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10);
//    options.Limits.MaxRequestBodySize = null;
//    options.Limits.MaxRequestBufferSize = null;
//    options.Limits.MaxResponseBufferSize = null;
//}).UseIIS();

//add db context
var mySQLConnection = new MySqlConnectionStringBuilder
{
    Server = builder.Configuration["MySql:Server"],
    Database = builder.Configuration["MySql:Database"],
    UserID = builder.Configuration["MySql:User"],
    Password = builder.Configuration["MySql:Password"],
    SslMode = MySqlSslMode.None,
    ConnectionLifeTime = 0,
    ConnectionTimeout = 900,
    DefaultCommandTimeout = 900,
    AllowUserVariables = true,
    AllowLoadLocalInfile = true
};

builder.Services.AddDbContext<ApplicationDbContext>(
                options => options.UseMySql(
                    connectionString: mySQLConnection.ConnectionString,
                    serverVersion: new MySqlServerVersion(builder.Configuration["MySql:Version"]),
                    mySqlOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null);
                    }));

//add identity
builder.Services.AddIdentity<User, Role>()
                .AddRoles<Role>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

//add CORS
var allowSpecificOriginsPolicy = "AllowSpecificOriginsPolicy";
var corsOrigins = (builder.Configuration["CorsOrigins"] ?? "http://localhost:8080").Split(',');
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowSpecificOriginsPolicy, builder =>
    {
        builder.WithOrigins(corsOrigins)
               .AllowAnyHeader()
               .AllowAnyMethod()
               .SetIsOriginAllowed((_) => true)
               .AllowCredentials();
    });
});

//add SignalR
//builder.Services.AddSignalR().AddHubOptions<HubClient>(options => options.EnableDetailedErrors = true);

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Add JWT configuration
builder.Services
    .AddAuthentication(option =>
    {
        option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(option =>
    {
        option.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero
        };
    });

//add claims- permissions
builder.Services.AddAuthorization(options => options.AddCustomizedAuthorizationOptions());

// add Swagger & JWT authen to Swagger
var securityScheme = new OpenApiSecurityScheme()
{
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey,
    Scheme = "Bearer",
    BearerFormat = "JWT",
    In = ParameterLocation.Header,
    Description = "JSON Web Token based security",
};
var securityReq = new OpenApiSecurityRequirement()
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[] {}
    }
};
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    //options.SwaggerDoc("v1", info);
    options.AddSecurityDefinition("Bearer", securityScheme);
    options.AddSecurityRequirement(securityReq);
});
builder.Services.AddMemoryCache();

//add DependencyInjection
//builder.Services.AddSingleton<ISendMailService, SendMailService>();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddSingleton<IFileStorageService, FileStorageService>();
builder.Services.AddSingleton<IMemoryCacheService>(sp => new MemoryCacheService(sp.GetService<IMemoryCache>(), mySQLConnection.ConnectionString));

//add Queue job in background
//builder.Services.AddHostedService<QueuedHostedService>();
//builder.Services.AddSingleton<IBackgroundTaskQueue>(ctx =>
//{
//    if (!int.TryParse(builder.Configuration["QueueCapacity"], out var queueCapacity))
//        queueCapacity = 100;
//    return new BackgroundTaskQueue(queueCapacity);
//});

//add extra mapp config
MapperConfig.AddMapperConfigs();

var app = builder.Build();

// response ExceptionHandler
app.UseExceptionHandler(c => c.Run(async context =>
{
    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    if (exception is not null)
    {
        var response = new { error = exception.Message };
        //context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
        await context.Response.WriteAsJsonAsync(response);
    }
}));

// Configure UseSwaggerUI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(config =>
    {
        config.DisplayRequestDuration();
    });
}

// CORS & Authen & Authorize
app.UseHttpsRedirection();

app.UseCors(allowSpecificOriginsPolicy);
app.UseAuthentication();
app.UseAuthorization();
//app.MapHub<HubClient>("/hubClient");

//add Services
app.AddAdminUserService();
app.AddRoleDataService();
app.AddBankDataService();
app.AddBrandDataService();
app.AddDepartmentDataService();
app.AddRankDataService();
app.AddEmployeeDataService();
app.AddCurrencyDataService();
app.AddFileDataService();
app.AddStaffRecordDataService(mySQLConnection.ConnectionString);
app.AddLeaveHistoryService(mySQLConnection.ConnectionString);

app.Run();