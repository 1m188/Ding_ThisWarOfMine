using System;
using System.ComponentModel;

namespace TwomTrainer.Core.Features;

/// <summary>一处补丁：改哪个偏移、原始字节是什么、开启时写成什么。</summary>
public readonly record struct Patch(int Rva, byte[] Original, byte[] Patched);

/// <summary>修改器功能接口。一个功能就是一张补丁表，开关动作由默认实现统一完成。</summary>
public interface IFeature
{
    /// <summary>这个功能要改的所有补丁点；开启按顺序写入，关闭按相反顺序恢复。</summary>
    public Patch[] Patches => []; // 默认空补丁

    /// <summary>开启功能。所有补丁点都写成目标字节才返回 true；有一处对不上或写失败就整体放弃，已写过的会回滚。</summary>
    public bool Start() => PatchSet.Apply(Patches);

    /// <summary>关闭功能。把仍是「已改」状态的补丁点恢复成原始字节，返回是否全部回到原始状态。</summary>
    public bool Stop() => PatchSet.Revert(Patches);
}

/// <summary>补丁表的读写：先整体校验再动手，写坏了回滚；恢复时只还原确定是我们改过的那几处。</summary>
internal static class PatchSet
{
    /// <summary>开启。每处要么是原始字节、要么已经是目标字节；出现第三种情况就一个字节都不写。</summary>
    internal static bool Apply(ReadOnlySpan<Patch> patches)
    {
        var alreadyPatched = new bool[patches.Length];

        try
        {
            for (var i = 0; i < patches.Length; i++)
            {
                var patch = patches[i];
                var current = GameMemory.Read(patch.Rva, patch.Original.Length);

                if (current.AsSpan().SequenceEqual(patch.Original))
                {
                    continue;
                }

                if (!current.AsSpan().SequenceEqual(patch.Patched))
                {
                    // 既不是原样也不是我们改的样子：游戏版本不对，或者别的修改器动过这一处。
                    return false;
                }

                alreadyPatched[i] = true;
            }
        }
        catch (Win32Exception)
        {
            // 读不到（游戏没开、句柄失效）就当失败，不写。
            return false;
        }

        var written = new bool[patches.Length];

        try
        {
            for (var i = 0; i < patches.Length; i++)
            {
                if (alreadyPatched[i])
                {
                    continue;
                }

                GameMemory.Write(patches[i].Rva, patches[i].Patched);
                written[i] = true;
            }

            return true;
        }
        catch (Win32Exception)
        {
            Rollback(patches, written);
            return false;
        }
    }

    /// <summary>关闭。已经是原始字节的跳过，被别人改过的也不动，其余恢复。</summary>
    internal static bool Revert(ReadOnlySpan<Patch> patches)
    {
        var ok = true;

        for (var i = patches.Length - 1; i >= 0; i--)
        {
            var patch = patches[i];

            try
            {
                var current = GameMemory.Read(patch.Rva, patch.Original.Length);

                if (current.AsSpan().SequenceEqual(patch.Original))
                {
                    continue;
                }

                if (!current.AsSpan().SequenceEqual(patch.Patched))
                {
                    // 被别人改过，不知道该恢复成什么，保持不动并如实报告没恢复干净。
                    ok = false;
                    continue;
                }

                GameMemory.Write(patch.Rva, patch.Original);
            }
            catch (Win32Exception)
            {
                ok = false;
            }
        }

        return ok;
    }

    private static void Rollback(ReadOnlySpan<Patch> patches, bool[] written)
    {
        for (var i = patches.Length - 1; i >= 0; i--)
        {
            if (!written[i])
            {
                continue;
            }

            try
            {
                GameMemory.Write(patches[i].Rva, patches[i].Original);
            }
            catch (Win32Exception)
            {
                // 回滚也可能失败（例如游戏正好在这时退出），只能尽力。
            }
        }
    }
}
