# CITY//ZERO — GitHub Actions Secrets

## No Secrets Required for Core CI

CITY//ZERO uses **Godot 4** (open-source, no license activation) and **xUnit** for testing.  
The CI workflows in `.github/workflows/` require **zero repository secrets** to run.

---

## Workflows Summary

| Workflow | File | Triggers | Secrets Needed |
|----------|------|----------|----------------|
| .NET Build & Test | `dotnet-build.yml` | push/PR to main, develop | None |
| Godot CI | `godot-ci.yml` | push/PR to main, develop | None |

---

## Optional: Deployment Secrets

If you add automated deployment to itch.io or Steam, set these:

```
ITCHIO_API_KEY     — itch.io Butler API key (for itch.io deploy)
STEAM_USERNAME     — Steam partner account (for Steamworks deploy)
STEAM_CONFIG_VDF   — Base64-encoded Steam config.vdf (for Steamworks deploy)
```

Set secrets via:
```bash
gh secret set ITCHIO_API_KEY -b "<your_key>"
```

Or via **Settings → Secrets & variables → Actions → New repository secret**.

---

## No Unity Secrets

The old `unity-ci.yml` (UNITY_EMAIL, UNITY_PASSWORD, UNITY_SERIAL) has been replaced  
by `godot-ci.yml`. Unity is no longer the engine for this project.

