using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace TwomTrainer.Core;

/// <summary>按「模块基址 + RVA」读写目标进程内存，写入前临时放开写权限、写完还原。</summary>
public static partial class GameMemory
{
    private const uint PageReadWrite = 0x04;
    private const uint PageExecuteReadWrite = 0x40;

    /// <summary>读 RVA 处的 length 个字节。</summary>
    public static byte[] Read(int rva, int length)
    {
        var buffer = new byte[length];
        if (!ReadProcessMemory(GameProcess.Handle, new IntPtr(GameProcess.BaseAddress + rva), buffer,
                (nuint)length, out var read)
            || read != (nuint)length)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        return buffer;
    }

    /// <summary>改游戏代码段用：临时放开「可读写 + 可执行」再写。</summary>
    public static void Write(int rva, ReadOnlySpan<byte> bytes) => Write(rva, bytes, PageExecuteReadWrite);

    /// <summary>改数据段（旗标、数值）用：只需要可读写，不必放开可执行。</summary>
    public static void WriteData(int rva, ReadOnlySpan<byte> bytes) => Write(rva, bytes, PageReadWrite);

    private static void Write(int rva, ReadOnlySpan<byte> bytes, uint protection)
    {
        var address = new IntPtr(GameProcess.BaseAddress + rva);
        var buffer = bytes.ToArray();
        var size = (nuint)buffer.Length;

        if (!VirtualProtectEx(GameProcess.Handle, address, size, protection, out var oldProtect))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        try
        {
            if (!WriteProcessMemory(GameProcess.Handle, address, buffer, size, out var written) || written != size)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
        finally
        {
            VirtualProtectEx(GameProcess.Handle, address, size, oldProtect, out _);
        }
    }

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ReadProcessMemory(IntPtr process, IntPtr address, byte[] buffer, nuint size, out nuint read);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool WriteProcessMemory(IntPtr process, IntPtr address, byte[] buffer, nuint size, out nuint written);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool VirtualProtectEx(IntPtr process, IntPtr address, nuint size, uint newProtect,
        out uint oldProtect);
}
