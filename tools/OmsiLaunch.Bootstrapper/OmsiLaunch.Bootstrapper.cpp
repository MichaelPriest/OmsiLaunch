#include <windows.h>
#include <nethost.h>
#include <hostfxr.h>

#include <string>
#include <vector>

int wmain(int argc, wchar_t** argv)
{
    wchar_t executablePath[MAX_PATH] = {};
    const auto executableLength = GetModuleFileNameW(nullptr, executablePath, MAX_PATH);
    if (executableLength == 0 || executableLength == MAX_PATH)
    {
        return 1;
    }

    std::wstring directory(executablePath, executableLength);
    directory.erase(directory.find_last_of(L"\\/") + 1);
    const auto controller = directory + L"OmsiLaunch.Controller.dll";

    size_t hostfxrPathSize = 0;
    // nethost documents the first null-buffer call as the size probe. The
    // numeric error constant is intentionally not exported by older headers.
    if (get_hostfxr_path(nullptr, &hostfxrPathSize, nullptr) == 0 || hostfxrPathSize == 0)
    {
        return 2;
    }
    std::vector<wchar_t> hostfxrPath(hostfxrPathSize);
    if (get_hostfxr_path(hostfxrPath.data(), &hostfxrPathSize, nullptr) != 0)
    {
        return 3;
    }

    const auto hostfxr = LoadLibraryW(hostfxrPath.data());
    if (hostfxr == nullptr)
    {
        return 4;
    }
    const auto initializeForCommandLine = reinterpret_cast<hostfxr_initialize_for_dotnet_command_line_fn>(
        GetProcAddress(hostfxr, "hostfxr_initialize_for_dotnet_command_line"));
    const auto runApp = reinterpret_cast<hostfxr_run_app_fn>(GetProcAddress(hostfxr, "hostfxr_run_app"));
    const auto closeHostContext = reinterpret_cast<hostfxr_close_fn>(GetProcAddress(hostfxr, "hostfxr_close"));
    if (initializeForCommandLine == nullptr || runApp == nullptr || closeHostContext == nullptr)
    {
        FreeLibrary(hostfxr);
        return 5;
    }

    // The command-line initializer takes the managed assembly first and passes
    // only the remaining values to its managed Main method. This keeps the
    // controller path out of the public CLI argument list.
    std::vector<const wchar_t*> hostArguments;
    hostArguments.reserve(static_cast<size_t>(argc));
    hostArguments.push_back(controller.c_str());
    for (auto index = 1; index < argc; ++index)
    {
        hostArguments.push_back(argv[index]);
    }

    hostfxr_initialize_parameters parameters = {};
    parameters.size = sizeof(parameters);
    parameters.host_path = executablePath;
    hostfxr_handle context = nullptr;
    if (initializeForCommandLine(static_cast<int>(hostArguments.size()), hostArguments.data(), &parameters, &context) != 0 || context == nullptr)
    {
        FreeLibrary(hostfxr);
        return 6;
    }

    const auto exitCode = runApp(context);
    closeHostContext(context);
    FreeLibrary(hostfxr);
    return exitCode;
}
