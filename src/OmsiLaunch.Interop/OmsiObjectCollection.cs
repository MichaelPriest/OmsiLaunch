// Derived in concept from OmsiHook MemArrayObjList.cs at upstream commit
// 7687b6623f5f74b4419695257bd2a4eef54dd93e (LGPL-3.0-only).
namespace OmsiLaunch.Interop;

// Read-only by design. Replacing Delphi collection pointers is not a safe
// generic operation; a profile-specific native operation must own mutations.
public sealed class OmsiObjectCollection<TObject> where TObject : OmsiRemoteObject
{
    private readonly IOmsiMemory memory;
    private readonly uint arrayPointerAddress;
    private readonly Func<IOmsiMemory, uint, TObject> factory;

    public OmsiObjectCollection(IOmsiMemory memory, uint arrayPointerAddress, Func<IOmsiMemory, uint, TObject> factory)
    {
        this.memory = memory;
        this.arrayPointerAddress = arrayPointerAddress;
        this.factory = factory;
    }

    public async ValueTask<IReadOnlyList<TObject>> SnapshotAsync(int maximumElements = 16 * 1024, CancellationToken cancellationToken = default)
    {
        var addresses = await memory.ReadObjectArrayAsync(arrayPointerAddress, maximumElements: maximumElements, cancellationToken: cancellationToken).ConfigureAwait(false);
        return Array.ConvertAll(addresses, address => factory(memory, address.Value));
    }
}
