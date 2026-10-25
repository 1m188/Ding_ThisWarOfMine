using System;
using System.Runtime.InteropServices;
using System.Text;

namespace TwomTrainer.Cli;

/// <summary>控制台输出：一次性写整帧、光标归位、清掉残留，颜色全部用转义序列。</summary>
public static partial class Terminal
{
    public const string Reset = "\x1b[0m";
    public const string Cyan = "\x1b[36m";
    public const string Green = "\x1b[32m";
    public const string Red = "\x1b[91m";
    public const string Gray = "\x1b[90m";

    private const int StdOutputHandle = -11;
    private const uint EnableVirtualTerminalProcessing = 0x0004;

    private static readonly StringBuilder Frame = new();

    /// <summary>启用转义序列、隐藏光标、清屏。程序启动时调一次。</summary>
    public static void Initialize()
    {
        Console.OutputEncoding = Encoding.UTF8;

        var handle = GetStdHandle(StdOutputHandle);
        if (GetConsoleMode(handle, out var mode))
        {
            SetConsoleMode(handle, mode | EnableVirtualTerminalProcessing);
        }

        Console.Write("\x1b[?25l\x1b[2J");
    }

    /// <summary>把一整帧写到屏幕上：光标回左上角，写完后清掉下面残留的旧内容。</summary>
    public static void Render(string frame)
    {
        Frame.Clear();
        Frame.Append("\x1b[H").Append(frame).Append("\x1b[0J");
        Console.Write(Frame);
    }

    /// <summary>恢复光标显示和默认颜色。程序退出时调一次。</summary>
    public static void Restore()
    {
        Console.Write("\x1b[?25h\x1b[0m");
    }

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial IntPtr GetStdHandle(int handle);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetConsoleMode(IntPtr handle, out uint mode);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetConsoleMode(IntPtr handle, uint mode);
}
