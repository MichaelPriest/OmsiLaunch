using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using Microsoft.Win32;
using OmsiLaunch.Api;
using OmsiLaunch.Core;
using OmsiLaunch.Configuration;
using OmsiLaunch.Process;

namespace OmsiLaunch.Gui;

public partial class MainWindow : Window
{
    private readonly IOmsiLaunch launch;
    private SessionHandle? activeSession;
    private CancellationTokenSource? monitorCancellation;
    private bool allowWindowClose;
    private bool refreshingContent;
    private bool recoveryPending;

    private sealed record Choice(string Display, string Identity);
    private sealed record LauncherSettings(string? OmsiExecutable);

    public MainWindow()
    {
        var pluginRuntime = Path.Combine(AppContext.BaseDirectory, "plugins");
        var nativeRuntime = Path.Combine(pluginRuntime, "OmsiLaunch.Native.x86.dll");
        launch = new OmsiLaunchService(
            new CurrentWindowsX64Platform(),
            new OmsiLaunchRuntimePaths(pluginRuntime, nativeRuntime));

        InitializeComponent();
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        var localOmsi = Path.Combine(AppContext.BaseDirectory, "Omsi.exe");
        var savedOmsi = LoadSettings()?.OmsiExecutable;

        if (File.Exists(localOmsi))
        {
            ExecutablePathTextBox.Text = localOmsi;
            AppendLog("OMSI detectado na mesma pasta do OmsiLaunch.");
            await RefreshContentAsync();
        }
        else if (!string.IsNullOrWhiteSpace(savedOmsi) && File.Exists(savedOmsi))
        {
            ExecutablePathTextBox.Text = savedOmsi;
            AppendLog("Instalação do OMSI restaurada das preferências do launcher.");
            await RefreshContentAsync();
        }
        else
        {
            AppendLog("Selecione o arquivo Omsi.exe para começar.");
        }
    }

    private async void BrowseExecutable_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Selecione o Omsi.exe",
            Filter = "OMSI 2 (Omsi.exe)|Omsi.exe|Executáveis (*.exe)|*.exe",
            CheckFileExists = true,
            Multiselect = false
        };

        if (dialog.ShowDialog(this) != true) return;
        ExecutablePathTextBox.Text = dialog.FileName;
        SaveSettings(new LauncherSettings(dialog.FileName));
        await RefreshContentAsync();
    }

    private async void RefreshContent_Click(object sender, RoutedEventArgs e) => await RefreshContentAsync();

    private async void LaunchMode_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (!IsLoaded || activeSession is not null || refreshingContent) return;
        await RefreshContentAsync();
    }

    private async void Content_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (!IsLoaded || refreshingContent || LaunchModeComboBox.SelectedIndex != 0) return;
        await LoadEntrypointsAsync();
    }

    private async Task RefreshContentAsync()
    {
        if (refreshingContent || activeSession is not null) return;

        try
        {
            refreshingContent = true;
            SetStatus("Lendo instalação...");
            var root = ResolveInstallationRoot();
            var installation = new InstallationSpec(root);

            var plugin = Path.Combine(root, "plugins", "OmsiLaunch.Plugin.opl");
            if (!File.Exists(plugin))
                AppendLog("Aviso: o plugin OmsiLaunch.Plugin.opl não foi encontrado em plugins\\. A validação da sessão poderá falhar.");

            if (LaunchModeComboBox.SelectedIndex == 1)
            {
                ContentLabel.Text = "Situação salva";
                EntrypointLabel.Visibility = Visibility.Collapsed;
                EntrypointComboBox.Visibility = Visibility.Collapsed;

                var situations = await launch.DiscoverAsync(installation, ContentQueryKind.Situations);
                ContentComboBox.ItemsSource = situations
                    .Select(x => new Choice(x.DisplayName ?? x.Identity, x.Identity))
                    .ToArray();
                ContentComboBox.SelectedIndex = situations.Count > 0 ? 0 : -1;
                AppendLog($"{situations.Count} situação(ões) salva(s) encontrada(s).");
            }
            else
            {
                ContentLabel.Text = "Mapa";
                EntrypointLabel.Visibility = Visibility.Visible;
                EntrypointComboBox.Visibility = Visibility.Visible;

                var maps = await launch.DiscoverAsync(installation, ContentQueryKind.Maps);
                ContentComboBox.ItemsSource = maps
                    .Select(x => new Choice(x.DisplayName ?? x.Identity, x.Identity))
                    .ToArray();
                ContentComboBox.SelectedIndex = maps.Count > 0 ? 0 : -1;
                AppendLog($"{maps.Count} mapa(s) encontrado(s).");

                await LoadEntrypointsAsync();
            }

            await RefreshRecoveryStateAsync(root);
            UpdateActionButtons();
            SetStatus("Pronto");
        }
        catch (Exception exception)
        {
            ContentComboBox.ItemsSource = null;
            EntrypointComboBox.ItemsSource = null;
            SetStatus("Erro de instalação");
            AppendLog(exception.Message);
            UpdateActionButtons();
        }
        finally
        {
            refreshingContent = false;
        }
    }

    private async Task LoadEntrypointsAsync()
    {
        if (LaunchModeComboBox.SelectedIndex != 0) return;
        if (ContentComboBox.SelectedItem is not Choice map)
        {
            EntrypointComboBox.ItemsSource = null;
            UpdateActionButtons();
            return;
        }

        try
        {
            var installation = new InstallationSpec(ResolveInstallationRoot());
            var entries = await launch.DiscoverAsync(
                installation,
                ContentQueryKind.Entrypoints,
                OptionalValue<string>.Set(map.Identity));

            EntrypointComboBox.ItemsSource = entries
                .Select(x => new Choice(x.DisplayName ?? x.Identity, x.Identity))
                .ToArray();
            EntrypointComboBox.SelectedIndex = entries.Count > 0 ? 0 : -1;
            AppendLog($"{entries.Count} ponto(s) de entrada encontrado(s) em {map.Display}.");
        }
        catch (Exception exception)
        {
            EntrypointComboBox.ItemsSource = null;
            AppendLog("Falha ao ler pontos de entrada: " + exception.Message);
        }

        UpdateActionButtons();
    }

    private async Task RefreshRecoveryStateAsync(string root)
    {
        try
        {
            var transaction = new FileConfigurationTransaction(root, new Dictionary<string, byte[]>());
            recoveryPending = await transaction.HasPendingRecoveryAsync();
            RecoveryButton.IsEnabled = recoveryPending && activeSession is null;
            if (recoveryPending) AppendLog("Há uma restauração pendente de uma sessão anterior.");
        }
        catch (Exception exception)
        {
            recoveryPending = false;
            RecoveryButton.IsEnabled = false;
            AppendLog("Não foi possível verificar a recuperação pendente: " + exception.Message);
        }
    }

    private async void InstallPlugin_Click(object sender, RoutedEventArgs e)
    {
        if (activeSession is not null) return;

        try
        {
            var root = ResolveInstallationRoot();
            var sourceDirectory = Path.Combine(AppContext.BaseDirectory, "plugins");
            var nativeSource = Path.Combine(sourceDirectory, "OmsiLaunch.Native.x86.dll");
            var artifacts = RuntimeArtifactSet.Load(sourceDirectory, nativeSource);

            var backupRoot = Path.Combine(
                root,
                ".omsilaunch",
                "plugin-backup",
                DateTime.Now.ToString("yyyyMMdd-HHmmss"));

            var copied = 0;
            var skipped = 0;
            foreach (var artifact in artifacts.Artifacts)
            {
                var source = Path.GetFullPath(artifact.SourcePath);
                var destination = Path.GetFullPath(Path.Combine(root, artifact.DestinationRelativePath));
                Directory.CreateDirectory(Path.GetDirectoryName(destination)!);

                if (source.Equals(destination, StringComparison.OrdinalIgnoreCase))
                {
                    skipped++;
                    continue;
                }

                if (File.Exists(destination))
                {
                    var relative = Path.GetRelativePath(root, destination);
                    var backup = Path.Combine(backupRoot, relative);
                    Directory.CreateDirectory(Path.GetDirectoryName(backup)!);
                    File.Copy(destination, backup, true);
                }

                var temporary = destination + ".omsilaunch-update.tmp";
                File.Copy(source, temporary, true);
                File.Move(temporary, destination, true);
                copied++;
            }

            artifacts.ValidateInstalled(root);
            AppendLog($"Plugin validado. {copied} arquivo(s) atualizado(s), {skipped} já estava(m) no destino.");
            if (Directory.Exists(backupRoot))
                AppendLog("Backup da versão anterior: " + backupRoot);

            SetStatus("Plugin pronto");
            await RefreshContentAsync();
        }
        catch (Exception exception)
        {
            SetStatus("Falha ao instalar plugin");
            AppendLog("Falha ao instalar/atualizar plugin: " + exception.Message);
            MessageBox.Show(this,
                exception.Message,
                "Falha ao instalar plugin",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void Recovery_Click(object sender, RoutedEventArgs e)
    {
        if (activeSession is not null) return;

        try
        {
            var root = ResolveInstallationRoot();
            var transaction = new FileConfigurationTransaction(root, new Dictionary<string, byte[]>());
            if (!await transaction.HasPendingRecoveryAsync())
            {
                recoveryPending = false;
                RecoveryButton.IsEnabled = false;
                AppendLog("Nenhuma recuperação pendente foi encontrada.");
                return;
            }

            var answer = MessageBox.Show(
                this,
                "Uma sessão anterior deixou uma restauração pendente. Restaurar agora os arquivos originais do OMSI?",
                "Recuperação do OmsiLaunch",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (answer != MessageBoxResult.Yes) return;

            SetStatus("Restaurando arquivos...");
            await transaction.RestorePendingAsync();
            recoveryPending = false;
            RecoveryButton.IsEnabled = false;
            AppendLog("Recuperação concluída. Os arquivos transacionais foram restaurados.");
            SetStatus("Recuperação concluída");
        }
        catch (Exception exception)
        {
            SetStatus("Falha na recuperação");
            AppendLog("Falha na recuperação: " + exception.Message);
            MessageBox.Show(this,
                exception.Message,
                "Falha na recuperação",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void Validate_Click(object sender, RoutedEventArgs e)
    {
        await ValidateCurrentSelectionAsync();
    }

    private async Task<SessionPlan?> ValidateCurrentSelectionAsync()
    {
        try
        {
            SetStatus("Validando...");
            var spec = BuildLaunchSpec();
            var plan = await launch.PlanSessionAsync(spec);

            AppendLog($"Perfil: {plan.BuildProfileId}");
            if (plan.Diagnostics.Count == 0)
                AppendLog(plan.IsRunnable ? "Validação concluída: sessão pronta para iniciar." : "A sessão não está pronta para iniciar.");

            foreach (var diagnostic in plan.Diagnostics)
                AppendDiagnostic(diagnostic);

            SetStatus(plan.IsRunnable ? "Validado" : "Validação falhou");
            return plan;
        }
        catch (Exception exception)
        {
            SetStatus("Validação falhou");
            AppendLog(exception.Message);
            return null;
        }
    }

    private async void Launch_Click(object sender, RoutedEventArgs e)
    {
        if (activeSession is not null) return;

        var plan = await ValidateCurrentSelectionAsync();
        if (plan is null || !plan.IsRunnable)
        {
            MessageBox.Show(this,
                "A sessão não passou na validação. Veja o painel de diagnóstico.",
                "OmsiLaunch",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        try
        {
            SetStatus("Iniciando OMSI...");
            var handle = await launch.StartSessionAsync(plan);
            activeSession = handle;
            monitorCancellation = new CancellationTokenSource();
            SetSessionUi(true);
            AppendLog($"Sessão {handle.SessionId:D} iniciada.");
            _ = MonitorSessionAsync(handle, monitorCancellation.Token);
        }
        catch (Exception exception)
        {
            activeSession = null;
            SetSessionUi(false);
            SetStatus("Falha ao iniciar");
            AppendLog(exception.ToString());
        }
    }

    private async Task MonitorSessionAsync(SessionHandle handle, CancellationToken cancellationToken)
    {
        long lastEventSequence = 0;
        SessionState? lastState = null;

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var status = await launch.GetStatusAsync(handle, cancellationToken);

                if (status.State != lastState)
                {
                    lastState = status.State;
                    SetStatus(DescribeState(status.State));
                    AppendLog("Estado: " + DescribeState(status.State));
                }

                foreach (var runtimeEvent in status.RuntimeEvents ?? Array.Empty<RuntimeEvent>())
                {
                    if (runtimeEvent.Sequence <= lastEventSequence) continue;
                    lastEventSequence = runtimeEvent.Sequence;
                    AppendLog("Evento: " + runtimeEvent.Type);
                }

                if (status.State is SessionState.Completed or SessionState.Failed)
                {
                    foreach (var diagnostic in status.Diagnostics)
                        AppendDiagnostic(diagnostic);

                    await launch.CloseAsync(handle, cancellationToken);
                    if (activeSession?.SessionId == handle.SessionId) activeSession = null;
                    monitorCancellation?.Dispose();
                    monitorCancellation = null;
                    SetSessionUi(false);
                    SetStatus(status.State == SessionState.Completed ? "Sessão encerrada" : "Sessão falhou");
                    return;
                }

                await Task.Delay(400, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception exception)
        {
            AppendLog("Falha ao acompanhar a sessão: " + exception.Message);
            if (activeSession?.SessionId == handle.SessionId)
            {
                try { await launch.CloseAsync(handle, CancellationToken.None); }
                catch (Exception closeException) { AppendLog("Falha no fechamento seguro: " + closeException.Message); }
                activeSession = null;
            }
            SetSessionUi(false);
            SetStatus("Falha de monitoramento");
        }
    }

    private async void Stop_Click(object sender, RoutedEventArgs e)
    {
        await StopActiveSessionAsync();
    }

    private async Task StopActiveSessionAsync()
    {
        var handle = activeSession;
        if (handle is null) return;

        activeSession = null;
        monitorCancellation?.Cancel();
        monitorCancellation?.Dispose();
        monitorCancellation = null;
        StopButton.IsEnabled = false;
        SetStatus("Encerrando sessão...");

        try
        {
            await launch.StopAsync(handle);
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(45));
            var final = await launch.WaitForAsync(handle, SessionState.Completed, TimeSpan.FromSeconds(40), timeout.Token);
            await launch.CloseAsync(handle, timeout.Token);
            AppendLog("Sessão encerrada com estado: " + final.State);
            SetStatus(final.State == SessionState.Completed ? "Sessão encerrada" : "Sessão finalizada com erro");
        }
        catch (Exception exception)
        {
            AppendLog("Falha ao encerrar a sessão: " + exception.Message);
            SetStatus("Falha ao encerrar");
        }
        finally
        {
            SetSessionUi(false);
        }
    }

    private LaunchSpec BuildLaunchSpec()
    {
        var root = ResolveInstallationRoot();
        var empty = new Dictionary<string, OptionalValue<string>>(StringComparer.OrdinalIgnoreCase);

        WorldSpec world;
        if (LaunchModeComboBox.SelectedIndex == 1)
        {
            if (ContentComboBox.SelectedItem is not Choice situation)
                throw new InvalidOperationException("Selecione uma situação salva.");

            world = new WorldSpec(
                WorldMode.SavedSituation,
                OptionalValue<string>.Unset,
                OptionalValue<string>.Set(situation.Identity),
                OptionalValue<int>.Unset,
                OptionalValue<string>.Unset);
        }
        else
        {
            if (ContentComboBox.SelectedItem is not Choice map)
                throw new InvalidOperationException("Selecione um mapa.");
            if (EntrypointComboBox.SelectedIndex < 0)
                throw new InvalidOperationException("Selecione um ponto de entrada.");

            world = new WorldSpec(
                WorldMode.NewMap,
                OptionalValue<string>.Set(map.Identity),
                OptionalValue<string>.Unset,
                OptionalValue<int>.Set(EntrypointComboBox.SelectedIndex),
                OptionalValue<string>.Unset);
        }

        return new LaunchSpec(
            new InstallationSpec(root),
            world,
            new DateSpec(DateTimeMode.Unset, OptionalValue<SemanticDate>.Unset),
            new TimeSpec(DateTimeMode.Unset, OptionalValue<SemanticTime>.Unset),
            OptionalValue<PlayerVehicleSpec>.Unset,
            new EnvironmentSpec(empty, empty, empty, empty, empty, empty, empty, empty),
            new LaunchBehaviorSpec(),
            Presentation: new SessionPresentationSpec(
                ManagedSplashCheckBox.IsChecked == true ? SplashMode.Managed : SplashMode.Unset,
                OptionalValue<string>.Unset,
                OptionalValue<string>.Unset),
            InternetTextures: new InternetTexturesSpec(
                DisableInternetTexturesCheckBox.IsChecked == true ? InternetTexturesMode.Disabled : InternetTexturesMode.Native,
                OptionalValue<string>.Unset));
    }

    private string ResolveInstallationRoot()
    {
        var raw = ExecutablePathTextBox.Text.Trim().Trim('"');
        if (string.IsNullOrWhiteSpace(raw))
            throw new InvalidOperationException("Selecione o arquivo Omsi.exe.");

        var executable = Path.GetFullPath(Environment.ExpandEnvironmentVariables(raw));
        if (!File.Exists(executable))
            throw new FileNotFoundException("Omsi.exe não encontrado.", executable);
        if (!Path.GetFileName(executable).Equals("Omsi.exe", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("O arquivo selecionado precisa ser Omsi.exe.");

        return Path.GetDirectoryName(executable)
            ?? throw new InvalidOperationException("Não foi possível determinar a pasta do OMSI.");
    }

    private static LauncherSettings? LoadSettings()
    {
        try
        {
            var path = SettingsPath();
            if (!File.Exists(path)) return null;
            return JsonSerializer.Deserialize<LauncherSettings>(File.ReadAllText(path));
        }
        catch
        {
            return null;
        }
    }

    private static void SaveSettings(LauncherSettings settings)
    {
        try
        {
            var path = SettingsPath();
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch
        {
            // Preferences are optional; a read-only profile must not block launch.
        }
    }

    private static string SettingsPath() =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OmsiLaunch", "launcher-settings.json");

    private void SetSessionUi(bool running)
    {
        ExecutablePathTextBox.IsEnabled = !running;
        BrowseExecutableButton.IsEnabled = !running;
        RefreshContentButton.IsEnabled = !running;
        LaunchModeComboBox.IsEnabled = !running;
        ContentComboBox.IsEnabled = !running;
        EntrypointComboBox.IsEnabled = !running;
        ManagedSplashCheckBox.IsEnabled = !running;
        DisableInternetTexturesCheckBox.IsEnabled = !running;
        ValidateButton.IsEnabled = !running;
        LaunchButton.IsEnabled = !running;
        InstallPluginButton.IsEnabled = !running;
        RecoveryButton.IsEnabled = !running && recoveryPending;
        StopButton.IsEnabled = running;
    }

    private void UpdateActionButtons()
    {
        if (activeSession is not null)
        {
            ValidateButton.IsEnabled = false;
            LaunchButton.IsEnabled = false;
            InstallPluginButton.IsEnabled = false;
            RecoveryButton.IsEnabled = false;
            StopButton.IsEnabled = true;
            return;
        }

        var hasContent = ContentComboBox.SelectedItem is Choice;
        var hasEntrypoint = LaunchModeComboBox.SelectedIndex == 1 || EntrypointComboBox.SelectedIndex >= 0;
        ValidateButton.IsEnabled = hasContent && hasEntrypoint;
        LaunchButton.IsEnabled = hasContent && hasEntrypoint;
        InstallPluginButton.IsEnabled = true;
        RecoveryButton.IsEnabled = recoveryPending;
        StopButton.IsEnabled = false;
    }

    private void AppendDiagnostic(LaunchDiagnostic diagnostic)
    {
        AppendLog($"{diagnostic.Code}: {diagnostic.Message}");
        if (diagnostic.Data is null || diagnostic.Data.Count == 0) return;
        foreach (var pair in diagnostic.Data)
            AppendLog($"  {pair.Key}: {pair.Value}");
    }

    private void SetStatus(string text) => StatusTextBlock.Text = text;

    private void AppendLog(string text)
    {
        LogTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {text}{Environment.NewLine}");
        LogTextBox.ScrollToEnd();
    }

    private static string DescribeState(SessionState state) => state switch
    {
        SessionState.Created => "Criada",
        SessionState.AcquiringInstallationLock => "Bloqueando instalação",
        SessionState.RecoveringPreviousTransaction => "Verificando recuperação",
        SessionState.Snapshotting => "Criando backup",
        SessionState.ApplyingConfiguration => "Aplicando configuração",
        SessionState.DeployingRuntime => "Preparando runtime",
        SessionState.CreatingStartupHandoff => "Preparando inicialização",
        SessionState.StartingProcess => "Abrindo OMSI",
        SessionState.WaitingForPlugin => "Aguardando plugin",
        SessionState.PluginBootstrap => "Plugin carregado",
        SessionState.StartingWorld => "Carregando mapa",
        SessionState.EnteringGameplay => "Entrando no jogo",
        SessionState.Running => "OMSI em execução",
        SessionState.ProcessExited => "OMSI fechado",
        SessionState.Restoring => "Restaurando arquivos",
        SessionState.CleaningRuntime => "Finalizando",
        SessionState.Completed => "Concluída",
        SessionState.Failed => "Falhou",
        _ => state.ToString()
    };

    private async void Window_Closing(object? sender, CancelEventArgs e)
    {
        if (allowWindowClose || activeSession is null) return;

        e.Cancel = true;
        var answer = MessageBox.Show(
            this,
            "Existe uma sessão do OMSI ativa. Fechar o OmsiLaunch também encerrará a sessão de forma segura. Deseja continuar?",
            "OmsiLaunch",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (answer != MessageBoxResult.Yes) return;

        await StopActiveSessionAsync();
        allowWindowClose = true;
        Close();
    }
}
