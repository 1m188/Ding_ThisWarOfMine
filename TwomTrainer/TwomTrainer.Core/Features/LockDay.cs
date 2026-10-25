namespace TwomTrainer.Core.Features;

/// <summary>锁定白天时间：写入时钟进度时改成读回旧进度，白天时钟就停住不动。</summary>
public sealed class LockDay : IFeature
{
    // 原指令 0F 28 D9 / F3 0F 11 59 10 = movaps xmm3,xmm1 ; movss [rcx+0x10],xmm3
    // 改成 F3 0F 10 49 10 / 0F 28 D9 = movss xmm1,[rcx+0x10] ; movaps xmm3,xmm1
    private static readonly Patch[] PatchList =
    [
        new(0x3F5724,
            [0x0F, 0x28, 0xD9, 0xF3, 0x0F, 0x11, 0x59, 0x10],
            [0xF3, 0x0F, 0x10, 0x49, 0x10, 0x0F, 0x28, 0xD9]),
    ];

    public Patch[] Patches => PatchList;
}
