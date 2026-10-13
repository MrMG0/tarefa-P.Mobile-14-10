using SQLite;

namespace VanderClean.App.Models;

[Table("EmpresaLocal")]
public class EmpresaLocal
{
    [PrimaryKey, AutoIncrement]
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
    public string Senha { get; set; } = string.Empty;
    public int SincronizadoApi { get; set; } = 0; // 0 = nao, 1 = sim
}

[Table("PedidoEstoqueLocal")]
public class PedidoEstoqueLocal
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int? IdPedidoApi { get; set; }
    public string DataPedido { get; set; } = string.Empty;
    public string Status { get; set; } = "PENDENTE";
    public int PrazoEntregaDias { get; set; } = 3;
    public string? Observacao { get; set; }
    public decimal ValorTotal { get; set; }
    public int SincronizadoApi { get; set; } = 0; // 0 = nao, 1 = sim
}

[Table("ItemPedidoEstoqueLocal")]
public class ItemPedidoEstoqueLocal
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int IdPedidoEstoque { get; set; }
    public int IdProdutoApi { get; set; }
    public string NomeProduto { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal Subtotal { get; set; }
}
