// Derived in concept from OmsiHook MemArrayList.cs and MemArrayObjList.cs at
// upstream commit 7687b6623f5f74b4419695257bd2a4eef54dd93e (LGPL-3.0-only).
namespace OmsiLaunch.Interop;

// The offsets are profile data supplied by the caller. This supports Delphi
// TList/OList-style collections without embedding a historical OmsiHook layout.
public sealed record OmsiPointerListLayout(uint ArrayDataOffset, uint CountOffset, int MaximumElements = 16 * 1024)
{
    public static readonly OmsiPointerListLayout DelphiTList = new(0x04, 0x08);
    public static readonly OmsiPointerListLayout OmsiOList = new(0x04, 0x0C);
}

public sealed class OmsiPointerList<TObject> where TObject : OmsiRemoteObject
{
    private readonly IOmsiMemory memory;
    private readonly uint listAddress;
    private readonly OmsiPointerListLayout layout;
    private readonly Func<IOmsiMemory, uint, TObject> factory;

    public OmsiPointerList(IOmsiMemory memory, uint listAddress, OmsiPointerListLayout layout, Func<IOmsiMemory, uint, TObject> factory)
    {
        this.memory = memory;
        this.listAddress = listAddress;
        this.layout = layout;
        this.factory = factory;
    }

    public async ValueTask<IReadOnlyList<TObject?>> SnapshotAsync(CancellationToken cancellationToken = default)
    {
        if (listAddress == 0) return Array.Empty<TObject>();
        var count = await memory.ReadValueAsync<int>(checked(listAddress + layout.CountOffset), cancellationToken).ConfigureAwait(false);
        if (count < 0 || count > layout.MaximumElements) throw new InvalidDataException("Invalid OMSI pointer-list count.");
        if (count == 0) return Array.Empty<TObject>();
        var arrayAddress = await memory.ReadPointer32Async(checked(listAddress + layout.ArrayDataOffset), cancellationToken).ConfigureAwait(false);
        if (arrayAddress == 0) throw new InvalidDataException("OMSI pointer list has elements but no item array.");
        var bytes = new byte[checked(count * sizeof(uint))];
        await memory.ReadExactAsync(arrayAddress, bytes, cancellationToken).ConfigureAwait(false);
        var result = new TObject?[count];
        for (var index = 0; index < count; index++)
        {
            var address = System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(index * sizeof(uint)));
            result[index] = address == 0 ? null : factory(memory, address);
        }
        return result;
    }
}
