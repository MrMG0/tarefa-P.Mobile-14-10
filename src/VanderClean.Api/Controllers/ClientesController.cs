using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VanderClean.Api.Data;
using VanderClean.Core.DTOs;
using VanderClean.Core.Enums;
using VanderClean.Core.Models;
using VanderClean.Core.Utils;

namespace VanderClean.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ClientesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] ClienteRegistroDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome) || string.IsNullOrWhiteSpace(dto.Email) ||
            string.IsNullOrWhiteSpace(dto.CpfCnpj) || string.IsNullOrWhiteSpace(dto.Senha))
        {
            return BadRequest(new { mensagem = "Nome, e-mail, CPF/CNPJ e senha são obrigatórios." });
        }

        // RN07: Validação do documento conforme tipo de pessoa
        if (dto.TipoPessoa == TipoPessoa.PF)
        {
            if (!DocumentValidator.ValidarCpf(dto.CpfCnpj))
                return BadRequest(new { mensagem = "CPF informado é inválido." });
        }
        else
        {
            if (!DocumentValidator.ValidarCnpj(dto.CpfCnpj))
                return BadRequest(new { mensagem = "CNPJ informado é inválido." });
        }

        var docLimpo = DocumentValidator.SomenteNumeros(dto.CpfCnpj);
        var emailLimpo = dto.Email.Trim().ToLower();

        // RN08: Cadastro único por CPF/CNPJ ou e-mail
        var existeDoc = await _context.Clientes.AnyAsync(c => c.CpfCnpj == dto.CpfCnpj || c.CpfCnpj == docLimpo);
        if (existeDoc)
        {
            return BadRequest(new { mensagem = "Já existe um cliente cadastrado com este documento." });
        }

        var existeEmail = await _context.Clientes.AnyAsync(c => c.Email.ToLower() == emailLimpo);
        if (existeEmail)
        {
            return BadRequest(new { mensagem = "Já existe um cliente cadastrado com este e-mail." });
        }

        var cliente = new Cliente
        {
            Nome = dto.Nome.Trim(),
            Email = emailLimpo,
            Telefone = dto.Telefone?.Trim(),
            TipoPessoa = dto.TipoPessoa,
            CpfCnpj = dto.CpfCnpj.Trim(),
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

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(ObterPorId), new { id = cliente.Id }, cliente);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Senha))
        {
            return BadRequest(new { mensagem = "E-mail e senha são obrigatórios." });
        }

        var emailLimpo = dto.Email.Trim().ToLower();
        var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email.ToLower() == emailLimpo && c.Ativo);

        if (cliente == null || !BCrypt.Net.BCrypt.Verify(dto.Senha, cliente.SenhaHash))
        {
            return Unauthorized(new { mensagem = "E-mail ou senha incorretos." });
        }

        return Ok(new
        {
            mensagem = "Login efetuado com sucesso.",
            cliente = new
            {
                cliente.Id,
                cliente.Nome,
                cliente.Email,
                cliente.Telefone,
                cliente.TipoPessoa,
                cliente.CpfCnpj,
                cliente.Cidade,
                cliente.Uf
            }
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente == null) return NotFound(new { mensagem = "Cliente não encontrado." });
        return Ok(cliente);
    }
}
