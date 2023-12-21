using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using blazorback.Data;
using MongoDB.Driver;
using System;
using blazorback.Mock;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly MongoDBContext _mongoDBContext;
    private readonly ILogger<AuthController> _logger;

    public AuthController(MongoDBContext mongoDBContext, ILogger<AuthController> logger)
    {
        _mongoDBContext = mongoDBContext;
        _logger = logger;
        _logger.LogInformation("AuthController foi inicializado.");
    }

   [HttpPost("cadastrar")]
public IActionResult Cadastrar([FromBody] Usuario usuario)
{
    try
    {
       
        if (!usuario.Email.EndsWith("@aluno.ifnmg.edu.br"))
        {
            _logger.LogWarning("Tentativa de cadastro com email inválido: {Email}", usuario.Email);
            return BadRequest("O email deve ser do domínio @aluno.ifnmg.edu.br");
        }

        
        var usuarioExistente = _mongoDBContext.Usuarios.Find(u => u.Email == usuario.Email).FirstOrDefault();
        if (usuarioExistente != null)
        {
            _logger.LogWarning("Tentativa de cadastro de usuário já existente: {Email}", usuario.Email);
            return BadRequest("Usuário já cadastrado com esse e-mail");
        }

       
        _mongoDBContext.Usuarios.InsertOne(usuario);
        _logger.LogInformation("Usuário cadastrado com sucesso: {Email}", usuario.Email);

        return Ok("Usuário cadastrado com sucesso");
    }
    catch (Exception ex)
    {
        _logger.LogError($"Erro ao cadastrar usuário: {ex.Message}");
        return BadRequest($"Erro ao cadastrar usuário: {ex.Message}");
    }
}
    [HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
{
    
    var usuarioAutenticado = _mongoDBContext.Usuarios.Find(u => u.Email == loginModel.Email && u.Senha == loginModel.Senha).FirstOrDefault();

    var _googleSalaDeAulaAPI = new MockGoogleSalaDeAulaAPI();

    if (usuarioAutenticado != null)
    {
        try
        {
            var informacoesUsuario = await _googleSalaDeAulaAPI.ObterInformacoesAlunoAsync(loginModel.Email);

            return Ok(new
            {
                Mensagem = "Login bem-sucedido",
                Usuario = new
                {
                    Id = informacoesUsuario.Id,
                    Nome = informacoesUsuario.Nome,
                    MateriasEmCurso =informacoesUsuario.MateriasEmCurso
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter informações do usuário do Google Sala de Aula: {Email}", loginModel.Email);
            return StatusCode(500, "Erro interno ao obter informações do usuário do Google Sala de Aula");
        }
    }

    _logger.LogWarning("Tentativa de login com credenciais inválidas: {Email}", loginModel.Email);
    return BadRequest("Credenciais inválidas");
}


    [HttpGet("usuarios")]
    public IActionResult ObterUsuarios()
    {
        try
        {
            var usuarios = _mongoDBContext.Usuarios.Find(_ => true).ToList();

            if (usuarios.Count == 0)
            {
                _logger.LogInformation("Nenhum usuário cadastrado.");
                return Ok("Nenhum usuário cadastrado.");
            }

            _logger.LogInformation("Usuários cadastrados:");
            foreach (var usuario in usuarios)
            {
                _logger.LogInformation($"Nome: {usuario.Nome}, Email: {usuario.Email}");
            }

            return Ok(usuarios);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter usuários: {ex.Message}");
            return StatusCode(500, "Erro interno ao obter usuários.");
        }
    }
}
