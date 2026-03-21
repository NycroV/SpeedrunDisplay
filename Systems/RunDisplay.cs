using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpeedrunTimer.Configs;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SpeedrunTimer.Systems
{
    public class RunDisplay : ModSystem
    {
        public static bool DisplayTimer { get; set; } = true;

        public override void OnModLoad() => On_Main.DrawMenu += DrawMenuRunButtons;

        private void DrawMenuRunButtons(On_Main.orig_DrawMenu orig, Main self, GameTime gameTime)
        {
            orig(self, gameTime);

            Main.spriteBatch.Begin();
            DrawSpeedrunTimer(Main.spriteBatch, new Vector2(Main.graphics.PreferredBackBufferWidth, Main.graphics.PreferredBackBufferHeight));
            Main.spriteBatch.End();

            // TODO: Draw run start/cancel buttons
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            if (!DisplayTimer)
                return;

            int layerIndex = 0;// layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            if (layerIndex == -1)
                return;

            layers.Insert(layerIndex, new LegacyGameInterfaceLayer(
                "SpeedrunTimer: Timer Display", () =>
                {
                    DrawSpeedrunTimer(Main.spriteBatch, Main.ScreenSize.ToVector2());
                    return true;
                }, InterfaceScaleType.None));
        }

        public static void DrawSpeedrunTimer(SpriteBatch spriteBatch, Vector2 screenSize)
        {
            Vector2 drawCenter = screenSize * SpeedrunConfig.Instance.SpeedrunUIPos;
            Vector2 drawSize = new Vector2(200f, 400f) * SpeedrunConfig.Instance.SpeedrunUIScale;

            Rectangle drawArea = CenteredRectangle(drawCenter, drawSize);
            spriteBatch.DrawRectangle(drawArea, Color.Red, fill: true);
        }
    }
}
