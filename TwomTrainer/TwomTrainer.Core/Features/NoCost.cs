namespace TwomTrainer.Core.Features;

/// <summary>制作不消耗资源：跳过「把配方材料从仓库搬进工作台容器」这一步，制作、建造、升级都不再扣材料。</summary>
public sealed class NoCost : IFeature
{
    // 原指令 E8 1F D7 09 00 = call 0x1403E0310（搬材料），全文只此一处
    private const int PatchRva = 0x342BEC;

    private static readonly byte[] OriginalBytes = [0xE8, 0x1F, 0xD7, 0x09, 0x00];

    private static readonly byte[] PatchedBytes = [0x90, 0x90, 0x90, 0x90, 0x90];

    public int Rva => PatchRva;

    public byte[] Original => OriginalBytes;

    public byte[] Patched => PatchedBytes;
}
