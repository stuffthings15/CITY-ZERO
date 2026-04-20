# CITY//ZERO Unity Master Pack

This master pack consolidates all previously generated CITY//ZERO Unity starter exports into one place.

## Included Packages
- Production Package
- Code Scaffold
- Next Pack
- Compile Hardening Pack
- Scene and Cleanup Pack
- Vertical Slice Wiring Pack
- Editor Workflow Pack

## Folder Layout
- `MergedProject/` — combined file tree for a single import workflow
- `OriginalPackages/` — untouched copies of each previous package
- `Docs/` — master guides, overlap notes, conflict notes

## Recommended Use
Use `MergedProject/` as your working reference/import source.
Use `OriginalPackages/` only if you want to inspect the original package boundaries.

## Import / Merge Order
1. Review `Docs/MASTER_IMPORT_ORDER.md`
2. Review `Docs/KNOWN_OVERLAPS_AND_DECISIONS.md`
3. Import the code under `MergedProject/Assets/_Project/`
4. Import configs and docs
5. Let Unity reimport the `.inputactions` file
6. Resolve any compile issues noted in the docs

## Important Note
Some files intentionally overlap across packages because later packs introduced safer or more integrated variants.
Where conflicts existed and contents differed, both versions were preserved in `MergedProject/` using a `__from_<package>` suffix.
