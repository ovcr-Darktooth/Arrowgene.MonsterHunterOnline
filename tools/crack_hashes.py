"""Brute-force the 4 hashes from hit_packetv3.txt against every string cell in every CSV
under static_csv/, using several CRC32 variants.

Target hashes (big-endian as read from the packet):
  hashAttacker     = 0xF7D3AB2A
  hashWeaponClass  = 0xB7AFD4AF
  hashFireMode     = 0x2AA21FBC
  hashCurEvent     = 0x61BD8701
"""

import os, sys, zlib, binascii

STATIC_CSV = r"O:\jeux-backup\MONSTER HUNTER ONLINE\MHO_TOOL\static_csv"

TARGETS = {
    0xF7D3AB2A: "hashAttacker",
    0xB7AFD4AF: "hashWeaponClass",
    0x2AA21FBC: "hashFireMode",
    0x61BD8701: "hashCurEvent",
}

# Also try bytewise-reversed (in case endianness was misread)
for h in list(TARGETS.keys()):
    rev = int.from_bytes(h.to_bytes(4, "big"), "little")
    if rev not in TARGETS:
        TARGETS[rev] = TARGETS[h] + "(rev)"

def crc32_zlib(s: bytes) -> int:
    return binascii.crc32(s) & 0xFFFFFFFF

def crc32_zlib_lc(s: str) -> int:
    return binascii.crc32(s.lower().encode("utf-8", "ignore")) & 0xFFFFFFFF

def crc32_nobit_reflect(s: bytes, poly=0x04C11DB7, init=0xFFFFFFFF, xor_out=0xFFFFFFFF) -> int:
    # Non-reflected CRC-32 (MPEG-2 style), to cover CryEngine variants
    crc = init
    for b in s:
        crc ^= b << 24
        for _ in range(8):
            if crc & 0x80000000:
                crc = ((crc << 1) ^ poly) & 0xFFFFFFFF
            else:
                crc = (crc << 1) & 0xFFFFFFFF
    return crc ^ xor_out

def crc32c(s: bytes) -> int:
    # Castagnoli CRC-32C, poly 0x1EDC6F41 reversed = 0x82F63B78
    poly = 0x82F63B78
    crc = 0xFFFFFFFF
    for b in s:
        crc ^= b
        for _ in range(8):
            crc = (crc >> 1) ^ (poly if crc & 1 else 0)
    return crc ^ 0xFFFFFFFF

def fnv1a(s: bytes) -> int:
    h = 0x811C9DC5
    for b in s:
        h ^= b
        h = (h * 0x01000193) & 0xFFFFFFFF
    return h

def djb2(s: bytes) -> int:
    h = 5381
    for b in s:
        h = ((h * 33) + b) & 0xFFFFFFFF
    return h

ALGOS = [
    ("crc32-zlib-raw",    lambda s: crc32_zlib(s.encode("utf-8", "ignore"))),
    ("crc32-zlib-lower",  lambda s: crc32_zlib_lc(s)),
    ("crc32-mpeg2-raw",   lambda s: crc32_nobit_reflect(s.encode("utf-8", "ignore"))),
    ("crc32-mpeg2-lower", lambda s: crc32_nobit_reflect(s.lower().encode("utf-8", "ignore"))),
    ("crc32c-raw",        lambda s: crc32c(s.encode("utf-8", "ignore"))),
    ("crc32c-lower",      lambda s: crc32c(s.lower().encode("utf-8", "ignore"))),
    ("fnv1a-raw",         lambda s: fnv1a(s.encode("utf-8", "ignore"))),
    ("fnv1a-lower",       lambda s: fnv1a(s.lower().encode("utf-8", "ignore"))),
    ("djb2-raw",          lambda s: djb2(s.encode("utf-8", "ignore"))),
    ("djb2-lower",        lambda s: djb2(s.lower().encode("utf-8", "ignore"))),
]

def scan():
    n_files = 0
    n_strings = 0
    hits = []
    for root, _, files in os.walk(STATIC_CSV):
        for fn in files:
            if not fn.lower().endswith(".csv"):
                continue
            n_files += 1
            path = os.path.join(root, fn)
            try:
                with open(path, "r", encoding="utf-8", errors="ignore") as f:
                    for line_no, line in enumerate(f, 1):
                        for cell in line.rstrip("\r\n").split(","):
                            c = cell.strip().strip('"').strip()
                            if not c:
                                continue
                            n_strings += 1
                            for algo_name, fn_h in ALGOS:
                                h = fn_h(c)
                                if h in TARGETS:
                                    hits.append((TARGETS[h], algo_name, c, fn, line_no))
            except Exception as e:
                print(f"! {path}: {e}", file=sys.stderr)
    print(f"Scanned {n_files} CSVs, {n_strings} cells")
    if not hits:
        print("NO HITS")
        return
    for name, algo, s, fn, line in hits:
        print(f"[{name}] via {algo}  '{s}'  ({fn}:{line})")

if __name__ == "__main__":
    scan()
