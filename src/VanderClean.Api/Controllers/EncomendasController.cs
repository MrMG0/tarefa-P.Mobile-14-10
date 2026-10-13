using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VanderClean.Api.Data;
using VanderClean.Core.DTOs;
using VanderClean.Core.Enums;
using VanderClean.Core.Models;

namespace VanderClean.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EncomendasController : ControllerBase
{
    private readonly AppDbContext _context;

    public EncomendasController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("cliente/{idCliente}")]
    public async Task<IActionResult> ListarPorCliente(int idCliente)
    {
        var encomendas = await _context.Encomendas
            .Where(e => e.IdCliente == idCliente)
            .Include(e => e.Itens)
                .ThenInclude(i => i.Produto)
            .OrderByDescending(e => e.DataEncomenda)
            .ToListAsync();

        return Ok(encomendas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var encomenda = await _context.Encomendas
            .Include(e => e.Cliente)
            .Include(e => e.Itens)
                .ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (encomenda == null)
        {
            return NotFound(new { mensagem = "Encomenda não encontrada." });
        }

        return Ok(encomenda);
    }

    [HttpPost]
    public async Task<IActionResult> CriarEncomenda([FromBody] CriarEncomendaDto dto)
    {
        if (dto.IdCliente <= 0 || dto.Itens == null || !dto.Itens.Any())
        {
            return BadRequest(new { mensagem = "A encomenda deve ter um cliente válido e ao menos um produto." });
        }

        var cliente = await _context.Clientes.FindAsync(dto.IdCliente);
        if (cliente == null)
        {
            return BadRequest(new { mensagem = "Cliente não encontrado." });
        }

        var idProdutos = dto.Itens.Select(i => i.IdProduto).Distinct().ToList();
        var produtos = await _context.Produtos
            .Where(p => idProdutos.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        // RN10 e RN11: validação de disponibilidade e estoque para cliente final
        foreach (var itemDto in dto.Itens)
        {
            if (itemDto.Quantidade <= 0)
            {
                return BadRequest(new { mensagem = "Quantidade deve ser maior que zero." });
            }

            if (!produtos.TryGetValue(itemDto.IdProduto, out var prod))
            {
                return BadRequest(new { mensagem = $"Produto ID {itemDto.IdProduto} não encontrado." });
            }

            if (!prod.Disponivel || !prod.Ativo)
            {
                return BadRequest(new { mensagem = $"Produto '{prod.Nome}' não está disponível para encomenda." });
            }

            if (prod.Estoque < itemDto.Quantidade)
            {
                return BadRequest(new
                {
                    mensagem = $"Estoque insuficiente para '{prod.Nome}'. Disponível: {prod.Estoque}, solicitado: {itemDto.Quantidade}."
                });
            }
        }

        var encomenda = new Encomenda
        {
            IdCliente = dto.IdCliente,
            DataEncomenda = DateTime.UtcNow,
            Status = StatusPedido.PENDENTE,
            PrazoEntregaDias = 3,
            DataEntregaPrev = DateTime.UtcNow.AddDays(3),
            Observacao = dto.Observacao,
            ValorTotal = 0
        };

        decimal total = 0;
        foreach (var itemDto in dto.Itens)
        {
            var prod = produtos[itemDto.IdProduto];
            var subtotal = itemDto.Quantidade * prod.Preco;
            total += subtotal;

            // Abate do estoque do produto
            prod.Estoque -= (int)itemDto.Quantidade;

            encomenda.Itens.Add(new ItemEncomenda
            {
                IdProduto = prod.Id,
                Quantidade = itemDto.Quantidade,
                PrecoUnitario = prod.Preco,
                Subtotal = subtotal
            });
        }

        encomenda.ValorTotal = total;

        _context.Encomendas.Add(encomenda);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(ObterPorId), new { id = encomenda.Id }, encomenda);
    }

    [HttpPost("{id}/repetir")]
    public async Task<IActionResult> RepetirEncomenda(int id)
    {
        var encomendaAnterior = await _context.Encomendas
            .Include(e => e.Itens)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (encomendaAnterior == null)
        {
            return NotFound(new { mensagem = "Encomenda original não encontrada." });
        }

        var idProdutos = encomendaAnterior.Itens.Select(i => i.IdProduto).ToList();
        var produtos = await _context.Produtos
            .Where(p => idProdutos.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        // RN12: Verifica disponibilidade e estoque
        foreach (var item in encomendaAnterior.Itens)
        {
            if (!produtos.TryGetValue(item.IdProduto, out var prod) || !prod.Disponivel || !prod.Ativo)
            {
                var nomeProd = prod != null ? prod.Nome : $"ID {item.IdProduto}";
                return BadRequest(new
                {
                    mensagem = $"Não foi possível repetir a encomenda: o produto '{nomeProd}' não está mais disponível."
                });
            }

            if (prod.Estoque < item.Quantidade)
            {
                return BadRequest(new
                {
                    mensagem = $"Estoque insuficiente para repetir a encomenda do produto '{prod.Nome}'."
                });
            }
        }

        var novaEncomenda = new Encomenda
        {
            IdCliente = encomendaAnterior.IdCliente,
            DataEncomenda = DateTime.UtcNow,
            Status = StatusPedido.PENDENTE,
            PrazoEntregaDias = 3,
            DataEntregaPrev = DateTime.UtcNow.AddDays(3),
            Observacao = $"Repetição da Encomenda #{encomendaAnterior.Id}",
            ValorTotal = 0
        };

        decimal total = 0;
        foreach (var item in encomendaAnterior.Itens)
        {
            var prod = produtos[item.IdProduto];
            var subtotal = item.Quantidade * prod.Preco;
            total += subtotal;

            prod.Estoque -= (int)item.Quantidade;

            novaEncomenda.Itens.Add(new ItemEncomenda
            {
                IdProduto = prod.Id,
                Quantidade = item.Quantidade,
                PrecoUnitario = prod.Preco,
                Subtotal = subtotal
            });
        }

        novaEncomenda.ValorTotal = total;

        _context.Encomendas.Add(novaEncomenda);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(ObterPorId), new { id = novaEncomenda.Id }, novaEncomenda);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> AtualizarStatus(int id, [FromBody] AtualizarStatusDto dto)
    {
        var encomenda = await _context.Encomendas
            .Include(e => e.Itens)
                .ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (encomenda == null)
        {
            return NotFound(new { mensagem = "Encomenda não encontrada." });
        }

        if (dto.NovoStatus == StatusPedido.CANCELADO &&
            encomenda.Status is not (StatusPedido.PENDENTE or StatusPedido.CONFIRMADO or StatusPedido.EM_SEPARACAO))
        {
            return BadRequest(new { mensagem = "Só é possível cancelar encomendas pendentes, confirmadas ou em separação." });
        }

        if (dto.NovoStatus == StatusPedido.DEVOLVIDO && encomenda.Status != StatusPedido.ENTREGUE)
        {
            return BadRequest(new { mensagem = "Devolução só pode ser aplicada após a encomenda ter sido entregue." });
        }

        if (dto.NovoStatus == StatusPedido.CANCELADO)
        {
            foreach (var item in encomenda.Itens)
            {
                if (item.Produto is not null)
                {
                    item.Produto.Estoque += (int)item.Quantidade;
                }
            }
        }

        encomenda.Status = dto.NovoStatus;
        await _context.SaveChangesAsync();

        return Ok(new { mensagem = "Status da encomenda atualizado com sucesso.", status = encomenda.Status });
    }
}
