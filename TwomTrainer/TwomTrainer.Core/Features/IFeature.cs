namespace TwomTrainer.Core.Features;

/// <summary>修改器功能接口。一个修改器功能：只描述「改哪一处、改成什么」，开关动作由默认实现统一完成。</summary>
public interface IFeature
{
    /// <summary>要改的那一处相对游戏主程序基址的偏移。</summary>
    public int Rva { get; }

    /// <summary>关闭时写回的原始字节。</summary>
    public byte[] Original { get; }

    /// <summary>开启时写入的字节。</summary>
    public byte[] Patched { get; }

    /// <summary>开启功能。</summary>
    public void Start() => GameMemory.Write(Rva, Patched);

    /// <summary>关闭功能。</summary>
    public void Stop() => GameMemory.Write(Rva, Original);
}
