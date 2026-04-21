"""Brute-force the 4 hashes against extracted XML/Lua/BT files too, and also test
file basenames (weapon class / attacker names are often referenced by xml filename).
"""
import os, sys, binascii, re

ROOT = r"O:\jeux-backup\MONSTER HUNTER ONLINE\MHO_TOOL\extracted"

TARGETS = {
    0xF7D3AB2A: "hashAttacker",
    0xB7AFD4AF: "hashWeaponClass",
    0x2AA21FBC: "hashFireMode",
    0x61BD8701: "hashCurEvent",
}
# also reversed-byte variants
for h in list(TARGETS.keys()):
    rev = int.from_bytes(h.to_bytes(4, "big"), "little")
    if rev not in TARGETS:
        TARGETS[rev] = TARGETS[h] + "(rev)"

def crc32_zlib(s: bytes) -> int:
    return binascii.crc32(s) & 0xFFFFFFFF

def crc32_mpeg2(s: bytes) -> int:
    poly = 0x04C11DB7
    crc = 0xFFFFFFFF
    for b in s:
        crc ^= b << 24
        for _ in range(8):
            crc = ((crc << 1) ^ poly) & 0xFFFFFFFF if crc & 0x80000000 else (crc << 1) & 0xFFFFFFFF
    return crc ^ 0xFFFFFFFF

ALGOS = [
    ("zlib-raw",   lambda s: crc32_zlib(s.encode("utf-8", "ignore"))),
    ("zlib-lower", lambda s: crc32_zlib(s.lower().encode("utf-8", "ignore"))),
    ("mpeg2-raw",  lambda s: crc32_mpeg2(s.encode("utf-8", "ignore"))),
    ("mpeg2-lower",lambda s: crc32_mpeg2(s.lower().encode("utf-8", "ignore"))),
]

TOKEN_RE = re.compile(rb'[A-Za-z_][A-Za-z0-9_]{2,63}')

def scan_file(path, exts):
    name, ext = os.path.splitext(path)
    if ext.lower() not in exts:
        return []
    hits = []
    try:
        with open(path, "rb") as f:
            data = f.read()
    except Exception:
        return []
    # Scan tokens
    seen = set()
    for m in TOKEN_RE.finditer(data):
        tok = m.group(0)
        if tok in seen: continue
        seen.add(tok)
        try:
            s = tok.decode("ascii")
        except Exception:
            continue
        for aname, fn in ALGOS:
            h = fn(s)
            if h in TARGETS:
                hits.append((TARGETS[h], aname, s, path))
    # Also test basename-derived tokens
    base = os.path.basename(path)
    stem = os.path.splitext(base)[0]
    for candidate in {base, stem, stem.lower(), stem.replace(".", "_")}:
        for aname, fn in ALGOS:
            h = fn(candidate)
            if h in TARGETS:
                hits.append((TARGETS[h], aname, f"<basename:{candidate}>", path))
    return hits

def scan(exts):
    total_hits = []
    n_files = 0
    for root, _, files in os.walk(ROOT):
        for fn in files:
            path = os.path.join(root, fn)
            ext = os.path.splitext(fn)[1].lower()
            if ext not in exts:
                continue
            n_files += 1
            hits = scan_file(path, exts)
            total_hits.extend(hits)
    print(f"Scanned {n_files} files ({','.join(exts)})")
    for name, algo, s, path in total_hits:
        print(f"[{name}] via {algo}  '{s}'  {path}")
    if not total_hits:
        print("NO HITS")

if __name__ == "__main__":
    # Target text-ish files
    exts = {".xml", ".lua", ".txt", ".bt", ".cfg", ".ini", ".json", ".csv"}
    scan(exts)
