Setting GitHub Actions secrets for Unity CI

The unity-ci workflow requires Unity activation credentials. You must set these as repository secrets before the workflow will run successfully.

Required secrets (example names used in .github/workflows/unity-ci.yml):
- UNITY_EMAIL
- UNITY_PASSWORD
- UNITY_SERIAL

Optional environment variable to set in GitHub Actions secrets or repository variables:
- UNITY_VERSION (e.g., "6.0.0f1")

Set secrets using GitHub CLI (locally) or via the repository Settings -> Secrets & variables -> Actions -> New repository secret UI.

GitHub CLI example:
- gh secret set UNITY_EMAIL -b "you@example.com"
- gh secret set UNITY_PASSWORD -b "<password>"
- gh secret set UNITY_SERIAL -b "<serial>"
- gh secret set UNITY_VERSION -b "6.0.0f1"

Notes:
- For automated CI, using a license file (unity-builder supports license file activation) is often more stable than storing passwords.
- Do not commit secrets to the repository.
- If you prefer to run Unity CI with a build agent that has a pre-activated license, you may omit these secrets and adjust the workflow accordingly.
