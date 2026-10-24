using TwomTrainer.Core.Features;

namespace TwomTrainer.Cli;

/// <summary>界面上的一个功能项：显示名 + Core 里的功能 + 当前开关状态。</summary>
public sealed class FeatureItem(string name, IFeature feature)
{
    public string Name { get; } = name;

    public IFeature Feature { get; } = feature;

    public bool Enabled { get; private set; }

    /// <summary>按一下：开着就关，关着就开。</summary>
    public void Toggle()
    {
        if (Enabled)
        {
            Feature.Stop();
        }
        else
        {
            Feature.Start();
        }

        Enabled = !Enabled;
    }

    /// <summary>游戏进程不在了：只清状态，不碰内存。</summary>
    public void Reset() => Enabled = false;

    /// <summary>退出时把还开着的功能关掉。</summary>
    public void Disable()
    {
        if (!Enabled)
        {
            return;
        }

        Feature.Stop();
        Enabled = false;
    }
}
