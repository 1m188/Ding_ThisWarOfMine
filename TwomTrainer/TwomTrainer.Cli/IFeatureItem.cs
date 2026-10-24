using TwomTrainer.Core.Features;

namespace TwomTrainer.Cli;

/// <summary>界面上的一行：一个功能、一个按键、一种显示方式。</summary>
public interface IFeatureItem
{
    /// <summary>显示在界面上的名称。</summary>
    public string Name { get; }

    /// <summary>修改器功能。</summary>
    public IFeature Feature { get; }

    /// <summary>是否处于「已生效」状态。</summary>
    public bool Active { get; }

    /// <summary>状态文字。</summary>
    public string State { get; }

    /// <summary>按下对应按键时调用：改内存、放声音、更新状态等。</summary>
    public void Press();

    /// <summary>游戏进程不在了：只把界面状态清掉，不碰内存。</summary>
    public void Reset();

    /// <summary>退出修改器：把已经改过的内存写回原样（可重复调用）。</summary>
    public void Disable();
}
