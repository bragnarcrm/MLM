param(
    [Parameter(Mandatory = $true)]
    [ValidatePattern('^https?://')]
    [string]$DeploymentUrl
)

$uri = $DeploymentUrl.TrimEnd('/')
$response = Invoke-WebRequest -Uri $uri -Method Get -MaximumRedirection 5 -UseBasicParsing

if ($response.StatusCode -lt 200 -or $response.StatusCode -ge 400) {
    throw "Deployment smoke test failed with HTTP $($response.StatusCode)."
}

Write-Host "Deployment smoke test passed: $uri returned HTTP $($response.StatusCode)."
