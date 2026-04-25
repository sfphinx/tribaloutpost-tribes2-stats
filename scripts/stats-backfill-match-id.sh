#!/usr/bin/env bash
# Backfill client_match_id into legacy .match files on a game server.
#
# Run once after upgrading TribalOutpostStats.vl2 to a build that emits
# client_match_id. Idempotent — re-running is safe; only adds the field
# where it is missing. The id is derived from the filename, which already
# encodes the unique per-server prefix (yy-mm-dd_HHnnss_<map>).
#
# Usage:
#   ./scripts/stats-backfill-match-id.sh [stats-dir]
#
# Default stats-dir: TribalOutpostStats
set -euo pipefail

STATS_DIR="${1:-TribalOutpostStats}"

if [ ! -d "$STATS_DIR/matches" ]; then
  echo "No matches/ directory in $STATS_DIR" >&2
  exit 1
fi

ADDED=0
SKIPPED=0
shopt -s nullglob
for f in "$STATS_DIR"/matches/*.match; do
  if grep -q "^client_match_id=" "$f"; then
    SKIPPED=$((SKIPPED + 1))
  else
    prefix=$(basename "$f" .match)
    echo "client_match_id=$prefix" >> "$f"
    ADDED=$((ADDED + 1))
  fi
done

echo "Backfill complete: $ADDED added, $SKIPPED already present."
