using ConsoleVideo.IO;
using NUnit.Framework;
using System.IO;

namespace ConsoleVideo.Tests {
    internal static class IoUtilitiesTest {
        #region FindDirectory
        [Test]
        public static void TestFindDirectory() {
            const string folderName = "TestTargetFolder";
            string baseDirectory = $"{TestContext.CurrentContext.TestDirectory}{Path.DirectorySeparatorChar}";

            //Target directory name on the same directory.
            string temp = $"{baseDirectory}{folderName}";
            Assert.IsTrue(FindDirectoryTestHelper(folderName,
                                                  temp,
                                                  temp,
                                                  temp));

            //Duplicate directory name on different directory level.
            temp = $"{baseDirectory}{folderName}";
            Assert.IsTrue(FindDirectoryTestHelper(folderName,
                                                  $"{temp}{Path.DirectorySeparatorChar}{folderName}",
                                                  temp,
                                                  temp));

            //Target directory name on a deeper directory level.
            string temp2 = $"{baseDirectory}TestingFolder12345가나다라مرحبا";
            temp = $"{temp2}{Path.DirectorySeparatorChar}{folderName}";
            Assert.IsTrue(FindDirectoryTestHelper(folderName,
                                                  temp,
                                                  temp,
                                                  temp2));

            //100 levels deep directory.
            temp = baseDirectory;
            for (byte i = 0; i < 100; ++i) {
                temp += $"{i}{Path.DirectorySeparatorChar}";
            }
            temp += folderName;
            Assert.IsTrue(FindDirectoryTestHelper(folderName,
                                                  temp,
                                                  temp,
                                                  $"{baseDirectory}{0}"));
            return;
        }

        /// <summary>
        ///     A helper method to simplify <see cref="TestFindDirectory" /> method.
        /// </summary>
        /// <param name="folderName">
        ///     Name of the folder to search for.
        /// </param>
        /// <param name="directoryPath">
        ///     Path to the directory to create for testing.
        /// </param>
        /// <param name="expected">
        ///     Expected directory path.
        /// </param>
        /// <param name="directoryToDelete">
        ///     Path to a directory to delete after testing.
        /// </param>
        /// <returns>
        ///     If the expected and the found directory string is the same, it returns true.<br />
        ///     Otherwise, it returns false.
        /// </returns>
        private static bool FindDirectoryTestHelper(string folderName,
                                                    string directoryPath,
                                                    string expected,
                                                    string directoryToDelete) {
            directoryPath = directoryPath.TrimEnd(Path.DirectorySeparatorChar);
            Directory.CreateDirectory(directoryPath);

            bool isEqual = (expected == FindDirectoryTrim(folderName));

            if (directoryToDelete != null) {
                Directory.Delete(directoryToDelete, true);
            }
            return isEqual;
        }

        /// <summary>
        ///     A helper method to simplify <see cref="FindDirectoryTestHelper" /> method.
        /// </summary>
        /// <param name="folderName">
        ///     Name of the folder to search for.
        /// </param>
        /// <returns>
        ///     It returns a directory path to the folder without the separator at the end.<br />
        ///     If it finds nothing, it returns null.
        /// </returns>
        private static string FindDirectoryTrim(string folderName) {
            return IoUtilities.FindDirectory(folderName)
                              ?.TrimEnd(Path.DirectorySeparatorChar);
        }
        #endregion

        [Test]
        public static void TestFindAFileWithExtension() {
            /*
                File with the target extension on the same directory.
                Single extension.
            */
            const string fileName = "TEST_FILE",
                         extension1 = ".TEST_FILE_EXTENSION1",
                         extension2 = ".TEST_FILE_EXTENSION2";
            string baseDirectory = $"{TestContext.CurrentContext.TestDirectory}{Path.DirectorySeparatorChar}",
                   temp = $"{baseDirectory}{fileName}{extension1}";
            string[] extensions = {
                extension1
            };
            File.Create(temp)
                .Close();

            Assert.AreEqual(temp, IoUtilities.FindAFileWithExtension(extensions));

            File.Delete(temp);

            //2 extensions.
            temp = $"{baseDirectory}{fileName}{extension2}";
            extensions = new[] {
                extension1,
                extension2
            };
            File.Create(temp)
                .Close();

            Assert.AreEqual(temp, IoUtilities.FindAFileWithExtension(extensions));

            File.Delete(temp);

            /*
                Duplicate file name on different directory level.
                Single extension.
            */
            temp = $"{baseDirectory}{fileName}{extension1}";
            extensions = new[] {
                extension1
            };
            File.Create(temp)
                .Close();
            Directory.CreateDirectory($"{baseDirectory}TEST_DIRECTORY");
            File.Create($"{baseDirectory}TEST_DIRECTORY{Path.DirectorySeparatorChar}{fileName}{extension1}")
                .Close();

            Assert.AreEqual(temp, IoUtilities.FindAFileWithExtension(extensions));

            File.Delete(temp);
            Directory.Delete($"{baseDirectory}TEST_DIRECTORY", true);

            //2 extensions.
            temp = $"{baseDirectory}{fileName}{extension2}";
            extensions = new[] {
                extension1,
                extension2
            };
            File.Create(temp)
                .Close();
            Directory.CreateDirectory($"{baseDirectory}TEST_DIRECTORY_2");
            File.Create($"{baseDirectory}TEST_DIRECTORY_2{Path.DirectorySeparatorChar}{fileName}{extension2}")
                .Close();

            Assert.AreEqual(temp, IoUtilities.FindAFileWithExtension(extensions));

            File.Delete(temp);
            Directory.Delete($"{baseDirectory}TEST_DIRECTORY_2", true);

            /*
                Target file on a deeper directory level.
                Single extension.
            */
            temp = $"{baseDirectory}TEST_FOLDER_1{Path.DirectorySeparatorChar}TEST_FOLDER_2";
            string temp2 = $"{temp}{Path.DirectorySeparatorChar}{fileName}{extension1}";
            extensions = new[] {
                extension1
            };
            Directory.CreateDirectory(temp);
            File.Create(temp2)
                .Close();

            Assert.AreEqual(temp2, IoUtilities.FindAFileWithExtension(extensions));

            Directory.Delete($"{baseDirectory}TEST_FOLDER_1", true);

            //2 extensions.
            temp = $"{baseDirectory}TEST_FOLDER_200{Path.DirectorySeparatorChar}TEST_FOLDER_999";
            temp2 = $"{temp}{Path.DirectorySeparatorChar}{fileName}{extension2}";
            extensions = new[] {
                extension1,
                extension2
            };
            Directory.CreateDirectory(temp);
            File.Create(temp2)
                .Close();

            Assert.AreEqual(temp2, IoUtilities.FindAFileWithExtension(extensions));

            Directory.Delete($"{baseDirectory}TEST_FOLDER_200", true);

            /*
                Target file in a 100 levels deep directory.
                Single extension.
            */
            temp = baseDirectory;
            for (byte i = 0; i < 100; ++i) {
                temp += $"{i}{Path.DirectorySeparatorChar}";
            }
            temp2 = $"{temp}{fileName}{extension1}";
            extensions = new[] {
                extension1
            };
            Directory.CreateDirectory(temp);
            File.Create(temp2)
                .Close();

            Assert.AreEqual(temp2, IoUtilities.FindAFileWithExtension(extensions));

            Directory.Delete($"{baseDirectory}0", true);

            //2 extensions.
            temp2 = $"{temp}{fileName}{extension2}";
            extensions = new[] {
                extension1,
                extension2
            };
            Directory.CreateDirectory(temp);
            File.Create(temp2)
                .Close();

            Assert.AreEqual(temp2, IoUtilities.FindAFileWithExtension(extensions));

            Directory.Delete($"{baseDirectory}0", true);
            return;
        }
    }
}