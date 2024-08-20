using HarmonyLib;
using YMG.Utils;

namespace YMG.Patches;

[HarmonyPatch(typeof(Constants), nameof(Constants.GetBroadcastVersion))]
class ServerUpdatePatch
{
    public static int re = 50605450;
    static void Postfix(ref int __result)
    {
        re = 50605450;
        if (GetPlayer.IsLocalGame)
        {
            Logger.Info($"IsLocalGame: {__result}", "VersionServer");
        }
        if (GetPlayer.IsOnlineGame)
        {
            // Changing server version for AU mods
            if (Main.isModProtocol)
                __result += 25;
            re = __result;
            Logger.Info($"IsOnlineGame: {__result}", "VersionServer");
        }
    }
}

[HarmonyPatch(typeof(Constants), nameof(Constants.IsVersionModded))]
public static class IsVersionModdedPatch
{
    public static bool Prefix(ref bool __result)
    {
        if (!Main.isModProtocol) return true;
        __result = true;
        return false;
    }
}