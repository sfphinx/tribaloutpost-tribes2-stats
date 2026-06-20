#!/usr/bin/env bash
# Build TribalOutpostStats.vl2 from z_toStats.cs
# VL2 = ZIP with scripts/autoexec/ structure
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
PROJECT_DIR="$(dirname "$SCRIPT_DIR")"
OUT_DIR="${OUT_DIR:-$PROJECT_DIR}"

# Extract version from z_toStats.cs
VERSION=$(grep -oP '\$TribalOutpost::Version = "\K[^"]+' "$PROJECT_DIR/z_toStats.cs")
if [ -z "$VERSION" ]; then
  echo "Error: Could not extract version from z_toStats.cs"
  exit 1
fi

VL2_NAME="TribalOutpostStats-v${VERSION}.vl2"

TMPDIR=$(mktemp -d)
trap 'rm -rf "$TMPDIR"' EXIT

mkdir -p "$TMPDIR/scripts/autoexec"
cp "$PROJECT_DIR/z_toStats.cs" "$TMPDIR/scripts/autoexec/z_toStats.cs"

mkdir -p "$OUT_DIR"
cd "$TMPDIR"
zip -r "$OUT_DIR/$VL2_NAME" scripts/

echo "Built $OUT_DIR/$VL2_NAME (v$VERSION)"
