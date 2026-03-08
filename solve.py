#!/usr/bin/env python3
"""
CTF Challenge Solver: Bowling Base64
Flag format: ATHACKCTF{...}
Hint: "These aliens from Base 64 man, they're really bad at bowling.
       I mean, just go frame-by-frame to see what I mean."

Tries 20+ approaches to extract the flag from bowling scorecards.
"""

import base64
import re

# ---------------------------------------------------------------------------
# Bowling scorecard data — 101 lines, 10 frames each
# ---------------------------------------------------------------------------
DATA = """\
-2 4/ 4/ 3/ -2 1- 2- 6/ 52 9/X
7/ -7 4/ 9/ 13 6- 3/ 23 2/ 12
6/ 9/ 15 -5 5/ 44 -- 8/ 9/ 35
-9 8/ 54 12 18 9/ 7/ 13 6/ 44
4/ 63 X 1- 53 9/ 45 -8 7/ -2
-8 -- 6/ 44 7- 7/ 7/ 1/ 72 81
-3 X 9- 5/ 1- 12 14 2- 1/ 31
-1 1/ 5/ X 9/ 7- 7/ 8/ 22 3/8
5/ 2/ 2/ 4/ 15 71 31 7/ 8/ 31
26 44 52 9/ 41 X 53 X 7/ 5/4
9- 6- 3- 6/ -4 52 9/ -9 -1 5/6
6/ 9/ 26 8- 7/ X 6/ 4/ 8/ 63
4/ -8 9/ X 3/ 6/ 31 9/ 22 5/7
7/ 3/ 7/ 9/ 35 9/ 7/ 2- 44 9/2
5/ X 8/ 7/ 62 X 63 62 X 5-
7- 8/ 51 5/ X 7/ X X 54 9/9
7/ 6/ 4/ 22 18 35 X X -/ X23
3- 72 5/ 72 7/ 4/ 33 7/ 32 9/X
72 8/ X 9/ 6/ 71 6/ 43 9/ 8/5
4/ 9- 8/ 9/ 9/ 41 35 72 9- 1-
8/ 3/ 11 3/ 53 12 1/ 7/ 9/ 5/6
45 -8 8/ 4/ 17 X 33 2- 7/ 25
7/ 14 16 24 6/ X 51 11 53 3/3
23 1- 81 6- 44 9/ 27 8/ 41 3-
9/ 1- 36 52 X 4/ 4/ 41 9/ 16
7/ 4/ -/ 6/ 9/ 8/ 4/ 9/ 63 5/8
16 53 2/ 36 24 14 9/ X 6- -9
9- 5/ 8/ 2/ 4- 7- 8/ X 34 6/7
X 7/ 8/ 34 6/ 7/ 9/ X X 7/4
X 7/ 26 4/ 5/ 71 7- -3 9/ 6/9
8/ 8- 3- 6- 54 -1 7/ 1- 8/ 7-
7/ 9- 8/ 9/ 72 41 32 4/ 8/ 6/-
7/ X X 6/ 7/ 15 62 3- 9- 44
17 6/ 17 81 63 X -8 -7 4- 32
-- X -9 32 7/ X 1/ 5/ X 9/-
6- 12 X X -7 -3 15 53 54 9-
6- 23 7/ 52 X 5/ 8/ -- 9/ -1
81 4/ X 81 62 4- 2/ -1 -9 9/8
4/ 14 1- 9/ 6/ 11 52 7/ 17 8/1
X 72 -3 27 7/ 9/ 3/ 2- 7/ 16
1/ 5/ 6/ 25 6/ 8/ 8/ 6/ 3/ 6-
4/ 3/ 27 8- 7/ 5/ 7/ -- 8/ 3/-
-4 -/ 13 -2 -/ 9/ 7/ 4/ 35 -2
-/ 52 -8 -/ 1/ 4/ 8/ 4/ 52 XX4
16 -9 X -6 14 -8 1/ 7/ 12 -/9
4/ 2- 4/ 7/ 36 X 8- 13 11 13
7/ 63 42 34 5- 62 61 53 7/ 44
5/ X 7/ 1- 54 8/ 25 6/ 5/ 8/6
X 43 9/ 9/ 4- 53 4/ X 23 2/7
9/ 8/ 4/ 9/ X X 51 17 42 61
3/ 12 35 -2 X 6/ 34 X 36 4/2
7/ -4 X 71 5/ 36 X 7- 7/ 7/3
-1 X 25 8/ 9/ 6/ 61 5/ 9- 7/X
43 -3 25 72 8/ 9/ 4/ -6 9/ 42
5/ 1/ 36 -3 8/ X 4- 5- -5 5/6
-3 7/ X 43 45 26 22 42 81 8/7
14 X 7/ 8/ 9/ 8/ 8/ 32 7/ 31
3/ -9 3/ 1/ 8/ 8/ X 5/ 6/ -1
7/ 44 -3 -8 16 7/ 8/ 6/ 33 42
X X 7/ 8/ 7/ 33 9/ -6 5/ X17
3- 7/ 4- 4/ 27 34 35 7/ 8/ 16
21 4/ 36 52 5/ 9/ 35 -2 3- X63
3/ 27 2/ X 72 72 53 9/ 45 27
71 X X 3/ X 71 7/ 8/ -2 -6
7/ 3/ 72 21 36 41 71 2/ 4/ 7/3
X 4/ 18 2- 6/ -4 33 8/ 8/ 4/3
6/ 1/ 34 21 X 43 9/ 2/ X 4/3
2/ 6/ 4/ 7/ 5/ 3/ 9/ -1 6/ 3-
12 8/ 8/ 9/ 6/ 7/ 2/ 1- -1 5/1
26 7/ 12 4/ 1/ X 72 7/ 2- 33
9/ 16 -6 -- 36 44 7/ 5/ 23 32
45 9/ 5/ 8/ 9/ 21 42 -6 8/ 6-
-7 8- 9/ 9/ 8/ 9/ 8/ 4/ 25 1-
9- 52 6- 5/ 4/ X 8/ 9/ 9/ -7
63 2/ 71 6/ 9/ -5 -6 3/ 13 61
7- 9/ 8/ 4/ 45 9/ 7/ 2/ 2/ 21
X 7/ 4/ 44 8/ 9/ 5/ 72 61 54
31 24 9/ 12 2/ 9/ 8- 7/ 17 X1-
72 3/ 1/ 8/ 7/ 42 6/ 13 -/ 63
6/ 35 3/ 5/ 9/ 9/ 71 X 6/ 9/2
43 X 34 X 3- 51 -6 18 X 44
25 X 31 6/ 45 5- 51 5/ X 6/4
45 X X -8 7/ -2 33 5/ -9 5/2
51 27 18 -2 5/ 14 24 25 3/ -9
9/ 5/ 72 8/ 9/ 21 X 9- 8/ 6/X
4/ 7/ 8/ 9/ 4- 7/ X 53 9/ 6/-
1/ 9/ 3/ X 9/ 6/ 6/ 81 22 4/7
16 8/ 72 4/ 6/ -3 72 72 9/ 5-
-6 8- 4- 11 33 9/ -9 27 23 63
2- 18 5- X 72 -4 27 8/ 42 1-
34 3- 81 5/ 33 17 -5 5/ 8/ 24
31 3/ X 17 26 9/ 8/ 9/ 3/ 7-
8/ 14 2/ 21 25 9/ 8/ 6/ -9 1-
X X X 18 63 -1 8/ -4 5/ 2-
2/ X 13 5- 8- -4 9/ 18 X 9/5
9/ 8/ 5/ X 42 27 62 2/ 6/ X26
3/ 6/ 11 4/ 2/ 1/ -/ 4/ 9/ 45
-6 2/ 4/ 6/ 7- 6/ 8- 52 42 41
5/ 5- 5/ 16 1/ 9/ 41 -5 34 9/4
26 7/ 72 11 -2 X 44 7/ 5- 41
7/ 3- 25 X 8/ 52 31 7/ 24 9/5"""

# ---------------------------------------------------------------------------
# Constants
# ---------------------------------------------------------------------------
B64 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/"
FLAG_RE = re.compile(r'ATHACK[A-Za-z0-9{]')

# ---------------------------------------------------------------------------
# Utilities
# ---------------------------------------------------------------------------

def sd(s, urlsafe=False):
    """Safe base64 decode with auto-padding."""
    s = s.strip()
    r = len(s) % 4
    if r == 1:
        s = s + "AAA"
    elif r == 2:
        s = s + "=="
    elif r == 3:
        s = s + "="
    try:
        if urlsafe:
            return base64.urlsafe_b64decode(s)
        return base64.b64decode(s)
    except Exception:
        return None


def flag_in(data_bytes):
    """Return True + print if flag is found in bytes."""
    if data_bytes is None:
        return False
    try:
        t = data_bytes.decode("latin-1")
    except Exception:
        return False
    if FLAG_RE.search(t):
        return True
    # Also check double-decoded
    for sub in ["+", "A", ""]:
        s2 = t.replace("-", sub) if sub else t.replace("-", "")
        d2 = sd(s2)
        if d2:
            try:
                t2 = d2.decode("latin-1")
                if FLAG_RE.search(t2):
                    return True
            except Exception:
                pass
    return False


def report(label, data_bytes):
    """Check bytes for flag and print full result."""
    if data_bytes is None:
        return
    try:
        t = data_bytes.decode("latin-1")
    except Exception:
        return
    m = FLAG_RE.search(t)
    if m:
        print(f"\n{'='*60}")
        print(f"*** FLAG FOUND [{label}] ***")
        print(f"{'='*60}")
        print(f"  Match: {m.group()!r}")
        print(f"  Full:  {t!r}")
    # Try double-decode
    for sub in ["+", "A", ""]:
        s2 = t.replace("-", sub) if sub else t.replace("-", "")
        d2 = sd(s2)
        if d2:
            try:
                t2 = d2.decode("latin-1")
                m2 = FLAG_RE.search(t2)
                if m2:
                    print(f"\n*** FLAG (double-b64) [{label} -{sub!r}] ***")
                    print(f"  Match: {m2.group()!r}")
                    print(f"  Full:  {t2!r}")
            except Exception:
                pass


def report_str(label, s):
    """Check string for flag, and also try b64-decoding it."""
    m = FLAG_RE.search(s)
    if m:
        print(f"\n*** FLAG_str [{label}]: {m.group()!r}")
        print(f"  Full: {s!r}")
    for urlsafe in [False, True]:
        d = sd(s, urlsafe=urlsafe)
        if d:
            report(label + (" url" if urlsafe else ""), d)


# ---------------------------------------------------------------------------
# Bowling parsing helpers
# ---------------------------------------------------------------------------

def parse_lines(data):
    return [line.split() for line in data.strip().splitlines()]


def ball_pins(ch, prev=None):
    if ch == "X":
        return 10
    if ch == "-":
        return 0
    if ch == "/":
        return 10 - (prev or 0)
    return int(ch)


def parse_frame_balls(frame_str, frame_idx):
    """Return list of (pin_count, char) for all balls in a frame."""
    s = frame_str
    if frame_idx < 9:
        if s == "X":
            return [(10, "X")]
        p1 = ball_pins(s[0])
        p2 = ball_pins(s[1], p1) if len(s) > 1 else 0
        return [(p1, s[0]), (p2, s[1] if len(s) > 1 else "-")]
    else:
        balls, prev = [], None
        for ch in s:
            p = ball_pins(ch, prev)
            balls.append((p, ch))
            prev = p
        return balls


def score_game(frames):
    """Official bowling score for a 10-frame game."""
    all_b = []
    for i, f in enumerate(frames):
        for p, _ in parse_frame_balls(f, i):
            all_b.append(p)
    total, bi = 0, 0
    for fr in range(9):
        if bi >= len(all_b):
            break
        if all_b[bi] == 10:
            total += 10 + (all_b[bi+1] if bi+1 < len(all_b) else 0) + \
                         (all_b[bi+2] if bi+2 < len(all_b) else 0)
            bi += 1
        elif bi+1 < len(all_b) and all_b[bi] + all_b[bi+1] == 10:
            total += 10 + (all_b[bi+2] if bi+2 < len(all_b) else 0)
            bi += 2
        else:
            total += all_b[bi] + (all_b[bi+1] if bi+1 < len(all_b) else 0)
            bi += 2
    total += sum(all_b[bi:])
    return total


def frame_scores_official(frames):
    """Return list of 10 per-frame official bowling scores."""
    all_b = []
    for i, f in enumerate(frames):
        for p, _ in parse_frame_balls(f, i):
            all_b.append(p)
    scores, bi = [], 0
    for fr in range(9):
        if bi >= len(all_b):
            scores.append(0)
            continue
        if all_b[bi] == 10:
            scores.append(10 + (all_b[bi+1] if bi+1 < len(all_b) else 0) +
                              (all_b[bi+2] if bi+2 < len(all_b) else 0))
            bi += 1
        elif bi+1 < len(all_b) and all_b[bi] + all_b[bi+1] == 10:
            scores.append(10 + (all_b[bi+2] if bi+2 < len(all_b) else 0))
            bi += 2
        else:
            scores.append(all_b[bi] + (all_b[bi+1] if bi+1 < len(all_b) else 0))
            bi += 2
    scores.append(sum(all_b[bi:]))
    return scores


def raw_pins(frame_str, frame_idx):
    return sum(p for p, _ in parse_frame_balls(frame_str, frame_idx))


def is_strike(f):
    return f.startswith("X")


def is_spare(f):
    return "/" in f and not f.startswith("X")


# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------

def main():
    lines = parse_lines(DATA)
    n = len(lines)
    print(f"Loaded {n} lines of bowling data")

    # Pre-compute
    all_concat = "".join("".join(frames) for frames in lines)
    raw_totals = [sum(raw_pins(f, i) for i, f in enumerate(line))
                  for line in lines]
    official_totals = [score_game(line) for line in lines]
    all_off_scr = [frame_scores_official(line) for line in lines]
    flat_off_scr = [s for row in all_off_scr for s in row]
    all_balls = [(p, c) for line in lines
                 for i, f in enumerate(line)
                 for p, c in parse_frame_balls(f, i)]
    ball_p = [p for p, _ in all_balls]
    ball_c = [c for _, c in all_balls]
    raw_frame_pins_list = [raw_pins(f, i) for line in lines
                           for i, f in enumerate(line)]

    print(f"Total chars (no spaces): {len(all_concat)}")
    print(f"Total balls: {len(ball_p)}")
    print(f"Raw totals range: [{min(raw_totals)}, {max(raw_totals)}]")
    print(f"Official totals range: [{min(official_totals)}, {max(official_totals)}]")
    print(f"Frame scores range: [{min(flat_off_scr)}, {max(flat_off_scr)}]")

    # =====================================================================
    # A: Concatenate all frame chars, try ALL substitutions for '-'
    # =====================================================================
    print("\n" + "="*60)
    print("A: All-chars concat, every substitution for '-'")
    for sub in B64:
        s = all_concat.replace("-", sub)
        d = sd(s)
        report(f"A std ->{sub!r}", d)
        d2 = sd(s.replace("/", "_"), urlsafe=True)
        report(f"A url ->{sub!r}", d2)
    # Reversed
    for sub in ["+", "A"]:
        s = all_concat.replace("-", sub)[::-1]
        report(f"A rev -{sub!r}", sd(s))
    print("  A done")

    # =====================================================================
    # B/C: First chars / second chars of all frames
    # =====================================================================
    print("\n" + "="*60)
    print("B/C: First/second/third chars of all frames")
    fc = "".join(f[0] for line in lines for f in line)
    sc = "".join(f[1] for line in lines for f in line if len(f) > 1)
    tc = "".join(f[2] for line in lines for f in line if len(f) > 2)
    print(f"  first({len(fc)}): {fc[:50]!r}...")
    print(f"  second({len(sc)}): {sc[:50]!r}...")
    print(f"  third({len(tc)}): {tc[:30]!r}...")
    for chars, lbl in [(fc, "first"), (sc, "second"), (tc, "third")]:
        for sub in ["+", "A", ""]:
            t = chars.replace("-", sub) if sub else chars.replace("-", "")
            report_str(f"{lbl} -{sub!r}", t)
            report_str(f"{lbl} url -{sub!r}", t.replace("/", "_"))

    # =====================================================================
    # D: Column-by-column extraction
    # =====================================================================
    print("\n" + "="*60)
    print("D: Column-by-column (all frame-N strings from all 101 lines)")
    for col in range(10):
        col_str = "".join("".join(lines[row][col]) for row in range(n))
        print(f"  Col {col} ({len(col_str)}): {col_str[:25]!r}...")
        for sub in ["+", "A", "K"]:
            report(f"D col{col} -{sub!r}", sd(col_str.replace("-", sub)))
            report(f"D col{col} url -{sub!r}",
                   sd(col_str.replace("-", sub).replace("/", "_"), urlsafe=True))

    # =====================================================================
    # E: Binary encoding from frame types
    # =====================================================================
    print("\n" + "="*60)
    print("E: Binary encoding")
    for fn_name, fn in [
        ("strike",  lambda f: is_strike(f)),
        ("spare",   lambda f: is_spare(f)),
        ("ss",      lambda f: is_strike(f) or is_spare(f)),
        ("open",    lambda f: not is_strike(f) and not is_spare(f)),
        ("dash",    lambda f: "-" in f),
    ]:
        for inv in [False, True]:
            bits = [(0 if inv else 1) if fn(f) else (1 if inv else 0)
                    for line in lines for f in line]
            bstr = "".join(str(b) for b in bits)
            for start in range(8):
                sl = bstr[start:]
                bdata = bytes(int(sl[i:i+8], 2)
                              for i in range(0, len(sl)-7, 8))
                report(f"E {fn_name} inv={inv} s={start}", bdata)

    # Per-line: 10 bits per row → 8-bit window
    for fn_name, fn in [("ss", lambda f: is_strike(f) or is_spare(f)),
                         ("strike", is_strike),
                         ("dash", lambda f: "-" in f)]:
        for start in range(3):
            bdata = bytes(
                int("".join([str(1 if fn(f) else 0) for f in line])[start:start+8], 2)
                for line in lines)
            report(f"E_per_line {fn_name} s={start}", bdata)

    # =====================================================================
    # F: Official per-frame scores → B64 indices → decode
    # =====================================================================
    print("\n" + "="*60)
    print("F: Official frame scores as B64 indices")
    mn_s, mx_s = min(flat_off_scr), max(flat_off_scr)
    print(f"  Frame score range: [{mn_s}, {mx_s}]")
    # Every valid offset
    for off in range(-mn_s, 64 - mx_s):
        s = "".join(B64[v + off] for v in flat_off_scr)
        report(f"F frame_scr+{off}", sd(s))
    # Transposed (column-first)
    trans = [all_off_scr[row][col] for col in range(10) for row in range(n)]
    for off in range(-mn_s, 64 - mx_s):
        s = "".join(B64[v + off] for v in trans)
        report(f"F trans_scr+{off}", sd(s))
    # mod 64
    report("F scr%64", sd("".join(B64[v % 64] for v in flat_off_scr)))
    report("F trans%64", sd("".join(B64[v % 64] for v in trans)))

    # =====================================================================
    # G: Raw totals / official totals → B64 string → decode
    # =====================================================================
    print("\n" + "="*60)
    print("G: Totals as B64 indices")
    mn_r, mx_r = min(raw_totals), max(raw_totals)
    mn_o, mx_o = min(official_totals), max(official_totals)
    print(f"  Raw range span={mx_r-mn_r}, Official range span={mx_o-mn_o}")
    for off in range(-mn_r, 64 - mx_r):
        report(f"G raw+{off}", sd("".join(B64[v+off] for v in raw_totals)))
    for off in range(-mn_o, 64 - mx_o):
        report(f"G off+{off}", sd("".join(B64[v+off] for v in official_totals)))
    report("G raw%64", sd("".join(B64[v % 64] for v in raw_totals)))
    report("G off%64", sd("".join(B64[v % 64] for v in official_totals)))

    # =====================================================================
    # H: Totals as ASCII chars (offset search)
    # =====================================================================
    print("\n" + "="*60)
    print("H: Totals ± offset → ASCII")
    for off in range(-300, 300):
        t_r = bytes((v + off) % 256 for v in raw_totals)
        if FLAG_RE.search(t_r.decode("latin-1")):
            print(f"  FLAG raw_total off={off}: "
                  f"{t_r.decode('latin-1')!r}")
        t_o = bytes((v + off) % 256 for v in official_totals)
        if FLAG_RE.search(t_o.decode("latin-1")):
            print(f"  FLAG off_total off={off}: "
                  f"{t_o.decode('latin-1')!r}")
    print("  H done")

    # =====================================================================
    # I: Ball pin values → various byte encodings
    # =====================================================================
    print("\n" + "="*60)
    print("I: Ball pin values → bytes")
    # Hex pairs: pin 0-10 → hex digit, pair → byte
    hex_str = "".join("0123456789A"[min(p, 10)] for p in ball_p)
    try:
        hb = bytes.fromhex(hex_str[:len(hex_str)//2*2])
        report("I hex_pairs", hb)
    except Exception:
        pass
    # Pair consecutive pins as (p1*11+p2)%256
    pairs11 = bytes((ball_p[i]*11 + ball_p[i+1]) % 256
                    for i in range(0, len(ball_p)-1, 2))
    report("I pairs_x11", pairs11)
    # (p1*10+p2)%256
    pairs10 = bytes((ball_p[i]*10 + ball_p[i+1]) % 256
                    for i in range(0, len(ball_p)-1, 2))
    report("I pairs_x10", pairs10)
    # nibble pairs from raw frame totals
    nibbles = bytes(((raw_frame_pins_list[i] & 0xF) << 4) |
                    (raw_frame_pins_list[i+1] & 0xF)
                    for i in range(0, len(raw_frame_pins_list)-1, 2))
    report("I nibbles", nibbles)
    # Per ball as B64[pin_count]
    ball_b64 = "".join(B64[p % 64] for p in ball_p)
    report("I ball_b64_dec", sd(ball_b64))

    # =====================================================================
    # J: Threshold-based binary encoding
    # =====================================================================
    print("\n" + "="*60)
    print("J: Threshold-based binary (per-frame)")
    for thresh in range(0, 12):
        bits_f = [1 if raw_pins(f, i) >= thresh else 0
                  for line in lines for i, f in enumerate(line)]
        bstr = "".join(str(b) for b in bits_f)
        for start in range(4):
            sl = bstr[start:]
            bdata = bytes(int(sl[i:i+8], 2)
                          for i in range(0, len(sl)-7, 8))
            report(f"J thresh={thresh} s={start}", bdata)

    # =====================================================================
    # K: Chars after/before '/' in concatenated string
    # =====================================================================
    print("\n" + "="*60)
    print("K: Chars around '/'")
    after_slash = "".join(all_concat[i+1]
                          for i in range(len(all_concat)-1)
                          if all_concat[i] == "/")
    before_slash = "".join(all_concat[i-1]
                           for i in range(1, len(all_concat))
                           if all_concat[i] == "/")
    print(f"  after_slash ({len(after_slash)}): {after_slash[:40]!r}...")
    print(f"  before_slash ({len(before_slash)}): {before_slash[:40]!r}...")
    for chars, lbl in [(after_slash, "after/"), (before_slash, "before/")]:
        for sub in ["+", "A", "K"]:
            report_str(f"K_{lbl} -{sub!r}", chars.replace("-", sub))

    # =====================================================================
    # L: Every Nth character extraction
    # =====================================================================
    print("\n" + "="*60)
    print("L: Every Nth char from concat")
    for step in range(2, 8):
        for off in range(step):
            s = all_concat[off::step].replace("-", "+")
            report(f"L step={step} off={off}", sd(s))
            report(f"L url step={step} off={off}",
                   sd(all_concat[off::step].replace("-", "-").replace("/", "_"),
                      urlsafe=True))

    # =====================================================================
    # M: Interleaved / strided row selection
    # =====================================================================
    print("\n" + "="*60)
    print("M: Strided row selection")
    for step in range(2, 6):
        for start in range(step):
            sel = [lines[i] for i in range(start, n, step)]
            s = "".join("".join(line) for line in sel).replace("-", "+")
            report(f"M step={step} start={start}", sd(s))

    # =====================================================================
    # N: Strike/spare/open chars only concat
    # =====================================================================
    print("\n" + "="*60)
    print("N: Filtered frame types concat")
    ss_chars = "".join("".join(f) for line in lines for f in line
                       if is_strike(f) or is_spare(f))
    open_chars = "".join("".join(f) for line in lines for f in line
                         if not is_strike(f) and not is_spare(f))
    for chars, lbl in [(ss_chars, "ss"), (open_chars, "open")]:
        for sub in ["+", "A"]:
            report_str(f"N_{lbl} -{sub!r}", chars.replace("-", sub))

    # =====================================================================
    # O: Per-line b64 decode → collect first byte per line
    # =====================================================================
    print("\n" + "="*60)
    print("O: Per-line b64 decode, various strategies")
    for sub in ["+", "A", "K", "X"]:
        # Collect ALL decoded bytes from each line
        all_collected = b""
        first_bytes = []
        for line in lines:
            raw = "".join(line).replace("-", sub)
            d = sd(raw)
            if d:
                all_collected += d
                first_bytes.append(d[0])
        report(f"O all_decoded -{sub!r}", all_collected)
        if first_bytes:
            report(f"O first_bytes -{sub!r}", bytes(first_bytes))
    # URL-safe variant
    for sub in ["-", "A"]:
        all_col = b""
        for line in lines:
            raw = "".join(line).replace("/", "_").replace("-", sub)
            d = sd(raw, urlsafe=True)
            if d:
                all_col += d
        report(f"O url_all_{sub!r}", all_col)

    # =====================================================================
    # P: Unique frame notation index mapping
    # =====================================================================
    print("\n" + "="*60)
    print("P: Unique frame notation index")
    all_f = [f for line in lines for f in line]
    unique_f = sorted(set(all_f))
    print(f"  {len(unique_f)} unique frame notations")
    if len(unique_f) <= 64:
        s = "".join(B64[unique_f.index(f)] for f in all_f)
        report("P unique_idx_b64", sd(s))
    # Map by raw_pins value mod 64
    s2 = "".join(B64[raw_pins(f, 0) % 64] for f in all_f)
    report("P rawpin_b64", sd(s2))
    # Official frame score mod 64 for each individual frame
    s3 = "".join(B64[raw_pins(f, i) % 64]
                 for line in lines for i, f in enumerate(line))
    report("P rawpin_indexed", sd(s3))

    # =====================================================================
    # Q: 10th frame analysis
    # =====================================================================
    print("\n" + "="*60)
    print("Q: 10th frame analysis")
    tenth = [line[9] for line in lines]
    tenth_cat = "".join(tenth)
    print(f"  10th frames: {tenth_cat!r}")
    for sub in ["+", "A", "K"]:
        report_str(f"Q 10th -{sub!r}", tenth_cat.replace("-", sub))
        report_str(f"Q 10th url -{sub!r}",
                   tenth_cat.replace("-", sub).replace("/", "_"))
    last_chars = "".join(f[-1] for f in tenth)
    first_chars_10th = "".join(f[0] for f in tenth)
    for chars, lbl in [(last_chars, "last"), (first_chars_10th, "first")]:
        for sub in ["+", "A"]:
            report_str(f"Q 10th_{lbl} -{sub!r}", chars.replace("-", sub))

    # =====================================================================
    # R: Diagonals
    # =====================================================================
    print("\n" + "="*60)
    print("R: Diagonal reads")
    diag1 = "".join("".join(lines[i][i % 10]) for i in range(n))
    diag2 = "".join("".join(lines[i][(9 - i % 10)]) for i in range(n))
    for diag, lbl in [(diag1, "main_diag"), (diag2, "anti_diag")]:
        for sub in ["+", "A"]:
            report_str(f"R {lbl} -{sub!r}", diag.replace("-", sub))

    # =====================================================================
    # S: XOR between adjacent line totals
    # =====================================================================
    print("\n" + "="*60)
    print("S: XOR/difference operations")
    xor_r = [raw_totals[i] ^ raw_totals[i+1] for i in range(n-1)]
    diff_r = [abs(raw_totals[i+1] - raw_totals[i]) for i in range(n-1)]
    for vals, lbl in [(xor_r, "xor_raw"), (diff_r, "diff_raw")]:
        for off in range(-20, 21):
            t = bytes((v + off) % 256 for v in vals)
            if FLAG_RE.search(t.decode("latin-1")):
                print(f"  FLAG {lbl} off={off}: {t.decode('latin-1')!r}")

    # =====================================================================
    # T: Official totals as ASCII forming a b64 string
    # =====================================================================
    print("\n" + "="*60)
    print("T: Totals as ASCII of a base64 string")
    ot_str = "".join(chr(v) for v in official_totals if 33 <= v <= 126)
    rt_str = "".join(chr(v) for v in raw_totals)
    print(f"  official_totals as chars: {ot_str!r}")
    print(f"  raw_totals as chars:      {rt_str!r}")
    report_str("T off_as_b64str", ot_str)
    report_str("T raw_as_b64str", rt_str)

    # =====================================================================
    # U: Per-frame score × 2 (spread 0-60 in B64)
    # =====================================================================
    print("\n" + "="*60)
    print("U: Frame score × 2 and other scalings")
    for mult in [1, 2, 3]:
        s = "".join(B64[min(v*mult, 63)] for v in flat_off_scr)
        report(f"U scr*{mult}", sd(s))
        s2 = "".join(B64[min(v*mult, 63)] for v in trans)
        report(f"U trans_scr*{mult}", sd(s2))

    # =====================================================================
    # V: Column score per-row aggregation
    # =====================================================================
    print("\n" + "="*60)
    print("V: Column frame scores as ASCII")
    for col in range(10):
        col_scr = [all_off_scr[row][col] for row in range(n)]
        mn_c, mx_c = min(col_scr), max(col_scr)
        for off in range(-mn_c, 64-mx_c):
            s = "".join(B64[v+off] for v in col_scr)
            report(f"V col{col}+{off}", sd(s))

    # =====================================================================
    # W: Sliding window blocks
    # =====================================================================
    print("\n" + "="*60)
    print("W: Sliding window blocks")
    for block_size in [4, 8, 10, 12, 16, 20]:
        for start_line in range(0, n, block_size):
            block = lines[start_line:start_line+block_size]
            for sub in ["+"]:
                s = "".join("".join(line) for line in block).replace("-", sub)
                d = sd(s)
                if d and FLAG_RE.search(d.decode("latin-1")):
                    print(f"  FLAG block{block_size}_line{start_line}: "
                          f"{d.decode('latin-1')!r}")

    # =====================================================================
    # X: Comprehensive exhaustive match for ATHACKCTF{
    # =====================================================================
    print("\n" + "="*60)
    print("X: Exhaustive exact ATHACKCTF match search")
    target = b"ATHACKCTF{"
    for totals, lbl in [(raw_totals, "raw"), (official_totals, "off"),
                        (flat_off_scr, "frame_scr")]:
        for start in range(len(totals) - len(target) + 1):
            chunk = totals[start:start+len(target)]
            for off in range(-256, 256):
                adj = bytes((v + off) % 256 for v in chunk)
                if adj == target:
                    print(f"  PERFECT MATCH {lbl} off={off} pos={start}!")

    # =====================================================================
    # Y: Literal base64 char mapping for each ball/frame
    # =====================================================================
    print("\n" + "="*60)
    print("Y: Literal B64 char value for each character")

    def char_b64_idx(c, dash_as=0):
        if c == "X":
            return 23
        if c == "/":
            return 63
        if c == "-":
            return dash_as
        if c.isdigit():
            return 52 + int(c)  # '0'→52,'1'→53,...,'9'→61
        if c.isupper():
            return ord(c) - ord("A")
        if c.islower():
            return 26 + ord(c) - ord("a")
        return 0

    for dash_as in [0, 62, 10]:
        # All balls
        s = "".join(B64[char_b64_idx(c, dash_as)] for c in ball_c)
        report(f"Y all_balls dash={dash_as}", sd(s))
        # First char of each frame
        s2 = "".join(B64[char_b64_idx(f[0], dash_as)]
                     for line in lines for f in line)
        report(f"Y first_chars dash={dash_as}", sd(s2))
        # Second char of each frame (non-strikes)
        s3 = "".join(B64[char_b64_idx(f[1], dash_as)]
                     for line in lines for f in line if len(f) > 1)
        report(f"Y second_chars dash={dash_as}", sd(s3))
        # Column-by-column first chars
        for col in range(10):
            col_chars = "".join(
                B64[char_b64_idx(lines[row][col][0], dash_as)]
                for row in range(n))
            report(f"Y col{col}_first dash={dash_as}", sd(col_chars))

    # =====================================================================
    # Z: Final summary print of decoded samples
    # =====================================================================
    print("\n" + "="*60)
    print("Z: Decoded content samples (first 80 chars of key approaches)")
    key_approaches = []
    # raw totals - min as b64 chars
    mn = min(raw_totals)
    s = "".join(B64[v-mn] for v in raw_totals)
    d = sd(s)
    if d:
        print(f"  raw_totals-min decoded: {d.decode('latin-1')[:80]!r}")
        key_approaches.append(("raw_totals-min", d))
    # official totals mod 64 as b64 chars
    s2 = "".join(B64[v % 64] for v in official_totals)
    d2 = sd(s2)
    if d2:
        print(f"  official%64 decoded:    {d2.decode('latin-1')[:80]!r}")
        key_approaches.append(("official%64", d2))
    # All chars concat with - -> +
    d3 = sd(all_concat.replace("-", "+"))
    if d3:
        print(f"  all_concat (-→+):       {d3.decode('latin-1')[:80]!r}")
        key_approaches.append(("all_concat", d3))
    # Check double-decode of all key approaches
    for lbl, d in key_approaches:
        for sub in ["+", "A"]:
            try:
                t = d.decode("latin-1").replace("-", sub)
                d4 = sd(t)
                if d4 and FLAG_RE.search(d4.decode("latin-1")):
                    print(f"  DOUBLE-DECODE FLAG {lbl}: "
                          f"{d4.decode('latin-1')!r}")
            except Exception:
                pass

    print("\n" + "="*60)
    print("All approaches completed.")
    print("If no FLAG FOUND was printed above, flag was not located.")
    print("="*60)


if __name__ == "__main__":
    main()
