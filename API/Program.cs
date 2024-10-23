using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entity;
using minimal_api.Domain.Enuns;
using minimal_api.Domain.Interfaces;
using minimal_api.Domain.ModelViews;
using minimal_api.Domain.Servicos;
using minimal_api.Infrastructure.Db;

#region Builder
var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration.GetSection("Jwt").ToString();
if(string.IsNullOrEmpty(key)) key = "123456";

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters{
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateAudience = false,
        ValidateIssuer = false,
        
    };
});

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( options => {
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira seu token JWT aqui :{Seu Token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                    {   
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
            },
            new string[] {}
        }

        
    });
});

builder.Services.AddScoped<IAdminServicos, AdminServicos>();
builder.Services.AddScoped<IVeiculosServicos, VeiculosServicos>();


builder.Services.AddDbContext<Contexto>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexaoPadrao")));

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json( new Home())).WithTags("Home");
#endregion

#region Admin
string GerarTokenJwt(Admin admin){
    if(string.IsNullOrEmpty(key)) return string.Empty;
     var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
     var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>();
    {
        new Claim("Email", admin.Email);
        new Claim("Perfil", admin.Perfil);
        new Claim(ClaimTypes.Role, admin.Perfil);
    };

     var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: credentials
     );

     return new JwtSecurityTokenHandler().WriteToken(token);
}

app.MapPost("Admins/login", ([FromBody] LoginDTO loginDTO, IAdminServicos adminServicos) =>{
    var adm = adminServicos.Login(loginDTO);
    if(adm != null)
    {
        string token = GerarTokenJwt(adm);
        return Results.Ok(new AdminLogado {
            Email = adm.Email,
            Perfil = adm.Perfil,
            Token = token
        });
    }
    else 
        return Results.Unauthorized();
}).WithTags("Admins");

app.MapPost("Admins", ([FromBody]AdminDTO adminDTO, IAdminServicos adminServicos) => {
        var admin = new Admin{
            Email = adminDTO.Email,
            Senha = adminDTO.Senha,
            Perfil = adminDTO.Perfil.ToString()
    };
    adminServicos.Incluir(admin);
    return Results.Created($"/admins/{admin.Id}", admin);
})
.RequireAuthorization(new AuthorizeAttribute{Roles = "adm"})
.WithTags("Admins");



app.MapGet("Buscar/admins", ([FromQuery]int? pagina, IAdminServicos adminServicos) => {
    var adms = new List<AdminModelsViews>();
    var admin = adminServicos.Todos(pagina);

    foreach(var adm in admin)
    {
        adms.Add(new AdminModelsViews{
            Id = adm.Id,
            Email = adm.Email,
            Perfil = adm.Perfil
        });
    }

    return Results.Ok(adms);
}).RequireAuthorization(new AuthorizeAttribute{Roles = "adm"})
.WithTags("Admins");

#endregion

#region Veiculos

app.MapPost("Adicionar/Veiculo", ([FromBody] VeiculosDTO veiculosDTO, IVeiculosServicos veiculosServicos) =>{
    var veiculo = new Veiculos{
        Nome = veiculosDTO.Nome,
        Marca = veiculosDTO.Marca,
        Ano = veiculosDTO.Ano
    };

    veiculosServicos.Incluir(veiculo);
    return Results.Created($"/Veiculos{veiculo.Id}", veiculo);
}).RequireAuthorization(new AuthorizeAttribute {Roles = "adm,editor"})
.WithTags("Veiculo"); 
app.MapGet("/Veiculos", ([FromQuery] int? pagina, IVeiculosServicos veiculosServicos) =>{
    var veiculo = veiculosServicos.Todos(pagina);

    return Results.Ok(veiculo);
}).RequireAuthorization(new AuthorizeAttribute{Roles = "adm,editor"})
.WithTags("Veiculo");

app.MapPut("Atualizar/Veiculo/{Id}", ([FromRoute]int Id, VeiculosDTO veiculosDTO, IVeiculosServicos veiculosServicos) => {
    var veiculo = veiculosServicos.BuscarPorId(Id);
    if(veiculo == null)
        return Results.NotFound();
    
    veiculo.Nome = veiculosDTO.Nome;
    veiculo.Marca = veiculosDTO.Marca;
    veiculo.Ano = veiculosDTO.Ano;
    
    veiculosServicos.Atualizar(veiculo);

    return Results.Ok(veiculo);
}).RequireAuthorization(new AuthorizeAttribute{Roles = "adm"})
.WithTags("Veiculo");

app.MapDelete("Deleta/Veiculo/{Id}", ([FromRoute] int Id, IVeiculosServicos veiculosServicos) =>{
    var veiculo = veiculosServicos.BuscarPorId(Id);
    if(veiculo == null)
    return Results.NotFound();

    veiculosServicos.Apagar(veiculo);

    return Results.NoContent();
}).RequireAuthorization(new AuthorizeAttribute{Roles = "adm"})
.WithTags("Veiculo");

#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
#endregion

