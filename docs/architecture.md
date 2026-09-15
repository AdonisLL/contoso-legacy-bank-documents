# Architecture

## Runtime

The executable is a .NET Framework 4.8 console worker. `--once` drains the current queue and exits; the default mode polls until Ctrl+C. `--service` passes the same host to `ServiceBase`, providing a seam for installation as a Windows Service without duplicating processing logic.

## File queue

The root defaults to `%LOCALAPPDATA%\ContosoLegacyBank\Documents` and contains:

- `Pending`: producers place complete versioned JSON files here.
- `Processing`: the worker atomically claims a file with `File.Move`.
- `Output`: generated PDFs.
- `Completed`: completion metadata containing `pdfPath`.
- `Failed`: preserved input plus separate sanitized failure metadata.

Only one worker can successfully move a given file. PDF and metadata files are written through a temporary file and renamed/replaced, preventing consumers from seeing partial output.

## Components

- `JobParser`: strict JSON deserialization and semantic validation.
- `PdfStatementGenerator`: branded PDFsharp-based rendering.
- `DocumentWorker`: queue claim, orchestration, completion, and failure handling.
- `ConsoleWorkerHost`: shared polling/cancellation lifecycle.
- `DocumentWindowsService`: Windows Service adapter.

Raw jobs, customer names, account numbers, and transactions are never logged. Failure metadata uses a diagnostic ID, exception type, and bounded safe message; it omits stack traces and raw payloads.
