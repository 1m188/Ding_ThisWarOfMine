namespace TwomTrainer.Core.Features;

/// <summary>无限生命：打开游戏自带的「防弹」作弊旗标，居民受到的伤害被直接拦掉。</summary>
public sealed class UnlimitedLife : IFeature
{
    // 配置对象 RVA 0x9236E0，旗标在它 +0x2C6 处（合起来就是 0x9239A6）；0 = 关，1 = 开
    private static readonly Patch[] PatchList = [new(0x9239A6, [0x00], [0x01])];

    public Patch[] Patches => PatchList;
}
