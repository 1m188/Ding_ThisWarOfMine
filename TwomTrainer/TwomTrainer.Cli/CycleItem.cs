using TwomTrainer.Core.Features;

namespace TwomTrainer.Cli;

/// <summary>档位型功能：按一下升一档，到头回到 1 档；没有「开关」的概念。</summary>
public sealed class CycleItem(string name, GameSpeed feature) : IFeatureItem
{
    public string Name { get; } = name;

    public IFeature Feature => feature;

    public bool Active => feature.Level > 1;

    public string State => $"×{feature.Level}";

    public void Press()
    {
        feature.Start();
        Sounds.Play(Active);
    }

    // 档位记在 GameSpeed 里（它还按 PID 判断是不是同一次游戏），这里不用清。
    public void Reset()
    {
    }

    public void Disable() => feature.Stop();
}
