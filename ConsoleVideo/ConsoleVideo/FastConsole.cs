using System;
using System.IO;
using System.Text;

namespace ConsoleVideo {
    //https://stackoverflow.com/a/57837393/11848701/
    internal static class FastConsole {
        private static readonly BufferedStream _BufferedStream;

        static FastConsole() {
            //This is crucial.
            Console.OutputEncoding = Encoding.Unicode;

            //Avoid special "ShadowBuffer" for hard-coded size 0x14000 in "BufferedStream".
            _BufferedStream = new BufferedStream(Console.OpenStandardOutput(), 0x15000);
            return;
        }

        internal static void Write(string stringToPrint) {
            //Avoid endless "GetByteCount" dithering in "Encoding.Unicode.GetBytes(s)".
            int length = stringToPrint.Length;
            byte[] rgb = new byte[(length << 1)];
            Encoding.Unicode.GetBytes(stringToPrint,
                                      0,
                                      length,
                                      rgb,
                                      0);

            //This lock is optional.
            lock (_BufferedStream) {
                _BufferedStream.Write(rgb,
                                      0,
                                      rgb.Length);
            }
            return;
        }

        internal static void Flush() {
            lock (_BufferedStream) {
                _BufferedStream.Flush();
            }
            return;
        }
    }
}