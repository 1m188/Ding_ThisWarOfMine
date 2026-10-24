using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace TwomTrainer.Cli;

/// <summary>开关音效：wav 打包进程序里，用系统自带的 PlaySound 播放，不依赖外部文件。</summary>
public static partial class Sounds
{
    private const uint SndAsync = 0x0001;
    private const uint SndNoDefault = 0x0002;
    private const uint SndMemory = 0x0004;

    private static readonly GCHandle On = Pin("on.wav");
    private static readonly GCHandle Off = Pin("off.wav");

    /// <summary>开功能放 on.wav，关功能放 off.wav。</summary>
    public static void Play(bool on) =>
        PlaySound((on ? On : Off).AddrOfPinnedObject(), IntPtr.Zero, SndMemory | SndAsync | SndNoDefault);

    /// <summary>把 wav 读进内存并固定住：异步播放期间缓冲区必须一直有效。</summary>
    private static GCHandle Pin(string name)
    {
        using var stream = Assembly.GetExecutingAssembly()
                               .GetManifestResourceStream($"TwomTrainer.Cli.Resources.{name}")
                           ?? throw new InvalidOperationException($"缺少嵌入的音效资源：{name}");

        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        return GCHandle.Alloc(memory.ToArray(), GCHandleType.Pinned);
    }

    [LibraryImport("winmm.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool PlaySound(nint sound, IntPtr module, uint flags);
}
