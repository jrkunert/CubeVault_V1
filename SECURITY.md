# Security Policy

## Purpose

CubeVault is committed to developing and maintaining secure, reliable, and trustworthy software. This document describes how to report security vulnerabilities and outlines the project's approach to security.

---

# Supported Versions

Security updates are provided for the current active development version.

| Version | Supported |
|---------|-----------|
| Current Development | ✅ |
| Archived Releases | ❌ |

---

# Reporting a Vulnerability

If you discover a potential security vulnerability:

- Do **not** create a public GitHub Issue.
- Report the vulnerability privately to the project maintainer.
- Include sufficient detail to reproduce and understand the issue.
- If available, include:
  - affected version
  - component(s)
  - reproduction steps
  - potential impact
  - suggested mitigation (optional)

Reports will be acknowledged as promptly as practical.

---

# Response Process

Reported vulnerabilities will generally follow this process:

1. Acknowledge receipt.
2. Validate the report.
3. Assess severity and impact.
4. Develop and test a fix.
5. Release the fix.
6. Communicate the resolution to affected users, as appropriate.

The timing of each step depends on the nature and complexity of the issue.

---

# Secure Development Practices

Contributors are expected to:

- Validate all external inputs.
- Follow the project's Engineering Standards.
- Minimize dependencies and keep them up to date.
- Avoid committing secrets, credentials, or sensitive information.
- Apply the principle of least privilege.
- Address security findings before merging changes when feasible.

---

# Secrets Management

Never commit:

- Passwords
- API keys
- Connection strings containing credentials
- Certificates or private keys
- Tokens
- Customer or production data

Configuration secrets should be managed outside the source repository.

---

# Dependency Management

Dependencies should:

- Come from trusted sources.
- Be reviewed before adoption.
- Be updated regularly.
- Be monitored for known vulnerabilities.

---

# Responsible Disclosure

Security issues should remain confidential until:

- The issue has been validated.
- A mitigation or fix is available.
- Disclosure will not unnecessarily increase risk to users.

The project supports coordinated disclosure whenever possible.

---

# Questions

Questions about this policy or the project's security practices should be directed to the project maintainer.