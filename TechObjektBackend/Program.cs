using TechObjektBackend.Models;
using TechObjektBackend.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<ProjectDatabaseSettings>(
    builder.Configuration.GetSection("ProjectDatabase"));

builder.Services.AddSingleton<PrisonersServices>();
builder.Services.AddSingleton<VaccinationDataService>();
builder.Services.AddSingleton<HeightDataService>();
builder.Services.AddScoped<EducationDataService>();

builder.Services.AddSingleton<NewBuildingsServices>();
builder.Services.AddSingleton<QueryBuilderServices>();
// Add services to the container.

builder.Services.AddControllers();
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

app.UseHttpsRedirection();

app.UseRouting();

// Konfiguracja obsługi CORS
app.UseCors(options =>
{
    options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
});

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
