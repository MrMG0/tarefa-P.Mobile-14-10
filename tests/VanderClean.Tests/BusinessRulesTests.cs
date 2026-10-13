using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VanderClean.Api.Controllers;
using VanderClean.Api.Data;
using VanderClean.Core.DTOs;
using VanderClean.Core.Enums;
using VanderClean.Core.Models;
using Xunit;

namespace VanderClean.Tests;

public class BusinessRulesTests
{
    private AppDbContext ObterDbContextEmMemoria(string nomeDb)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: nomeDb)
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task RN05_RepetirPedidoEstoque_DeveFalhar_SeProdutoEstiverIndisponivel()
    {
        // Arrange
        using var context = ObterDbContextEmMemoria("DB_Teste_RN05");
        
        var empresa = new Empresa { Id = 1, RazaoSocial = "VanderClean", Cnpj = "11222333000181", Email = "a@a.com", SenhaHash = "hash" };
        var produto = new Produto { Id = 10, Nome = "Detergente 5L", Preco = 20, Disponivel = true, Ativo = true };
        
        context.Empresas.Add(empresa);
        context.Produtos.Add(produto);
        
        var pedido = new PedidoEstoque
        {
            Id = 100,
            IdEmpresa = 1,
            Itens = new List<ItemPedidoEstoque>
            {
                new ItemPedidoEstoque { IdProduto = 10, Quantidade = 2, PrecoUnitario = 20, Subtotal = 40 }
            }
        };
        context.PedidosEstoque.Add(pedido);
        await context.SaveChangesAsync();

        var controller = new PedidosEstoqueController(context);

        // Produto fica indisponível antes da repetição
        produto.Disponivel = false;
        await context.SaveChangesAsync();

        // Act
        var result = await controller.RepetirPedido(100);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
    }

    [Fact]
    public async Task RN11_CriarEncomenda_DeveFalhar_SeEstoqueForInsuficiente()
    {
        // Arrange
        using var context = ObterDbContextEmMemoria("DB_Teste_RN11");
        
        var cliente = new Cliente { Id = 1, Nome = "Cliente Teste", CpfCnpj = "11144477705", Email = "c@c.com", SenhaHash = "hash" };
        var produto = new Produto { Id = 20, Nome = "Desengordurante", Preco = 30, Estoque = 3, Disponivel = true, Ativo = true };
        
        context.Clientes.Add(cliente);
        context.Produtos.Add(produto);
        await context.SaveChangesAsync();

        var controller = new EncomendasController(context);

        var dto = new CriarEncomendaDto
        {
            IdCliente = 1,
            Itens = new List<ItemPedidoCriacaoDto>
            {
                new ItemPedidoCriacaoDto { IdProduto = 20, Quantidade = 10 } // Pediu 10, mas só tem 3 no estoque
            }
        };

        // Act
        var result = await controller.CriarEncomenda(dto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
    }

    [Fact]
    public async Task RN14_AtualizarStatus_NaoDevePermitirCancelarPedidoEnviado()
    {
        // Arrange
        using var context = ObterDbContextEmMemoria("DB_Teste_RN14");
        
        var pedido = new PedidoEstoque
        {
            Id = 200,
            IdEmpresa = 1,
            Status = StatusPedido.ENVIADO
        };
        context.PedidosEstoque.Add(pedido);
        await context.SaveChangesAsync();

        var controller = new PedidosEstoqueController(context);

        // Act: tenta cancelar pedido que já foi enviado
        var result = await controller.AtualizarStatus(200, new AtualizarStatusDto { NovoStatus = StatusPedido.CANCELADO });

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
    }

    [Fact]
    public async Task RN15_CriarPedidoEstoque_DeveCalcularValorTotalCorretamente()
    {
        // Arrange
        using var context = ObterDbContextEmMemoria("DB_Teste_Calculo");
        
        var empresa = new Empresa { Id = 1, RazaoSocial = "VanderClean", Cnpj = "11222333000181", Email = "a@a.com", SenhaHash = "hash" };
        var p1 = new Produto { Id = 1, Nome = "Produto 1", Preco = 10.0m, Disponivel = true, Ativo = true };
        var p2 = new Produto { Id = 2, Nome = "Produto 2", Preco = 25.5m, Disponivel = true, Ativo = true };
        
        context.Empresas.Add(empresa);
        context.Produtos.AddRange(p1, p2);
        await context.SaveChangesAsync();

        var controller = new PedidosEstoqueController(context);

        var dto = new CriarPedidoEstoqueDto
        {
            IdEmpresa = 1,
            Itens = new List<ItemPedidoCriacaoDto>
            {
                new ItemPedidoCriacaoDto { IdProduto = 1, Quantidade = 3 },  // 3 * 10.0 = 30.0
                new ItemPedidoCriacaoDto { IdProduto = 2, Quantidade = 2 }   // 2 * 25.5 = 51.0
            }
        };

        // Act
        var result = await controller.CriarPedido(dto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var pedidoCriado = Assert.IsType<PedidoEstoque>(createdResult.Value);
        Assert.Equal(81.0m, pedidoCriado.ValorTotal);
        Assert.Equal(StatusPedido.PENDENTE, pedidoCriado.Status);
        Assert.Equal(3, pedidoCriado.PrazoEntregaDias);
    }
}
