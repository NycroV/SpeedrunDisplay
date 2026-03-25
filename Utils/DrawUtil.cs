using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
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

        /// <summary>
        /// Creates a new rectangle from a "cookie cutter" slice of another rectangle.<br/>
        /// <br/>
        /// <paramref name="center"/> is the position within the given rectangle that you want to center your new rectangle. <c>-1f</c> for the top/left, <c>1f</c> for the bottom/right.<br/>
        /// <paramref name="size"/> is the scale of your new rectangle, based on the size of the given rectangle. <c>1f</c> is the same size as the given rectangle, <c>0.5f</c> is half the size.<br/>
        /// <br/>
        /// Both <paramref name="center"/> and <paramref name="size"/> can be beyond the bounds of the given rectangle (ie. <paramref name="center"/> can be greater/less than <c>1f</c>/<c>-1f</c>, <paramref name="size"/> can be greater/less than <c>1f</c>/<c>0f</c>.
        /// </summary>
        public static Rectangle CookieCutter(this Rectangle rectangle, Vector2 center, Vector2 size)
        {
            Vector2 cookieCenter = rectangle.Center.ToVector2() + (center * rectangle.Size() * 0.5f);
            return CenteredRectangle(cookieCenter, size * rectangle.Size());
        }
    }
}
