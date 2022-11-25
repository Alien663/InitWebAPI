using Microsoft.AspNetCore.Mvc.Infrastructure;
using WebAPI.Lib;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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

builder.Services.AddSpaStaticFiles (configuration =>
{
    configuration.RootPath = "ClientApp";
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
    app.UseCors("CorsPolicy");
}
app.UseMiddleware<ExceptionMiddleware>();

app.UseStaticFiles();
app.UseSpaStaticFiles();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
     name: "default",
     pattern: "{controller}/{id?}");
});
app.UseSpa(spa =>
{
    spa.Options.SourcePath = "ClientApp";
});

app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
