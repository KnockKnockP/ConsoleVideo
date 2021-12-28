using System;
using System.Runtime.InteropServices;

namespace ConsoleVideo.Windows; 

public struct WindowsApi {
    #region Constants.
    public const int LF_FACESIZE = 32;

    public static readonly IntPtr INVALID_HANDLE_VALUE = new(-1);
    #endregion

    #region kernel32.dll
    /// <summary>
    ///     Retrieves the window handle used by the console associated with the calling process.<br />
    ///     <a href="https://docs.microsoft.com/en-us/windows/console/getconsolewindow/">Microsoft documentation</a>.
    /// </summary>
    /// <returns>
    ///     The return value is a handle to the window used by the console associated with the calling process or NULL if there
    ///     is no such associated console.
    /// </returns>
    [DllImport("kernel32.dll")]
    public static extern IntPtr GetConsoleWindow();

    /// <summary>
    ///     Retrieves a handle to the specified standard device.<br />
    ///     (Standard input, standard output, or standard error.).<br />
    ///     <a href="https://docs.microsoft.com/en-us/windows/console/getstdhandle/">Microsoft documentation</a>.
    /// </summary>
    /// <param name="nStdHandle">
    ///     The standard device.<br />
    ///     This parameter can be one of the following values :<br />
    ///     <see cref="ParameterConstant.STD_INPUT_HANDLE" />,<br />
    ///     <see cref="ParameterConstant.STD_OUTPUT_HANDLE" />,<br />
    ///     <see cref="ParameterConstant.STD_ERROR_HANDLE" />.
    /// </param>
    /// <returns>
    ///     If the function succeeds, the return value is a handle to the specified device, or a redirected handle set by a
    ///     previous call to <a href="https://docs.microsoft.com/en-us/windows/console/setstdhandle/">"SetStdHandle"</a>.
    ///     <br />
    ///     The handle has "GENERIC_READ" and "GENERIC_WRITE" access rights, unless the application has used
    ///     <a href="https://docs.microsoft.com/en-us/windows/console/setstdhandle/">"SetStdHandle"</a> to set a standard
    ///     handle with lesser access.<br />
    ///     If the function fails, the return value is <see cref="INVALID_HANDLE_VALUE" />.<br />
    ///     To get extended error information, call
    ///     <a href="https://docs.microsoft.com/en-us/windows/win32/api/errhandlingapi/nf-errhandlingapi-getlasterror/">"GetLastError"</a>
    ///     .<br />
    ///     If an application does not have associated standard handles, such as a service running on an interactive desktop,
    ///     and has not redirected them, the return value is NULL.
    /// </returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetStdHandle(int nStdHandle);

    /// <summary>
    ///     Sets extended information about the current console font.
    ///     <a href="https://docs.microsoft.com/en-us/windows/console/setcurrentconsolefontex/">Microsoft documentation</a>.
    /// </summary>
    /// <param name="hConsoleOutput">
    ///     A handle to the console screen buffer.<br />
    ///     The handle must have the "GENERIC_WRITE" access right.<br />
    ///     For more information, see
    ///     <a href="https://docs.microsoft.com/en-us/windows/console/console-buffer-security-and-access-rights/">
    ///         Console
    ///         Buffer Security and Access Rights
    ///     </a>
    ///     .
    /// </param>
    /// <param name="bMaximumWindow">
    ///     If this parameter is TRUE, font information is set for the maximum window size.<br />
    ///     If this parameter is FALSE, font information is set for the current window size.
    /// </param>
    /// <param name="lpConsoleCurrentFontEx">
    ///     A pointer to a <see cref="CONSOLE_FONT_INFOEX" /> structure that contains the font information.
    /// </param>
    /// <returns>
    ///     If the function succeeds, the return value is nonzero.<br />
    ///     If the function fails, the return value is zero.<br />
    ///     To get extended error information, call
    ///     <a href="https://docs.microsoft.com/en-us/windows/win32/api/errhandlingapi/nf-errhandlingapi-getlasterror/">"GetLastError"</a>
    ///     .
    /// </returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetCurrentConsoleFontEx(IntPtr hConsoleOutput,
                                                      bool bMaximumWindow,
                                                      ref CONSOLE_FONT_INFOEX lpConsoleCurrentFontEx);
    #endregion

    #region user32.dll
    /// <summary>
    ///     Deletes an item from the specified menu.<br />
    ///     If the menu item opens a menu or submenu, this function destroys the handle to the menu or submenu and frees the
    ///     memory used by the menu or submenu.
    ///     <a href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-deletemenu/">Microsoft documentation</a>
    ///     .
    /// </summary>
    /// <param name="hMenu">
    ///     A handle to the menu to be changed.
    /// </param>
    /// <param name="uPosition">
    ///     The menu item to be deleted, as determined by the <paramref name="uFlags" /> parameter.
    /// </param>
    /// <param name="uFlags">
    ///     Indicates how the <paramref name="uPosition" /> parameter is interpreted.<br />
    ///     This parameter must be one of the following values.<br />
    ///     <see cref="ParameterConstant.MF_BYCOMMAND" />,<br />
    ///     <see cref="ParameterConstant.MF_BYPOSITION" />.
    /// </param>
    /// <returns>
    ///     If the function succeeds, the return value is nonzero.<br />
    ///     If the function fails, the return value is zero.<br />
    ///     To get extended error information, call
    ///     <a href="https://docs.microsoft.com/en-us/windows/win32/api/errhandlingapi/nf-errhandlingapi-getlasterror/">"GetLastError"</a>
    ///     .
    /// </returns>
    [DllImport("user32.dll")]
    public static extern bool DeleteMenu(IntPtr hMenu,
                                         int uPosition,
                                         int uFlags);

    /// <summary>
    ///     Enables the application to access the window menu (Also known as the system menu or the control menu.) for copying
    ///     and modifying.<br />
    ///     <a href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getsystemmenu/">
    ///         Microsoft
    ///         documentation
    ///     </a>
    ///     .
    /// </summary>
    /// <param name="hWnd">
    ///     A handle to the window that will own a copy of the window menu.
    /// </param>
    /// <param name="bRevert">
    ///     The action to be taken.<br />
    ///     If this parameter is FALSE, <see cref="GetSystemMenu" /> returns a handle to the copy of the window menu currently
    ///     in use.<br />
    ///     The copy is initially identical to the window menu, but it can be modified.<br />
    ///     If this parameter is TRUE, <see cref="GetSystemMenu" /> resets the window menu back to the default state.<br />
    ///     The previous window menu, if any, is destroyed.
    /// </param>
    /// <returns>
    ///     If the <paramref name="bRevert" /> parameter is FALSE, the return value is a handle to a copy of the window menu.
    ///     <br />
    ///     If the <paramref name="bRevert" /> parameter is TRUE, the return value is NULL.
    /// </returns>
    [DllImport("user32.dll")]
    public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
    #endregion
}