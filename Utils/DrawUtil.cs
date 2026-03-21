using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria.ModLoader;

namespace SpeedrunTimer.Utils
{
    public static partial class SpeedrunUtil
    {
        public static Asset<Texture2D> MagicPixel { get; set; } = ModContent.Request<Texture2D>("Terraria/Images/MagicPixel");

        /// <summary>
        /// Centers a rectangle on a give point.
        /// </summary>
        public static Rectangle CenteredRectangle(Vector2 center, Vector2 size)
        {
            size = new(Math.Abs(size.X), Math.Abs(size.Y));
            return new((int)center.X - ((int)size.X / 2), (int)center.Y - ((int)size.Y / 2), (int)size.X, (int)size.Y);
        }

        /// <summary>
        /// Draws a simple rectangle to the spritebatch.
        /// </summary>
        public static void DrawRectangle(this SpriteBatch spriteBatch, Rectangle rectangle, Color color, float stroke = 2f, bool fill = false)
        {
            Texture2D pixel = MagicPixel.Value;

            if (fill)
            {
                spriteBatch.Draw(pixel, rectangle, color);
                return;
            }

            int halfStroke = (int)Math.Ceiling(stroke * 0.5f);
            spriteBatch.Draw(pixel, new Rectangle(rectangle.Left - halfStroke, rectangle.Top - halfStroke, rectangle.Width + (int)stroke, (int)stroke), color);
            spriteBatch.Draw(pixel, new Rectangle(rectangle.Left - halfStroke, rectangle.Top - halfStroke, (int)stroke, rectangle.Height + (int)stroke), color);
            spriteBatch.Draw(pixel, new Rectangle(rectangle.Left - halfStroke, rectangle.Bottom - halfStroke, rectangle.Width + (int)stroke, (int)stroke), color);
            spriteBatch.Draw(pixel, new Rectangle(rectangle.Right - halfStroke, rectangle.Top - halfStroke, (int)stroke, rectangle.Height + (int)stroke), color);
        }
    }
}
