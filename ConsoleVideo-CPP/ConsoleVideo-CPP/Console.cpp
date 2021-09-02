#include <Console.hpp>
#include <iostream>
#include <Windows.h>
#include <string>

Console::Console(const std::wstring& title) {
    consoleHandle = GetStdHandle(STD_OUTPUT_HANDLE);
    if (consoleHandle == nullptr) {
        std::cerr << "Failed to get console object handle." << std::endl;
    }
    
    consoleWindow = GetConsoleWindow();
    if (consoleWindow == nullptr) {
        std::cerr << "Failed to get console window handle." << std::endl;
    }
    
    originalGWLStyle = GetWindowLongPtr(consoleWindow, GWL_STYLE);
    
    if (GetConsoleCursorInfo(consoleHandle, &originalConsoleCursorInfo) == false) {
        std::cerr << "Failed to get console cursor information." << std::endl;
    }

    SetConsoleTitle(title.c_str());
    return;
}

void Console::SetupConsoleRestrictions(void) const {
    if (SetWindowLongPtr(consoleWindow,
                         GWL_STYLE,
                         (originalGWLStyle &
                             ~WS_MAXIMIZEBOX &
                             ~WS_SIZEBOX)) == 0) {
        std::cerr << "Failed to set GWL_STYLE." << std::endl;
        std::cout << "Please do not resize or maximize the console window." << std::endl;
    }

    CONSOLE_CURSOR_INFO newConsoleCursorInfo = originalConsoleCursorInfo;
    newConsoleCursorInfo.bVisible = false;
    if (SetConsoleCursorInfo(consoleHandle, &newConsoleCursorInfo) == false) {
        std::cerr << "Failed to disable console cursor." << std::endl;
    }
    return;
}

void Console::RevertConsoleRestrictions(void) const {
    bool success = true;

    if (SetWindowLongPtr(consoleWindow, GWL_STYLE, originalGWLStyle) == 0) {
        std::cerr << "Failed to revert GWL_STYLE." << std::endl;
        success = false;
    }

    if (SetConsoleCursorInfo(consoleHandle, &originalConsoleCursorInfo) == false) {
        std::cerr << "Failed to revert console cursor." << std::endl;
        success = false;
    }

    if (success == false) {
        std::cout << "Failed to revert console settings." << std::endl <<
                     "You might want to reset your console." << std::endl;
    }
    return;
}