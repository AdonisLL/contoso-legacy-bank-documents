# Copilot instructions

- This repository intentionally targets .NET Framework 4.8 with classic, non-SDK MSBuild projects and `packages.config`.
- Keep the console entry point and Windows Service hosting seam operational.
- Preserve atomic queue claims by moving jobs from `Pending` to `Processing` on the same volume.
- Treat statement data as sensitive. Never log customer names, account numbers, transaction details, or raw job JSON.
- Keep schema version 1 backward compatible. Add a new schema version rather than silently changing fields.
- Use focused NUnit tests and validate with Visual Studio MSBuild on Windows.
- Do not replace the legacy implementation with .NET (Core/5+) until the modernization plan explicitly authorizes it.
