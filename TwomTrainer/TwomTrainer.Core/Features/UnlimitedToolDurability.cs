namespace TwomTrainer.Core.Features;

/// <summary>无限工具耐久：把耐久递减那一条减法换成空操作，工具的耐久值不再下降。</summary>
public sealed class UnlimitedToolDurability : IFeature
{
    // 原指令 F3 0F 5C C2 = subss xmm0,xmm2（当前耐久 − 衰减量）
    private const int PatchRva = 0x319B41;

    private static readonly byte[] OriginalBytes = [0xF3, 0x0F, 0x5C, 0xC2];

    private static readonly byte[] PatchedBytes = [0x90, 0x90, 0x90, 0x90];

    public int Rva => PatchRva;

    public byte[] Original => OriginalBytes;

    public byte[] Patched => PatchedBytes;
}
