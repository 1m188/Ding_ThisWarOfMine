using System;
using System.Threading;
using TwomTrainer.Core;
using TwomTrainer.Core.Features;

namespace TwomTrainer.Cli;

internal static class Program
{
    private static readonly FeatureItem[] Items =
    [
        new("无限生命", new UnlimitedLife()),
        new("无限背包空间", new UnlimitedPackage()),
        new("工具无限耐久度", new UnlimitedToolDurability()),
        new("瞬间制作物品", new CreateInstantly()),
        new("锁定白天时间", new LockDay()),
        new("锁定晚上时间", new LockNight()),
        new("一击必杀", new OneHitKill()),
        new("制作不消耗资源", new NoCost()),
    ];

    private static int Main()
    {
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

                Items[i].Toggle();
                Terminal.Render(UI.Render(Items));
            }

            if (Keyboard.Pressed(Keyboard.Escape))
            {
                return;
            }

            Thread.Sleep(40);
        }
    }

    private static void Shutdown()
    {
        if (GameProcess.IsRunning)
        {
            foreach (var item in Items)
            {
                item.Disable();
            }
        }

        GameProcess.Release();
        Terminal.Restore();
    }
}
