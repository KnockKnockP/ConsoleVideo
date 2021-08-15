using System;

namespace ConsoleVideo.IO {
    public static class UserInput {
        public static bool AskUserChoice(char yesChar,
                                         string yesMessage,
                                         string noMessage) {
            Console.Write($"('{char.ToUpper(yesChar)}', '{char.ToLower(yesChar)}'): {yesMessage}\r\n" +
                          $"(Other): {noMessage}\r\n");
            ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
            return (char.ToLower(consoleKeyInfo.KeyChar) == char.ToLower(yesChar));
        }
    }
}