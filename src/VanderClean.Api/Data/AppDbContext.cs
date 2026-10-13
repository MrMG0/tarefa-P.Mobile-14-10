using Microsoft.EntityFrameworkCore;
using VanderClean.Core.Models;

namespace VanderClean.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Empresa> Empresas => Set<Empresa>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<PedidoEstoque> PedidosEstoque => Set<PedidoEstoque>();
    public DbSet<ItemPedidoEstoque> ItensPedidoEstoque => Set<ItemPedidoEstoque>();
    public DbSet<Encomenda> Encomendas => Set<Encomenda>();
    public DbSet<ItemEncomenda> ItensEncomenda => Set<ItemEncomenda>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Mapeamento Empresa
        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.ToTable("empresa");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RazaoSocial).HasColumnName("razao_social").HasMaxLength(200).IsRequired();
            entity.Property(e => e.NomeFantasia).HasColumnName("nome_fantasia").HasMaxLength(200);
            entity.Property(e => e.Cnpj).HasColumnName("cnpj").HasMaxLength(18).IsRequired();
            entity.HasIndex(e => e.Cnpj).IsUnique();
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(200).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Telefone).HasColumnName("telefone").HasMaxLength(20);
            entity.Property(e => e.Cep).HasColumnName("cep").HasMaxLength(10);
            entity.Property(e => e.Rua).HasColumnName("rua").HasMaxLength(200);
            entity.Property(e => e.Numero).HasColumnName("numero").HasMaxLength(20);
            entity.Property(e => e.Bairro).HasColumnName("bairro").HasMaxLength(100);
            entity.Property(e => e.Cidade).HasColumnName("cidade").HasMaxLength(100);
            entity.Property(e => e.Uf).HasColumnName("uf").HasMaxLength(2);
            entity.Property(e => e.SenhaHash).HasColumnName("senha_hash").HasMaxLength(255).IsRequired();
            entity.Property(e => e.DataCadastro).HasColumnName("data_cadastro");
            entity.Property(e => e.Ativo).HasColumnName("ativo").HasDefaultValue(true);
        });

        // Mapeamento Cliente
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("clientes");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).HasColumnName("id");
            entity.Property(c => c.Nome).HasColumnName("nome").HasMaxLength(200).IsRequired();
            entity.Property(c => c.Email).HasColumnName("email").HasMaxLength(200).IsRequired();
            entity.HasIndex(c => c.Email).IsUnique();
            entity.Property(c => c.Telefone).HasColumnName("telefone").HasMaxLength(20);
            entity.Property(c => c.TipoPessoa).HasColumnName("tipo_pessoa").HasConversion<string>().IsRequired();
            entity.Property(c => c.CpfCnpj).HasColumnName("cpf_cnpj").HasMaxLength(18).IsRequired();
            entity.HasIndex(c => c.CpfCnpj).IsUnique();
            entity.Property(c => c.Cep).HasColumnName("cep").HasMaxLength(10);
            entity.Property(c => c.Rua).HasColumnName("rua").HasMaxLength(200);
            entity.Property(c => c.Numero).HasColumnName("numero").HasMaxLength(20);
            entity.Property(c => c.Bairro).HasColumnName("bairro").HasMaxLength(100);
            entity.Property(c => c.Cidade).HasColumnName("cidade").HasMaxLength(100);
            entity.Property(c => c.Uf).HasColumnName("uf").HasMaxLength(2);
            entity.Property(c => c.SenhaHash).HasColumnName("senha_hash").HasMaxLength(255).IsRequired();
            entity.Property(c => c.DataCadastro).HasColumnName("data_cadastro");
            entity.Property(c => c.Ativo).HasColumnName("ativo").HasDefaultValue(true);
        });

        // Mapeamento Categoria
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.ToTable("categorias");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).HasColumnName("id");
            entity.Property(c => c.Nome).HasColumnName("nome").HasMaxLength(100).IsRequired();
            entity.Property(c => c.Descricao).HasColumnName("descricao");
            entity.Property(c => c.Ativo).HasColumnName("ativo").HasDefaultValue(true);
        });

        // Mapeamento Produto
        modelBuilder.Entity<Produto>(entity =>
        {
            entity.ToTable("produtos");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).HasColumnName("id");
            entity.Property(p => p.Nome).HasColumnName("nome").HasMaxLength(200).IsRequired();
            entity.Property(p => p.Descricao).HasColumnName("descricao");
            entity.Property(p => p.Preco).HasColumnName("preco").HasPrecision(10, 2);
            entity.Property(p => p.IdCategoria).HasColumnName("id_categoria");
            entity.Property(p => p.Disponivel).HasColumnName("disponivel").HasDefaultValue(true);
            entity.Property(p => p.ImagemUrl).HasColumnName("imagem_url").HasMaxLength(500);
            entity.Property(p => p.Estoque).HasColumnName("estoque").HasDefaultValue(0);
            entity.Property(p => p.UnidadeMedida).HasColumnName("unidade_medida").HasConversion<string>();
            entity.Property(p => p.DataCadastro).HasColumnName("data_cadastro");
            entity.Property(p => p.Ativo).HasColumnName("ativo").HasDefaultValue(true);

            entity.HasOne(p => p.Categoria)
                  .WithMany(c => c.Produtos)
                  .HasForeignKey(p => p.IdCategoria)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Mapeamento PedidosEstoque
        modelBuilder.Entity<PedidoEstoque>(entity =>
        {
            entity.ToTable("pedidos_estoque");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).HasColumnName("id");
            entity.Property(p => p.IdEmpresa).HasColumnName("id_empresa");
            entity.Property(p => p.DataPedido).HasColumnName("data_pedido");
            entity.Property(p => p.Status).HasColumnName("status").HasConversion<string>();
            entity.Property(p => p.PrazoEntregaDias).HasColumnName("prazo_entrega_dias").HasDefaultValue(3);
            entity.Property(p => p.DataEntregaPrev).HasColumnName("data_entrega_prev");
            entity.Property(p => p.Observacao).HasColumnName("observacao");
            entity.Property(p => p.ValorTotal).HasColumnName("valor_total").HasPrecision(10, 2);

            entity.HasOne(p => p.Empresa)
                  .WithMany(e => e.PedidosEstoque)
                  .HasForeignKey(p => p.IdEmpresa)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Mapeamento ItensPedidoEstoque
        modelBuilder.Entity<ItemPedidoEstoque>(entity =>
        {
            entity.ToTable("itens_pedido_estoque");
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Id).HasColumnName("id");
            entity.Property(i => i.IdPedidoEstoque).HasColumnName("id_pedido_estoque");
            entity.Property(i => i.IdProduto).HasColumnName("id_produto");
            entity.Property(i => i.Quantidade).HasColumnName("quantidade").HasPrecision(10, 3);
            entity.Property(i => i.PrecoUnitario).HasColumnName("preco_unitario").HasPrecision(10, 2);
            entity.Property(i => i.Subtotal).HasColumnName("subtotal").HasPrecision(10, 2);

            entity.HasOne(i => i.PedidoEstoque)
                  .WithMany(p => p.Itens)
                  .HasForeignKey(i => i.IdPedidoEstoque)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(i => i.Produto)
                  .WithMany()
                  .HasForeignKey(i => i.IdProduto)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Mapeamento Encomendas
        modelBuilder.Entity<Encomenda>(entity =>
        {
            entity.ToTable("encomendas");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.DataEncomenda).HasColumnName("data_encomenda");
            entity.Property(e => e.Status).HasColumnName("status").HasConversion<string>();
            entity.Property(e => e.PrazoEntregaDias).HasColumnName("prazo_entrega_dias").HasDefaultValue(3);
            entity.Property(e => e.DataEntregaPrev).HasColumnName("data_entrega_prev");
            entity.Property(e => e.Observacao).HasColumnName("observacao");
            entity.Property(e => e.ValorTotal).HasColumnName("valor_total").HasPrecision(10, 2);

            entity.HasOne(e => e.Cliente)
                  .WithMany(c => c.Encomendas)
                  .HasForeignKey(e => e.IdCliente)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Mapeamento ItensEncomenda
        modelBuilder.Entity<ItemEncomenda>(entity =>
        {
            entity.ToTable("itens_encomenda");
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Id).HasColumnName("id");
            entity.Property(i => i.IdEncomenda).HasColumnName("id_encomenda");
            entity.Property(i => i.IdProduto).HasColumnName("id_produto");
            entity.Property(i => i.Quantidade).HasColumnName("quantidade").HasPrecision(10, 3);
            entity.Property(i => i.PrecoUnitario).HasColumnName("preco_unitario").HasPrecision(10, 2);
            entity.Property(i => i.Subtotal).HasColumnName("subtotal").HasPrecision(10, 2);

            entity.HasOne(i => i.Encomenda)
                  .WithMany(e => e.Itens)
                  .HasForeignKey(i => i.IdEncomenda)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(i => i.Produto)
                  .WithMany()
                  .HasForeignKey(i => i.IdProduto)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
