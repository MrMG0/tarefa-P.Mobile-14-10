using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VanderClean.Api.Data;
using VanderClean.Core.Models;

namespace VanderClean.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProdutosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int? idCategoria, [FromQuery] bool apenasDisponiveis = true)
    {
        var query = _context.Produtos.Include(p => p.Categoria).AsQueryable();

        if (apenasDisponiveis)
        {
            query = query.Where(p => p.Disponivel && p.Ativo);
        }

        if (idCategoria.HasValue && idCategoria.Value > 0)
        {
            query = query.Where(p => p.IdCategoria == idCategoria.Value);
        }

        var produtos = await query.OrderBy(p => p.Nome).ToListAsync();
        return Ok(produtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var produto = await _context.Produtos.Include(p => p.Categoria).FirstOrDefaultAsync(p => p.Id == id);
        if (produto == null) return NotFound(new { mensagem = "Produto não encontrado." });
        return Ok(produto);
    }
}
