using System.IO;
using System.Linq;

namespace ConsoleVideo.IO {
    public static class IoUtilities {
        #region FindDirectory
        public static string FindDirectory(string directoryName) => FindDirectory(directoryName, FilePath.ApplicationDirectory);

        public static string FindDirectory(string directoryName, string currentDirectory) {
            foreach (string directory in Directory.GetDirectories(currentDirectory)) {
                if (directory.EndsWith(directoryName) == true) {
                    return directory;
                }

                string returnPath = FindDirectory(directoryName, directory);
                if (returnPath != null) {
                    return returnPath;
                }
            }
            return null;
        }
        #endregion

        #region FindAFileWithExtension
        public static string FindAFileWithExtension(string[] extensions) => FindAFileWithExtension(extensions, FilePath.ApplicationDirectory);

        public static string FindAFileWithExtension(string[] extensions, string currentDirectory) {
            foreach (FileInfo fileInfo in new DirectoryInfo(currentDirectory).GetFiles()) {
                if (extensions.Contains(fileInfo.Extension) == true) {
                    return fileInfo.FullName;
                }
            }

            foreach (string directory in Directory.GetDirectories(currentDirectory)) {
                string returnPath = FindAFileWithExtension(extensions, directory);
                if (returnPath != null) {
                    return returnPath;
                }
            }
            return null;
        }
        #endregion
    }
}