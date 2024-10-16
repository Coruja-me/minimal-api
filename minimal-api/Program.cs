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
    app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region LoginAdmin
    app.MapPost("/admin/login", ([FromBody] LoginDTO log, iAdminService adminService) => {
        if(adminService.Login(log) != null)
            return Results.Ok("Login realizado com sucesso! Bem vindo!");
        else
            return Results.Unauthorized();
    }).WithTags("Admin");
#endregion

#region Veiculos
    //Cadastrar
    app.MapPost("/veiculos", ([FromBody] VeiculoDTO vDTO, iVeiculoService vService) => {
        var validation = new Error();
        
        if(string.IsNullOrEmpty(vDTO.Nome))
            validation.Msgs.Add("O nome não pode ser vazio");
        else if(string.IsNullOrEmpty(vDTO.Marca))

            validation.Msgs.Add("A marca não pode ser vazia");

        else if(vDTO.Ano < 1950)
            validation.Msgs.Add("O ano não pode ser vazio e nem menor que 1950!");

        var veiculo = new Veiculo{
            Nome = vDTO.Nome,
            Marca = vDTO.Marca,
            Ano = vDTO.Ano
        };
        vService.Incluir(veiculo);

        return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
    }).WithTags("Veiculos");

    //Listar por Página
    app.MapGet("/veiculos", ([FromQuery] int? pagina, iVeiculoService vService) => {
        var veic = vService.Veiculos(pagina);

        return Results.Ok(veic);
    }).WithTags("Veiculos");

    //Listar por Id
    app.MapGet("/veiculos/{id}", ([FromRoute] int id, iVeiculoService vService) => {
        var veic = vService.LerId(id);

        if(veic == null)
            return Results.NotFound();

        return Results.Ok(veic);
    }).WithTags("Veiculos");

    //Atualizar
    app.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO vDTO, iVeiculoService vService) => {
        var veic = vService.LerId(id);

        if(veic == null)
            return Results.NotFound();
        
        veic.Nome = vDTO.Nome;
        veic.Marca = vDTO.Marca;
        veic.Ano = vDTO.Ano;

        vService.Atualizar(veic);

        return Results.Ok(veic);
    }).WithTags("Veiculos");

    //Deletar Por ID
    app.MapDelete("/veiculos/{id}", ([FromRoute] int id, iVeiculoService vService) => {
        var veic = vService.LerId(id);

        if(veic == null)
            return Results.NotFound();
        

        vService.Deletar(veic);

        return Results.NoContent();
    }).WithTags("Veiculos");    
#endregion

#region app
    app.UseSwagger();
    app.UseSwaggerUI();

    app.Run();
#endregion