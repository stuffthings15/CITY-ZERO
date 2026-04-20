#!/usr/bin/env python3
"""
CITY//ZERO — Asset Naming Convention Linter
Checks all files in assets/, audio/, data/ for naming convention compliance.
Run: python scripts/check_naming.py
"""

import os
import sys
import re
from pathlib import Path

REPO_ROOT = Path(__file__).parent.parent

# Naming rules per directory prefix
RULES = {
    "assets/":  r"^(mdl|tex|mat|col)_[a-z][a-z0-9_]*\.[a-z0-9]+$",
    "audio/sfx": r"^snd_[a-z][a-z0-9_]*\.ogg$",
    "audio/music": r"^mus_[a-z][a-z0-9_]*\.ogg$",
    "audio/ambient": r"^amb_[a-z][a-z0-9_]*\.ogg$",
    "audio/vo": r"^vo_[a-z][a-z0-9_]*\.ogg$",
    "data/": r"^[a-z][a-z0-9_]*\.json$",
    "scenes/": r"^(scn_)?[a-z][a-z0-9_]*\.tscn$",
}

IGNORED_EXTENSIONS = {".import", ".uid", ".md", ".py", ".cs", ".cfg", ".gitkeep"}
IGNORED_DIRS = {".godot", ".git", ".vs", "__pycache__", "builds"}

def check_file(rel_path: str, filename: str) -> str | None:
    ext = Path(filename).suffix.lower()
    if ext in IGNORED_EXTENSIONS:
        return None

    for prefix, pattern in RULES.items():
        if rel_path.startswith(prefix):
            if not re.match(pattern, filename):
                return f"  ✗ {rel_path}/{filename}  (expected: {pattern})"
            return None
    return None  # No rule applies; skip

def main():
    print("=" * 60)
    print("CITY//ZERO — Naming Convention Lint")
    print("=" * 60)

    violations = []

    for root, dirs, files in os.walk(REPO_ROOT):
        # Prune ignored dirs
        dirs[:] = [d for d in dirs if d not in IGNORED_DIRS]

        rel_root = os.path.relpath(root, REPO_ROOT).replace("\\", "/")

        for filename in files:
            result = check_file(rel_root, filename)
            if result:
                violations.append(result)

    if violations:
        print(f"\nFound {len(violations)} naming violation(s):\n")
        for v in violations:
            print(v)
        print()
        sys.exit(1)
    else:
        print("\n✓ All checked files conform to naming conventions.")

if __name__ == "__main__":
    main()
