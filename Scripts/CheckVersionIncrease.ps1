# Get current version
[xml]$currentVersionFile = Get-Content ./MappaVersion.targets
[semver]$currentMappaVersion = $currentVersionFile.Project.PropertyGroup.MappaVersion

if ($null -ne $currentMappaVersion.PreReleaseLabel -and -not $currentMappaVersion.PreReleaseLabel.StartsWith("alpha"))
{
    Write-Host "Pre-release label must starh with alpha (e.g. 1.1.0-alpha.7)."
    exit 1
}

# Get version from main
[xml]$mainVersionFileDiff = git show origin/main:MappaVersion.targets
[semver]$mainMappaVersion = $mainVersionFileDiff.Project.PropertyGroup.MappaVersion

Write-Host "Current version: '$currentMappaVersion'"
Write-Host "Main version: '$mainMappaVersion'"

# Compare
if ($currentMappaVersion -gt $mainMappaVersion)
{
    Write-Host "New version is CORRECT ( '$currentMappaVersion' > '$mainMappaVersion' ) "
    exit 0
}
else
{
    Write-Host "New verison is INCORRECT ( '$currentMappaVersion' <= '$mainMappaVersion' )"
    exit 1
}