using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;
using Sample.Service1.Entities;
using Sample.Service1.Interfaces;
using Sample.Service1.Services;
using Sample.Service2.Interfaces;
using Sample.Service2.Services;
using Sample.Service2.Options;
using System.Net;
using WebAPI.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AzureOption>(builder.Configuration.GetSection("Azure"));
builder.Services.Configure<ProxyOption>(builder.Configuration.GetSection("Proxy"));
builder.Services.AddControllers();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:3000")
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "ClientApp";
});

// Regist application
builder.Services.AddScoped<ISampleServices1, SampleService1>();

builder.Services.AddHttpClient<IMyHttpClient, MyHttpClient>((sp, client) =>
{
    var azureOption = sp.GetRequiredService<IOptions<AzureOption>>().Value;
    if (!string.IsNullOrEmpty(azureOption?.Resource))
    {
        client.BaseAddress = new Uri(azureOption.Resource);
    }
})
.ConfigurePrimaryHttpMessageHandler(sp =>
{
    var proxyOption = sp.GetRequiredService<IOptions<ProxyOption>>().Value;
    if (!string.IsNullOrEmpty(proxyOption?.IP))
    {
        return new HttpClientHandler
        {
            Proxy = new WebProxy(proxyOption.IP)
            {
                Credentials = new NetworkCredential(proxyOption.Account, proxyOption.Password)
            },
            UseProxy = true 
        };
    }
    return new HttpClientHandler();
});

// Entity Framework Core configuration
builder.Services.AddDbContext<SampleContext>(options =>
{
    string connectionStirng = builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
    options.UseSqlServer(connectionStirng);
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// If you want CORS in non-development too, move this outside the Development block.
app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseSpaStaticFiles();

app.MapControllers();

app.UseSpa(spa =>
{
    spa.Options.SourcePath = "ClientApp";
});

app.Run();
