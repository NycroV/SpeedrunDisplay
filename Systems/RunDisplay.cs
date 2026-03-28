using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using ReLogic.Content;
using SpeedrunTimer.Configs;
using SpeedrunTimer.DataStructures;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace SpeedrunTimer.Systems
{
    public class RunDisplay : ModSystem
    {
        public static bool DisplayTimer { get; set; } = true;

        public static SpriteFont JetbrainsMono { get; set; } = ModContent.Request<SpriteFont>("SpeedrunTimer/Assets/Fonts/JetbrainsMono", AssetRequestMode.ImmediateLoad).Value;

        public override void OnModLoad() => IL_Main.DrawMenu += IL_Main_DrawMenu;

        private void IL_Main_DrawMenu(ILContext il)
        {
            try
            {
                ILCursor c = new(il);
                c.GotoNext(i => i.MatchCall("Terraria.ModLoader.UI.Interface", "AddMenuButtons"));
                c.Index++;
                c.EmitDelegate(DrawMenuRunButtons);
            }
            catch
            {
                MonoModHooks.DumpIL(ModContent.GetInstance<SpeedrunTimer>(), il);
            }
        }

        private void DrawMenuRunButtons()
        {
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
            Rectangle timerBox = drawArea.CookieCutter(new(0f, 0.375f), new Vector2(1f, 0.595f));
            Rectangle igtBox = drawArea.CookieCutter(new(0f, 0.15f), new(0.96f, 0.4f));
            Rectangle rtaBox = drawArea.CookieCutter(new(0f, 0.7f), new(0.96f, 0.25f));

            drawArea.Height += splitsOffset;
            timerBox.Y += splitsOffset;
            igtBox.Y += splitsOffset;
            rtaBox.Y += splitsOffset;

            spriteBatch.DrawRectangle(drawArea, Color.Black * 0.45f, fill: true);
            spriteBatch.DrawRectangle(titleBox.Scale(new Vector2(1.05f, 1.25f)), Color.Black * 0.35f, fill: true);
            spriteBatch.DrawRectangle(timerBox, Color.Black * 0.35f, fill: true);

            string runTitle = RunTracker.RunActive ? Language.GetTextValue(SpeedrunTimer.AllCategories[RunTracker.RunCategory].LocalizationKey) : "---";
            spriteBatch.DrawOutlinedStringInRectangle(titleBox, JetbrainsMono, Color.White, Color.Black, runTitle);

            TimeSpan igtTime = RunTracker.RunActive ? TimeSpan.FromSeconds(RunTracker.IGT_FrameCounter / 60f) : TimeSpan.Zero;
            string igt = $"{(igtTime.Hours > 0 ? $"{(int)igtTime.TotalHours}:" : "")}{(igtTime.Hours > 0 ? igtTime.ToString("mm") : igtTime.ToString("%m"))}:{igtTime:ss\\.fff}";
            spriteBatch.DrawOutlinedStringInRectangle(igtBox, JetbrainsMono, Color.White, Color.Black, igt, alignment: Utils.TextAlignment.Right);

            TimeSpan rtaTime = RunTracker.RunActive ? DateTime.UtcNow - RunTracker.RTA_RunStart : TimeSpan.Zero;
            string rta = $"{(rtaTime.Hours > 0 ? $"{(int)rtaTime.TotalHours}:" : "")}{(rtaTime.Hours > 0 ? rtaTime.ToString("mm") : rtaTime.ToString("%m"))}:{rtaTime:ss}";
            spriteBatch.DrawOutlinedStringInRectangle(rtaBox, JetbrainsMono, Color.DarkGray, Color.Black, rta, alignment: Utils.TextAlignment.Right);

            if (splits == 0)
                return;

            void DrawSplit(Rectangle box, RunSplit? runSplit)
            {
                Rectangle iconArea = box.CookieCutter(new(-0.85f, 0f), new(0.17f, 1f));
                Rectangle textArea = box.CookieCutter(new(-0.14f, 0f), new(0.5f, 1f));
                Rectangle timeArea = box.CookieCutter(new(0.7f, 0f), new(0.3f, 1f));

                //spriteBatch.DrawRectangle(iconArea, Color.Red);
                //spriteBatch.DrawRectangle(textArea, Color.Yellow);
                //spriteBatch.DrawRectangle(timeArea, Color.Lime);

                if (!runSplit.HasValue)
                {
                    spriteBatch.DrawOutlinedStringInRectangle(iconArea, JetbrainsMono, Color.DarkGray, Color.Black, "-", 1f);
                    spriteBatch.DrawOutlinedStringInRectangle(timeArea, JetbrainsMono, Color.DarkGray, Color.Black, "-:-:-", 1f, alignment: Utils.TextAlignment.Right);
                    return;
                }

                Split split = runSplit.Value.Split;
                Texture2D splitIcon = split.Icon.Value;
                float scale = float.Min((float)splitIcon.Width / iconArea.Width, (float)splitIcon.Height / iconArea.Height);

                string splitText = Language.GetTextValue(split.LocalizationKey);
                TimeSpan splitRunTime = TimeSpan.FromSeconds(runSplit.Value.RunTime / 60f);
                string splitTime = $"{(splitRunTime.Hours > 0 ? $"{(int)splitRunTime.TotalHours}:" : "")}{(splitRunTime.Hours > 0 ? splitRunTime.ToString("mm") : splitRunTime.ToString("%m"))}:{splitRunTime:ss}";

                spriteBatch.Draw(splitIcon, iconArea.Center.ToVector2(), null, Color.White, 0f, splitIcon.Size() * 0.5f, scale, SpriteEffects.None, 0f);
                spriteBatch.DrawOutlinedStringInRectangle(textArea, JetbrainsMono, Color.White, Color.Black, splitText, alignment: Utils.TextAlignment.Left);
                spriteBatch.DrawOutlinedStringInRectangle(timeArea, JetbrainsMono, Color.White, Color.Black, splitTime, 1f, alignment: Utils.TextAlignment.Right);
            }

            int splitCount = RunTracker.CurrentSplits?.Count ?? 0;
            Rectangle splitBox = titleBox.CookieCutter(new(0f, 2.5f), Vector2.One);
            RunSplit? split = splitCount > 0 ? RunTracker.CurrentSplits[^1] : null;
            DrawSplit(splitBox, split);

            for (int i = 1; i < splits; i++)
            {
                splitBox = splitBox.CookieCutter(new(0f, 2.5f), Vector2.One);
                split = splitCount > i ? RunTracker.CurrentSplits[^(i + 1)] : null;
                DrawSplit(splitBox, split);
            }
        }

    }
}
