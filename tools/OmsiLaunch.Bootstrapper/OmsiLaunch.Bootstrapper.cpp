#include <windows.h>
#include <nethost.h>
#include <hostfxr.h>

#include <string>
#include <vector>

using hostfxr_main_fn = int(__cdecl*)(int argc, const wchar_t** argv);

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
    const auto hostfxrMain = reinterpret_cast<hostfxr_main_fn>(GetProcAddress(hostfxr, "hostfxr_main"));
    if (hostfxrMain == nullptr)
    {
        FreeLibrary(hostfxr);
        return 5;
    }

    std::vector<const wchar_t*> hostArguments;
    hostArguments.reserve(static_cast<size_t>(argc) + 1);
    hostArguments.push_back(executablePath);
    hostArguments.push_back(controller.c_str());
    for (auto index = 1; index < argc; ++index)
    {
        hostArguments.push_back(argv[index]);
    }

    const auto exitCode = hostfxrMain(static_cast<int>(hostArguments.size()), hostArguments.data());
    FreeLibrary(hostfxr);
    return exitCode;
}
