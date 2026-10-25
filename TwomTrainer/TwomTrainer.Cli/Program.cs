using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using TwomTrainer.Core;
using TwomTrainer.Core.Features;

namespace TwomTrainer.Cli;

internal static partial class Program
{
    private static readonly IFeatureItem[] Items =
    [
        new SwitchItem("无限生命", new UnlimitedLife()),
        new SwitchItem("无限背包空间", new UnlimitedPackage()),
        new SwitchItem("工具无限耐久度", new UnlimitedToolDurability()),
        new SwitchItem("瞬间制作物品", new CreateInstantly()),
        new SwitchItem("锁定白天时间", new LockDay()),
        new SwitchItem("锁定晚上时间", new LockNight()),
        new SwitchItem("一击必杀", new OneHitKill()),
        new SwitchItem("制作不消耗资源", new NoCost()),
        new CycleItem("游戏倍速", new GameSpeed()),
    ];

    // 系统回调只持有函数指针，委托必须由静态字段挂住，否则会被 GC 回收。
    private static readonly ConsoleCtrlHandler Handler = OnConsoleSignal;

    private static bool _stopped;

    private static int Main()
    {
        AppDomain.CurrentDomain.ProcessExit += (_, _) => Shutdown();
        Console.CancelKeyPress += (_, _) => Shutdown();
        SetConsoleCtrlHandler(Handler, true);

        Terminal.Initialize();
        try
        {
            Run();
        }
        finally
        {
            Shutdown();
        }

        return 0;
    }

    private static void Run()
    {
        var attached = false;

        while (true)
        {
            // 游戏没开：释放句柄、清掉开关状态（都是幂等操作），然后继续尝试连接。
            if (!GameProcess.IsRunning)
            {
                GameProcess.Release();
                foreach (var item in Items)
                {
                    item.Reset();
                }

                attached = false;
                Terminal.Render(UI.Render(Items));
                Thread.Sleep(500);
                GameProcess.Attach();
                continue;
            }

            // 刚连上：立刻画一帧「已连接」。
            if (!attached)
            {
                attached = true;
                Terminal.Render(UI.Render(Items));
            }

            for (var i = 0; i < Items.Length; i++)
            {
                if (!Keyboard.Pressed(Keyboard.Num1 + i))
                {
                    continue;
                }

                try
                {
                    Items[i].Press();
                    Terminal.Render(UI.Render(Items));
                }
                catch (Win32Exception)
                {
                    // 游戏恰好在这一瞬间退出：下一轮循环会走「未连接」分支重新来过。
                }
            }

            Thread.Sleep(40);
        }
    }

    /// <summary>Ctrl+C、关闭终端窗口时由系统调用；返回 false 表示不阻止系统继续退出。</summary>
    private static bool OnConsoleSignal(uint signal)
    {
        Shutdown();
        return false;
    }

    /// <summary>退出前把还开着的功能关掉、放开句柄。<see cref="_stopped"/> 保证只执行一次。</summary>
    private static void Shutdown()
    {
        if (_stopped)
        {
            return;
        }

        _stopped = true;

        if (GameProcess.IsRunning)
        {
            foreach (var item in Items)
            {
                try
                {
                    item.Disable();
                }
                catch (Exception)
                {
                    // 游戏可能正好在退出：这一项写不进去就跳过，别影响其它项和后续清理。
                }
            }
        }

        GameProcess.Release();

        try
        {
            Terminal.Restore();
        }
        catch (Exception)
        {
            // 关窗口时控制台已经没了，写不进去是正常的。
        }
    }

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetConsoleCtrlHandler(ConsoleCtrlHandler handler,
        [MarshalAs(UnmanagedType.Bool)] bool add);

    private delegate bool ConsoleCtrlHandler(uint signal);
}
