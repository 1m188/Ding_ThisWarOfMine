namespace TwomTrainer.Core.Features;

/// <summary>一处补丁：改哪个偏移、原始字节是什么、开启时写成什么。</summary>
public readonly record struct Patch(int Rva, byte[] Original, byte[] Patched);

/// <summary>修改器功能接口。一个功能就是一张补丁表，开关动作由默认实现统一完成。</summary>
public interface IFeature
{
    /// <summary>这个功能要改的所有补丁点；开启按顺序写入，关闭按相反顺序恢复。</summary>
    public Patch[] Patches => []; // 默认空补丁

    /// <summary>开启功能。</summary>
    public void Start()
    {
        foreach (var patch in Patches)
        {
            GameMemory.Write(patch.Rva, patch.Patched);
        }
    }

    /// <summary>关闭功能。</summary>
    public void Stop()
    {
        for (var i = Patches.Length - 1; i >= 0; i--)
        {
            GameMemory.Write(Patches[i].Rva, Patches[i].Original);
        }
    }
}
