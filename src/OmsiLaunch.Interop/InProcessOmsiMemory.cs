using System.Runtime.InteropServices;

namespace OmsiLaunch.Interop;

// Current in-process provider. It deliberately validates the requested page
// before copying, rather than adopting OmsiHook's external-process attach path.
public sealed class InProcessOmsiMemory : IOmsiMemory
{
    public unsafe ValueTask<int> ReadAsync(nint address, Memory<byte> destination, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Validate(address, destination.Length, write: false);
        using var pin = destination.Pin();
        Buffer.MemoryCopy((void*)address, pin.Pointer, destination.Length, destination.Length);
        return ValueTask.FromResult(destination.Length);
    }

    public unsafe ValueTask WriteAsync(nint address, ReadOnlyMemory<byte> source, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Validate(address, source.Length, write: true);
        using var pin = source.Pin();
        Buffer.MemoryCopy(pin.Pointer, (void*)address, source.Length, source.Length);
        return ValueTask.CompletedTask;
    }

    private static void Validate(nint address, int length, bool write)
    {
        if (address == 0 || length <= 0) throw new ArgumentOutOfRangeException(nameof(address));
        if (VirtualQuery(address, out var info, (nuint)Marshal.SizeOf<MemoryBasicInformation>()) == 0 ||
            info.State != MemCommit || (info.Protect & (PageGuard | PageNoAccess)) != 0)
            throw new InvalidOperationException("OMSI memory page is not accessible.");

        var start = unchecked((nuint)address);
        var end = start + checked((nuint)length);
        var regionEnd = unchecked((nuint)info.BaseAddress) + info.RegionSize;
        if (end < start || end > regionEnd || (write && !IsWritable(info.Protect)))
            throw new InvalidOperationException("OMSI memory range is not valid for the requested operation.");
    }

    private static bool IsWritable(uint protect) => (protect & 0xff) is PageReadWrite or PageWriteCopy or PageExecuteReadWrite or PageExecuteWriteCopy;
    private const uint MemCommit = 0x1000;
    private const uint PageNoAccess = 0x01;
    private const uint PageReadWrite = 0x04;
    private const uint PageWriteCopy = 0x08;
    private const uint PageExecuteReadWrite = 0x40;
    private const uint PageExecuteWriteCopy = 0x80;
    private const uint PageGuard = 0x100;

    [StructLayout(LayoutKind.Sequential)]
    private struct MemoryBasicInformation
    {
        public nint BaseAddress;
        public nint AllocationBase;
        public uint AllocationProtect;
        public nuint RegionSize;
        public uint State;
        public uint Protect;
        public uint Type;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nuint VirtualQuery(nint address, out MemoryBasicInformation buffer, nuint length);
}
