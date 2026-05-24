# 🐾 Pet Health API

> **FIAP — Challenge 2026 | Sprint 1**  
> Advanced Business Development with .NET  
> RM556649

---

## 📋 Descrição do Projeto

A **Pet Health API** é uma API RESTful desenvolvida em **ASP.NET Core (.NET 8)** como parte do Challenge 2026 da FIAP, em parceria com a **CLYVO VET**.

O projeto resolve um problema real do mercado pet: a **fragmentação da jornada de saúde do animal**. Tutores esquecem vacinas, perdem histórico clínico e clínicas perdem recorrência por falta de acompanhamento preventivo.

A API centraliza **tutores, pets, vacinas, consultas e medicamentos**, formando a base da plataforma Pet Health — uma solução de cuidado contínuo e preventivo para pets.

### 🏗️ Tecnologias Utilizadas

| Tecnologia | Versão | Finalidade |
|---|---|---|
| ASP.NET Core | .NET 8 | Framework principal da API |
| Entity Framework Core | 8.0 | ORM para mapeamento das entidades |
| Oracle.EntityFrameworkCore | 8.21.121 | Driver de conexão com Oracle |
| Swashbuckle (Swagger) | 6.5.0 | Documentação OpenAPI |
| Oracle Database | — | Banco de dados (servidor FIAP) |

---

## 🗂️ Estrutura do Projeto

```
PetHealthAPI/
├── Controllers/
│   ├── TutoresController.cs      # CRUD + 4 rotas parametrizadas
│   ├── PetsController.cs         # CRUD + 4 rotas parametrizadas
│   ├── VacinasController.cs      # CRUD + 3 rotas parametrizadas
│   ├── ConsultasController.cs    # CRUD + 3 rotas parametrizadas
│   └── MedicamentosController.cs # CRUD + 3 rotas parametrizadas
├── Data/
│   └── AppDbContext.cs           # DbContext + mapeamento das entidades
├── Models/
│   ├── Tutor.cs
│   ├── Pet.cs
│   ├── Vacina.cs
│   ├── Consulta.cs
│   └── Medicamento.cs
├── appsettings.json              # String de conexão Oracle
└── Program.cs                   # Configuração da aplicação
```

---

## 🗄️ Modelo de Banco de Dados

```
TB_PH_TUTOR (1) ──── (N) TB_PH_PET
                              │
              ┌───────────────┼──────────────────┐
              │               │                  │
        TB_PH_VACINA   TB_PH_CONSULTA   TB_PH_MEDICAMENTO
```

---

## 🛣️ Documentação das Rotas

### 👤 Tutores — `/api/tutores`

| Método | Rota | Descrição | HTTP |
|--------|------|-----------|------|
| GET | `/api/tutores` | Lista todos os tutores | 200 |
| GET | `/api/tutores/{id}` | Busca tutor por ID | 200 / 404 |
| GET | `/api/tutores/email/{email}` | Busca tutor por email | 200 / 404 |
| GET | `/api/tutores/nome/{nome}` | Busca tutores por nome (parcial) | 200 |
| GET | `/api/tutores/{id}/pets` | Lista os pets de um tutor | 200 / 404 |
| POST | `/api/tutores` | Cadastra novo tutor | 201 / 400 |
| PUT | `/api/tutores/{id}` | Atualiza dados do tutor | 204 / 400 / 404 |
| DELETE | `/api/tutores/{id}` | Remove tutor | 204 / 404 |

### 🐶 Pets — `/api/pets`

| Método | Rota | Descrição | HTTP |
|--------|------|-----------|------|
| GET | `/api/pets` | Lista todos os pets | 200 |
| GET | `/api/pets/{id}` | Busca pet por ID (com histórico completo) | 200 / 404 |
| GET | `/api/pets/especie/{especie}` | Lista pets por espécie | 200 |
| GET | `/api/pets/nome/{nome}` | Busca pets por nome | 200 |
| GET | `/api/pets/castrados` | Lista apenas pets castrados | 200 |
| POST | `/api/pets` | Cadastra novo pet | 201 / 400 |
| PUT | `/api/pets/{id}` | Atualiza dados do pet | 204 / 400 / 404 |
| DELETE | `/api/pets/{id}` | Remove pet | 204 / 404 |

### 💉 Vacinas — `/api/vacinas`

| Método | Rota | Descrição | HTTP |
|--------|------|-----------|------|
| GET | `/api/vacinas` | Lista todas as vacinas | 200 |
| GET | `/api/vacinas/{id}` | Busca vacina por ID | 200 / 404 |
| GET | `/api/vacinas/pet/{petId}` | Lista vacinas de um pet | 200 / 404 |
| GET | `/api/vacinas/vencendo` | Vacinas com próxima dose em 30 dias | 200 |
| POST | `/api/vacinas` | Registra nova vacina | 201 / 400 |
| PUT | `/api/vacinas/{id}` | Atualiza dados da vacina | 204 / 400 / 404 |
| DELETE | `/api/vacinas/{id}` | Remove registro de vacina | 204 / 404 |

### 🏥 Consultas — `/api/consultas`

| Método | Rota | Descrição | HTTP |
|--------|------|-----------|------|
| GET | `/api/consultas` | Lista todas as consultas | 200 |
| GET | `/api/consultas/{id}` | Busca consulta por ID | 200 / 404 |
| GET | `/api/consultas/pet/{petId}` | Histórico clínico de um pet | 200 / 404 |
| GET | `/api/consultas/retornos` | Retornos agendados nos próximos 30 dias | 200 |
| POST | `/api/consultas` | Registra nova consulta | 201 / 400 |
| PUT | `/api/consultas/{id}` | Atualiza dados da consulta | 204 / 400 / 404 |
| DELETE | `/api/consultas/{id}` | Remove registro de consulta | 204 / 404 |

### 💊 Medicamentos — `/api/medicamentos`

| Método | Rota | Descrição | HTTP |
|--------|------|-----------|------|
| GET | `/api/medicamentos` | Lista todos os medicamentos | 200 |
| GET | `/api/medicamentos/{id}` | Busca medicamento por ID | 200 / 404 |
| GET | `/api/medicamentos/pet/{petId}` | Lista medicamentos de um pet | 200 / 404 |
| GET | `/api/medicamentos/ativos` | Lista medicamentos em uso ativo | 200 |
| POST | `/api/medicamentos` | Registra novo medicamento | 201 / 400 |
| PUT | `/api/medicamentos/{id}` | Atualiza dados do medicamento | 204 / 400 / 404 |
| DELETE | `/api/medicamentos/{id}` | Remove registro de medicamento | 204 / 404 |

---

## ⚙️ Instalação e Execução

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Git](https://git-scm.com/)
- Acesso à rede FIAP (VPN se necessário para o Oracle)

### Passo a Passo

**1. Clone o repositório**
```bash
git clone https://github.com/SEU_USUARIO/PetHealthAPI.git
cd PetHealthAPI
```

**2. Restaure os pacotes NuGet**
```bash
dotnet restore
```

**3. Execute as Migrations para criar as tabelas no Oracle**
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

**4. Execute a aplicação**
```bash
dotnet run
```

**5. Acesse o Swagger**
```
http://localhost:5000
```

---

## 🔌 Configuração do Banco de Dados

A string de conexão está no arquivo `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "OracleConnection": "User Id=rm556649;Password=280306;Data Source=oracle.fiap.com.br:1521/orcl;"
  }
}
```

---

## 📌 Retornos HTTP Utilizados

| Código | Significado | Quando ocorre |
|--------|-------------|---------------|
| 200 OK | Sucesso | GET com resultado |
| 201 Created | Criado com sucesso | POST bem-sucedido |
| 204 No Content | Sucesso sem corpo | PUT e DELETE bem-sucedidos |
| 400 Bad Request | Dados inválidos | Validação falhou ou dados inconsistentes |
| 404 Not Found | Não encontrado | Recurso inexistente no banco |

---

## 👥 Integrantes

| Nome | RM |
|------|----|
| [Seu Nome] | RM556649 |

---

*Challenge 2026 — FIAP × CLYVO VET*  
*"Cuidado contínuo para quem faz parte da família."*
