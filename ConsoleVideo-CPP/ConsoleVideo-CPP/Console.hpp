#ifndef CONSOLE_HPP
    #define CONSOLE_HPP

    #include <string>
    #include <Windows.h>
    
    class Console {
        LONG_PTR originalGWLStyle;
        CONSOLE_CURSOR_INFO originalConsoleCursorInfo;

    public:
        HANDLE consoleHandle;
        HWND consoleWindow;

        explicit Console(const std::wstring& title);
        
        void SetupConsoleRestrictions(void) const;
    
        void RevertConsoleRestrictions(void) const;
    };
#endif