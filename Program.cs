using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (minimal_api.Domain.DTOs.LoginDTO loginDTO) =>{
    if (loginDTO.Email == "admin@teste.com" && loginDTO.Senha == "12345678")
        return Results.Ok("login com sucesso");
    else 
        return Results.Unauthorized();
});


app.Run();

