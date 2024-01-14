using HarmonyLib;

namespace LCLimitedTeleporterCharges.Patches
{
    [HarmonyPatch(typeof(ShipTeleporter))]
    internal class ShipTeleporterPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        static void AwakePatch(ref bool ___isInverseTeleporter, ref InteractTrigger ___buttonTrigger)
        {
            if (!___isInverseTeleporter)
            {
                ModBase.teleportButton = ___buttonTrigger;
                ModBase.baseBeamUpString = ___buttonTrigger.hoverTip;
                if (ModBase.TeleporterCharges==1)
                {
                    ___buttonTrigger.hoverTip = ModBase.baseBeamUpString + "\n[" + ModBase.TeleporterCharges + " charge left]";
                } else
                {
                    ___buttonTrigger.hoverTip = ModBase.baseBeamUpString + "\n[" + ModBase.TeleporterCharges + " charges left]";
                }
            }
        }

        [HarmonyPatch("PressTeleportButtonOnLocalClient")]
        [HarmonyPrefix]
        static bool PressTeleportButtonOnLocalClientPatch(ref bool ___isInverseTeleporter)
        {
            if (!___isInverseTeleporter) {
                if (ModBase.TeleporterCharges > 0)
                {
                    ModBase.TeleporterCharges--;
                    ModBase.TeleporterActivated();
                }
                else
                {
                    ModBase.Instance.mls.LogWarning("No teleporter charges remaining yet button is interactable. Should not reach this point!");
                    return false; // Skips the original method
                }
            }
            return true;
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void UpdatePostPatch(ref bool ___isInverseTeleporter, ref InteractTrigger ___buttonTrigger) 
        {
            if (!___isInverseTeleporter && ModBase.TeleporterCharges <= 0)
            {
                ___buttonTrigger.interactable = false;
                ___buttonTrigger.disabledHoverTip = "[No charges left]";
            }
            else if (___isInverseTeleporter)
            {
                ___buttonTrigger.hoverTip = ModBase.baseBeamUpString;
            }
        }
    }
}
