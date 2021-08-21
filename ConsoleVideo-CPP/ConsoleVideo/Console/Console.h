#ifndef CONSOLE_H
    #define CONSOLE_H

    //https://stackoverflow.com/a/47359526/
    #define _WIN32_WINNT 0x0500
    #include <windows.h>
    
    class Console {
        LONG_PTR originalGWLStyle;
        HWND consoleWindow;
    
    public:
        Console(void);
        
        bool SetupConsoleRestrictions(void) const;
    
        bool RevertConsoleRestrictions(void) const;
    };
#endif