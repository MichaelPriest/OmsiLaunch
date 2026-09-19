namespace OmsiLaunch.Interop;

public interface IOmsiMemory
{
    ValueTask<int> ReadAsync(nint address, Memory<byte> destination, CancellationToken cancellationToken = default);
    ValueTask WriteAsync(nint address, ReadOnlyMemory<byte> source, CancellationToken cancellationToken = default);
}

// Allocation belongs to a profile-gated native service. Reading a Delphi
// value never implies that OmsiLaunch may allocate or free OMSI heap memory.
public interface IOmsiRemoteAllocator
{
    ValueTask<uint> AllocateAsync(uint byteCount, CancellationToken cancellationToken = default);
    ValueTask FreeAsync(uint address, CancellationToken cancellationToken = default);
}

// Selected OmsiHook-derived code belongs here only after provenance review.
// Public API and launch orchestration must not depend on addresses or this ABI.
