$ReleaseName = $Env:RELEASE_VERSION.Trim()

if (-not $ReleaseName.StartsWith("v"))
{
    Write-Host "Release name ($ReleaseName) must be name 'vMAJOR.MINOR.BUILD'" -ForegroundColor Red
    Exit 1
}

[semver]$ReleaseVersion = $ReleaseName.Substring(1)
[xml]$currentVersionFile = Get-Content ./MappaVersion.targets
[semver]$currentMappaVersion = $currentVersionFile.Project.PropertyGroup.MappaVersion

if($null -ne $currentMappaVersion.PreReleaseLabel)
{
    Write-Host "MappaVersion ($currentMappaVersion) must NOT contain a pre-release label." -ForegroundColor Red
    Exit 1
}

if($ReleaseVersion -ne $currentMappaVersion){
    Write-Host "Release version ($ReleaseVersion) and MappaVersion ($currentMappaVersion) does not match" -ForegroundColor Red
    Exit 1
}

Write-Host "Release name and versions are correct."
Exit 0