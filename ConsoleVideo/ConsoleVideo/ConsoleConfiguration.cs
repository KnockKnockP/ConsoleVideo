using ConsoleVideo.Windows;
using System;
using System.Runtime.InteropServices;

namespace ConsoleVideo;

internal static class ConsoleConfiguration {
    internal static unsafe ExitCode SwitchFont(IntPtr handle,
                                               CONSOLE_FONT_INFOEX newFont,
                                               string fontName) {
        IntPtr ptr = new(newFont.FaceName);
        Marshal.Copy(fontName.ToCharArray(),
                     0,
                     ptr,
                     fontName.Length);

        return ((WindowsApi.SetCurrentConsoleFontEx(handle,
                                                    false,
                                                    ref newFont) ==
                 false)
            ? ExitCode.FailedToModifyConsoleWindow
            : ExitCode.Success);
    }

    internal static ExitCode DisableResize(IntPtr consoleHandle) {
        if (WindowsApi.DeleteMenu(WindowsApi.GetSystemMenu(consoleHandle, false),
                                  ParameterConstant.SC_MAXIMIZE,
                                  (int)(ParameterConstant.MF_BYCOMMAND)) ==
            false) {
            return ExitCode.FailedToModifyConsoleWindow;
        }

        return ((WindowsApi.DeleteMenu(WindowsApi.GetSystemMenu(consoleHandle, false),
                                       ParameterConstant.SC_SIZE,
                                       (int)(ParameterConstant.MF_BYCOMMAND)) ==
                 false)
            ? ExitCode.FailedToModifyConsoleWindow
            : ExitCode.Success);
    }
}