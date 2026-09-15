# Contoso Legacy Bank Documents

Classic C# .NET Framework 4.8 worker that converts versioned statement jobs into branded PDF statements. It uses PDFsharp (MIT licensed), non-SDK projects, and `packages.config`.

## Build and test

Use a Visual Studio Developer PowerShell:

```powershell
nuget restore .\Contoso.LegacyBank.Documents.sln
msbuild .\Contoso.LegacyBank.Documents.sln /m /p:Configuration=Release
vstest.console.exe .\tests\Contoso.LegacyBank.Documents.Tests\bin\Release\Contoso.LegacyBank.Documents.Tests.dll /TestAdapterPath:.\packages\NUnit3TestAdapter.4.5.0\build\net462
```

## Run a deterministic demo

The default root is `%LOCALAPPDATA%\ContosoLegacyBank\Documents`. The executable creates all required directories.

```powershell
$root = Join-Path $env:LOCALAPPDATA 'ContosoLegacyBank\Documents'
New-Item -ItemType Directory -Force (Join-Path $root 'Pending') | Out-Null
Copy-Item .\samples\statement-job.v1.json (Join-Path $root 'Pending')
.\src\Contoso.LegacyBank.Documents\bin\Release\Contoso.LegacyBank.Documents.exe --once
```

The PDF appears in `Output`; completion JSON appears in `Completed`. Invalid jobs are moved to `Failed` beside sanitized failure metadata.

Run without arguments for polling mode. Run with `--service` when hosted by the Windows Service Control Manager. Configure `DocumentsRoot` and `PollingIntervalSeconds` in the executable `.config`; environment variables in `DocumentsRoot` are expanded.

## Job contract

Schema version 1 requires:

- `schemaVersion`: `1`
- `jobId`: letters, digits, `_`, or `-`
- `customer`, `account`
- `fromDate`, `toDate`
- `openingBalance`, `closingBalance`
- `transactions[]`: `date`, `description`, `amount`, `balance`

Unknown JSON properties and unsupported versions are rejected to prevent silent contract drift. See [`samples/statement-job.v1.json`](samples/statement-job.v1.json), [`docs/architecture.md`](docs/architecture.md), and [`docs/modernization.md`](docs/modernization.md).
