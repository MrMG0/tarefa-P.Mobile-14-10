using System.Collections.ObjectModel;
using System.ComponentModel;
using VanderClean.App.Models;
using VanderClean.App.Services;
using VanderClean.Core.DTOs;
using VanderClean.Core.Models;

namespace VanderClean.App.Views;

public class ItemSelecaoViewModel : INotifyPropertyChanged
{
    public Produto Produto { get; set; } = new();

    private decimal _quantidade = 0;
    public decimal Quantidade
    {
        get => _quantidade;
        set
        {
            if (_quantidade != value)
            {
                _quantidade = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Quantidade)));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}

public partial class NovoPedidoPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly LocalDbService _localDbService;
    private ObservableCollection<ItemSelecaoViewModel> _itens = new();

    public NovoPedidoPage()
    {
        InitializeComponent();
        _apiService = new ApiService();
        _localDbService = new LocalDbService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarCategoriasAsync();
        await CarregarProdutosAsync();
    }

    private async Task CarregarCategoriasAsync()
    {
        var categorias = await _apiService.ObterCategoriasAsync();
        CategoriaPicker.ItemsSource = categorias;
    }

    private async Task CarregarProdutosAsync(int? idCategoria = null)
    {
        var produtos = await _apiService.ObterProdutosAsync(idCategoria);
        _itens = new ObservableCollection<ItemSelecaoViewModel>(
            produtos.Select(p => new ItemSelecaoViewModel { Produto = p, Quantidade = 0 })
        );
        ProdutosCollectionView.ItemsSource = _itens;
        AtualizarTotal();
    }

    private async void OnCategoriaChanged(object sender, EventArgs e)
    {
        if (CategoriaPicker.SelectedItem is Categoria cat)
        {
            await CarregarProdutosAsync(cat.Id);
        }
        else
        {
            await CarregarProdutosAsync();
        }
    }

    private void OnAumentarQtdClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is ItemSelecaoViewModel item)
        {
            item.Quantidade += 1;
            AtualizarTotal();
        }
    }

    private void OnDiminuirQtdClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is ItemSelecaoViewModel item)
        {
            if (item.Quantidade > 0)
            {
                item.Quantidade -= 1;
                AtualizarTotal();
            }
        }
    }

    private void AtualizarTotal()
    {
        decimal total = _itens.Sum(i => i.Quantidade * i.Produto.Preco);
        TotalLabel.Text = $"R$ {total:F2}";
    }

    private async void OnConfirmarPedidoClicked(object sender, EventArgs e)
    {
        var selecionados = _itens.Where(i => i.Quantidade > 0).ToList();
        if (!selecionados.Any())
        {
            await DisplayAlert("Atenção", "Selecione ao menos um produto com quantidade maior que zero.", "OK");
            return;
        }

        BtnConfirmar.IsEnabled = false;

        var dto = new CriarPedidoEstoqueDto
        {
            IdEmpresa = SessionManager.EmpresaId,
            Observacao = "Pedido de reposição via ShopVander App",
            Itens = selecionados.Select(i => new ItemPedidoCriacaoDto
            {
                IdProduto = i.Produto.Id,
                Quantidade = i.Quantidade
            }).ToList()
        };

        // Salva pedido localmente no SQLite primeiro
        var total = selecionados.Sum(i => i.Quantidade * i.Produto.Preco);
        var pedidoLocal = new PedidoEstoqueLocal
        {
            DataPedido = DateTime.UtcNow.ToString("o"),
            Status = "PENDENTE",
            PrazoEntregaDias = 3,
            Observacao = dto.Observacao,
            ValorTotal = total,
            SincronizadoApi = 0
        };

        var itensLocais = selecionados.Select(i => new ItemPedidoEstoqueLocal
        {
            IdProdutoApi = i.Produto.Id,
            NomeProduto = i.Produto.Nome,
            Quantidade = i.Quantidade,
            PrecoUnitario = i.Produto.Preco,
            Subtotal = i.Quantidade * i.Produto.Preco
        }).ToList();

        var localId = await _localDbService.SalvarPedidoAsync(pedidoLocal, itensLocais);

        // Envia para a API central
        var resultado = await _apiService.CriarPedidoEstoqueAsync(dto);

        if (resultado.Sucesso && resultado.Pedido != null)
        {
            await _localDbService.MarcarPedidoSincronizadoAsync(localId, resultado.Pedido.Id);
            await DisplayAlert("Pedido Realizado!", $"Pedido #{resultado.Pedido.Id} enviado com sucesso! Prazo: 3 dias úteis.", "OK");
        }
        else
        {
            await DisplayAlert("Gravado Offline", "Pedido gravado localmente no dispositivo. Será sincronizado quando houver conexão com o servidor.", "OK");
        }

        BtnConfirmar.IsEnabled = true;
        await Navigation.PopAsync(); // Volta para a tela inicial de histórico
    }
}
