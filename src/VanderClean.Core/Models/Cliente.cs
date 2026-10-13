using System.Text.Json.Serialization;
using VanderClean.Core.Enums;

namespace VanderClean.Core.Models;

public class Cliente
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public TipoPessoa TipoPessoa { get; set; } = TipoPessoa.PF;
    public string CpfCnpj { get; set; } = string.Empty;
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
    public List<Encomenda> Encomendas { get; set; } = new();
}
