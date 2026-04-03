#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
DOTNET_DIR="$ROOT_DIR/.vercel-tools/dotnet"

if [[ ! -x "$DOTNET_DIR/dotnet" ]]; then
    bash "$ROOT_DIR/scripts/vercel-install-dotnet.sh"
fi

export DOTNET_ROOT="$DOTNET_DIR"
export PATH="$DOTNET_ROOT:$PATH"

dotnet publish "$ROOT_DIR/src/SchemaForge.Wasm/SchemaForge.Wasm.csproj" -c Release -o "$ROOT_DIR/output" --nologo
