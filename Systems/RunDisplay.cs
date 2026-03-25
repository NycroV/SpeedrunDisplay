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
                "SpeedrunTimer: Timer Display", () => {
                    DrawSpeedrunTimer(Main.spriteBatch, Main.ScreenSize.ToVector2());
                    return true;
                }, InterfaceScaleType.None));
        }

        public static void DrawSpeedrunTimer(SpriteBatch spriteBatch, Vector2 screenSize)
        {
            Vector2 drawTopCenter = screenSize * SpeedrunConfig.Instance.SpeedrunUIPos;
            Vector2 drawSize = new Vector2(200f, 100f) * SpeedrunConfig.Instance.SpeedrunUIScale;
            int splits = SpeedrunConfig.Instance.SplitsToShow;

            Rectangle drawArea = CenteredRectangle(drawTopCenter + new Vector2(0f, drawSize.Y * 0.5f), drawSize);
            Vector2 splitSize = new(drawArea.Width, drawArea.Height * 0.375f);
            int splitsOffset = (int)float.Ceiling(splitSize.Y * splits);

            Rectangle titleBox = drawArea.CookieCutter(new(0f, -0.6f), new(0.95f, 0.3f));
            Rectangle igtBox = drawArea.CookieCutter(new(0.4f, 0.35f), new(0.55f, 0.55f));
            Rectangle rtaBox = drawArea.CookieCutter(new(-0.6f, 0.55f), new(0.35f, 0.35f));

            drawArea.Height += splitsOffset;
            igtBox.Y += splitsOffset;
            rtaBox.Y += splitsOffset;

            spriteBatch.DrawRectangle(drawArea, Color.Black * 0.45f, fill: true);
            spriteBatch.DrawRectangle(titleBox, Color.Red);
            spriteBatch.DrawRectangle(igtBox, Color.Lime);
            spriteBatch.DrawRectangle(rtaBox, Color.Yellow);

            if (splits == 0)
                return;

            Rectangle splitBox = titleBox.CookieCutter(new(0f, 2.5f), Vector2.One);
            spriteBatch.DrawRectangle(splitBox, Color.Blue);

            for (int i = 1; i < splits; i++)
            {
                splitBox = splitBox.CookieCutter(new(0f, 2.5f), Vector2.One);
                spriteBatch.DrawRectangle(splitBox, Color.Blue);
            }
        }
    }
}
