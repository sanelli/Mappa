<##
# Create the NuGet configuration that is used to point to
# the local .packages.
#>

$CurrentLocation = Get-Location
$NuGetFile = @”
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <packageSources>
        <add key="NuGet.org" value="https://api.nuget.org/v3/index.json" />
        <add key="Local .packages" value="file://$CurrentLocation/.packages" />
    </packageSources>
</configuration>
“@

Write-Host $NuGetFile

$NuGetFileExists = Test-Path -Path "nuget.config" -PathType Leaf
if ($NuGetFileExists)
{
    Remove-Item "nuget.config"
}

$NuGetFile | Out-File -FilePath "nuget.config" -NoNewline