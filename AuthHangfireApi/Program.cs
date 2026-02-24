using AuthHangfireApi.Authentication;
using Microsoft.OpenApi.Models;
using Hangfire;
using Hangfire.Storage;
using AuthHangfireApi.Jobs;
using AuthHangfireApi.Models;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(jwt =>
{
    jwt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    jwt.AddSecurityRequirement(new()
    {
        {
            new OpenApiSecurityScheme { Reference = new() { Id = "Bearer", Type = ReferenceType.SecurityScheme }},
            Array.Empty<string>()
        }
    });
});

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddAuthorization();

builder.Services.AddHangfire(cfg =>
    cfg.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddHangfireServer();

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings")
);
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IOptions<EmailSettings>>().Value
);

builder.Services.AddTransient<SampleJobs>();
builder.Services.AddTransient<TestCronJobs>();
builder.Services.AddTransient<EmailCronJob>();
builder.Services.AddTransient<AcesFileJob>();

var app = builder.Build();

if(app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireDashboardAuthorization() }
});

using (var scope = app.Services.CreateScope())
{
    var connection = JobStorage.Current.GetConnection();
    var jobs = connection.GetRecurringJobs();

    foreach (var job in jobs)
    {
        RecurringJob.RemoveIfExists(job.Id);
    }
}

app.MapControllers();
app.Run();
