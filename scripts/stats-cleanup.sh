#!/usr/bin/env bash
# Clean up stats files for matches confirmed ingested.
#
# Removes <prefix>.match / .players / .ext / .plays / .done sets where:
#   - a .done marker exists (catchup confirmed server-side ingestion), and
#   - the .match file is older than --age days (default 14).
#
# Designed to run after stats-catchup, to keep the stats directory from
# growing indefinitely. Recent matches stay on disk so a server-side
# issue inside the retention window can still be re-investigated.
#
# Usage:
#   ./scripts/stats-cleanup.sh [options] <stats-dir>
#
# Options:
#   --age <days>       Minimum age of .match file (default: 14)
#   --dry-run          Show what would be deleted without deleting
#   --verbose          Show per-match detail
#
# Cron example (daily at 4am):
#   0 4 * * * /path/to/stats-cleanup.sh /path/to/TribalOutpostStats >> /var/log/stats-cleanup.log 2>&1
#
set -euo pipefail

AGE_DAYS=14
DRY_RUN=0
VERBOSE=0
STATS_DIR=""

while [[ $# -gt 0 ]]; do
  case "$1" in
    --age)     AGE_DAYS="$2"; shift 2 ;;
    --dry-run) DRY_RUN=1; shift ;;
    --verbose) VERBOSE=1; shift ;;
    -h|--help) sed -n '2,/^$/p' "$0" | sed 's/^# \?//'; exit 0 ;;
    *)         STATS_DIR="$1"; shift ;;
  esac
done

if [ -z "$STATS_DIR" ]; then
  echo "Usage: $0 [options] <stats-dir>"
  exit 1
fi

DONE_DIR="$STATS_DIR/done"
MATCH_DIR="$STATS_DIR/matches"

if [ ! -d "$DONE_DIR" ]; then
  echo "No done/ directory in $STATS_DIR — nothing to clean."
  exit 0
fi

log()  { echo "[$(date '+%Y-%m-%d %H:%M:%S')] $*"; }
vlog() { [ "$VERBOSE" = "1" ] && log "$*" || true; }

DELETED=0
SKIPPED_FRESH=0
ORPHANED=0

shopt -s nullglob
for done_file in "$DONE_DIR"/*.done; do
  prefix=$(basename "$done_file" .done)
  match_file="$MATCH_DIR/$prefix.match"

  # Orphaned .done — .match already gone. Clean the marker regardless of age.
  if [ ! -f "$match_file" ]; then
    vlog "  $prefix: .match missing, removing orphaned .done"
    if [ "$DRY_RUN" = "1" ]; then
      log "  [DRY RUN] Would remove orphaned: $done_file"
    else
      rm -f "$done_file"
    fi
    ORPHANED=$((ORPHANED + 1))
    continue
  fi

  # find -mtime +N matches files strictly older than N*24h.
  if [ -z "$(find "$match_file" -mtime +"$AGE_DAYS" 2>/dev/null)" ]; then
    SKIPPED_FRESH=$((SKIPPED_FRESH + 1))
    continue
  fi

  if [ "$DRY_RUN" = "1" ]; then
    log "  [DRY RUN] Would delete: $prefix"
    DELETED=$((DELETED + 1))
    continue
  fi

  vlog "  Deleting: $prefix"
  rm -f \
    "$match_file" \
    "$STATS_DIR/players/$prefix.players" \
    "$STATS_DIR/ext/$prefix.ext" \
    "$STATS_DIR/plays/$prefix.plays" \
    "$done_file"
  DELETED=$((DELETED + 1))
done

log "Cleanup: $DELETED removed, $SKIPPED_FRESH kept (younger than ${AGE_DAYS}d), $ORPHANED orphaned .done cleaned"
