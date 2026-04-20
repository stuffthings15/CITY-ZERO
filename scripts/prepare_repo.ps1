# Prepare repository for first commit and push to GitHub
# Usage: Set-Variable -Name remoteUrl -Value 'https://github.com/<owner>/<repo>.git'; .\scripts\prepare_repo.ps1
param(
    [string]$remoteUrl = ''
)

if (-not (Test-Path .git)) {
    git init
    git add -A
    git commit -m "chore: initial commit - project scaffold and inventory"
    Write-Host "Initialized git repository and created initial commit."
} else {
    Write-Host "Git repo already initialized."
}

if ($remoteUrl -ne '') {
    git remote add origin $remoteUrl -ErrorAction SilentlyContinue
    Write-Host "Remote origin set to $remoteUrl"
    Write-Host "To push: git push -u origin main"
} else {
    Write-Host "No remoteUrl provided. To add remote and push:
1) git remote add origin https://github.com/<owner>/<repo>.git
2) git branch -M main
3) git push -u origin main"
}
