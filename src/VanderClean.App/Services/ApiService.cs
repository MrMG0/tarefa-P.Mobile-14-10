using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using VanderClean.Core.DTOs;
using VanderClean.Core.Enums;
using VanderClean.Core.Models;

namespace VanderClean.App.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions ApiJsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    // No Android Emulator usa 10.0.2.2; no Windows desktop usa localhost
    public static string BaseUrl => DeviceInfo.Platform == DevicePlatform.Android 
        ? "http://10.0.2.2:5000" 
        : "http://localhost:5000";

    public ApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl),
            Timeout = TimeSpan.FromSeconds(10)
        };
    }

    public async Task<(bool Sucesso, string Mensagem, int? EmpresaId)> LoginAsync(string email, string senha)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/empresa/login", new LoginDto
            {
                Email = email,
                Senha = senha
            });

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(content);
                var id = doc.RootElement.GetProperty("empresa").GetProperty("id").GetInt32();
                return (true, "Login realizado com sucesso", id);
            }
            else
            {
                var err = await response.Content.ReadAsStringAsync();
                return (false, "E-mail ou senha inválidos.", null);
            }
        }
        catch (Exception ex)
        {
            return (false, $"Erro de conexão com o servidor: {ex.Message}", null);
        }
    }

    public async Task<(bool Sucesso, string Mensagem, Empresa? Empresa)> RegistrarEmpresaAsync(EmpresaRegistroDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/empresa/registrar", dto);
            if (response.IsSuccessStatusCode)
            {
                var emp = await response.Content.ReadFromJsonAsync<Empresa>(ApiJsonOptions);
                return (true, "Empresa registrada com sucesso!", emp);
            }
            else
            {
                var err = await response.Content.ReadAsStringAsync();
                return (false, err, null);
            }
        }
        catch (Exception ex)
        {
            return (false, $"Erro ao conectar com a API: {ex.Message}", null);
        }
    }

    public async Task<List<Produto>> ObterProdutosAsync(int? idCategoria = null)
    {
        var url = idCategoria.HasValue ? $"/api/produtos?idCategoria={idCategoria.Value}" : "/api/produtos";
        var produtos = await _httpClient.GetFromJsonAsync<List<Produto>>(url, ApiJsonOptions);
        return produtos ?? throw new InvalidDataException("A API retornou uma lista de produtos vazia ou inválida.");
    }

    public async Task<List<Categoria>> ObterCategoriasAsync()
    {
        var categorias = await _httpClient.GetFromJsonAsync<List<Categoria>>("/api/categorias", ApiJsonOptions);
        return categorias ?? throw new InvalidDataException("A API retornou uma lista de categorias vazia ou inválida.");
    }

    public async Task<List<PedidoEstoque>> ObterPedidosEmpresaAsync(int idEmpresa)
    {
        try
        {
            var pedidos = await _httpClient.GetFromJsonAsync<List<PedidoEstoque>>(
                $"/api/pedidos-estoque/empresa/{idEmpresa}", ApiJsonOptions);
            return pedidos ?? new List<PedidoEstoque>();
        }
        catch
        {
            return new List<PedidoEstoque>();
        }
    }

    public async Task<(bool Sucesso, string Mensagem, PedidoEstoque? Pedido)> CriarPedidoEstoqueAsync(CriarPedidoEstoqueDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/pedidos-estoque", dto);
            if (response.IsSuccessStatusCode)
            {
                var pedido = await response.Content.ReadFromJsonAsync<PedidoEstoque>(ApiJsonOptions);
                return (true, "Pedido criado com sucesso!", pedido);
            }
            else
            {
                var err = await response.Content.ReadAsStringAsync();
                return (false, err, null);
            }
        }
        catch (Exception ex)
        {
            return (false, $"Erro de conexão: {ex.Message}", null);
        }
    }

    public async Task<(bool Sucesso, string Mensagem, PedidoEstoque? Pedido)> RepetirPedidoEstoqueAsync(int idPedido)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/pedidos-estoque/{idPedido}/repetir", null);
            if (response.IsSuccessStatusCode)
            {
                var pedido = await response.Content.ReadFromJsonAsync<PedidoEstoque>(ApiJsonOptions);
                return (true, "Pedido repetido com sucesso!", pedido);
            }
            else
            {
                var err = await response.Content.ReadAsStringAsync();
                return (false, err, null);
            }
        }
        catch (Exception ex)
        {
            return (false, $"Erro de conexão: {ex.Message}", null);
        }
    }

    public async Task<(bool Sucesso, string Mensagem)> CancelarPedidoEstoqueAsync(int idPedido)
    {
        try
        {
            var response = await _httpClient.PatchAsJsonAsync(
                $"/api/pedidos-estoque/{idPedido}/status",
                new AtualizarStatusDto { NovoStatus = StatusPedido.CANCELADO },
                ApiJsonOptions);

            if (response.IsSuccessStatusCode)
            {
                return (true, "Pedido cancelado com sucesso.");
            }

            var erro = await response.Content.ReadAsStringAsync();
            using var documento = JsonDocument.Parse(erro);
            var mensagem = documento.RootElement.TryGetProperty("mensagem", out var campoMensagem)
                ? campoMensagem.GetString() ?? "Não foi possível cancelar o pedido."
                : erro;
            return (false, mensagem);
        }
        catch (HttpRequestException ex)
        {
            return (false, $"Não foi possível conectar à API: {ex.Message}");
        }
        catch (TaskCanceledException ex)
        {
            return (false, $"A conexão com a API excedeu o tempo limite: {ex.Message}");
        }
        catch (JsonException ex)
        {
            return (false, $"A API retornou uma resposta inválida: {ex.Message}");
        }
    }
}
