//using Application.IRepository;
//using Application.IService;
//using Application.Mapper;
//using AutoMapper;
//using Infrastructure.Context;
//using Infrastructure.Repository;
//using Infrastructure.Service;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using System.Reflection;
//using System.Text;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllers();

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//    .AddJwtBearer(options =>
//    {

//        options.SaveToken = true;
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,

//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = builder.Configuration["Jwt:Issuer"],
//            ValidAudience = builder.Configuration["Jwt:Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(
//                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))

//        };
//        options.Events = new JwtBearerEvents
//        {
//            OnMessageReceived = context =>
//            {
//                var accessToken = context.Request.Query["access_token"];
//                var path = context.HttpContext.Request.Path;

//                if (!string.IsNullOrEmpty(accessToken) &&
//                    path.StartsWithSegments("/notificationHub"))
//                {
//                    context.Token = accessToken;
//                }
//                return Task.CompletedTask;
//            }
//        };
//    });





//builder.Services.AddCors(options =>
//{

//    options.AddPolicy("AllowAll", policy =>
//    {
//        policy.WithOrigins(
//            "https://localhost:60805",
//            "https://localhost:7039"
//            ).AllowAnyOrigin(


//            ).AllowAnyHeader().AllowAnyMethod();

//    });


//});

//// Add Swagger
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(options =>
//{
//    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
//    {
//        Title = "Arabic ChatBot API",
//        Version = "v1",
//        Description = "API for Arabic ChatBot Application",
//        Contact = new Microsoft.OpenApi.Models.OpenApiContact
//        {
//            Name = "Your Name",
//            Email = "your.email@example.com"
//        }
//    });
//});


//// Database
//builder.Services.AddDbContext<DBContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("default")));

//// Services
//builder.Services.AddScoped<IChatService, ChatService>();
//builder.Services.AddScoped<IMassageService, MessageService>();
//builder.Services.AddScoped<IUserService, UserService>();
//builder.Services.AddScoped(typeof(IAppRepository<>), typeof(Repository<>));
//builder.Services.AddScoped<IJwtService, JwtService>();
//builder.Services.AddScoped<IPasswordHash, PasswordHasher>();
//builder.Services.AddHttpContextAccessor();
//// AutoMapper
//builder.Services.AddAutoMapper(profiles =>
//{
//    profiles.AddProfile<ChatProfile>();
//    profiles.AddProfile<MessageProfile>();
//    profiles.AddProfile<UserProfile>();
//});

//// HttpClient for AI Service
//builder.Services.AddHttpClient<IAIService, AiService>(client =>
//{
//    client.DefaultRequestHeaders.Add("Accept", "application/json");
//    client.Timeout = TimeSpan.FromSeconds(60);
//});
////builder.Services.AddHostedService<SimpleSshTunnelService>();
////// ✅ بناء التطبيق
//var app = builder.Build();


////app.Urls.Add("http://0.0.0.0:5000");
////app.Urls.Add("https://0.0.0.0:5001");

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    // ✅ استخدام Swagger Middleware
//    app.UseSwagger();
//    app.UseSwaggerUI(options =>
//    {
//        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Arabic ChatBot API V1");
//        options.RoutePrefix = "swagger"; // جعل Swagger UI متاحاً على /swagger
//        options.DisplayRequestDuration();
//        options.EnableDeepLinking();
//        options.EnableFilter();
//    });
//}

//app.UseHttpsRedirection();
//app.UseAuthorization();
//app.MapControllers();

//app.Run();


// Program.cs
using Application.IRepository;
using Application.IService;
using Application.Mapper;
using AutoMapper;
using Infrastructure.Context;
using Infrastructure.Repository;
using Infrastructure.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// تكوين CORS أولاً
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins(
                "http://localhost:60805", // عنوان React التطويري
                "http://localhost:3000",  // عنوان React البديل
                "https://localhost:60805",
                "https://localhost:3000"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // إذا كنت تستخدم Cookies أو Authentication
        });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/notificationHub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Arabic ChatBot API",
        Version = "v1",
        Description = "API for Arabic ChatBot Application",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Your Name",
            Email = "your.email@example.com"
        }
    });
});

// Database
builder.Services.AddDbContext<DBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("default")));

// Services
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IMassageService, MessageService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped(typeof(IAppRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IPasswordHash, PasswordHasher>();
builder.Services.AddHttpContextAccessor();

// AutoMapper
builder.Services.AddAutoMapper(profiles =>
{
    profiles.AddProfile<ChatProfile>();
    profiles.AddProfile<MessageProfile>();
    profiles.AddProfile<UserProfile>();
});

// HttpClient for AI Service
builder.Services.AddHttpClient<IAIService, AiService>(client =>
{
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(60);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Arabic ChatBot API V1");
        options.RoutePrefix = "swagger";
        options.DisplayRequestDuration();
        options.EnableDeepLinking();
        options.EnableFilter();
    });
}

// إضافة Middleware بالترتيب الصحيح
app.UseHttpsRedirection();

// CORS يجب أن يكون قبل Authentication
app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();