namespace OmsiLaunch.Configuration;

public sealed record ConfigurationSetting(string Key, string FileName, string Token, bool IsPresenceToken, bool Inverted, bool Writable, string Evidence, int? VectorIndex = null, decimal? Minimum = null, decimal? Maximum = null, decimal? Divisor = null, IReadOnlySet<string>? AllowedValues = null);

// Public names describe positive product semantics; raw OMSI negative tokens stay internal.
public static class ConfigurationCatalog
{
    private static readonly IReadOnlyDictionary<string, ConfigurationSetting> Settings =
        new Dictionary<string, ConfigurationSetting>(StringComparer.OrdinalIgnoreCase)
        {
            ["general.language"] = new("general.language", "options.cfg", "language", false, false, true, "STATICALLY_VALIDATED"),
            ["general.radio"] = new("general.radio", "options.cfg", "radio", false, false, true, "STATICALLY_VALIDATED"),
            ["general.alternateView"] = new("general.alternateView", "options.cfg", "altView", true, false, true, "STATICALLY_VALIDATED"),
            ["general.showOwnDriver"] = new("general.showOwnDriver", "options.cfg", "see_own_driver", true, false, true, "STATICALLY_VALIDATED"),
            ["general.showErrorMessages"] = new("general.showErrorMessages", "options.cfg", "showerrormessages", true, false, true, "STATICALLY_VALIDATED"),
            ["general.autoSave"] = new("general.autoSave", "options.cfg", "noAutoSave", true, true, true, "STATICALLY_VALIDATED"),
            ["general.currentTime"] = new("general.currentTime", "options.cfg", "useActTime", true, false, true, "STATICALLY_VALIDATED"),
            ["general.currentDate"] = new("general.currentDate", "options.cfg", "useActDate", true, false, true, "STATICALLY_VALIDATED"),
            ["general.currentYear"] = new("general.currentYear", "options.cfg", "useActYear", true, false, true, "STATICALLY_VALIDATED"),
            ["graphics.screenRatio"] = new("graphics.screenRatio", "options.cfg", "screenratio", false, false, true, "STATICALLY_VALIDATED"),
            ["graphics.maxFPS"] = new("graphics.maxFPS", "options.cfg", "maxFPS", false, false, true, "STATICALLY_VALIDATED", null, 10, 200),
            ["graphics.tileDistance"] = new("graphics.tileDistance", "options.cfg", "performance_tiledistmax", false, false, true, "STATICALLY_VALIDATED", null, 1, 20),
            ["graphics.maxObjectDistanceMeters"] = new("graphics.maxObjectDistanceMeters", "options.cfg", "performance_maxObjDist", false, false, true, "STATICALLY_VALIDATED", null, 20, 5000),
            ["graphics.minObjectScreenPercent"] = new("graphics.minObjectScreenPercent", "options.cfg", "performance_minObjSize", false, false, true, "STATICALLY_VALIDATED", null, 0, 10, 100),
            ["graphics.minReflectionObjectScreenPercent"] = new("graphics.minReflectionObjectScreenPercent", "options.cfg", "performance_minObjSizeRefl", false, false, true, "STATICALLY_VALIDATED", null, 0, 50, 100),
            ["graphics.maxObjectComplexity"] = new("graphics.maxObjectComplexity", "options.cfg", "maxcomplexity", false, false, true, "STATICALLY_VALIDATED", null, 0, 3),
            ["graphics.maxMapComplexity"] = new("graphics.maxMapComplexity", "options.cfg", "maxcomplexity_map", false, false, true, "STATICALLY_VALIDATED", null, 0, 2),
            ["graphics.sunGlow"] = new("graphics.sunGlow", "options.cfg", "sunglow", true, false, true, "STATICALLY_VALIDATED"),
            ["graphics.loadAllTiles"] = new("graphics.loadAllTiles", "options.cfg", "loadAllTiles", true, false, true, "STATICALLY_VALIDATED"),
            ["graphics.stencilBuffer"] = new("graphics.stencilBuffer", "options.cfg", "no_stencilbuffer", true, true, true, "STATICALLY_VALIDATED"),
            ["graphics.stencilShadows"] = new("graphics.stencilShadows", "options.cfg", "shadow_stencil", false, false, true, "STATICALLY_VALIDATED"),
            ["graphics.rainReflections"] = new("graphics.rainReflections", "options.cfg", "no_rain_refl", true, true, true, "STATICALLY_VALIDATED"),
            ["graphics.humansInRainReflections"] = new("graphics.humansInRainReflections", "options.cfg", "no_humans_on_rain_refl", true, true, true, "STATICALLY_VALIDATED"),
            ["graphics.realTimeReflections"] = new("graphics.realTimeReflections", "options.cfg", "performance_realreflexions", false, false, true, "STATICALLY_PARTIAL", null, null, null, null, new HashSet<string>(new[] { "economy", "full" }, StringComparer.OrdinalIgnoreCase)),
            ["graphics.particles"] = new("graphics.particles", "options.cfg", "smokesystems", false, false, true, "STATICALLY_VALIDATED"),
            ["simulation.collision"] = new("simulation.collision", "options.cfg", "no_collision", true, true, true, "STATICALLY_VALIDATED"),
            ["simulation.collisionTerrain"] = new("simulation.collisionTerrain", "options.cfg", "no_collision_terrain", true, true, true, "STATICALLY_VALIDATED"),
            ["simulation.collisionVehicles"] = new("simulation.collisionVehicles", "options.cfg", "no_collision_vehToVeh", true, true, true, "STATICALLY_VALIDATED"),
            ["simulation.collisionPedestrians"] = new("simulation.collisionPedestrians", "options.cfg", "no_collision_pedastrians", true, true, true, "STATICALLY_VALIDATED"),
            ["simulation.ticketSelling"] = new("simulation.ticketSelling", "options.cfg", "ticketselling", false, false, true, "STATICALLY_VALIDATED", null, 0, 2),
            ["simulation.maintenance"] = new("simulation.maintenance", "options.cfg", "wear_lifespan", false, false, true, "STATICALLY_VALIDATED", null, 0, 4),
            ["simulation.disableAutomaticScheduleAnalysisPopup"] = new("simulation.disableAutomaticScheduleAnalysisPopup", "options.cfg", "no_schedAnaPopUp", true, false, true, "STATICALLY_VALIDATED"),
            ["simulation.ticketInfo"] = new("simulation.ticketInfo", "options.cfg", "no_ticketinfo_visible", true, true, true, "STATICALLY_VALIDATED"),
            ["simulation.automaticClutch"] = new("simulation.automaticClutch", "options.cfg", "no_automaticClutch", true, true, true, "STATICALLY_VALIDATED"),
            ["advanced.reducedMultithreading"] = new("advanced.reducedMultithreading", "options.cfg", "no_multithreading_calculate+no_multithreading_texload", true, false, true, "RUNTIME_PROVEN"),
            ["advanced.multithreadingCalculate"] = new("advanced.multithreadingCalculate", "options.cfg", "no_multithreading_calculate", true, true, false, "SUPERSEDED_BY_REDUCED_MULTITHREADING"),
            ["advanced.multithreadingTextureLoad"] = new("advanced.multithreadingTextureLoad", "options.cfg", "no_multithreading_texload", true, true, false, "SUPERSEDED_BY_REDUCED_MULTITHREADING"),
            ["view.driverSmooth"] = new("view.driverSmooth", "options.cfg", "driverview_smooth", true, false, true, "STATICALLY_VALIDATED"),
            ["view.driverMoving"] = new("view.driverMoving", "options.cfg", "driverview_moving", true, false, true, "STATICALLY_VALIDATED"),
            ["controls.autoCenter"] = new("controls.autoCenter", "options.cfg", "autoCenter", true, false, true, "STATICALLY_VALIDATED"),
            ["controls.reducedSteeringSpeed"] = new("controls.reducedSteeringSpeed", "options.cfg", "redSteerSpd", true, false, true, "STATICALLY_VALIDATED"),
            ["traffic.randomVehicles"] = new("traffic.randomVehicles", "options.cfg", "AIMaxCountRandom", false, false, true, "STATICALLY_VALIDATED", 0, 0, 1000),
            ["traffic.humans"] = new("traffic.humans", "options.cfg", "AIMaxCountRandom", false, false, true, "STATICALLY_VALIDATED", 1, 0, 1000),
            ["traffic.factorPercent"] = new("traffic.factorPercent", "options.cfg", "AIUnschedFactor", false, false, true, "STATICALLY_VALIDATED", null, 1, 300),
            ["traffic.parkedVehiclesPercent"] = new("traffic.parkedVehiclesPercent", "options.cfg", "AIMaxCountParked", false, false, true, "STATICALLY_VALIDATED", null, 0, 100),
            ["traffic.scheduledVehicles"] = new("traffic.scheduledVehicles", "options.cfg", "AIMaxCountScheduled", false, false, true, "STATICALLY_VALIDATED", null, 0, 1000),
            ["traffic.scheduledLinePriority"] = new("traffic.scheduledLinePriority", "options.cfg", "AIPriorityScheduled", false, false, true, "STATICALLY_VALIDATED", null, 1, 4),
            ["traffic.passengerFactorPercent"] = new("traffic.passengerFactorPercent", "options.cfg", "AIPassFactor", false, false, true, "STATICALLY_VALIDATED", null, 0, 200),
            ["sound.stereo"] = new("sound.stereo", "options.cfg", "sound_stereo", false, false, true, "STATICALLY_VALIDATED", null, 0, 100),
            ["sound.maxSimultaneousSounds"] = new("sound.maxSimultaneousSounds", "options.cfg", "sound_maxcount", false, false, true, "STATICALLY_VALIDATED", null, 5, 1000),
            ["sound.masterVolume"] = new("sound.masterVolume", "options.cfg", "sound_vol_master", false, false, true, "STATICALLY_VALIDATED", null, 0, 1),
            ["graphics.texture"] = new("graphics.texture", "options.cfg", "texture", false, false, false, "STATICALLY_PARTIAL"),
            ["graphics.textureFilter"] = new("graphics.textureFilter", "options.cfg", "texFilter", false, false, false, "STATICALLY_PARTIAL")
        };

    public static bool TryGet(string key, out ConfigurationSetting setting) => Settings.TryGetValue(key, out setting!);
    public static IReadOnlyCollection<ConfigurationSetting> All => Settings.Values.ToArray();

    public static IConfigSemanticPatch CreatePatch(string key, string value)
    {
        if (!TryGet(key, out var setting)) throw new ArgumentException("OL_E_UNKNOWN_SETTING: " + key, nameof(key));
        if (!setting.Writable) throw new InvalidOperationException("OL_E_SETTING_NOT_WRITABLE: " + key);
        if (string.Equals(key, "advanced.reducedMultithreading", StringComparison.OrdinalIgnoreCase))
        {
            if (!bool.TryParse(value, out var reduced)) throw new ArgumentException("OL_E_INVALID_SETTING_VALUE: " + key, nameof(value));
            return new ReducedMultithreadingPatch(reduced);
        }
        if (string.Equals(key, "graphics.stencilShadows", StringComparison.OrdinalIgnoreCase))
        {
            if (!bool.TryParse(value, out var stencilShadows)) throw new ArgumentException("OL_E_INVALID_SETTING_VALUE: " + key, nameof(value));
            return new BooleanValueTokenPatch(setting.Token, stencilShadows, "on", "off");
        }
        if (string.Equals(key, "graphics.particles", StringComparison.OrdinalIgnoreCase))
        {
            var fields = value.Split(',', StringSplitOptions.TrimEntries);
            if (fields.Length != 4 || !bool.TryParse(fields[0], out var enabled) || !int.TryParse(fields[1], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var maxPerEmitter) || maxPerEmitter < 0 || !bool.TryParse(fields[2], out var playerVehicleOnly) || !bool.TryParse(fields[3], out var inReflections)) throw new ArgumentException("OL_E_INVALID_SETTING_VALUE: " + key, nameof(value));
            return new SmokeSystemsPatch(enabled, maxPerEmitter, playerVehicleOnly, inReflections);
        }
        if (setting.VectorIndex is { } index)
        {
            if (!int.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var numeric) || numeric < setting.Minimum.GetValueOrDefault() || numeric > setting.Maximum.GetValueOrDefault(int.MaxValue)) throw new ArgumentException("OL_E_INVALID_SETTING_VALUE: " + key, nameof(value));
            return new AiMaxCountRandomPatch(index, numeric);
        }
        if (!setting.IsPresenceToken)
        {
            if (setting.AllowedValues is not null && !setting.AllowedValues.Contains(value)) throw new ArgumentException("OL_E_INVALID_SETTING_VALUE: " + key, nameof(value));
            if (setting.Minimum is { } minimum)
            {
                if (!decimal.TryParse(value, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var numeric) || numeric < minimum || numeric > setting.Maximum) throw new ArgumentException("OL_E_INVALID_SETTING_VALUE: " + key, nameof(value));
                value = (setting.Divisor is { } divisor ? numeric / divisor : numeric).ToString(System.Globalization.CultureInfo.InvariantCulture);
            }
            return new TokenPatch(setting.FileName, setting.Token, value);
        }
        if (!bool.TryParse(value, out var boolean)) throw new ArgumentException("OL_E_INVALID_SETTING_VALUE: " + key, nameof(value));
        return new TokenPatch(setting.FileName, setting.Token, null, setting.Inverted ? !boolean : boolean);
    }
}
