#!/usr/bin/env bash
# Submit or resubmit stats data to the t2stats API.
#
# Usage:
#   ./scripts/submit-stats.sh [options] <fixture-dir>
#   ./scripts/submit-stats.sh [options] --name <prefix> <stats-dir>
#
# Mode 1: Flat fixture directory (all files in one dir)
#   fixture-dir/
#   ├── *.match, *.players, *.ext, *.plays
#   └── token.txt
#
# Mode 2: TribalOutpostStats layout with --name
#   stats-dir/
#   ├── token.txt
#   ├── matches/<name>.match
#   ├── players/<name>.players
#   ├── ext/<name>.ext
#   └── plays/<name>.plays
#
# Options:
#   --name <prefix>    Match prefix name — resolves files from standard subdirectories
#   --mid <id>         Skip match submission, use this match ID for players/ext/plays
#   --api <url>        API base URL (default: http://localhost:4321)
#   --token <token>    Auth token (overrides token.txt and TOKEN env var)
#   --player-batch <n> Players per batch (default: 10)
#   --ext-batch <n>    Ext stats per batch (default: 200)
#   --play-batch <n>   Plays per batch (default: 200)
#   --skip-players     Skip player submission
#   --skip-ext         Skip ext stats submission
#   --skip-plays       Skip plays submission
#   --dry-run          Show what would be submitted without sending
#
# Examples:
#   # Submit from flat fixture directory
#   ./scripts/submit-stats.sh scripts/fixtures/cut-pug1
#
#   # Submit a specific match by name from TribalOutpostStats directory
#   ./scripts/submit-stats.sh --name 2026-03-28_203254_TWL2_Celerity ~/TribalOutpostStats
#
#   # Resubmit only players and ext stats to existing match
#   ./scripts/submit-stats.sh --mid 12109 --skip-plays scripts/fixtures/cut-pug1
#
#   # Submit to production
#   ./scripts/submit-stats.sh --api https://to.com --token abc123 --name 2026-03-28_Katabatic ~/TribalOutpostStats
#
set -uo pipefail

# Defaults
API_URL="http://localhost:4321"
PLAYER_BATCH_SIZE=10
EXT_BATCH_SIZE=200
PLAY_BATCH_SIZE=200
HINT_MID=""
SKIP_PLAYERS=0
SKIP_EXT=0
SKIP_PLAYS=0
DRY_RUN=0
CLI_TOKEN=""
FIXTURE_DIR=""
MATCH_NAME=""

# Parse args
while [[ $# -gt 0 ]]; do
  case "$1" in
    --name)       MATCH_NAME="$2"; shift 2 ;;
    --mid)        HINT_MID="$2"; shift 2 ;;
    --api)        API_URL="$2"; shift 2 ;;
    --token)      CLI_TOKEN="$2"; shift 2 ;;
    --player-batch) PLAYER_BATCH_SIZE="$2"; shift 2 ;;
    --ext-batch)  EXT_BATCH_SIZE="$2"; shift 2 ;;
    --play-batch) PLAY_BATCH_SIZE="$2"; shift 2 ;;
    --skip-players) SKIP_PLAYERS=1; shift ;;
    --skip-ext)   SKIP_EXT=1; shift ;;
    --skip-plays) SKIP_PLAYS=1; shift ;;
    --dry-run)    DRY_RUN=1; shift ;;
    -h|--help)
      sed -n '2,/^$/p' "$0" | sed 's/^# \?//'
      exit 0
      ;;
    *)            FIXTURE_DIR="$1"; shift ;;
  esac
done

if [ -z "$FIXTURE_DIR" ]; then
  echo "Usage: $0 [options] <fixture-dir>"
  echo "       $0 --help for full usage"
  exit 1
fi

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m'

log()  { echo -e "${CYAN}[stats]${NC} $*"; }
pass() { echo -e "${GREEN}[OK]${NC} $*"; }
fail() { echo -e "${RED}[FAIL]${NC} $*"; }
warn() { echo -e "${YELLOW}[WARN]${NC} $*"; }

# Find fixture files
if [ -n "$MATCH_NAME" ]; then
  # --name mode: resolve from standard TribalOutpostStats subdirectories
  MATCH_FILE="$FIXTURE_DIR/matches/$MATCH_NAME.match"
  PLAYERS_FILE="$FIXTURE_DIR/players/$MATCH_NAME.players"
  EXT_FILE="$FIXTURE_DIR/ext/$MATCH_NAME.ext"
  PLAYS_FILE="$FIXTURE_DIR/plays/$MATCH_NAME.plays"
  # Clear any that don't exist
  [ -f "$MATCH_FILE" ]   || MATCH_FILE=""
  [ -f "$PLAYERS_FILE" ] || PLAYERS_FILE=""
  [ -f "$EXT_FILE" ]     || EXT_FILE=""
  [ -f "$PLAYS_FILE" ]   || PLAYS_FILE=""
else
  # Flat directory mode: find first of each type
  MATCH_FILE=$(find "$FIXTURE_DIR" -name '*.match' -type f 2>/dev/null | head -1)
  PLAYERS_FILE=$(find "$FIXTURE_DIR" -name '*.players' -type f 2>/dev/null | head -1)
  EXT_FILE=$(find "$FIXTURE_DIR" -name '*.ext' -type f 2>/dev/null | head -1)
  PLAYS_FILE=$(find "$FIXTURE_DIR" -name '*.plays' -type f 2>/dev/null | head -1)
fi

# Resolve token: CLI flag > env var > token.txt in fixture dir
if [ -n "$CLI_TOKEN" ]; then
  TOKEN="$CLI_TOKEN"
elif [ -n "${TOKEN:-}" ]; then
  TOKEN="$TOKEN"
else
  TOKEN_FILE="$FIXTURE_DIR/token.txt"
  if [ -f "$TOKEN_FILE" ]; then
    TOKEN=$(cat "$TOKEN_FILE")
  else
    fail "No token found. Use --token, TOKEN env var, or place token.txt in fixture dir."
    exit 1
  fi
fi

# Validate we have something to submit
if [ -z "$HINT_MID" ] && [ -z "$MATCH_FILE" ]; then
  fail "No .match file found and no --mid specified. Nothing to do."
  exit 1
fi

# Print plan
log "API URL:     $API_URL"
log "Fixture dir: $FIXTURE_DIR"
if [ -n "$HINT_MID" ]; then
  log "Match ID:    $HINT_MID (provided, skipping match submission)"
else
  log "Match:       $MATCH_FILE"
fi
if [ -n "$PLAYERS_FILE" ] && [ "$SKIP_PLAYERS" = "0" ]; then log "Players:     $PLAYERS_FILE ($(wc -l < "$PLAYERS_FILE") lines, batch size $PLAYER_BATCH_SIZE)"; fi
if [ -n "$EXT_FILE" ] && [ "$SKIP_EXT" = "0" ]; then log "Ext stats:   $EXT_FILE ($(wc -l < "$EXT_FILE") lines, batch size $EXT_BATCH_SIZE)"; fi
if [ -n "$PLAYS_FILE" ] && [ "$SKIP_PLAYS" = "0" ]; then log "Plays:       $PLAYS_FILE ($(wc -l < "$PLAYS_FILE") lines, batch size $PLAY_BATCH_SIZE)"; fi
if [ "$DRY_RUN" = "1" ]; then warn "DRY RUN — no data will be sent"; fi
echo ""

# Helper: POST text/plain and capture response + status
post() {
  local url="$1"
  local body="$2"
  if [ "$DRY_RUN" = "1" ]; then
    echo "200|dry-run=true"
    return
  fi
  local response
  response=$(curl -s -w "\n%{http_code}" \
    -X POST \
    -H "Content-Type: text/plain" \
    -H "Authorization: Bearer $TOKEN" \
    -H "X-Stats-Version: 2.1.0" \
    --data-binary "$body" \
    "$url")
  local http_code
  http_code=$(echo "$response" | tail -1)
  local body_out
  body_out=$(echo "$response" | head -n -1)
  echo "$http_code|$body_out"
}

# Helper: send file in batches
send_batches() {
  local file="$1"
  local endpoint="$2"
  local batch_size="$3"
  local label="$4"
  local total_lines
  total_lines=$(grep -c . "$file" 2>/dev/null || echo 0)

  if [ "$total_lines" = "0" ]; then
    warn "$label: file is empty, skipping"
    return
  fi

  local offset=0
  local batch_num=0
  local total_ok=0

  while [ "$offset" -lt "$total_lines" ]; do
    batch_num=$((batch_num + 1))
    local batch
    batch=$(tail -n +"$((offset + 1))" "$file" | head -n "$batch_size")
    local lines_in_batch
    lines_in_batch=$(echo "$batch" | grep -c . 2>/dev/null || echo 0)

    local result
    result=$(post "$endpoint" "$batch")
    local code="${result%%|*}"
    local body="${result#*|}"

    if [ "$code" = "200" ]; then
      local ok_count
      ok_count=$(echo "$body" | sed -n 's/.*ok=\([0-9]*\).*/\1/p')
      ok_count="${ok_count:-?}"
      total_ok=$((total_ok + ok_count))
      pass "$label batch $batch_num: $lines_in_batch lines -> ok=$ok_count"
    else
      fail "$label batch $batch_num: $lines_in_batch lines -> HTTP $code - $body"
    fi

    offset=$((offset + lines_in_batch))
  done

  log "$label complete: $total_ok accepted from $total_lines lines"
  echo ""
}

# Step 1: Get or create match ID
MID="$HINT_MID"

if [ -z "$MID" ]; then
  log "Submitting match..."
  MATCH_RESULT=$(post "$API_URL/api/t2stats/import" "@$MATCH_FILE")
  MATCH_CODE="${MATCH_RESULT%%|*}"
  MATCH_BODY="${MATCH_RESULT#*|}"

  if [ "$MATCH_CODE" = "200" ]; then
    MID=$(echo "$MATCH_BODY" | sed -n 's/.*mid=\([0-9]*\).*/\1/p')
    pass "Match created: mid=$MID"
  elif [ "$MATCH_CODE" = "409" ]; then
    MID=$(echo "$MATCH_BODY" | sed -n 's/.*mid=\([0-9]*\).*/\1/p')
    warn "Match already exists: mid=$MID (resubmitting data to existing match)"
  else
    fail "Match submission failed: HTTP $MATCH_CODE - $MATCH_BODY"
    exit 1
  fi
  echo ""
fi

# Step 2: Submit players
if [ "$SKIP_PLAYERS" = "0" ] && [ -n "$PLAYERS_FILE" ] && [ -s "$PLAYERS_FILE" ]; then
  send_batches "$PLAYERS_FILE" "$API_URL/api/t2stats/import/$MID/players" "$PLAYER_BATCH_SIZE" "Players"
fi

# Step 3: Submit ext stats
if [ "$SKIP_EXT" = "0" ] && [ -n "$EXT_FILE" ] && [ -s "$EXT_FILE" ]; then
  send_batches "$EXT_FILE" "$API_URL/api/t2stats/import/$MID/stats" "$EXT_BATCH_SIZE" "Ext stats"
fi

# Step 4: Submit plays
if [ "$SKIP_PLAYS" = "0" ] && [ -n "$PLAYS_FILE" ] && [ -s "$PLAYS_FILE" ]; then
  send_batches "$PLAYS_FILE" "$API_URL/api/t2stats/import/$MID/plays" "$PLAY_BATCH_SIZE" "Plays"
fi

log "Done. mid=$MID"
log "View at: $API_URL/stats/matches/$MID"
