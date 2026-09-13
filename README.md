<p align="center">
  <!-- Replace with the official OmsiLaunch horizontal logo once the asset is added to the repository -->
  <img src="docs/assets/omsilaunch-logo.png" alt="OmsiLaunch" width="620">
</p>

<p align="center">
  <strong>Session control for OMSI 2.</strong>
</p>

<p align="center">
  Open source · Programmable · Community driven
</p>

---

# OmsiLaunch

**OmsiLaunch** is an open-source session control and orchestration layer for **OMSI 2**.

It is designed to give applications, tools and automation workflows a structured way to define, prepare, start, observe and restore OMSI sessions without relying on UI automation.

OmsiLaunch is **not a graphical launcher**.

Instead, it provides the technical foundation that launchers, content managers, automation tools and other community projects can build upon.

> **Define the session, not the clicks.**

## Project status

> **Pre-release / repository preparation**

OmsiLaunch is currently under active development.

The public repository is being prepared before the first source release. There are currently **no public binaries, packages or stable API guarantees**.

The initial implementation focuses on establishing the core architecture, OMSI integration boundaries, session lifecycle and safe configuration handling before publishing a usable release.

Development progress and technical documentation will be published here as the project reaches its first public milestones.

## What OmsiLaunch is for

A normal OMSI session involves several separate pieces of state:

- OMSI installation
- map
- entry point
- new, saved or last situation
- date and time
- player vehicle
- repaint
- HOF
- fleet number
- registration
- OMSI options
- input configuration
- process lifecycle

OmsiLaunch aims to expose these as a coherent **session definition** instead of requiring external applications to reproduce OMSI's interface workflow.

Conceptually:

```text
Define
  ↓
Prepare
  ↓
Launch
  ↓
Observe
  ↓
Restore
```

The goal is not to replace OMSI.

**The goal is to make OMSI controllable.**

## Intended capabilities

### Session planning

Describe an OMSI session before starting the simulator.

A session may include, where supported:

```text
Map
Entry point
Situation
Date
Time
Vehicle
Repaint
HOF
Fleet number
Registration
Configuration overrides
```

### Controlled startup

OmsiLaunch is designed to interact with OMSI's startup lifecycle directly rather than simulating mouse clicks or keyboard input.

**UI automation is not part of the architecture.**

### Process lifecycle

Applications using OmsiLaunch are intended to manage OMSI sessions through semantic operations such as:

```text
PlanSession
StartSession
GetSessionStatus
WaitSession
StopSession
CloseSession
```

### Content discovery

OmsiLaunch is intended to provide structured discovery of installed OMSI content, including supported categories such as:

```text
Maps
Situations
Vehicles
Repaints
HOF files
Fleet-number sources
Registration sources
```

### Configuration handling

OMSI configuration files require careful handling because they may contain settings unknown to third-party tools.

OmsiLaunch is being designed around minimally destructive configuration processing, preserving unknown data whenever possible.

Initial configuration targets include:

```text
options.cfg
Inputs\keyboard.cfg
Inputs\gamectrler.cfg
```

### Session transactions and recovery

Temporary changes made for a session should not permanently alter the user's OMSI installation.

The architecture therefore includes concepts such as:

```text
Installation locking
State snapshots
Temporary configuration
Runtime staging
Process monitoring
Cleanup
Restoration
Crash recovery
```

The objective is simple:

**prepare what the session needs, then restore what was there before.**

## Designed for other applications

OmsiLaunch is infrastructure.

It is intended to be consumed by other software instead of imposing a single user interface.

Possible clients include:

- graphical launchers
- content managers
- command-line tools
- server-management software
- testing utilities
- automation scripts
- community integrations
- development tools

## OmsiLaunch and Stadt92

**Stadt92** is a separate user-facing OMSI launcher and content manager being developed within the same ecosystem.

Stadt92 is expected to use OmsiLaunch as part of its underlying OMSI control infrastructure while providing many additional features of its own.

The projects are intentionally separate:

```text
Stadt92
    │
    │ uses
    ▼
OmsiLaunch
    │
    │ controls
    ▼
OMSI 2
```

OmsiLaunch itself is designed to remain usable independently by other applications and community projects.

## Architecture

OmsiLaunch is being built as a modular project rather than a single launcher executable.

The planned solution is divided into components responsible for areas such as:

```text
OmsiLaunch.Api
OmsiLaunch.Core
OmsiLaunch.Content
OmsiLaunch.Configuration
OmsiLaunch.Process
OmsiLaunch.Interop
OmsiLaunch.Plugin
OmsiLaunch.Native.x86
OmsiLaunch.Builds
```

OMSI itself remains a 32-bit application.

The OmsiLaunch architecture separates higher-level control from OMSI-facing x86 components so that applications using the project do not need to reproduce low-level integration themselves.

## OMSI integration

OmsiLaunch works around the behavior and limitations of OMSI itself instead of pretending they do not exist.

Its architecture is designed around:

- exact OMSI build identification
- controlled native interoperability
- process lifecycle management
- temporary runtime deployment
- configuration snapshots and restoration
- explicit capability reporting
- structured telemetry and errors

Unsupported or unverified behavior should be reported as such instead of being guessed.

## OmsiHook / Omsi-Extensions

OmsiLaunch builds upon knowledge and selected technical foundations from the open-source OMSI ecosystem, including **OmsiHook / Omsi-Extensions**.

Reuse is intentionally selective.

Low-level interoperability components may be adapted where appropriate, while OmsiLaunch maintains its own session-control architecture and public API.

Provenance and licensing information for reused components will be documented alongside the source code.

## Open source

OmsiLaunch is an **open-source project** and is intended to follow the same open development philosophy as OmsiHook.

The project will be distributed under the **GNU General Public License (GPL)**. The exact license text and version will be included in the repository before the first public source release.

Open development is a core part of the project:

- source available to the community
- community contributions welcome
- reusable by other OMSI projects
- no dependency on a proprietary OmsiLaunch service
- no requirement to use Stadt92
- no proprietary graphical client required

## Platform

The initial implementation target is:

```text
Windows 10 / 11
x64 host environment
OMSI 2 x86
```

OMSI-facing components remain compatible with the simulator's 32-bit runtime requirements.

Support for additional Windows environments may be investigated later.

Platform support will be documented explicitly rather than assumed.

## Documentation

Project documentation will be published alongside the source code and at:

**https://omsilaunch.omsimods.com.br**

Planned documentation areas include:

- getting started
- architecture
- public API
- session specification
- configuration
- content discovery
- OMSI build profiles
- plugin architecture
- native interoperability
- testing and validation
- recovery behavior
- integration examples

## Contributing

OmsiLaunch is being built for the OMSI community, and contributions will be welcome once the initial public source tree is available.

Contribution guidelines, coding conventions and development environment instructions will be added before the repository opens for external development.

Until then, this repository serves as the public home of the project.

## Project links

- **Website:** https://omsilaunch.omsimods.com.br
- **OmsiModsBR:** https://omsimods.com.br
- **Source code:** this repository
- **Issues:** available after the first public source release
- **Discussions:** planned

## Disclaimer

OmsiLaunch is an independent community project and is not an official component of OMSI 2.

OMSI 2 and related names, trademarks and assets belong to their respective owners.

OmsiLaunch does not include or redistribute OMSI 2 game files.

---

<p align="center">
  <strong>OmsiLaunch</strong><br>
  Session control for OMSI 2.
</p>

<p align="center">
  Open source · Built for tools · Built for the community
</p>
