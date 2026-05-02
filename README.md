# TribalOutpost Stats

TorqueScript mod for Tribes 2 servers that collects match statistics and play-by-play events, reporting them to [TribalOutpost](https://tribaloutpost.com).

Originally by kryrand, rewritten by Sfphinx. Original made for tournament play in association with TWL and Branzone.

Most recent version is mostly a rewrite with AI to leverage DarkTiger's dtStats.cs collection and send them to the server via automation, with the addition of play-by-play metrics tracked and sent as well.

## How it works

- **Registration** — On first mission load, the script registers with the TribalOutpost API and receives an auth token. The token is saved to `TribalOutpostStats/token.txt` and reused across restarts. Stats record immediately but only become publicly visible after the server is approved in the admin UI.
- **Match reporting** — At game over, the script builds a `text/plain` body with `#MATCH` and `#PLAYER` sections and POSTs it to `/api/t2stats/import` with Bearer token auth.
- **Play-by-play** — During the match, events (kills, flag grabs/caps/drops/returns, connects/disconnects) are written to disk as pipe-delimited lines. After the match import succeeds, the script reads the plays file and sends them in batches (default 100 per batch) to `/api/t2stats/import/{mid}/plays`.

## Installation

You can install the mod two ways. Pick whichever fits your workflow.

### Option A: VL2

Drop `TribalOutpostStats.vl2` (download from tribaloutpost.com, or build it yourself) into `GameData/Base/`. Restart the server.

### Option B: Raw script

Copy `z_toStats.cs` from this repo directly into `GameData/Base/scripts/autoexec/`. Restart the server. The `z_` prefix ensures it loads after other autoexec scripts.

---

After either path, on first mission load the script registers with the API and writes `TribalOutpostStats/token.txt`. Confirm registration in the admin UI and approve the server (otherwise stats record but stay invisible).

Then drop the helper scripts (`scripts/stats-catchup.sh`, `scripts/submit-stats.sh`, `scripts/stats-backfill-match-id.sh`) somewhere stable on the server (e.g. `/opt/tribaloutpost/scripts/`) and install the catchup cron — see [Catchup cron](#catchup-cron) below.

## Configuration

```cs
$TribalOutpost::Debug = 0;                       // Debug logging
$TribalOutpost::StatsURL = "tribaloutpost.com";  // API host
$TribalOutpost::EnablePlayByPlay = 1;            // Enable play-by-play tracking
$TribalOutpost::PlayBatchSize = 100;             // Plays per HTTP request
$TribalOutpost::PlayBatchDelay = 1000;           // ms between batches
```

## Stats tracked

### Per player

| Stat | Description |
|------|-------------|
| score, kills, deaths, suicides, teamkills | Core combat stats |
| time | Time played in match (seconds) |
| caps, grabs, egrabs | Flag captures, grabs, enemy grabs |
| flagtime, flagdist, flagpercentdist | Flag carry time/distance/% of total |
| cappedflagtime, cappedflagdist, cappedflagpercentdist | Same but only for capped runs |
| gendestroys, sensordestroys, turretdestroys | Base asset destroys |
| invdestroys, vpaddestroys, solardestroys | Station destroys |
| sentrydestroys, depsensordestroys, depturretdestroys, depinvdestroys | Deployable destroys |
| flagdefends, gendefends | Defensive stats |
| flagkills, farmkills | Carrier kills, turret kills |
| returnpoints | Return points |
| captain | Was team captain (auto-detected) |

### Per match

Map, gametype, tournament mode, length, team names/scores, flag-to-flag distance.

### Play-by-play events

`Kill`, `TeamKill`, `SelfKill`, `Death`, `FlagGrab`, `FlagDrop`, `FlagCap`, `FlagReturn`, `PlayerConnect`, `PlayerDisconnect`, `MissionStart`, `MissionEnd` — each with timestamp, involved players, location, and weapon/damage type.

## API endpoints

| Endpoint | Method | Format | Auth |
|----------|--------|--------|------|
| `/api/t2stats/register` | POST | text/plain key=value | None |
| `/api/t2stats/import` | POST | text/plain #MATCH/#PLAYER sections | Bearer token |
| `/api/t2stats/import/{mid}/plays` | POST | text/plain pipe-delimited | Bearer token |

---

# Operator playbook

For game-server operators running `TribalOutpostStats`. Covers upgrading to 2.4.0, the catchup cron, verification, and troubleshooting.

## Upgrading an existing server to 2.4.0

Version 2.4.0 adds a `client_match_id` field to every `.match` file, generated from the per-second filename prefix (e.g. `26-04-25_143052_Katabatic`). The server uses it for deterministic dedupe — retries become safe regardless of how late they happen.

Run these in order. Steps 3–4 only need to happen once per server.

### 1. Drop in the new script

Either rebuild/replace `GameData/Base/TribalOutpostStats.vl2`, or copy the new `z_toStats.cs` over the old one in `GameData/Base/scripts/autoexec/`. The script auto-loads on the next mission load. New `.match` files will carry `client_match_id=` from this point forward.

### 2. Update the helper scripts

Pull or copy the latest from the repo:

- `scripts/stats-catchup.sh`
- `scripts/submit-stats.sh`
- `scripts/stats-backfill-match-id.sh` (new)

Place them somewhere stable on the server (e.g. `/opt/tribaloutpost/scripts/`).

### 3. Backfill legacy `.match` files

Any `.match` files written before the upgrade are missing `client_match_id`. The backfill script appends it from the filename. Idempotent — safe to re-run.

```bash
./scripts/stats-backfill-match-id.sh /path/to/TribalOutpostStats
```

Output:

```
Backfill complete: 47 added, 12 already present.
```

### 4. (Optional) Drain the existing queue once

If you have a long backlog of pending matches (`.match` without a corresponding `.done`), run the catchup once manually before enabling the cron:

```bash
./scripts/stats-catchup.sh \
  --max 200 \
  --verbose \
  /path/to/TribalOutpostStats
```

For matches that were already ingested but lost their `.done` marker, the server's heal logic will recognize them by fingerprint within 90 days and attach the new `client_match_id` to the existing row — no duplicates created. Older than 90 days, you may get one duplicate match per affected file; resolve via admin UI if needed.

### 5. Install the cron (see below)

---

## Catchup cron

The catchup script scans `TribalOutpostStats/matches/` for `.match` files without a matching `done/<prefix>.done` marker, queries the API to see what's actually missing for that match, and resubmits only the missing pieces. It writes the `.done` marker on success.

### Recommended cron entry

Every 5 minutes, processing up to 20 matches per run:

```cron
*/15 * * * * /opt/tribaloutpost/scripts/stats-catchup.sh --max 20 /home/t2server/Tribes2/GameData/Classic/TribalOutpostStats > /var/log/stats-catchup.log 2>&1
```

Adjust paths to your install. The `> /var/log/...` redirect captures both stdout and stderr; set to override last run to prevent log growth.

### Defaults (API URL + token)

For the default TribalOutpost server, the catchup script needs no extra flags beyond `--max` and the stats-dir path:

- **API URL** — defaults to `https://tribaloutpost.com`. Override with `--api <url>` only if pointing at a different server (e.g. local dev).
- **Token** — resolved in this order: `--token <token>` flag → `$TOKEN` env var → `<stats-dir>/token.txt` (auto-written by the game server on first registration).

The `token.txt` path is the right setup for cron — drop the cron entry in and it works.

### Tuning `--max`

- Default `20` is fine for a server that runs a handful of matches per day.
- For a backlog or a high-traffic server, raise to `100` or `200`.
- The cap exists to prevent a single cron run from monopolizing the network.

### Verbose mode for debugging

```bash
./scripts/stats-catchup.sh --verbose --dry-run /path/to/TribalOutpostStats
```

`--dry-run` shows what would be processed without sending anything. `--verbose` adds per-match detail (current API counts vs. local file counts, decision per slice).

---

## What the catchup script does

For each `.match` file without a `.done` marker:

1. POSTs the `.match` body to `/api/t2stats/import` to recover the MID.
   - With `client_match_id` present (post-upgrade), the server returns 409 with the existing MID if the match was already ingested. No duplicate row.
   - Without it (pre-upgrade legacy files), the server falls back to a 5-minute fingerprint window. The backfill script in step 3 above is what closes this gap.
2. GETs `/api/t2stats/import/{mid}/status` to see how many players, ext stats, and plays the server already has for that MID.
3. For each slice (players / ext / plays) the server has zero of but the local file has data for, resubmits that slice. The backing tables have unique constraints, so individual inserts are idempotent — partial retries don't double-count.
4. Writes `done/<prefix>.done` so the cron skips it next time.

If everything was already ingested, the script writes the `.done` marker and moves on. No work, no duplicates.

---

## Verification

After the upgrade, confirm things are healthy:

### On the game server

```bash
# Most recent .match should contain client_match_id=
tail -n +1 $(ls -t TribalOutpostStats/matches/*.match | head -1)

# Pending count (should approach zero with cron running)
comm -23 \
  <(ls TribalOutpostStats/matches/ | sed 's/\.match$//' | sort) \
  <(ls TribalOutpostStats/done/ 2>/dev/null | sed 's/\.done$//' | sort) \
  | wc -l

# Cron log
tail -n 50 /var/log/stats-catchup.log
```

### Server-side (operator check)

In the admin UI, look at the matches list for your server. After running catchup once on a backfilled queue, every recent match should have `client_match_id` populated. Confirm no obvious duplicates were created in the heal window.

---

## Troubleshooting

| Symptom | Likely cause | Fix |
|---|---|---|
| Catchup logs `FAIL: ... could not get MID (HTTP 401)` | Token missing or invalid | Check `token.txt` exists in stats-dir, or pass `--token` |
| Catchup logs `FAIL: ... (HTTP 404)` on status check | Match was ingested, then the `matches` row was deleted server-side | Delete the local `.done`-less files, or contact admin |
| Backfill says `0 added` and you expected more | `.match` files already have `client_match_id` (good — nothing to do) | No action |
| Duplicate match rows showing up after upgrade | Heal window is 90 days; matches older than that may dupe on first retry | Merge/delete via admin UI; future retries dedupe deterministically |
| Cron runs but no `.done` files appear | `done/` directory missing or unwritable | The script auto-creates it; check permissions on the stats-dir |

---

## File layout reference

```
TribalOutpostStats/
├── token.txt                     # Auth token (auto-written on first registration)
├── matches/<prefix>.match        # Match metadata — now includes client_match_id
├── players/<prefix>.players      # CSV per-player stats
├── ext/<prefix>.ext              # guid,key,value extension stats
├── plays/<prefix>.plays          # Pipe-delimited play-by-play events
└── done/<prefix>.done            # Marker written by catchup on successful ingest
```

`<prefix>` format: `yy-mm-dd_HHnnss_<map>` — generated by the script at mission start, shared across all four files for one match. This is also the value of `client_match_id`.

---

## License

MIT
