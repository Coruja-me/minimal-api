using minimal_api.Domain.DTO;
using minimal_api.Infrastructure.DB;
using minimal_api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.Services;
using minimal_api.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using minimal_api.Domain.Models;

#region Builder
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddScoped<iAdminService, AdminService>();
    builder.Services.AddScoped<iVeiculoService, VeiculoService>();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddDbContext<DBContext>(options =>{
        options.UseMySql(
            builder.Configuration.GetConnectionString("mysql"),
            ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
        );
    });

    var app = builder.Build();
#endregion

#region Home
    app.MapGet("/", () => Results.Json(new Home()));
#endregion

#region LoginAdmin
    app.MapPost("/admin/login", ([FromBody] LoginDTO log, iAdminService adminService) => {
        if(adminService.Login(log) != null)
            return Results.Ok("Login realizado com sucesso! Bem vindo!");
        else
            return Results.Unauthorized();
    });
#endregion

#region Veiculos
    app.MapPost("/veiculos", ([FromBody] VeiculoDTO veic, iVeiculoService vService) => {
        var veiculo = new Veiculo{
            Nome = veic.Nome,
            Marca = veic.Marca,
            Ano = veic.Ano
        };
        vService.Incluir(veiculo);

        return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
    });
    app.MapGet("/veiculos", ([FromQuery] int? pagina, iVeiculoService vService) => {
        var veic = vService.Veiculos(pagina);

        return Results.Ok(veic);
    });
#endregion

#region app
    app.UseSwagger();
    app.UseSwaggerUI();

    app.Run();
#endregion