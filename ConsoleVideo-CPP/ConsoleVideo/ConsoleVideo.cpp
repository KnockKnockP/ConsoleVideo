#include "ExitCode.hpp"
#include "Console/Console.h"
#include <iostream>

int main(void) {
    ExitCode exitCode = Success;
    
    try {
        const Console console;

        if (console.SetupConsoleRestrictions() == false) {
            std::cout << "Please do not resize or maximize the console window." << std::endl;
        }

        if (console.RevertConsoleRestrictions() == false) {
            std::cerr << "You might want to reset your console." << std::endl;
        }
    } catch (std::string exceptionMessage) {
        std::cerr << "Exception: " << exceptionMessage << std::endl;
        exitCode = UnhandledException;
    }
    
    std::cin.get();
    return exitCode;
}