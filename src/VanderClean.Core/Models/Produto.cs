using VanderClean.Core.Enums;

namespace VanderClean.Core.Models;

public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public int IdCategoria { get; set; }
    public Categoria? Categoria { get; set; }
    public bool Disponivel { get; set; } = true;
    public string? ImagemUrl { get; set; }
    public int Estoque { get; set; } = 0;
    public UnidadeMedida UnidadeMedida { get; set; } = UnidadeMedida.UNIDADE;
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    public bool Ativo { get; set; } = true;
}
