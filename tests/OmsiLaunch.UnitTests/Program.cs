using System.Buffers.Binary;
using OmsiLaunch.Builds.Omsi23004;
using OmsiLaunch.Interop;

var makeVehicleSymbols = Omsi23004.Profile.Methods;
if (!Omsi23004.Profile.CompatibleExecutableHashes.TryGetValue("7DAB063D1F62E73B3A2C7A6AC1921D7EDF5E5DB0FBC731481D117EEC8DE7D759", out var steamVariant) || steamVariant != "Steam LAA" ||
    Omsi23004.Profile.CompatibleExecutableHashes.ContainsKey("0000000000000000000000000000000000000000000000000000000000000000"))
    throw new InvalidOperationException("Compatible OMSI LAA executable hash allow-list regression.");
if (Omsi23004.Profile.Globals["ProgMan"] != 0x00858BDC || Omsi23004.Profile.Globals["ProgManLiveObject"] != 0x00862F28 ||
    Omsi23004.Profile.Globals["RoadVehicles"] != 0x00861508 || Omsi23004.Profile.Globals["RoadVehicleTypes"] != 0x008615A8 ||
    makeVehicleSymbols["TTempRVListCreate"].Address != 0x0074A0E0 || makeVehicleSymbols["TProgManMakeVehicle"].Address != 0x0070A250 ||
    makeVehicleSymbols["CopyTempListIntoMainList"].Address != 0x0074A240 || Omsi23004.Profile.ObjectLayouts["MakeVehicle"]["TempListVmt"] != 0x0074802C ||
    Omsi23004.Profile.ObjectLayouts["MakeVehicle"]["ProgramManagerCriticalSection"] != 0x1B4)
    throw new InvalidOperationException("MakeVehicle profile reconciliation changed unexpectedly.");

if (!Omsi23004.Profile.Globals.ContainsKey("Drivers") || !Omsi23004.Profile.Globals.ContainsKey("TicketPack") ||
    !Omsi23004.Profile.Globals.ContainsKey("TimeTableLogs") || !Omsi23004.Profile.ObjectLayouts.ContainsKey("ScriptConstants") ||
    !Omsi23004.Profile.ObjectLayouts.ContainsKey("RoadVehicleDefinition") || !Omsi23004.Profile.ObjectLayouts.ContainsKey("Driver") ||
    !Omsi23004.Profile.ObjectLayouts.ContainsKey("TicketPack") || !Omsi23004.Profile.ObjectLayouts.ContainsKey("TimeTableLog") ||
    !Omsi23004.Profile.ObjectLayouts.ContainsKey("MapTile") || !Omsi23004.Profile.ObjectLayouts["Map"].ContainsKey("Tiles"))
    throw new InvalidOperationException("Wave A profiled runtime layouts are incomplete.");

if (Omsi23004.Profile.Globals["D3DDevice"] != 0x008627D0 || !Omsi23004.Profile.HasEvidence("D3DDevice") ||
    OmsiRuntimeSurface.ForDomain(OmsiRuntimeDomain.D3D).Count != 5)
    throw new InvalidOperationException("Wave D device binding or public operation catalog is incomplete.");


var memory = new FakeMemory();
const uint list = 0x1000, wrapper = 0x2000, vehicleData = 0x3000, vehicle = 0x4000;
memory.WriteUInt32(Omsi23004.Profile.Globals["RoadVehicles"], list);
memory.WriteUInt32(list + Omsi23004.Profile.ObjectLayouts["RoadVehicleList"]["Items"], wrapper);
memory.WriteUInt32(wrapper + 4, vehicleData); memory.WriteInt32(wrapper + 8, 1); memory.WriteUInt32(vehicleData, vehicle);
memory.WriteInt32(vehicle + Omsi23004.Profile.ObjectLayouts["RoadVehicle"]["RuntimeIndex"], 42);

const uint humanData = 0x7000, human = 0x8000;
memory.WriteUInt32(Omsi23004.Profile.Globals["Humans"], humanData); memory.WriteInt32(humanData - 4, 1); memory.WriteUInt32(humanData, human);
memory.WriteInt32(human + Omsi23004.Profile.ObjectLayouts["Human"]["HumanIndex"], 7);

var readers = new OmsiRuntimeReaders(memory, Omsi23004.Profile.Globals, Omsi23004.Profile.ObjectLayouts);
var vehicles = await readers.ListRoadVehiclesAsync();
if (!vehicles.TryGetValue("vehicle.0.handle", out var vehicleHandle)) throw new InvalidOperationException("Road vehicle handle was not assigned.");
var vehicleSnapshot = await readers.ReadRoadVehicleAsync(vehicleHandle);
if (vehicleSnapshot["runtime_index"] != "42") throw new InvalidOperationException("Road vehicle profile layout was not applied.");

var humans = await readers.ListHumansAsync();
if (!humans.TryGetValue("human.0.handle", out var humanHandle)) throw new InvalidOperationException("Human handle was not assigned.");
var humanSnapshot = await readers.ReadHumanAsync(humanHandle);
if (humanSnapshot["human_index"] != "7") throw new InvalidOperationException("Human profile layout was not applied.");

var variables = Omsi23004.Profile.ObjectLayouts["ScriptVariables"];
const uint definition = 0x9000, state = 0xA000, variableNames = 0xB000, stringVariableNames = 0xB100, valueSlot = 0xC000, values = 0xD000, numericValue = 0xE000, stringValues = 0xF000;
memory.WriteUInt32(vehicle + variables["ComplMapObject"], definition);
memory.WriteUInt32(vehicle + variables["ComplObjectInstance"], state);
memory.WriteAnsiArray(definition + variables["VariableNames"], variableNames, "throttle");
memory.WriteAnsiArray(definition + variables["StringVariableNames"], stringVariableNames, "destination");
memory.WriteUInt32(vehicle + variables["PublicValues"], valueSlot); memory.WriteUInt32(valueSlot, values); memory.WriteInt32(values - 4, 1); memory.WriteUInt32(values, numericValue); memory.WriteSingle(numericValue, 12.5f);
memory.WriteUInt32(state + variables["StringValues"], stringValues); memory.WriteInt32(stringValues - 4, 1); memory.WriteUnicodeStringArrayEntry(stringValues, 0, "Spandau");
var listedVariables = await readers.ListRoadVehicleVariablesAsync(vehicleHandle, stringVariables: false);
if (listedVariables["name.0"] != "throttle" || listedVariables["truncated"] != "false") throw new InvalidOperationException("Numeric variable names were not decoded.");
var variable = await readers.ReadRoadVehicleVariableAsync(vehicleHandle, "throttle");
if (variable["value"] != "12.5") throw new InvalidOperationException("Numeric vehicle variable was not decoded.");
var writtenVariable = await readers.WriteRoadVehicleVariableAsync(vehicleHandle, "throttle", 7.25f);
if (writtenVariable["requested_value"] != "7.25" || writtenVariable["value"] != "7.25") throw new InvalidOperationException("Numeric vehicle variable write did not read back.");
var restoredVariable = await readers.WriteRoadVehicleVariableAsync(vehicleHandle, "throttle", 12.5f);
if (restoredVariable["value"] != "12.5") throw new InvalidOperationException("Numeric vehicle variable restore did not read back.");
var listedStringVariables = await readers.ListRoadVehicleVariablesAsync(vehicleHandle, stringVariables: true);
if (listedStringVariables["name.0"] != "destination") throw new InvalidOperationException("String variable names were not decoded.");
var stringVariable = await readers.ReadRoadVehicleStringVariableAsync(vehicleHandle, "destination");
if (stringVariable["value"] != "Spandau") throw new InvalidOperationException("String vehicle variable was not decoded.");

const uint timetableManager = 0x18000, timetableTracks = 0x18100, timetableEntries = 0x18200;
var timetableManagerLayout = Omsi23004.Profile.ObjectLayouts["TimeTableManager"];
var timetableTrackLayout = Omsi23004.Profile.ObjectLayouts["TimeTableTrack"];
var timetableEntryLayout = Omsi23004.Profile.ObjectLayouts["TimeTableTrackEntry"];
memory.WriteUInt32(Omsi23004.Profile.Globals["TimeTableManager"], timetableManager);
memory.WriteUInt32(timetableManager + timetableManagerLayout["Tracks"], timetableTracks); memory.WriteInt32(timetableTracks - 4, 1);
memory.WriteUInt32(timetableTracks + timetableTrackLayout["Entries"], timetableEntries); memory.WriteInt32(timetableEntries - 4, 1);
memory.WriteUInt32(timetableEntries + timetableEntryLayout["IdCode"], 77); memory.WriteInt32(timetableEntries + timetableEntryLayout["PathIndexOnObject"], 4);
memory.WriteInt32(timetableEntries + timetableEntryLayout["PathTile"], 9); memory.WriteInt32(timetableEntries + timetableEntryLayout["PathIndex"], 6);
memory.WriteSingle(timetableEntries + timetableEntryLayout["RelativeDistance"], 2.5f); memory.WriteSingle(timetableEntries + timetableEntryLayout["Distance"], 12.5f);
memory.WriteBool(timetableEntries + timetableEntryLayout["Valid"], true); memory.WriteByte(timetableEntries + timetableEntryLayout["PathOrderCheck"], 3);
memory.WriteInt32(timetableEntries + timetableEntryLayout["ChronoOrigin"], 5);
var trackEntries = await readers.ListTimeTableTrackEntriesAsync();
if (trackEntries["count"] != "1" || trackEntries["track_entry.0.id"] != "77" || trackEntries["track_entry.0.path_tile"] != "9" || trackEntries["track_entry.0.distance"] != "12.5") throw new InvalidOperationException("Timetable track-entry layout was not decoded.");

memory.WriteUInt32(vehicleData, 0);
try { await readers.ReadRoadVehicleAsync(vehicleHandle); throw new InvalidOperationException("Stale vehicle handle was accepted."); }
catch (InvalidOperationException exception) when (exception.Message == "OL_E_RUNTIME_OBJECT_HANDLE_STALE") { }

Console.WriteLine("PASS interop.profiled-entity-snapshots");

sealed class FakeMemory : IOmsiMemory
{
    private readonly Dictionary<uint, byte> bytes = new();
    public ValueTask<int> ReadAsync(nint address, Memory<byte> destination, CancellationToken cancellationToken = default)
    {
        for (var index = 0; index < destination.Length; index++) destination.Span[index] = bytes.TryGetValue(checked((uint)address + (uint)index), out var value) ? value : (byte)0;
        return ValueTask.FromResult(destination.Length);
    }
    public ValueTask WriteAsync(nint address, ReadOnlyMemory<byte> source, CancellationToken cancellationToken = default)
    {
        for (var index = 0; index < source.Length; index++) bytes[checked((uint)address + (uint)index)] = source.Span[index];
        return ValueTask.CompletedTask;
    }
    public void WriteUInt32(uint address, uint value) => WriteBytes(address, BitConverter.GetBytes(value));
    public void WriteInt32(uint address, int value) => WriteBytes(address, BitConverter.GetBytes(value));
    public void WriteSingle(uint address, float value) => WriteBytes(address, BitConverter.GetBytes(value));
    public void WriteBool(uint address, bool value) => bytes[address] = value ? (byte)1 : (byte)0;
    public void WriteByte(uint address, byte value) => bytes[address] = value;
    public void WriteUnicodeArray(uint pointerField, uint dataAddress, string value)
    {
        WriteUInt32(pointerField, dataAddress); WriteInt32(dataAddress - 4, 1); WriteUnicodeStringArrayEntry(dataAddress, 0, value);
    }
    public void WriteAnsiArray(uint pointerField, uint dataAddress, string value)
    {
        WriteUInt32(pointerField, dataAddress); WriteInt32(dataAddress - 4, 1); WriteAnsiStringArrayEntry(dataAddress, 0, value);
    }
    public void WriteAnsiStringArrayEntry(uint arrayData, int index, string value)
    {
        var dataAddress = checked(0x16000u + (arrayData - 0xB000u) + (uint)index * 0x100u);
        WriteUInt32(arrayData + checked((uint)index * 4), dataAddress); WriteInt32(dataAddress - 4, value.Length); WriteBytes(dataAddress, System.Text.Encoding.ASCII.GetBytes(value));
    }
    public void WriteUnicodeStringArrayEntry(uint arrayData, int index, string value)
    {
        var dataAddress = checked(0x11000u + (arrayData - 0xB000u) + (uint)index * 0x100u);
        WriteUInt32(arrayData + checked((uint)index * 4), dataAddress); WriteInt32(dataAddress - 4, value.Length); WriteBytes(dataAddress, System.Text.Encoding.Unicode.GetBytes(value));
    }
    private void WriteBytes(uint address, byte[] value) { for (var index = 0; index < value.Length; index++) bytes[address + (uint)index] = value[index]; }
}
