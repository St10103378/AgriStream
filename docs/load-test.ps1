# Simple concurrent load test against the App Service
# Usage: .\load-test.ps1 -Url "https://yourapp.azurewebsites.net" -Users 50 -Seconds 60

param(
    [string]$Url = "https://YOUR-APP.azurewebsites.net",
    [int]$Users = 30,
    [int]$Seconds = 45
)

Write-Host "Starting load test against $Url with $Users concurrent users for $Seconds seconds..." -ForegroundColor Cyan

$jobs = @()
$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()

1..$Users | ForEach-Object {
    $jobs += Start-Job -ScriptBlock {
        param($target, $duration)
        $end = (Get-Date).AddSeconds($duration)
        $count = 0
        while ((Get-Date) -lt $end) {
            try {
                Invoke-WebRequest -Uri $target -UseBasicParsing -TimeoutSec 10 | Out-Null
                $count++
            } catch {}
        }
        return $count
    } -ArgumentList $Url, $Seconds
}

Write-Host "Waiting for jobs to complete..."
$results = $jobs | Wait-Job | Receive-Job
$jobs | Remove-Job

$total = ($results | Measure-Object -Sum).Sum
$elapsed = $stopwatch.Elapsed.TotalSeconds

Write-Host "`nResults:" -ForegroundColor Green
Write-Host "  Total requests : $total"
Write-Host "  Duration       : $([math]::Round($elapsed,1)) s"
Write-Host "  Approx RPS     : $([math]::Round($total / $elapsed, 1))"
Write-Host "`nNow check Azure Monitor / Application Insights / Auto-scale history for evidence."
