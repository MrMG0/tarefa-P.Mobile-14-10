using VanderClean.Core.Enums;

namespace VanderClean.Core.Models;

public class Encomenda
{
    public int Id { get; set; }
    public int IdCliente { get; set; }
    public Cliente? Cliente { get; set; }
    public DateTime DataEncomenda { get; set; } = DateTime.UtcNow;
    public StatusPedido Status { get; set; } = StatusPedido.PENDENTE;
    public int PrazoEntregaDias { get; set; } = 3;
    public DateTime? DataEntregaPrev { get; set; }
    public string? Observacao { get; set; }
    public decimal ValorTotal { get; set; }

    public List<ItemEncomenda> Itens { get; set; } = new();
}
