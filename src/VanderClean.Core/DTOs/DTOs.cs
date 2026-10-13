using VanderClean.Core.Enums;

namespace VanderClean.Core.DTOs;

public class EmpresaRegistroDto
{
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
}

public class ClienteRegistroDto
{
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
    public string Senha { get; set; } = string.Empty;
}

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

public class ItemPedidoCriacaoDto
{
    public int IdProduto { get; set; }
    public decimal Quantidade { get; set; }
}

public class CriarPedidoEstoqueDto
{
    public int IdEmpresa { get; set; }
    public string? Observacao { get; set; }
    public List<ItemPedidoCriacaoDto> Itens { get; set; } = new();
}

public class CriarEncomendaDto
{
    public int IdCliente { get; set; }
    public string? Observacao { get; set; }
    public List<ItemPedidoCriacaoDto> Itens { get; set; } = new();
}

public class AtualizarStatusDto
{
    public StatusPedido NovoStatus { get; set; }
}
