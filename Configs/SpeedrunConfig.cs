using Microsoft.Xna.Framework;
using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace SpeedrunTimer.Configs
{
    public class SpeedrunConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        public static SpeedrunConfig Instance => ModContent.GetInstance<SpeedrunConfig>();

        public Vector2 SpeedrunUIPos => new(SpeedrunUIPosX, SpeerunUIPosY);

        [DefaultValue(true)]
        public bool LockSpeedrunUIPos { get; set; } = true;

        [DefaultValue(6)]
        public int SplitsToShow { get; set; } = 6;

        [DefaultValue("Any%")]
        public string DefaultRunCategory { get; set; } = "Any%";

        [DefaultValue(false)]
        public bool AutoRestart { get; set; } = false;

        [DefaultValue(0.9f)]
        public float SpeedrunUIPosX { get; set; } = 0.9f;

        [DefaultValue(0.3f)]
        public float SpeerunUIPosY { get; set; } = 0.3f;

        [DefaultValue(1f)]
        public float SpeedrunUIScale { get; set; } = 1f;
    }
}
