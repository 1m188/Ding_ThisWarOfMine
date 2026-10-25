namespace TwomTrainer.Core.Features;

/// <summary>无限生命：打开游戏自带的「防弹」作弊旗标，居民受到的伤害被直接拦掉。</summary>
public sealed class UnlimitedLife : IFeature
{
    // 配置对象 RVA 0x9236E0，旗标在它 +0x2C6 处（合起来就是 0x9239A6）；0 = 关，1 = 开
    private const int PatchRva = 0x9239A6;

    private static readonly byte[] OriginalBytes = [0x00];

    private static readonly byte[] PatchedBytes = [0x01];

    public int Rva => PatchRva;

    public byte[] Original => OriginalBytes;

    public byte[] Patched => PatchedBytes;
}
