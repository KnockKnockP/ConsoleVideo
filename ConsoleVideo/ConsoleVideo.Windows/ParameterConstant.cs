namespace ConsoleVideo.Windows; 

public struct ParameterConstant {
    #region MF
    /// <summary>
    ///     Indicates that uPosition gives the identifier of the menu item.<br />
    ///     The <see cref="MF_BYCOMMAND" /> flag is the default flag if neither the <see cref="MF_BYCOMMAND" /> nor <see cref="MF_BYPOSITION" /> flag is specified.
    /// </summary>
    public const long MF_BYCOMMAND = 0x00000000L;

    /// <summary>
    ///     Indicates that "uPosition" gives the zero-based relative position of the menu item.
    /// </summary>
    public const long MF_BYPOSITION = 0x00000400L;
    #endregion

    #region SC
    /// <summary>
    ///     Closes the window.
    /// </summary>
    public const int SC_CLOSE = 0xF060;

    /// <summary>
    ///     Changes the cursor to a question mark with a pointer.<br />
    ///     If the user then clicks a control in the dialog box, the control receives a "WM_HELP" message.
    /// </summary>
    public const int SC_CONTEXTHELP = 0xF180;

    /// <summary>
    ///     Selects the default item; the user double-clicked the window menu.
    /// </summary>
    public const int SC_DEFAULT = 0xF160;

    /// <summary>
    ///     Activates the window associated with the application-specified hot key.<br />
    ///     The "lParam" parameter identifies the window to activate.
    /// </summary>
    public const int SC_HOTKEY = 0xF150;

    /// <summary>
    ///     Scrolls horizontally.
    /// </summary>
    public const int SC_HSCROLL = 0xF080;

    /// <summary>
    ///     Indicates whether the screen saver is secure.
    /// </summary>
    public const int SCF_ISSECURE = 0x00000001;

    /// <summary>
    ///     Retrieves the window menu as a result of a keystroke.<br />
    ///     For more information, see the
    ///     <a href="https://docs.microsoft.com/en-us/windows/win32/menurc/wm-syscommand#remarks">"Remarks"</a> section.
    /// </summary>
    public const int SC_KEYMENU = 0xF100;

    /// <summary>
    ///     Maximizes the window.
    /// </summary>
    public const int SC_MAXIMIZE = 0xF030;

    /// <summary>
    ///     Minimizes the window.
    /// </summary>
    public const int SC_MINIMIZE = 0xF020;

    /// <summary>
    ///     Sets the state of the display.<br />
    ///     This command supports devices that have power-saving features, such as a battery-powered personal computer.<br />
    ///     The "lParam" parameter can have the following values :<br />
    ///     -1 : The display is powering on,<br />
    ///     1 : The display is going to low power,<br />
    ///     2 : The display is being shut off.
    /// </summary>
    public const int SC_MONITORPOWER = 0xF170;

    /// <summary>
    ///     Retrieves the window menu as a result of a mouse click.
    /// </summary>
    public const int SC_MOUSEMENU = 0xF090;

    /// <summary>
    ///     Moves the window.
    /// </summary>
    public const int SC_MOVE = 0xF010;

    /// <summary>
    ///     Moves to the next window.
    /// </summary>
    public const int SC_NEXTWINDOW = 0xF040;

    /// <summary>
    ///     Moves to the previous window.
    /// </summary>
    public const int SC_PREVWINDOW = 0xF050;

    /// <summary>
    ///     Restores the window to its normal position and size.
    /// </summary>
    public const int SC_RESTORE = 0xF120;

    /// <summary>
    ///     Executes the screen saver application specified in the "[boot]" section of the "System.ini" file.
    /// </summary>
    public const int SC_SCREENSAVE = 0xF140;

    /// <summary>
    ///     Sizes the window.
    /// </summary>
    public const int SC_SIZE = 0xF000;

    /// <summary>
    ///     Activates the start menu.
    /// </summary>
    public const int SC_TASKLIST = 0xF130;

    /// <summary>
    ///     Scrolls vertically.
    /// </summary>
    public const int SC_VSCROLL = 0xF070;
    #endregion

    #region STD_HANDLE
    /// <summary>
    ///     The standard input device.<br />
    ///     Initially, this is the console input buffer, "CONIN$".
    /// </summary>
    public const int STD_INPUT_HANDLE = -10;

    /// <summary>
    ///     The standard output device.<br />
    ///     Initially, this is the active console screen buffer, "CONOUT$".
    /// </summary>
    public const int STD_OUTPUT_HANDLE = -11;

    /// <summary>
    ///     The standard error device.<br />
    ///     Initially, this is the active console screen buffer, "CONOUT$".
    /// </summary>
    public const int STD_ERROR_HANDLE = -12;
    #endregion
}