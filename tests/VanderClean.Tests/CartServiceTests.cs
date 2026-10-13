using VanderClean.Core.Models;
using VanderClean.Web.Services;

namespace VanderClean.Tests;

public class CartServiceTests
{
    [Fact]
    public void AdicionarSelecionaProdutoEAtualizaTotal()
    {
        var carrinho = new CartService();
        var produto = new Produto { Id = 1, Nome = "Detergente", Preco = 12.50m, Estoque = 5 };

        Assert.True(carrinho.Adicionar(produto));
        Assert.True(carrinho.Adicionar(produto));

        Assert.Equal(2, carrinho.TotalItens);
        Assert.Equal(25m, carrinho.Total);
    }

    [Fact]
    public void AtualizarQuantidadePermiteRemoverProdutoDoCarrinho()
    {
        var carrinho = new CartService();
        var produto = new Produto { Id = 1, Nome = "Detergente", Preco = 12.50m, Estoque = 5 };
        carrinho.Adicionar(produto, 3);

        Assert.True(carrinho.AtualizarQuantidade(produto.Id, 1));
        Assert.Equal(1, carrinho.TotalItens);
        Assert.Equal(12.50m, carrinho.Total);

        Assert.True(carrinho.AtualizarQuantidade(produto.Id, 0));
        Assert.Empty(carrinho.Itens);
        Assert.Equal(0m, carrinho.Total);
    }

    [Fact]
    public void QuantidadeNaoPodeExcederEstoque()
    {
        var carrinho = new CartService();
        var produto = new Produto { Id = 1, Nome = "Detergente", Preco = 12.50m, Estoque = 2 };

        Assert.True(carrinho.Adicionar(produto, 2));
        Assert.False(carrinho.Adicionar(produto));
        Assert.False(carrinho.AtualizarQuantidade(produto.Id, 3));
        Assert.Equal(2, carrinho.TotalItens);
    }
}
