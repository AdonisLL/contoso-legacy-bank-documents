# Security Assessment Report

**Generated:** 2026-09-16T04:22:36.3030000Z

## Summary

| Metric | Count |
|--------|-------|
| Total Findings | 2 |
| CVE Vulnerabilities | 0 |
| CWE Vulnerabilities | 2 |
| Total Rules Assessed | 59 |
| Rules Passed | 57 |

### By Severity

| Severity | Count |
|----------|-------|
| mandatory | 0 |
| optional | 0 |
| potential | 2 |

## CVE Findings (Dependency Vulnerabilities)

No CVE vulnerabilities meeting the high severity threshold were found.

## CWE Findings (Code-Level Vulnerabilities)

### CWE-606: Unchecked Input for Loop Condition
- **Category:** Code Quality
- **Severity:** potential
- **Story Points:** 3
- **Files:** src/Contoso.LegacyBank.Documents/Processing/PdfStatementGenerator.cs:47

PdfStatementGenerator.Generate iterates over the externally supplied job.Transactions collection without enforcing a maximum transaction count, so a crafted job can cause excessive PDF rendering work.

### CWE-772: Missing Release of Resource after Effective Lifetime
- **Category:** Code Quality
- **Severity:** potential
- **Story Points:** 3
- **Files:** src/Contoso.LegacyBank.Documents/Processing/PdfStatementGenerator.cs:17

PdfStatementGenerator.Generate creates a PdfDocument but closes it only on the normal path at line 76; exceptions during rendering or saving bypass resource release.

