using minimal_api.Domain.DTO;
using minimal_api.Infrastructure.DB;
using minimal_api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<DBContext>

builder.Services.AddDbContext<DBContext>(options =>{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (LoginDTO loginDTO) => {
    if(loginDTO.Email == "adm@teste.com" && loginDTO.Senha == "admin")
        return Results.Ok("Login realizado com sucesso!");
    else
        return Results.Unauthorized();
});

app.Run();
