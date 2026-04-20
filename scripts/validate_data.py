#!/usr/bin/env python3
"""
CITY//ZERO — JSON Data Validator
Validates all JSON data files against their schemas before builds.
Run: python scripts/validate_data.py
"""

import json
import os
import sys
from pathlib import Path

try:
    import jsonschema
except ImportError:
    print("jsonschema not installed. Run: pip install jsonschema")
    sys.exit(1)

REPO_ROOT = Path(__file__).parent.parent
DATA_DIR = REPO_ROOT / "data"
SCHEMA_DIR = REPO_ROOT / "docs" / "schemas"

SCHEMA_MAP = {
    "missions":  "mission.schema.json",
    "vehicles":  "vehicle.schema.json",
    "weapons":   "weapon.schema.json",
    "factions":  "faction.schema.json",
    "districts": "district.schema.json",
}

def load_json(path: Path):
    with open(path, "r", encoding="utf-8") as f:
        return json.load(f)

def validate_directory(category: str, schema_file: str) -> tuple[int, int]:
    schema_path = SCHEMA_DIR / schema_file
    if not schema_path.exists():
        print(f"  [WARN] Schema not found: {schema_path}. Skipping {category}.")
        return 0, 0

    schema = load_json(schema_path)
    data_dir = DATA_DIR / category

    if not data_dir.exists():
        print(f"  [INFO] Data directory not found: {data_dir}. Skipping.")
        return 0, 0

    passed = 0
    failed = 0

    for json_file in sorted(data_dir.glob("*.json")):
        try:
            data = load_json(json_file)
            jsonschema.validate(instance=data, schema=schema)
            print(f"  ✓ {json_file.name}")
            passed += 1
        except jsonschema.ValidationError as e:
            print(f"  ✗ {json_file.name}: {e.message} (at {list(e.path)})")
            failed += 1
        except json.JSONDecodeError as e:
            print(f"  ✗ {json_file.name}: JSON parse error — {e}")
            failed += 1

    return passed, failed

def main():
    print("=" * 60)
    print("CITY//ZERO — Data Validation")
    print("=" * 60)

    total_passed = 0
    total_failed = 0

    for category, schema_file in SCHEMA_MAP.items():
        print(f"\n[{category.upper()}]")
        p, f = validate_directory(category, schema_file)
        total_passed += p
        total_failed += f

    print("\n" + "=" * 60)
    print(f"Results: {total_passed} passed | {total_failed} failed")
    print("=" * 60)

    if total_failed > 0:
        sys.exit(1)

if __name__ == "__main__":
    main()
