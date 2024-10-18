using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using minimal_api;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Enuns;
using minimal_api.Infrastructure.Interfaces;
using minimal_api.Domain.Models;
using minimal_api.Domain.Services;
using minimal_api.Domain.DTO;
using minimal_api.Infrastructure.DB;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        key = Configuration?.GetSection("Jwt")?.ToString() ?? "";
    }

    private string key = "";
    public IConfiguration Configuration { get;set; } = default!;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication(option => {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(option => {
            option.TokenValidationParameters = new TokenValidationParameters{
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateIssuer = false,
                ValidateAudience = false,
            };
        });

        services.AddAuthorization();

        services.AddScoped<iAdminService, AdminService>();
        services.AddScoped<iVeiculoService, VeiculoService>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options => {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insira o token JWT aqui"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme{
                        Reference = new OpenApiReference 
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        services.AddDbContext<DBContext>(options => {
            options.UseMySql(
                Configuration.GetConnectionString("MySql"),
                ServerVersion.AutoDetect(Configuration.GetConnectionString("MySql"))
            );
        });

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseCors();

        app.UseEndpoints(endpoints => {

            #region Home
            endpoints.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
            #endregion

            #region Administradores
            string GerarTokenJwt(Admin adm){
                if(string.IsNullOrEmpty(key)) return string.Empty;

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>()
                {
                    new Claim("Email", adm.Email),
                    new Claim("Perfil", adm.Perfil),
                    new Claim(ClaimTypes.Role, adm.Perfil),
                };
                
                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }

            endpoints.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, iAdminService admService) => {
                var adm = admService.Login(loginDTO);
                if(adm != null)
                {
                    string token = GerarTokenJwt(adm);
                    return Results.Ok(new AdmLogado
                    {
                        Email = adm.Email,
                        Perfil = adm.Perfil,
                        Token = token
                    });
                }
                else
                    return Results.Unauthorized();
            }).AllowAnonymous().WithTags("Administradores");

            endpoints.MapGet("/administradores", ([FromQuery] int? pagina, iAdminService admService) => {
                var adms = new List<AdmMV>();
                var administradores = admService.Admins(pagina);
                foreach(var adm in administradores)
                {
                    adms.Add(new AdmMV{
                        Id = adm.Id,
                        Email = adm.Email,
                        Perfil = adm.Perfil
                    });
                }
                return Results.Ok(adms);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Administradores");

            endpoints.MapGet("/Administradores/{id}", ([FromRoute] int id, iAdminService admService) => {
                var administrador = admService.LerId(id);
                if(administrador == null) return Results.NotFound();
                return Results.Ok(new AdmMV{
                        Id = administrador.Id,
                        Email = administrador.Email,
                        Perfil = administrador.Perfil
                });
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Administradores");

            endpoints.MapPost("/administradores", ([FromBody] AdminDTO aDTO, iAdminService admService) => {
                var validacao = new Error{
                    Msgs = new List<string>()
                };

                if(string.IsNullOrEmpty(aDTO.Email))
                    validacao.Msgs.Add("Email não pode ser vazio");
                if(string.IsNullOrEmpty(aDTO.Senha))
                    validacao.Msgs.Add("Senha não pode ser vazia");
                if(aDTO.Perfil == null)
                    validacao.Msgs.Add("Perfil não pode ser vazio");

                if(validacao.Msgs.Count > 0)
                    return Results.BadRequest(validacao);
                
                var administrador = new Admin{
                    Email = aDTO.Email,
                    Senha = aDTO.Senha,
                    Perfil = aDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
                };

                admService.Incluir(administrador);

                return Results.Created($"/administrador/{administrador.Id}", new AdmMV{
                    Id = administrador.Id,
                    Email = administrador.Email,
                    Perfil = administrador.Perfil
                });
                
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Administradores");
            #endregion

            #region Veiculos
            Error validaDTO(VeiculoDTO veiculoDTO)
            {
                var validacao = new Error{
                    Msgs = new List<string>()
                };

                if(string.IsNullOrEmpty(veiculoDTO.Nome))
                    validacao.Msgs.Add("O nome não pode ser vazio");

                if(string.IsNullOrEmpty(veiculoDTO.Marca))
                    validacao.Msgs.Add("A Marca não pode ficar em branco");

                if(veiculoDTO.Ano < 1950)
                    validacao.Msgs.Add("Veículo muito antigo, aceito somete anos superiores a 1950");

                return validacao;
            }

            endpoints.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, iVeiculoService veiculoService) => {
                var validacao = validaDTO(veiculoDTO);
                if(validacao.Msgs.Count > 0)
                    return Results.BadRequest(validacao);
                
                var veiculo = new Veiculo{
                    Nome = veiculoDTO.Nome,
                    Marca = veiculoDTO.Marca,
                    Ano = veiculoDTO.Ano
                };
                veiculoService.Incluir(veiculo);

                return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" })
            .WithTags("Veiculos");

            endpoints.MapGet("/veiculos", ([FromQuery] int? pagina, iVeiculoService veiculoService) => {
                var veiculos = veiculoService.Veiculos(pagina);

                return Results.Ok(veiculos);
            }).RequireAuthorization().WithTags("Veiculos");

            endpoints.MapGet("/veiculos/{id}", ([FromRoute] int id, iVeiculoService veiculoService) => {
                var veiculo = veiculoService.LerId(id);
                if(veiculo == null) return Results.NotFound();
                return Results.Ok(veiculo);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" })
            .WithTags("Veiculos");

            endpoints.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, iVeiculoService veiculoService) => {
                var veiculo = veiculoService.LerId(id);
                if(veiculo == null) return Results.NotFound();
                
                var validacao = validaDTO(veiculoDTO);
                if(validacao.Msgs.Count > 0)
                    return Results.BadRequest(validacao);
                
                veiculo.Nome = veiculoDTO.Nome;
                veiculo.Marca = veiculoDTO.Marca;
                veiculo.Ano = veiculoDTO.Ano;

                veiculoService.Atualizar(veiculo);

                return Results.Ok(veiculo);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Veiculos");

            endpoints.MapDelete("/veiculos/{id}", ([FromRoute] int id, iVeiculoService veiculoService) => {
                var veiculo = veiculoService.LerId(id);
                if(veiculo == null) return Results.NotFound();

                veiculoService.Deletar(veiculo);

                return Results.NoContent();
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Veiculos");
            #endregion
        });
    }
}