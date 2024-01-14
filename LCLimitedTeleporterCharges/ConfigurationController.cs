using BepInEx.Configuration;

namespace LCLimitedTeleporterCharges
{
    internal class ConfigurationController
    {
        private ConfigEntry<int> TeleportMaxChargesCfg;
        internal int TeleportMaxCharges
        {
            get
            {
                if (TeleportMaxChargesCfg.Value < 0)
                {
                    return (int) TeleportMaxChargesCfg.DefaultValue;
                }
                return TeleportMaxChargesCfg.Value;
            }
            set => TeleportMaxChargesCfg.Value = value;
        }
        public ConfigurationController(ConfigFile Config)
        {
            TeleportMaxChargesCfg = Config.Bind("General", "Teleporter max charges", 2, "The maximum number of times the teleporter can be used in one round. Note that everyone's maximum number of charges will depend on the host's setting of this value.");
        }
    }
}
