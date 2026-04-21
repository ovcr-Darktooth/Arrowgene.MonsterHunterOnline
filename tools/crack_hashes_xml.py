"""Wider scan: extract *attribute values* and tag names from XML, and any quoted string.
Also test concatenations like Name+"Hit" or Name+suffix.
"""
import os, re, binascii, sys

ROOT = r"O:\jeux-backup\MONSTER HUNTER ONLINE\MHO_TOOL\extracted"

TARGETS = {
    0xF7D3AB2A: "hashAttacker",
    0xB7AFD4AF: "hashWeaponClass",
    0x2AA21FBC: "hashFireMode",
    0x61BD8701: "hashCurEvent",
}

def z(b): return binascii.crc32(b) & 0xFFFFFFFF

VALUE_RE = re.compile(rb'"([^"\n]{1,200})"')
TAG_RE   = re.compile(rb'<([A-Za-z_][A-Za-z0-9_]{1,60})')

def scan(exts):
    strings = {}  # str -> list of (path, line_kind)
    n_files = 0
    for root, _, files in os.walk(ROOT):
        for fn in files:
            ext = os.path.splitext(fn)[1].lower()
            if ext not in exts: continue
            n_files += 1
            path = os.path.join(root, fn)
            try:
                with open(path, "rb") as f: data = f.read()
            except: continue
            for m in VALUE_RE.finditer(data):
                try:
                    s = m.group(1).decode("utf-8")
                except:
                    continue
                strings.setdefault(s, []).append(("attr", path))
            for m in TAG_RE.finditer(data):
                try:
                    s = m.group(1).decode("utf-8")
                except:
                    continue
                strings.setdefault(s, []).append(("tag", path))
    print(f"Scanned {n_files} files ({','.join(exts)}), collected {len(strings)} distinct strings")

    hits = []
    for s, occ in strings.items():
        for algo_name, encoded in [
            ("zlib-raw", s.encode("utf-8", "ignore")),
            ("zlib-lower", s.lower().encode("utf-8", "ignore")),
        ]:
            h = z(encoded)
            if h in TARGETS:
                hits.append((TARGETS[h], algo_name, s, occ[0]))
    for h in hits:
        print(h)
    if not hits:
        print("NO HITS")
    return strings

if __name__ == "__main__":
    scan({".xml", ".lua"})
