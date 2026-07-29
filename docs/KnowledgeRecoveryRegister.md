# CubeVault M0-B02 Archive Analysis Report v1.0

**Project:** CubeVault  
**Milestone:** M0 - Project Recovery  
**Batch:** M0-B02  
**Repository scope supplied:** `Archive/` snapshot from `jrkunert/CubeVault_V1`  
**Analysis date:** 2026-07-28  
**Status:** Complete for supplied archive snapshot

## 1. Document Control
| Field | Value |
| --- | --- |
| Document | CubeVault Archive Analysis Report |
| Version | 1.0 |
| Deliverable | `CubeVault_M0-B02_Archive_Analysis_Report_v1.0.md` |
| Prepared for | Primary Chat review |
| Evidence base | Uploaded `Archive.zip`, extracted to a read-only working copy |
| Files inventoried | 158 files in the supplied archive snapshot |
| Nested packages inspected | 13 numbered batch ZIPs plus `CubeVault_Project_Starter.zip` |
| Evidence labels | Confirmed, Probable, Possible, Unknown |

This report is a recovery and governance artifact. It does not approve archived code for production use and does not modify, relocate, or delete any archived material.
## 2. Executive Summary
**Confirmed:** The archive preserves the complete historical bridge from the OneStream Finance Business Rule `MTCH_ClearCubeData` to the planned CubeDataManager framework and then to an early multi-project CubeVault platform concept. Losing the archive would remove behavioral requirements, operational schemas, implementation experiments, architecture intent, and evidence of prior foundation work.
**Confirmed:** The most valuable behavioral evidence is the Business Rule family, especially `MTCH_ClearCubeData.vb`, `CubeDataManager.vb`, `Source/CubeDataManager.vb`, and the V5.x lineage. These files contain export, restore, clear, filter, hierarchy, integrity, statistics, safety-limit, and health-check behavior.
**Confirmed:** The archived `src/CubeVault.Common` contains a broad but incomplete common-services foundation targeting .NET Framework 4.7.2. Its interfaces and concrete services cover file systems, hashing, serialization, compression, configuration, logging, results, integrity, retries, timing, and dependency injection. It is a reusable design reference, not a drop-in production dependency.
**Confirmed:** The 13 numbered batch ZIPs are non-cumulative increments. Together they reconstruct the checked-in `CubeVault.Common` tree; later packages add focused files rather than replacing the entire project. The starter package preserves an earlier clean-architecture plan and ADR.
**Confirmed:** Operational exports and troubleshooting XML contain environment-specific, user-related, financial, application, and proprietary OneStream material. The vendor API PDF explicitly carries copyright restrictions. These artifacts require private storage or sanitization and should not remain in an unrestricted public repository.
**Probable:** `MTCH_ClearCubeData.vb` and the three identical `CubeDataManager.vb` copies represent the most complete consolidated behavioral state. They are richer than V5.3.2 in line count and health/statistics content, but naming and branch history are insufficient to declare them formally approved without owner confirmation and Git history.
Primary recovery direction: preserve all original evidence privately; extract stable behavior into governed specifications and tests; redesign the production implementation behind explicit domain and provider boundaries; do not compile temporary, copied, or troubleshooting artifacts; and require owner decisions on the authoritative Business Rule baseline, public-repository policy, and architecture lineage.
## 3. Scope
Primary scope was the supplied `Archive/` directory. The analysis included source files, documentation, SQL, CSV exports, XML troubleshooting packages, logs, Visual Studio metadata, shortcuts, Office documents, PDFs, and nested ZIP packages. Nested ZIP contents were extracted and inspected in a temporary working directory. The active repository outside the supplied archive was not available, so active-versus-archive comparisons are limited to active-looking files embedded in the snapshot.
## 4. Methodology
The archive was extracted without modifying original files. The analysis combined directory inventory, file size and SHA-256 comparison, source-code feature scanning, manual inspection of representative source sections, document text extraction, PDF rendering and text extraction, nested-ZIP expansion, CSV schema/count review, XML structural review, and cross-document comparison. Large operational data values were not reproduced.
Classification is conservative: existence is not approval; numerical version order is not treated as authority; duplicates are identified by exact SHA-256 where possible; and inferences are labeled.
## 5. Limitations
**Confirmed:** No Git history, commit metadata, tags, pull requests, or owner approvals were supplied. Therefore chronology is inferred from filenames, document dates, file content, and package sequence.
**Confirmed:** The active repository state was not supplied. Statements about whether a capability exists in the active project are marked Unknown unless represented by the archived foundation snapshot.
**Confirmed:** `ErrorLog_20260630_212831Z.xml` is malformed XML near line 3576. It was readable as text but could not be fully parsed as a valid XML tree.
**Confirmed:** `Projects.lnk`, `.suo`, SQLite workspace data, and the blank/near-blank spreadsheet are binary or generated artifacts with limited semantic value; they were inventoried but not treated as authoritative design evidence.
**Confirmed:** The OneStream API PDF is proprietary vendor documentation. It was inspected only to establish scope, version, and licensing risk; its contents are not reproduced.
## 6. Archive Overview
| Artifact family | Approximate contents | Recovery value | Primary concern |
| --- | --- | --- | --- |
| Business Rule lineage | 26+ VB files from V2.2 through V5.3.2, Y7.0, temporary/hierarchy/6ish branches, and consolidated copies | Critical behavioral and operational knowledge | Conflicting branches; no approval history |
| CubeVault.Common | ~70 VB project files plus solution/build files | High architectural and reusable-design value | Incomplete, untested, net472, uneven implementation quality |
| Batch packages | 13 numbered ZIPs and starter ZIP | High reconstruction and provenance value | Generated packages and duplicate content |
| Documentation | Manifests, plans, guides, status, changelog, outline | Critical intent and governance knowledge | Contradictory project names and frozen architectures |
| Operational evidence | 3 CSV exports, logs, 4 XML files | High schema/test-fixture value | Sensitive and environment-specific data |
| Database | Application table and registration procedure | Moderate platform-roadmap evidence | Needs security and schema review |
| IDE/generated | `.vs`, shortcut, blank workbook | Low recovery value | Noise, privacy, machine-specific state |
| Vendor documentation | OneStream 9.2.1 API Overview | Contextual dependency evidence | Copyright/licensing restrictions |

**Confirmed:** The supplied archive has 158 files. Three exact duplicate groups were found: the two CubeDataManager manifest DOCX files; the three CubeDataManager source copies; and any duplicates within nested package reconstruction are package-generated copies rather than separately authored evidence.
## 7. Archive Inventory
| Path/group | Type | Apparent purpose | Condition | Classification |
| --- | --- | --- | --- | --- |
| `MTCH_ClearCubeData_V2.2.vb` through `V2.6.1b.vb` | VB.NET Business Rule generations | Clear/export/restore foundation and safety controls | Readable; versioned; some near duplicates | Behavioral Reference; Superseded |
| `MTCH_ClearCubeData_V3.0.vb` through `V3.2.vb` | VB.NET Business Rule generations | Expanded restore, parameters, filters, file handling | Readable | Behavioral Reference |
| `MTCH_ClearCubeData_V5.vb` through `V5.3.2.vb` | VB.NET Business Rule generations | Metadata/hierarchy expansion, integrity, health/statistics | Readable; strongest named lineage | Authoritative Historical Reference candidate; Behavioral Reference |
| `MTCH_ClearCubeData.vb` | VB.NET consolidated Business Rule | Most complete observed implementation | Readable; 4,519 lines | Behavioral Reference; Recover Candidate |
| `CubeDataManager.vb`; `Source/CubeDataManager.vb`; copy | VB.NET consolidated framework-in-BR form | Identical to one another; near-equivalent to final MTCH file | Readable; exact duplicates | Architectural/Behavioral Reference; Duplicate |
| `MTCH_ClearCubeData_Hier.vb`; `6ish`; `Y7.0`; `TEMP*`; `Re org.vb` | Experimental branches | Hierarchy, refactoring, and alternate evolution experiments | Readable; unclear approval | Experimental; Unknown; never compile by default |
| `Source/CubeDataExportDM.vb` | VB.NET source | Data Management/export-related implementation | Readable | Behavioral Reference; Recover Candidate |
| `src/CubeVault.Common/**` | VB.NET class library | Common abstractions and infrastructure services | Readable; incomplete; no tests | Architectural Reference; Reusable Candidate |
| `CubeVault.sln`; `.vbproj`; `Directory.Build.props` | Build artifacts | Historical solution and build assumptions | Readable | Architectural Reference |
| `CubeVault_Batch1.zip` ... `Batch13.zip` | ZIP packages | Incremental foundation delivery history | All inspected | Generated Artifact; Historical Reference |
| `CubeVault_Project_Starter.zip` | ZIP package | Early docs/ADR/architecture baseline | Inspected | Architectural Reference; Superseded in parts |
| Manifests and guides | DOCX/MD/PDF/TXT | Mission, methodology, architecture, implementation plans | Readable/extracted | Authoritative Historical Reference candidates |
| `API_Overview_Guide.pdf` | Vendor PDF | OneStream 9.2.1 API context | Readable/rendered | Sensitive Review Required; licensed documentation |
| `Export_*.csv` | CSV exports | Real export schemas and volume evidence | Readable; 86,939/86,939/155 rows | Operational Evidence; Sensitive Review Required |
| Troubleshooting XML | XML | DM and Business Rule deployment captures | Mostly readable; one malformed log XML | Operational Evidence; Sensitive Review Required |
| `EXPLAIN_*.log`; BR execution log | Log files | Performance/diagnostic evidence | Readable; large | Operational Evidence; Sensitive Review Required |
| `database/**` | SQL | Application registration schema and procedure | Readable | Architectural Reference; Reusable Candidate after review |
| `.vs/**`; `Projects.lnk`; blank XLSX | Generated/machine artifacts | Little durable project knowledge | Binary or empty-like | Generated; Deprecated; remove from governed archive index |

## 8. Artifact Classification
| Classification | Principal artifacts | Rationale |
| --- | --- | --- |
| Authoritative Historical Reference | CubeDataManager manifest, CubeVault manifest, named V5 lineage, SQL scripts | Direct evidence of prior stated intent or developed behavior; still requires approval context |
| Behavioral Reference | All Business Rule generations and operational exports | Shows expected input, output, safety, and execution behavior |
| Architectural Reference | CubeVault.Common, starter docs, solution/build files, repository abstraction | Shows boundaries, interfaces, and intended platform structure |
| Operational Evidence | CSV, XML, logs | Shows actual schemas, version markers, volumes, and failure modes |
| Reusable Candidate | Hashing, serialization, result models, selected file abstractions, SQL schema concepts | Potentially portable after tests and modernization |
| Recover Candidate | Export/restore/filter/hierarchy/health behavior and missing specifications | Knowledge appears richer than current foundation documentation |
| Superseded | Early V2/V3 files, older manifests, earlier package copies | Later artifacts contain expanded behavior or planning |
| Experimental | TEMP, 6ish, Hier, Y7.0, Re org | Naming and divergence indicate exploratory work |
| Duplicate | Manifest DOCX pair; three CubeDataManager source copies | Exact SHA-256 matches |
| Generated Artifact | Batch ZIPs, `.vs`, exports, logs | Produced by development/build/runtime processes |
| Sensitive Review Required | Operational evidence, API PDF, shortcut/workspace state | May expose proprietary, personal, financial, or environment details |
| Unknown | Unversioned alternate branches and blank workbook | Purpose or approval cannot be established |

## 9. Business Rule Evolution
**Confirmed:** Source size and feature density increase materially across generations: V2.2 is roughly 956 lines; V3.2 roughly 2,174; V5.3.2 roughly 4,124; and the consolidated `MTCH_ClearCubeData.vb`/CubeDataManager form roughly 4,519.
| Generation | Representative files | Major observed evolution | Assessment |
| --- | --- | --- | --- |
| 2.2 | `MTCH_ClearCubeData_V2.2.vb` | Core clear path, preview and MaxCells safeguards, initial export/restore/file logic | Early baseline; superseded |
| 2.3-2.4.1 | V2.3, V2.3.1, V2.4, V2.4.1 | Substantial export expansion and file/integrity handling; minor point-branch duplication | Behavioral lineage |
| 2.5-2.5.8 | V2.5 series | Export matures; restore references grow; preview and integrity behavior become more prominent | Important transition |
| 2.6-2.6.1b / Y7.0 / TEMP | V2.6 series and alternates | Parameter/logging/restore experiments; branch-like divergence | Do not infer order or approval from names |
| 3.0-3.2 | V3 series | Restore, parameter processing, filter construction, and file handling expand | Stable middle-generation reference |
| 5.0-5.2 | V5, V5.1, V5.2 | Metadata and hierarchy processing introduced and expanded; filters become substantially richer | Critical recovery generation |
| 5.3 / 5.3.2 | V5.3, V5.3.2 | Broader parameter framework; V5.3.2 adds health/statistics/security-related structures and more file/integrity logic | Strongest explicitly versioned reference |
| Consolidated | `MTCH_ClearCubeData.vb`; `CubeDataManager.vb` copies | Largest observed implementation; health checks, statistics, hierarchy, export/restore, integrity and configuration concepts coexist | Most complete behavioral reference candidate; owner approval required |

Significant evolution findings:
- **Confirmed:** Execution flow evolves from a direct Business Rule operation toward explicit request/configuration/plan/controller/dispatcher concepts documented in the CubeDataManager manifest.
- **Confirmed:** Parameter handling expands after the V2 series and is much denser in V5.3.2/consolidated files.
- **Confirmed:** Filter behavior grows sharply in V5, coinciding with metadata and hierarchy support.
- **Confirmed:** Export files evolve from 21-22-column schemas to a 24-column schema that adds `BRVersion`, `Cube`, and `User` and normalizes dimension ordering.
- **Confirmed:** Restore handling increases steadily through V3 and V5, indicating bidirectional lifecycle management rather than clear-only behavior.
- **Confirmed:** Health-check references appear only in V5.3.2 and consolidated forms among the named lineage scanned.
- **Confirmed:** Integrity/checksum concepts are pervasive in later Business Rules and formalized as services in CubeVault.Common.
- **Confirmed:** No meaningful compression or retry implementation was detected in the Business Rule family; those concerns appear in CubeVault.Common instead.
- **Confirmed:** OneStream integration assumes Finance Business Rule execution, Data Management custom-calculate arguments/POV, BRApi access, DataBuffer formulas, and OneStream 9.2.1-era APIs.
**Probable:** The consolidated `MTCH_ClearCubeData.vb` is the best single behavioral reference, with V5.3.2 used as the strongest named checkpoint. Direct reuse should be prohibited until compile validation, behavioral tests, and owner confirmation establish which branch was production-approved.
## 10. Functional Capability Recovery
| Capability | Evidence | Maturity | Active repository | Disposition |
| --- | --- | --- | --- | --- |
| Data export | Business Rule family; `Export_*.csv`; `CubeDataExportDM.vb` | High | Unknown | Preserve behavior; redesign provider |
| Data restore | V2.2 onward, strongest in V3/V5/consolidated | Medium-High | Unknown | Recover and test |
| Cube data clearing | All principal Business Rules; implementation guides | High | Unknown | Preserve safety semantics |
| Metadata extraction | V5+ and troubleshooting metadata XML | Medium | Unknown | Recover behind OneStream provider |
| Hierarchy processing | V5+, Hier branch, consolidated | Medium | Unknown | Recover with explicit tests |
| Filter construction | All generations, strongly expanded in V5 | High | Unknown | Promote to formal specification |
| Parameter processing | All generations; DM XML; manifest framework | High | Unknown | Recover and validate |
| Logging | BR logging plus CubeVault.Common log abstractions | Medium | Foundation snapshot only | Redesign unified structured logging |
| Error handling | BR try/catch/logs; exception/result classes | Medium | Foundation snapshot only | Standardize |
| Statistics | BR generations and consolidated | Medium | Unknown | Preserve metrics definitions |
| Health checks | V5.3.2/consolidated; `ApplicationHealth` | Medium | Foundation snapshot only | Recover |
| File creation/management | BR and broad Common file abstractions | High | Partial foundation | Controlled reuse |
| Manifest generation | `ManifestService`, `ManifestEntry` | Low-Medium | Partial foundation | Complete design |
| Integrity verification | BR checks plus SHA-256/checksum services | High | Partial foundation | Controlled reuse and tests |
| Compression | GZip service in Common | Low-Medium | Partial foundation | Validate format/stream semantics |
| Retry handling | `RetryHelper` | Low-Medium | Partial foundation | Redesign policy-based |
| Configuration | ApplicationSettings/provider/validator | Medium | Partial foundation | Preserve concepts; harden |
| Security | Limited BR checks; SHA hashing; operational access groups | Low | Unknown | Redesign; security review |
| Database registration | `cv.Application` and `cv.uspApplicationRegister` | Medium | Unknown | Review and recover in DB project |
| Application repository | `IApplicationRepository.vb` | Low-Medium | Unknown | Align with domain/application boundaries |
| Deployment | Guides, DM XML, Business Rule packaging | Medium | Unknown | Extract deployment runbook |
| OneStream BR packaging | Implementation guide and metadata XML | Medium | Unknown | Preserve privately; create synthetic package fixture |

## 11. Previous CubeVault Architecture
**Confirmed:** The archived project declares `CubeVault.Common` as an SDK-style VB.NET project targeting `net472`, with root namespace and assembly name `CubeVault.Common` and XML documentation enabled.
The implementation contains these major areas: Abstractions, Assembly, Common, Compression, Configuration, Constants, DependencyInjection, Diagnostics, Enums, Exceptions, Extensions, Guards, IO, Integrity, Logging, Models, Results, Security, Serialization, Services, Utilities, Validation, and ValueObjects.
| Concern | Archived approach | Assessment |
| --- | --- | --- |
| Namespace strategy | Root namespace `CubeVault.Common`; folders imply layered subnamespaces | Clear intent, but verify explicit namespace declarations and VB root-namespace behavior |
| Interfaces | Fine-grained interfaces for readers, writers, paths, directories, locking, metadata, enumeration, hashing, serialization, compression, clock, configuration | Strong testability intent; possibly over-fragmented |
| Concrete services | Physical file system, file service/operations, checksums, integrity, manifests, async files, locks, metadata, environment, path, directory | Broad coverage; requires integration tests and consolidation |
| Dependency injection | `ServiceCollectionExtensions` registration | Modern composition intent; external package/version assumptions need validation on net472 |
| Configuration | `ApplicationSettings`, in-memory provider, validator | Useful baseline; no durable/secrets-aware provider evident |
| Logging | ILogger/ILogProvider, console/null/structured loggers, LogEntry | Useful abstraction; likely not aligned with Microsoft.Extensions.Logging |
| File-system abstraction | Many narrow interfaces plus physical implementation | Testable, but overlapping responsibilities create drift risk |
| Integrity/hashing | SHA-256 hasher, checksum service, file integrity result/service | High-value reusable concept |
| Serialization | System.Text.Json serializer abstraction | Modern API, but net472 package compatibility and options require review |
| Results | Result, OperationResult, ErrorResult, ErrorInfo, status enum | Consistent non-exception flow candidate; overlapping result types need rationalization |
| Utilities | Atomic writer, synchronizer, retry, streams, timing, metrics, paths, versions | Useful candidates; concurrency and failure semantics must be tested |
| Build settings | net472; solution/build props; no tests in supplied archive | Compatible with OneStream-era .NET Framework, but modernization constraints remain |

Strengths: separation of concerns, dependency inversion, test seams, explicit result models, integrity primitives, and production-oriented file operations. Weaknesses: no test projects, no package-lock evidence, incomplete XML documentation consistency, overlapping abstractions, uncertain thread safety, and no demonstrated integration with the Business Rule or OneStream provider. Architecture drift is visible between the compact Common library, the manifest’s eight-project target solution, and the single-file CubeDataManager migration strategy.
**Recommendation:** Reuse contracts and behavior selectively, not the entire project wholesale. Start with hashing/checksum, serialization contract, clock, result/error model, and atomic-file semantics after tests. Consolidate file abstractions before expanding the platform.
## 12. Batch Package Analysis
| Package | Files | Intended increment |
| --- | --- | --- |
| Batch1 | 8 | Repository foundation, solution, project, build/editor config, core docs |
| Batch2 | 7 | Clock, assembly constants, status enum, exception, string extensions, global imports, operation result |
| Batch3 | 6 | File-system base, guard, result, system clock, path utility, error info |
| Batch4 | 6 | Hasher, JSON serializer, logging contract, SHA-256, System.Text.Json, physical file system |
| Batch5 | 7 | Compression, environment, settings, DI, collection extensions |
| Batch6 | 7 | Reader/writer, directory utility, null logger, error result, file service, retry |
| Batch7 | 6 | Path service, date extensions, console logger, file metadata, stream utility |
| Batch8 | 6 | Configuration and directory abstractions/services, disposable base, file checksum |
| Batch9 | 6 | Checksum and metadata services, application info, log provider |
| Batch10 | 7 | Enumeration and file operations, log entry, application version, version utility |
| Batch11 | 6 | File locking, structured logging, atomic writes, timing, configuration validation |
| Batch12 | 6 | Change detection, exception diagnostics, health, manifest service, directory synchronization |
| Batch13 | 6 | Async files, manifest/integrity models, integrity service, metrics |
| Starter | 5 | Project manifest, status, changelog, architecture, ADR-0001 Clean Architecture |

**Confirmed:** The numbered packages are incremental rather than cumulative. Their combined files match the conceptual checked-in `CubeVault.Common` foundation. Functionality is not known to exist only inside the numbered ZIPs; their principal value is delivery sequence and provenance.
**Confirmed:** The starter ZIP contains documentation not represented in the numbered code batches, including an ADR and architecture baseline; this information should be preserved even where superseded.
## 13. Documentation Recovery
| Document | Recovered knowledge | Conflict/authority note |
| --- | --- | --- |
| CubeDataManager Manifest & Memory v0.1.0 | Incremental refactoring, production stability, Business Rule as deployment artifact, execution-centric design, M-001 through M-008 migration plan | Most complete migration philosophy; project name predates CubeVault |
| CubeVault Project Manifest v0.1 | Commercial platform, Clean Architecture, multiple projects, SQL persistence, thin BR, domain independent of OneStream | Conflicts with single-BR incremental strategy; appears later platform vision |
| MASTER_MANIFEST.md | Foundation standards: Option Strict/Explicit/Infer, XML docs, constructor injection, no placeholders | Very terse; likely generated during Batch1 |
| PROJECT_STATUS.md / CHANGELOG | Repository initialized; Session 001; Common foundation | Historical checkpoint only |
| Starter architecture and ADR | Clean Architecture decision and planned boundaries | Preserve as historical ADR; approval status unknown |
| Implementation package/OneStream guide | Finance BR, DM POV, preview, MaxCells, DataBuffer pattern, deployment/test intent | Early design package may be superseded by larger source |
| Project plan PDF / Outline / Func.txt | Planning, function inventory, implementation structure | Supporting evidence; authority varies |
| API Overview Guide | OneStream 9.2.1 platform/API context | Licensed vendor material; not project-owned documentation |

Recovered principles suitable for governance include: preserve production behavior; architecture before implementation; replace responsibilities rather than scattered lines; keep deliverables buildable; document every migration; measure performance; favor readability; use complete replacement files; maintain thin OneStream adapters; and keep domain logic independent of vendor APIs.
Core contradiction requiring decision: the CubeDataManager document freezes a deployable single-Business-Rule incremental migration, while the CubeVault manifest freezes a multi-project Clean Architecture platform. These can be reconciled by treating the Business Rule as an adapter/deployment host and CubeVault as the long-term system, but that reconciliation is an architectural recommendation, not a confirmed historical decision.
## 14. Operational Evidence Review
**Confirmed:** Two large exports contain 86,939 data rows each; a later export contains 155 rows. The large exports use related but differently ordered 21-22-column schemas. The later export adds `BRVersion`, `Cube`, and `User`, producing a 24-column schema.
| Evidence | Observed structure | Recovery use |
| --- | --- | --- |
| `Export_20260704_130240.csv` | LineNo, timestamp, Scenario/Time/Entity/Parent/Cons/Currency, Account/Flow/Origin/IC/UD1-UD8/View/Amount | Legacy schema fixture; volume/performance fixture after sanitization |
| `Export_20260704_134142.csv` | Similar dimensions; `View` earlier; no explicit Currency field | Schema evolution and compatibility tests |
| `Export_20260709_174650.csv` | Adds BRVersion, Cube, User; 24 fields | Most informative schema/versioning fixture; sensitive |
| DM troubleshooting XML | Custom-calculate definition, detailed logging, Cube and dimensional filters | Deployment/configuration fixture after sanitization |
| Metadata troubleshooting XML | Business Rule package, language, access/maintenance groups, source code | Packaging fixture; highly sensitive/proprietary |
| Error XML | Large error capture; malformed XML | Parser resilience and failure-case fixture after sanitization |
| EXPLAIN log / execution log | Diagnostic/performance traces and Business Rule execution evidence | Performance and troubleshooting taxonomy |

Timestamp filenames use compact UTC-like patterns such as `YYYYMMDD_HHMMSSZ` and non-Z variants. Export timestamps and version markers should be normalized in a future file-format specification. Amount fields and dimensional member names must be treated as potentially confidential financial/customer data.
## 15. Sensitive Data and Repository Safety Review
| Concern | Evidence | Disposition |
| --- | --- | --- |
| User identities/personal names | Later CSV includes `User`; logs/XML may include identities | Replace with synthetic values; private original |
| Financial values and customer data | CSV `Amount` and multidimensional intersections | Move originals to restricted private storage; synthetic fixtures only |
| Production application/environment names | `Prod1_*`, Cube/Application fields, XML package names | Sanitize and remove from public Git history if already published |
| Server/database/file-system details | Logs, SQL, troubleshooting data, shortcut and workspace files | Owner/security review; sanitize |
| Credentials/secrets/tokens | No secret value is asserted in this report; operational files are high-risk | Run automated and manual secret scan before any publication |
| OneStream configuration/source | Metadata and DM XML include packaged definitions and source | Private storage; synthetic packaging fixtures |
| Proprietary vendor documentation | `API_Overview_Guide.pdf` copyright notice | Remove from public repo; retain licensed copy outside source control |
| IDE/user state | `.suo`, SQLite workspace, `.lnk` | Remove from Git history/public archive; no recovery value |
| Troubleshooting/error data | XML/log files | Restrict access and apply retention policy |

**Confirmed:** No sensitive values are reproduced here. Presence or absence of credentials was not conclusively established; a dedicated secret scanner and owner review are mandatory.
## 16. Duplicate and Obsolete Material
Exact duplicates:
- `CubeDataManager_Project_Manifest_and_Memory_v0.1.0.docx`, `Documentation/Manifest_v0.1.0.docx`
- `CubeDataManager.vb`, `Source/CubeDataManager  - Copy.vb`, `Source/CubeDataManager.vb`
Near-duplicate/superseded groups include V2.x point releases, V3.1/V3.1.1, V5 branches, temporary copies, package copies, and terse root documentation that overlaps richer DOCX manifests. `New Microsoft Excel Worksheet.xlsx` appears empty or placeholder-like. `Projects.lnk`, `.vs`, `.suo`, and workspace SQLite are machine-specific. None should be implementation inputs.
Files that should never be compiled by default: `MTCH_ClearCubeDatatEMP*.vb`, `MTCH_ClearCubeData6ish.vb`, `MTCH_ClearCubeData_Hier.vb`, `MTCH_ClearCubeData_Y7.0.vb`, `Re org.vb`, copied source files, and any source extracted from troubleshooting XML. They may be read only as historical evidence.
## 17. Knowledge Recovery Register
| ID | Topic | Source | Recovered information | Confidence | Destination |
| --- | --- | --- | --- | --- | --- |
| KR-001 | Product origin | Business Rule family; CubeDataManager manifest | CubeVault originates in production-style cube data lifecycle operations, not a greenfield generic file product | High | Manifest |
| KR-002 | Deployment model | Manifest; guides; metadata XML | OneStream Business Rule remains a deployment adapter/artifact | High | Architecture Guide/ADR |
| KR-003 | Safety semantics | V2+ source; guides | Preview and MaxCells limits are core protections | High | Constitution/Testing Strategy |
| KR-004 | Execution model | CubeDataManager manifest | Request -> configuration -> plan -> controller -> dispatcher -> engine -> result | High | Architecture Guide |
| KR-005 | Export schema evolution | CSV files | Schemas changed in field order and added version/cube/user metadata | High | Format specification/Testing |
| KR-006 | Hierarchy/filter complexity | V5+ source | Hierarchy-aware filters are a major source of complexity and risk | High | Architecture Guide/Technical Debt |
| KR-007 | Integrity | BR source; Common services | Checksums and integrity verification are first-class lifecycle concerns | High | Engineering Standards |
| KR-008 | Incremental migration | CubeDataManager manifest | Preserve deployability and behavior after each migration | High | Constitution/Standards |
| KR-009 | Platform architecture | CubeVault manifest/starter ADR | Long-term target uses Clean Architecture, repositories, DI, thin BR and SQL persistence | Medium | Manifest/Architecture Guide |
| KR-010 | Common foundation | `src/CubeVault.Common` and batches | Prior work already defines broad common-service contracts | High | Recovery Package |
| KR-011 | Operational volume | Large CSV exports | At least ~86,939-row export cases must be performance-tested | High | Testing Strategy |
| KR-012 | Packaging/configuration | DM and metadata XML | Deployment depends on POV filters, detailed logging, access groups and BR packaging | High | Deployment Guide |
| KR-013 | Security/privacy | Operational artifacts and vendor PDF | Archive cannot safely be assumed public | High | Security Review |
| KR-014 | Database roadmap | SQL scripts, manifest | Application registration and SQL repository were planned early | Medium | Roadmap/ADR |
| KR-015 | Contradictory frozen architectures | Two manifests | “Frozen” was applied to both single-BR migration and multi-project platform plans | High | Primary decision/ADR |

## 18. Architecture and Governance Implications
| Upcoming document | Required incorporation |
| --- | --- |
| M0-B03 Constitution | Evidence over assumption; preserve production behavior; safety-first destructive operations; no secrets/production data in public source; complete self-contained batches; buildable state; documentation as deliverable |
| M0-B04 Project Manifest | OneStream data lifecycle mission; BR as adapter; export/restore/clear/validate/compare/archive scope; explicit authority hierarchy; migration roadmap; schema compatibility; owner decisions |
| M0-B05 Engineering Standards | Option Strict/Explicit/Infer; complete replacement files; tests for preview/MaxCells/filter/hierarchy/integrity; structured logging; synthetic fixtures; no inline secrets; atomic writes; checksum requirements |
| M0-B06 Architecture Guide | Reconcile BR migration with Clean Architecture; define domain/provider boundaries; execution pipeline; file-format/versioning; repository/database boundary; Common consolidation |
| M0-B07 Recovery Package | Include this report, canonical private archive index, hashes, sanitized fixtures, recovered specs, authoritative-baseline decision, and excluded-sensitive-material register |

## 19. Technical Debt Register
| ID | Debt | Severity | Evidence | Disposition | Target |
| --- | --- | --- | --- | --- | --- |
| TD-001 | No established authoritative Business Rule baseline | Critical | Multiple divergent late branches | Owner decision plus Git/history review | M0 |
| TD-002 | No automated test suite in archived foundation | Critical | No tests directory in supplied code snapshot | Create characterization/unit/integration tests | M1 |
| TD-003 | Overlapping file abstractions and result types | High | Common interfaces/services | Rationalize before expansion | M1 |
| TD-004 | net472 dependency/package uncertainty | High | vbproj and modern APIs | Rebuild and lock dependencies in supported environment | M1 |
| TD-005 | Business logic coupled to OneStream APIs | High | Large monolithic BR files | Extract specifications and provider adapter | M1-M2 |
| TD-006 | Export schema versions undocumented | High | Three differing CSV headers | Formal schema/version contract | M1 |
| TD-007 | Sensitive operational data in archive | Critical | CSV/XML/log/vendor PDF | Private storage and history remediation | M0 |
| TD-008 | Malformed error XML | Medium | ErrorLog parse failure | Preserve as negative fixture; repair copy only | M1 |
| TD-009 | Architecture contradiction | High | Two “frozen” manifests | Resolve via ADR | M0 |
| TD-010 | Database scripts lack full migration/test context | Medium | Two SQL files only | Create DB project, migrations and tests | M2 |
| TD-011 | Batch packages duplicate checked-in code | Low | 13 ZIPs | Retain provenance privately; index hashes | M0 |
| TD-012 | Generated IDE artifacts tracked | Medium | `.vs`, `.suo`, SQLite, shortcut | Remove from active/public Git and update ignore rules | M0 |

## 20. Risk Register
| ID | Risk | Severity | Probability | Impact | Recommended disposition |
| --- | --- | --- | --- | --- | --- |
| R-001 | Wrong archived branch becomes production baseline | Critical | High | Behavioral regression or data loss | Owner approval and characterization tests before reuse |
| R-002 | Destructive clear operation exceeds intended scope | Critical | Medium | Financial data loss | Mandatory preview, MaxCells, POV validation and authorization |
| R-003 | Sensitive data remains publicly accessible | Critical | High | Privacy, contractual and security exposure | Immediate repository/privacy review and history remediation |
| R-004 | OneStream API/version incompatibility | High | Medium | Compile/runtime failure | Pin supported OneStream version; adapter tests |
| R-005 | Hierarchy/filter edge cases produce incomplete exports/restores | High | High | Silent data inconsistency | Golden fixtures and reconciliation tests |
| R-006 | File integrity/atomicity failure | High | Medium | Corrupt archives or restores | Checksums, atomic writes, idempotency and recovery tests |
| R-007 | Large-volume performance regression | High | Medium | Operational timeouts | Benchmark with sanitized 86k+ row fixtures |
| R-008 | Direct reuse of incomplete Common services | High | Medium | Concurrency, locking, compatibility defects | Code review and test-first controlled migration |
| R-009 | Licensed vendor material redistributed | High | Medium | IP/license violation | Remove from public source; retain licensed private reference |
| R-010 | Documentation contradictions become governance ambiguity | High | High | Architecture drift | Authority hierarchy and ADR resolution |

## 21. Recovery Recommendations
1. Preserve permanently in restricted storage: every Business Rule generation, both historical manifests, implementation guides, starter docs/ADR, SQL scripts, batch ZIPs with hashes, and original operational evidence under an explicit retention policy.
2. Extract into governed documentation: operation semantics, parameter contract, filter grammar, hierarchy rules, export schemas, restore reconciliation, clear safeguards, health checks, statistics definitions, file naming, timestamp/version rules, and OneStream deployment procedure.
3. Consider for controlled code reuse: SHA-256/checksum contracts, clock abstraction, serializer contract, operation result/error model, atomic writer behavior, manifest/integrity models, and selected path/file utilities. Require tests and modernization review.
4. Reconstruct behavior rather than copying code for: Business Rule dispatch, DataBuffer formulas, metadata/hierarchy traversal, restore and clear engines, DM argument parsing, security checks, and logging. Use archived code as characterization evidence.
5. Sanitize or remove from any public repository: all CSV/XML/log operational evidence, `.vs`, `.suo`, SQLite workspace, shortcut, blank workbook, and proprietary vendor PDF. Consider Git-history removal, not merely deletion in a later commit.
6. Require owner review for: authoritative BR baseline; whether V5.3.2 or consolidated CubeDataManager is production-approved; meaning of Y7.0/6ish/Hier branches; public/private repository policy; application/customer identifiers; and rights to retain vendor documentation.
7. Formal ADR candidates: deployment-host strategy; Clean Architecture reconciliation; target framework/runtime; file-format versioning; storage provider and SQL repository; checksum/integrity policy; destructive-operation safety; sensitive fixture policy; and authority hierarchy for legacy artifacts.
8. Future investigation: Git history, original OneStream environment version/build, compile logs, actual deployment package, test results, database migration history, and whether missing Domain/Application/Infrastructure projects ever existed outside this archive.
## 22. Recommended Archive Organization
Proposed logical organization (implement only after owner approval and with hashes/history preserved):
```text
Archive/
  Index/
    ARCHIVE_MANIFEST.csv
    SHA256SUMS.txt
    CLASSIFICATION.md
  LegacyBusinessRules/
    CanonicalCandidates/
    Versioned/
    Experimental/
  LegacySource/
    CubeDataManager/
    CubeVault.Common/
    Database/
  PreviousBuilds/
    BatchPackages/
    StarterPackages/
  Documentation/
    ProjectManifests/
    Architecture/
    ImplementationGuides/
    VendorRestricted/
  OperationalEvidence-Restricted/
    Exports/
    Logs/
    Troubleshooting/
  SyntheticFixtures/
  GeneratedAndMachineState/
  Unclassified/
```
The public repository should contain only the archive index, governed historical summaries, hashes where safe, and synthetic fixtures. Originals containing operational or licensed content should live in restricted storage referenced by stable recovery identifiers.
## 23. Findings Requiring Primary Chat Decision
| Decision | Options requiring resolution | Recommended default |
| --- | --- | --- |
| Authoritative Business Rule baseline | V5.3.2 vs consolidated `MTCH_ClearCubeData`/CubeDataManager vs another production copy | Treat consolidated as characterization candidate and V5.3.2 as named checkpoint until owner evidence |
| Architecture lineage | Incremental single-BR framework vs multi-project Clean Architecture | Adopt multi-project core with thin BR adapter and incremental behavioral migration |
| Repository visibility | Keep archive public, sanitize, or move originals private | Move sensitive/licensed originals private; publish synthetic evidence only |
| Target runtime | Remain net472 or multi-target/modernize | Keep OneStream adapter compatible; modernize independent core where support permits |
| Legacy code reuse policy | Copy, refactor, or rewrite from specs | Characterize behavior, then controlled redesign; no wholesale copy |
| Database scope | Resume application registration now or defer | Record roadmap/ADR; defer implementation until domain/application boundaries |
| Archive cleanup | Delete duplicates vs preserve all | Preserve originals privately; create indexed canonical views without destructive cleanup |

## 24. Conclusion
**Confirmed:** The Archive folder is not disposable legacy clutter. It is the only supplied source that connects proven OneStream data-management behavior, operational file formats, deployment assumptions, platform architecture plans, and prior foundation code. Its loss would erase requirements that are not captured in the active foundation documents.
The correct recovery action is neither to compile the archive nor to discard it. CubeVault should preserve the evidence privately, convert stable knowledge into governed specifications and tests, resolve the authoritative lineage through explicit decisions, and migrate behavior into a safer architecture with thin OneStream integration, versioned formats, integrity controls, and destructive-operation safeguards.
## 25. Appendices
### Appendix A - Exact Duplicate Groups
1. `CubeDataManager_Project_Manifest_and_Memory_v0.1.0.docx`, `Documentation/Manifest_v0.1.0.docx`
2. `CubeDataManager.vb`, `Source/CubeDataManager  - Copy.vb`, `Source/CubeDataManager.vb`
### Appendix B - CSV Schemas
- `Export_20260704_130240.csv`: 86,939 data rows; fields: `LineNo`, `ExportDateTime`, `Scenario`, `Time`, `Entity`, `Parent`, `Cons`, `Currency`, `Account`, `Flow`, `Origin`, `IC`, `UD1`, `UD2`, `UD3`, `UD4`, `UD5`, `UD6`, `UD7`, `UD8`, `View`, `Amount`
- `Export_20260704_134142.csv`: 86,939 data rows; fields: `LineNo`, `ExportDateTime`, `Scenario`, `Time`, `Entity`, `Parent`, `Cons`, `View`, `Account`, `Origin`, `Flow`, `IC`, `UD1`, `UD2`, `UD3`, `UD4`, `UD5`, `UD6`, `UD7`, `UD8`, `Amount`
- `Export_20260709_174650.csv`: 155 data rows; fields: `LineNo`, `ExportDateTime`, `BRVersion`, `Cube`, `User`, `Entity`, `Parent`, `Cons`, `Scenario`, `Time`, `View`, `Account`, `Flow`, `Origin`, `IC`, `UD1`, `UD2`, `UD3`, `UD4`, `UD5`, `UD6`, `UD7`, `UD8`, `Amount`
### Appendix C - Nested Package File Sequence
- `CubeVault_Batch1` (7 files): `CHANGELOG.md`, `CubeVault.sln`, `Directory.Build.props`, `MASTER_MANIFEST.md`, `PROJECT_STATUS.md`, `README.md`, `src/CubeVault.Common/CubeVault.Common.vbproj`
- `CubeVault_Batch10` (7 files): `src/CubeVault.Common/Abstractions/IFileEnumerator.vb`, `src/CubeVault.Common/Abstractions/IFileOperations.vb`, `src/CubeVault.Common/Logging/LogEntry.vb`, `src/CubeVault.Common/Models/ApplicationVersion.vb`, `src/CubeVault.Common/Services/FileEnumerator.vb`, `src/CubeVault.Common/Services/FileOperations.vb`, `src/CubeVault.Common/Utilities/VersionUtility.vb`
- `CubeVault_Batch11` (6 files): `src/CubeVault.Common/Abstractions/IFileLockService.vb`, `src/CubeVault.Common/Logging/StructuredLogger.vb`, `src/CubeVault.Common/Services/FileLockService.vb`, `src/CubeVault.Common/Utilities/AtomicFileWriter.vb`, `src/CubeVault.Common/Utilities/ExecutionTimer.vb`, `src/CubeVault.Common/Validation/ConfigurationValidator.vb`
- `CubeVault_Batch12` (6 files): `src/CubeVault.Common/Abstractions/IFileChangeDetector.vb`, `src/CubeVault.Common/Diagnostics/ExceptionExtensions.vb`, `src/CubeVault.Common/Models/ApplicationHealth.vb`, `src/CubeVault.Common/Services/FileChangeDetector.vb`, `src/CubeVault.Common/Services/ManifestService.vb`, `src/CubeVault.Common/Utilities/DirectorySynchronizer.vb`
- `CubeVault_Batch13` (6 files): `src/CubeVault.Common/Abstractions/IAsyncFileService.vb`, `src/CubeVault.Common/Integrity/FileIntegrityResult.vb`, `src/CubeVault.Common/Models/ManifestEntry.vb`, `src/CubeVault.Common/Services/AsyncFileService.vb`, `src/CubeVault.Common/Services/FileIntegrityService.vb`, `src/CubeVault.Common/Utilities/MetricsCounter.vb`
- `CubeVault_Batch2` (7 files): `src/CubeVault.Common/Abstractions/IClock.vb`, `src/CubeVault.Common/Constants/AssemblyInfo.vb`, `src/CubeVault.Common/Enums/OperationStatus.vb`, `src/CubeVault.Common/Exceptions/CubeVaultException.vb`, `src/CubeVault.Common/Extensions/StringExtensions.vb`, `src/CubeVault.Common/GlobalImports.vb`, `src/CubeVault.Common/Results/OperationResult.vb`
- `CubeVault_Batch3` (6 files): `src/CubeVault.Common/Abstractions/IFileSystem.vb`, `src/CubeVault.Common/Guards/Guard.vb`, `src/CubeVault.Common/Results/Result.vb`, `src/CubeVault.Common/Services/SystemClock.vb`, `src/CubeVault.Common/Utilities/PathUtility.vb`, `src/CubeVault.Common/ValueObjects/ErrorInfo.vb`
- `CubeVault_Batch4` (6 files): `src/CubeVault.Common/Abstractions/IHasher.vb`, `src/CubeVault.Common/Abstractions/IJsonSerializer.vb`, `src/CubeVault.Common/Logging/ILogger.vb`, `src/CubeVault.Common/Security/Sha256Hasher.vb`, `src/CubeVault.Common/Serialization/SystemTextJsonSerializer.vb`, `src/CubeVault.Common/Services/PhysicalFileSystem.vb`
- `CubeVault_Batch5` (7 files): `src/CubeVault.Common/Abstractions/ICompressionService.vb`, `src/CubeVault.Common/Abstractions/IEnvironmentProvider.vb`, `src/CubeVault.Common/Compression/GZipCompressionService.vb`, `src/CubeVault.Common/Configuration/ApplicationSettings.vb`, `src/CubeVault.Common/DependencyInjection/ServiceCollectionExtensions.vb`, `src/CubeVault.Common/Extensions/CollectionExtensions.vb`, `src/CubeVault.Common/Services/EnvironmentProvider.vb`
- `CubeVault_Batch6` (7 files): `src/CubeVault.Common/Abstractions/IFileReader.vb`, `src/CubeVault.Common/Abstractions/IFileWriter.vb`, `src/CubeVault.Common/IO/DirectoryUtility.vb`, `src/CubeVault.Common/Logging/NullLogger.vb`, `src/CubeVault.Common/Results/ErrorResult.vb`, `src/CubeVault.Common/Services/FileService.vb`, `src/CubeVault.Common/Utilities/RetryHelper.vb`
- `CubeVault_Batch7` (6 files): `src/CubeVault.Common/Abstractions/IPathService.vb`, `src/CubeVault.Common/Extensions/DateTimeExtensions.vb`, `src/CubeVault.Common/Logging/ConsoleLogger.vb`, `src/CubeVault.Common/Models/FileMetadata.vb`, `src/CubeVault.Common/Services/PathService.vb`, `src/CubeVault.Common/Utilities/StreamUtility.vb`
- `CubeVault_Batch8` (6 files): `src/CubeVault.Common/Abstractions/IConfigurationProvider.vb`, `src/CubeVault.Common/Abstractions/IDirectoryService.vb`, `src/CubeVault.Common/Common/DisposableBase.vb`, `src/CubeVault.Common/Configuration/InMemoryConfigurationProvider.vb`, `src/CubeVault.Common/Models/FileChecksum.vb`, `src/CubeVault.Common/Services/DirectoryService.vb`
- `CubeVault_Batch9` (6 files): `src/CubeVault.Common/Abstractions/IChecksumService.vb`, `src/CubeVault.Common/Abstractions/IFileMetadataService.vb`, `src/CubeVault.Common/Assembly/ApplicationInfo.vb`, `src/CubeVault.Common/Logging/ILogProvider.vb`, `src/CubeVault.Common/Services/ChecksumService.vb`, `src/CubeVault.Common/Services/FileMetadataService.vb`
- `CubeVault_Project_Starter` (5 files): `docs/ADR/ADR-0001-CleanArchitecture.md`, `docs/ARCHITECTURE.md`, `docs/CHANGELOG.md`, `docs/PROJECT_MANIFEST.md`, `docs/PROJECT_STATUS.md`
### Appendix D - Files Not Fully Semantically Inspected
| File/group | Reason |
| --- | --- |
| `.vs/CubeVault/v16/.suo`, `.vs/slnx.sqlite`, `Projects.lnk` | Binary machine/user state; inventoried but not treated as knowledge sources |
| `New Microsoft Excel Worksheet.xlsx` | Appears blank/placeholder-like; no meaningful project data established |
| `ErrorLog_20260630_212831Z.xml` | Malformed XML; text evidence available but full structural parsing failed |
| `API_Overview_Guide.pdf` | Inspected for version/scope/licensing only; proprietary content not reproduced |
| Operational CSV/XML/log record bodies | Schemas, counts and structural characteristics inspected; sensitive row-level values intentionally not reproduced |

### Appendix E - Full File Inventory
| Path | Type | Bytes | SHA-256 prefix |
| --- | --- | --- | --- |
| `.vs/CubeVault/v16/.suo` | (none) | 15,872 | 01cbafdc46c4dcb1... |
| `.vs/ProjectSettings.json` | .json | 37 | debe308c356f9f0c... |
| `.vs/VSWorkspaceState.json` | .json | 78 | f8315c8ace58425a... |
| `.vs/slnx.sqlite` | .sqlite | 126,976 | edbac02f9223dd0f... |
| `API_Overview_Guide.pdf` | .pdf | 2,339,069 | 419b27a457cb9cdf... |
| `Archive/CubeVault_Batch1.zip` | .zip | 3,642 | 891927dce21e978a... |
| `Archive/CubeVault_Batch10.zip` | .zip | 4,122 | 049b83e65562b7a0... |
| `Archive/CubeVault_Batch11.zip` | .zip | 3,999 | 6247a39276dafd29... |
| `Archive/CubeVault_Batch12.zip` | .zip | 3,214 | 16924273cc12ba32... |
| `Archive/CubeVault_Batch13.zip` | .zip | 4,109 | 4e553816d552778d... |
| `Archive/CubeVault_Batch2.zip` | .zip | 3,243 | cd36b228709bf5e9... |
| `Archive/CubeVault_Batch3.zip` | .zip | 3,458 | 301d3e95f560b44f... |
| `Archive/CubeVault_Batch4.zip` | .zip | 3,273 | 057bbe46ddd6bffb... |
| `Archive/CubeVault_Batch5.zip` | .zip | 4,204 | 634ca523cd3df1dd... |
| `Archive/CubeVault_Batch6.zip` | .zip | 4,001 | 0bb43f4ffc50d2f2... |
| `Archive/CubeVault_Batch7.zip` | .zip | 3,173 | fbfaf95536d56161... |
| `Archive/CubeVault_Batch8.zip` | .zip | 3,648 | 02d07f47326ba4c1... |
| `Archive/CubeVault_Batch9.zip` | .zip | 3,296 | 110ba6b3f83d8c4e... |
| `CHANGELOG.md` | .md | 107 | 04a8793b6e0e5eb8... |
| `CubeDataManager.vb` | .vb | 94,749 | 573e6f5ad4bd9aa1... |
| `CubeDataManager_Project_Manifest_and_Memory_v0.1.0.docx` | .docx | 38,597 | 646b5cd6176c82dc... |
| `CubeVault.sln` | .sln | 978 | b5c424ededc51c8f... |
| `CubeVault_Project_Manifest_v0.1.docx` | .docx | 37,883 | ca3e3e73879ce84a... |
| `CubeVault_Project_Starter.zip` | .zip | 2,408 | 7f7b9ced87c3c1f5... |
| `Directory.Build.props` | .props | 588 | ac5b4a6a616d988d... |
| `Documentation/Manifest_v0.1.0.docx` | .docx | 38,597 | 646b5cd6176c82dc... |
| `EXPLAIN_20260702_000248.log` | .log | 7,531,897 | 70861fc86b7f6a33... |
| `ErrorLog_20260630_212831Z.xml` | .xml | 294,234 | 540cb10ba0e61549... |
| `Export_20260704_130240.csv` | .csv | 10,996,737 | 0b76648565b787a8... |
| `Export_20260704_134142.csv` | .csv | 10,475,805 | 1ba7f603d5a5fb3c... |
| `Export_20260709_174650.csv` | .csv | 34,056 | ba9ce27c85b0f2f5... |
| `Func.txt` | .txt | 3,534 | 7c4b397c412d6905... |
| `IApplicationRepository.vb` | .vb | 737 | 7f2c9ac17ebc3cd9... |
| `MASTER_MANIFEST.md` | .md | 278 | d1d42be23fa92a23... |
| `MTCH_ClearCubeData.vb` | .vb | 94,758 | 37a28750fc75232d... |
| `MTCH_ClearCubeData6ish.vb` | .vb | 73,893 | c40a56dfa6c34d2c... |
| `MTCH_ClearCubeData_20260708_173324_180.log` | .log | 2,370 | 5e30af440968951f... |
| `MTCH_ClearCubeData_Hier.vb` | .vb | 72,216 | aa0c1664b1d2c3ac... |
| `MTCH_ClearCubeData_Implementation_Package.docx` | .docx | 37,489 | e68259dcbe6074a0... |
| `MTCH_ClearCubeData_V2.2.vb` | .vb | 21,525 | 15384d5d603f1da6... |
| `MTCH_ClearCubeData_V2.3.1.vb` | .vb | 28,124 | c743d5aef4f2dc08... |
| `MTCH_ClearCubeData_V2.3.vb` | .vb | 26,070 | 0bf813e527df1a02... |
| `MTCH_ClearCubeData_V2.4.1.vb` | .vb | 30,550 | 5df3be8e2b4d3e18... |
| `MTCH_ClearCubeData_V2.4.vb` | .vb | 28,137 | aeb17fa8319bcdab... |
| `MTCH_ClearCubeData_V2.5.2.vb` | .vb | 38,397 | d3385b5c1b99e75d... |
| `MTCH_ClearCubeData_V2.5.4.vb` | .vb | 38,397 | b17e0505bc9a0067... |
| `MTCH_ClearCubeData_V2.5.6.vb` | .vb | 40,929 | 3e66617c8efac298... |
| `MTCH_ClearCubeData_V2.5.7.vb` | .vb | 41,193 | 6191ce6e0ba86654... |
| `MTCH_ClearCubeData_V2.5.8.vb` | .vb | 41,706 | b8bfac2f2b7805eb... |
| `MTCH_ClearCubeData_V2.5.vb` | .vb | 36,343 | 62f7e18d0b2196b1... |
| `MTCH_ClearCubeData_V2.6.1.vb` | .vb | 41,853 | 73ea456adeddd0df... |
| `MTCH_ClearCubeData_V2.6.1b.vb` | .vb | 44,395 | 7069dfbda05a558d... |
| `MTCH_ClearCubeData_V2.6.vb` | .vb | 42,070 | 31b929bc179271fa... |
| `MTCH_ClearCubeData_V3.0.vb` | .vb | 46,665 | fd3ea839518e7cd6... |
| `MTCH_ClearCubeData_V3.1.1.vb` | .vb | 48,164 | 3f6e8e14b130c172... |
| `MTCH_ClearCubeData_V3.1.vb` | .vb | 47,986 | db2d57ab1eb92a42... |
| `MTCH_ClearCubeData_V3.2.vb` | .vb | 48,934 | 19c9c3e5632a2964... |
| `MTCH_ClearCubeData_V5.1.vb` | .vb | 65,087 | 647efe37a18ffb17... |
| `MTCH_ClearCubeData_V5.2.vb` | .vb | 70,649 | 2c61d27b27bd95a4... |
| `MTCH_ClearCubeData_V5.3.2.vb` | .vb | 90,361 | d3bfdb0e463abf34... |
| `MTCH_ClearCubeData_V5.3.vb` | .vb | 76,551 | ee1fd85491281caa... |
| `MTCH_ClearCubeData_V5.vb` | .vb | 59,005 | 58e8e587cfeb2a9c... |
| `MTCH_ClearCubeData_Y7.0.vb` | .vb | 44,631 | 97b5d34b708cc744... |
| `MTCH_ClearCubeDatatEMP - Copy.vb` | .vb | 44,059 | e7b00bafa4506061... |
| `MTCH_ClearCubeDatatEMP.vb` | .vb | 44,345 | 693af33351153c45... |
| `New Microsoft Excel Worksheet.xlsx` | .xlsx | 6,190 | f977c6cb12040d34... |
| `OneStream_9.2_ClearCubeData_Implementation_Guide.docx` | .docx | 37,299 | 2812f5700948fd5a... |
| `OneStream_ClearCubeData_Project_Plan.pdf` | .pdf | 2,059 | 6dce8886c770e692... |
| `Outline.txt` | .txt | 11,267 | 1163d1e52459fe75... |
| `PROJECT_STATUS.md` | .md | 146 | 8b097fc1f997bce8... |
| `Prod1_Troubleshooting_DataManagement_20260625_202558Z.xml` | .xml | 2,138 | f355e16cfcab6d40... |
| `Prod1_Troubleshooting_DataManagement_20260629_214544Z.xml` | .xml | 2,127 | f72e16f4f4df02d6... |
| `Prod1_Troubleshooting_Metadata_20260629_214801Z.xml` | .xml | 8,787 | f8c1162e33b9973e... |
| `Projects.lnk` | .lnk | 878 | f164e3fb531db89e... |
| `README.md` | .md | 151 | 0230f4e95e755025... |
| `Re org.vb` | .vb | 74,386 | add3189278a22c73... |
| `Source/CubeDataExportDM.vb` | .vb | 0 | e3b0c44298fc1c14... |
| `Source/CubeDataManager  - Copy.vb` | .vb | 94,749 | 573e6f5ad4bd9aa1... |
| `Source/CubeDataManager.vb` | .vb | 94,749 | 573e6f5ad4bd9aa1... |
| `database/StoredProcedures/cv.uspApplicationRegister.sql` | .sql | 1,639 | 5f5190f2ce3de877... |
| `database/Tables/cv.Application.sql` | .sql | 1,337 | c5a5483c506ce225... |
| `src/CubeVault.Common/Abstractions/IAsyncFileService.vb` | .vb | 458 | 5169ce98a783c8ec... |
| `src/CubeVault.Common/Abstractions/IChecksumService.vb` | .vb | 234 | 3bc38a6d3a0b5bed... |
| `src/CubeVault.Common/Abstractions/IClock.vb` | .vb | 207 | c34aee1c7455de74... |
| `src/CubeVault.Common/Abstractions/ICompressionService.vb` | .vb | 333 | a4bfd2f3f5b21e56... |
| `src/CubeVault.Common/Abstractions/IConfigurationProvider.vb` | .vb | 227 | c29f3952528ea704... |
| `src/CubeVault.Common/Abstractions/IDirectoryService.vb` | .vb | 258 | 957b256f05c95a58... |
| `src/CubeVault.Common/Abstractions/IEnvironmentProvider.vb` | .vb | 222 | 81048d33edc336e4... |
| `src/CubeVault.Common/Abstractions/IFileChangeDetector.vb` | .vb | 237 | 8a3dcc768afca4cb... |
| `src/CubeVault.Common/Abstractions/IFileEnumerator.vb` | .vb | 270 | 45dd2aa8248b3cb2... |
| `src/CubeVault.Common/Abstractions/IFileLockService.vb` | .vb | 290 | 65aa8d553cec43d5... |
| `src/CubeVault.Common/Abstractions/IFileMetadataService.vb` | .vb | 242 | 8259be2b9e44d5bd... |
| `src/CubeVault.Common/Abstractions/IFileOperations.vb` | .vb | 362 | 6f898b0eac214ea0... |
| `src/CubeVault.Common/Abstractions/IFileReader.vb` | .vb | 220 | 08d9011a4616bd9a... |
| `src/CubeVault.Common/Abstractions/IFileSystem.vb` | .vb | 222 | 3843b54feba79dbe... |
| `src/CubeVault.Common/Abstractions/IFileWriter.vb` | .vb | 226 | 723c0c57fc9b5888... |
| `src/CubeVault.Common/Abstractions/IHasher.vb` | .vb | 221 | 766092c984feeec6... |
| `src/CubeVault.Common/Abstractions/IJsonSerializer.vb` | .vb | 224 | 8e1fc5eda73d7704... |
| `src/CubeVault.Common/Abstractions/IPathService.vb` | .vb | 219 | e5e1f700cf9be18b... |
| `src/CubeVault.Common/Assembly/ApplicationInfo.vb` | .vb | 279 | 3bde5934e53dbb30... |
| `src/CubeVault.Common/Common/DisposableBase.vb` | .vb | 550 | 5d8219db8eea457b... |
| `src/CubeVault.Common/Compression/GZipCompressionService.vb` | .vb | 925 | 392feeb85b0359ea... |
| `src/CubeVault.Common/Configuration/ApplicationSettings.vb` | .vb | 296 | 40a846cb8875369e... |
| `src/CubeVault.Common/Configuration/InMemoryConfigurationProvider.vb` | .vb | 675 | 5cc6619ba40d8f86... |
| `src/CubeVault.Common/Constants/AssemblyInfo.vb` | .vb | 331 | b3d5359b38511209... |
| `src/CubeVault.Common/CubeVault.Common.vbproj` | .vbproj | 302 | ec39c4fb4cfd761d... |
| `src/CubeVault.Common/DependencyInjection/ServiceCollectionExtensions.vb` | .vb | 233 | 324136ec74ca38fb... |
| `src/CubeVault.Common/Diagnostics/ExceptionExtensions.vb` | .vb | 374 | 7ef16c1acea1d68a... |
| `src/CubeVault.Common/Enums/OperationStatus.vb` | .vb | 209 | be728a8130bdd8cf... |
| `src/CubeVault.Common/Exceptions/CubeVaultException.vb` | .vb | 284 | cb0be375e2f7ac49... |
| `src/CubeVault.Common/Extensions/CollectionExtensions.vb` | .vb | 394 | 8124c24c5dc36df3... |
| `src/CubeVault.Common/Extensions/DateTimeExtensions.vb` | .vb | 369 | e645b5239f5bbaae... |
| `src/CubeVault.Common/Extensions/StringExtensions.vb` | .vb | 372 | 7ea63712067abd06... |
| `src/CubeVault.Common/GlobalImports.vb` | .vb | 207 | 83aa5d106d5b33fd... |
| `src/CubeVault.Common/Guards/Guard.vb` | .vb | 648 | ab24a45af83c53fb... |
| `src/CubeVault.Common/IO/DirectoryUtility.vb` | .vb | 382 | dfad800a441ad637... |
| `src/CubeVault.Common/Integrity/FileIntegrityResult.vb` | .vb | 299 | 7905343d5b040114... |
| `src/CubeVault.Common/Logging/ConsoleLogger.vb` | .vb | 465 | f5fc8902accd3afd... |
| `src/CubeVault.Common/Logging/ILogProvider.vb` | .vb | 218 | 055e9d5365ec7f91... |
| `src/CubeVault.Common/Logging/ILogger.vb` | .vb | 243 | ae088caefb05421f... |
| `src/CubeVault.Common/Logging/LogEntry.vb` | .vb | 284 | 9dfa6fa9f0dc783f... |
| `src/CubeVault.Common/Logging/NullLogger.vb` | .vb | 376 | 1b6853c8ed19b0b7... |
| `src/CubeVault.Common/Logging/StructuredLogger.vb` | .vb | 620 | bbad3329bc487e96... |
| `src/CubeVault.Common/Models/ApplicationHealth.vb` | .vb | 261 | f496a9a8ae3ba9bf... |
| `src/CubeVault.Common/Models/ApplicationVersion.vb` | .vb | 245 | 89dee2c1785a702b... |
| `src/CubeVault.Common/Models/FileChecksum.vb` | .vb | 237 | 1f0ba91ad829b8cf... |
| `src/CubeVault.Common/Models/FileMetadata.vb` | .vb | 285 | c9a83ac01ddf4570... |
| `src/CubeVault.Common/Models/ManifestEntry.vb` | .vb | 278 | bad762fa653bb6b3... |
| `src/CubeVault.Common/Results/ErrorResult.vb` | .vb | 330 | 1f5cb385b0370e04... |
| `src/CubeVault.Common/Results/OperationResult.vb` | .vb | 435 | dc7dd5d526338a91... |
| `src/CubeVault.Common/Results/Result.vb` | .vb | 617 | f4fa12b4a4e0a7f6... |
| `src/CubeVault.Common/Security/Sha256Hasher.vb` | .vb | 630 | 977ea96f6da79309... |
| `src/CubeVault.Common/Serialization/SystemTextJsonSerializer.vb` | .vb | 514 | 4f0dd2dc9e402e33... |
| `src/CubeVault.Common/Services/AsyncFileService.vb` | .vb | 1,122 | d3b4a238ab2d753d... |
| `src/CubeVault.Common/Services/ChecksumService.vb` | .vb | 612 | 37cab915e2bf8f10... |
| `src/CubeVault.Common/Services/DirectoryService.vb` | .vb | 589 | 3da2183c7fb5105d... |
| `src/CubeVault.Common/Services/EnvironmentProvider.vb` | .vb | 435 | 9ef09c2f1abda084... |
| `src/CubeVault.Common/Services/FileChangeDetector.vb` | .vb | 460 | 1d5447a88128193d... |
| `src/CubeVault.Common/Services/FileEnumerator.vb` | .vb | 508 | ab54734664b7cd49... |
| `src/CubeVault.Common/Services/FileIntegrityService.vb` | .vb | 564 | bbcd6bcdee62c2cc... |
| `src/CubeVault.Common/Services/FileLockService.vb` | .vb | 482 | f664e63c65dfabd6... |
| `src/CubeVault.Common/Services/FileMetadataService.vb` | .vb | 629 | b9c9cccdea3d3468... |
| `src/CubeVault.Common/Services/FileOperations.vb` | .vb | 804 | cb8fa3a9222071ce... |
| `src/CubeVault.Common/Services/FileService.vb` | .vb | 600 | 82958a817e833201... |
| `src/CubeVault.Common/Services/ManifestService.vb` | .vb | 330 | f074cb87e1c2379d... |
| `src/CubeVault.Common/Services/PathService.vb` | .vb | 390 | 5dc4a038f372978e... |
| `src/CubeVault.Common/Services/PhysicalFileSystem.vb` | .vb | 377 | 851545a6b895901f... |
| `src/CubeVault.Common/Services/SystemClock.vb` | .vb | 379 | 0170a4b7f8441ede... |
| `src/CubeVault.Common/Utilities/AtomicFileWriter.vb` | .vb | 581 | 803f1e5aa23b0d74... |
| `src/CubeVault.Common/Utilities/DirectorySynchronizer.vb` | .vb | 448 | 4093ca893e680d45... |
| `src/CubeVault.Common/Utilities/ExecutionTimer.vb` | .vb | 454 | 1ad29faea5c24e58... |
| `src/CubeVault.Common/Utilities/MetricsCounter.vb` | .vb | 308 | a4b4a7e4cc437ec0... |
| `src/CubeVault.Common/Utilities/PathUtility.vb` | .vb | 361 | cdc70c11cd12011e... |
| `src/CubeVault.Common/Utilities/RetryHelper.vb` | .vb | 687 | bd6b576c9f087b6c... |
| `src/CubeVault.Common/Utilities/StreamUtility.vb` | .vb | 405 | c3f57a1d3bb6d892... |
| `src/CubeVault.Common/Utilities/VersionUtility.vb` | .vb | 425 | 0b518868db65d221... |
| `src/CubeVault.Common/Validation/ConfigurationValidator.vb` | .vb | 486 | 916b755d447f85be... |
| `src/CubeVault.Common/ValueObjects/ErrorInfo.vb` | .vb | 239 | 1feb27b84ed4d5de... |

