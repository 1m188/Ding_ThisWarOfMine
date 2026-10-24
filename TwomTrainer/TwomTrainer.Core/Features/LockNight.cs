namespace TwomTrainer.Core.Features;

/// <summary>锁定晚上时间：跳过夜间流程入口的计时累加，夜间时钟的计数不再增长。</summary>
public sealed class LockNight : IFeature
{
    // 原指令 E8 29 49 00 00 = call 0x1403F6550（把本帧秒数累加到流程对象上）
    private const int PatchRva = 0x3F1C22;

    private static readonly byte[] OriginalBytes = [0xE8, 0x29, 0x49, 0x00, 0x00];

    private static readonly byte[] PatchedBytes = [0x90, 0x90, 0x90, 0x90, 0x90];

    public int Rva => PatchRva;

    public byte[] Original => OriginalBytes;

    public byte[] Patched => PatchedBytes;
}
