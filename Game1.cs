using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Text;

using TiledSharp;

namespace NotAnMMO
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Debug int
        public int debug = 0;

        // Player Controller
        private PlayerController player = new PlayerController();

        // Loader object that is used to load in all textures
        private Loader loader = new Loader();

        // List of Maps
        private List<Tilemap> maps = new List<Tilemap>();

        // List of loaded Maps
        private HashSet<Vector2> loadedWalls = new HashSet<Vector2>();

        // Sprite Worker object
        private Sprite spriteWorker = new Sprite();

        /* Random walk
        private Dungeon dungeonGenerator = new Dungeon();
        private HashSet<Vector2> dungeonFloors = new HashSet<Vector2>();
        private HashSet<Vector2> dungeonWalls = new HashSet<Vector2>();
        private List<Rectangle> floors = new List<Rectangle>();
        private List<Rectangle> walls = new List<Rectangle>();
        */

        private Dungeon dungeon;

        private MISC myLevel;
        private LevelLoader levelLoader = new LevelLoader();


        private Texture2D bobby;


        private Texture2D green;
        private Texture2D blue;

        private Texture2D black;
        private Texture2D white;
        private Texture2D grey;

        private Texture2D red;
        private Texture2D brown;
        private Texture2D gold;
        private Texture2D orange;

        private Texture2D brightRed;



        private Texture2D back;
        private Texture2D floor;
        private Texture2D topWall;
        private Texture2D bottomWall;
        private Texture2D leftWall;
        private Texture2D rightWall;
        private Texture2D longWall;
        private Texture2D longRightCornerWall;
        private Texture2D longLeftCornerWall;
        private Texture2D topLeftCorner;
        private Texture2D topRightCorner;
        private Texture2D bottomLeftCorner;
        private Texture2D bottomRightCorner;
        private Texture2D innerTopLeftCorner;
        private Texture2D innerTopRightCorner;
        private Texture2D innerBottomLeftCorner;
        private Texture2D innerBottomRightCorner;




        // Camera object
        public Camera camera;

        /**
         * Game Constructor
         */
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }


        /**
         * Initializes graphics
         */
        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = (int)(1920 * 1.5);
            _graphics.PreferredBackBufferHeight = (int)(1080 * 1.5);
            _graphics.ApplyChanges();

            base.Initialize();
        }


        /**
         * Loads asset content
         */
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Loads player textures
            player.Load(_spriteBatch, Content, spriteWorker);

            // Loads enemy textures
            loader.LoadEnemies(_spriteBatch, Content, spriteWorker);

            // Loads map textures
            loader.LoadMaps(_spriteBatch, Content, spriteWorker, maps);

            camera = new Camera(GraphicsDevice.Viewport);

            bobby = Content.Load<Texture2D>("Player/Bobby");

            green = CreateColoredTexture(GraphicsDevice, Color.Green, 64, 64);
            blue = CreateColoredTexture(GraphicsDevice, Color.Blue, 64, 64);

            white = CreateColoredTexture(GraphicsDevice, Color.White, 64, 64);
            black = CreateColoredTexture(GraphicsDevice, Color.Black, 128, 128);
            grey = CreateColoredTexture(GraphicsDevice, Color.DarkSlateGray, 64, 64);

            red = CreateColoredTexture(GraphicsDevice, Color.DarkRed, 64, 64);
            brown = CreateColoredTexture(GraphicsDevice, Color.SandyBrown, 64, 64);
            gold = CreateColoredTexture(GraphicsDevice, Color.Gold, 64, 64);
            orange = CreateColoredTexture(GraphicsDevice, Color.OrangeRed, 64, 64);

            //back = Content.Load<Texture2D>("BasicArt/rockyBack");
            floor = Content.Load<Texture2D>("BasicArt/rockyFloor");
            topWall = Content.Load<Texture2D>("BasicArt/rockyTopWall");
            bottomWall = Content.Load<Texture2D>("BasicArt/rockyBottomWall");
            leftWall = Content.Load<Texture2D>("BasicArt/rockyLeftWall");
            rightWall = Content.Load<Texture2D>("BasicArt/rockyRightWall");
            longWall = Content.Load<Texture2D>("BasicArt/rockyLongWall");
            longRightCornerWall = Content.Load<Texture2D>("BasicArt/rockyLongRightCornerWall");
            longLeftCornerWall = Content.Load<Texture2D>("BasicArt/rockyLongLeftCornerWall");
            topLeftCorner = Content.Load<Texture2D>("BasicArt/rockyTopLeftCorner");
            topRightCorner = Content.Load<Texture2D>("BasicArt/rockyTopRightCorner");
            bottomLeftCorner = Content.Load<Texture2D>("BasicArt/rockyBottomLeftCorner");
            bottomRightCorner = Content.Load<Texture2D>("BasicArt/rockyBottomRightCorner");
            innerTopLeftCorner = Content.Load<Texture2D>("BasicArt/rockyInnerTopLeftCorner");
            innerTopRightCorner = Content.Load<Texture2D>("BasicArt/rockyInnerTopRightCorner");
            innerBottomLeftCorner = Content.Load<Texture2D>("BasicArt/rockyInnerBottomLeftCorner");
            innerBottomRightCorner = Content.Load<Texture2D>("BasicArt/rockyInnerBottomRightCorner");



            dungeon = new Dungeon(6400, 6400, player, loadedWalls, _graphics.GraphicsDevice, camera);

            //levelLoader.Load(Content);
            //myLevel = new Level(levelLoader);

        }


        /**
         * Updates the game
         */
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            player.Update(gameTime, GraphicsDevice, maps, camera, _graphics);

            var looper = 0;
            foreach (var e in dungeon.enemies)
            {
                //looper += 1;
                e.Update(loadedWalls);
                if (looper > 0) break;
            }


            base.Update(gameTime);
        }


        /**
         * Draws assets
         */
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.Transform);


            var cameraW = (camera.CameraView.Width / 0.8);
            var cameraH = (camera.CameraView.Height / 0.8);


            /*foreach (var r in dungeon.mapSections)
            {
                _spriteBatch.Draw(gold, r.sectionContainer, Color.White);

            }*/
            _spriteBatch.Draw(green, dungeon.dungeonContainer, Color.White);



            //_spriteBatch.Draw(orange, dungeon.debugRectangle, Color.White);


            foreach (var d in dungeon.dungeonTiles)
            {
                _spriteBatch.Draw(gold, d.tileContainer, Color.White);
            }



            foreach (var d in player.viewableTiles)
            {
                if (d.type == "floor")
                    _spriteBatch.Draw(floor, d.tileContainer, Color.White);

                

                else if (d.type == "topWall")
                    _spriteBatch.Draw(topWall, d.tileContainer, Color.White);
                else if (d.type == "bottomWall")
                    _spriteBatch.Draw(bottomWall, d.tileContainer, Color.White);
                else if (d.type == "leftWall")
                    _spriteBatch.Draw(blue, d.tileContainer, Color.White);
                else if (d.type == "rightWall")
                    _spriteBatch.Draw(red, d.tileContainer, Color.White);

                else if (d.type == "longWall")
                    _spriteBatch.Draw(longWall, d.tileContainer, Color.White);
                else if (d.type == "longRightCornerWall")
                    _spriteBatch.Draw(longRightCornerWall, d.tileContainer, Color.White);
                else if (d.type == "longLeftCornerWall")
                    _spriteBatch.Draw(longLeftCornerWall, d.tileContainer, Color.White);

                else if (d.type == "topRightCorner")
                    _spriteBatch.Draw(topRightCorner, d.tileContainer, Color.White);
                else if (d.type == "topLeftCorner")
                    _spriteBatch.Draw(topLeftCorner, d.tileContainer, Color.White);
                else if (d.type == "bottomRightCorner")
                    _spriteBatch.Draw(bottomRightCorner, d.tileContainer, Color.White);
                else if (d.type == "bottomLeftCorner")
                    _spriteBatch.Draw(bottomLeftCorner, d.tileContainer, Color.White);
                

                else if (d.type == "innerTopRightCorner")
                    _spriteBatch.Draw(innerTopRightCorner, d.tileContainer, Color.White);
                else if (d.type == "innerTopLeftCorner")
                    _spriteBatch.Draw(innerTopLeftCorner, d.tileContainer, Color.White);
                else if (d.type == "innerBottomRightCorner")
                    _spriteBatch.Draw(innerBottomRightCorner, d.tileContainer, Color.White);
                else if (d.type == "innerBottomLeftCorner")
                    _spriteBatch.Draw(innerBottomLeftCorner, d.tileContainer, Color.White);
            }

            /*_spriteBatch.Draw(white, dungeon.dungeonContainer, Color.White);
            
            foreach (var r in dungeon.debugTiles)
            {
                if (r.type == "tree")
                    _spriteBatch.Draw(blue, r.tileContainer, Color.White);
            }

            foreach (var r in dungeon.debugTiles)
            {
                if (r.type == "wall")
                    _spriteBatch.Draw(red, r.tileContainer, Color.White);
            }*/
                
                foreach (var r in dungeon.debugTiles)
                {
                    //if (r.type == "wall")
                    //    _spriteBatch.Draw(red, r.tileContainer, Color.White);

                    if (r.type == "enemy")
                        _spriteBatch.Draw(bobby, r.tileContainer, Color.White);
                    if (r.type == "small")
                        _spriteBatch.Draw(grey, r.tileContainer, Color.White);
                    if (r.type == "medium")
                        _spriteBatch.Draw(orange, r.tileContainer, Color.White);
                    if (r.type == "large")
                        _spriteBatch.Draw(brown, r.tileContainer, Color.White);
                    if (r.type == "boss")
                        _spriteBatch.Draw(blue, r.tileContainer, Color.White);

                    /*else if (r.type == "small")
                        _spriteBatch.Draw(red, r.tileContainer, Color.White);
                    else if (r.type == "medium")
                        _spriteBatch.Draw(orange, r.tileContainer, Color.White);
                    else if (r.type == "large")
                        _spriteBatch.Draw(brown, r.tileContainer, Color.White);
                    else if (r.type == "boss")
                        _spriteBatch.Draw(black, r.tileContainer, Color.White);*/

                }



                /*foreach (var d in player.viewableTiles)
                {
                    if (d.type == "back")
                        _spriteBatch.Draw(black, d.tileContainer, Color.White);
                }


                foreach (var r in dungeon.enemyRectangles)
                {
                    _spriteBatch.Draw(gold, r, Color.White);
                }


                foreach (var r in dungeon.dungeonTiles)
                {
                    if (r.type == "spawn")
                        _spriteBatch.Draw(gold, r.tileContainer, Color.White);
                    else if (r.type == "smallStruct")
                        _spriteBatch.Draw(red, r.tileContainer, Color.White);
                    else if (r.type == "mediumStruct")
                        _spriteBatch.Draw(orange, r.tileContainer, Color.White);
                    else if (r.type == "largeStruct")
                        _spriteBatch.Draw(brown, r.tileContainer, Color.White);
                    else if (r.type == "boss")
                        _spriteBatch.Draw(black, r.tileContainer, Color.White);
                }




                /*foreach (var r in dungeon.enemySpawns)
                {
                    _spriteBatch.Draw(green, r.roomContainer, Color.White);
                }

                //foreach (var d in dungeon.enemySpawns) { _spriteBatch.Draw(green, d.roomContainer, Color.White); }

                var loop = 0;
                foreach (var d in dungeon.enemies)
                {
                    //loop += 1;
                    //_spriteBatch.Draw(green, d.patrolArea, Color.White);
                    if (d.type == "melee")d.Draw(red, _spriteBatch);
                    if (d.type == "range")d.Draw(gold, _spriteBatch);
                    //_spriteBatch.Draw(red, d.enemyContainer, Color.White);
                    if (loop > 1) break;
                }*/





                player.Draw(gameTime, _spriteBatch);


            _spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }









        private Texture2D CreateColoredTexture(GraphicsDevice graphicsDevice, Color color, int width, int height)
        {
            Texture2D texture = new Texture2D(graphicsDevice, width, height);

            Color[] colorData = new Color[width * height];
            for (int i = 0; i < colorData.Length; i++)
            {
                colorData[i] = color;
            }

            texture.SetData(colorData);

            return texture;
        }

    }
}