using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Text;

using TiledSharp;

namespace NotAnMMO
{
    class Loader
    {
        private Texture2D[] enemy;


        /** 
         * Loads the textures for all Maps
         */
        public void LoadMaps(SpriteBatch _spriteBatch, Microsoft.Xna.Framework.Content.ContentManager Content, Sprite spriteWorker, List<Tilemap> Maps)
        {
            //Maps.Add(new Tilemap(new TmxMap("Content/WorldArt/Maps/TestDungeon.tmx"), Content.Load<Texture2D>("WorldArt/Tilesets/TestDungeon")));
            /*
            Maps.Add(new Tilemap(new TmxMap("Content/WorldArt/Maps/Entrance.tmx"), Content.Load<Texture2D>("WorldArt/Tilesets/Entrance")));
            Maps.Add(new Tilemap(new TmxMap("Content/WorldArt/Maps/Hallway1.tmx"), Content.Load<Texture2D>("WorldArt/Tilesets/Hallway1")));
            Maps.Add(new Tilemap(new TmxMap("Content/WorldArt/Maps/Hallway2.tmx"), Content.Load<Texture2D>("WorldArt/Tilesets/Hallway2")));
            Maps.Add(new Tilemap(new TmxMap("Content/WorldArt/Maps/Hallway3.tmx"), Content.Load<Texture2D>("WorldArt/Tilesets/Hallway3")));
            Maps.Add(new Tilemap(new TmxMap("Content/WorldArt/Maps/Hallway4.tmx"), Content.Load<Texture2D>("WorldArt/Tilesets/Hallway4")));
            Maps.Add(new Tilemap(new TmxMap("Content/WorldArt/Maps/Room1.tmx"), Content.Load<Texture2D>("WorldArt/Tilesets/Room1")));
            Maps.Add(new Tilemap(new TmxMap("Content/WorldArt/Maps/Room2.tmx"), Content.Load<Texture2D>("WorldArt/Tilesets/Room2")));
            Maps.Add(new Tilemap(new TmxMap("Content/WorldArt/Maps/Grand1.tmx"), Content.Load<Texture2D>("WorldArt/Tilesets/Grand1")));
            Maps.Add(new Tilemap(new TmxMap("Content/WorldArt/Maps/Boss1.tmx"), Content.Load<Texture2D>("WorldArt/Tilesets/Boss1")));
            */
        }

        /** 
         * Loads the textures for all Enemies
         */
        public void LoadEnemies(SpriteBatch _spriteBatch, Microsoft.Xna.Framework.Content.ContentManager Content, Sprite spriteWorker)
        {
            enemy = spriteWorker.cut(Content.Load<Texture2D>("Mobs/Enemies/Skeleton"), 64, 64);
        }

        /** 
         * Loads the textures for all Bullets
         */
        public void LoadBullets(SpriteBatch _spriteBatch, Microsoft.Xna.Framework.Content.ContentManager Content, Sprite spriteWorker)
        {
        }
    }
}
