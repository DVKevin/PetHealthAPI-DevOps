#!/bin/bash
# ============================================================
# PetHealthAPI - Script Azure CLI
# Challenge FIAP 2026 - DevOps Tools & Cloud Computing
# ------------------------------------------------------------
# O QUE ESTE SCRIPT FAZ (em sequência, sem intervenção manual):
#   1) Faz login na Azure (caso ainda não esteja autenticado)
#   2) Cria um Resource Group
#   3) Cria uma VM Linux Ubuntu 22.04 LTS
#   4) Abre as portas: 22 (SSH), 80, 1521 (Oracle), 8080 (API)
#   5) Instala Docker, Docker Compose, Git, Nano e Curl na VM
#
# APÓS O SCRIPT: conecte via SSH e rode manualmente:
#   git clone https://github.com/DVKevin/PetHealthAPI.git
#   cd PetHealthAPI
#   docker compose up -d --build
#
# USO:
#   chmod +x deploy-azure.sh
#   ./deploy-azure.sh
# ============================================================


# ────────────────────────────────────────────────────────────
# 1) VARIÁVEIS DE CONFIGURAÇÃO
# ────────────────────────────────────────────────────────────
RESOURCE_GROUP="rg-pethealth-fiap"
LOCATION="chilecentral"
VM_NAME="vm-pethealth-fiap"
VM_USER="azureuser"
VM_IMAGE="Ubuntu2204"
VM_SIZE="Standard_B2als_v2"
ADMIN_PASSWORD="PetHealth@2026!"
GITHUB_REPO="https://github.com/DVKevin/PetHealthAPI-DevOps.git"

# ────────────────────────────────────────────────────────────
# 2) Funções auxiliares
# ────────────────────────────────────────────────────────────
GREEN='\033[0;32m'; YELLOW='\033[1;33m'; CYAN='\033[0;36m'; NC='\033[0m'
step() { echo -e "\n${CYAN}===> $1${NC}"; }
ok()   { echo -e "${GREEN}[OK] $1${NC}"; }
warn() { echo -e "${YELLOW}[!]  $1${NC}"; }

# ────────────────────────────────────────────────────────────
# 3) LOGIN NA AZURE
# ────────────────────────────────────────────────────────────
step "Verificando login na Azure"
if ! az account show > /dev/null 2>&1; then
  warn "Você ainda não está logado. Abrindo navegador..."
  az login
fi
ok "Login confirmado. Subscription ativa:"
az account show --query "{nome:name}" -o table

# ────────────────────────────────────────────────────────────
# 4) RESOURCE GROUP
# ────────────────────────────────────────────────────────────
step "Criando Resource Group '$RESOURCE_GROUP' em $LOCATION"
az group create \
  --name "$RESOURCE_GROUP" \
  --location "$LOCATION" \
  --output table
ok "Resource Group criado."

# ────────────────────────────────────────────────────────────
# 5) MÁQUINA VIRTUAL LINUX
# ────────────────────────────────────────────────────────────
step "Provisionando VM Linux '$VM_NAME' (pode levar 2-3 minutos)"
az vm create \
  --resource-group "$RESOURCE_GROUP" \
  --name           "$VM_NAME" \
  --image          "$VM_IMAGE" \
  --size           "$VM_SIZE" \
  --admin-username "$VM_USER" \
  --admin-password "$ADMIN_PASSWORD" \
  --authentication-type password \
  --public-ip-sku Standard \
  --os-disk-size-gb 30 \
  --output table
ok "VM provisionada com sucesso."

PUBLIC_IP=$(az vm show \
  --resource-group "$RESOURCE_GROUP" \
  --name "$VM_NAME" \
  --show-details \
  --query publicIps -o tsv)
ok "IP público da VM: $PUBLIC_IP"

# ────────────────────────────────────────────────────────────
# 6) ABERTURA DE PORTAS
# ────────────────────────────────────────────────────────────
step "Abrindo portas no NSG (22, 80, 1521, 8080)"
warn "Aguardando NSG ficar disponível (45s)..."
sleep 45

az vm open-port --resource-group "$RESOURCE_GROUP" --name "$VM_NAME" --port 80   --priority 1001 --output none
az vm open-port --resource-group "$RESOURCE_GROUP" --name "$VM_NAME" --port 8081 --priority 1002 --output none
az vm open-port --resource-group "$RESOURCE_GROUP" --name "$VM_NAME" --port 1521 --priority 1003 --output none
ok "Portas 22, 80, 1521 e 8081 liberadas."

# ────────────────────────────────────────────────────────────
# 7) INSTALAÇÃO DE FERRAMENTAS NA VM
# ────────────────────────────────────────────────────────────
step "Instalando Docker + Docker Compose + Git + Nano + Curl na VM"

az vm run-command invoke \
  --resource-group "$RESOURCE_GROUP" \
  --name "$VM_NAME" \
  --command-id RunShellScript \
  --scripts '
        export DEBIAN_FRONTEND=noninteractive
    sudo apt-get update -y
    sudo apt-get install -y git nano curl wget ca-certificates gnupg lsb-release htop
    sudo install -m 0755 -d /etc/apt/keyrings
    curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg
    sudo chmod a+r /etc/apt/keyrings/docker.gpg
    echo "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable" | sudo tee /etc/apt/sources.list.d/docker.list
    sudo apt-get update -y
    sudo apt-get install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
    sudo usermod -aG docker azureuser
    sudo systemctl enable docker
    sudo systemctl start docker
    docker --version
    docker compose version
    git --version
  ' \
  --output table
ok "Docker, Docker Compose, Git, Nano e Curl instalados."

# ────────────────────────────────────────────────────────────
# 8) RESUMO FINAL
# ────────────────────────────────────────────────────────────
echo ""
echo -e "${GREEN}=============================================================${NC}"
echo -e "${GREEN}  INFRAESTRUTURA PROVISIONADA COM SUCESSO!${NC}"
echo -e "${GREEN}=============================================================${NC}"
echo -e "Resource Group : ${CYAN}$RESOURCE_GROUP${NC}"
echo -e "VM             : ${CYAN}$VM_NAME${NC}"
echo -e "IP público     : ${CYAN}$PUBLIC_IP${NC}"
echo -e "SSH            : ${CYAN}ssh $VM_USER@$PUBLIC_IP${NC}  (senha: $ADMIN_PASSWORD)"
echo ""
echo -e "${YELLOW}PRÓXIMOS PASSOS — rode estes comandos após conectar via SSH:${NC}"
echo -e "   ${CYAN}git clone $GITHUB_REPO${NC}"
echo -e "   ${CYAN}cd PetHealthAPI${NC}"
echo -e "   ${CYAN}docker compose up -d --build${NC}"
echo ""
echo -e "Swagger (API)  : ${CYAN}http://$PUBLIC_IP:8081${NC}"
echo -e "Endpoint base  : ${CYAN}http://$PUBLIC_IP:8081/api/tutores${NC}"
echo ""
echo -e "${YELLOW}Para apagar TUDO ao final do Challenge, rode:${NC}"
echo -e "   ${CYAN}az group delete --name $RESOURCE_GROUP --yes --no-wait${NC}"
echo -e "${GREEN}=============================================================${NC}"