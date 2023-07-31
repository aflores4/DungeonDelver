using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Text;

//System.Diagnostics.Debug.WriteLine();

namespace NotAnMMO
{    
    class PlayerController
    {
        private Texture2D[] player;
        private Texture2D cursor;

        // Class Keyboard State
        private KeyboardState kState;

        // Class Mouse State
        private MouseState mState;

        // Keyboard input bools
        bool w = false;
        bool a = false;
        bool s = false;
        bool d = false;
        float wTimer = 0;
        float aTimer = 0;
        float sTimer = 0;
        float dTimer = 0;

        // Player Position
        //public Vector2 position = new Vector2(400, 350);
        public Vector2 position = new Vector2(0, 0);
        private Vector2 tempPosition = new Vector2(0, 0); // temporary position used for collision handling
        private Vector2 playerDim = new Vector2(64/2, 64/2);
        private float angleOfAttack = 1.571f;
        private float rotationOffset = 1.571f;

        // Mouse Position
        private Vector2 mousePosition = new Vector2(0, 0);

        // Mouse Vars
        private bool attacking = false;
        private Vector2 cursorDim = new Vector2(32/2, 32/2);
        private Vector2 cursorCenter = new Vector2(0, 0);

        // Movement Vars
        private Vector2 inputDirection = new Vector2(0, 0);

        // Walking Vars
        private bool moving = false;
        //private float walk = 50f;
        private float walk = 5.5f;

        // Sprinting Vars
        private bool sprinting = false;
        private float sprint = 1f;

        // Rolling Vars
        private bool rolling = false;
        private float roll = 1f;
        private int rollTimer = 0;
        private float rollCoolDown = 4f;

        // Player collision circle
        public Collider playerCollider = new Collider(true, new Vector2(0, 0), 32, 0, 0);


        // Loaded Tiles
        private HashSet<Tile> loadedTiles = new HashSet<Tile>();
        public HashSet<Tile> viewableTiles = new HashSet<Tile>();
        public HashSet<Tile> viewableBackTiles = new HashSet<Tile>();


        // Loaded Positions
        private HashSet<Vector2> loadedPositions = new HashSet<Vector2>();
        private HashSet<Vector2> loadedBackPositions = new HashSet<Vector2>();
        private HashSet<Vector2> viewablePositions = new HashSet<Vector2>();
        private HashSet<Vector2> viewableBackPositions = new HashSet<Vector2>();
        private mapSection[,] loadedMapSections;
        private HashSet<mapSection> viewableMapSections = new HashSet<mapSection>();


        // Maps Collisions
        private HashSet<Collider> colliders = new HashSet<Collider>();

        public Rectangle playerView;
        private Vector2 oldPlayerPosition = new Vector2(0, 0);
        
        public void Load(SpriteBatch _spriteBatch, Microsoft.Xna.Framework.Content.ContentManager Content, Sprite spriteWorker)
        {
            player = spriteWorker.cut(Content.Load<Texture2D>("Player/Player"), 64, 64);

            cursor = Content.Load<Texture2D>("Player/Cursor");
        }

        public void LoadCollider(Tile tile)
        {
            colliders.Add(new Collider(false, new Vector2(tile.tileContainer.X, tile.tileContainer.Y), 0, tile.tileContainer.Width, tile.tileContainer.Height));
        }

        public void UpdateLoaded(mapSection[,] sections)
        {
            loadedMapSections = sections;
        }

        public void Update(GameTime gameTime, GraphicsDevice graphics, List<Tilemap> Maps, Camera camera, GraphicsDeviceManager _graphics)
        {
            camera.Update(position, graphics);

            // Gets Keyboard State
            kState = Keyboard.GetState();

            // Gets Mouse State
            mState = Mouse.GetState();

            // Updates cursor's center
            cursorCenter = mousePosition + cursorDim;

            // Runs input check to update input/Movement values
            inputCheck(gameTime, camera);

            // Updates the player's collider with the player's position
            playerCollider.Position = position;

            // Updates Player movement based on input values
            movement(Maps);
            
            // Updates player's view
            view(graphics, camera);

            // Runs attack handling when player is attacking
            if (attacking) {attack();}
        }

        public void Draw(GameTime gameTime, SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(player[0], position, null, Color.White, (angleOfAttack - rotationOffset), playerDim, 1, SpriteEffects.None, 0);

            //draws cursor
            _spriteBatch.Draw(cursor, mousePosition, Color.White);
        }

        /**
         * Updates Keyboard input values as well as mouse values, as well as other misc values
         */
        private void inputCheck(GameTime gameTime, Camera camera)
        {
            if (kState.IsKeyDown(Keys.N))
            {
                camera.Zoom = .044f;
                //camera.Zoom = .024f;
            }
            else if (kState.IsKeyDown(Keys.M))
            {
                camera.Zoom = 0.010f;
            }
            else
            {
                camera.Zoom = .5f;
            }

            // Keyboard Values:
            // Rolling
            if (kState.IsKeyDown(Keys.Space) && rollTimer == 0) { rolling = true; rollTimer = 1; }
            else { rolling = false; }

            // Moving Up
            if (kState.IsKeyDown(Keys.W) && !rolling) { w = true; wTimer += 1; }
            else { w = false; wTimer = 0; }

            // Moving Left
            if (kState.IsKeyDown(Keys.A) && !rolling) { a = true; aTimer += 1; }
            else { a = false; aTimer = 0; }

            // Moving Down
            if (kState.IsKeyDown(Keys.S) && !rolling) { s = true; sTimer += 1; }
            else { s = false; sTimer = 0; }

            // Moving Right
            if (kState.IsKeyDown(Keys.D) && !rolling) { d = true; dTimer += 1; }
            else { d = false; dTimer = 0; }

            // Sprinting
            if (kState.IsKeyDown(Keys.LeftShift)) { sprint = 2; }
            else { sprint = 1; }

            // Checks to see if player is moving both left and right
            if (a && d)
            {
                // Uses timers to see direction the player moved in last
                if (aTimer > dTimer)
                {
                    a = false;
                }
                else if (dTimer > aTimer)
                {
                    d = false;
                }
            }

            // Checks to see if player is moving both up and down
            if (w && s)
            {
                // Uses timers to see direction the player moved in last
                if (wTimer > sTimer)
                {
                    w = false;
                }
                else if (sTimer > wTimer)
                {
                    s = false;
                }
            }


            // Checks if Moving
            if (w || a || s || d)
            {
                moving = true;
            }
            else
            {
                moving = false;
            }

            
            // Handles Rolling speed, and rolling length
            if (rollTimer > 0)
            {
                if (rollTimer < 35)
                {
                    rollTimer += 1;
                    roll = 3.4f;
                    attacking = false;
                    sprint = 1;
                }
                else
                {
                    rollTimer = -1;
                    roll = 1f;
                }
            }

            // Roll cooldown, and checks if player is not holding down space
            if (rollTimer == -1)
            {
                rollCoolDown -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (!kState.IsKeyDown(Keys.Space) && rollCoolDown < 0)
                {
                    rollTimer = 0;
                    rollCoolDown = 4;
                }
            }

            // Checks if attacking
            if (mState.LeftButton == ButtonState.Pressed)
            {
                attacking = true;
            }
            else
            {
                attacking = false;
            }


            // Calculates the offset of the mouse position in relation to the camera position/size
            var offsetX = position.X - (camera.CameraView.Width / camera.Zoom) / 2;
            var offsetY = position.Y - (camera.CameraView.Height / camera.Zoom) / 2;

            // Updates mouse position based on the calculated offset
            mousePosition.X = (mState.X/0.8f) + (float)offsetX;
            mousePosition.Y = (mState.Y/0.8f) + (float)offsetY;
        }

        /**
         * Updates player movement based on keyboard input bool values
         */
        private void movement(List<Tilemap> Maps)
        {
            // Up
            if (w) { inputDirection.Y = -1; }

            // Left
            if (a) { inputDirection.X = -1; }

            // Down
            if (s) { inputDirection.Y = 1; }

            // Right
            if (d) { inputDirection.X = 1; }

            // Checks to see if there is no movement
            if (!w && !s) { inputDirection.Y = 0; }
            if (!a && !d) { inputDirection.X = 0; }

            // Normalizes the direction of movement, will make values, NaN if they are 0
            inputDirection.Normalize();

            // Checks to see if inputDirection values NaN after Normalization, if they are, set them back to 0
            if (float.IsNaN(inputDirection.X)) { inputDirection.X = 0f; }
            if (float.IsNaN(inputDirection.Y)) { inputDirection.Y = 0f; }

            // Apply the movement direction and speed to the position
            mousePosition += (inputDirection * (walk * sprint * roll));
            tempPosition = position + (inputDirection * (walk * sprint * roll));

            collisionChecking(colliders);
        }


        /**
         * Updates the tiles that the player can view, and will draw
         */
        private void view(GraphicsDevice graphics, Camera camera)
        {
            // Updates the view when the player moves
            if (position != oldPlayerPosition)
            {
                oldPlayerPosition = position;


                
                playerView = new Rectangle(
                    (int)((((position.X / 64) * 64) - (((graphics.Viewport.Width / 2) / camera.Zoom))) - (1 * 64)),     // X
                    (int)((((position.Y / 64) * 64) - (((graphics.Viewport.Height / 2) / camera.Zoom))) - (1 * 64)),    // Y
                    (int)((graphics.Viewport.Width / camera.Zoom) + (2 * 64)),                                         // Width
                    (int)(graphics.Viewport.Height / camera.Zoom) + (2 * 64));                                         // Height
                


                // Clears the viewable tiles
                viewableTiles.Clear();

                var x = 0;
                var y = 0;

                var countX = -1;
                var countY = -1;

                while (countX <= 1)
                {
                    x = ((int)((position.X + (countX * loadedMapSections[0, 0].sectionContainer.Width)) / loadedMapSections[0, 0].sectionContainer.Width) * loadedMapSections[0, 0].sectionContainer.Width) / loadedMapSections[0, 0].sectionContainer.Width;

                    while (countY <= 1)
                    {
                        y = ((int)((position.Y + (countY * loadedMapSections[0, 0].sectionContainer.Height)) / loadedMapSections[0, 0].sectionContainer.Height) * loadedMapSections[0, 0].sectionContainer.Height) / loadedMapSections[0, 0].sectionContainer.Height;


                        if (x >= 0 && y >= 0 && x < loadedMapSections.GetLength(0) && y < loadedMapSections.GetLength(1))
                            viewableTiles.UnionWith(loadedMapSections[x, y].sectionTiles);

                        countY += 1;
                    }

                    countY = -1;
                    countX += 1;
                }
            }
        }

        private void collisionChecking(HashSet<Collider> collisions)
        {
            foreach (var c in collisions)
                // Check collision between the new position and the rectangle
                if (playerCollider.collision(tempPosition, c.Rectangle))
                {
                    // Adjust the new position based on collision resolution
                    tempPosition = playerCollider.ResolveCollision(tempPosition, c.Rectangle);
                }

            // Update the position only if it's different from the current position
            if (tempPosition != position)
            {
                position = tempPosition;
            }
        }

        /**
         * Updates player movement based on keyboard input bool values
         */
        private void attack()
        {
            rotationOffset = -1.571f;

            // Calculate the angle between the player and the mouse and converts the angle from radians to degrees.
            angleOfAttack = (float)Math.Atan2(position.Y - (mousePosition.Y + cursorDim.Y), position.X - (mousePosition.X + cursorDim.X));
        }
    }
}