#include "Console.h"

//https://stackoverflow.com/a/47359526/
#define _WIN32_WINNT 0x0500
#include <windows.h>
#include <iostream>

Console::Console(void) {
    consoleWindow = GetConsoleWindow();
    originalGWLStyle = GetWindowLongPtr(consoleWindow, GWL_STYLE);
    return;
}

bool Console::SetupConsoleRestrictions(void) const {
    if (SetWindowLongPtr(consoleWindow,
                         GWL_STYLE,
                         (originalGWLStyle &
                             ~WS_MAXIMIZEBOX &
                             ~WS_SIZEBOX)) == 0) {
        std::cerr << "Failed to set GWL_STYLE." << std::endl;
        return false;
    }
    return true;
}

bool Console::RevertConsoleRestrictions(void) const {
    if (SetWindowLongPtr(consoleWindow, GWL_STYLE, originalGWLStyle) == 0) {
        std::cerr << "Failed to revert GWL_STYLE." << std::endl;
    }
    return true;
}