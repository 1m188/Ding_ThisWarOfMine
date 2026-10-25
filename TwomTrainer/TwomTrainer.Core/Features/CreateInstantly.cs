namespace TwomTrainer.Core.Features;

/// <summary>瞬间制作：把「进度 += 本帧时间片」改成「进度 += 整段时长」，一次推进就把进度顶满。</summary>
public sealed class CreateInstantly : IFeature
{
    // 原指令 F3 0F 58 77 54 = addss xmm6,[rdi+0x54]，只改末尾两个字节
    // 变成 F3 0F 58 F1 90 = addss xmm6,xmm1 ; nop
    private static readonly Patch[] PatchList = [new(0x345625, [0x77, 0x54], [0xF1, 0x90])];

    public Patch[] Patches => PatchList;
}
