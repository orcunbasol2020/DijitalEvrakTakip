using DijitalEvrakTakip.Application.Behaviors;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Repositories;
using DijitalEvrakTakip.Persistance.Context;
using DijitalEvrakTakip.Persistance.Repositories;
using DijitalEvrakTakip.Persistance.Services;
using DijitalEvrakTakip.WebApi.Middleware;
using FluentValidation;
using GenericRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

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
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddTransient<ErrorMiddleware>();
builder.Services.AddScoped<IUnitOfWork>(srv => srv.GetRequiredService<AppDbContext>());
builder.Services.AddScoped<IIncomingDocumentService, IncomingDocumentService>();
builder.Services.AddScoped<IIncomingDocumentApplicationService, IncomingDocumentApplicationService>();
builder.Services.AddScoped<IOutgoingDocumentService, OutgoingDocumentService>();
builder.Services.AddScoped<IExternalInstitutionService, ExternalInstitutionService>();
builder.Services.AddScoped<IDocumentTransactionService, DocumentTransactionService>();
builder.Services.AddScoped<IDocumentAllocationService, DocumentAllocationService>();
builder.Services.AddScoped<IOutgoingDocumentAllocationService, OutgoingDocumentAllocationService>();
builder.Services.AddScoped<IDocumentAssignmentService, DocumentAssignmentService>();
builder.Services.AddScoped<IScannedDocumentService, ScannedDocumentService>();
builder.Services.AddScoped<IEnvelopeService, EnvelopeService>();
builder.Services.AddScoped<IEnvelopeDocumentService, EnvelopeDocumentService>();
builder.Services.AddScoped<IExternalUserService, ExternalUserService>();
builder.Services.AddScoped<IAtlasEbysService, AtlasEbysService>();
builder.Services.AddScoped<ILanguageService, LanguageService>();

//repository
builder.Services.AddScoped<IExternalUserRepository, ExternalUserRepository>();
builder.Services.AddScoped<IEnvelopeDocumentRepository, EnvelopeDocumentRepository>();
builder.Services.AddScoped<IEnvelopeRepository, EnvelopeRepository>();
builder.Services.AddScoped<IScannedDocumentRepository, ScannedDocumentRepository>();
builder.Services.AddScoped<IDocumentAssignmentRepository, DocumentAssignmentRepository>();
builder.Services.AddScoped<IIncomingDocumentRepository, IncomingDocumentRepository>();
builder.Services.AddScoped<IOutgoingDocumentRepository, OutgoingDocumentRepository>();
builder.Services.AddScoped<IOutgoingDocumentTransactionRepository, OutgoingDocumentTransactionRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IExternalInstitutionRepository, ExternalInstitutionRepository>();
builder.Services.AddScoped<IDocumentTransactionRepository, DocumentTransactionRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IDocumentAllocationRepository, DocumentAllocationRepository>();
builder.Services.AddScoped<IOutgoingDocumentAllocationRepository, OutgoingDocumentAllocationRepository>();
builder.Services.AddScoped<IAtlasZimmetChangeRepository, AtlasZimmetChangeRepository>();
builder.Services.AddScoped<ILanguageRepository, LanguageRepository>();

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
app.MapControllers();
app.MapOpenApi();
app.MapScalarApiReference();

app.Run();
