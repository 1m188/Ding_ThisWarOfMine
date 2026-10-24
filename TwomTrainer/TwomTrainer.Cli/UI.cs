using System.Collections.Generic;
using System.Text;
using TwomTrainer.Core;

namespace TwomTrainer.Cli;

/// <summary>把当前状态拼成一整帧字符串；写到屏幕上的事交给 <see cref="Terminal"/>。</summary>
public static class UI
{
    public static string Render(IReadOnlyList<FeatureItem> items)
    {
        var sb = new StringBuilder();
        sb.Append(Terminal.Cyan).Append(" 这是我的战争 修改器").Append(Terminal.Reset).Append('\n');
        sb.Append(Terminal.Gray).Append(" ────────────────────────────").Append(Terminal.Reset).Append('\n');

        if (GameProcess.IsRunning)
        {
            sb.Append(Terminal.Green)
                .Append($" 已连接  PID {GameProcess.Pid}  基址 0x{GameProcess.BaseAddress:X}")
                .Append(Terminal.Reset);
        }
        else
        {
            sb.Append(Terminal.Gray).Append(" 未连接  等待游戏启动…").Append(Terminal.Reset);
        }

        sb.Append("\n\n");

        for (var i = 0; i < items.Count; i++)
        {
            var item = items[i];
            sb.Append($" [Num{i + 1}] {item.Name}");
            sb.Append(item.Enabled
                ? $"{Terminal.Green}  ● 开{Terminal.Reset}"
                : $"{Terminal.Gray}  ○ 关{Terminal.Reset}");
            sb.Append('\n');
        }

        sb.Append('\n')
            .Append(Terminal.Gray)
            .Append(" Num1~Num8 开关功能  ·  Esc 退出（退出时会关掉所有开着的功能）")
            .Append(Terminal.Reset);

        return sb.ToString();
    }
}
