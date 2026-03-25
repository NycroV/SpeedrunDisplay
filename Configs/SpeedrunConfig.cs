using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace SpeedrunTimer.Configs
{
    public class SpeedrunConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [JsonIgnore]
        public static SpeedrunConfig Instance => ModContent.GetInstance<SpeedrunConfig>();

        [JsonIgnore]
        public Vector2 SpeedrunUIPos => new(SpeedrunUIPosX, SpeerunUIPosY);

        [DefaultValue(true)]
        public bool LockSpeedrunUIPos { get; set; }

        [DefaultValue(1f)]
        public float SpeedrunUIScale { get; set; }

        [DefaultValue(6)]
        public int SplitsToShow { get; set; }

        [DefaultValue("Any%")]
        public string DefaultRunCategory { get; set; }

        [DefaultValue(false)]
        public bool AutoRestart { get; set; }

        [DefaultValue(0.9f)]
        public float SpeedrunUIPosX { get; set; }

        [DefaultValue(0.3f)]
        public float SpeerunUIPosY { get; set; }
    }
}
