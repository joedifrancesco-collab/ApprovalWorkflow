# start.ps1 — Launch the Approval Workflow app
# Run from the solution root: 

$apiProject = "$PSScriptRoot\ApprovalWorkflow.API"
$url        = "http://localhost:5268"

# Stop any existing instance
$existing = Get-Process -Name "ApprovalWorkflow.API" -ErrorAction SilentlyContinue
if ($existing) {
    Write-Host "Stopping existing instance..." -ForegroundColor Yellow
    $existing | Stop-Process -Force
    Start-Sleep 1
}

Write-Host "Starting Approval Workflow API..." -ForegroundColor Cyan
$process = Start-Process -FilePath "dotnet" `
                         -ArgumentList "run --project `"$apiProject`"" `
                         -PassThru `
                         -NoNewWindow:$false

# Wait for the API to become ready
Write-Host "Waiting for API to start..." -ForegroundColor Cyan
$ready = $false
for ($i = 0; $i -lt 15; $i++) {
    Start-Sleep 1
    try {
        $null = Invoke-WebRequest -Uri "$url/api/approvalrequests" -Method Get -UseBasicParsing -ErrorAction Stop
        $ready = $true
        break
    } catch { }
}

if ($ready) {
    Write-Host "API is ready. Opening browser..." -ForegroundColor Green
    Start-Process $url
} else {
    Write-Host "API did not respond in time. Check for errors above." -ForegroundColor Red
}
