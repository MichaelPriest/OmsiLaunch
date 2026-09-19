using System.Globalization;

namespace OmsiLaunch.Interop;

public sealed class OmsiCameraWriter
{
    private readonly IOmsiMemory memory;
    private readonly IReadOnlyDictionary<string, uint> globals;
    private readonly IReadOnlyDictionary<string, uint> layout;

    public OmsiCameraWriter(IOmsiMemory memory, IReadOnlyDictionary<string, uint> globals, IReadOnlyDictionary<string, IReadOnlyDictionary<string, uint>> layouts)
    {
        this.memory = memory; this.globals = globals;
        layout = layouts.TryGetValue("Camera", out var value) ? value : throw new InvalidOperationException("Missing profiled camera layout.");
    }

    public async ValueTask WriteAsync(IReadOnlyDictionary<string, string> values, CancellationToken cancellationToken = default)
    {
        var wrote = false;
        if (values.TryGetValue("family", out var familyText))
        {
            if (!int.TryParse(familyText, NumberStyles.None, CultureInfo.InvariantCulture, out var family) || family is < 0 or > 3)
                throw new ArgumentOutOfRangeException("family", "OL_E_RUNTIME_VALUE_OUT_OF_RANGE");
            await memory.WriteValueAsync(globals["CameraFamily"], family, cancellationToken).ConfigureAwait(false); wrote = true;
        }
        if (values.TryGetValue("field_of_view", out var fovText))
        {
            if (!float.TryParse(fovText, NumberStyles.Float, CultureInfo.InvariantCulture, out var fov) || float.IsNaN(fov) || float.IsInfinity(fov) || fov is < 10 or > 170)
                throw new ArgumentOutOfRangeException("field_of_view", "OL_E_RUNTIME_VALUE_OUT_OF_RANGE");
            var camera = await memory.ReadPointer32Async(globals["Camera"], cancellationToken).ConfigureAwait(false);
            if (camera == 0) throw new InvalidOperationException("OMSI camera object is unavailable.");
            await memory.WriteValueAsync(camera + layout["FieldOfView"], fov, cancellationToken).ConfigureAwait(false); wrote = true;
        }
        if (!wrote) throw new InvalidOperationException("OL_E_RUNTIME_ARGUMENT_REQUIRED");
    }
}
