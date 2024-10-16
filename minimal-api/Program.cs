using minimal_api.Domain.DTO;
using minimal_api.Infrastructure.DB;
using minimal_api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.Services;
using minimal_api.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using minimal_api.Domain.Models;
using minimal_api.Domain.Enuns;

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

#region Admin
    //LoginADM
    app.MapPost("/admin/login", ([FromBody] LoginDTO log, iAdminService aService) => {
        if(aService.Login(log) != null)
            return Results.Ok("Login realizado com sucesso! Bem vindo!");
        else
            return Results.Unauthorized();
    }).WithTags("Admin");

    //CadastrarADM
    app.MapPost("/admin", ([FromBody] AdminDTO aDTO, iAdminService aService) => {
        var validation = new Error{
            Msgs = []
        };
        if(string.IsNullOrEmpty(aDTO.Email))
            validation.Msgs.Add("O email não pode ser vazio");
        if(string.IsNullOrEmpty(aDTO.Senha))
            validation.Msgs.Add("A senha não pode ser vazia");
        if(aDTO.Perfil == null)
            validation.Msgs.Add("O Perfil não pode ser vazio");
        
        if(validation.Msgs.Count > 0)
            return Results.BadRequest(validation);

        var adm = new Admin{
            Email = aDTO.Email,
            Senha = aDTO.Senha,
            Perfil = aDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
        };

        aService.Incluir(adm);

        return Results.Created($"/admin/{adm.Id}", adm);
    }).WithTags("Admin");

    //Lista por Pagina
    app.MapGet("/admin", ([FromQuery] int? pagina, iAdminService aService) => {
        var adms = new List<AdmMV>();
        var admins = aService.Admins(pagina);
        foreach (var adm in admins){
            adms.Add(new AdmMV{
                Id = adm.Id,
                Email = adm.Email,
                Perfil = adm.Perfil
            });
        }

        return Results.Ok(adms);
    }).WithTags("Admin");

    //Lista por Id
    app.MapGet("/admin/{id}", ([FromRoute] int id, iAdminService aService) => {
        var adm = aService.LerId(id);

        if(adm == null)
            return Results.NotFound();

        return Results.Ok(adm);
    }).WithTags("Admin");
#endregion

#region Veiculos
    Error validaDTO(VeiculoDTO vDTO){
        var validation = new Error{
            Msgs = []
        };

        if(string.IsNullOrEmpty(vDTO.Nome))
            validation.Msgs.Add("O nome não pode ser vazio");
        if(string.IsNullOrEmpty(vDTO.Marca))
            validation.Msgs.Add("A marca não pode ser vazia");
        if(vDTO.Ano < 1950)
            validation.Msgs.Add("O ano não pode ser vazio e nem menor que 1950!");
        
        return validation;
    }
    //Cadastrar
    app.MapPost("/veiculos", ([FromBody] VeiculoDTO vDTO, iVeiculoService vService) => {

        var validation = validaDTO(vDTO);

        if(validation.Msgs.Count > 0)
            return Results.BadRequest(validation);

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