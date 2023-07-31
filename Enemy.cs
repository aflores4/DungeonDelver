using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Text;

namespace NotAnMMO
{
    class Enemy
    {
        public Vector2 position = new Vector2(0, 0);
        public Vector2 tempPosition = new Vector2(0, 0);
        public Rectangle enemyContainer;
        public Rectangle patrolArea;

        public Random rand = new Random();

        /*
         * 0 = idle
         * 1 = left
         * 2 = right
         * 3 = up
         * 4 = down
         */
        private int action = 0;

        private int timer = 0;
        private int moveX = 0;
        private int moveY = 0;
        private bool collide = false;
        private bool moving = true;

        public String type;

        private bool attacking = false;

        public void Update(HashSet<Vector2> walls)
        {
            if (moving)
            {
                if (timer <= 0)
                {
                    //System.Diagnostics.Debug.WriteLine(position.X + ", " + position.Y);
                    timer = 120;

                    switch (rand.Next(1, 9))
                    {
                        case 1:
                            moveX = -2;
                            moveY = 0;
                            break;
                        case 2:
                            moveX = 2;
                            moveY = 0;
                            break;
                        case 3:
                            moveX = 0;
                            moveY = -2;
                            break;
                        case 4:
                            moveX = 0;
                            moveY = 2;
                            break;
                        default:
                            moveX = 0;
                            moveY = 0;
                            break;
                    }
                }
                else
                {
                    tempPosition.X = position.X + moveX;
                    tempPosition.Y = position.Y + moveY;

                    if (tempPosition.X < patrolArea.X ||
                        tempPosition.Y < patrolArea.Y ||
                        tempPosition.X + enemyContainer.Width > patrolArea.X + patrolArea.Width ||
                        tempPosition.Y + enemyContainer.Height > patrolArea.Y + patrolArea.Height)
                    {
                        collide = true;
                    }
                    else
                    {
                        tempPosition.X = (int)((position.X + moveX) / 64) * 64;
                        tempPosition.Y = (int)((position.Y + moveY) / 64) * 64;

                        if (walls.Contains(tempPosition))
                            collide = true;



                        tempPosition.X = (int)(((position.X + 64) + moveX) / 64) * 64;
                        tempPosition.Y = (int)((position.Y + moveY) / 64) * 64;

                        if (walls.Contains(tempPosition))
                            collide = true;


                        tempPosition.X = (int)((position.X + moveX) / 64) * 64;
                        tempPosition.Y = (int)(((position.Y + 64) + moveY) / 64) * 64;

                        if (walls.Contains(tempPosition))
                            collide = true;


                        tempPosition.X = (int)(((position.X + 64) + moveX) / 64) * 64;
                        tempPosition.Y = (int)(((position.Y + 64) + moveY) / 64) * 64;

                        if (walls.Contains(tempPosition))
                            collide = true;
                    }

                    if (!collide)
                    {

                        position.X += moveX;
                        position.Y += moveY;

                        timer -= 1;
                    }
                    else
                    {
                        //System.Diagnostics.Debug.WriteLine("COLLISION");
                        timer = 0;
                        collide = false;
                        //moving = false;
                    }
                }
            }
        }

        public void Draw(Texture2D texture, SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(texture, position, Color.White);
        }


        public Enemy(Rectangle container, String _type)
        {
            enemyContainer = container;

            position = new Vector2(container.X, container.Y);

            patrolArea = new Rectangle((int)(position.X - (64 * 4.5)), (int)(position.Y - (64 * 4.5)), 9 * 64, 9 * 64);

            type = _type;
        }
    }

    class EnemySpawn
    {
        public Rectangle roomContainer;

        public int tileCount = 0;

        //public HashSet<Enemy> enemies = new HashSet<Enemy>();

        public EnemySpawn(Vector2 position, int w, int h)
        {
            roomContainer = new Rectangle((int)position.X, (int)position.Y, w, h);
        }
    }
}
