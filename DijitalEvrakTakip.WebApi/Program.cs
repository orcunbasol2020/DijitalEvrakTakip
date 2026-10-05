using DijitalEvrakTakip.WebApi.BackgroundServices;
using DijitalEvrakTakip.Application.Behaviors;
using DijitalEvrakTakip.Application.Options;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Repositories;
using DijitalEvrakTakip.Infrastructure.Atlas;
using DijitalEvrakTakip.Infrastructure.Eyp;
using DijitalEvrakTakip.Persistance.Context;
using DijitalEvrakTakip.Persistance.Repositories;
using DijitalEvrakTakip.Persistance.Services;
using DijitalEvrakTakip.WebApi.Middleware;
using FluentValidation;
using GenericRepository;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
//services 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin();
        builder.AllowAnyMethod();
        builder.AllowAnyHeader();
    });
});

//service
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<IUserLoginLogService, UserLoginLogService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddTransient<ErrorMiddleware>();
builder.Services.AddScoped<IUnitOfWork>(srv => srv.GetRequiredService<AppDbContext>());
builder.Services.AddScoped<IIncomingDocumentService, IncomingDocumentService>();
builder.Services.AddScoped<IIncomingDocumentApplicationService, IncomingDocumentApplicationService>();
builder.Services.AddScoped<IOutgoingDocumentService, OutgoingDocumentService>();
builder.Services.AddScoped<IOutgoingDocumentDistributionService, OutgoingDocumentDistributionService>();
builder.Services.AddScoped<IOutgoingDocumentShipmentService, OutgoingDocumentShipmentService>();
builder.Services.AddScoped<IExternalInstitutionService, ExternalInstitutionService>();
builder.Services.AddScoped<IDocumentTransactionService, DocumentTransactionService>();
builder.Services.AddScoped<IDocumentAllocationService, DocumentAllocationService>();
builder.Services.AddScoped<IOutgoingDocumentAllocationService, OutgoingDocumentAllocationService>();
builder.Services.AddScoped<IUserActiveDocumentService, UserActiveDocumentService>();
builder.Services.AddScoped<IDocumentAssignmentService, DocumentAssignmentService>();
builder.Services.AddScoped<IScannedDocumentService, ScannedDocumentService>();
builder.Services.AddScoped<IEnvelopeService, EnvelopeService>();
builder.Services.AddScoped<IEnvelopeDocumentService, EnvelopeDocumentService>();
builder.Services.AddScoped<IExternalUserService, ExternalUserService>();
builder.Services.AddScoped<IAtlasEbysService, AtlasEbysService>();
builder.Services.AddScoped<ILanguageService, LanguageService>();
builder.Services.AddScoped<IAppSettingService, AppSettingService>();
builder.Services.AddScoped<IScannedDocumentImportService, ScannedDocumentImportService>();
builder.Services.AddHostedService<ScannedDocumentImportBackgroundService>();
builder.Services.AddScoped<IDocumentAllocationRequestService, DocumentAllocationRequestService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddHostedService<ZimmetApprovalReminderBackgroundService>();
// Atlas EBYS web servis sözleşmesi netleşene kadar sahte istemci; gerçek istemci yazılınca burası değişir
builder.Services.AddScoped<IAtlasEbysClient, FakeAtlasEbysClient>();
builder.Services.AddScoped<IAtlasDocumentNumberPoolService, AtlasDocumentNumberPoolService>();
builder.Services.AddHostedService<AtlasDocumentNumberPoolBackgroundService>();
// e-Yazışma kütüphanesi ve Atlas teslim yöntemi netleşene kadar sahte EYP servisi ve sahte gönderim istemcisi
builder.Services.AddScoped<IEypService, FakeEypService>();
builder.Services.AddScoped<IAtlasEypTransferClient, FakeAtlasEypTransferClient>();
builder.Services.AddScoped<IAtlasTransferService, AtlasTransferService>();
builder.Services.AddHostedService<AtlasTransferBackgroundService>();
builder.Services.AddSingleton<IAppVersionProvider, AssemblyAppVersionProvider>();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IWetSignedDocumentStorageService>(_ =>
{
    string configuredPath = builder.Configuration["FileStorage:WetSignedDocumentsPath"]
        ?? "App_Data/WetSignedDocuments";

    string rootPath = Path.IsPathRooted(configuredPath)
        ? configuredPath
        : Path.Combine(builder.Environment.ContentRootPath, configuredPath);

    return new WetSignedDocumentStorageService(rootPath);
});
builder.Services.AddScoped<IIncomingDocumentStorageService>(_ =>
{
    string configuredPath = builder.Configuration["FileStorage:IncomingDocumentsPath"]
        ?? @"C:\EvrakTakip\belgeler\Processed";

    string rootPath = Path.IsPathRooted(configuredPath)
        ? configuredPath
        : Path.Combine(builder.Environment.ContentRootPath, configuredPath);

    return new IncomingDocumentStorageService(rootPath);
});
builder.Services.AddScoped<IEypPackageStorageService>(_ =>
{
    string configuredPath = builder.Configuration["FileStorage:EypPackagesPath"]
        ?? "App_Data/EypPackages";

    string rootPath = Path.IsPathRooted(configuredPath)
        ? configuredPath
        : Path.Combine(builder.Environment.ContentRootPath, configuredPath);

    return new EypPackageStorageService(rootPath);
});

//repository
builder.Services.AddScoped<IExternalUserRepository, ExternalUserRepository>();
builder.Services.AddScoped<IEnvelopeDocumentRepository, EnvelopeDocumentRepository>();
builder.Services.AddScoped<IEnvelopeRepository, EnvelopeRepository>();
builder.Services.AddScoped<IScannedDocumentRepository, ScannedDocumentRepository>();
builder.Services.AddScoped<IDocumentAssignmentRepository, DocumentAssignmentRepository>();
builder.Services.AddScoped<IIncomingDocumentRepository, IncomingDocumentRepository>();
builder.Services.AddScoped<IOutgoingDocumentRepository, OutgoingDocumentRepository>();
builder.Services.AddScoped<IOutgoingDocumentDistributionRepository, OutgoingDocumentDistributionRepository>();
builder.Services.AddScoped<IOutgoingDocumentShipmentRepository, OutgoingDocumentShipmentRepository>();
builder.Services.AddScoped<IOutgoingDocumentTransactionRepository, OutgoingDocumentTransactionRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IExternalInstitutionRepository, ExternalInstitutionRepository>();
builder.Services.AddScoped<IDocumentTransactionRepository, DocumentTransactionRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserLoginLogRepository, UserLoginLogRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IDocumentAllocationRepository, DocumentAllocationRepository>();
builder.Services.AddScoped<IOutgoingDocumentAllocationRepository, OutgoingDocumentAllocationRepository>();
builder.Services.AddScoped<IAtlasZimmetChangeRepository, AtlasZimmetChangeRepository>();
builder.Services.AddScoped<ILanguageRepository, LanguageRepository>();
builder.Services.AddScoped<IAppSettingRepository, AppSettingRepository>();
builder.Services.AddScoped<IDocumentAllocationRequestRepository, DocumentAllocationRequestRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

//authentication (JWT)
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
JwtSettings jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
    ?? throw new InvalidOperationException("Jwt ayarları (appsettings 'Jwt' bölümü) bulunamadı.");
if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey) || jwtSettings.SecretKey.Length < 32)
    throw new InvalidOperationException("Jwt:SecretKey en az 32 karakter olmalıdır.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });
builder.Services.AddAuthorization();

string connectionString = builder.Configuration.GetConnectionString("SqlServer");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddControllers()
    .AddApplicationPart(typeof(
    DijitalEvrakTakip.Presentation.AssemblyReference).Assembly);

builder.Services.AddMediatR(cfr =>
cfr.RegisterServicesFromAssembly(typeof(DijitalEvrakTakip.Application.AssemblyReference).Assembly));

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddValidatorsFromAssembly(typeof(DijitalEvrakTakip.Application.AssemblyReference).Assembly);

builder.Services.AddOpenApi();

var app = builder.Build();

//middleware
app.UseCors("AllowAll");
app.UseMiddlewareExtensions();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapOpenApi();
app.MapScalarApiReference();

app.Run();
