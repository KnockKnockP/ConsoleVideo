using System.Runtime.InteropServices;

namespace ConsoleVideo.Windows; 

/// <summary>
///     Contains extended information for a console font.<br />
///     <a href="https://docs.microsoft.com/en-us/windows/console/console-font-infoex/">Microsoft documentation</a>.
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public unsafe struct CONSOLE_FONT_INFOEX {
    /// <summary>
    ///     The size of this structure, in bytes.<br />
    ///     This member must be set to "sizeof(CONSOLE_FONT_INFOEX)" before calling <a href="https://docs.microsoft.com/en-us/windows/console/getcurrentconsolefontex/">"GetCurrentConsoleFontEx"</a> or it will fail.
    /// </summary>
    public uint cbSize;

    /// <summary>
    ///     The index of the font in the system's console font table.
    /// </summary>
    private readonly uint nFont;

    /// <summary>
    ///     A <see cref="COORD" /> structure that contains the width and height of each character in the font, in logical units.<br />
    ///     The <see cref="COORD.X" /> member contains the width, while the <see cref="COORD.Y" /> member contains the height.
    /// </summary>
    public COORD dwFontSize;

    /// <summary>
    ///     The font pitch and family.<br />
    ///     For information about the possible values for this member, see the description of the <a href="https://docs.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-textmetrica#members">"tmPitchAndFamily"</a> member of the <a href="https://docs.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-textmetrica/">"TEXTMETRIC"</a> structure.
    /// </summary>
    public int FontFamily;

    /// <summary>
    ///     The font weight.<br />
    ///     The weight can range from 100 to 1000, in multiples of 100.<br />
    ///     For example, the normal weight is 400, while 700 is bold.
    /// </summary>
    public int FontWeight;

    /// <summary>
    ///     The name of the typeface. (Such as Courier or Arial.).
    /// </summary>
    public fixed char FaceName[WindowsApi.LF_FACESIZE];
}