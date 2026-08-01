# Shared helpers for GitHub gist publish (retry) and markdown history merge (version override).

function Test-MarkdownHistoryHeaderOrSeparatorLine
{
    param(
        [string]$Line,
        [int]$MinimumCellCount = 4
    )

    if ($Line -notmatch '^\|')
    {
        return $true
    }

    $cells = @($Line.Split("|") | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne "" })
    if ($cells.Count -lt $MinimumCellCount)
    {
        return $true
    }

    if ($cells[0] -eq "Timestamp")
    {
        return $true
    }

    foreach ($cell in $cells)
    {
        if ($cell -notmatch '^-+$')
        {
            return $false
        }
    }

    return $true
}

function Get-MarkdownHistoryVersions
{
    param(
        [string]$Markdown,
        [int]$VersionColumnIndex = 1,
        [int]$MinimumCellCount = 4
    )

    $versions = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    if ([string]::IsNullOrWhiteSpace($Markdown))
    {
        return $versions
    }

    foreach ($historyLine in ($Markdown -split "`r?`n"))
    {
        if (Test-MarkdownHistoryHeaderOrSeparatorLine -Line $historyLine -MinimumCellCount $MinimumCellCount)
        {
            continue
        }

        $cells = @($historyLine.Split("|") | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne "" })
        if ($cells.Count -le $VersionColumnIndex)
        {
            continue
        }

        [void]$versions.Add($cells[$VersionColumnIndex])
    }

    return $versions
}

function Merge-MarkdownHistoryByVersion
{
    param(
        [AllowNull()]
        [string]$ExistingMarkdown,

        [Parameter(Mandatory = $true)]
        [string]$NewMarkdown,

        [string]$DefaultHeader = $null,

        [int]$VersionColumnIndex = 1,

        [int]$MinimumCellCount = 4
    )

    $newRows = if ($null -eq $NewMarkdown) { "" } else { $NewMarkdown.Trim() }
    if ([string]::IsNullOrWhiteSpace($newRows))
    {
        throw "Cannot merge history: new markdown rows are empty."
    }

    $versionsToOverride = Get-MarkdownHistoryVersions `
        -Markdown $newRows `
        -VersionColumnIndex $VersionColumnIndex `
        -MinimumCellCount $MinimumCellCount

    if ($versionsToOverride.Count -eq 0)
    {
        throw "Cannot merge history: no version values found in new markdown rows."
    }

    $builder = New-Object System.Text.StringBuilder

    if ([string]::IsNullOrWhiteSpace($ExistingMarkdown))
    {
        if (-not [string]::IsNullOrWhiteSpace($DefaultHeader))
        {
            [void]$builder.AppendLine($DefaultHeader.TrimEnd())
        }

        [void]$builder.AppendLine($newRows)
        return $builder.ToString()
    }

    foreach ($historyLine in ($ExistingMarkdown -split "`r?`n"))
    {
        if ([string]::IsNullOrWhiteSpace($historyLine))
        {
            continue
        }

        if (Test-MarkdownHistoryHeaderOrSeparatorLine -Line $historyLine -MinimumCellCount $MinimumCellCount)
        {
            [void]$builder.AppendLine($historyLine)
            continue
        }

        $cells = @($historyLine.Split("|") | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne "" })
        if ($cells.Count -le $VersionColumnIndex)
        {
            continue
        }

        if ($versionsToOverride.Contains($cells[$VersionColumnIndex]))
        {
            continue
        }

        [void]$builder.AppendLine($historyLine)
    }

    [void]$builder.AppendLine($newRows)
    return $builder.ToString()
}

function Invoke-GhGistEdit
{
    param(
        [Parameter(Mandatory = $true)]
        [string]$GistId,

        [Parameter(Mandatory = $true)]
        [string]$RemoteFileName,

        [Parameter(Mandatory = $true)]
        [string]$LocalPath,

        [ValidateSet("Update", "Add")]
        [string]$Mode = "Update",

        [int]$MaxAttempts = 5
    )

    if (-not (Test-Path -LiteralPath $LocalPath))
    {
        throw "Cannot publish missing local file: $LocalPath"
    }

    $actionVerb = if ($Mode -eq "Add") { "add" } else { "update" }
    $lastExit = 1

    for ($attempt = 1; $attempt -le $MaxAttempts; $attempt++)
    {
        Write-Host "Gist $actionVerb attempt $attempt/$MaxAttempts for $RemoteFileName..."

        $previousEap = $ErrorActionPreference
        try
        {
            $ErrorActionPreference = "Continue"
            if ($Mode -eq "Add")
            {
                & gh gist edit $GistId -a $RemoteFileName $LocalPath
            }
            else
            {
                & gh gist edit $GistId -f $RemoteFileName $LocalPath
            }

            $lastExit = $LASTEXITCODE
        }
        finally
        {
            $ErrorActionPreference = $previousEap
        }

        if ($lastExit -eq 0)
        {
            if ($Mode -eq "Add")
            {
                Write-Host "Added gist $GistId ($RemoteFileName)."
            }
            else
            {
                Write-Host "Updated gist $GistId ($RemoteFileName)."
            }

            return
        }

        if ($attempt -lt $MaxAttempts)
        {
            $delaySeconds = Get-Random -Minimum 2 -Maximum 11
            Write-Host "Gist $actionVerb failed for $RemoteFileName (exit $lastExit); retrying in $delaySeconds seconds..."
            Start-Sleep -Seconds $delaySeconds
        }
    }

    throw "Failed to $actionVerb gist $GistId file $RemoteFileName from $LocalPath."
}
