<p align="center">
  <img src="assets/branding/omsilaunch-logo.png" alt="OmsiLaunch" width="620">
</p>

<p align="center"><strong>Session control for OMSI 2.</strong></p>
<p align="center">Open source · Programmable · Community driven</p>

---

# OmsiLaunch

**OmsiLaunch** is an open-source programmable launch, session-management,
and runtime-control layer for OMSI 2. It provides a public API, a functional
CLI, an in-process OMSI plugin/runtime, and an exact-build `BuildProfile`
boundary. It is infrastructure for launchers, tools, automation, and community
integrations rather than a graphical launcher.

> **Define the session, not the clicks.**

## Beta 0.1

The first public beta is **0.1.0-beta1**. It supports the exact OMSI profile
`Omsi23004_692EBFBF`, validates the executable fingerprint before launch, and
does not claim compatibility with unknown OMSI builds. The package and detailed
compatibility status are documented in [`docs/`](docs/README.md).

OmsiLaunch supports semantic session planning with `LaunchSpec` and
`PlanSession`, startup through `StartSession`, observed runtime control, and
normal `StopSession`/`CloseSession` cleanup. Supported runtime capabilities are
explicitly cataloged; experimental and unavailable capabilities are not hidden.

## Safe Session Ownership

All launch configuration overrides are session-scoped. OmsiLaunch snapshots,
journals, applies, verifies, and restores every temporary configuration or GUI
file it changes. It does not offer permanent configuration editing in this beta.

Product plugin files are permanently installed under `plugins\\OmsiLaunch.*`.
They are not copied in and removed for every session, and third-party plugins
are never transactionally owned. `.omsilaunch\\` is an OmsiLaunch-private
directory for assets, diagnostics, journals, and user examples.

Managed splash presentation is the default. PTB, ENG, DEU, and FRA product
assets are temporarily overlaid and restored exactly. Native/`Unset` splash
mode preserves OMSI files.

## Download

[⬇️ Download OmsiLaunch 0.1.0-beta1](https://github.com/lmonteirotech/OmsiLaunch/releases/download/v0.1.0-beta.1/OmsiLaunch-0.1.0-beta1.zip)

[SHA-256 checksum](https://github.com/lmonteirotech/OmsiLaunch/releases/download/v0.1.0-beta.1/OmsiLaunch-0.1.0-beta1.zip.sha256) · [View release notes](https://github.com/lmonteirotech/OmsiLaunch/releases/tag/v0.1.0-beta.1)

Extract the package directly into the supported OMSI root. It includes the
controller, its required dependencies, the permanent plugin closure, splash
assets, a Release session example, and a small offline user guide.

See [Installation](docs/getting-started/installation.md) and
[First Session](docs/getting-started/first-session.md). Use
`OmsiLaunch.exe /version` to inspect the installed controller.

## For Developers

The preferred product surface is the semantic public API. The CLI is a
reference frontend over that same API; it contains no separate OMSI logic.
Runtime control is session-scoped, profile-validated, and uses opaque semantic
handles rather than public native pointers.

- [Public API](docs/reference/PUBLIC-API.md)
- [Runtime control](docs/reference/RUNTIME-CONTROL.md)
- [Capability catalog](docs/reference/beta-0.1-capabilities.md)
- [Known limitations](docs/reference/known-limitations.md)
- [Build profile policy](docs/reference/BUILD-PROFILES.md)

## Community and License

OmsiLaunch is a community-oriented open-source project. It is independent from
other OMSI launchers and can be consumed by compatible community tooling.

OmsiLaunch is licensed under [LGPL-3.0-only](LICENSE). See the
[third-party notices](THIRD-PARTY-NOTICES.md) for incorporated-source
provenance and applicable notices.
