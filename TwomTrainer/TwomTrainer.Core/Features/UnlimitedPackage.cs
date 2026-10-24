namespace TwomTrainer.Core.Features;

/// <summary>无限背包空间：把「按角色模板读背包格数」改成直接返回 300，界面刷新与放入判定都按 300 算。</summary>
public sealed class UnlimitedPackage : IFeature
{
    // 原函数 48 8B 41 28 / 8B 80 A0 01 00 00 / C3 = 读模板 +0x1A0 的 InventorySlots 后返回
    private const int PatchRva = 0x2CDD00;

    private static readonly byte[]
        OriginalBytes = [0x48, 0x8B, 0x41, 0x28, 0x8B, 0x80, 0xA0, 0x01, 0x00, 0x00, 0xC3];

    // 改成 B8 2C 01 00 00 / C3 = mov eax,300 ; ret，其余补空操作
    private static readonly byte[] PatchedBytes = [0xB8, 0x2C, 0x01, 0x00, 0x00, 0xC3, 0x90, 0x90, 0x90, 0x90, 0x90];

    public int Rva => PatchRva;

    public byte[] Original => OriginalBytes;

    public byte[] Patched => PatchedBytes;
}
