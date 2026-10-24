using System.Runtime.InteropServices;

namespace TwomTrainer.Cli;

/// <summary>全局按键：直接轮询系统按键状态，不要求控制台窗口有焦点，游戏在前台也能收到。</summary>
public static partial class Keyboard
{
    /// <summary>小键盘 1；小键盘 2~8 依次加一。</summary>
    public const int Num1 = 0x61;

    public const int Escape = 0x1B;

    private static readonly bool[] Down = new bool[256];

    /// <summary>这个键这一轮是不是刚按下；按住不放只算一次。</summary>
    public static bool Pressed(int virtualKey)
    {
        var down = (GetAsyncKeyState(virtualKey) & 0x8000) != 0;
        var pressed = down && !Down[virtualKey];
        Down[virtualKey] = down;
        return pressed;
    }

    [LibraryImport("user32.dll")]
    private static partial short GetAsyncKeyState(int virtualKey);
}
