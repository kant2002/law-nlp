param (
    [Parameter(Mandatory = $true)]
    [string]$FilePath   # Path to the input file
)
$lines = Get-Content -Path $FilePath
mkdir database/rus -Force
mkdir database/kaz -Force
foreach ($line in $lines) {
    # Skip empty lines
    if ([string]::IsNullOrWhiteSpace($line)) {
        continue
    }

    # Trim spaces
    $line = $line.Trim()

    $url = "https://adilet.zan.kz/rus/docs/$line"
    $file = "database/rus/$line"
    if (-Not (Test-Path $file)) {
        Write-Host "Requesting: $url"
	    curl $url -o $file
    }

    $url = "https://adilet.zan.kz/kaz/docs/$line"
    $file = "database/kaz/$line"
    if (-Not (Test-Path $file)) {
        Write-Host "Requesting: $url"
	    curl $url -o $file
    }
}