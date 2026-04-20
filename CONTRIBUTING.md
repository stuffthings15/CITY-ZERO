CONTRIBUTING to CITY#ZERO
=========================

Welcome — thank you for contributing. This guide gives minimal conventions, branching, and PR rules so collaborators and automation can work smoothly.

1. Code style & conventions
- Language: C# for WinUI project; C# in Unity packages.
- Target framework: .NET 10 for WinUI.
- Nullable is enabled in the WinUI project; prefer explicit nullability annotations and address warnings.
- Naming conventions:
  - Classes: PascalCase
  - Private fields: _camelCase
  - Interfaces: IName
  - ScriptableObjects: suffix with Config (e.g., WeaponConfig)
  - Runtime DTOs: suffix Data (e.g., VehicleData)
- Keep changes small and focused; follow single-responsibility on commits.

2. Tests
- Add unit tests for library code under Tests/ with clear Arrange/Act/Assert structure.

3. Branching
- Use feature branches: feature/<short-description> or fix/<short-description>.
- Create pull requests against main (or the chosen trunk branch). Include description of changes and test/integration notes.

4. Pull request checklist
- Build succeeds locally.
- No new public API breaks without clear rationale.
- Add or update tests for behavior changes.
- Update inventory.json if you add or remove large files or assets.

5. Commit messages
- Use imperative, present-tense: "Add inventory manifest" not "Added".
- Include a short summary and, if necessary, bullet points.

6. Unity projects
- When editing Unity assets, prefer separate PRs per package to reduce merge conflicts.
- Do not commit Library/, Temp/, Obj/ folders.
- Use asmdef boundaries for code modularization.

7. CI and build
- The repository contains a GitHub Actions workflow to build the WinUI project. Ensure that CI passes before merging.

8. Security
- Do not commit secrets, private keys, or credentials. Use GitHub secrets for CI.

9. Ownership and contacts
- The project owner/maintainer will be the default reviewer for PRs. If you'd like to change ownership, update repository settings or contact the maintainer.
