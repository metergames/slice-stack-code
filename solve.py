"""
CTF Challenge: Behind The N0tes
================================

Fake alien currency notes were found printed with specific serial numbers.
Equations and a key were recovered from the abandoned printing press.

EQUATIONS (from Equations-And-Key.png)
---------------------------------------
    PH ∈ [0, 44]
    x  = PH
    y  = 41 + PH(Sin² + Cos²) + 4

KEY INSIGHT: Pythagorean trigonometric identity
    Sin²(θ) + Cos²(θ) = 1   ← for ANY angle θ

Therefore:
    y = 41 + PH × 1 + 4
    y = 45 + PH                ← simplified

CIPHER
-------
    key    = 5 × n_i           (key printed on the note)
    n_i    = 0  if special character  (brackets, underscores, etc.)
    n_i    ∈ (0, 39)           otherwise

Encoding:  key = 5 × (ASCII(char) − 45)
Decoding:  char = chr(45 + key / 5)

FAKE NOTE SERIAL NUMBERS (from the banknote image)
----------------------------------------------------
    Left   serial:  PR1S4528824
    Right  serial:  42800176301
    Bottom serial:  106852838

The letters in the left serial (P, R, S) act as special-character markers.
The digits in all serials are the encoded key values.
"""

# ---------------------------------------------------------------------------
# Decoder
# ---------------------------------------------------------------------------

def decode_key(key: int) -> str | None:
    """Decode a single cipher key to its plaintext character.

    Returns the decoded character, None for an invalid key, or
    a special-character placeholder for key == 0.
    """
    if key == 0:
        return None          # special character (bracket / underscore)
    if key % 5 != 0:
        return None          # not a valid key
    n_i = key // 5
    if not (1 <= n_i <= 44):
        return None          # outside PH range
    return chr(45 + n_i)


def decode_serial(serial: str, label: str = "") -> list[tuple[int, int, str]]:
    """Scan a serial string for all embedded valid 2- and 3-digit keys.

    Returns a list of (start_pos, key_value, decoded_char) tuples.
    """
    results = []
    for length in (2, 3):
        for i in range(len(serial) - length + 1):
            chunk = serial[i : i + length]
            # Disallow leading zeros on multi-digit keys (except "00")
            if length > 1 and chunk[0] == "0" and not all(c == "0" for c in chunk):
                continue
            k = int(chunk)
            c = decode_key(k)
            if c is not None:
                results.append((i, k, c))
    # Sort by position, deduplicate overlapping windows by preferring shorter keys
    results.sort(key=lambda x: (x[0], x[1]))
    return results


# ---------------------------------------------------------------------------
# Note serial numbers (as read from the banknote image)
# ---------------------------------------------------------------------------

LEFT_SERIAL   = "14528824"    # digit characters extracted from "PR1S4528824"
                              # (letters P, R, S are special-character markers)
RIGHT_SERIAL  = "42800176301"
BOTTOM_SERIAL = "106852838"

SERIALS = [
    ("Left   (PR1S4528824)", LEFT_SERIAL),
    ("Right  (42800176301)", RIGHT_SERIAL),
    ("Bottom (106852838)  ", BOTTOM_SERIAL),
]


# ---------------------------------------------------------------------------
# Main solve routine
# ---------------------------------------------------------------------------

def solve():
    print("=" * 60)
    print("  Behind The N0tes — CTF Solve Script")
    print("=" * 60)

    print("\n[STEP 1]  Pythagorean simplification:")
    print("  y = 41 + PH(Sin² + Cos²) + 4")
    print("    → Sin² + Cos² = 1  (Pythagorean identity)")
    print("  y = 41 + PH × 1 + 4  =  45 + PH")

    print("\n[STEP 2]  Cipher:")
    print("  Encoding:  key  = 5 × (ASCII(char) − 45)")
    print("  Decoding:  char = chr(45 + key / 5)")
    print("  Special chars (brackets, underscores …) use key = 0")

    print("\n[STEP 3]  Scan note serial numbers for valid keys:")
    print()

    found_chars = []
    seen_positions: dict[str, set[int]] = {}

    for label, serial in SERIALS:
        print(f"  Serial {label}: '{serial}'")
        hits = decode_serial(serial, label)

        # Track which positions are consumed to avoid double-counting
        seen_positions[serial] = set()
        for pos, key, char in hits:
            # Skip if any position in this hit was already used
            span = set(range(pos, pos + len(str(key))))
            if span & seen_positions[serial]:
                continue
            seen_positions[serial] |= span
            key_len = len(str(key))
            print(f"    pos [{pos}:{pos+key_len}] "
                  f"key = {key:3d}  →  n_i = {key//5:2d}  →  '{char}'")
            found_chars.append((label, pos, key, char))

        # Show zero positions (special characters)
        for i, d in enumerate(serial):
            if d == "0":
                print(f"    pos [{i}:{i+1}] key = 0   →  n_i =  0  →  special")

        print()

    # Assemble flag from characters in reading order (left → right → bottom)
    content = "".join(char for _, _, _, char in found_chars)

    print("[STEP 4]  Decoded characters (left → right → bottom serial):")
    for label, pos, key, char in found_chars:
        print(f"  '{char}'  (key={key}, from {label.strip()}, pos {pos})")

    print()
    flag = f"ATHACKCTF{{{content}}}"
    print("=" * 60)
    print(f"  FLAG:  {flag}")
    print("=" * 60)
    return flag


# ---------------------------------------------------------------------------
# Character encoding reference table
# ---------------------------------------------------------------------------

def print_encoding_table():
    """Print the full encoding table for reference."""
    print("\nFull encoding table (key → character):")
    print(f"  {'key':>5}  {'n_i':>4}  {'y':>4}  char")
    print(f"  {'---':>5}  {'---':>4}  {'---':>4}  ----")
    # key = 0: special
    print(f"  {'0':>5}  {'0':>4}  {'—':>4}  (special: brackets, underscores, …)")
    for n_i in range(1, 45):
        key = 5 * n_i
        y = 45 + n_i
        print(f"  {key:>5}  {n_i:>4}  {y:>4}  '{chr(y)}'")


# ---------------------------------------------------------------------------
# Entry point
# ---------------------------------------------------------------------------

if __name__ == "__main__":
    flag = solve()
    # Uncomment to see the full encoding reference table:
    # print_encoding_table()
