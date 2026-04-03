#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
TOOLS_DIR="$ROOT_DIR/.vercel-tools"
DOTNET_DIR="$TOOLS_DIR/dotnet"
INSTALL_SCRIPT="$TOOLS_DIR/dotnet-install.sh"
SDK_VERSION="$(sed -n 's/.*"version": *"\([^"]*\)".*/\1/p' "$ROOT_DIR/global.json" | head -n 1)"

if [[ -z "$SDK_VERSION" ]]; then
    echo "Unable to determine .NET SDK version from global.json." >&2
    exit 1
fi

mkdir -p "$TOOLS_DIR"

if [[ -x "$DOTNET_DIR/dotnet" ]]; then
    INSTALLED_VERSION="$("$DOTNET_DIR/dotnet" --version || true)"
    if [[ "$INSTALLED_VERSION" == "$SDK_VERSION" ]]; then
        echo "Using cached .NET SDK $INSTALLED_VERSION"
        exit 0
    fi
fi

echo "Installing .NET SDK $SDK_VERSION for Vercel build"
curl -fsSL https://dot.net/v1/dotnet-install.sh -o "$INSTALL_SCRIPT"
bash "$INSTALL_SCRIPT" --version "$SDK_VERSION" --install-dir "$DOTNET_DIR" --no-path
