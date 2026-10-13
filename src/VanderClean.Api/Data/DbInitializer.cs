using Microsoft.EntityFrameworkCore;
using VanderClean.Core.Enums;
using VanderClean.Core.Models;

namespace VanderClean.Api.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (await context.Categorias.AnyAsync())
        {
            return; // Já populado
        }

        var categorias = new List<Categoria>
        {
            new Categoria { Nome = "Detergentes e Desengordurantes", Descricao = "Produtos para limpeza de louças, cozinhas profissionais e superfícies oleosas" },
            new Categoria { Nome = "Desinfetantes e Cloro", Descricao = "Bactericidas, sanitizantes e hipoclorito para higienização profunda" },
            new Categoria { Nome = "Lava-Roupas e Amaciantes", Descricao = "Cuidados têxteis profissionais, lavanderias e perfumes duradouros" },
            new Categoria { Nome = "Limpa-Vidros e Multiuso", Descricao = "Superfícies de vidro, espelhos e bancadas com secagem rápida" },
            new Categoria { Nome = "Tratamento de Pisos e Ceras", Descricao = "Removedores de cera, impermeabilizantes e ceras acrílicas antiderrapantes" },
            new Categoria { Nome = "Acessórios e EPIs", Descricao = "Panos microfibra, luvas de proteção, baldes e rodos profissionais" }
        };

        await context.Categorias.AddRangeAsync(categorias);
        await context.SaveChangesAsync();

        var produtos = new List<Produto>
        {
            // Categoria 0: Detergentes e Desengordurantes
            new Produto
            {
                Nome = "Detergente Neutro Concentrado 5L",
                Descricao = "Detergente neutro biodegradável de alto rendimento para louças e superfícies gerais.",
                Preco = 24.90m,
                IdCategoria = categorias[0].Id,
                Disponivel = true,
                Estoque = 150,
                UnidadeMedida = UnidadeMedida.LITRO,
                ImagemUrl = "https://images.unsplash.com/photo-1584308666744-24d5c474f2ae?w=400"
            },
            new Produto
            {
                Nome = "Desengordurante Industrial Flash Clean 5L",
                Descricao = "Ação rápida contra gorduras pesadas em cozinhas industriais, chapas, coifas e fornos.",
                Preco = 39.50m,
                IdCategoria = categorias[0].Id,
                Disponivel = true,
                Estoque = 80,
                UnidadeMedida = UnidadeMedida.LITRO,
                ImagemUrl = "https://images.unsplash.com/photo-1585421514738-01798e348b17?w=400"
            },
            new Produto
            {
                Nome = "Detergente Clorado Alcalino 5L",
                Descricao = "Detergente e alvejante simultâneo para laticínios, açougues e câmaras frias.",
                Preco = 45.00m,
                IdCategoria = categorias[0].Id,
                Disponivel = true,
                Estoque = 60,
                UnidadeMedida = UnidadeMedida.LITRO,
                ImagemUrl = "https://images.unsplash.com/photo-1584308666744-24d5c474f2ae?w=400"
            },

            // Categoria 1: Desinfetantes e Cloro
            new Produto
            {
                Nome = "Desinfetante Floral Lavanda Francesa 5L",
                Descricao = "Bactericida concentrado com fixação prolongada de aroma de lavanda.",
                Preco = 29.90m,
                IdCategoria = categorias[1].Id,
                Disponivel = true,
                Estoque = 120,
                UnidadeMedida = UnidadeMedida.LITRO,
                ImagemUrl = "https://images.unsplash.com/photo-1583947215259-38e31be8751f?w=400"
            },
            new Produto
            {
                Nome = "Desinfetante Hospitalar Quaternário de Amônio 5L",
                Descricao = "Amplo espectro antimicrobiano indicado para clínicas, hospitais e academias.",
                Preco = 58.00m,
                IdCategoria = categorias[1].Id,
                Disponivel = true,
                Estoque = 50,
                UnidadeMedida = UnidadeMedida.LITRO,
                ImagemUrl = "https://images.unsplash.com/photo-1584308666744-24d5c474f2ae?w=400"
            },
            new Produto
            {
                Nome = "Água Sanitária Premium 5L (Cloro Ativo 2.5%)",
                Descricao = "Alvejante e desinfetante potente para sanitização de pisos, ralos e banheiros.",
                Preco = 18.00m,
                IdCategoria = categorias[1].Id,
                Disponivel = true,
                Estoque = 200,
                UnidadeMedida = UnidadeMedida.LITRO,
                ImagemUrl = "https://images.unsplash.com/photo-1584308666744-24d5c474f2ae?w=400"
            },

            // Categoria 2: Lava-Roupas e Amaciantes
            new Produto
            {
                Nome = "Sabão Líquido Especial Para Roupas 5L",
                Descricao = "Remove sujeiras profundas sem agredir as fibras nem desbotar as cores.",
                Preco = 34.90m,
                IdCategoria = categorias[2].Id,
                Disponivel = true,
                Estoque = 90,
                UnidadeMedida = UnidadeMedida.LITRO,
                ImagemUrl = "https://images.unsplash.com/photo-1610557892470-55d9e80c0bce?w=400"
            },
            new Produto
            {
                Nome = "Amaciante Têxtil Toque Suave 5L",
                Descricao = "Maciez instantânea e perfumação agradável para toalhas, lençóis e uniformes.",
                Preco = 28.50m,
                IdCategoria = categorias[2].Id,
                Disponivel = true,
                Estoque = 110,
                UnidadeMedida = UnidadeMedida.LITRO,
                ImagemUrl = "https://images.unsplash.com/photo-1610557892470-55d9e80c0bce?w=400"
            },
            new Produto
            {
                Nome = "Alvejante Sem Cloro O2 White 5L",
                Descricao = "Poder do oxigênio ativo para remoção de manchas em tecidos brancos e coloridos.",
                Preco = 36.00m,
                IdCategoria = categorias[2].Id,
                Disponivel = true,
                Estoque = 70,
                UnidadeMedida = UnidadeMedida.LITRO,
                ImagemUrl = "https://images.unsplash.com/photo-1610557892470-55d9e80c0bce?w=400"
            },

            // Categoria 3: Limpa-Vidros e Multiuso
            new Produto
            {
                Nome = "Limpa Vidros Cristal Clean 500ml",
                Descricao = "Transparência total sem resíduos nem manchas em vidros, vitrines e espelhos.",
                Preco = 9.80m,
                IdCategoria = categorias[3].Id,
                Disponivel = true,
                Estoque = 180,
                UnidadeMedida = UnidadeMedida.UNIDADE,
                ImagemUrl = "https://images.unsplash.com/photo-1585421514738-01798e348b17?w=400"
            },
            new Produto
            {
                Nome = "Multiuso Antibac Floral 500ml",
                Descricao = "Praticidade para limpeza instantânea de mesas, bancadas e eletrodomésticos.",
                Preco = 8.50m,
                IdCategoria = categorias[3].Id,
                Disponivel = true,
                Estoque = 250,
                UnidadeMedida = UnidadeMedida.UNIDADE,
                ImagemUrl = "https://images.unsplash.com/photo-1583947215259-38e31be8751f?w=400"
            },
            new Produto
            {
                Nome = "Lustra Móveis Silicone e Cera de Carnaúba 500ml",
                Descricao = "Protege superfícies amadeiradas, criando camada protetora contra poeira e umidade.",
                Preco = 14.20m,
                IdCategoria = categorias[3].Id,
                Disponivel = true,
                Estoque = 95,
                UnidadeMedida = UnidadeMedida.UNIDADE,
                ImagemUrl = "https://images.unsplash.com/photo-1585421514738-01798e348b17?w=400"
            },

            // Categoria 4: Tratamento de Pisos e Ceras
            new Produto
            {
                Nome = "Cera Acrílica Alto Brilho e Tráfego 5L",
                Descricao = "Película ultra resistente para pisos de cerâmica, vinílico, granilite e ardósia.",
                Preco = 72.00m,
                IdCategoria = categorias[4].Id,
                Disponivel = true,
                Estoque = 40,
                UnidadeMedida = UnidadeMedida.LITRO,
                ImagemUrl = "https://images.unsplash.com/photo-1584308666744-24d5c474f2ae?w=400"
            },
            new Produto
            {
                Nome = "Removedor de Cera e Gorduras de Piso 5L",
                Descricao = "Amolece e desincrusta camadas antigas de ceras e sujeiras pesadas sem agredir o piso.",
                Preco = 49.90m,
                IdCategoria = categorias[4].Id,
                Disponivel = true,
                Estoque = 45,
                UnidadeMedida = UnidadeMedida.LITRO,
                ImagemUrl = "https://images.unsplash.com/photo-1584308666744-24d5c474f2ae?w=400"
            },

            // Categoria 5: Acessórios e EPIs
            new Produto
            {
                Nome = "Kit Panos Microfibra Super Absorventes (5 un)",
                Descricao = "Não solta fiapos, alta retenção de poeira e líquido para acabamento impecável.",
                Preco = 19.90m,
                IdCategoria = categorias[5].Id,
                Disponivel = true,
                Estoque = 100,
                UnidadeMedida = UnidadeMedida.UNIDADE,
                ImagemUrl = "https://images.unsplash.com/photo-1584308666744-24d5c474f2ae?w=400"
            },
            new Produto
            {
                Nome = "Luvas Nitrílicas Profissionais Cano Longo (Par)",
                Descricao = "Alta resistência química para contato com alvejantes, desengordurantes e solventes.",
                Preco = 16.50m,
                IdCategoria = categorias[5].Id,
                Disponivel = true,
                Estoque = 150,
                UnidadeMedida = UnidadeMedida.UNIDADE,
                ImagemUrl = "https://images.unsplash.com/photo-1584308666744-24d5c474f2ae?w=400"
            },
            new Produto
            {
                Nome = "Balde Espremedor Duplo 30L com Rodízios",
                Descricao = "Sistema de duas cubas com prensa reforçada para higienização profissional de pisos.",
                Preco = 189.00m,
                IdCategoria = categorias[5].Id,
                Disponivel = true,
                Estoque = 20,
                UnidadeMedida = UnidadeMedida.UNIDADE,
                ImagemUrl = "https://images.unsplash.com/photo-1584308666744-24d5c474f2ae?w=400"
            }
        };

        await context.Produtos.AddRangeAsync(produtos);

        // 1. Empresa Padrão (Dono/Funcionário - App)
        var empresaTeste = new Empresa
        {
            RazaoSocial = "VanderClean Comércio de Produtos de Limpeza Ltda",
            NomeFantasia = "VanderClean Matriz",
            Cnpj = "12.345.678/0001-90",
            Email = "contato@vanderclean.com.br",
            Telefone = "(11) 98765-4321",
            Cep = "01001-000",
            Rua = "Praça da Sé",
            Numero = "100",
            Bairro = "Sé",
            Cidade = "São Paulo",
            Uf = "SP",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            Ativo = true,
            DataCadastro = DateTime.UtcNow
        };
        await context.Empresas.AddAsync(empresaTeste);
        await context.SaveChangesAsync();

        // 2. Cliente Padrão (Cliente Final - Web)
        var clienteTeste = new Cliente
        {
            Nome = "Carlos Eduardo Silva",
            Email = "cliente@teste.com",
            Telefone = "(11) 97777-8888",
            TipoPessoa = TipoPessoa.PF,
            CpfCnpj = "123.456.789-00",
            Cep = "04567-000",
            Rua = "Av. Paulista",
            Numero = "1500",
            Bairro = "Bela Vista",
            Cidade = "São Paulo",
            Uf = "SP",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            Ativo = true,
            DataCadastro = DateTime.UtcNow
        };
        await context.Clientes.AddAsync(clienteTeste);
        await context.SaveChangesAsync();

        // 3. Pedidos de Estoque Iniciais (App)
        var pedido1 = new PedidoEstoque
        {
            IdEmpresa = empresaTeste.Id,
            DataPedido = DateTime.UtcNow.AddDays(-3),
            Status = StatusPedido.EM_SEPARACAO,
            PrazoEntregaDias = 3,
            DataEntregaPrev = DateTime.UtcNow,
            Observacao = "Reposição semanal de produtos de alta demanda",
            ValorTotal = 74.30m,
            Itens = new List<ItemPedidoEstoque>
            {
                new ItemPedidoEstoque { IdProduto = produtos[0].Id, Quantidade = 2, PrecoUnitario = produtos[0].Preco, Subtotal = produtos[0].Preco * 2 },
                new ItemPedidoEstoque { IdProduto = produtos[3].Id, Quantidade = 1, PrecoUnitario = produtos[3].Preco, Subtotal = produtos[3].Preco }
            }
        };

        var pedido2 = new PedidoEstoque
        {
            IdEmpresa = empresaTeste.Id,
            DataPedido = DateTime.UtcNow.AddDays(-1),
            Status = StatusPedido.PENDENTE,
            PrazoEntregaDias = 3,
            DataEntregaPrev = DateTime.UtcNow.AddDays(2),
            Observacao = "Reposição de alvejantes e produtos de vidro",
            ValorTotal = 53.60m,
            Itens = new List<ItemPedidoEstoque>
            {
                new ItemPedidoEstoque { IdProduto = produtos[5].Id, Quantidade = 2, PrecoUnitario = produtos[5].Preco, Subtotal = produtos[5].Preco * 2 },
                new ItemPedidoEstoque { IdProduto = produtos[9].Id, Quantidade = 1, PrecoUnitario = produtos[9].Preco, Subtotal = produtos[9].Preco },
                new ItemPedidoEstoque { IdProduto = produtos[10].Id, Quantidade = 1, PrecoUnitario = produtos[10].Preco, Subtotal = produtos[10].Preco }
            }
        };

        await context.PedidosEstoque.AddRangeAsync(pedido1, pedido2);

        // 4. Encomenda Inicial do Cliente (Web)
        var encomenda1 = new Encomenda
        {
            IdCliente = clienteTeste.Id,
            DataEncomenda = DateTime.UtcNow.AddDays(-2),
            Status = StatusPedido.CONFIRMADO,
            PrazoEntregaDias = 3,
            DataEntregaPrev = DateTime.UtcNow.AddDays(1),
            Observacao = "Encomenda para condomínio residencial",
            ValorTotal = 84.70m,
            Itens = new List<ItemEncomenda>
            {
                new ItemEncomenda { IdProduto = produtos[0].Id, Quantidade = 2, PrecoUnitario = produtos[0].Preco, Subtotal = 49.80m },
                new ItemEncomenda { IdProduto = produtos[6].Id, Quantidade = 1, PrecoUnitario = produtos[6].Preco, Subtotal = 34.90m }
            }
        };
        await context.Encomendas.AddAsync(encomenda1);

        await context.SaveChangesAsync();
    }
}
