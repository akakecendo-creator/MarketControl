# 🛒 MarketControl

![C#](https://img.shields.io/badge/C%23-.NET%208-68217A)
![SQL Server](https://img.shields.io/badge/SQL%20Server-Database-CC2927)
![ADO.NET](https://img.shields.io/badge/ADO.NET-Data%20Access-512BD4)
![status](https://img.shields.io/badge/status-Em%20desenvolvimento-yellowgreen)

Implementação de um sistema de controle de mercado em C#, executado via console, com autenticação por perfil, cadastro de clientes e produtos, e registro de vendas com persistência em SQL Server.

---

## 📌 Sobre o Projeto

O MarketControl foi desenvolvido com foco em prática de back-end com C# e SQL Server, aplicando regras reais de operação de mercado em um sistema simples e funcional.

Este projeto trabalha principalmente com:

- organização por serviços
- conexão com banco usando ADO.NET
- autenticação com perfis de acesso
- validação de entradas do usuário
- controle de estoque durante vendas
- transações para garantir integridade dos dados

---

## 🚀 Funcionalidades

- ✅ Login obrigatório antes de acessar o sistema
- ✅ Perfis separados entre administrador e operador
- ✅ Cadastro de produtos
- ✅ Listagem de produtos
- ✅ Cadastro de clientes
- ✅ Listagem de clientes
- ✅ Registro de vendas com transação
- ✅ Validação de cliente e produto durante a venda
- ✅ Venda por ID ou nome
- ✅ Atualização automática do estoque
- ✅ Leitura segura de entradas no console

---

## 🔐 Perfis de Acesso

### Administrador

Permissões:

- cadastrar produtos
- listar produtos
- cadastrar clientes
- listar clientes
- registrar vendas
- trocar de usuário

### Operador

Permissões:

- listar produtos
- listar clientes
- registrar vendas
- trocar de usuário

## 👤 Usuários Iniciais

Na primeira execução, o sistema garante a existência da tabela `Usuario` e cria usuários padrão se eles ainda não existirem.

- Login: `admin`
  Senha: `admin123`
  Perfil: `Administrador`
- Login: `operador`
  Senha: `operador123`
  Perfil: `Operador`

---

## 💻 Como Executar o Projeto

### 1️⃣ Criar o banco de dados

Execute o script `database/script.sql` no SQL Server para criar o banco e as tabelas.

### 2️⃣ Configurar a conexão

Crie o arquivo `appsettings.Development.json` com a string de conexão do seu ambiente. Você pode usar `appsettings.Development.json.example` como base.

### 3️⃣ Compilar o projeto

```powershell
dotnet build .\MarketControl.csproj
```

### 4️⃣ Executar o sistema

```powershell
dotnet run --project .\MarketControl.csproj
```

---

## 📈 Próximas Melhorias

- cadastro e gestão de usuários pelo administrador
- troca obrigatória de senha no primeiro acesso
- auditoria de quem realizou cada venda
- relatórios de vendas e estoque
- busca parcial por nome com seleção assistida
