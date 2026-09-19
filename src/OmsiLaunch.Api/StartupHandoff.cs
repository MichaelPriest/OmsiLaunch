using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;

namespace OmsiLaunch.Api;

public sealed record StartupHandoff(
    Guid SessionId,
    string BuildProfileId,
    WorldMode WorldMode,
    string MapIdentity,
    int PresentedEntrypointIndex,
    bool HeadlessStart,
    bool PlayerVehicleEnabled,
    DateTimeMode DateMode,
    DateTimeMode TimeMode,
    string EntrypointIdentity = "",
    string SituationIdentity = "");

// Canonical portable wire protocol: little-endian fixed-width fields, UTF-8,
// and SHA-256 payload integrity. It contains no CLR object or pointer layout.
public static class StartupHandoffWire
{
    public const uint Magic = 0x4F4C5348;
    public const ushort Version = 4;
    private const int HeaderSize = 64;
    public static byte[] Serialize(StartupHandoff value)
    {
        var profile = Encoding.UTF8.GetBytes(value.BuildProfileId); var map = Encoding.UTF8.GetBytes(value.MapIdentity); var entrypoint = Encoding.UTF8.GetBytes(value.EntrypointIdentity); var situation = Encoding.UTF8.GetBytes(value.SituationIdentity);
        var payload = new byte[4 + profile.Length + 4 + map.Length + 4 + entrypoint.Length + 4 + situation.Length + 8]; var p = payload.AsSpan();
        BinaryPrimitives.WriteUInt32LittleEndian(p, (uint)profile.Length); profile.CopyTo(p[4..]); var offset = 4 + profile.Length;
        BinaryPrimitives.WriteUInt32LittleEndian(p[offset..], (uint)map.Length); map.CopyTo(p[(offset + 4)..]); offset += 4 + map.Length;
        BinaryPrimitives.WriteUInt32LittleEndian(p[offset..], (uint)entrypoint.Length); entrypoint.CopyTo(p[(offset + 4)..]); offset += 4 + entrypoint.Length;
        BinaryPrimitives.WriteUInt32LittleEndian(p[offset..], (uint)situation.Length); situation.CopyTo(p[(offset + 4)..]); offset += 4 + situation.Length;
        BinaryPrimitives.WriteInt32LittleEndian(p[offset..], value.PresentedEntrypointIndex); offset += 4;
        p[offset++] = (byte)value.WorldMode;
        p[offset++] = (byte)((value.HeadlessStart ? 0x01 : 0) | (value.PlayerVehicleEnabled ? 0x02 : 0));
        p[offset++] = (byte)value.DateMode;
        p[offset] = (byte)value.TimeMode;
        var output = new byte[HeaderSize + payload.Length]; var h = output.AsSpan(); BinaryPrimitives.WriteUInt32LittleEndian(h, Magic); BinaryPrimitives.WriteUInt16LittleEndian(h[4..], Version); BinaryPrimitives.WriteUInt16LittleEndian(h[6..], HeaderSize); BinaryPrimitives.WriteUInt32LittleEndian(h[8..], (uint)output.Length); value.SessionId.TryWriteBytes(h[12..28]); BinaryPrimitives.WriteUInt32LittleEndian(h[28..], (uint)payload.Length); SHA256.HashData(payload, h[32..64]); payload.CopyTo(h[64..]); return output;
    }
    public static bool TryDeserialize(ReadOnlySpan<byte> bytes, out StartupHandoff? value)
    {
        value = null; if (bytes.Length < HeaderSize || BinaryPrimitives.ReadUInt32LittleEndian(bytes) != Magic || BinaryPrimitives.ReadUInt16LittleEndian(bytes[6..]) != HeaderSize || BinaryPrimitives.ReadUInt32LittleEndian(bytes[8..]) != bytes.Length) return false;
        var version = BinaryPrimitives.ReadUInt16LittleEndian(bytes[4..]); if (version is not 3 and not Version) return false;
        var payloadSize = BinaryPrimitives.ReadUInt32LittleEndian(bytes[28..]); if (payloadSize != bytes.Length - HeaderSize) return false; var payload = bytes[HeaderSize..]; if (!CryptographicOperations.FixedTimeEquals(SHA256.HashData(payload), bytes[32..64]) || payload.Length < 20) return false;
        var profileLength = BinaryPrimitives.ReadUInt32LittleEndian(payload); if (profileLength > payload.Length - 20) return false; var offset = 4 + checked((int)profileLength); var mapLength = BinaryPrimitives.ReadUInt32LittleEndian(payload[offset..]); if (mapLength > payload.Length - offset - 16) return false;
        var profile = Encoding.UTF8.GetString(payload.Slice(4, checked((int)profileLength))); var map = Encoding.UTF8.GetString(payload.Slice(offset + 4, checked((int)mapLength))); offset += 4 + checked((int)mapLength);
        var entrypointLength = BinaryPrimitives.ReadUInt32LittleEndian(payload[offset..]); if (entrypointLength > payload.Length - offset - (version == 3 ? 12 : 16)) return false;
        var entrypoint = Encoding.UTF8.GetString(payload.Slice(offset + 4, checked((int)entrypointLength))); offset += 4 + checked((int)entrypointLength);
        var situation = string.Empty;
        if (version == Version)
        {
            var situationLength = BinaryPrimitives.ReadUInt32LittleEndian(payload[offset..]); if (situationLength > payload.Length - offset - 12) return false;
            situation = Encoding.UTF8.GetString(payload.Slice(offset + 4, checked((int)situationLength))); offset += 4 + checked((int)situationLength);
        }
        var index = BinaryPrimitives.ReadInt32LittleEndian(payload[offset..]); offset += 4;
        var worldMode = (WorldMode)payload[offset++]; var flags = payload[offset++]; var dateMode = (DateTimeMode)payload[offset++]; var timeMode = (DateTimeMode)payload[offset];
        if (!Enum.IsDefined(typeof(WorldMode), worldMode) || !Enum.IsDefined(typeof(DateTimeMode), dateMode) || !Enum.IsDefined(typeof(DateTimeMode), timeMode)) return false;
        value = new StartupHandoff(new Guid(bytes.Slice(12,16)), profile, worldMode, map, index, (flags & 0x01) != 0, (flags & 0x02) != 0, dateMode, timeMode, entrypoint, situation); return true;
    }
}
