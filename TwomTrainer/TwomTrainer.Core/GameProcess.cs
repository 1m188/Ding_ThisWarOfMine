using System;
using System.Diagnostics;

namespace TwomTrainer.Core;

/// <summary>游戏进程：查找、持有句柄、保存主程序在内存中的加载地址。</summary>
public static class GameProcess
{
    private const string ProcessName = "This War of Mine"; // 进程名称

    private static Process? _process; // 进程

    /// <summary>目标进程句柄，供 <see cref="GameMemory"/> 使用。</summary>
    public static IntPtr Handle { get; private set; }

    /// <summary>游戏主程序在内存中的加载地址；所有 RVA 都以它为基准。每次启动游戏都会变。</summary>
    public static long BaseAddress { get; private set; }

    public static int Pid { get; private set; }

    /// <summary>游戏是否还在运行。查不出来（比如权限不足）按已退出处理。</summary>
    public static bool IsRunning
    {
        get
        {
            try
            {
                return _process is { HasExited: false };
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    /// <summary>查找游戏进程并接管内存，成功返回 true；游戏没开或读不到（权限不足、刚好退出）返回 false。</summary>
    public static bool Attach()
    {
        Process[] running;
        try
        {
            running = Process.GetProcessesByName(ProcessName);
        }
        catch (Exception)
        {
            return false;
        }

        Process? target = null;

        // 第一个读得到的进程对象留作目标，其余的全部立刻释放（不然它们的句柄要等 GC 才收）。
        foreach (var process in running)
        {
            if (target is null)
            {
                try
                {
                    if (!process.HasExited)
                    {
                        target = process;
                        continue;
                    }
                }
                catch (Exception)
                {
                    // 看不了状态的进程（权限不足、刚好退出）当作没找到。
                }
            }

            process.Dispose();
        }

        if (target is null)
        {
            return false;
        }

        try
        {
            Handle = target.Handle;
            BaseAddress = target.MainModule!.BaseAddress.ToInt64();
            Pid = target.Id;
            _process = target;
            return true;
        }
        catch (Exception)
        {
            target.Dispose();
            Handle = IntPtr.Zero;
            BaseAddress = 0;
            Pid = 0;
            return false;
        }
    }

    /// <summary>放开当前进程（不会结束游戏）。</summary>
    public static void Release()
    {
        _process?.Dispose();
        _process = null;
        Handle = IntPtr.Zero;
        BaseAddress = 0;
        Pid = 0;
    }
}
