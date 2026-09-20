# Host application integration

OmsiLaunch is designed so graphical frontends and other OMSI tools can use the
same session engine without reproducing command-line behavior.

## Recommended boundary

External applications should integrate at a semantic boundary:

```text
Host application
    -> OmsiLaunch.Api (IOmsiLaunch / LaunchSpec / runtime commands)
        -> OmsiLaunch.Core
            -> OmsiLaunch.Process
            -> OmsiLaunch.Plugin / Native runtime
                -> OMSI 2
```

The host application should not patch `Omsi.exe`, depend on internal native
addresses, or duplicate transaction/recovery logic. Those responsibilities stay
inside OmsiLaunch and the selected exact BuildProfile.

## In-process .NET integration

A compatible .NET host can reference the public API and construct an
`IOmsiLaunch` implementation using the packaged runtime artifacts. This is the
same boundary used by `OmsiLaunch.Launcher.exe`.

Typical host responsibilities are:

1. let the user select or detect the OMSI installation;
2. discover maps, situations, vehicles, HOFs, and entrypoints;
3. build a semantic `LaunchSpec`;
4. call `PlanSessionAsync` and present diagnostics;
5. call `StartSessionAsync`;
6. monitor `SessionStatus` and runtime events;
7. use semantic runtime commands while the session is running;
8. call `StopAsync` / `CloseAsync` and allow exact restore to finish.

## Process-isolated integration

Applications that should not load OmsiLaunch assemblies directly can use the
local control surface exposed by the controller. This keeps the OMSI session
owner in a separate process while still exposing semantic status and runtime
operations.

## Future consumers

This boundary is intentionally suitable for independent OMSI tools such as a
multiplayer client or map editor. Those projects should remain separate
repositories/products and consume the public contract instead of copying
OmsiLaunch internals.

The public integration contract must remain build-profile aware. A host must
never treat an unknown executable as compatible merely because its file name or
product version looks similar.
