Unity license activation for CI

This repository's unity-ci workflow supports two activation methods:

1) Email + Serial (legacy):
   - Set the following repository secrets:
     - UNITY_EMAIL
     - UNITY_PASSWORD
     - UNITY_SERIAL
   - The workflow uses these to activate Unity during the build (game-ci/unity-builder supports this method).

2) License file (recommended):
   - Export your Unity license file (a .ulf or .alf) and base64-encode it, for example:
       base64 Unity_lic.ulf > unity_lic.b64
   - Add the base64 content as a repository secret named UNITY_LICENSE:
       gh secret set UNITY_LICENSE -b "$(cat unity_lic.b64)"
   - The workflow step 'Restore Unity license (optional)' will decode the secret into $HOME/.local/share/unity3d/Unity_lic.ulf before the builder runs.

Notes and security
- Do NOT commit license files into source control.
- Use repository secrets and restrict access to trusted collaborators.
- On self-hosted runners with activated Unity license, you may omit the secrets entirely.

Troubleshooting
- If the workflow fails during Unity activation, check the Action logs for activation output and ensure the license is valid for the Unity version defined by UNITY_VERSION.
- For long-term CI, a license-file approach is more stable than password-based activation.

