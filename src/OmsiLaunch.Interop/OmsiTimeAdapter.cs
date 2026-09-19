namespace OmsiLaunch.Interop;

// Profile values are injected by the build-specific assembly. This adapter
// owns no absolute address and can therefore be reused by another profile.
public sealed record OmsiTimeLayout(uint Hour, uint Minute, uint Second, uint Day, uint Month, uint Year);
public sealed record OmsiTimeSnapshot(byte Hour, byte Minute, float Second, int Day, int Month, int Year);

public sealed class OmsiTimeAdapter
{
    private readonly IOmsiMemory memory;
    private readonly OmsiTimeLayout layout;

    public OmsiTimeAdapter(IOmsiMemory memory, OmsiTimeLayout layout) { this.memory = memory; this.layout = layout; }

    public async ValueTask<OmsiTimeSnapshot> ReadAsync(CancellationToken cancellationToken = default) => new(
        await memory.ReadValueAsync<byte>(layout.Hour, cancellationToken).ConfigureAwait(false),
        await memory.ReadValueAsync<byte>(layout.Minute, cancellationToken).ConfigureAwait(false),
        await memory.ReadValueAsync<float>(layout.Second, cancellationToken).ConfigureAwait(false),
        await memory.ReadValueAsync<int>(layout.Day, cancellationToken).ConfigureAwait(false),
        await memory.ReadValueAsync<int>(layout.Month, cancellationToken).ConfigureAwait(false),
        await memory.ReadValueAsync<int>(layout.Year, cancellationToken).ConfigureAwait(false));

    // Scalar clock fields are the OmsiHook 2.5.3 surface reconciled for this
    // profile. Call the profiled native SetTime routine after a successful write.
    public async ValueTask WriteClockAsync(byte? hour, byte? minute, float? second, CancellationToken cancellationToken = default)
    {
        if (hour is { } valueHour) await memory.WriteValueAsync(layout.Hour, valueHour, cancellationToken).ConfigureAwait(false);
        if (minute is { } valueMinute) await memory.WriteValueAsync(layout.Minute, valueMinute, cancellationToken).ConfigureAwait(false);
        if (second is { } valueSecond) await memory.WriteValueAsync(layout.Second, valueSecond, cancellationToken).ConfigureAwait(false);
    }
}
