//
// TribalOutpost Stats v2.2.0 — Thin Reporting Layer
//
// Reads DarkTiger's dtStats stat objects and reports to the TribalOutpost API.
// Requires z_dtStats.cs to be loaded first (files load alphabetically in scripts/autoexec/).
//
// Place this file in Base/Scripts/autoexec folder and restart the server.
//
// Stats Script by Sfphinx
//
// License: MIT License
//

// Load optional config overrides (create this file to override defaults)
if (isFile("TribalOutpostStats/config.cs"))
	exec("TribalOutpostStats/config.cs");

// -- Configuration (override in TribalOutpostStats/config.cs) --
$TribalOutpost::Version = "2.2.0";
if ($TribalOutpost::StatsURL $= "") $TribalOutpost::StatsURL = "https://tribaloutpost.com";
if ($TribalOutpost::Debug $= "") $TribalOutpost::Debug = 0;
$TribalOutpost::RegisterPath = "/api/t2stats/register";
$TribalOutpost::ImportPath = "/api/t2stats/import";
$TribalOutpost::PlayersPath = "/api/t2stats/import/";
$TribalOutpost::ExtPath = "/api/t2stats/import/";
$TribalOutpost::PlaysPath = "/api/t2stats/import/";
$TribalOutpost::TokenFile = "TribalOutpostStats/token.txt";
if ($TribalOutpost::EnablePlayByPlay $= "") $TribalOutpost::EnablePlayByPlay = 1;
if ($TribalOutpost::PlayBatchSize $= "") $TribalOutpost::PlayBatchSize = 100;
if ($TribalOutpost::PlayBatchDelay $= "") $TribalOutpost::PlayBatchDelay = 1000;
if ($TribalOutpost::ExtBatchSize $= "") $TribalOutpost::ExtBatchSize = 100;

// -- Distance settings --
$T2Stats::NegativeDistance = 0;
$T2Stats::EGrabDistance = 90;

// -- Damage type names --
$DamageName[$DamageType::Blaster] = "Blaster";
$DamageName[$DamageType::Plasma] = "Plasma";
$DamageName[$DamageType::Bullet] = "Bullet";
$DamageName[$DamageType::Disc] = "Disc";
$DamageName[$DamageType::Grenade] = "Grenade";
$DamageName[$DamageType::Laser] = "Laser";
$DamageName[$DamageType::ELF] = "ELF";
$DamageName[$DamageType::Mortar] = "Mortar";
$DamageName[$DamageType::Missile] = "Missile";
$DamageName[$DamageType::ShockLance] = "ShockLance";
$DamageName[$DamageType::Mine] = "Mine";
$DamageName[$DamageType::Explosion] = "Explosion";
$DamageName[$DamageType::Impact] = "Impact";
$DamageName[$DamageType::Ground] = "Ground";
$DamageName[$DamageType::Turret] = "Turret";
$DamageName[$DamageType::PlasmaTurret] = "PlasmaTurret";
$DamageName[$DamageType::AATurret] = "AATurret";
$DamageName[$DamageType::ElfTurret] = "ElfTurret";
$DamageName[$DamageType::MortarTurret] = "MortarTurret";
$DamageName[$DamageType::MissileTurret] = "MissileTurret";
$DamageName[$DamageType::IndoorDepTurret] = "IndoorDepTurret";
$DamageName[$DamageType::OutdoorDepTurret] = "OutdoorDepTurret";
$DamageName[$DamageType::SentryTurret] = "SentryTurret";
$DamageName[$DamageType::OutOfBounds] = "OutOfBounds";
$DamageName[$DamageType::Lava] = "Lava";
$DamageName[$DamageType::ShrikeBlaster] = "ShrikeBlaster";
$DamageName[$DamageType::BellyTurret] = "BellyTurret";
$DamageName[$DamageType::BomberBombs] = "BomberBombs";
$DamageName[$DamageType::TankChaingun] = "TankChaingun";
$DamageName[$DamageType::TankMortar] = "TankMortar";
$DamageName[$DamageType::SatchelCharge] = "SatchelCharge";
$DamageName[$DamageType::MPBMissile] = "MPBMissile";
$DamageName[$DamageType::Lightning] = "Lightning";
$DamageName[$DamageType::VehicleSpawn] = "VehicleSpawn";
$DamageName[$DamageType::ForceFieldPowerup] = "ForceFieldPowerup";
$DamageName[$DamageType::Crash] = "Crash";
$DamageName[$DamageType::NexusCamping] = "NexusCamping";
$DamageName[$DamageType::Suicide] = "Suicide";

// -- Known extension stat keys to read from DT --
$T2Stats::ExtKeyCount = 0;
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "aaTurretDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "aaTurretKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "airTime";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "armorH";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "armorHH";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "armorHK";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "armorHL";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "armorHM";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "armorL";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "armorLH";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "armorLK";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "armorLL";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "armorLM";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "armorM";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "armorMH";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "armorMK";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "armorML";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "armorMM";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "assaultRD";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "assaultRK";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "assaultTankDes";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "assist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "assists";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "avgSpeed";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "bellyTurretDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "bellyTurretDmg";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "bellyTurretKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "blasterACC";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "blasterCom";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "blasterDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "blasterDmg";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "blasterHitDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "blasterHits";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "blasterHitSV";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "blasterKillDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "blasterKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "blasterMA";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "blasterMAHitDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "blasterMARatio";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "blasterReflectHit";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "blasterReflectKill";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "blasterShotsFired";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "bomberBombsDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "bomberBombsDmg";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "bomberBombsKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "bomberDes";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "bomberFlyerRD";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "bomberFlyerRK";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "Bonus";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "capEfficiency";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "carrierKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "cgACC";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "cgCom";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "cgDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "cgDmg";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "cgHitDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "cgHits";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "cgHitSV";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "cgKillDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "cgKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "cgMA";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "cgMAHitDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "cgShotsFired";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "chainKill";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "chatallCount";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "chatteamCount";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "cloakerKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "cloakersKilled";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "comboCount";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "concussFlag";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "concussHit";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "concussTaken";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "crashDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "crashKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "ctrlKKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "deadDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "deathKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "deaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "decupleChainKill";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "decupleKill";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "defenseScore";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "DefKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "DefKillsH";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "DefKillsL";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "DefKillsM";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "depInvRepairs";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "depInvyUse";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "depSensorDestroys";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "depSensorRepairs";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "depStationDestroys";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "depTurretDestroys";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "depTurretRepairs";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "destruction";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "discACC";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "discCom";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "discDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "discDmg";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "discDmgACC";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "discDmgHits";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "discHitDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "discHits";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "discHitSV";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "discJump";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "discKillDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "discKillGround";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "discKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "discMA";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "discMAHitDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "discMARatio";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "discReflectHit";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "discReflectKill";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "discShotsFired";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "distMov";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "doubleChainKill";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "doubleKill";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "efficiency";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "elfShotsFired";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "elfTurretDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "elfTurretKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "escortAssists";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "EVHitWep";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "EVKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "EVMAHit";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "explosionDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "explosionKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "firstKill";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "flagCaps";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "flagCatch";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "flagCatchSpeed";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "flagDefends";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "flagGrabAtStand";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "flagGrabs";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "flagReturns";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "flagTimeMin";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "flagTimeMS";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "flagToss";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "flagTossCatch";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "flareHit";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "flareKill";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "flipflopCount";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "flipFlopDefends";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "forceFieldPowerUpDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "forceFieldPowerUpKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "friendlyFire";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "genDefends";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "genDestroys";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "genRepairs";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "genSolRepairs";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "grabSpeed";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "gravCycleDes";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "grenadeACC";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "grenadeCom";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "grenadeDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "grenadeDmg";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "grenadeDmgACC";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "grenadeDmgHits";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "grenadeHitDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "grenadeHits";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "grenadeHitSV";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "grenadeKillDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "grenadeKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "grenadeMA";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "grenadeMAHitDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "grenadeMARatio";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "grenadeShotsFired";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "groundDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "groundKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "groundTime";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "hapcFlyerRD";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "hapcFlyerRK";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "hArmorTime";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "hatTricks";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "heavyTransportDes";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "heldTimeSec";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "hGrenadeACC";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "hGrenadeCom";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "hGrenadeDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "hGrenadeDmg";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "hGrenadeHitDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "hGrenadeHits";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "hGrenadeHitSV";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "hGrenadeKillDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "hGrenadeKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "hGrenadeMA";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "hGrenadeMAHitDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "hGrenadeShotsFired";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "idleTime";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "impactDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "impactKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "indoorDepTurretDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "indoorDepTurretDmg";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "indoorDepTurretKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "interceptedFlag";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "interceptFlagSpeed";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "interceptSpeed";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "inventoryDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "InventoryDep";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "inventoryKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "invyEatRepairPack";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "iStationDestroys";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "jammer";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "kdr";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "kickCount";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "killCounter";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "killerDiscJump";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "kills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "killStreak";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "KillStreakBonus";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "lagSpikes";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "lArmorTime";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "laserACC";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "laserCom";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "laserDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "laserDmg";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "laserHeadShot";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "laserHitDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "laserHits";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "laserHitSV";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "laserHSKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "laserKillDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "laserKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "laserMA";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "laserMAHitDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "laserMARatio";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "laserShotsFired";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "lastKill";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "lavaDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "lavaKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "leavemissionareaCount";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "lightningDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "lightningKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "lightningMAEVHits";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "lightningMAEVKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "lightningMAHits";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "lightningMAkills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "lossCount";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "maFlagCatch";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "maFlagCatchSpeed";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "maHitDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "maHitHeight";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "maHitSV";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "maHitVV";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "maInterceptedFlag";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mannedTurretKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mArmorTime";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mas";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "matchRunTime";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "maxSpeed";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "MidAir";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mineACC";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mineCom";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mineDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mineDmg";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mineHitDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mineHits";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mineHitVV";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mineKillDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mineKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mineMA";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mineMAHitDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "minePlusDisc";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "minePlusDiscKill";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mineShotsFired";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "missileACC";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "missileCom";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "missileDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "missileDmg";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "missileHitDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "missileHits";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "missileHitSV";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "missileKillDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "missileKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "missileMA";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "missileMAHitDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "missileShotsFired";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "missileTK";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "missileTurretDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "missileTurretKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mobileBaseRD";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mobileBaseRK";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "morepoints";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mortarACC";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mortarCom";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mortarDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mortarDmg";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mortarDmgACC";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mortarDmgHits";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mortarHitDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mortarHits";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mortarHitSV";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mortarKillDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mortarKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mortarMA";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mortarMAHitDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mortarShotsFired";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mortarTurretDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mortarTurretKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "MotionSensorDep";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "MPBDes";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mpbGlitch";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mpbtstationDestroys";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "mpbtstationRepairs";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "multiKill";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "nexusCampingDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "nexusCampingKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "nonupleChainKill";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "nonupleKill";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "nuclearKill";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "objScore";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "obstimeoutkickCount";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "octupleChainKill";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "octupleKill";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "offenseScore";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "OffKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "OffKillsH";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "OffKillsL";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "OffKillsM";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "outdoorDepTurretDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "outdoorDepTurretDmg";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "outdoorDepTurretKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "outOfBoundDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "outOfBoundKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "outOfBounds";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "packetLoss";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "packpickupCount";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "pingAvg";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "plasmaACC";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "plasmaCom";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "plasmaDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "plasmaDmg";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "plasmaDmgACC";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "plasmaDmgHits";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "plasmaHitDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "plasmaHits";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "plasmaHitSV";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "plasmaKillDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "plasmaKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "plasmaMA";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "plasmaMAHitDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "plasmaMARatio";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "plasmaShotsFired";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "plasmaTurretDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "plasmaTurretKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "PulseSensorDep";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "quadrupleChainKill";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "quadrupleKill";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "quintupleChainKill";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "quintupleKill";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "repairEnemy";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "repairpackpickupCount";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "repairpackpickupEnemy";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "repairs";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "returnPts";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "revenge";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "roadDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "roadDmg";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "roadKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "roundKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "roundsLost";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "roundsWon";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "satchelACC";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "satchelCom";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "satchelDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "satchelDmg";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "satchelHitDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "satchelHits";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "satchelHitVV";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "satchelKillDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "satchelKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "satchelMA";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "satchelShotsFired";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "score";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "scoreHeadshot";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "scoreMidAir";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "scoreRearshot";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "scoutFlyerRD";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "scoutFlyerRK";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "sensorDestroys";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "SensorRepairs";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "SensorsDep";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "sentryDestroys";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "sentryRepairs";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "sentryTurretDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "sentryTurretKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "septupleChainKill";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "septupleKill";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "sextupleChainKill";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "sextupleKill";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "shieldPackDmg";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "shockACC";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "shockCom";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "shockDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "shockDmg";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "shockHitDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "shockHits";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "shockHitSV";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "shockKillDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "shockKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "shockMA";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "shockMAHitDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "shockMARatio";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "shockRearShot";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "shockShotsFired";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "shotsFired";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "shrikeBlasterDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "shrikeBlasterDmg";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "shrikeBlasterKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "snipeKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "solarDestroys";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "solarRepairs";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "spawnobstimeoutCount";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "stalemateReturn";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "StationRepairs";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "suicides";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "switchteamCount";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "tankChaingunDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "tankChaingunDmg";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "tankChaingunKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "tankMortarDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "tankMortarDmg";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "tankMortarKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "teamkillCount";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "teamKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "teamOneCapTimes";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "teamTwoCapTimes";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "timeFarEnemyFS";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "timeFarTeamFS";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "timeNearEnemyFlag";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "timeNearEnemyFS";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "timeNearFlag";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "timeNearTeamFS";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "timeOnTeamOne";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "timeOnTeamTwo";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "timeOnTeamZero";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "timeTL";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "tkDestroys";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "totalChainAccuracy";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "totalChainHits";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "TotalDep";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "totalDistance";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "totalMA";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "totalShockHits";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "totalShocks";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "totalSnipeHits";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "totalSnipes";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "totalSpeed";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "totalTime";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "totalWepDmg";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "tripleChainKill";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "tripleKill";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "turbogravDes";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "turretDestroys";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "TurretIndoorDep";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "turretKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "TurretOutdoorDep";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "TurretRepairs";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "TurretsDep";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "vehicleBonus";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "vehicleScore";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "vehicleSpawnDeaths";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "vehicleSpawnKills";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "voicebindsallCount";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "voicebindsteamCount";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "voteCount";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "vstationDestroys";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "VStationRepairs";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "weaponHitDist";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "weaponpickupCount";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "wildRD";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "wildRK";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "winCount";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "winLostPct";
$T2Stats::ExtKey[$T2Stats::ExtKeyCount++] = "WLR";

// ============================================================
// Debug Logging
// ============================================================

$TribalOutpost::LogFile = "TribalOutpostStats/debug.log";

function tribaloutpost_log(%msg)
{
	echo("+++T2Stats: " @ %msg);
	if ($TribalOutpost::Debug)
	{
		%fo = new FileObject();
		%fo.openForAppend($TribalOutpost::LogFile);
		%fo.writeLine(getSimTime() @ " " @ %msg);
		%fo.close();
		%fo.delete();
	}
}

// ============================================================
// Token Persistence
// ============================================================

function tribaloutpost_loadToken()
{
	%fo = new FileObject();
	if (%fo.openForRead($TribalOutpost::TokenFile))
	{
		$TribalOutpost::Token = %fo.readLine();
		%fo.close();
		tribaloutpost_log("Token loaded.");
	}
	else
	{
		$TribalOutpost::Token = "";
		tribaloutpost_log("No token file found, will register.");
	}
	%fo.delete();
}

function tribaloutpost_saveToken(%token)
{
	// Ensure directory exists
	export("$TribalOutpost::_tmp", "TribalOutpostStats/empty", false);

	%fo = new FileObject();
	%fo.openForWrite($TribalOutpost::TokenFile);
	%fo.writeLine(%token);
	%fo.close();
	%fo.delete();

	$TribalOutpost::Token = %token;
	tribaloutpost_log("Token saved.");
}

// ============================================================
// Server Registration
// ============================================================

function tribaloutpost_register()
{
	tribaloutpost_log("Registering server...");

	%body = "server_name=" @ $Host::GameName @ "\nport=" @ $Host::Port @ "\nbind_address=" @ $pref::Net::BindAddress;

	if (isObject(T2StatsRegister))
	{
		T2StatsRegister.disconnect();
		T2StatsRegister.delete();
	}

	new HTTPObject(T2StatsRegister);

	T2StatsRegister.setHeader("Content-Type", "text/plain");
	T2StatsRegister.setHeader("Accept", "text/plain");
	T2StatsRegister.setHeader("X-Stats-Version", $TribalOutpost::Version);
	T2StatsRegister.post($TribalOutpost::StatsURL, $TribalOutpost::RegisterPath, "", %body);
}

function T2StatsRegister::onLine(%this, %line)
{
	tribaloutpost_log("Register response: " @ %line);

	if (strstr(%line, "token=") == 0)
	{
		%token = getSubStr(%line, 6, strlen(%line) - 6);
		tribaloutpost_saveToken(%token);
		tribaloutpost_log("Registered! Stats visible after admin approval.");
	}
}

function T2StatsRegister::onDisconnect(%this)
{
	if ($TribalOutpost::Token $= "")
		tribaloutpost_log("Registration failed. Will retry next map.");
	%this.delete();
}

function T2StatsRegister::onConnectFailed(%this)
{
	tribaloutpost_log("Could not connect for registration.");
	%this.delete();
}

function T2StatsRegister::onDNSFailed(%this)
{
	tribaloutpost_log("DNS failed for " @ $TribalOutpost::StatsURL);
	%this.delete();
}

// ============================================================
// Play-by-Play: Write to Disk During Match
// ============================================================

function tribaloutpost_initPlaysFile()
{
	// Generate a shared prefix for all files this match
	$T2Stats::FilePrefix = formatTimeString("yy-mm-dd_HHnnss") @ "_" @ $CurrentMission;

	$T2Stats::PlaysFile = "TribalOutpostStats/plays/" @ $T2Stats::FilePrefix @ ".plays";
	$T2Stats::PlaysCount = 0;

	// Ensure directories
	export("$TribalOutpost::_tmp", "TribalOutpostStats/plays/empty", false);
	export("$TribalOutpost::_tmp", "TribalOutpostStats/matches/empty", false);
	export("$TribalOutpost::_tmp", "TribalOutpostStats/players/empty", false);
	export("$TribalOutpost::_tmp", "TribalOutpostStats/ext/empty", false);

	// Create plays file with header
	%fo = new FileObject();
	%fo.openForWrite($T2Stats::PlaysFile);
	%fo.writeLine("#version=" @ $TribalOutpost::Version);
	%fo.writeLine("#match_id=" @ $T2Stats::FilePrefix);
	%fo.writeLine("#map=" @ $CurrentMission);
	%fo.writeLine("#gametype=" @ $CurrentMissionType);
	%fo.close();
	%fo.delete();

	// Reset dropped-player cache for this match
	$T2Stats::DroppedCount = 0;
}

// ============================================================
// Dropped Player Snapshot: Cache client-only fields lost on disconnect
// ============================================================
// dtStats preserves its ScriptObject (with all stat[] keys) in statsGroup
// after a player drops. At game over we read stats from there. But some
// fields only exist on the GameConnection object, so we cache those here.

function tribaloutpost_snapshotDroppedPlayer(%client)
{
	%idx = $T2Stats::DroppedCount;

	// Cap at 64 players per match
	if (%idx >= 64)
		return;

	// If this player was already snapshotted (reconnected then dropped again),
	// update the existing slot with their latest client-only data
	for (%c = 0; %c < %idx; %c++)
	{
		if ($T2Stats::Dropped[%c, "guid"] $= %client.guid)
		{
			%idx = %c;
			break;
		}
	}

	// Fields lost when the GameConnection is destroyed
	$T2Stats::Dropped[%idx, "guid"] = %client.guid;
	$T2Stats::Dropped[%idx, "nameBase"] = %client.nameBase;
	$T2Stats::Dropped[%idx, "name"] = StripMLControlChars(getTaggedString(%client.name));
	$T2Stats::Dropped[%idx, "clid"] = %client;
	$T2Stats::Dropped[%idx, "jointime"] = %client.jointime;
	$T2Stats::Dropped[%idx, "wasCaptain"] = (%client.wasCaptain $= "" ? 0 : %client.wasCaptain);
	$T2Stats::Dropped[%idx, "playersTeamed"] = (%client.playersTeamed $= "" ? 0 : %client.playersTeamed);

	// Engine flag-tracking properties — dtStats does not track these
	$T2Stats::Dropped[%idx, "flagTime"] = (%client.flagTime $= "" ? 0 : mFloor(%client.flagTime / 1000));
	$T2Stats::Dropped[%idx, "flagDist"] = (%client.flagDist $= "" ? 0 : %client.flagDist);
	$T2Stats::Dropped[%idx, "flagPercentDist"] = (%client.flagPercentDist $= "" ? 0 : %client.flagPercentDist);
	$T2Stats::Dropped[%idx, "cappedFlagTime"] = (%client.cappedFlagTime $= "" ? 0 : mFloor(%client.cappedFlagTime / 1000));
	$T2Stats::Dropped[%idx, "cappedFlagDist"] = (%client.cappedFlagDist $= "" ? 0 : %client.cappedFlagDist);
	$T2Stats::Dropped[%idx, "cappedFlagPercentDist"] = (%client.cappedFlagPercentDist $= "" ? 0 : %client.cappedFlagPercentDist);
	$T2Stats::Dropped[%idx, "eGrabs"] = (%client.eGrabs $= "" ? 0 : %client.eGrabs);

	// Only bump count if this was a new slot
	if (%idx == $T2Stats::DroppedCount)
		$T2Stats::DroppedCount = %idx + 1;

	tribaloutpost_log("Snapshotted dropped player: " @ %client.nameBase @ " (guid=" @ %client.guid @ ", clid=" @ %client @ ") idx=" @ %idx);
}

// Look up a dropped-player snapshot by guid. Returns index or -1.
function tribaloutpost_findDroppedPlayer(%guid)
{
	for (%i = 0; %i < $T2Stats::DroppedCount; %i++)
	{
		if ($T2Stats::Dropped[%i, "guid"] $= %guid)
			return %i;
	}
	return -1;
}

// Format: event|time|clid1|clid2|location|weapon|extra
// extra field: ma:1,d:85,vs:120,vh:25.3,va:L,ka:M
function tribaloutpost_writePlay(%event, %time, %clid1, %clid2, %location, %weapon, %extra)
{
	%line = %event @ "|" @ %time;
	%line = %line @ "|" @ %clid1;
	%line = %line @ "|" @ %clid2;
	%line = %line @ "|" @ %location;
	%line = %line @ "|" @ %weapon;
	if (%extra !$= "")
		%line = %line @ "|" @ %extra;

	%fo = new FileObject();
	%fo.openForAppend($T2Stats::PlaysFile);
	%fo.writeLine(%line);
	%fo.close();
	%fo.delete();

	$T2Stats::PlaysCount++;
}

// ============================================================
// Match Metadata: Write to Disk at Game Over
// ============================================================

function tribaloutpost_writeMatchFile(%game)
{
	$T2Stats::MatchFile = "TribalOutpostStats/matches/" @ $T2Stats::FilePrefix @ ".match";

	%elapsedTime = mFloor((getSimTime() - $missionStartTime) / 1000);

	%fo = new FileObject();
	%fo.openForWrite($T2Stats::MatchFile);

	// Match metadata as key=value (no #MATCH header, no #PLAYER — v2 format)
	%fo.writeLine("map=" @ $CurrentMission);
	%fo.writeLine("gametype=" @ $CurrentMissionType);
	%fo.writeLine("tournament=" @ $Host::TournamentMode);
	%fo.writeLine("length=" @ %elapsedTime);

	if (%game.numTeams > 1)
	{
		%fo.writeLine("team0_name=" @ getTaggedString(%game.getTeamName(1)));
		%fo.writeLine("team0_score=" @ ($TeamScore[1] $= "" ? 0 : $TeamScore[1]));
		%fo.writeLine("team1_name=" @ getTaggedString(%game.getTeamName(2)));
		%fo.writeLine("team1_score=" @ ($TeamScore[2] $= "" ? 0 : $TeamScore[2]));
	}
	else
	{
		%fo.writeLine("team0_name=");
		%fo.writeLine("team0_score=0");
		%fo.writeLine("team1_name=");
		%fo.writeLine("team1_score=0");
	}

	if (%game.class $= "CTFGame" || %game.class $= "LCTFGame" || %game.class $= "SCtFGame")
		%fo.writeLine("distance=" @ $CurrentMissionDistance);

	%fo.close();
	%fo.delete();

	tribaloutpost_log("Match file saved: " @ $T2Stats::MatchFile);
}

// ============================================================
// Player Stats: Read from DT and Write to Disk at Game Over
// ============================================================

function tribaloutpost_writePlayersFile(%game)
{
	$T2Stats::PlayersFile = "TribalOutpostStats/players/" @ $T2Stats::FilePrefix @ ".players";

	// Determine winning team
	%winningTeam = "";
	if (%game.numTeams > 1)
	{
		if (($TeamScore[1] + 0) > ($TeamScore[2] + 0))
			%winningTeam = getTaggedString(%game.getTeamName(1));
		else if (($TeamScore[2] + 0) > ($TeamScore[1] + 0))
			%winningTeam = getTaggedString(%game.getTeamName(2));
	}

	%fo = new FileObject();
	%fo.openForWrite($T2Stats::PlayersFile);

	%count = ClientGroup.getCount();
	for (%i = 0; %i < %count; %i++)
	{
		%cl = ClientGroup.getObject(%i);
		%elapsedTime = mFloor((getSimTime() - $missionStartTime) / 1000);
		%timeHere = mFloor((getSimTime() - %cl.jointime) / 1000);
		%time = (%elapsedTime < %timeHere) ? %elapsedTime : %timeHere;

		// Read DT stats if available — sum mid-airs from individual weapons
		// (postGameStats hasn't run yet, so totalMA/winCount/lossCount aren't computed)
		%dt = %cl.dtStats;
		%totalMA = 0;
		if (isObject(%dt))
		{
			%totalMA = (%dt.stat["discMA"] + 0) + (%dt.stat["grenadeMA"] + 0) +
				(%dt.stat["laserMA"] + 0) + (%dt.stat["mortarMA"] + 0) +
				(%dt.stat["shockMA"] + 0) + (%dt.stat["plasmaMA"] + 0) +
				(%dt.stat["blasterMA"] + 0) + (%dt.stat["hGrenadeMA"] + 0) +
				(%dt.stat["mineMA"] + 0);
		}

		// Compute win/loss from team scores
		%wincount = 0;
		%losscount = 0;
		if (%teamName !$= "" && %winningTeam !$= "")
		{
			if (%teamName $= %winningTeam)
				%wincount = 1;
			else
				%losscount = 1;
		}

		// Determine captain (player who teamed the most others)
		%captain = (%cl.wasCaptain $= "" ? 0 : %cl.wasCaptain);

		// Team name
		%teamName = "";
		if (%cl.team > 0)
			%teamName = getTaggedString(%game.getTeamName(%cl.team));

		// CSV line: guid,name,fullname,ip(reserved),clid,team,captain,time,score,kills,deaths,suicides,teamkills,
		//           flagtime,flagdist,flagpercentdist,cappedflagtime,cappedflagdist,cappedflagpercentdist,
		//           caps,grabs,egrabs,gendestroys,sensordestroys,turretdestroys,invdestroys,vpaddestroys,
		//           solardestroys,sentrydestroys,depsensordestroys,depturretdestroys,depinvdestroys,
		//           flagdefends,gendefends,flagkills,farmkills,returnpoints,midairs,wincount,losscount
		%line = %cl.guid;
		%line = %line @ "," @ %cl.nameBase;
		%line = %line @ "," @ StripMLControlChars(getTaggedString(%cl.name));
		%line = %line @ ",";
		%line = %line @ "," @ %cl;
		%line = %line @ "," @ %teamName;
		%line = %line @ "," @ %captain;
		%line = %line @ "," @ %time;
		%line = %line @ "," @ (%cl.score $= "" ? 0 : %cl.score);
		%line = %line @ "," @ (%cl.kills $= "" ? 0 : %cl.kills);
		%line = %line @ "," @ (%cl.deaths $= "" ? 0 : %cl.deaths);
		%line = %line @ "," @ (%cl.suicides $= "" ? 0 : %cl.suicides);
		%line = %line @ "," @ (%cl.teamKills $= "" ? 0 : %cl.teamKills);
		%line = %line @ "," @ (%cl.flagTime $= "" ? 0 : mFloor(%cl.flagTime / 1000));
		%line = %line @ "," @ (%cl.flagDist $= "" ? 0 : %cl.flagDist);
		%line = %line @ "," @ (%cl.flagPercentDist $= "" ? 0 : %cl.flagPercentDist);
		%line = %line @ "," @ (%cl.cappedFlagTime $= "" ? 0 : mFloor(%cl.cappedFlagTime / 1000));
		%line = %line @ "," @ (%cl.cappedFlagDist $= "" ? 0 : %cl.cappedFlagDist);
		%line = %line @ "," @ (%cl.cappedFlagPercentDist $= "" ? 0 : %cl.cappedFlagPercentDist);
		%line = %line @ "," @ (%cl.flagCaps $= "" ? 0 : %cl.flagCaps);
		%line = %line @ "," @ (%cl.flagGrabs $= "" ? 0 : %cl.flagGrabs);
		%line = %line @ "," @ (%cl.eGrabs $= "" ? 0 : %cl.eGrabs);
		%line = %line @ "," @ (%cl.genDestroys $= "" ? 0 : %cl.genDestroys);
		%line = %line @ "," @ (%cl.sensorDestroys $= "" ? 0 : %cl.sensorDestroys);
		%line = %line @ "," @ (%cl.turretDestroys $= "" ? 0 : %cl.turretDestroys);
		%line = %line @ "," @ (%cl.iStationDestroys $= "" ? 0 : %cl.iStationDestroys);
		%line = %line @ "," @ (%cl.vstationDestroys $= "" ? 0 : %cl.vstationDestroys);
		%line = %line @ "," @ (%cl.solarDestroys $= "" ? 0 : %cl.solarDestroys);
		%line = %line @ "," @ (%cl.sentryDestroys $= "" ? 0 : %cl.sentryDestroys);
		%line = %line @ "," @ (%cl.depSensorDestroys $= "" ? 0 : %cl.depSensorDestroys);
		%line = %line @ "," @ (%cl.depTurretDestroys $= "" ? 0 : %cl.depTurretDestroys);
		%line = %line @ "," @ (%cl.depStationDestroys $= "" ? 0 : %cl.depStationDestroys);
		%line = %line @ "," @ (%cl.flagDefends $= "" ? 0 : %cl.flagDefends);
		%line = %line @ "," @ (%cl.genDefends $= "" ? 0 : %cl.genDefends);
		%line = %line @ "," @ (%cl.carrierKills $= "" ? 0 : %cl.carrierKills);
		%line = %line @ "," @ (%cl.turretKills $= "" ? 0 : %cl.turretKills);
		%line = %line @ "," @ (%cl.returnPts $= "" ? 0 : %cl.returnPts);
		%line = %line @ "," @ %totalMA;
		%line = %line @ "," @ %wincount;
		%line = %line @ "," @ %losscount;

		%fo.writeLine(%line);
	}

	// Write dropped players — iterate statsGroup for clientLeft entries,
	// read game stats from dtStats ScriptObject, client-only fields from snapshot
	%droppedWritten = 0;
	if (isObject(statsGroup))
	{
		for (%s = 0; %s < statsGroup.getCount(); %s++)
		{
			%dt = statsGroup.getObject(%s);
			if (!%dt.clientLeft)
				continue;

			// Skip if this player reconnected and is still in ClientGroup
			%skipDrop = false;
			for (%j = 0; %j < %count; %j++)
			{
				%existing = ClientGroup.getObject(%j);
				if (%existing.guid $= %dt.guid)
				{
					%skipDrop = true;
					break;
				}
			}
			if (%skipDrop)
				continue;

			// Look up our snapshot for client-only fields — if no snapshot exists,
			// this dtStats entry may be stale from a previous map, skip it
			%snap = tribaloutpost_findDroppedPlayer(%dt.guid);
			if (%snap < 0)
				continue;

			// Compute play time from dtStats join/left times
			%elapsedTime = mFloor((getSimTime() - $missionStartTime) / 1000);
			%timeHere = mFloor((%dt.leftTime - %dt.joinTime) / 1000);
			%time = (%elapsedTime < %timeHere) ? %elapsedTime : %timeHere;

			// Sum mid-airs from individual weapon MA stats (same as connected players)
			%totalMA = (%dt.stat["discMA"] + 0) + (%dt.stat["grenadeMA"] + 0) +
				(%dt.stat["laserMA"] + 0) + (%dt.stat["mortarMA"] + 0) +
				(%dt.stat["shockMA"] + 0) + (%dt.stat["plasmaMA"] + 0) +
				(%dt.stat["blasterMA"] + 0) + (%dt.stat["hGrenadeMA"] + 0) +
				(%dt.stat["mineMA"] + 0);

			// Team name from dtStats team number
			%teamName = "";
			if (%dt.team > 0)
				%teamName = getTaggedString(%game.getTeamName(%dt.team));

			// Snapshot = client-only fields, dtStats = game stats
			%line = %dt.guid;
			%line = %line @ "," @ $T2Stats::Dropped[%snap, "nameBase"];
			%line = %line @ "," @ $T2Stats::Dropped[%snap, "name"];
			%line = %line @ ",";
			%line = %line @ "," @ $T2Stats::Dropped[%snap, "clid"];
			%line = %line @ "," @ %teamName;
			%line = %line @ "," @ $T2Stats::Dropped[%snap, "wasCaptain"];
			%line = %line @ "," @ %time;
			%line = %line @ "," @ (%dt.stat["score"] + 0);
			%line = %line @ "," @ (%dt.stat["kills"] + 0);
			%line = %line @ "," @ (%dt.stat["deaths"] + 0);
			%line = %line @ "," @ (%dt.stat["suicides"] + 0);
			%line = %line @ "," @ (%dt.stat["teamKills"] + 0);
			// Flag time/dist/percent — engine properties not tracked by dtStats
			%line = %line @ "," @ $T2Stats::Dropped[%snap, "flagTime"];
			%line = %line @ "," @ $T2Stats::Dropped[%snap, "flagDist"];
			%line = %line @ "," @ $T2Stats::Dropped[%snap, "flagPercentDist"];
			%line = %line @ "," @ $T2Stats::Dropped[%snap, "cappedFlagTime"];
			%line = %line @ "," @ $T2Stats::Dropped[%snap, "cappedFlagDist"];
			%line = %line @ "," @ $T2Stats::Dropped[%snap, "cappedFlagPercentDist"];
			%line = %line @ "," @ (%dt.stat["flagCaps"] + 0);
			%line = %line @ "," @ (%dt.stat["flagGrabs"] + 0);
			%line = %line @ "," @ $T2Stats::Dropped[%snap, "eGrabs"];
			%line = %line @ "," @ (%dt.stat["genDestroys"] + 0);
			%line = %line @ "," @ (%dt.stat["sensorDestroys"] + 0);
			%line = %line @ "," @ (%dt.stat["turretDestroys"] + 0);
			%line = %line @ "," @ (%dt.stat["iStationDestroys"] + 0);
			%line = %line @ "," @ (%dt.stat["vstationDestroys"] + 0);
			%line = %line @ "," @ (%dt.stat["solarDestroys"] + 0);
			%line = %line @ "," @ (%dt.stat["sentryDestroys"] + 0);
			%line = %line @ "," @ (%dt.stat["depSensorDestroys"] + 0);
			%line = %line @ "," @ (%dt.stat["depTurretDestroys"] + 0);
			%line = %line @ "," @ (%dt.stat["depStationDestroys"] + 0);
			%line = %line @ "," @ (%dt.stat["flagDefends"] + 0);
			%line = %line @ "," @ (%dt.stat["genDefends"] + 0);
			%line = %line @ "," @ (%dt.stat["carrierKills"] + 0);
			%line = %line @ "," @ (%dt.stat["turretKills"] + 0);
			%line = %line @ "," @ (%dt.stat["returnPts"] + 0);
			%line = %line @ "," @ %totalMA;
			// Compute win/loss from team scores
			%dWincount = 0;
			%dLosscount = 0;
			if (%teamName !$= "" && %winningTeam !$= "")
			{
				if (%teamName $= %winningTeam)
					%dWincount = 1;
				else
					%dLosscount = 1;
			}
			%line = %line @ "," @ %dWincount;
			%line = %line @ "," @ %dLosscount;

			%fo.writeLine(%line);
			%droppedWritten++;
		}
	}

	%fo.close();
	%fo.delete();

	tribaloutpost_log("Players file saved: " @ $T2Stats::PlayersFile @ " (" @ %count @ " connected + " @ %droppedWritten @ " dropped)");
}

// ============================================================
// Extension Stats: Read from DT stat objects
// ============================================================

function tribaloutpost_writeExtFile(%game)
{
	$T2Stats::ExtFile = "TribalOutpostStats/ext/" @ $T2Stats::FilePrefix @ ".ext";

	%fo = new FileObject();
	%fo.openForWrite($T2Stats::ExtFile);

	%count = ClientGroup.getCount();
	%written = 0;
	for (%i = 0; %i < %count; %i++)
	{
		%cl = ClientGroup.getObject(%i);
		%dt = %cl.dtStats;
		if (!isObject(%dt))
			continue;

		for (%k = 0; %k < $T2Stats::ExtKeyCount; %k++)
		{
			%key = $T2Stats::ExtKey[%k];
			%val = %dt.stat[%key] + 0;
			if (%val != 0)
			{
				%fo.writeLine(%cl.guid @ "," @ %key @ "," @ %val);
				%written++;
			}
		}
	}

	// Write dropped players' ext stats from dtStats ScriptObjects in statsGroup
	if (isObject(statsGroup))
	{
		for (%s = 0; %s < statsGroup.getCount(); %s++)
		{
			%dt = statsGroup.getObject(%s);
			if (!%dt.clientLeft)
				continue;

			// Must have a snapshot to confirm this player was in this match
			if (tribaloutpost_findDroppedPlayer(%dt.guid) < 0)
				continue;

			// Skip if this player reconnected and is still in ClientGroup
			%skipDrop = false;
			for (%j = 0; %j < %count; %j++)
			{
				%existing = ClientGroup.getObject(%j);
				if (%existing.guid $= %dt.guid)
				{
					%skipDrop = true;
					break;
				}
			}
			if (%skipDrop)
				continue;

			for (%k = 0; %k < $T2Stats::ExtKeyCount; %k++)
			{
				%key = $T2Stats::ExtKey[%k];
				%val = %dt.stat[%key] + 0;
				if (%val != 0)
				{
					%fo.writeLine(%dt.guid @ "," @ %key @ "," @ %val);
					%written++;
				}
			}
		}
	}

	%fo.close();
	%fo.delete();

	tribaloutpost_log("Ext stats file saved: " @ $T2Stats::ExtFile @ " (" @ %written @ " entries)");
}

// ============================================================
// HTTP Submission Chain
// ============================================================
//
// Each gameOver creates a submission with a unique ID ($T2Stats::SubmitSeq).
// All state for that submission is keyed by the ID so concurrent
// submissions from rapid map changes don't clobber each other.
//

// Step 1: Submit match metadata → returns mid
function tribaloutpost_submitMatch()
{
	if ($TribalOutpost::Token $= "")
	{
		tribaloutpost_log("No token, cannot submit. Data saved locally.");
		return;
	}

	// Allocate a unique submission ID
	if ($T2Stats::SubmitSeq $= "") $T2Stats::SubmitSeq = 0;
	$T2Stats::SubmitSeq++;
	%sid = $T2Stats::SubmitSeq;

	// Snapshot file paths for this submission
	$T2Stats::Sub[%sid, "matchFile"] = $T2Stats::MatchFile;
	$T2Stats::Sub[%sid, "playersFile"] = $T2Stats::PlayersFile;
	$T2Stats::Sub[%sid, "extFile"] = $T2Stats::ExtFile;
	$T2Stats::Sub[%sid, "playsFile"] = $T2Stats::PlaysFile;
	$T2Stats::Sub[%sid, "playsCount"] = $T2Stats::PlaysCount;
	$T2Stats::Sub[%sid, "mid"] = "";

	tribaloutpost_log("[" @ %sid @ "] Submitting match...");

	// Read match file
	%fo = new FileObject();
	if (!%fo.openForRead($T2Stats::Sub[%sid, "matchFile"]))
	{
		tribaloutpost_log("[" @ %sid @ "] Could not read match file.");
		%fo.delete();
		return;
	}

	%body = "";
	while (!%fo.isEOF())
	{
		%line = %fo.readLine();
		if (%body !$= "")
			%body = %body @ "\n";
		%body = %body @ %line;
	}
	%fo.close();
	%fo.delete();

	if (isObject(T2StatsImport))
	{
		T2StatsImport.disconnect();
		T2StatsImport.delete();
	}

	new HTTPObject(T2StatsImport);
	T2StatsImport.submitId = %sid;

	T2StatsImport.setHeader("Content-Type", "text/plain");
	T2StatsImport.setHeader("Accept", "text/plain");
	T2StatsImport.setHeader("Authorization", "Bearer " @ $TribalOutpost::Token);
	T2StatsImport.setHeader("X-Stats-Version", $TribalOutpost::Version);
	T2StatsImport.post($TribalOutpost::StatsURL, $TribalOutpost::ImportPath, "", %body);
}

function T2StatsImport::onLine(%this, %line)
{
	%sid = %this.submitId;
	tribaloutpost_log("[" @ %sid @ "] Import response: " @ %line);

	if (strstr(%line, "mid=") == 0)
	{
		%mid = getSubStr(%line, 4, strlen(%line) - 4);
		$T2Stats::Sub[%sid, "mid"] = %mid;
		$T2Stats::LastMID = %mid;
		tribaloutpost_log("[" @ %sid @ "] Match recorded (mid=" @ %mid @ ")");

		// Chain: send players → ext stats → plays
		schedule($TribalOutpost::PlayBatchDelay, 0, "tribaloutpost_sendPlayerBatch", %sid, 0);
	}

	if (strstr(%line, "error=") == 0)
		tribaloutpost_log("[" @ %sid @ "] Error: " @ getSubStr(%line, 6, strlen(%line) - 6));
}

function T2StatsImport::onDisconnect(%this)
{
	%this.delete();
}

function T2StatsImport::onConnectFailed(%this)
{
	tribaloutpost_log("[" @ %this.submitId @ "] Connection failed for match import.");
	%this.delete();
}

function T2StatsImport::onDNSFailed(%this)
{
	tribaloutpost_log("[" @ %this.submitId @ "] DNS failed for match import.");
	%this.delete();
}

// Step 2: Send player stats
function tribaloutpost_sendPlayerBatch(%sid, %lineOffset)
{
	%mid = $T2Stats::Sub[%sid, "mid"];
	if (%mid $= "")
		return;

	%fo = new FileObject();
	if (!%fo.openForRead($T2Stats::Sub[%sid, "playersFile"]))
	{
		tribaloutpost_log("[" @ %sid @ "] Could not read players file.");
		%fo.delete();
		// Skip to ext stats
		schedule($TribalOutpost::PlayBatchDelay, 0, "tribaloutpost_sendExtBatch", %sid, 0);
		return;
	}

	// Skip to offset
	%currentLine = 0;
	while (%currentLine < %lineOffset && !%fo.isEOF())
	{
		%fo.readLine();
		%currentLine++;
	}

	// Read batch (10 players per batch)
	%body = "";
	%linesRead = 0;
	while (%linesRead < 10 && !%fo.isEOF())
	{
		%line = %fo.readLine();
		if (%line !$= "")
		{
			if (%linesRead > 0)
				%body = %body @ "\n";
			%body = %body @ %line;
			%linesRead++;
		}
	}

	%hasMore = !%fo.isEOF();
	%fo.close();
	%fo.delete();

	if (%linesRead == 0)
	{
		// No more players, move to ext stats
		schedule($TribalOutpost::PlayBatchDelay, 0, "tribaloutpost_sendExtBatch", %sid, 0);
		return;
	}

	tribaloutpost_log("[" @ %sid @ "] Sending player batch, offset=" @ %lineOffset @ " lines=" @ %linesRead);

	$T2Stats::Sub[%sid, "playerOffset"] = %lineOffset + %linesRead;
	$T2Stats::Sub[%sid, "playerHasMore"] = %hasMore;

	if (isObject(T2StatsPlayers))
	{
		T2StatsPlayers.disconnect();
		T2StatsPlayers.delete();
	}

	new HTTPObject(T2StatsPlayers);
	T2StatsPlayers.submitId = %sid;

	T2StatsPlayers.setHeader("Content-Type", "text/plain");
	T2StatsPlayers.setHeader("Accept", "text/plain");
	T2StatsPlayers.setHeader("Authorization", "Bearer " @ $TribalOutpost::Token);
	T2StatsPlayers.setHeader("X-Stats-Version", $TribalOutpost::Version);
	T2StatsPlayers.post($TribalOutpost::StatsURL, $TribalOutpost::PlayersPath @ %mid @ "/players", "", %body);
}

function T2StatsPlayers::onLine(%this, %line)
{
	tribaloutpost_log("[" @ %this.submitId @ "] Players response: " @ %line);
}

function T2StatsPlayers::onDisconnect(%this)
{
	%sid = %this.submitId;
	if ($T2Stats::Sub[%sid, "playerHasMore"])
		schedule($TribalOutpost::PlayBatchDelay, 0, "tribaloutpost_sendPlayerBatch", %sid, $T2Stats::Sub[%sid, "playerOffset"]);
	else
	{
		tribaloutpost_log("[" @ %sid @ "] All player data sent.");
		schedule($TribalOutpost::PlayBatchDelay, 0, "tribaloutpost_sendExtBatch", %sid, 0);
	}
	%this.delete();
}

function T2StatsPlayers::onConnectFailed(%this)
{
	%sid = %this.submitId;
	tribaloutpost_log("[" @ %sid @ "] Connection failed for player batch.");
	// Try ext stats anyway
	schedule($TribalOutpost::PlayBatchDelay, 0, "tribaloutpost_sendExtBatch", %sid, 0);
	%this.delete();
}

function T2StatsPlayers::onDNSFailed(%this)
{
	tribaloutpost_log("[" @ %this.submitId @ "] DNS failed for player batch.");
	%this.delete();
}

// Step 3: Send extension stats
function tribaloutpost_sendExtBatch(%sid, %lineOffset)
{
	%mid = $T2Stats::Sub[%sid, "mid"];
	if (%mid $= "")
		return;

	%fo = new FileObject();
	if (!%fo.openForRead($T2Stats::Sub[%sid, "extFile"]))
	{
		tribaloutpost_log("[" @ %sid @ "] Could not read ext file.");
		%fo.delete();
		// Skip to plays
		if ($TribalOutpost::EnablePlayByPlay && $T2Stats::Sub[%sid, "playsCount"] > 0)
			schedule($TribalOutpost::PlayBatchDelay, 0, "tribaloutpost_sendPlayBatch", %sid, 0);
		return;
	}

	// Skip to offset
	%currentLine = 0;
	while (%currentLine < %lineOffset && !%fo.isEOF())
	{
		%fo.readLine();
		%currentLine++;
	}

	// Read batch
	%body = "";
	%linesRead = 0;
	while (%linesRead < $TribalOutpost::ExtBatchSize && !%fo.isEOF())
	{
		%line = %fo.readLine();
		if (%line !$= "")
		{
			if (%linesRead > 0)
				%body = %body @ "\n";
			%body = %body @ %line;
			%linesRead++;
		}
	}

	%hasMore = !%fo.isEOF();
	%fo.close();
	%fo.delete();

	if (%linesRead == 0)
	{
		// No more ext stats, move to plays
		if ($TribalOutpost::EnablePlayByPlay && $T2Stats::Sub[%sid, "playsCount"] > 0)
			schedule($TribalOutpost::PlayBatchDelay, 0, "tribaloutpost_sendPlayBatch", %sid, 0);
		else
			tribaloutpost_log("[" @ %sid @ "] Submission complete.");
		return;
	}

	tribaloutpost_log("[" @ %sid @ "] Sending ext batch, offset=" @ %lineOffset @ " lines=" @ %linesRead);

	$T2Stats::Sub[%sid, "extOffset"] = %lineOffset + %linesRead;
	$T2Stats::Sub[%sid, "extHasMore"] = %hasMore;

	if (isObject(T2StatsExt))
	{
		T2StatsExt.disconnect();
		T2StatsExt.delete();
	}

	new HTTPObject(T2StatsExt);
	T2StatsExt.submitId = %sid;

	T2StatsExt.setHeader("Content-Type", "text/plain");
	T2StatsExt.setHeader("Accept", "text/plain");
	T2StatsExt.setHeader("Authorization", "Bearer " @ $TribalOutpost::Token);
	T2StatsExt.setHeader("X-Stats-Version", $TribalOutpost::Version);
	T2StatsExt.post($TribalOutpost::StatsURL, $TribalOutpost::ExtPath @ %mid @ "/stats", "", %body);
}

function T2StatsExt::onLine(%this, %line)
{
	tribaloutpost_log("[" @ %this.submitId @ "] Ext response: " @ %line);
}

function T2StatsExt::onDisconnect(%this)
{
	%sid = %this.submitId;
	if ($T2Stats::Sub[%sid, "extHasMore"])
		schedule($TribalOutpost::PlayBatchDelay, 0, "tribaloutpost_sendExtBatch", %sid, $T2Stats::Sub[%sid, "extOffset"]);
	else
	{
		tribaloutpost_log("[" @ %sid @ "] All ext stats sent.");
		if ($TribalOutpost::EnablePlayByPlay && $T2Stats::Sub[%sid, "playsCount"] > 0)
			schedule($TribalOutpost::PlayBatchDelay, 0, "tribaloutpost_sendPlayBatch", %sid, 0);
		else
			tribaloutpost_log("[" @ %sid @ "] Submission complete.");
	}
	%this.delete();
}

function T2StatsExt::onConnectFailed(%this)
{
	%sid = %this.submitId;
	tribaloutpost_log("[" @ %sid @ "] Connection failed for ext batch.");
	// Try plays anyway
	if ($TribalOutpost::EnablePlayByPlay && $T2Stats::Sub[%sid, "playsCount"] > 0)
		schedule($TribalOutpost::PlayBatchDelay, 0, "tribaloutpost_sendPlayBatch", %sid, 0);
	%this.delete();
}

function T2StatsExt::onDNSFailed(%this)
{
	tribaloutpost_log("[" @ %this.submitId @ "] DNS failed for ext batch.");
	%this.delete();
}

// Step 4: Send play-by-play batches
function tribaloutpost_sendPlayBatch(%sid, %lineOffset)
{
	%mid = $T2Stats::Sub[%sid, "mid"];
	if (%mid $= "")
	{
		tribaloutpost_log("[" @ %sid @ "] No MID, skipping play-by-play.");
		return;
	}

	%fo = new FileObject();
	if (!%fo.openForRead($T2Stats::Sub[%sid, "playsFile"]))
	{
		tribaloutpost_log("[" @ %sid @ "] Could not read plays file.");
		%fo.delete();
		return;
	}

	// Skip to offset
	%currentLine = 0;
	while (%currentLine < %lineOffset && !%fo.isEOF())
	{
		%fo.readLine();
		%currentLine++;
	}

	// Read batch
	%body = "";
	%linesRead = 0;
	while (%linesRead < $TribalOutpost::PlayBatchSize && !%fo.isEOF())
	{
		%line = %fo.readLine();
		if (%line !$= "")
		{
			if (%linesRead > 0)
				%body = %body @ "\n";
			%body = %body @ %line;
			%linesRead++;
		}
	}

	%hasMore = !%fo.isEOF();
	%fo.close();
	%fo.delete();

	if (%linesRead == 0)
	{
		tribaloutpost_log("[" @ %sid @ "] All play-by-play sent. Submission complete.");
		return;
	}

	tribaloutpost_log("[" @ %sid @ "] Sending play batch, offset=" @ %lineOffset @ " lines=" @ %linesRead);

	$T2Stats::Sub[%sid, "playOffset"] = %lineOffset + %linesRead;
	$T2Stats::Sub[%sid, "playHasMore"] = %hasMore;

	if (isObject(T2StatsPlays))
	{
		T2StatsPlays.disconnect();
		T2StatsPlays.delete();
	}

	new HTTPObject(T2StatsPlays);
	T2StatsPlays.submitId = %sid;

	T2StatsPlays.setHeader("Content-Type", "text/plain");
	T2StatsPlays.setHeader("Accept", "text/plain");
	T2StatsPlays.setHeader("Authorization", "Bearer " @ $TribalOutpost::Token);
	T2StatsPlays.setHeader("X-Stats-Version", $TribalOutpost::Version);
	T2StatsPlays.post($TribalOutpost::StatsURL, $TribalOutpost::PlaysPath @ %mid @ "/plays", "", %body);
}

function T2StatsPlays::onLine(%this, %line)
{
	tribaloutpost_log("[" @ %this.submitId @ "] Plays response: " @ %line);
}

function T2StatsPlays::onDisconnect(%this)
{
	%sid = %this.submitId;
	if ($T2Stats::Sub[%sid, "playHasMore"])
		schedule($TribalOutpost::PlayBatchDelay, 0, "tribaloutpost_sendPlayBatch", %sid, $T2Stats::Sub[%sid, "playOffset"]);
	else
		tribaloutpost_log("[" @ %sid @ "] All play-by-play sent. Submission complete.");

	%this.delete();
}

function T2StatsPlays::onConnectFailed(%this)
{
	tribaloutpost_log("[" @ %this.submitId @ "] Connection failed for play-by-play batch.");
	%this.delete();
}

function T2StatsPlays::onDNSFailed(%this)
{
	tribaloutpost_log("DNS failed for play-by-play batch.");
	%this.delete();
}

// ============================================================
// Enriched Kill Event Handler
// ============================================================

function tribaloutpost_handleKillStat(%game, %victim, %killer, %damageType, %implement, %damageLocation)
{
	if (%damageType == $DamageType::Impact)
	{
		if ((%controller = %implement.getControllingClient()) > 0)
			%killer = %controller;
		else
			%killer = 0;
	}
	else if (isObject(%implement) && (%implement.getClassName() $= "Turret" || %implement.getClassName() $= "VehicleTurret" || %implement.getClassName() $= "FlyingVehicle" || %implement.getClassName() $= "HoverVehicle"))
	{
		if (%implement.getControllingClient() != 0)
			%killer = %implement.getControllingClient();
		else if (isObject(%implement.owner))
			%killer = %implement.owner;
		else
			%killer = 0;
	}

	// Build enriched extra field from DT data
	%extra = "";
	if (isObject(%killer) && isObject(%victim))
	{
		%victimPlayer = isObject(%victim.player) ? %victim.player : %victim.lastPlayer;
		%killerPlayer = isObject(%killer.player) ? %killer.player : %killer.lastPlayer;

		if (isObject(%victimPlayer) && isObject(%killerPlayer))
		{
			// Mid-air detection (uses DT's rayTest function)
			%isMidair = rayTest(%victimPlayer, 10);
			if (%isMidair)
				%extra = "ma:1";

			// Distance between killer and victim
			%dis = mFloor(vectorDist(%killerPlayer.getPosition(), %victimPlayer.getPosition()));
			%extra = (%extra $= "") ? ("d:" @ %dis) : (%extra @ ",d:" @ %dis);

			// Victim speed (km/h)
			%vs = mFloor(vectorLen(%victimPlayer.getVelocity()) * 3.6);
			%extra = %extra @ ",vs:" @ %vs;

			// Victim height above ground (uses DT's rayTestDis function)
			%vh = rayTestDis(%victimPlayer);
			if (%vh > 0)
				%extra = %extra @ ",vh:" @ %vh;

			// Armor sizes
			%va = %victimPlayer.getArmorSize();
			%ka = %killerPlayer.getArmorSize();
			if (%va !$= "")
			{
				// Abbreviate: Light→L, Medium→M, Heavy→H
				if (%va $= "Light") %va = "L";
				else if (%va $= "Medium") %va = "M";
				else if (%va $= "Heavy") %va = "H";
				%extra = %extra @ ",va:" @ %va;
			}
			if (%ka !$= "")
			{
				if (%ka $= "Light") %ka = "L";
				else if (%ka $= "Medium") %ka = "M";
				else if (%ka $= "Heavy") %ka = "H";
				%extra = %extra @ ",ka:" @ %ka;
			}
		}
	}

	if (isObject(%killer))
	{
		if (%game.numTeams > 1 && %killer.team == %victim.team && %killer != %victim)
			tribaloutpost_writePlay("TeamKill", getSimTime(), %killer, %victim, %victim.player.position, $DamageName[%damageType], %extra);
		else if (%killer == %victim)
			tribaloutpost_writePlay("SelfKill", getSimTime(), %killer, "", "", $DamageName[%damageType], %extra);
		else
			tribaloutpost_writePlay("Kill", getSimTime(), %killer, %victim, %victim.player.position, $DamageName[%damageType], %extra);
	}

	// Environmental/unknown deaths
	if (isObject(%victim) && %killer == 0)
	{
		%implClass = isObject(%implement) ? %implement.getClassName() : "";
		tribaloutpost_writePlay("Death", getSimTime(), %victim, 0, %victim.player.position, %implClass, "");
	}
}

// ============================================================
// Asset Destroy / Repair Play-by-Play Helpers
// ============================================================

function tribaloutpost_writeAssetDestroy(%cl, %obj, %friendly)
{
	%dataName = %obj.getDataBlock().getName();
	%assetType = "";
	switch$(%dataName)
	{
		case "GeneratorLarge": %assetType = "Generator";
		case "SolarPanel": %assetType = "Solar";
		case "SensorLargePulse" or "SensorMediumPulse": %assetType = "Sensor";
		case "TurretBaseLarge": %assetType = "Turret";
		case "StationInventory" or "StationAmmo": %assetType = "Inventory";
		case "StationVehicle": %assetType = "VehiclePad";
		case "SentryTurret": %assetType = "Sentry";
		case "DeployedMotionSensor" or "DeployedPulseSensor": %assetType = "DepSensor";
		case "TurretDeployedWallIndoor" or "TurretDeployedFloorIndoor" or "TurretDeployedCeilingIndoor" or "TurretDeployedOutdoor": %assetType = "DepTurret";
		case "DeployedStationInventory": %assetType = "DepInventory";
		case "MPBTeleporter": %assetType = "MPBStation";
		default: %assetType = %dataName;
	}

	%event = %friendly ? "TkAssetDestroy" : "AssetDestroy";
	tribaloutpost_writePlay(%event, getSimTime(), %cl, "", %obj.position, %assetType, "");
}

function tribaloutpost_writeAssetRepair(%obj)
{
	%client = %obj.repairedBy;
	if (!isObject(%client))
		return;

	%dataName = %obj.getDataBlock().getName();
	%assetType = "";
	switch$(%dataName)
	{
		case "GeneratorLarge": %assetType = "Generator";
		case "SolarPanel": %assetType = "Solar";
		case "SensorLargePulse" or "SensorMediumPulse": %assetType = "Sensor";
		case "TurretBaseLarge": %assetType = "Turret";
		case "StationInventory" or "StationAmmo": %assetType = "Inventory";
		case "StationVehicle": %assetType = "VehiclePad";
		case "SentryTurret": %assetType = "Sentry";
		case "DeployedMotionSensor" or "DeployedPulseSensor": %assetType = "DepSensor";
		case "TurretDeployedWallIndoor" or "TurretDeployedFloorIndoor" or "TurretDeployedCeilingIndoor" or "TurretDeployedOutdoor": %assetType = "DepTurret";
		case "DeployedStationInventory": %assetType = "DepInventory";
		case "MPBTeleporter": %assetType = "MPBStation";
		default: %assetType = %dataName;
	}
	tribaloutpost_writePlay("AssetRepair", getSimTime(), %client, "", %obj.position, %assetType, "");
}

// ============================================================
// Captain Detection
// ============================================================

function tribaloutpost_setCaptains(%game)
{
	if (%game.numTeams > 1)
	{
		for (%team = 1; %team <= %game.numTeams; %team++)
			%captain[%team] = 0;

		for (%i = 0; %i < ClientGroup.getCount(); %i++)
		{
			%cl = ClientGroup.getObject(%i);
			if (%cl.team != 0 && %cl.playersTeamed > 0 && (%captain[%cl.team] == 0 || %captain[%cl.team].playersTeamed < %cl.playersTeamed))
				%captain[%cl.team] = %cl;
		}

		for (%team = 1; %team <= %game.numTeams; %team++)
		{
			if (isObject(%captain[%team]))
				%captain[%team].wasCaptain = 1;
		}
	}
}

// ============================================================
// Game Over Handler (shared by all game types)
// ============================================================

function tribaloutpost_onGameOver(%game)
{
	tribaloutpost_log("gameOver: " @ %game.class);

	// Skip submission if no players ever participated (e.g. first boot map change)
	if (ClientGroup.getCount() == 0 && $T2Stats::DroppedCount == 0)
	{
		tribaloutpost_log("No players, skipping submission.");
		return;
	}

	if ($TribalOutpost::EnablePlayByPlay)
		tribaloutpost_writePlay("MissionEnd", getSimTime(), "", "", "", "", "");

	// Set captains before writing
	tribaloutpost_setCaptains(%game);

	// Write all files BEFORE Parent::gameOver (DT schedules stat reset)
	tribaloutpost_writeMatchFile(%game);
	tribaloutpost_writePlayersFile(%game);
	tribaloutpost_writeExtFile(%game);

	// Start async submission chain
	tribaloutpost_submitMatch();
}

// ============================================================
// Mission Load Handler (shared by all game types)
// ============================================================

function tribaloutpost_onMissionLoadDone(%game)
{
	tribaloutpost_log("missionLoadDone: " @ %game.class);
	tribaloutpost_log("Game object class: " @ %game.class @ " getName: " @ %game.getName() @ " getId: " @ %game.getId());
	tribaloutpost_log("Package dtStats active: " @ isActivePackage(dtStats));
	tribaloutpost_log("Package TribalOutpost active: " @ isActivePackage(TribalOutpost));

	// Load token on first map load
	if ($TribalOutpost::Token $= "")
		tribaloutpost_loadToken();

	// Register if no token
	if ($TribalOutpost::Token $= "")
		tribaloutpost_register();

	// Init play-by-play file
	if ($TribalOutpost::EnablePlayByPlay)
	{
		tribaloutpost_initPlaysFile();
		tribaloutpost_writePlay("MissionStart", getSimTime(), "", "", "", "", "");
	}
}

// ============================================================
// Game Hooks — All 6 Game Types
// ============================================================

package TribalOutpost
{
	// ---- CTFGame ----

	function CTFGame::missionLoadDone(%game)
	{
		tribaloutpost_onMissionLoadDone(%game);
		Parent::missionLoadDone(%game);
		$CurrentMissionDistance = vectorDist($TeamFlag[1].getTransform(), $TeamFlag[2].getTransform());
	}

	function CTFGame::gameOver(%game)
	{
		tribaloutpost_onGameOver(%game);
		Parent::gameOver(%game);
	}

	function CTFGame::onClientKilled(%game, %clVictim, %clKiller, %damageType, %implement, %damageLocation)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_handleKillStat(%game, %clVictim, %clKiller, %damageType, %implement, %damageLocation);
		Parent::onClientKilled(%game, %clVictim, %clKiller, %damageType, %implement, %damageLocation);
	}

	function CTFGame::playerGotFlagTarget(%game, %player)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writePlay("FlagGrab", getSimTime(), %player.client, "", %player.position, "", "");
		Parent::playerGotFlagTarget(%game, %player);
	}

	function CTFGame::playerDroppedFlag(%game, %player)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writePlay("FlagDrop", getSimTime(), %player.client, "", %player.position, "", "");
		Parent::playerDroppedFlag(%game, %player);
	}

	function CTFGame::flagCap(%game, %player)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writePlay("FlagCap", getSimTime(), %player.client, "", %player.position, "", "");
		Parent::flagCap(%game, %player);
	}

	function CTFGame::flagReturn(%game, %flag, %player)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writePlay("FlagReturn", getSimTime(), %player.client, "", %player.position, "", "");
		Parent::flagReturn(%game, %flag, %player);
	}

	function CTFGame::awardScoreStaticShapeDestroy(%game, %cl, %obj)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writeAssetDestroy(%cl, %obj, false);
		Parent::awardScoreStaticShapeDestroy(%game, %cl, %obj);
	}

	function CTFGame::awardScoreTkDestroy(%game, %cl, %obj)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writeAssetDestroy(%cl, %obj, true);
		Parent::awardScoreTkDestroy(%game, %cl, %obj);
	}

	function CTFGame::awardScoreVehicleDestroyed(%game, %client, %vehicleType, %mult, %passengers)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writePlay("VehicleDestroy", getSimTime(), %client, "", "", %vehicleType, "");
		Parent::awardScoreVehicleDestroyed(%game, %client, %vehicleType, %mult, %passengers);
	}

	function CTFGame::staticShapeOnRepaired(%game, %obj, %objName)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writeAssetRepair(%obj);
		Parent::staticShapeOnRepaired(%game, %obj, %objName);
	}

	function CTFGame::awardScoreFlagDefend(%game, %killerID)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writePlay("FlagDefend", getSimTime(), %killerID, "", "", "", "");
		Parent::awardScoreFlagDefend(%game, %killerID);
	}

	function CTFGame::awardScoreGenDefend(%game, %killerID)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writePlay("GenDefend", getSimTime(), %killerID, "", "", "", "");
		Parent::awardScoreGenDefend(%game, %killerID);
	}

	function CTFGame::awardScoreCarrierKill(%game, %killerID)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writePlay("CarrierKill", getSimTime(), %killerID, "", "", "", "");
		Parent::awardScoreCarrierKill(%game, %killerID);
	}

	function CTFGame::awardScoreEscortAssist(%game, %killerID)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writePlay("EscortAssist", getSimTime(), %killerID, "", "", "", "");
		Parent::awardScoreEscortAssist(%game, %killerID);
	}

	function CTFGame::applyConcussion(%game, %player)
	{
		if ($TribalOutpost::EnablePlayByPlay && %player.holdingFlag > 0)
			tribaloutpost_writePlay("ConcussFlag", getSimTime(), "", %player.client, %player.position, "", "");
		Parent::applyConcussion(%game, %player);
	}

	// ---- LakRabbitGame ----

	function LakRabbitGame::missionLoadDone(%game)
	{
		tribaloutpost_onMissionLoadDone(%game);
		Parent::missionLoadDone(%game);
	}

	function LakRabbitGame::gameOver(%game)
	{
		tribaloutpost_onGameOver(%game);
		Parent::gameOver(%game);
	}

	function LakRabbitGame::onClientKilled(%game, %clVictim, %clKiller, %damageType, %implement, %damageLocation)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_handleKillStat(%game, %clVictim, %clKiller, %damageType, %implement, %damageLocation);
		Parent::onClientKilled(%game, %clVictim, %clKiller, %damageType, %implement, %damageLocation);
	}

	// ---- DMGame ----

	function DMGame::missionLoadDone(%game)
	{
		tribaloutpost_onMissionLoadDone(%game);
		Parent::missionLoadDone(%game);
	}

	function DMGame::gameOver(%game)
	{
		tribaloutpost_onGameOver(%game);
		Parent::gameOver(%game);
	}

	function DMGame::onClientKilled(%game, %clVictim, %clKiller, %damageType, %implement, %damageLocation)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_handleKillStat(%game, %clVictim, %clKiller, %damageType, %implement, %damageLocation);
		Parent::onClientKilled(%game, %clVictim, %clKiller, %damageType, %implement, %damageLocation);
	}

	// ---- LCTFGame ----

	function LCTFGame::missionLoadDone(%game)
	{
		tribaloutpost_onMissionLoadDone(%game);
		Parent::missionLoadDone(%game);
		$CurrentMissionDistance = vectorDist($TeamFlag[1].getTransform(), $TeamFlag[2].getTransform());
	}

	function LCTFGame::gameOver(%game)
	{
		tribaloutpost_onGameOver(%game);
		Parent::gameOver(%game);
	}

	function LCTFGame::onClientKilled(%game, %clVictim, %clKiller, %damageType, %implement, %damageLocation)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_handleKillStat(%game, %clVictim, %clKiller, %damageType, %implement, %damageLocation);
		Parent::onClientKilled(%game, %clVictim, %clKiller, %damageType, %implement, %damageLocation);
	}

	function LCTFGame::playerGotFlagTarget(%game, %player)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writePlay("FlagGrab", getSimTime(), %player.client, "", %player.position, "", "");
		Parent::playerGotFlagTarget(%game, %player);
	}

	function LCTFGame::playerDroppedFlag(%game, %player)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writePlay("FlagDrop", getSimTime(), %player.client, "", %player.position, "", "");
		Parent::playerDroppedFlag(%game, %player);
	}

	function LCTFGame::flagCap(%game, %player)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writePlay("FlagCap", getSimTime(), %player.client, "", %player.position, "", "");
		Parent::flagCap(%game, %player);
	}

	function LCTFGame::flagReturn(%game, %flag, %player)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writePlay("FlagReturn", getSimTime(), %player.client, "", %player.position, "", "");
		Parent::flagReturn(%game, %flag, %player);
	}

	function LCTFGame::awardScoreStaticShapeDestroy(%game, %cl, %obj)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writeAssetDestroy(%cl, %obj, false);
		Parent::awardScoreStaticShapeDestroy(%game, %cl, %obj);
	}

	function LCTFGame::awardScoreTkDestroy(%game, %cl, %obj)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writeAssetDestroy(%cl, %obj, true);
		Parent::awardScoreTkDestroy(%game, %cl, %obj);
	}

	function LCTFGame::awardScoreVehicleDestroyed(%game, %client, %vehicleType, %mult, %passengers)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writePlay("VehicleDestroy", getSimTime(), %client, "", "", %vehicleType, "");
		Parent::awardScoreVehicleDestroyed(%game, %client, %vehicleType, %mult, %passengers);
	}

	function LCTFGame::staticShapeOnRepaired(%game, %obj, %objName)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writeAssetRepair(%obj);
		Parent::staticShapeOnRepaired(%game, %obj, %objName);
	}

	function LCTFGame::awardScoreFlagDefend(%game, %killerID)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writePlay("FlagDefend", getSimTime(), %killerID, "", "", "", "");
		Parent::awardScoreFlagDefend(%game, %killerID);
	}

	function LCTFGame::awardScoreGenDefend(%game, %killerID)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writePlay("GenDefend", getSimTime(), %killerID, "", "", "", "");
		Parent::awardScoreGenDefend(%game, %killerID);
	}

	function LCTFGame::awardScoreCarrierKill(%game, %killerID)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writePlay("CarrierKill", getSimTime(), %killerID, "", "", "", "");
		Parent::awardScoreCarrierKill(%game, %killerID);
	}

	function LCTFGame::awardScoreEscortAssist(%game, %killerID)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writePlay("EscortAssist", getSimTime(), %killerID, "", "", "", "");
		Parent::awardScoreEscortAssist(%game, %killerID);
	}

	function LCTFGame::applyConcussion(%game, %player)
	{
		if ($TribalOutpost::EnablePlayByPlay && %player.holdingFlag > 0)
			tribaloutpost_writePlay("ConcussFlag", getSimTime(), "", %player.client, %player.position, "", "");
		Parent::applyConcussion(%game, %player);
	}

	// ---- SCtFGame ----

	function SCtFGame::missionLoadDone(%game)
	{
		tribaloutpost_onMissionLoadDone(%game);
		Parent::missionLoadDone(%game);
		$CurrentMissionDistance = vectorDist($TeamFlag[1].getTransform(), $TeamFlag[2].getTransform());
	}

	function SCtFGame::gameOver(%game)
	{
		tribaloutpost_onGameOver(%game);
		Parent::gameOver(%game);
	}

	function SCtFGame::onClientKilled(%game, %clVictim, %clKiller, %damageType, %implement, %damageLocation)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_handleKillStat(%game, %clVictim, %clKiller, %damageType, %implement, %damageLocation);
		Parent::onClientKilled(%game, %clVictim, %clKiller, %damageType, %implement, %damageLocation);
	}

	function SCtFGame::playerGotFlagTarget(%game, %player)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writePlay("FlagGrab", getSimTime(), %player.client, "", %player.position, "", "");
		Parent::playerGotFlagTarget(%game, %player);
	}

	function SCtFGame::playerDroppedFlag(%game, %player)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writePlay("FlagDrop", getSimTime(), %player.client, "", %player.position, "", "");
		Parent::playerDroppedFlag(%game, %player);
	}

	function SCtFGame::flagCap(%game, %player)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writePlay("FlagCap", getSimTime(), %player.client, "", %player.position, "", "");
		Parent::flagCap(%game, %player);
	}

	function SCtFGame::flagReturn(%game, %flag, %player)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writePlay("FlagReturn", getSimTime(), %player.client, "", %player.position, "", "");
		Parent::flagReturn(%game, %flag, %player);
	}

	function SCtFGame::awardScoreStaticShapeDestroy(%game, %cl, %obj)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writeAssetDestroy(%cl, %obj, false);
		Parent::awardScoreStaticShapeDestroy(%game, %cl, %obj);
	}

	function SCtFGame::awardScoreTkDestroy(%game, %cl, %obj)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writeAssetDestroy(%cl, %obj, true);
		Parent::awardScoreTkDestroy(%game, %cl, %obj);
	}

	function SCtFGame::awardScoreVehicleDestroyed(%game, %client, %vehicleType, %mult, %passengers)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writePlay("VehicleDestroy", getSimTime(), %client, "", "", %vehicleType, "");
		Parent::awardScoreVehicleDestroyed(%game, %client, %vehicleType, %mult, %passengers);
	}

	function SCtFGame::staticShapeOnRepaired(%game, %obj, %objName)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writeAssetRepair(%obj);
		Parent::staticShapeOnRepaired(%game, %obj, %objName);
	}

	function SCtFGame::awardScoreFlagDefend(%game, %killerID)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writePlay("FlagDefend", getSimTime(), %killerID, "", "", "", "");
		Parent::awardScoreFlagDefend(%game, %killerID);
	}

	function SCtFGame::awardScoreGenDefend(%game, %killerID)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writePlay("GenDefend", getSimTime(), %killerID, "", "", "", "");
		Parent::awardScoreGenDefend(%game, %killerID);
	}

	function SCtFGame::awardScoreCarrierKill(%game, %killerID)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writePlay("CarrierKill", getSimTime(), %killerID, "", "", "", "");
		Parent::awardScoreCarrierKill(%game, %killerID);
	}

	function SCtFGame::awardScoreEscortAssist(%game, %killerID)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writePlay("EscortAssist", getSimTime(), %killerID, "", "", "", "");
		Parent::awardScoreEscortAssist(%game, %killerID);
	}

	function SCtFGame::applyConcussion(%game, %player)
	{
		if ($TribalOutpost::EnablePlayByPlay && %player.holdingFlag > 0)
			tribaloutpost_writePlay("ConcussFlag", getSimTime(), "", %player.client, %player.position, "", "");
		Parent::applyConcussion(%game, %player);
	}

	// ---- ArenaGame ----

	function ArenaGame::missionLoadDone(%game)
	{
		tribaloutpost_onMissionLoadDone(%game);
		Parent::missionLoadDone(%game);
	}

	function ArenaGame::gameOver(%game)
	{
		tribaloutpost_onGameOver(%game);
		Parent::gameOver(%game);
	}

	function ArenaGame::onClientKilled(%game, %clVictim, %clKiller, %damageType, %implement, %damageLocation)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_handleKillStat(%game, %clVictim, %clKiller, %damageType, %implement, %damageLocation);
		Parent::onClientKilled(%game, %clVictim, %clKiller, %damageType, %implement, %damageLocation);
	}

	// ---- Shared hooks ----

	function GameConnection::onConnect(%client, %name, %raceGender, %skin, %voice, %voicePitch)
	{
		Parent::onConnect(%client, %name, %raceGender, %skin, %voice, %voicePitch);
		%client.jointime = getSimTime();

		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writePlay("PlayerConnect", %client.jointime, %client, "", "", "", "");
	}

	function GameConnection::onDrop(%client, %reason)
	{
		if ($TribalOutpost::EnablePlayByPlay)
			tribaloutpost_writePlay("PlayerDisconnect", getSimTime(), %client, "", "", "", "");

		// Snapshot stats before client object is destroyed so dropped players
		// appear in the players/ext files at game over
		tribaloutpost_snapshotDroppedPlayer(%client);

		Parent::onDrop(%client, %reason);
	}
};

activatePackage(TribalOutpost);
tribaloutpost_log("TribalOutpost Stats v" @ $TribalOutpost::Version @ " loaded. URL=" @ $TribalOutpost::StatsURL);
