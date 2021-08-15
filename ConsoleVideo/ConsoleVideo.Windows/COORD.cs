#region ReSharper
//ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
#endregion

using System.Runtime.InteropServices;

namespace ConsoleVideo {
    namespace Windows {
        /// <summary>
        ///     Defines the coordinates of a character cell in a console screen buffer.<br />
        ///     The origin of the coordinate system (0,0) is at the top, left cell of the buffer.<br />
        ///     <a href="https://docs.microsoft.com/en-us/windows/console/coord-str/">Microsoft documentation</a>.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public readonly struct COORD {
            /// <summary>
            ///     The horizontal coordinate or column value.<br />
            ///     The units depend on the function call.
            /// </summary>
            private readonly short X;

            /// <summary>
            ///     The vertical coordinate or row value.<br />
            ///     The units depend on the function call.
            /// </summary>
            private readonly short Y;

            public COORD(short x, short y) {
                X = x;
                Y = y;
            }
        }
    }
}