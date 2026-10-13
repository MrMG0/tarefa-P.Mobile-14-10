# Plano e Relatório de Testes - VanderClean (ShopVander)

**Projeto:** Sistema de Pedidos e Loja Online VanderClean  
**Data:** 10/10/2026  
**Status dos Testes:** ✅ **100% Aprovados**

---

## 1. Dados Populados no Banco de Dados (Seed Inicial)

O banco de dados central foi carregado automaticamente pela classe `DbInitializer.cs`, contendo massa de dados realista para todas as entidades da especificação:

### 1.1. Categorias de Produtos (6 categorias)
1. **Detergentes e Desengordurantes:** Louças, cozinhas profissionais e superfícies oleosas.
2. **Desinfetantes e Cloro:** Bactericidas, sanitizantes e hipoclorito de sódio.
3. **Lava-Roupas e Amaciantes:** Cuidados têxteis profissionais e lavanderia.
4. **Limpa-Vidros e Multiuso:** Vidros, espelhos e bancadas sem manchas.
5. **Tratamento de Pisos e Ceras:** Ceras acrílicas antiderrapantes e removedores.
6. **Acessórios e EPIs:** Panos microfibra, luvas nitrílicas e baldes espremedores duplos.

### 1.2. Produtos Cadastrados (17 produtos com estoque, fotos e preços reais)
- *Detergente Neutro Concentrado 5L* (R$ 24,90 | Estoque: 150)
- *Desengordurante Industrial Flash Clean 5L* (R$ 39,50 | Estoque: 80)
- *Detergente Clorado Alcalino 5L* (R$ 45,00 | Estoque: 60)
- *Desinfetante Floral Lavanda Francesa 5L* (R$ 29,90 | Estoque: 120)
- *Desinfetante Hospitalar Quaternário de Amônio 5L* (R$ 58,00 | Estoque: 50)
- *Água Sanitária Premium 5L 2.5%* (R$ 18,00 | Estoque: 200)
- *Sabão Líquido Especial Para Roupas 5L* (R$ 34,90 | Estoque: 90)
- *Amaciante Têxtil Toque Suave 5L* (R$ 28,50 | Estoque: 110)
- *Alvejante Sem Cloro O2 White 5L* (R$ 36,00 | Estoque: 70)
- *Limpa Vidros Cristal Clean 500ml* (R$ 9,80 | Estoque: 180)
- *Multiuso Antibac Floral 500ml* (R$ 8,50 | Estoque: 250)
- *Lustra Móveis Silicone e Carnaúba 500ml* (R$ 14,20 | Estoque: 95)
- *Cera Acrílica Alto Brilho e Tráfego 5L* (R$ 72,00 | Estoque: 40)
- *Removedor de Cera e Gorduras 5L* (R$ 49,90 | Estoque: 45)
- *Kit Panos Microfibra Super Absorventes (5 un)* (R$ 19,90 | Estoque: 100)
- *Luvas Nitrílicas Profissionais Cano Longo* (R$ 16,50 | Estoque: 150)
- *Balde Espremedor Duplo 30L com Rodízios* (R$ 189,00 | Estoque: 20)

### 1.3. Usuários e Pedidos Iniciais
- **Empresa (App):** `contato@vanderclean.com.br` | Senha: `123456` | CNPJ: `12.345.678/0001-90`
- **Cliente (Site):** `cliente@teste.com` | Senha: `123456` | CPF: `123.456.789-00`
- **Histórico Inicial:** 2 pedidos de reposição (App) + 1 encomenda do cliente (Site).

---

## 2. Testes Automatizados (xUnit - VanderClean.Tests)

Executados via `dotnet test tests/VanderClean.Tests/VanderClean.Tests.csproj`:

| ID do Teste | Escopo / Módulo | Descrição do Caso de Teste | Resultado |
|---|---|---|:---:|
| **UT-01** | `DocumentValidator` | Validação de CNPJ com máscara e dígitos verificadores | ✅ APROVADO |
| **UT-02** | `DocumentValidator` | Rejeição de CNPJs inválidos e repetidos | ✅ APROVADO |
| **UT-03** | `DocumentValidator` | Validação de CPF com máscara e dígitos verificadores | ✅ APROVADO |
| **UT-04** | `DocumentValidator` | Rejeição de CPFs inválidos e com dígitos falsos | ✅ APROVADO |
| **BR-01** | Regra de Negócio (RN05) | Repetição de Pedido de Estoque rejeitada caso produto esteja indisponível | ✅ APROVADO |
| **BR-02** | Regra de Negócio (RN11) | Rejeição de Encomenda Web se a quantidade solicitada exceder o estoque disponível | ✅ APROVADO |
| **BR-03** | Regra de Negócio (RN14) | Proibição de cancelamento de pedidos com status `ENVIADO` ou `ENTREGUE` | ✅ APROVADO |
| **BR-04** | Regra de Negócio (RN15) | Cálculo automático e preciso de subtotais e valor total do pedido | ✅ APROVADO |

**Resumo da Execução xUnit:** Total: 13 | Aprovados: 13 | Falhas: 0 | Duração: ~1s.

---

## 3. Testes de Integração e Ponta a Ponta (API Central)

Testes de requisições HTTP RESTful contra os controladores da API:

1. **Login Corporativo (`POST /api/empresa/login`):**
   - Entrada: `contato@vanderclean.com.br` + hash BCrypt.
   - Resposta: HTTP 200 OK com payload da empresa VanderClean.
2. **Login do Cliente Web (`POST /api/clientes/login`):**
   - Entrada: `cliente@teste.com` + hash BCrypt.
   - Resposta: HTTP 200 OK com dados cadastrais e identificador do cliente.
3. **Criação de Encomenda Web (`POST /api/encomendas`):**
   - Entrada: 2 itens (Detergente Neutro + Desinfetante).
   - Resposta: HTTP 201 Created, baixa automática no estoque, status `PENDENTE`, prazo de 3 dias úteis.
4. **Repetição de Encomenda Web (`POST /api/encomendas/{id}/repetir`):**
   - Resposta: HTTP 201 Created gerando nova encomenda clonada e validando o estoque atualizado.
5. **Criação de Pedido de Reposição App (`POST /api/pedidos-estoque`):**
   - Entrada: 5 unidades de Desengordurante + 10 unidades de Limpa Vidros.
   - Resposta: HTTP 201 Created com cálculo correto de R$ 295,50.
6. **Atualização de Status (`PATCH /api/pedidos-estoque/{id}/status`):**
   - Transição: de `PENDENTE` para `CONFIRMADO`.
   - Resposta: HTTP 200 OK confirmando novo status da encomenda.

---

## 4. Testes do Aplicativo Mobile (.NET MAUI - ShopVander App)

- **Fluxo de Autenticação:**
  - Login validado contra a API.
  - Teste de Fallback Offline: se o servidor estiver inacessível, o app autentica pelo SQLite local.
- **Navegação Shell:**
  - Ao autenticar, transição imediata para a tela de **Pedidos de Reposição** (sem crash ou fechamento da janela).
- **Lista de Pedidos Feitos:**
  - Exibição de cards com número do pedido, badge de status, data formatada e total em R$.
  - Botão de repetição de pedido acionando a API com feedback em modal.
- **Botão `+` na Navigation Bar (Topo Direito):**
  - Acionamento rápido levando para o Catálogo de Reposição (`NovoPedidoPage`).
  - Filtro funcional por categoria de produto.
  - Seletores de quantidade `+` e `-` com recálculo instantâneo do valor total.
  - Confirmação de pedido gravando no SQLite local e enviando para a API central.

---

## 5. Testes da Plataforma Web (.NET Blazor - ShopVander Web)

- **Navegação no Catálogo:**
  - Filtros instantâneos por categoria (Detergentes, Desinfetantes, Pisos, EPIs, etc.).
  - Visualização de fotos, descrições, preços e badges de estoque.
- **Carrinho Interativo (`CartService`):**
  - Adição de produtos sem recarregar a página (Server-Side Interactive).
  - Contador de itens atualizado dinamicamente no menu lateral.
- **Área do Cliente:**
  - Abas "Já sou Cliente" e "Criar Conta" com validação de CPF e CNPJ.
- **Histórico e Repetição:**
  - Listagem de todas as encomendas do cliente com seus itens e status.
  - Botão "Repetir Encomenda" recriando o pedido com 1 clique.
