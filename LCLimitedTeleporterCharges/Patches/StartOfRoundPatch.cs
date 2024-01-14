using HarmonyLib;

namespace LCLimitedTeleporterCharges.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]

    internal class StartOfRoundPatch
    {
        [HarmonyPatch("StartGame")]
        [HarmonyPostfix]
        static void StartGamePatch()
        {
            ModBase.ResetTeleporterCharges();
        }

        [HarmonyPatch("ShipHasLeft")]
        [HarmonyPostfix]
        static void ShipHasLeftPatch()
        {
            ModBase.ResetTeleporterCharges();
        }
    }
}
