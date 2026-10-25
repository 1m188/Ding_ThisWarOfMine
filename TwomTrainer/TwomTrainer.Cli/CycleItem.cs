using TwomTrainer.Core.Features;

namespace TwomTrainer.Cli;

/// <summary>档位型功能：按一下升一档，到头回到 1 档；没有「开关」的概念。</summary>
public sealed class CycleItem(string name, GameSpeed feature) : IFeatureItem
{
    public string Name { get; } = name;

    public IFeature Feature => feature;

    public bool Active => feature.Level > 1;

    public string State => $"×{feature.Level}";

    public string? Notice { get; private set; }

    public void Press()
    {
        // 档位没变（例如时间系统还没初始化）就提示一下，不响声，界面上的档位也就不会变。
        if (feature.Start())
        {
            Notice = null;
            Sounds.Play(Active);
        }
        else
        {
            Notice = "档位切换失败";
        }
    }

    // 这里只清提示。
    public void Reset() => Notice = null;

    public void Disable() => feature.Stop();
}
