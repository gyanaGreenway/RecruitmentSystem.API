using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RecruitmentSystem.API.Data;
using RecruitmentSystem.API.Services;
using RecruitmentSystem.API;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Bind SMTP options from configuration
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Email:Smtp"));
// Register email sender
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();

// Swagger/OpenAPI configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Recruitment System API",
        Version = "v1",
        Description = "A comprehensive recruitment management system API",
        Contact = new OpenApiContact
        {
            Name = "Recruitment System",
            Email = "support@recruitment.com"
        },
        License = new OpenApiLicense
        {
            Name = "MIT License"
        }
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
 Array.Empty<string>()
 }
 });

    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
 options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ??
 "Server=(localdb)\\mssqllocaldb;Database=RecruitmentSystemDb;Trusted_Connection=True;MultipleActiveResultSets=true"));

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "3f9KpL82xQ7mT1bCzR6vN0gHqW4ySdVu!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "RecruitmentSystem";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "RecruitmentSystem";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    // Development-friendly policy: allow any origin (including different localhost ports) and allow credentials.
    // WARNING: Allowing any origin with credentials is insecure for production. Restrict origins in production.
    options.AddPolicy("AllowAngular", policy =>
    {
        policy
     .SetIsOriginAllowed(_ => true) // allow any origin
     .AllowAnyHeader()
     .AllowAnyMethod()
     .AllowCredentials();
    });
});

// Services
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<ICandidateService, CandidateService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Recruitment System API v1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    c.DocumentTitle = "Recruitment System API Documentation";
    c.DefaultModelsExpandDepth(-1);
    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);

    // Custom CSS for beautiful UI
    c.InjectStylesheet("/swagger-ui/custom.css");
    c.InjectJavascript("/swagger-ui/custom.js");

    // Enable dark mode and custom theme
    c.EnableDeepLinking();
    c.EnableFilter();
    c.EnableValidator();
    c.DisplayRequestDuration();
});

app.UseHttpsRedirection();

// Serve static files for Swagger custom styling
app.UseStaticFiles();

app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Apply migrations & seed single HR user if configured
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    dbContext.Database.Migrate();
    var seedEmail = configuration["Seed:HR:Email"];
    var seedPassword = configuration["Seed:HR:Password"];
    if (!string.IsNullOrWhiteSpace(seedEmail) && !string.IsNullOrWhiteSpace(seedPassword) && !dbContext.Users.Any(u => u.Email == seedEmail))
    {
        dbContext.Users.Add(new RecruitmentSystem.API.Models.User
        {
            Email = seedEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(seedPassword),
            Role = RecruitmentSystem.API.Models.UserRole.HR
        });
        dbContext.SaveChanges();
    }
}

app.Run();

