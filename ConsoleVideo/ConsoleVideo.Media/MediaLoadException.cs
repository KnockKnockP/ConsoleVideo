using System;

namespace ConsoleVideo.Media {
    internal sealed class MediaLoadException : Exception {
        internal MediaLoadException(string message) : base(message) { }
    }
}