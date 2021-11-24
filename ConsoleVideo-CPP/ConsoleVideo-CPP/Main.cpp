#include <Console.hpp>
#include <ExitCode.hpp>
#include <iostream>
#include <Video.hpp>

int main(int argc, char *argv[]) {
    ExitCode exitCode = Success;

    const Console console(L"ConsoleVideo");
    console.SetupConsoleRestrictions();
    
    try {
        Video video("D:\\GitHub\\ConsoleVideo\\ConsoleVideo\\ConsoleVideo\\Resources\\Videos\\TestVideo.mp4");

        std::cout << "(" << video.width << ", " << video.height << ")" << std::endl;

        video.LoadNextFrame();
    } catch (const std::exception& exception) {
        std::cerr << "Exception: " << exception.what() << std::endl;
        exitCode = Failure;
    }

    console.RevertConsoleRestrictions();

    std::cout << "Execution done." << std::endl <<
                 "Exit code: " << exitCode << std::endl;
    std::cin.get();
    return exitCode;
}