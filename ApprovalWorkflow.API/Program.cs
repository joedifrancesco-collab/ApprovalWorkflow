using ApprovalWorkflow.Application.Interfaces;
using ApprovalWorkflow.Infrastructure.Data;
using ApprovalWorkflow.Infrastructure.Repositories;
using ApprovalWorkflow.Infrastructure.Seeding;
using ApprovalWorkflow.Infrastructure.Services;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddSingleton(new DbConnectionFactory(connectionString));
builder.Services.AddSingleton(new SchemaInitializer(new DbConnectionFactory(connectionString)));
builder.Services.AddScoped<IApprovalRequestRepository, ApprovalRequestRepository>();
builder.Services.AddScoped<IUserRepository,            UserRepository>();
builder.Services.AddScoped<IUserRoleRepository,        UserRoleRepository>();
builder.Services.AddScoped<IUserXRoleRepository,       UserXRoleRepository>();
builder.Services.AddScoped<ITierRepository,            TierRepository>();
builder.Services.AddScoped<IStatusRepository,          StatusRepository>();
builder.Services.AddScoped<ICardRequestRepository,     CardRequestRepository>();
builder.Services.AddScoped<IRequestAuditLogRepository, RequestAuditLogRepository>();
builder.Services.AddScoped<ILocationRepository,             LocationRepository>();
builder.Services.AddScoped<IPasswordResetTokenRepository,   PasswordResetTokenRepository>();

var smtpSettings = builder.Configuration.GetSection("Smtp").Get<SmtpSettings>() ?? new SmtpSettings();
builder.Services.AddSingleton(smtpSettings);
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<DataSeeder>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseDefaultFiles();   // serves index.html for /
app.UseStaticFiles();    // serves files from wwwroot/
app.MapControllers();

// Initialize schema and seed the database on first run
using (var scope = app.Services.CreateScope())
{
    var schema = scope.ServiceProvider.GetRequiredService<SchemaInitializer>();
    await schema.InitializeAsync();

    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await seeder.SeedAsync();
}
app.Run();
