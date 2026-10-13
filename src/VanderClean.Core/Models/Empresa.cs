using System.Text.Json.Serialization;

namespace VanderClean.Core.Models;

public class Empresa
{
    public int Id { get; set; }
    public string RazaoSocial { get; set; } = string.Empty;
    public string? NomeFantasia { get; set; }
    public string Cnpj { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Cep { get; set; }
    public string? Rua { get; set; }
    public string? Numero { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public string? Uf { get; set; }
    
    [JsonIgnore]
    public string SenhaHash { get; set; } = string.Empty;
    
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    public bool Ativo { get; set; } = true;

    [JsonIgnore]
    public List<PedidoEstoque> PedidosEstoque { get; set; } = new();
}
