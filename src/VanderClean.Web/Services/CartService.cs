using VanderClean.Core.Models;

namespace VanderClean.Web.Services;

public class CartService
{
    public event Action? OnChange;
    private readonly Dictionary<int, (Produto Produto, decimal Quantidade)> _itens = new();

    public IReadOnlyCollection<(Produto Produto, decimal Quantidade)> Itens => _itens.Values;

    public void Adicionar(Produto produto, decimal quantidade = 1)
    {
        if (_itens.ContainsKey(produto.Id))
        {
            var atual = _itens[produto.Id];
            _itens[produto.Id] = (produto, atual.Quantidade + quantidade);
        }
        else
        {
            _itens[produto.Id] = (produto, quantidade);
        }
        NotifyStateChanged();
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
