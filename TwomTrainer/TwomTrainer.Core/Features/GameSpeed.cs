using System;

namespace TwomTrainer.Core.Features;

/// <summary>游戏倍速：每次 Start 升一档（1→2→…→8→1），Stop 回到 1 档并写回原值。</summary>
public sealed class GameSpeed : IFeature
{
    private const int MaxLevel = 8;

    /// <summary>原值是从哪个进程读的；换了进程要重新读。</summary>
    private int _pid;

    private int _level = 1;
    private byte[]? _original;

    public int Rva => 0x823FB8;

    public byte[] Original
    {
        get
        {
            if (_pid != GameProcess.Pid)
            {
                _original = null;
                _level = 1;
            }

            _original ??= GameMemory.Read(Rva, sizeof(double));
            _pid = GameProcess.Pid;
            return _original;
        }
    }

    /// <summary>下一档的字节（接口要求公开；实际写入在 <see cref="Start"/> 里算）。</summary>
    public byte[] Patched => BitConverter.GetBytes(BitConverter.ToDouble(Original) / (_level == MaxLevel ? 1 : _level + 1));

    /// <summary>当前档位，1 表示原始速度。</summary>
    public int Level => _pid == GameProcess.Pid ? _level : 1;

    public void Start()
    {
        var original = BitConverter.ToDouble(Original);
        if (original is < 1000 or > 1e10)
        {
            // 时间系统还没初始化，这次不写，档位也不动。
            return;
        }

        _level = _level == MaxLevel ? 1 : _level + 1;
        GameMemory.Write(Rva, Bytes(original, _level));
    }

    public void Stop()
    {
        var original = BitConverter.ToDouble(Original);
        _level = 1;
        if (original is >= 1000 and <= 1e10)
        {
            GameMemory.Write(Rva, Bytes(original, 1));
        }
    }

    private static byte[] Bytes(double original, int level) =>
        BitConverter.GetBytes(level == 1 ? original : original / level);
}
