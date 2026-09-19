// Derived in concept from OmsiHook MemArrayStringList.cs at upstream commit
// 7687b6623f5f74b4419695257bd2a4eef54dd93e (LGPL-3.0-only).
namespace OmsiLaunch.Interop;

// Read-only adapter for a Delphi TList/OList whose entries are Delphi string
// data pointers. The list layout and encoding are profile data, not guesses.
public sealed class OmsiStringList
{
    private readonly IOmsiMemory memory;
    private readonly uint listAddress;
    private readonly OmsiPointerListLayout layout;
    private readonly OmsiStringEncoding encoding;

    public OmsiStringList(IOmsiMemory memory, uint listAddress, OmsiPointerListLayout layout, OmsiStringEncoding encoding)
    {
        this.memory = memory;
        this.listAddress = listAddress;
        this.layout = layout;
        this.encoding = encoding;
    }

    public async ValueTask<IReadOnlyList<string?>> SnapshotAsync(CancellationToken cancellationToken = default)
    {
        if (listAddress == 0) return Array.Empty<string>();
        var count = await memory.ReadValueAsync<int>(checked(listAddress + layout.CountOffset), cancellationToken).ConfigureAwait(false);
        if (count < 0 || count > layout.MaximumElements) throw new InvalidDataException("Invalid OMSI string-list count.");
        if (count == 0) return Array.Empty<string>();
        var data = await memory.ReadPointer32Async(checked(listAddress + layout.ArrayDataOffset), cancellationToken).ConfigureAwait(false);
        if (data == 0) throw new InvalidDataException("OMSI string list has elements but no item array.");
        var bytes = new byte[checked(count * sizeof(uint))];
        await memory.ReadExactAsync(data, bytes, cancellationToken).ConfigureAwait(false);
        var values = new string?[count];
        for (var index = 0; index < count; index++)
        {
            var pointer = System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(index * sizeof(uint)));
            values[index] = await memory.ReadStringAsync(pointer, encoding, pointerIsData: true, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        return values;
    }
}
