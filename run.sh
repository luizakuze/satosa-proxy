#!/bin/bash

# Caminho do diretório base
BASE_DIR=$(dirname "$0")

# Caminho da configuração
CONFIG="$BASE_DIR/saml2-social/proxy_conf.yaml"
KEY="$BASE_DIR/saml2-social/pki/frontend.key"
CERT="$BASE_DIR/saml2-social/pki/frontend.crt"

# Ativa o ambiente virtual se necessário
if [ -f "$BASE_DIR/.venv/bin/activate" ]; then
  source "$BASE_DIR/.venv/bin/activate"
fi

# Sobe o SATOSA
SATOSA_CONFIG="$CONFIG" gunicorn -b 0.0.0.0:5002 satosa.wsgi:app \
  --keyfile "$KEY" \
  --certfile "$CERT"
