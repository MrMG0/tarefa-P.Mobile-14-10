using System.Text.Json.Serialization;

namespace VanderClean.Core.Models;

public class ItemEncomenda
{
    public int Id { get; set; }
    public int IdEncomenda { get; set; }

    [JsonIgnore]
    public Encomenda? Encomenda { get; set; }

    public int IdProduto { get; set; }
    public Produto? Produto { get; set; }
    public decimal Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal Subtotal { get; set; }
}
