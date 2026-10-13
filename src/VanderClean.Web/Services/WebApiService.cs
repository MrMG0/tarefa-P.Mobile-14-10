using System.Net.Http.Json;
using System.Text.Json;
using VanderClean.Core.DTOs;
using VanderClean.Core.Models;

namespace VanderClean.Web.Services;

public class WebApiService
{
    private readonly HttpClient _http;
    private const string ApiBaseUrl = "http://localhost:5000";

    public WebApiService(HttpClient http)
    {
        _http = http;
        if (_http.BaseAddress == null)
        {
            _http.BaseAddress = new Uri(ApiBaseUrl);
        }
    }

    public async Task<(bool Sucesso, string Mensagem, int? ClienteId, string? Nome)> LoginClienteAsync(string email, string senha)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("/api/clientes/login", new LoginDto { Email = email, Senha = senha });
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(content);
                var clienteObj = doc.RootElement.GetProperty("cliente");
                var id = clienteObj.GetProperty("id").GetInt32();
                var nome = clienteObj.GetProperty("nome").GetString();
                return (true, "Login com sucesso", id, nome);
            }
            return (false, "E-mail ou senha inválidos.", null, null);
        }
        catch (Exception ex)
        {
            return (false, $"Erro ao conectar com API: {ex.Message}", null, null);
        }
    }

    public async Task<(bool Sucesso, string Mensagem)> RegistrarClienteAsync(ClienteRegistroDto dto)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("/api/clientes/registrar", dto);
            if (response.IsSuccessStatusCode) return (true, "Cadastro realizado com sucesso!");
            var err = await response.Content.ReadAsStringAsync();
            return (false, err);
        }
        catch (Exception ex)
        {
            return (false, $"Erro: {ex.Message}");
        }
    }

    public async Task<List<Produto>> ObterProdutosAsync(int? idCategoria = null)
    {
        try
        {
            var url = idCategoria.HasValue ? $"/api/produtos?idCategoria={idCategoria.Value}" : "/api/produtos";
            return await _http.GetFromJsonAsync<List<Produto>>(url) ?? new List<Produto>();
        }
        catch
        {
            return new List<Produto>();
        }
    }

    public async Task<List<Categoria>> ObterCategoriasAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<Categoria>>("/api/categorias") ?? new List<Categoria>();
        }
        catch
        {
            return new List<Categoria>();
        }
    }

    public async Task<List<Encomenda>> ObterEncomendasClienteAsync(int idCliente)
    {
        try
        {
            return await _http.GetFromJsonAsync<List<Encomenda>>($"/api/encomendas/cliente/{idCliente}") ?? new List<Encomenda>();
        }
        catch
        {
            return new List<Encomenda>();
        }
    }

    public async Task<(bool Sucesso, string Mensagem)> CriarEncomendaAsync(CriarEncomendaDto dto)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("/api/encomendas", dto);
            if (response.IsSuccessStatusCode) return (true, "Encomenda realizada com sucesso!");
            var err = await response.Content.ReadAsStringAsync();
            return (false, err);
        }
        catch (Exception ex)
        {
            return (false, $"Erro: {ex.Message}");
        }
    }

    public async Task<(bool Sucesso, string Mensagem)> RepetirEncomendaAsync(int idEncomenda)
    {
        try
        {
            var response = await _http.PostAsync($"/api/encomendas/{idEncomenda}/repetir", null);
            if (response.IsSuccessStatusCode) return (true, "Encomenda repetida com sucesso!");
            var err = await response.Content.ReadAsStringAsync();
            return (false, err);
        }
        catch (Exception ex)
        {
            return (false, $"Erro: {ex.Message}");
        }
    }
}
