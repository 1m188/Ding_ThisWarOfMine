namespace TwomTrainer.Core.Features;

/// <summary>锁定晚上时间：夜间时钟的累计秒数不再写回，时钟停在当前这一格，流程的节奏计时照常走。</summary>
public sealed class LockNight : IFeature
{
    // 原指令 F3 0F 11 41 24 = movss [rcx+0x24],xmm0（把本帧累加后的夜间秒数写回流程对象）
    private const int PatchRva = 0x3F6581;

    private static readonly byte[] OriginalBytes = [0xF3, 0x0F, 0x11, 0x41, 0x24];

    private static readonly byte[] PatchedBytes = [0x90, 0x90, 0x90, 0x90, 0x90];

    public int Rva => PatchRva;

    public byte[] Original => OriginalBytes;

    public byte[] Patched => PatchedBytes;
}
