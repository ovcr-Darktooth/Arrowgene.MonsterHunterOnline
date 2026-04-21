"""Try MHO-specific naming conventions for the 4 hashes."""
import binascii, itertools

TARGETS = {
    0xF7D3AB2A: "hashAttacker",
    0xB7AFD4AF: "hashWeaponClass",
    0x2AA21FBC: "hashFireMode",
    0x61BD8701: "hashCurEvent",
}

def z(s): return binascii.crc32(s.encode("utf-8", "ignore")) & 0xFFFFFFFF
def zl(s): return binascii.crc32(s.lower().encode("utf-8", "ignore")) & 0xFFFFFFFF

def crc32_nobit(s: bytes) -> int:
    poly = 0x04C11DB7
    crc = 0xFFFFFFFF
    for b in s:
        crc ^= b << 24
        for _ in range(8):
            crc = ((crc << 1) ^ poly) & 0xFFFFFFFF if crc & 0x80000000 else (crc << 1) & 0xFFFFFFFF
    return crc ^ 0xFFFFFFFF

def m(s): return crc32_nobit(s.encode("utf-8", "ignore"))
def ml(s): return crc32_nobit(s.lower().encode("utf-8", "ignore"))

# Candidate token pools
WEAPON_CLASSES = [
    "Weapon", "Melee", "Ranged", "Bow", "LongSword", "GreatSword", "DualBlade", "DualBlades",
    "DualSword", "Hammer", "HuntingHorn", "Lance", "GunLance", "Gunlance", "SwitchAxe", "SwitchAxeG",
    "ChargeBlade", "InsectGlaive", "LightBowgun", "HeavyBowgun", "Bowgun", "Bowgun_Light",
    "Bowgun_Heavy", "Sword", "SwordAndShield", "SnS",
    "MonsterClaw", "MonsterBite", "MonsterBreath", "MonsterTail", "MonsterTailSpike",
    "Monster", "MonsterFire", "MonsterAttack", "MonsterMelee",
    "GreatSwordClass", "LongSwordClass",
]

FIREMODE = [
    "Default", "Single", "Melee", "Charge", "Charged", "Rapid", "Auto", "Burst", "Strong",
    "Weak", "Heavy", "Light", "Normal", "Special", "Shoot", "Firework",
    "Claw", "Bite", "Breath", "Tail", "TailSpike", "FireBreath", "IceBreath", "WaterBreath",
    "Combo1", "Combo2", "Primary", "Secondary", "Fury", "Fire", "Stun", "Stomp", "Kick",
    "Rush", "Dash", "Sweep", "Rage", "Roar",
]

ATTACKER_CANDIDATES = [
    # From AttackName column (already known to not match), re-test with other algos
    "AreaFireN", "Template_Attack_10", "Template_Attack_20", "Template_Attack_30",
    "em001", "Em001", "em002", "Em003",
    # Composed
    "Attack", "Attacker",
    "AreaFire", "Fire", "Ice", "Water", "Dragon",
    # Monster species names
    "Rathian", "Rathalos", "Diablos", "Tigrex", "Khezu", "Congalala",
]

EVENTS = [
    "AttackHit", "AttackBegin", "AttackEnd",
    "OnHit", "OnAttack", "OnFire", "OnShoot",
    "HitBegin", "HitEnd",
    "Event", "EventAttackHit", "MeleeHit",
    "HitEvent", "Melee",
    "Default", "Begin", "End", "Start",
]

def test(candidates, label):
    found = []
    for s in candidates:
        for algo_name, fn in [("zlib", z), ("zlib-lower", zl), ("mpeg2", m), ("mpeg2-lower", ml)]:
            h = fn(s)
            if h in TARGETS:
                found.append((TARGETS[h], algo_name, s))
    if found:
        print(f"[{label}] hits:")
        for x in found: print(" ", x)
    else:
        print(f"[{label}] no hits from {len(candidates)} candidates")

test(WEAPON_CLASSES, "WEAPON_CLASSES")
test(FIREMODE,       "FIREMODE")
test(ATTACKER_CANDIDATES, "ATTACKER_CANDIDATES")
test(EVENTS,         "EVENTS")

# Also print the known CryEngine CRC32 check value so we can confirm our zlib matches their CCrc32
print("\nsanity: zlib('123456789') =", hex(z("123456789")))
