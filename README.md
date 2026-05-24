# 🐾 PetHealthAPI

**Challenge FIAP 2026 — DevOps Tools & Cloud Computing**

API RESTful em **.NET 8** para gestão completa de saúde de pets (tutores, pets, vacinas, consultas e medicamentos), containerizada com **Docker Compose** e implantada em uma **VM Linux Ubuntu na Azure**, com banco **Oracle XE 18c** containerizado e persistência via volume nomeado.

---

## 📑 Índice

1. [Descrição do Projeto](#-descrição-do-projeto)
2. [Benefícios para o Negócio](#-benefícios-para-o-negócio)
3. [Arquitetura Macro](#-arquitetura-macro)
4. [Tecnologias Utilizadas](#-tecnologias-utilizadas)
5. [Estrutura do Projeto](#-estrutura-do-projeto)
6. [Rotas da API (CRUD)](#-rotas-da-api-crud)
7. [How To — Instalação e Execução na Azure](#-how-to--instalação-e-execução-na-azure)
8. [Validação da Persistência](#-validação-da-persistência)
9. [Comandos Úteis](#-comandos-úteis)
10. [Equipe](#-equipe)

---

## 📋 Descrição do Projeto

O **PetHealthAPI** é uma plataforma de gestão veterinária que centraliza o histórico de saúde de pets em uma única API REST. Tutores cadastram seus animais, registram vacinas aplicadas (com data, fabricante, lote e veterinário), agendam e arquivam consultas (com diagnóstico, tratamento e custo), e controlam medicamentos em uso (dosagem, frequência, início e fim do tratamento).

A solução é entregue como dois containers orquestrados via Docker Compose:

- **`pethealth-api`** — aplicação .NET 8 rodando como usuário não-root, expondo Swagger em `:8080`.
- **`pethealth-oracle`** — banco Oracle XE 18c com volume nomeado para persistência real dos dados.

Tudo provisionado em uma VM Linux na Azure por meio de um único script Azure CLI.

---

## 💼 Benefícios para o Negócio

| Benefício | Impacto |
|---|---|
| 🩺 **Histórico clínico centralizado** | Veterinários acessam vacinas, consultas e medicamentos em segundos, sem dependência de papel. |
| 💰 **Redução de custos com consultas duplicadas** | Tutores e clínicas evitam exames repetidos por desconhecimento do histórico. |
| 📅 **Lembretes preventivos** | Próximas doses de vacina e datas de retorno ficam rastreáveis (`DataProximaDose`, `DataRetorno`). |
| 🔗 **Integração com qualquer front-end** | API REST + Swagger pública para apps mobile, sistemas de clínicas e telemetria de IoT. |
| ⚡ **Time-to-market acelerado** | Deploy completo em nuvem em menos de 5 minutos via script Azure CLI. |
| ♻️ **Custo de infraestrutura sob controle** | Containerização permite escalar horizontalmente e deletar o ambiente com 1 comando. |

---

## 🏗 Arquitetura Macro

```
                              ┌────────────────────────────────────────┐
                              │         AZURE CLOUD (Chile Central)    │
                              │                                        │
   ┌──────────┐               │   ┌─────────────────────────────────┐  │
   │ Usuário  │   HTTP        │   │   VM Linux Ubuntu 22.04 LTS     │  │
   │ /Postman │ ─────────────►│   │   IP público: x.x.x.x           │  │
   └──────────┘   :8080       │   │                                 │  │
                              │   │   ┌─── Docker Network ─────┐    │  │
                              │   │   │   (pethealth-net)      │    │  │
                              │   │   │                        │    │  │
                              │   │   │  ┌──────────────────┐  │    │  │
                              │   │   │  │ pethealth-api    │  │    │  │
                              │   │   │  │ .NET 8 + EF Core │  │    │  │
                              │   │   │  │ usuário não-root │  │    │  │
                              │   │   │  │ porta 8080       │  │    │  │
                              │   │   │  └────────┬─────────┘  │    │  │
                              │   │   │           │ Oracle     │    │  │
                              │   │   │           ▼ 1521       │    │  │
                              │   │   │  ┌──────────────────┐  │    │  │
                              │   │   │  │ pethealth-oracle │  │    │  │
                              │   │   │  │ Oracle XE 18c    │  │    │  │
                              │   │   │  └────────┬─────────┘  │    │  │
                              │   │   │           │            │    │  │
                              │   │   └───────────┼────────────┘    │  │
                              │   │               ▼                 │  │
                              │   │   ┌─────────────────────────┐   │  │
                              │   │   │  Volume nomeado         │   │  │
                              │   │   │  pethealth-oracle-data  │   │  │
                              │   │   │  (persistência real)    │   │  │
                              │   │   └─────────────────────────┘   │  │
                              │   └─────────────────────────────────┘  │
                              └────────────────────────────────────────┘
```

**Fluxo de requisições:**
1. Cliente (Postman, navegador, front-end) chama `http://IP_VM:8080/api/...`
2. NSG da Azure permite tráfego nas portas 22, 80, 1521 e 8080
3. Kestrel (servidor da API .NET) recebe e roteia para os controllers
4. EF Core consulta o Oracle via DNS interno do Docker (`oracle-db:1521`)
5. Oracle persiste no volume `pethealth-oracle-data` (mantido mesmo se o container morrer)

---

## 🛠 Tecnologias Utilizadas

| Camada | Tecnologia | Versão |
|---|---|---|
| Linguagem | C# / .NET | 8.0 |
| ORM | Entity Framework Core + Oracle Provider | 8.21 |
| API | ASP.NET Core Web API + Swagger | 8.0 / 6.5 |
| Banco | Oracle Database Express Edition | 18c (gvenzl/oracle-xe:18-slim-faststart) |
| Containerização | Docker + Docker Compose | latest |
| Sistema Operacional | Ubuntu Server | 22.04 LTS |
| Cloud | Microsoft Azure (Standard_B2ats_v2) | — |
| Provisionamento | Azure CLI | latest |

---

## 📁 Estrutura do Projeto

```
PetHealthAPI-DevOps/
├── PetHealthAPI/                     ← Código-fonte da API .NET
│   ├── Controllers/                  ← CRUDs (Tutores, Pets, Vacinas, Consultas, Medicamentos)
│   ├── Models/                       ← Entidades do domínio
│   ├── Data/AppDbContext.cs          ← DbContext do EF Core
│   ├── Program.cs                    ← Bootstrap + retry de conexão com Oracle
│   ├── appsettings.json
│   ├── appsettings.Production.json   ← Connection string para Oracle containerizado
│   └── PetHealthAPI.csproj
│
├── db-init/
│   └── 01-seed.sql                   ← 2 inserts significativos iniciais (tutores + pets)
│
├── scripts/
│   ├── deploy-azure.sh               ← Script Azure CLI completo (provisiona tudo)
│   └── destroy-azure.sh              ← Script para apagar a VM ao final
│
├── Dockerfile                        ← Multi-stage .NET 8 + usuário não-root (petuser)
├── docker-compose.yml                ← API + Oracle 18c + volume nomeado + network
├── .gitignore
└── README.md                         ← este arquivo
```

---

## 🌐 Rotas da API (CRUD)

A API expõe 5 controllers, cada um com CRUD completo (GET, POST, PUT, DELETE). Swagger disponível em `http://IP_VM:8080/`.

### 👤 Tutores — `/api/tutores`

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/tutores` | Lista todos os tutores |
| GET | `/api/tutores/{id}` | Busca tutor por ID |
| POST | `/api/tutores` | Cadastra novo tutor |
| PUT | `/api/tutores/{id}` | Atualiza tutor |
| DELETE | `/api/tutores/{id}` | Remove tutor (cascade nos pets) |

### 🐶 Pets — `/api/pets`
### 💉 Vacinas — `/api/vacinas`
### 🩺 Consultas — `/api/consultas`
### 💊 Medicamentos — `/api/medicamentos`

Todos seguem o mesmo padrão CRUD.

#### Exemplo: POST de Tutor
```http
POST http://IP_VM:8080/api/tutores
Content-Type: application/json

{
  "nome": "Ana Beatriz Costa",
  "email": "ana.beatriz@pethealth.com.br",
  "telefone": "11933334444",
  "endereco": "Rua dos Pinheiros, 500 - São Paulo - SP"
}
```

---

## 🚀 How To — Instalação e Execução na Azure

### Pré-requisitos (na sua máquina)

- **Azure CLI** instalado
- Conta Azure ativa
- Git Bash instalado
- Git instalado

### Passo 1 — Clonar o repositório

```bash
git clone https://github.com/DVKevin/PetHealthAPI-DevOps.git
cd PetHealthAPI-DevOps
```

### Passo 2 — Executar o script Azure CLI

```bash
./scripts/deploy-azure.sh
```

O script faz em sequência:
1. Login na Azure
2. Cria o Resource Group `rg-pethealth-fiap` (Chile Central)
3. Provisiona a VM Linux Ubuntu 22.04
4. Abre as portas 22, 80, 1521 e 8080
5. Instala Docker, Docker Compose, Git, Nano e Curl

### Passo 3 — Conectar na VM e subir os containers

```bash
ssh azureuser@IP_VM
# senha: PetHealth@2026!

sudo sysctl -w kernel.shmmax=536870912
sudo sysctl -w kernel.shmall=131072

git clone https://github.com/DVKevin/PetHealthAPI-DevOps.git
cd PetHealthAPI-DevOps
docker compose up -d --build
```

### Passo 4 — Acessar a API

- 🌐 **Swagger UI:** `http://IP_VM:8080/`
- 📡 **Endpoint base:** `http://IP_VM:8080/api/tutores`

### Passo 5 — Deletar tudo ao final (OBRIGATÓRIO)

```bash
az group delete --name rg-pethealth-fiap --yes --no-wait
```

---

## 🔒 Validação da Persistência

```bash
# 1) Cria um registro
curl -X POST http://IP_VM:8080/api/tutores \
  -H "Content-Type: application/json" \
  -d '{"nome":"Teste Persistência","email":"teste@pers.com","telefone":"11900000000","endereco":"Rua Teste, 1"}'

# 2) Derruba os containers
cd ~/PetHealthAPI-DevOps && docker compose down

# 3) Sobe novamente
docker compose up -d && sleep 90

# 4) Confirma que o registro ainda existe
curl http://IP_VM:8080/api/tutores
```

✅ O registro continua na lista → **persistência via volume nomeado funcionando**.

---

## 📊 Comandos Úteis

```bash
# Status dos containers
docker compose ps

# Logs da API
docker compose logs -f pethealth-api

# Logs do banco
docker compose logs -f oracle-db

# Volumes nomeados
docker volume ls

# Confirma usuário não-root
docker exec pethealth-api whoami

# Consumo de recursos
docker stats --no-stream
```

---

## 👥 Equipe

| RM | Nome |
|---|---|
| 563454 | Kevin Martins Campos |
| 566058 | Pedro Gabriel Claes |
| 556649 | Matheus Arazin de Oliveira |
| 565597 | Arthur Pioli Silva |

📺 **Vídeo da entrega:** [link do YouTube]
📂 **Repositório:** [https://github.com/DVKevin/PetHealthAPI-DevOps](https://github.com/DVKevin/PetHealthAPI-DevOps)

---

## 📜 Licença

Projeto acadêmico — FIAP Challenge 2026.
