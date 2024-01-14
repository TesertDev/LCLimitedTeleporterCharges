using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LCLimitedTeleporterCharges.Patches;
using System;
using LC_API.Networking;
using LC_API.GameInterfaceAPI.Events.EventArgs.Player;
using System.Runtime.InteropServices;

namespace LCLimitedTeleporterCharges
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class ModBase : BaseUnityPlugin
    {
        private const string modGUID = "Tesert.LCLimitedTeleporterCharges";
        private const string modName = "Limited Teleporter Charges";
        private const string modVersion = "1.0.0";
            
        private readonly Harmony harmony = new Harmony(modGUID);

        internal static ModBase Instance;

        internal ManualLogSource mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

        internal ConfigurationController ConfigManager;

        internal static InteractTrigger teleportButton = null;
        internal static string baseBeamUpString = null;

        private static int teleportMaxCharges;
        private static int teleporterCharges;
        internal static int TeleporterCharges
        {
            get { return teleporterCharges; }
            set 
            {
                Instance.mls.LogInfo("Changing teleporter charges from: " + teleporterCharges + " to: " + value);
                teleporterCharges = value;
                if (teleportButton!=null)
                {
                    if (teleporterCharges == 1)
                    {
                        teleportButton.hoverTip = baseBeamUpString + "\n[" + TeleporterCharges + " charge left]";
                    }
                    else
                    {
                        teleportButton.hoverTip = baseBeamUpString + "\n[" + TeleporterCharges + " charges left]";
                    }
                }
            }
        }

        void Awake()
        {
            if (Instance==null)
            {
                Instance = this;
            }
            
            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo("LimitedTeleporterCharges loaded!");
            ConfigManager = new ConfigurationController(Config);
            harmony.PatchAll(typeof(ModBase));
            harmony.PatchAll(typeof(ShipTeleporterPatch));
            harmony.PatchAll(typeof(StartOfRoundPatch));
            Network.RegisterAll();
            teleportMaxCharges = ConfigManager.TeleportMaxCharges;
            teleporterCharges = teleportMaxCharges;
            LC_API.GameInterfaceAPI.Events.Handlers.Player.Joined += PlayerJoined;
        }

        internal static void ResetTeleporterCharges()
        {
            Instance.mls.LogInfo("Resetting teleporter charges to: " + teleportMaxCharges);
            TeleporterCharges = teleportMaxCharges;
        }

        [NetworkMessage("teleporterActivatedMessage")]
        static void TeleporterActivatedHandler(ulong sender)
        {
            Instance.mls.LogInfo("Teleport use message received from " + sender + "!");
            TeleporterCharges--;
        }

        internal static void TeleporterActivated()
        {
            Instance.mls.LogInfo("Teleport use message sent!");
            Network.Broadcast("teleporterActivatedMessage");
        }

        [NetworkMessage("hostSharingTeleporterMaximumChargesMsg")]
        static void HostSharingTeleporterMaximumChargesHandler(ulong sender, string message)
        {
            if (!LC_API.GameInterfaceAPI.Features.Player.LocalPlayer.IsHost)
            {
                string[] stringArray = message.Split(',');
                Instance.mls.LogInfo("Client received max teleporter charges + [" + stringArray[0] + "] and current teleporter charges + [" + stringArray[1] + "] from host, updating charges to sync with host.");
                teleportMaxCharges = Int32.Parse(stringArray[0]);
                TeleporterCharges = Int32.Parse(stringArray[1]);
            }
        }

        internal bool isHost()
        {
            Instance.mls.LogInfo("Attempting to check if player is host.");
            return LC_API.GameInterfaceAPI.Features.Player.LocalPlayer.IsHost;
        }

        void PlayerJoined(JoinedEventArgs e)
        {
            LC_API.GameInterfaceAPI.Features.Player player = e.Player;
            if (isHost())
            {
                Instance.mls.LogInfo("A player joined, sharing teleporter charges data with them.");
                Network.Broadcast("hostSharingTeleporterMaximumChargesMsg", teleportMaxCharges.ToString() + "," + TeleporterCharges.ToString());
            }
        }
    }
}
