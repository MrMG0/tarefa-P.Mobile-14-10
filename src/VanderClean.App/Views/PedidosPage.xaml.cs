using System.Collections.ObjectModel;
using VanderClean.App.Models;
using VanderClean.App.Services;
using VanderClean.Core.Enums;
using VanderClean.Core.Models;

namespace VanderClean.App.Views;

public class PedidoItemViewModel
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public string DataFormatada { get; set; } = string.Empty;
    public string TotalFormatado { get; set; } = string.Empty;
    public string Observacao { get; set; } = string.Empty;
    public int? IdPedidoOriginal { get; set; }
    public bool PodeCancelar { get; set; }
}

public partial class PedidosPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly LocalDbService _localDbService;
    private ObservableCollection<PedidoItemViewModel> _pedidosView = new();

    public PedidosPage()
    {
        InitializeComponent();
        _apiService = new ApiService();
        _localDbService = new LocalDbService();
        PedidosCollectionView.ItemsSource = _pedidosView;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarPedidosAsync();
    }

    private async Task CarregarPedidosAsync()
    {
        ListaRefreshView.IsRefreshing = true;

        try
        {
            var itensFormatados = new List<PedidoItemViewModel>();

            // Tenta obter da API
            var pedidosApi = await _apiService.ObterPedidosEmpresaAsync(SessionManager.EmpresaId);

            if (pedidosApi != null && pedidosApi.Any())
            {
                foreach (var p in pedidosApi)
                {
                    itensFormatados.Add(new PedidoItemViewModel
                    {
                        Id = p.Id,
                        IdPedidoOriginal = p.Id,
                        Status = p.Status.ToString(),
                        DataFormatada = $"Data: {p.DataPedido:dd/MM/yyyy HH:mm}",
                        TotalFormatado = $"Total: R$ {p.ValorTotal:F2}",
                        Observacao = p.Observacao ?? string.Empty,
                        PodeCancelar = PodeCancelar(p.Status)
                    });

                    // Salva/sincroniza cópia no banco local SQLite para modo offline
                    var localPedido = new PedidoEstoqueLocal
                    {
                        IdPedidoApi = p.Id,
                        DataPedido = p.DataPedido.ToString("o"),
                        Status = p.Status.ToString(),
                        PrazoEntregaDias = p.PrazoEntregaDias,
                        Observacao = p.Observacao,
                        ValorTotal = p.ValorTotal,
                        SincronizadoApi = 1
                    };

                    var itensLocais = p.Itens.Select(i => new ItemPedidoEstoqueLocal
                    {
                        IdProdutoApi = i.IdProduto,
                        NomeProduto = i.Produto?.Nome ?? "Produto",
                        Quantidade = i.Quantidade,
                        PrecoUnitario = i.PrecoUnitario,
                        Subtotal = i.Subtotal
                    }).ToList();

                    await _localDbService.SalvarPedidoAsync(localPedido, itensLocais);
                }
            }
            else
            {
                // Fallback para SQLite local
                var pedidosLocais = await _localDbService.ObterPedidosAsync();
                foreach (var p in pedidosLocais)
                {
                    DateTime.TryParse(p.DataPedido, out var dt);
                    itensFormatados.Add(new PedidoItemViewModel
                    {
                        Id = p.IdPedidoApi ?? p.Id,
                        IdPedidoOriginal = p.IdPedidoApi ?? p.Id,
                        Status = p.Status,
                        DataFormatada = dt != default ? $"Data: {dt:dd/MM/yyyy HH:mm}" : $"Data: {p.DataPedido}",
                        TotalFormatado = $"Total: R$ {p.ValorTotal:F2}",
                        Observacao = p.Observacao ?? string.Empty,
                        PodeCancelar = false
                    });
                }
            }

            _pedidosView.Clear();
            foreach (var item in itensFormatados)
            {
                _pedidosView.Add(item);
            }
        }
        catch (Exception)
        {
            // Em caso de falha de conexão, consulta SQLite local
            try
            {
                var pedidosLocais = await _localDbService.ObterPedidosAsync();
                _pedidosView.Clear();
                foreach (var p in pedidosLocais)
                {
                    DateTime.TryParse(p.DataPedido, out var dt);
                    _pedidosView.Add(new PedidoItemViewModel
                    {
                        Id = p.IdPedidoApi ?? p.Id,
                        IdPedidoOriginal = p.IdPedidoApi ?? p.Id,
                        Status = p.Status,
                        DataFormatada = dt != default ? $"Data: {dt:dd/MM/yyyy HH:mm}" : $"Data: {p.DataPedido}",
                        TotalFormatado = $"Total: R$ {p.ValorTotal:F2}",
                        Observacao = p.Observacao ?? string.Empty,
                        PodeCancelar = false
                    });
                }
            }
            catch
            {
                // Tratamento seguro para nunca derrubar a tela
            }
        }
        finally
        {
            ListaRefreshView.IsRefreshing = false;
        }
    }

    private async void OnNovoPedidoClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(NovoPedidoPage));
    }

    private async void OnAtualizarClicked(object sender, EventArgs e)
    {
        await CarregarPedidosAsync();
    }

    private async void OnRefreshRequested(object sender, EventArgs e)
    {
        await CarregarPedidosAsync();
    }

    private async void OnRepetirPedidoClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is PedidoItemViewModel pvm)
        {
            int pedidoId = pvm.IdPedidoOriginal ?? pvm.Id;
            if (pedidoId <= 0) return;

            bool confirmar = await DisplayAlert("Repetir Pedido", $"Deseja repetir o pedido #{pedidoId} agora?", "Sim, Repetir", "Cancelar");
            if (!confirmar) return;

            var resultado = await _apiService.RepetirPedidoEstoqueAsync(pedidoId);
            if (resultado.Sucesso && resultado.Pedido != null)
            {
                await DisplayAlert("Sucesso", $"Novo pedido #{resultado.Pedido.Id} gerado com sucesso!", "OK");
                await CarregarPedidosAsync();
            }
            else
            {
                await DisplayAlert("Não foi possível repetir", resultado.Mensagem, "OK");
            }
        }
    }

    private static bool PodeCancelar(StatusPedido status) =>
        status is StatusPedido.PENDENTE or StatusPedido.CONFIRMADO or StatusPedido.EM_SEPARACAO;

    private async void OnCancelarPedidoClicked(object sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: PedidoItemViewModel pedido })
        {
            return;
        }

        var confirmar = await DisplayAlert(
            "Cancelar pedido",
            $"Deseja realmente cancelar o pedido #{pedido.Id}?",
            "Sim, cancelar",
            "Manter pedido");
        if (!confirmar)
        {
            return;
        }

        var resultado = await _apiService.CancelarPedidoEstoqueAsync(pedido.IdPedidoOriginal ?? pedido.Id);
        if (resultado.Sucesso)
        {
            await DisplayAlert("Pedido cancelado", resultado.Mensagem, "OK");
            await CarregarPedidosAsync();
        }
        else
        {
            await DisplayAlert("Não foi possível cancelar", resultado.Mensagem, "OK");
        }
    }
}
