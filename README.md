# VanderClean - ShopVander

Sistema completo de reposição de estoque e encomendas para a loja **VanderClean**, conforme a especificação do projeto acadêmico.

## 🏗️ Estrutura da Solução

- **`src/VanderClean.Core`**: Modelos de domínio (Empresa, Cliente, Categoria, Produto, PedidoEstoque, Encomenda), DTOs, Enums e validadores de CPF e CNPJ.
- **`src/VanderClean.Api`**: Backend ASP.NET Core RESTful com Entity Framework Core (SQLite em desenvolvimento / MySQL para produção), Swagger/OpenAPI, e rotas completas de autenticação, catálogo e pedidos.
- **`src/VanderClean.App`**: Aplicativo Mobile .NET MAUI com SQLite local para suporte offline, sincronização com a API e fluxo personalizado de telas.
- **`src/VanderClean.Web`**: Aplicação Web para o cliente final.

---

## 🚀 Como Executar a API

Na raiz do projeto:

```bash
dotnet run --project src/VanderClean.Api/VanderClean.Api.csproj --urls "http://localhost:5000"
```

Acesse a documentação interativa e faça testes de todos os endpoints via Swagger:
👉 **http://localhost:5000**

### Usuário de Teste Padrão (Seed Automático):
- **E-mail:** `contato@vanderclean.com.br`
- **Senha:** `123456`
- **Empresa:** VanderClean Comércio de Produtos de Limpeza Ltda

---

## 📱 Fluxo do App Mobile (.NET MAUI)

1. **Login:** autenticação do dono/funcionário (com opção de registrar nova empresa).
2. **Tela Principal (Histórico de Pedidos):**
   - Lista todos os pedidos de reposição com data, valor total e status (*Pendente, Confirmado, Em Separação, etc.*).
   - Botão **`+`** no canto superior direito para abrir o catálogo e criar novo pedido.
   - Botão **"Repetir Pedido"** em cada card para repetição instantânea (com validação de disponibilidade dos produtos).
3. **Novo Pedido:**
   - Filtro por categoria de produtos.
   - Seletores de quantidade (`+` / `-`).
   - Cálculo automático do total estimado.
   - Gravação no SQLite local + envio sincronizado para a API.

---

## 🧪 Como Executar o App Mobile

Para rodar em Windows Desktop:
```bash
dotnet run --project src/VanderClean.App/VanderClean.App.csproj -f net10.0-windows10.0.19041.0
```

Para rodar no emulador Android (com emulador aberto ou dispositivo conectado):
```bash
dotnet build src/VanderClean.App/VanderClean.App.csproj -t:Run -f net10.0-android
```
