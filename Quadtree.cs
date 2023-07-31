using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace NotAnMMO
{
    class Quadtree
    {
        private bool debug = false;

        public Rectangle treeContainer = new Rectangle();

        private int spawnMax = 15;
        private int wallMax = 30;

        List<Rectangle> spawns = new List<Rectangle>();
        List<Rectangle> spawnIntersects = new List<Rectangle>();
        List<Rectangle> walls = new List<Rectangle>();

        private Quadtree main = null;
        
        private Quadtree nw = null;
        private Quadtree ne = null;
        private Quadtree sw = null;
        private Quadtree se = null;

        bool divided = false;

        string title = "main";

        public Quadtree(Rectangle container)
        {
            treeContainer = container;
        }

        public Quadtree(Rectangle container, string _title, Quadtree _main)
        {
            treeContainer = container;

            title = _title;

            main = _main;
        }


        public void insertSpawn(Vector2 newPosition, int width, int height, HashSet<Rectangle> badSpawns, Quadtree main)
        {
            Rectangle currentRectangle = new Rectangle((int)newPosition.X, (int)newPosition.Y, 64 * width, 64 * height);

            // Wall
            if (width == 1)
            {
                if (currentRectangle.Intersects(treeContainer))
                {
                    if (walls.Count < wallMax)
                    {
                        walls.Add(currentRectangle);
                    }
                    else
                    {
                        if (!divided)
                        {
                            subdivide(treeContainer, main);
                            divided = true;
                        }

                        nw.insertSpawn(newPosition, width, height, badSpawns, main);
                        ne.insertSpawn(newPosition, width, height, badSpawns, main);
                        sw.insertSpawn(newPosition, width, height, badSpawns, main);
                        se.insertSpawn(newPosition, width, height, badSpawns, main);
                    }
                }
                else
                {
                    return;
                }
            }
            // Spawn
            else
            {
                if (currentRectangle.Intersects(treeContainer))
                {
                    if (checkSpawnIntersect(currentRectangle))
                    {
                        return;
                    }

                    if (spawns.Count < spawnMax)
                    {
                        if (!main.checkWallIntersection(currentRectangle))
                        {
                            var padding = (int)(40 * Math.Pow(2, -width / 32));
                            currentRectangle = new Rectangle((int)newPosition.X - (64 * (padding / 2)), (int)newPosition.Y - (64 * (padding / 2)), 64 * width + (64 * padding), 64 * height + (64 * padding));
                            spawnIntersects.Add(currentRectangle);

                            currentRectangle = new Rectangle((int)newPosition.X, (int)newPosition.Y, 64 * width, 64 * height);
                            spawns.Add(currentRectangle);
                        }
                    }
                    else
                    {
                        if (!divided)
                        {
                            subdivide(treeContainer, main);
                            divided = true;
                        }

                        nw.insertSpawn(newPosition, width, height, badSpawns, main);
                        ne.insertSpawn(newPosition, width, height, badSpawns, main);
                        sw.insertSpawn(newPosition, width, height, badSpawns, main);
                        se.insertSpawn(newPosition, width, height, badSpawns, main);
                    }
                }
            }
        }


        public void subdivide(Rectangle container, Quadtree main)
        {
            var width = treeContainer.Width / 2;
            var height = treeContainer.Height / 2;

            var width2 = treeContainer.Width / 2;
            var height2 = treeContainer.Height / 2;

            if (width % 64 != 0)
            {
                width = width - 32;
                width2 = width2 + 32;
            }

            if (height % 64 != 0)
            {
                height = height - 32;
                height2 = height2 + 32;
            }

            nw = new Quadtree(new Rectangle(treeContainer.X, treeContainer.Y, width, height), "nw", main);

            ne = new Quadtree(new Rectangle(treeContainer.X + width, treeContainer.Y, width2, height), "ne", main);

            sw = new Quadtree(new Rectangle(treeContainer.X, treeContainer.Y + height, width, height2), "sw", main);

            se = new Quadtree(new Rectangle(treeContainer.X + width, treeContainer.Y + height, width2, height2), "se", main);

            if (debug) System.Diagnostics.Debug.WriteLine("DIVIDED");
        }


        private bool checkSpawnIntersect(Rectangle current)
        {
            if (current.Intersects(treeContainer))
            {
                foreach (var r in spawnIntersects)
                {
                    if (r.Intersects(current))
                    {
                        return true;
                    }
                }


                if (divided)
                {
                    return (
                    nw.checkSpawnIntersect(current) ||
                    ne.checkSpawnIntersect(current) ||
                    sw.checkSpawnIntersect(current) ||
                    se.checkSpawnIntersect(current));
                }

                return false;
            }
            else
            {
                return false;
            }
        }

        private bool checkWallIntersection(Rectangle current)
        {
            if (current.Intersects(treeContainer))
            {
                foreach (var r in walls)
                {
                    if (r.Intersects(current))
                    {
                        return true;
                    }
                }


                if (divided)
                {
                    return(
                    nw.checkWallIntersection(current) ||
                    ne.checkWallIntersection(current) ||
                    sw.checkWallIntersection(current) ||
                    se.checkWallIntersection(current));
                }

                return false;
            }
            else
            {
                return false;
            }
        }


        public void makeTiles(HashSet<Tile> tiles, HashSet<Rectangle> badSpawns, int es, int ss, int ms, int ls, int bs, HashSet<Rectangle> enemies, HashSet<Rectangle> smalls, HashSet<Rectangle> mediums, HashSet<Rectangle> larges, HashSet<Rectangle> bosses)
        {
            foreach (var r in spawns)
            {
                if (!badSpawns.Contains(r))
                {
                    if (r.Width == es * 64)
                    {
                        enemies.Add(r);
                        tiles.Add(new Tile(r, "enemy", false));
                    }
                    else if (r.Width == ss * 64)
                    {
                        smalls.Add(r);
                        tiles.Add(new Tile(r, "small", false));
                    }
                    else if (r.Width == ms * 64)
                    {
                        mediums.Add(r);
                        tiles.Add(new Tile(r, "medium", false));
                    }
                    else if (r.Width == ls * 64)
                    {
                        larges.Add(r);
                        tiles.Add(new Tile(r, "large", false));
                    }
                    else if (r.Width == bs * 64)
                    {
                        bosses.Add(r);
                        tiles.Add(new Tile(r, "boss", false));
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("ERROR");
                    }
                }
            }

            foreach (var r in walls)
            {
                tiles.Add(new Tile(new Vector2(r.X, r.Y), "wall", false));
            }

            if (divided)
            {
                nw.makeTiles(tiles, badSpawns, es, ss, ms, ls, bs, enemies, smalls, mediums, larges, bosses);
                ne.makeTiles(tiles, badSpawns, es, ss, ms, ls, bs, enemies, smalls, mediums, larges, bosses);
                sw.makeTiles(tiles, badSpawns, es, ss, ms, ls, bs, enemies, smalls, mediums, larges, bosses);
                se.makeTiles(tiles, badSpawns, es, ss, ms, ls, bs, enemies, smalls, mediums, larges, bosses);
            }
            else
            {
                tiles.Add(new Tile(new Rectangle(treeContainer.X + 32, treeContainer.Y + 32, treeContainer.Width - 64, treeContainer.Height - 64), "tree", false));
            }
        }
    }
}
