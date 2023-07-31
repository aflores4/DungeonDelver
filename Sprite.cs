using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Text;

namespace NotAnMMO
{
    class Sprite
    {
        public Texture2D[] cut(Texture2D spriteSheet, int h, int w)
        {
            int spritesPerRow = spriteSheet.Width / w;
            int spritesPerColumn = spriteSheet.Height / h;
            int spriteCount = spritesPerRow * spritesPerColumn;

            Texture2D[] sprites = new Texture2D[spriteCount];

            for (int row = 0; row < spritesPerColumn; row++)
            {
                for (int col = 0; col < spritesPerRow; col++)
                {
                    int index = row * spritesPerRow + col;
                    int x = col * w;
                    int y = row * h;

                    Color[] spriteData = new Color[w * h];
                    spriteSheet.GetData(0, new Rectangle(x, y, w, h), spriteData, 0, spriteData.Length);

                    bool isSpriteEmpty = true;
                    for (int i = 0; i < spriteData.Length; i++)
                    {
                        if (spriteData[i].A != 0)
                        {
                            isSpriteEmpty = false;
                            break;
                        }
                    }

                    if (!isSpriteEmpty)
                    {
                        Texture2D sprite = new Texture2D(spriteSheet.GraphicsDevice, w, h);
                        sprite.SetData(spriteData);
                        sprites[index] = sprite;
                    }
                }
            }

            return sprites;
        }

        public void animate(GameTime gameTime, SpriteBatch spriteBatch, Texture2D[] frames, Vector2 position)
        {
            // Set the initial timer to zero
            float timer = 0f;

            // Draw each frame in sequence with a 2-second delay between each one
            for (int i = 0; i < frames.Length; i++)
            {
                // Draw the current frame
                spriteBatch.Draw(frames[i], position, Color.White);

                // Increment the timer by the time since the last frame
                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Wait for 2 seconds before drawing the next frame
                if (timer >= 2f)
                {
                    timer = 0f;
                    i--; // Repeat this frame again to create the delay
                }
            }
        }
    }
}
