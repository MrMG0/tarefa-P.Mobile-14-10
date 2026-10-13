using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VanderClean.Api.Data;
using VanderClean.Core.DTOs;
using VanderClean.Core.Enums;
using VanderClean.Core.Models;

namespace VanderClean.Api.Controllers;

[ApiController]
[Route("api/pedidos-estoque")]
public class PedidosEstoqueController : ControllerBase
{
    private readonly AppDbContext _context;

    public PedidosEstoqueController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("empresa/{idEmpresa}")]
    public async Task<IActionResult> ListarPorEmpresa(int idEmpresa)
    {
        var pedidos = await _context.PedidosEstoque
            .Where(p => p.IdEmpresa == idEmpresa)
            .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
            .OrderByDescending(p => p.DataPedido)
            .ToListAsync();

        return Ok(pedidos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var pedido = await _context.PedidosEstoque
            .Include(p => p.Empresa)
            .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pedido == null)
        {
            return NotFound(new { mensagem = "Pedido de estoque não encontrado." });
        }

        return Ok(pedido);
    }

    [HttpPost]
    public async Task<IActionResult> CriarPedido([FromBody] CriarPedidoEstoqueDto dto)
    {
        if (dto.IdEmpresa <= 0 || dto.Itens == null || !dto.Itens.Any())
        {
            return BadRequest(new { mensagem = "O pedido deve ter uma empresa válida e ao menos um produto." });
        }

        var empresa = await _context.Empresas.FindAsync(dto.IdEmpresa);
        if (empresa == null)
        {
            return BadRequest(new { mensagem = "Empresa não encontrada." });
        }

        var idProdutos = dto.Itens.Select(i => i.IdProduto).Distinct().ToList();
        var produtos = await _context.Produtos
            .Where(p => idProdutos.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        // Validação de produtos e disponibilidade
        foreach (var itemDto in dto.Itens)
        {
            if (itemDto.Quantidade <= 0)
            {
                return BadRequest(new { mensagem = "Quantidade do item deve ser maior que zero." });
            }

            if (!produtos.TryGetValue(itemDto.IdProduto, out var prod))
            {
                return BadRequest(new { mensagem = $"Produto ID {itemDto.IdProduto} não encontrado." });
            }

            if (!prod.Disponivel || !prod.Ativo)
            {
                return BadRequest(new { mensagem = $"Produto '{prod.Nome}' não está disponível para reposição." });
            }
        }

        var pedido = new PedidoEstoque
        {
            IdEmpresa = dto.IdEmpresa,
            DataPedido = DateTime.UtcNow,
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

            pedido.Itens.Add(new ItemPedidoEstoque
            {
                IdProduto = prod.Id,
                Quantidade = itemDto.Quantidade,
                PrecoUnitario = prod.Preco,
                Subtotal = subtotal
            });
        }

        pedido.ValorTotal = total;

        _context.PedidosEstoque.Add(pedido);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(ObterPorId), new { id = pedido.Id }, pedido);
    }

    [HttpPost("{id}/repetir")]
    public async Task<IActionResult> RepetirPedido(int id)
    {
        var pedidoAnterior = await _context.PedidosEstoque
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pedidoAnterior == null)
        {
            return NotFound(new { mensagem = "Pedido original não encontrado." });
        }

        var idProdutos = pedidoAnterior.Itens.Select(i => i.IdProduto).ToList();
        var produtos = await _context.Produtos
            .Where(p => idProdutos.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        // Conforme RN05: verifica se todos os produtos ainda estão disponíveis
        foreach (var item in pedidoAnterior.Itens)
        {
            if (!produtos.TryGetValue(item.IdProduto, out var prod) || !prod.Disponivel || !prod.Ativo)
            {
                    var nomeProd = prod != null ? prod.Nome : $"ID {item.IdProduto}";
                    return BadRequest(new
                    {
                        mensagem = $"Não foi possível repetir o pedido: o produto '{nomeProd}' não está mais disponível."
                    });
                }
            }

        var novoPedido = new PedidoEstoque
        {
            IdEmpresa = pedidoAnterior.IdEmpresa,
            DataPedido = DateTime.UtcNow,
            Status = StatusPedido.PENDENTE,
            PrazoEntregaDias = 3,
            DataEntregaPrev = DateTime.UtcNow.AddDays(3),
            Observacao = $"Repetição do Pedido #{pedidoAnterior.Id}",
            ValorTotal = 0
        };

        decimal total = 0;
        foreach (var item in pedidoAnterior.Itens)
        {
            var prod = produtos[item.IdProduto];
            var subtotal = item.Quantidade * prod.Preco; // Atualiza com preço atual do fornecedor
            total += subtotal;

            novoPedido.Itens.Add(new ItemPedidoEstoque
            {
                IdProduto = prod.Id,
                Quantidade = item.Quantidade,
                PrecoUnitario = prod.Preco,
                Subtotal = subtotal
            });
        }

        novoPedido.ValorTotal = total;

        _context.PedidosEstoque.Add(novoPedido);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(ObterPorId), new { id = novoPedido.Id }, novoPedido);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> AtualizarStatus(int id, [FromBody] AtualizarStatusDto dto)
    {
        var pedido = await _context.PedidosEstoque.FindAsync(id);
        if (pedido == null)
        {
            return NotFound(new { mensagem = "Pedido de estoque não encontrado." });
        }

        // RN14: Validação de fluxo de status
        if (dto.NovoStatus == StatusPedido.CANCELADO && 
            pedido.Status is not (StatusPedido.PENDENTE or StatusPedido.CONFIRMADO or StatusPedido.EM_SEPARACAO))
        {
            return BadRequest(new { mensagem = "Só é possível cancelar pedidos pendentes, confirmados ou em separação." });
        }

        if (dto.NovoStatus == StatusPedido.DEVOLVIDO && pedido.Status != StatusPedido.ENTREGUE)
        {
            return BadRequest(new { mensagem = "Devolução só pode ser aplicada após o pedido ter sido entregue." });
        }

        pedido.Status = dto.NovoStatus;
        await _context.SaveChangesAsync();

        return Ok(new { mensagem = "Status atualizado com sucesso.", status = pedido.Status });
    }
}
