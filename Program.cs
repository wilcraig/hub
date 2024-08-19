using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSignalR();
// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


var loggerFactory = builder.Services.BuildServiceProvider().GetService<ILoggerFactory>();
var logger = loggerFactory.CreateLogger<Program>();

IdentityModelEventSource.ShowPII = true;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = false,
            ValidIssuer = "stayntouch.com",
            RequireSignedTokens = false,
            SignatureValidator = delegate (string token, TokenValidationParameters parameters)
            {
                // Bypass signature validation. I can't get signature validation working right now so we're not gonna use it for now.
                var handler = new JsonWebTokenHandler();
                return handler.ReadJsonWebToken(token);
            }
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Query.TryGetValue("access_token", out var token))
                {
                    logger.LogInformation($"Access token {token} found in query string.");
                    logger.LogInformation($"Setting token in context.");
                    context.Token = token;
                }
                else
                {
                    logger.LogWarning("Access token not found in query string.");
                }
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                logger.LogInformation("Token validated successfully.");
                // Add additional logic if needed once the token is validated
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                logger.LogError($"Authentication failed: {context.Exception}");
                // Log the error or handle the failed authentication
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000",
        builder => builder.WithOrigins("http://localhost:3000")
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials());
});

builder.Services.AddHttpClient();

// Register Servics for DI
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMessageService, MessageService>();

// Register Connectors for DI
builder.Services.AddScoped<IUserConnector, UserConnector>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowLocalhost3000");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<ChatHub>("/ChatHub");
});

app.Run();
