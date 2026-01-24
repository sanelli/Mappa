Write-Host "Appending branch number to .editorconfig to make sure all TODOs have been addressed/removed"

$BranchName = $ENV:BRANCH_NAME
Write-Host "Branch name is '$BranchName'"

if([System.String]::IsNullOrWhiteSpace($BranchName)){
    Write-Error "The branch name is incorrect: the name is either empty or null"
    exit 1
}

$components = $BranchName.Split("-");
if ($components.Length -lt 1) {
    Write-Error "The branch name is incorrect: cannot be split"
    exit 1
}

$possileNumber = $components[0]
if($possileNumber.StartsWith("#")){
    $possileNumber = $possileNumber.Substring(1);
}

if(-not [System.Int32]::TryParse($possileNumber, [Ref] $Null)){
    Write-Error "The branch name is not providing a correct issue number: branch name should start with ISSUENUMBER or #ISSUENUMBER"
    exit 1
}

Add-Content -Path ".editorconfig" -Value "`n`ntodo_analyzer.report_tasks = $possileNumber"
Write-Host "Value 'todo_analyzer.report_tasks = $possileNumber' appended to .editorconfig."

exit 0