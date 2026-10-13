using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VanderClean.Api.Data;
using VanderClean.Core.DTOs;
using VanderClean.Core.Models;
using VanderClean.Core.Utils;

namespace VanderClean.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmpresaController : ControllerBase
{
    private readonly AppDbContext _context;

    public EmpresaController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] EmpresaRegistroDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.RazaoSocial) || string.IsNullOrWhiteSpace(dto.Cnpj) || 
            string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Senha))
        {
            return BadRequest(new { mensagem = "Razão social, CNPJ, e-mail e senha são obrigatórios." });
        }

        if (!DocumentValidator.ValidarCnpj(dto.Cnpj))
        {
            return BadRequest(new { mensagem = "CNPJ informado é inválido." });
        }

        var cnpjLimpo = DocumentValidator.SomenteNumeros(dto.Cnpj);
        var emailLimpo = dto.Email.Trim().ToLower();

        var existeCnpj = await _context.Empresas.AnyAsync(e => e.Cnpj == dto.Cnpj || e.Cnpj == cnpjLimpo);
        if (existeCnpj)
        {
            return BadRequest(new { mensagem = "Já existe uma empresa cadastrada com este CNPJ." });
        }

        var existeEmail = await _context.Empresas.AnyAsync(e => e.Email.ToLower() == emailLimpo);
        if (existeEmail)
        {
            return BadRequest(new { mensagem = "Já existe uma empresa cadastrada com este e-mail." });
        }

        var empresa = new Empresa
        {
            RazaoSocial = dto.RazaoSocial.Trim(),
            NomeFantasia = dto.NomeFantasia?.Trim(),
            Cnpj = dto.Cnpj.Trim(),
            Email = emailLimpo,
            Telefone = dto.Telefone?.Trim(),
            Cep = dto.Cep?.Trim(),
            Rua = dto.Rua?.Trim(),
            Numero = dto.Numero?.Trim(),
            Bairro = dto.Bairro?.Trim(),
            Cidade = dto.Cidade?.Trim(),
            Uf = dto.Uf?.Trim().ToUpper(),
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
            DataCadastro = DateTime.UtcNow,
            Ativo = true
        };

        _context.Empresas.Add(empresa);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(ObterPorId), new { id = empresa.Id }, empresa);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Senha))
        {
            return BadRequest(new { mensagem = "E-mail e senha são obrigatórios." });
        }

        var emailLimpo = dto.Email.Trim().ToLower();
        var empresa = await _context.Empresas.FirstOrDefaultAsync(e => e.Email.ToLower() == emailLimpo && e.Ativo);

        if (empresa == null || !BCrypt.Net.BCrypt.Verify(dto.Senha, empresa.SenhaHash))
        {
            return Unauthorized(new { mensagem = "E-mail ou senha incorretos." });
        }

        return Ok(new
        {
            mensagem = "Login efetuado com sucesso.",
            empresa = new
            {
                empresa.Id,
                empresa.RazaoSocial,
                empresa.NomeFantasia,
                empresa.Cnpj,
                empresa.Email,
                empresa.Telefone,
                empresa.Cidade,
                empresa.Uf
            }
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var empresa = await _context.Empresas.FindAsync(id);
        if (empresa == null) return NotFound(new { mensagem = "Empresa não encontrada." });
        return Ok(empresa);
    }
}
