namespace TwomTrainer.Core.Features;

/// <summary>一击必杀：抹掉「伤害载荷里的必杀标记是否为 0」这个判断，任何伤害都直接走死亡分支。</summary>
public sealed class OneHitKill : IFeature
{
    // 原指令 74 06 = je 0x33E2CF（标记为 0 时跳过必杀）
    private const int PatchRva = 0x33E2C7;

    private static readonly byte[] OriginalBytes = [0x74, 0x06];

    private static readonly byte[] PatchedBytes = [0x90, 0x90];

    public int Rva => PatchRva;

    public byte[] Original => OriginalBytes;

    public byte[] Patched => PatchedBytes;
}
