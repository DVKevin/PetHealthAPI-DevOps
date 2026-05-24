#!/bin/bash
# ============================================================
# PetHealthAPI - Exclusão da VM e do Resource Group
# Challenge FIAP 2026
# ------------------------------------------------------------
# EXIGÊNCIA DO PROFESSOR: ao final da entrega, a VM DEVE ser
# excluída. Não excluir = ZERO pontos.
#
# Este script deleta TODOS os recursos criados no deploy.
# ============================================================

RESOURCE_GROUP="rg-pethealth-fiap"

echo "==> Excluindo Resource Group '$RESOURCE_GROUP' (VM, disco, IP, NSG, NIC, VNet)..."
echo "    Isso vai apagar TODOS os recursos do grupo."
read -p "    Tem certeza? (s/N): " confirma

if [[ "$confirma" =~ ^[sS]$ ]]; then
  az group delete --name "$RESOURCE_GROUP" --yes --no-wait
  echo "==> Comando enviado. A exclusão roda em background na Azure."
  echo "==> Verifique no Portal: https://portal.azure.com -> Resource Groups"
  echo ""
  echo "==> Acompanhar status (opcional):"
  echo "    az group show --name $RESOURCE_GROUP"
  echo "    (vai retornar 'ResourceGroupNotFound' quando terminar)"
else
  echo "==> Operação cancelada."
fi
