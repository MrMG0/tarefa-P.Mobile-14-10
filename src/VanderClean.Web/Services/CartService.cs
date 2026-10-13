using VanderClean.Core.Models;

namespace VanderClean.Web.Services;

public class CartService
{
    public event Action? OnChange;
    private readonly Dictionary<int, (Produto Produto, decimal Quantidade)> _itens = new();

    public IReadOnlyCollection<(Produto Produto, decimal Quantidade)> Itens => _itens.Values;

    public bool Adicionar(Produto produto, decimal quantidade = 1)
    {
        if (quantidade <= 0)
        {
            return false;
        }

        var quantidadeAtual = _itens.TryGetValue(produto.Id, out var atual) ? atual.Quantidade : 0;
        if (quantidadeAtual + quantidade > produto.Estoque)
        {
            return false;
        }

        _itens[produto.Id] = (produto, quantidadeAtual + quantidade);
        NotifyStateChanged();
        return true;
    }

    public bool AtualizarQuantidade(int produtoId, decimal quantidade)
    {
        if (!_itens.TryGetValue(produtoId, out var item) || quantidade < 0)
        {
            return false;
        }

        if (quantidade == 0)
        {
            _itens.Remove(produtoId);
        }
        else
        {
            if (quantidade > item.Produto.Estoque)
            {
                return false;
            }

            _itens[produtoId] = (item.Produto, quantidade);
        }

        NotifyStateChanged();
        return true;
    }

    public void Remover(int produtoId)
    {
        if (_itens.ContainsKey(produtoId))
        {
            _itens.Remove(produtoId);
            NotifyStateChanged();
        }
    }

    public void Limpar()
    {
        _itens.Clear();
        NotifyStateChanged();
    }

    public decimal Total => _itens.Values.Sum(i => i.Produto.Preco * i.Quantidade);
    public int TotalItens => (int)_itens.Values.Sum(i => i.Quantidade);

    private void NotifyStateChanged() => OnChange?.Invoke();
}

public class WebSessionState
{
    public int? ClienteId { get; set; }
    public string? ClienteNome { get; set; }
    public bool IsAutenticado => ClienteId.HasValue;
}
