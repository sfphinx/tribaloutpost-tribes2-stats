#!/usr/bin/env bash
# Catch up incomplete stats submissions.
#
# Scans a TribalOutpostStats directory for .match files without .done markers,
# checks the API for completeness, and resubmits missing data. Creates .done
# markers for matches that are already complete.
#
# Designed to run as a cron job on the game server, or manually after a
# failed session.
#
# Usage:
#   ./scripts/stats-catchup.sh [options] <stats-dir>
#
# Options:
#   --api <url>        API base URL (default: https://tribaloutpost.com)
#   --token <token>    Auth token (overrides token.txt)
#   --dry-run          Show what would be done without making changes
#   --max <n>          Maximum matches to process per run (default: 20)
#   --min-age <min>    Skip matches whose .match file is newer than N minutes
#                      (default: 15) — avoids racing the in-game upload.
#   --verbose          Show detailed output
#
# Cron example (every 5 minutes; threshold stays at 15 min for safety):
#   */5 * * * * /path/to/stats-catchup.sh --api https://to.com /path/to/TribalOutpostStats >> /var/log/stats-catchup.log 2>&1
#
set -euo pipefail

API_URL="https://tribaloutpost.com"
CLI_TOKEN=""
DRY_RUN=0
MAX_MATCHES=20
MIN_AGE_MINUTES=15
VERBOSE=0
STATS_DIR=""

while [[ $# -gt 0 ]]; do
  case "$1" in
    --api)     API_URL="$2"; shift 2 ;;
    --token)   CLI_TOKEN="$2"; shift 2 ;;
    --dry-run) DRY_RUN=1; shift ;;
    --max)     MAX_MATCHES="$2"; shift 2 ;;
    --min-age) MIN_AGE_MINUTES="$2"; shift 2 ;;
    --verbose) VERBOSE=1; shift ;;
    -h|--help) sed -n '2,/^$/p' "$0" | sed 's/^# \?//'; exit 0 ;;
    *)         STATS_DIR="$1"; shift ;;
  esac
done

if [ -z "$STATS_DIR" ]; then
  echo "Usage: $0 [options] <stats-dir>"
  exit 1
fi

# Resolve token
if [ -n "$CLI_TOKEN" ]; then
  TOKEN="$CLI_TOKEN"
elif [ -n "${TOKEN:-}" ]; then
  TOKEN="$TOKEN"
elif [ -f "$STATS_DIR/token.txt" ]; then
  TOKEN=$(cat "$STATS_DIR/token.txt")
else
  echo "No token found. Use --token, TOKEN env var, or place token.txt in $STATS_DIR"
  exit 1
fi

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
SUBMIT="$SCRIPT_DIR/submit-stats.sh"

MATCH_DIR="$STATS_DIR/matches"
DONE_DIR="$STATS_DIR/done"

if [ ! -d "$MATCH_DIR" ]; then
  echo "No matches/ directory in $STATS_DIR"
  exit 1
fi

mkdir -p "$DONE_DIR"

log() { echo "[$(date '+%Y-%m-%d %H:%M:%S')] $*"; }
vlog() { [ "$VERBOSE" = "1" ] && log "$*" || true; }

# Find match files without .done markers, skipping any whose .match file
# is newer than $MIN_AGE_MINUTES — the in-game script may still be uploading.
PENDING=()
SKIPPED_FRESH=0
for match_file in "$MATCH_DIR"/*.match; do
  [ -f "$match_file" ] || continue
  prefix=$(basename "$match_file" .match)
  [ -f "$DONE_DIR/$prefix.done" ] && continue
  if [ -n "$(find "$match_file" -mmin -"$MIN_AGE_MINUTES" 2>/dev/null)" ]; then
    SKIPPED_FRESH=$((SKIPPED_FRESH + 1))
    continue
  fi
  PENDING+=("$prefix")
done

if [ ${#PENDING[@]} -eq 0 ]; then
  vlog "No pending matches found. (skipped $SKIPPED_FRESH fresh match(es) under ${MIN_AGE_MINUTES}m)"
  exit 0
fi

log "Found ${#PENDING[@]} match(es) without .done markers (processing up to $MAX_MATCHES; skipped $SKIPPED_FRESH fresh under ${MIN_AGE_MINUTES}m)"

PROCESSED=0
SUBMITTED=0
ALREADY_DONE=0
FAILED=0

for prefix in "${PENDING[@]}"; do
  if [ "$PROCESSED" -ge "$MAX_MATCHES" ]; then
    log "Reached max ($MAX_MATCHES), stopping. ${#PENDING[@]} - $PROCESSED remaining."
    break
  fi
  PROCESSED=$((PROCESSED + 1))

  vlog "Checking: $prefix"

  # First, try to submit the match to get/confirm the MID
  # The API returns 409 with mid= if it already exists
  MATCH_FILE="$MATCH_DIR/$prefix.match"
  if [ ! -f "$MATCH_FILE" ]; then
    vlog "  No match file, skipping"
    continue
  fi

  if [ "$DRY_RUN" = "1" ]; then
    log "  [DRY RUN] Would process: $prefix"
    continue
  fi

  # Submit match (or get existing MID from 409)
  MATCH_RESPONSE=$(curl -s -w "\n%{http_code}" \
    -A "TribalOutpostStats" \
    -X POST \
    -H "Content-Type: text/plain" \
    -H "Authorization: Bearer $TOKEN" \
    -H "X-Stats-Version: catchup" \
    --data-binary "@$MATCH_FILE" \
    "$API_URL/api/t2stats/import")
  MATCH_CODE=$(echo "$MATCH_RESPONSE" | tail -1)
  MATCH_BODY=$(echo "$MATCH_RESPONSE" | head -n -1)

  MID=""
  if [ "$MATCH_CODE" = "200" ] || [ "$MATCH_CODE" = "409" ]; then
    MID=$(echo "$MATCH_BODY" | sed -n 's/.*mid=\([0-9]*\).*/\1/p')
  fi

  if [ -z "$MID" ]; then
    log "  FAIL: $prefix — could not get MID (HTTP $MATCH_CODE: $MATCH_BODY)"
    FAILED=$((FAILED + 1))
    continue
  fi

  vlog "  MID=$MID (HTTP $MATCH_CODE)"

  # Check status from API
  STATUS_RESPONSE=$(curl -s -w "\n%{http_code}" \
    -A "TribalOutpostStats" \
    -H "Authorization: Bearer $TOKEN" \
    "$API_URL/api/t2stats/import/$MID/status")
  STATUS_CODE=$(echo "$STATUS_RESPONSE" | tail -1)
  STATUS_BODY=$(echo "$STATUS_RESPONSE" | head -n -1)

  if [ "$STATUS_CODE" != "200" ]; then
    log "  FAIL: $prefix — status check failed (HTTP $STATUS_CODE)"
    FAILED=$((FAILED + 1))
    continue
  fi

  API_PLAYERS=$(echo "$STATUS_BODY" | sed -n 's/^players=\([0-9]*\)/\1/p')
  API_EXT=$(echo "$STATUS_BODY" | sed -n 's/^ext=\([0-9]*\)/\1/p')
  API_PLAYS=$(echo "$STATUS_BODY" | sed -n 's/^plays=\([0-9]*\)/\1/p')
  API_PLAYERS="${API_PLAYERS:-0}"
  API_EXT="${API_EXT:-0}"
  API_PLAYS="${API_PLAYS:-0}"

  # Check what local files exist and their line counts
  LOCAL_PLAYERS=0
  LOCAL_EXT=0
  LOCAL_PLAYS=0
  PLAYERS_FILE="$STATS_DIR/players/$prefix.players"
  EXT_FILE="$STATS_DIR/ext/$prefix.ext"
  PLAYS_FILE="$STATS_DIR/plays/$prefix.plays"
  [ -f "$PLAYERS_FILE" ] && LOCAL_PLAYERS=$(grep -c . "$PLAYERS_FILE" 2>/dev/null || echo 0)
  [ -f "$EXT_FILE" ] && LOCAL_EXT=$(grep -c . "$EXT_FILE" 2>/dev/null || echo 0)
  [ -f "$PLAYS_FILE" ] && LOCAL_PLAYS=$(grep -c . "$PLAYS_FILE" 2>/dev/null || echo 0)

  NEEDS_WORK=0
  SUBMIT_ARGS="--api $API_URL --token $TOKEN --mid $MID --name $prefix"

  # Check if each part needs resubmission
  if [ "$API_PLAYERS" = "0" ] && [ "$LOCAL_PLAYERS" -gt "0" ]; then
    vlog "  Players: API=$API_PLAYERS local=$LOCAL_PLAYERS — needs resubmit"
    NEEDS_WORK=1
  else
    SUBMIT_ARGS="$SUBMIT_ARGS --skip-players"
    vlog "  Players: OK ($API_PLAYERS)"
  fi

  if [ "$API_EXT" = "0" ] && [ "$LOCAL_EXT" -gt "0" ]; then
    vlog "  Ext: API=$API_EXT local=$LOCAL_EXT — needs resubmit"
    NEEDS_WORK=1
  else
    SUBMIT_ARGS="$SUBMIT_ARGS --skip-ext"
    vlog "  Ext: OK ($API_EXT)"
  fi

  if [ "$API_PLAYS" = "0" ] && [ "$LOCAL_PLAYS" -gt "0" ]; then
    vlog "  Plays: API=$API_PLAYS local=$LOCAL_PLAYS — needs resubmit"
    NEEDS_WORK=1
  else
    SUBMIT_ARGS="$SUBMIT_ARGS --skip-plays"
    vlog "  Plays: OK ($API_PLAYS)"
  fi

  if [ "$NEEDS_WORK" = "1" ]; then
    log "  Resubmitting: $prefix (mid=$MID) — API has players=$API_PLAYERS ext=$API_EXT plays=$API_PLAYS"
    if $SUBMIT $SUBMIT_ARGS "$STATS_DIR" 2>&1 | while read -r line; do vlog "    $line"; done; then
      SUBMITTED=$((SUBMITTED + 1))
    else
      FAILED=$((FAILED + 1))
      continue
    fi
  else
    ALREADY_DONE=$((ALREADY_DONE + 1))
  fi

  # Write .done marker
  echo "mid=$MID" > "$DONE_DIR/$prefix.done"
  vlog "  Done marker written"
done

log "Results: $PROCESSED processed, $SUBMITTED resubmitted, $ALREADY_DONE already complete, $FAILED failed"
