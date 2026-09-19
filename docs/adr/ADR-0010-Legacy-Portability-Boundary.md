# ADR-0010: Legacy Portability Boundary

Decision: OmsiLaunch preserves enough semantic/platform separation for future
Legacy NT6 and Legacy XP backports without requiring Current source compatibility
with legacy .NET Frameworks.

Portable authorities are LaunchSpec, errors/results, BuildProfile data, native
operation identities, startup wire protocol, configuration semantics, and
content identities. Platform-specific authorities are CLR hosting, DNNE,
process/shared-memory/filesystem implementations, and the supported Win32 API
set. Legacy ports begin only after Current is stable and before Current runtime
modernization.
