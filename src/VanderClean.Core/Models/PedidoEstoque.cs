using VanderClean.Core.Enums;

namespace VanderClean.Core.Models;

public class PedidoEstoque
{
    public int Id { get; set; }
    public int IdEmpresa { get; set; }
    public Empresa? Empresa { get; set; }
    public DateTime DataPedido { get; set; } = DateTime.UtcNow;
    public StatusPedido Status { get; set; } = StatusPedido.PENDENTE;
    public int PrazoEntregaDias { get; set; } = 3;
    public DateTime? DataEntregaPrev { get; set; }
    public string? Observacao { get; set; }
    public decimal ValorTotal { get; set; }

    public List<ItemPedidoEstoque> Itens { get; set; } = new();
}
