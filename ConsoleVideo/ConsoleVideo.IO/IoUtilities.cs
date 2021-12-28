using System.IO;
using System.Linq;

namespace ConsoleVideo.IO; 

public static class IoUtilities {
    #region FindDirectory
    public static string FindDirectory(string directoryName) => FindDirectory(directoryName, FilePath.ApplicationDirectory);

    public static string FindDirectory(string directoryName, string currentDirectory) {
        if (currentDirectory == null) {
            return null;
        }

        return Directory.GetDirectories(currentDirectory)
                        .Select(directory => directory.EndsWith(directoryName) ? directory : FindDirectory(directoryName, directory))
                        .FirstOrDefault();
    }
    #endregion

    #region FindAFileWithExtension
    public static string FindAFileWithExtension(string[] extensions) => FindAFileWithExtension(extensions, FilePath.ApplicationDirectory);

    public static string FindAFileWithExtension(string[] extensions, string currentDirectory) {
        if (currentDirectory == null) {
            return null;
        }

        foreach (FileInfo fileInfo in new DirectoryInfo(currentDirectory).GetFiles()) {
            if (extensions.Contains(fileInfo.Extension) == true) {
                return fileInfo.FullName;
            }
        }

        return Directory.GetDirectories(currentDirectory)
                        .Select(directory => FindAFileWithExtension(extensions, directory))
                        .FirstOrDefault();
    }
    #endregion
}