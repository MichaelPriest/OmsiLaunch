# Local Control Protocol

`OmsiLaunch.exe /serve` owns one managed session and exposes a current-user-only
named-pipe control endpoint. Secondary `OmsiLaunch.exe` invocations act as
clients; they never create a competing host for that session.

Protocol version: `0.1`.

Supported client routes:

- `session status --json`
- `session stop --json`
- `events read --json`
- `events watch --json` (until `Ctrl+C`)
- Runtime aliases such as `time get --json` and `time set --hour=18 --minute=10 --json`

Each request and response is length-prefixed JSON and bounded to 64 KiB. The
endpoint is local-only and uses `PipeOptions.CurrentUserOnly`. It is separate
from the host-to-plugin mailbox, which remains session/process-bound and is not
a public executable IPC surface.

When no active host exists, clients return `OL_E_NO_ACTIVE_SESSION` and exit
with code `4`. A client request never starts OMSI implicitly.

Every result is an envelope with `ok`, `command`, `protocol_version`, and
either `result` or a structured `error`. The initial stable exit-code contract
is: `0` success, `2` invalid arguments, `3` unsupported profile, `4` no
active session, `5` unavailable runtime, `6` not found, `7` operation
rejected, `8` recovery failure, and `10` internal failure. Error categories
are semantic (`invalid_argument`, `unsupported_profile`, `session`,
`runtime`, `not_found`, `transaction`, or `internal`); callers must not parse
human-readable messages.

Runtime commands receive unique request IDs inside the owning session. The
public pipe transports no OMSI pointers and does not expose the host-to-plugin
mailbox name. All handles returned by a runtime command remain session scoped
and become stale when the owner stops or closes the session.

Unknown runtime operation names are rejected locally as
`OL_E_RUNTIME_OPERATION_UNKNOWN`; the CLI does not create a session or forward
an unrecognized native operation.

The currently active public command registry is available through:

```powershell
OmsiLaunch.exe capabilities --json
OmsiLaunch.exe help time --json
```
