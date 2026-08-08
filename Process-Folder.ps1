<#
.SYNOPSIS
    Process database of files
.DESCRIPTION
    This script processes all files in the specified folder
.PARAMETER FilePath
    Specifies the path to the folder containing the files to process
.PARAMETER Count
    Specifies the maximum number of files to process
.EXAMPLE
    .\Process-Folder.ps1 -FilePath "database/rus"
#>
param (
    [Parameter(Mandatory = $True)]
    [string]$FilePath,
    [Parameter(Mandatory = $False)]
    [int]$Count
)
dotnet build law-nlp-processor
$processedCount = 0
foreach ($line in Get-ChildItem $FilePath) {
    $name = $line.Name
    Write-Host "Processing: $name"
    dotnet run --no-build --no-launch-profile --project law-nlp-processor -- --file $line.FullName
    $processedCount++
    if ($Count -and $processedCount -ge $Count) {
        break
    }
}