using ProductCatalog.Api.Extensions;
using ProductCatalog.Api.Infrastructure;
using ProductCatalog.Application;
using ProductCatalog.Infrastructure;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services));

    builder.Services.AddProblemDetails(options =>
        options.CustomizeProblemDetails = context => 
            context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier);
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

    builder.Services
        .AddApplication()
        .AddInfrastructure(builder.Configuration)
        .AddJwtBearerAuth();

    builder.Services.AddControllers();
    builder.Services.AddOpenApiWithBearer();

    const string frontendCorsPolicy = "Frontend";
    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
    builder.Services.AddCors(options => options.AddPolicy(frontendCorsPolicy, policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()));

    var app = builder.Build();

    await app.Services.MigrateDatabaseAsync();

    app.UseSerilogRequestLogging();
    app.UseExceptionHandler();

    app.MapApiDocs();

    app.UseCors(frontendCorsPolicy);
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapHealthChecks("/health").AllowAnonymous();
    app.MapControllers();

    await app.RunAsync();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
