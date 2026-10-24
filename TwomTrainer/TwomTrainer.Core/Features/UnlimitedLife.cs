namespace TwomTrainer.Core.Features;

/// <summary>无限生命：打开游戏自带的「防弹」作弊旗标，居民受到的伤害被直接拦掉。</summary>
public sealed class UnlimitedLife : IFeature
{
    // 配置对象 RVA 0x9236E0，旗标在它 +0x2C6 处
    private const int FlagRva = 0x9239A6;

    private static readonly byte[] OffBytes = [0x00];
    private static readonly byte[] OnBytes = [0x01];

    public int Rva => FlagRva;

    public byte[] Original => OffBytes;

    public byte[] Patched => OnBytes;
}
