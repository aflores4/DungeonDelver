using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Text;

namespace NotAnMMO
{
    class Bullet
    {
        private Vector2 position;
        private Vector2 velocity;
        private float speed;
        private double rotation;

        private int bulletHeight = 8;
        private int bulletWidth = 8;
        private Rectangle bulletBox;
        private Vector2 bulletBoxOrigin;

        public Bullet(Vector2 pos, int w, int h, double rot, float spe)
        {
            position.X = pos.X;
            position.Y = pos.Y;

            speed = spe;

            rotation = rot;
        }

        public void Update()
        {
            bulletBox = new Rectangle((int)position.X, (int)position.Y, bulletHeight, bulletWidth);
            bulletBoxOrigin.X = bulletWidth / 2f;
            bulletBoxOrigin.Y = bulletHeight / 2f;
            

            velocity = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation)) * speed;

            position += velocity;
        }

        public void Draw(GameTime gameTime, SpriteBatch _spriteBatch, Texture2D bullet)
        {
            _spriteBatch.Draw(bullet, bulletBox, null, Color.White, (float)(rotation + (Math.PI / 2)), bulletBoxOrigin, SpriteEffects.None, 0f);
        }
    }
}
