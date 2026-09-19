// Derived in concept from OmsiHook Memory.cs at upstream commit
// 7687b6623f5f74b4419695257bd2a4eef54dd93e (LGPL-3.0-only).
// This implementation deliberately contains no OMSI addresses or RPC protocol.
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;

namespace OmsiLaunch.Interop;

public static class OmsiMemoryPrimitives
{
    public static async ValueTask<T> ReadValueAsync<T>(this IOmsiMemory memory, uint address, CancellationToken cancellationToken = default) where T : unmanaged
    {
        var bytes = new byte[Marshal.SizeOf<T>()];
        await memory.ReadExactAsync(address, bytes, cancellationToken).ConfigureAwait(false);
        return MemoryMarshal.Read<T>(bytes);
    }

    public static async ValueTask WriteValueAsync<T>(this IOmsiMemory memory, uint address, T value, CancellationToken cancellationToken = default) where T : unmanaged
    {
        var bytes = new byte[Marshal.SizeOf<T>()];
        MemoryMarshal.Write(bytes, ref value);
        await memory.WriteAsync((nint)address, bytes, cancellationToken).ConfigureAwait(false);
    }

    public static ValueTask<uint> ReadPointer32Async(this IOmsiMemory memory, uint address, CancellationToken cancellationToken = default) => memory.ReadValueAsync<uint>(address, cancellationToken);

    // Delphi dynamic arrays and long strings store their element count/length at data - 4.
    public static async ValueTask<int> ReadDelphiLengthAsync(this IOmsiMemory memory, uint dataAddress, CancellationToken cancellationToken = default)
    {
        if (dataAddress < sizeof(int)) throw new ArgumentOutOfRangeException(nameof(dataAddress));
        return await memory.ReadValueAsync<int>(dataAddress - sizeof(int), cancellationToken).ConfigureAwait(false);
    }

    public static async ValueTask<string?> ReadDelphiUnicodeStringAsync(this IOmsiMemory memory, uint pointerAddress, int maximumCharacters = 16 * 1024, CancellationToken cancellationToken = default)
    {
        var dataAddress = await memory.ReadPointer32Async(pointerAddress, cancellationToken).ConfigureAwait(false);
        return await memory.ReadDelphiUnicodeStringDataAsync(dataAddress, maximumCharacters, cancellationToken).ConfigureAwait(false);
    }

    public static async ValueTask<string?> ReadDelphiUnicodeStringDataAsync(this IOmsiMemory memory, uint dataAddress, int maximumCharacters = 16 * 1024, CancellationToken cancellationToken = default)
    {
        if (dataAddress == 0) return null;
        var length = await memory.ReadDelphiLengthAsync(dataAddress, cancellationToken).ConfigureAwait(false);
        if (length < 0 || length > maximumCharacters) throw new InvalidDataException("Invalid Delphi UnicodeString length.");
        var bytes = new byte[checked(length * sizeof(char))];
        await memory.ReadExactAsync(dataAddress, bytes, cancellationToken).ConfigureAwait(false);
        return Encoding.Unicode.GetString(bytes);
    }

    public static async ValueTask<uint[]> ReadDelphiPointerArrayAsync(this IOmsiMemory memory, uint dataAddress, int maximumElements = 16 * 1024, CancellationToken cancellationToken = default)
    {
        if (dataAddress == 0) return Array.Empty<uint>();
        var length = await memory.ReadDelphiLengthAsync(dataAddress, cancellationToken).ConfigureAwait(false);
        if (length < 0 || length > maximumElements) throw new InvalidDataException("Invalid Delphi dynamic-array length.");
        var bytes = new byte[checked(length * sizeof(uint))];
        await memory.ReadExactAsync(dataAddress, bytes, cancellationToken).ConfigureAwait(false);
        var output = new uint[length];
        for (var index = 0; index < output.Length; index++) output[index] = BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(index * sizeof(uint)));
        return output;
    }

    public static async ValueTask ReadExactAsync(this IOmsiMemory memory, uint address, Memory<byte> destination, CancellationToken cancellationToken = default)
    {
        var read = await memory.ReadAsync((nint)address, destination, cancellationToken).ConfigureAwait(false);
        if (read != destination.Length) throw new EndOfStreamException("OMSI memory read was incomplete.");
    }
}

// Internal runtime wrappers carry explicit 32-bit OMSI addresses. They are not
// part of the platform-neutral public OmsiLaunch API.
public abstract class OmsiRemoteObject
{
    protected OmsiRemoteObject(IOmsiMemory memory, uint address) { Memory = memory; Address = address; }
    protected IOmsiMemory Memory { get; }
    public uint Address { get; }
    public bool IsNull => Address == 0;
}
