# CI/CD checks

The GitHub Actions workflow is in `.github/workflows/ci.yml`.

Every push and pull request runs:

- Restore and build for `VitalityPortal.slnx`
- The xUnit test project under `tests/VitalityPortal.Tests`

To run the deployed smoke test after the build, add a GitHub repository variable named `DEPLOYMENT_URL`, for example:

```text
https://portal.example.com
```

The workflow sends a GET request to that URL and fails unless the response is in the 2xx or 3xx range. The smoke test can also be run locally:

```powershell
./scripts/smoke-test.ps1 -DeploymentUrl http://localhost:5080
```

## Azure App Service deployment

The deployment workflow is `.github/workflows/azure-deploy.yml`. Before pushing to `main` or `master`, configure:

- Repository variable `AZURE_WEBAPP_NAME`: the Azure App Service name
- Repository secret `AZURE_WEBAPP_PUBLISH_PROFILE`: the App Service publish profile XML
- Repository variable `DEPLOYMENT_URL`: the public HTTPS URL for the post-deployment smoke test

Set these Azure App Service application settings before the first production deployment:

```text
Jwt__Key
OpenRouter__ApiKey
GoogleAuth__ClientId
GoogleAuth__ClientSecret
GmailSmtp__AppPassword
PayFast__MerchantId
PayFast__MerchantKey
PayFast__PassPhrase
```

The committed configuration intentionally contains placeholders for secrets. Rotate the previously exposed OpenRouter credential before deploying.
