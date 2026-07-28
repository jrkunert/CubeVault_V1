# CubeVault Project Manifest
**Version:** 1.0  
**Status:** Governing Project Document  
**Project:** CubeVault  
**Document Type:** Project Manifest  
**Owner:** CubeVault Primary Chat  
**Last Updated:** July 28, 2026

---

# 1. Document Control

## Purpose

This document serves as the authoritative governing document for the CubeVault project.

Its purpose is to define:

- Project objectives
- Architectural direction
- Development methodology
- Repository standards
- Recovery procedures
- Governance
- Long-term maintenance expectations

This document is intended to enable complete project recovery even if every active developer is unavailable.

---

## Scope

This document governs the entire CubeVault repository including:

- source code
- documentation
- testing
- releases
- recovery artifacts
- milestone planning

---

## Authority

Unless superseded by an approved Architecture Decision Record (ADR), this document is considered the primary governing document for project execution.

---

# 2. Project Overview

CubeVault is a modern .NET-based application platform intended to replace and significantly expand the capabilities of the legacy CubeDataManager system.

CubeVault preserves valuable business knowledge from the historical project while establishing a modern architecture designed for long-term maintainability, scalability, recoverability, and automated development.

The project emphasizes:

- clear architecture
- repeatable builds
- deterministic development
- comprehensive documentation
- disaster recovery
- maintainability over shortcuts

---

# 3. Product Vision

The vision of CubeVault is to become the authoritative platform for secure, extensible, and maintainable cube-related data management.

The platform is intended to support future expansion through modular components while maintaining a stable architectural core.

CubeVault is designed to remain understandable decades into the future through disciplined documentation and repository governance.

---

# 4. Product Goals

Primary goals include:

- Modernize the legacy application
- Preserve historical business knowledge
- Eliminate architectural ambiguity
- Support automated development workflows
- Support AI-assisted implementation
- Maintain production-quality coding standards
- Enable complete repository recovery
- Minimize technical debt
- Encourage long-term maintainability

---

# 5. Architecture Overview

CubeVault follows a layered architecture emphasizing separation of concerns.

Typical layers include:

- User Interface
- Application Services
- Domain Logic
- Infrastructure
- Shared/Common Libraries

Cross-cutting concerns include:

- Logging
- Configuration
- Validation
- Dependency Injection
- Testing
- Documentation

Major architectural changes require an approved ADR.

---

# 6. Project Structure

```text
docs/
src/
tests/
tools/
artifacts/
```

---

# 7. Development Methodology

Development proceeds through controlled milestones and batches. Worker Chats complete assigned batches only. Documentation is treated as a first-class project artifact.

---

# 8. Batch Methodology

Each batch defines explicit scope, produces complete replacement files, avoids unrelated changes, and preserves repository integrity.

---

# 9. Milestone Process

Planning → Repository Preparation → Implementation → Verification → Documentation → Release → Archive

---

# 10. Worker Chat Methodology

Worker Chats complete assigned work, maintain coding standards, and avoid architectural redesign.

---

# 11. Primary Chat Responsibilities

The Primary Chat governs architecture, milestones, ADR approval, integration, and repository direction.

---

# 12. Recovery Package Standard

Recovery packages include project status, milestone, completed batches, repository audit, ADRs, manifest, and roadmap.

---

# 13. Architecture Decision Records (ADR)

Architectural changes are documented through immutable ADRs containing context, decision, alternatives, consequences, and approval.

---

# 14. Repository Standards

Maintain deterministic builds, logical organization, consistent naming, complete documentation, and avoid committing temporary files.

---

# 15. Coding Standards

Favor readability, maintainability, consistency, meaningful names, defensive programming, and clear exception handling.

---

# 16. Documentation Standards

Documentation is versioned, discoverable, professional, internally consistent, and supports long-term recovery.

---

# 17. Testing Philosophy

Testing includes unit, integration, regression, and build verification with deterministic execution.

---

# 18. Build Philosophy

Builds should remain repeatable, deterministic, automated, and warning-aware.

---

# 19. Source of Truth Hierarchy

1. Approved ADRs
2. Project Manifest
3. Primary Chat Decisions
4. Milestone Documentation
5. Batch Documentation
6. Repository Source Code
7. Historical Reference Material

---

# 20. Milestone Roadmap

- M0 — Project Recovery
- M1 — Foundation
- M2 — Domain
- M3 — Application
- M4 — User Experience
- M5 — Stabilization

---

# 21. Current Project Status

Current milestone: **M0 — Project Recovery**.

---

# 22. Known Technical Debt

Current debt consists primarily of repository recovery, documentation reconciliation, ADR backlog, and implementation planning.

---

# 23. Future Vision

CubeVault aims to become a long-lived, enterprise-quality, modular platform emphasizing automation, testing, maintainability, and disaster recovery.

---

# Historical References

CubeVault replaces CubeDataManager. Historical artifacts are retained solely as reference unless formally adopted.

---

# Revision History

| Version | Date | Description |
|----------|------------|-----------------------------------------------|
| 1.0 | 2026-07-28 | Initial governing project manifest. |
