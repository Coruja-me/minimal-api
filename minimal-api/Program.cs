using minimal_api.Domain.DTO;
using minimal_api.Infrastructure.DB;
using minimal_api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.Services;
using minimal_api.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using minimal_api.Domain.Models;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<iAdminService, AdminService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DBContext>(options =>{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});

var app = builder.Build();

app.MapGet("/", () => Results.Json(new Home()));

app.MapPost("/login", ([FromBody] LoginDTO log, iAdminService adminService) => {
    if(adminService.Login(log) != null)
        return Results.Ok("Login realizado com sucesso!");
    else
        return Results.Unauthorized();
});

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
