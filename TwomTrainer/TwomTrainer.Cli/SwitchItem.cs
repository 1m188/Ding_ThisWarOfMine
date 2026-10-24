using TwomTrainer.Core.Features;

namespace TwomTrainer.Cli;

/// <summary>开关型功能：按一下开、再按一下关。</summary>
public sealed class SwitchItem(string name, IFeature feature) : IFeatureItem
{
    public string Name { get; } = name;

    public IFeature Feature { get; } = feature;

    public bool Active { get; private set; }

    public string State => Active ? "● 开" : "○ 关";

    public void Press()
    {
        if (Active)
        {
            Feature.Stop();
            Active = false;
        }
        else
        {
            Feature.Start();
            Active = true;
        }

        Sounds.Play(Active);
    }

    public void Reset() => Active = false;

    public void Disable()
    {
        if (!Active)
        {
            return;
        }

        Feature.Stop();
        Active = false;
    }
}
