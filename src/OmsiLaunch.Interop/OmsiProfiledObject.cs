// Derived in concept from OmsiHook OmsiObject.cs at upstream commit
// 7687b6623f5f74b4419695257bd2a4eef54dd93e (LGPL-3.0-only).
namespace OmsiLaunch.Interop;

public sealed record OmsiObjectLayout(string SemanticName, uint ExpectedVmt, IReadOnlyDictionary<string, uint> Fields);

// A profile-backed wrapper. The address is never trusted until its VMT matches
// the layout selected for the exact Omsi.exe fingerprint.
public sealed class OmsiProfiledObject : OmsiRemoteObject
{
    public OmsiProfiledObject(IOmsiMemory memory, uint address, OmsiObjectLayout layout) : base(memory, address)
    {
        Layout = layout;
    }

    public OmsiObjectLayout Layout { get; }

    public async ValueTask<bool> ValidateAsync(CancellationToken cancellationToken = default)
    {
        return !IsNull && await Memory.ReadPointer32Async(Address, cancellationToken).ConfigureAwait(false) == Layout.ExpectedVmt;
    }

    public async ValueTask<T> ReadFieldAsync<T>(string semanticField, CancellationToken cancellationToken = default) where T : unmanaged
    {
        var address = ResolveField(semanticField);
        if (!await ValidateAsync(cancellationToken).ConfigureAwait(false)) throw new InvalidDataException($"{Layout.SemanticName} VMT did not match the active profile.");
        return await Memory.ReadValueAsync<T>(address, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask WriteFieldAsync<T>(string semanticField, T value, CancellationToken cancellationToken = default) where T : unmanaged
    {
        var address = ResolveField(semanticField);
        if (!await ValidateAsync(cancellationToken).ConfigureAwait(false)) throw new InvalidDataException($"{Layout.SemanticName} VMT did not match the active profile.");
        await Memory.WriteValueAsync(address, value, cancellationToken).ConfigureAwait(false);
    }

    private uint ResolveField(string semanticField)
    {
        if (!Layout.Fields.TryGetValue(semanticField, out var offset)) throw new KeyNotFoundException($"{Layout.SemanticName} does not define field '{semanticField}'.");
        return checked(Address + offset);
    }
}
