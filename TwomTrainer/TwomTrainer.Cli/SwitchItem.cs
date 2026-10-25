using TwomTrainer.Core.Features;

namespace TwomTrainer.Cli;

/// <summary>开关型功能：按一下开、再按一下关。</summary>
public sealed class SwitchItem(string name, IFeature feature) : IFeatureItem
{
    public string Name { get; } = name;

    public IFeature Feature { get; } = feature;

    public bool Active { get; private set; }

    public string State => Active ? "● 开" : "○ 关";

    public string? Notice { get; private set; }

    public void Press()
    {
        if (Active)
        {
            // 只有真的全部恢复成原始字节才把状态改成「关」，否则保持原样并在这一行后面提示原因。
            if (Feature.Stop())
            {
                Active = false;
                Notice = null;
                Sounds.Play(false);
            }
            else
            {
                Notice = "关闭失败：这处字节已被别的程序改过";
            }

            return;
        }

        if (Feature.Start())
        {
            Active = true;
            Notice = null;
            Sounds.Play(true);
        }
        else
        {
            Notice = "开启失败：字节不符或读写出错";
        }
    }

    public void Reset()
    {
        Active = false;
        Notice = null;
    }

    public void Disable()
    {
        if (!Active)
        {
            return;
        }

        Feature.Stop();
        Active = false;
        Notice = null;
    }
}
