using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Text;

using TiledSharp;

namespace NotAnMMO
{
    class Tilemap
    {
        // Vars for the Map
        private TmxMap map;
        private Texture2D tileset;

        // Var to see how many tiles wide the tileset is
        private int tilesetTilesWide;
        
        // Vars for the Tiles
        private int tileWidth;
        int tileHeight;

        // List of colliders for the collision rectangles in the map
        private List<Collider> collisions = new List<Collider>();

        // List of transitions for the collision rectangles in the map
        public List<Rectangle> transitions = new List<Rectangle>();


        // Vars to handle map transform when drawing the map
        private Rectangle mapFrame = new Rectangle();
        public Vector2 position = new Vector2();

        public float rotation;
        public int xShift;
        public int yShift;
        




        /**
         * Constructor for a Tilemap. Sets the class vars, as well as pulls the collision rectangles and adds them to the collisions list
         */
        public Tilemap(TmxMap _map, Texture2D _tileset)
        {
            map = _map;
            tileset = _tileset;
            tileWidth = _map.Tilesets[0].TileWidth;
            tileHeight = _map.Tilesets[0].TileHeight;
            tilesetTilesWide = _tileset.Width / tileWidth;

            mapFrame = new Rectangle(0, 0, tileset.Width, tileset.Height);
            position = new Vector2(0, 0);

            rotation = 0;
            xShift = 0;
            yShift = 0;

            

            
            /*foreach (var o in map.ObjectGroups["Collisions"].Objects)
            {
                // Creates a collider for each of the collision rectangles
                collisions.Add(new Collider(false, new Vector2((float)o.X, (float)o.Y), 0, (int)o.Height, (int)o.Width));
            }*/

            
            foreach (var o in map.ObjectGroups["Transitions"].Objects)
            {
                // Creates a collider for each of the collision rectangles
                transitions.Add(new Rectangle((int)o.X, (int)o.Y, (int)o.Height, (int)o.Width));
            }
        }

        /**
         * Getter for the list of colliders for this map
         */
        public List<Collider> Collisions
        {
            get { return collisions; }
        }


        /**
         * Draws the Map by looping through each layer, and each tile in that layer
         */
        public void DrawTilemap(SpriteBatch _spriteBatch)
        {
            for (var i = 0; i < map.Layers.Count; i++)
            {
                for (var j = 0; j < map.Layers[i].Tiles.Count; j++)
                {
                    int gid = map.Layers[i].Tiles[j].Gid;
                    if (gid == 0)
                    {
                        //do nothing
                    }
                    else
                    {
                        int tileFrame = gid - 1;
                        int column = tileFrame % tilesetTilesWide;
                        int row = (int)Math.Floor((double)tileFrame / (double)tilesetTilesWide);
                        float x = (j % map.Width) * map.TileWidth;
                        float y = (float)Math.Floor(j / (double)map.Width) * map.TileHeight;
                        Rectangle tilesetRec = new Rectangle((tileWidth) * column, (tileHeight) * row, tileWidth, tileHeight);
                        _spriteBatch.Draw(tileset, new Rectangle((int)x, (int)y, tileWidth, tileHeight), tilesetRec, Color.White);
                    }
                }
            }
        }

        
        /**
         * Draws map based on the tileset (texture), only usable when tileset is whole map, and not parts that are then used to build the map
         */
        public void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(tileset, mapFrame, null, Color.White, rotation, new Vector2(tileset.Width/2, tileset.Height/2), SpriteEffects.None, 0) ;
        }
    }
}
