using System;
using System.Diagnostics;
using System.Linq;

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

    /// <summary>查找游戏进程并接管内存，成功返回 true；游戏没开返回 false。</summary>
    public static bool Attach()
    {
        var running = Process.GetProcessesByName(ProcessName).Where(p => !p.HasExited).ToArray();
        foreach (var extra in running[1..])
        {
            extra.Dispose();
        }

        if (running.Length == 0)
        {
            return false;
        }

        _process = running[0];
        Handle = _process.Handle;
        BaseAddress = _process.MainModule!.BaseAddress.ToInt64();
        Pid = _process.Id;
        return true;
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
