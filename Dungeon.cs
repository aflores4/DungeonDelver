using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;

using TiledSharp;

namespace NotAnMMO
{
    class Dungeon
    {
        private bool debug = true;
        private int percent = 0;

        public Rectangle debugRectangle = new Rectangle();

        private readonly Random random = new Random();

        private PlayerController player;

        private Vector2 playerPosition;

        public HashSet<Vector2> temp = new HashSet<Vector2>();

        public HashSet<Tile> dungeonTiles = new HashSet<Tile>();
        public HashSet<Tile> backTiles = new HashSet<Tile>();
        public HashSet<Tile> allLongWalls = new HashSet<Tile>();
        public HashSet<Tile> spawns = new HashSet<Tile>();


        // All Tile Positions
        public HashSet<Vector2> backPositions = new HashSet<Vector2>();
        public HashSet<Vector2> positions = new HashSet<Vector2>();
        public HashSet<Vector2> playerSpawnPositions = new HashSet<Vector2>();
        public HashSet<Vector2> floorPositions = new HashSet<Vector2>();
        public HashSet<Vector2> playerBossRoomPositions = new HashSet<Vector2>();
        public HashSet<Vector2> wallPositions = new HashSet<Vector2>();
        public HashSet<Vector2> cornerPositions = new HashSet<Vector2>();


        // Normal Walls
        public HashSet<Vector2> topWalls = new HashSet<Vector2>();
        public HashSet<Vector2> bottomWalls = new HashSet<Vector2>();


        // Long Walls
        public HashSet<Vector2> longWalls = new HashSet<Vector2>();
        public HashSet<Vector2> longRightCornerWalls = new HashSet<Vector2>();
        public HashSet<Vector2> longLeftCornerWalls = new HashSet<Vector2>();


        // Corner Walls
        public HashSet<Vector2> topLeftWalls = new HashSet<Vector2>();
        public HashSet<Vector2> topRightWalls = new HashSet<Vector2>();
        public HashSet<Vector2> bottomLeftWalls = new HashSet<Vector2>();
        public HashSet<Vector2> bottomRightWalls = new HashSet<Vector2>();


        // Inner Corner Wall
        public HashSet<Vector2> innerTopLeftWalls = new HashSet<Vector2>();
        public HashSet<Vector2> innerTopRightWalls = new HashSet<Vector2>();
        public HashSet<Vector2> innerBottomLeftWalls = new HashSet<Vector2>();
        public HashSet<Vector2> innerBottomRightWalls = new HashSet<Vector2>();


        // Map sctions
        //public HashSet<mapSection> mapSections = new HashSet<mapSection>();
        public mapSection[,] mapSections;
        private int mapSectionWidth = 0;
        private int mapSectionHeight = 0;



        // Least and most tile positions in dungeon
        private Vector2 least = new Vector2(0, 0);
        private Vector2 most = new Vector2(0, 0);


        // Player spawn anchors
        private Vector2 leftAnchor = new Vector2(0, 0);
        private Vector2 rightAnchor = new Vector2(0, 0);
        private Vector2 topAnchor = new Vector2(0, 0);
        private Vector2 bottomAnchor = new Vector2(0, 0);


        // Container for the player
        public Rectangle playerContainer;


        // Spawn positions
        public HashSet<Vector2> truePositions = new HashSet<Vector2>();
        public HashSet<Vector2> enemySpawns = new HashSet<Vector2>();
        public HashSet<Vector2> smallStructSpawns = new HashSet<Vector2>();
        public HashSet<Vector2> mediumStructSpawns = new HashSet<Vector2>();
        public HashSet<Vector2> largeStructSpawns = new HashSet<Vector2>();
        public HashSet<Vector2> bossSpawns = new HashSet<Vector2>();

        // Positions used to find correct spawn positions
        public HashSet<Vector2> noGoodPositionsE = new HashSet<Vector2>();
        public HashSet<Vector2> noGoodPositionsS = new HashSet<Vector2>();
        public HashSet<Vector2> noGoodPositionsM = new HashSet<Vector2>();
        public HashSet<Vector2> noGoodPositionsL = new HashSet<Vector2>();
        public HashSet<Vector2> noGoodPositionsB = new HashSet<Vector2>();






        // MAYBE

        public HashSet<Tile> debugTiles = new HashSet<Tile>();


        public HashSet<Enemy> enemies = new HashSet<Enemy>();
        public HashSet<Rectangle> enemyRectangles = new HashSet<Rectangle>();

        public Rectangle dungeonContainer;


        /**
         * Constructor for a Dungeon
         */
        public Dungeon(int width, int height, PlayerController _player, HashSet<Vector2> walls, GraphicsDevice graphics, Camera camera)
        {
            player = _player;
            dungeonContainer = new Rectangle(0, 0, width, height);
            GenerateDungeon(walls, graphics, camera);
        }


        /**
         * Creates enemySpawns, and adds each room to the dungeon's enemySpawns list, and splits the enemySpawns into tiles as well for the floor list
         */
        private void GenerateDungeon(HashSet<Vector2> walls, GraphicsDevice graphics, Camera camera)
        {
            var level = 100;

            // Max = 500,000, Min = 5,000
            var walk = (int)(level * 15.625 * 64);

            //var shrinker = (float)random.Next(10, 26) / 100;
            var shrinker = (float)40 / level;

            dungeonContainer = new Rectangle(0, 0, (int)((walk * shrinker) / 64) * 64, (int)((walk * shrinker) / 64) * 64);


            // Creates Map Sections
            createMapSections(graphics);

            // Random Walks to set the dungeon tile positions
            //positions = SimpleRandomWalk(dungeonContainer, walk);
            //positions = GeneratePointsInRectangle(dungeonContainer, 1000000, 10, 125);
            positions = GeneratePointsInRectangle(dungeonContainer, 1000000, 10, 125);
            System.Diagnostics.Debug.WriteLine("POSITIONS: " + positions.Count);

            var count = 0;
            foreach(var r in positions)
            {
                if (count == 0)
                {
                    player.position = r;
                }
                else
                {
                    break;
                }
                count++;
            }

            // Sets the least and most position while adjusting the side anchors
            least.X = (leftAnchor.X -= (64 * 2));
            least.Y = (topAnchor.Y -= (64 * 4));
            most.X = (rightAnchor.X += (64 * 2)) + 64;
            most.Y = (bottomAnchor.Y += (64 * 3)) + 64;


            // Sets the temporary dungeon container after adding the spawn room
            Rectangle tempDungeonContainer = new Rectangle((int)least.X, (int)least.Y, (int)most.X - (int)least.X, (int)most.Y - (int)least.Y);


            // Sets general wall positions
            setWalls(positions, wallPositions, walls, false);


            // Sets dungeon container to the appropriate container only fitting the dungeon tiles
            dungeonContainer = tempDungeonContainer;


            // Sets floor tiles
            setFloors();

            // Sets specifc wall positions
            setWalls(positions, wallPositions, walls, true);

            
            System.Diagnostics.Debug.WriteLine("Map Sections: " + mapSections.GetLength(0) + ", " + mapSections.GetLength(1) + "\n");


            // Creates spawns for enemies, and structures
            createSpawns();

            // Loads tiles/positions
            player.UpdateLoaded(mapSections);
        }


        private HashSet<Vector2> SimpleRandomWalk(Rectangle container, int walkLength)
        {
            HashSet<Vector2> path = new HashSet<Vector2>();

            var previousePosition = new Vector2(((container.X + (container.Width / 2)) / 64) * 64, ((container.Y + (container.Height / 2)) / 64) * 64);

            path.Add(previousePosition);

            player.position = previousePosition;

            var initial = 0;

            least = most = new Vector2(((container.X + (container.Width / 2)) / 64) * 64, ((container.Y + (container.Height / 2)) / 64) * 64);

            while (true)
            {
                for (int i = 0; i < walkLength; i++)
                {
                    var newPosition = previousePosition + Direction2D.RandomCardinalDirection(random);

                    if (newPosition.X - 128 >= container.X &&
                         newPosition.Y - 256 >= container.Y &&
                         newPosition.X + 192 <= container.X + container.Width &&
                         newPosition.Y + 256 <= container.Y + container.Height)
                    {
                        path.Add(newPosition);
                        mapSectionInsert(newPosition);
                        previousePosition = newPosition;
                    }
                    else
                    {
                        i--;
                    }

                    if (path.Count >= 100000)
                        break;
                }

                if (path.Count >= walkLength * 0.14 || path.Count >= 200000)
                {
                    break;
                }
            }

            System.Diagnostics.Debug.WriteLine(path.Count + "\n\n");

            truePositions.UnionWith(path);

            HashSet<Vector2> tempPath = new HashSet<Vector2>();
            tempPath.UnionWith(path);

            foreach (var r in tempPath)
            {
                // First layer
                path.Add(new Vector2((int)r.X + 64, (int)r.Y));
                path.Add(new Vector2((int)r.X - 64, (int)r.Y));
                path.Add(new Vector2((int)r.X, (int)r.Y + 64));
                path.Add(new Vector2((int)r.X, (int)r.Y - 64));
                path.Add(new Vector2((int)r.X + 64, (int)r.Y + 64));
                path.Add(new Vector2((int)r.X + 64, (int)r.Y - 64));
                path.Add(new Vector2((int)r.X - 64, (int)r.Y + 64));
                path.Add(new Vector2((int)r.X - 64, (int)r.Y - 64));


                // Second Layer
                path.Add(new Vector2((int)r.X - 128, (int)r.Y));
                path.Add(new Vector2((int)r.X - 128, (int)r.Y + 64));
                path.Add(new Vector2((int)r.X - 128, (int)r.Y + 128));
                path.Add(new Vector2((int)r.X - 128, (int)r.Y - 64));
                path.Add(new Vector2((int)r.X - 128, (int)r.Y - 128));
                path.Add(new Vector2((int)r.X - 64, (int)r.Y - 128));
                path.Add(new Vector2((int)r.X - 64, (int)r.Y + 128));
                path.Add(new Vector2((int)r.X, (int)r.Y - 128));
                path.Add(new Vector2((int)r.X, (int)r.Y + 128));
                path.Add(new Vector2((int)r.X + 64, (int)r.Y - 128));
                path.Add(new Vector2((int)r.X + 64, (int)r.Y + 128));
                path.Add(new Vector2((int)r.X + 128, (int)r.Y));
                path.Add(new Vector2((int)r.X + 128, (int)r.Y + 64));
                path.Add(new Vector2((int)r.X + 128, (int)r.Y + 128));
                path.Add(new Vector2((int)r.X + 128, (int)r.Y - 64));
                path.Add(new Vector2((int)r.X + 128, (int)r.Y - 128));


                // Additional top tiles
                path.Add(new Vector2((int)r.X - 128, (int)r.Y - 192));
                path.Add(new Vector2((int)r.X - 64, (int)r.Y - 192));
                path.Add(new Vector2((int)r.X, (int)r.Y - 192));
                path.Add(new Vector2((int)r.X + 64, (int)r.Y - 192));
                path.Add(new Vector2((int)r.X + 128, (int)r.Y - 192));
                // Additional top tiles
                path.Add(new Vector2((int)r.X - 128, (int)r.Y - 256));
                path.Add(new Vector2((int)r.X - 64, (int)r.Y - 256));
                path.Add(new Vector2((int)r.X, (int)r.Y - 256));
                path.Add(new Vector2((int)r.X + 64, (int)r.Y - 256));
                path.Add(new Vector2((int)r.X + 128, (int)r.Y - 256));

                // Additional bottom tiles
                path.Add(new Vector2((int)r.X - 128, (int)r.Y + 192));
                path.Add(new Vector2((int)r.X - 64, (int)r.Y + 192));
                path.Add(new Vector2((int)r.X, (int)r.Y + 192));
                path.Add(new Vector2((int)r.X + 64, (int)r.Y + 192));
                path.Add(new Vector2((int)r.X + 128, (int)r.Y + 192));

                if (r.X <= least.X)
                {
                    least.X = r.X;
                    leftAnchor = r;
                }

                if (r.Y <= least.Y)
                {
                    least.Y = r.Y;
                    topAnchor = r;
                }

                if (r.X >= most.X)
                {
                    most.X = r.X;
                    rightAnchor = r;
                }

                if (r.Y >= most.Y)
                {
                    most.Y = r.Y;
                    bottomAnchor = r;
                }

                if (initial == 0)
                {
                    playerPosition = new Vector2((int)r.X + 32, (int)r.Y + 32);
                    initial += 1;
                }
            }

            return path;
        }


        private void mapSectionInsert(Vector2 position)
        {
            var x = ((int)(position.X / mapSectionWidth) * mapSectionWidth) / mapSectionWidth;
            var y = ((int)(position.Y / mapSectionHeight) * mapSectionHeight) / mapSectionHeight;

            mapSections[x, y].positions.Add(position);
        }

        private void createMapSections(GraphicsDevice graphics)
        {
            mapSectionWidth = (int)(((graphics.Viewport.Width / 0.4) + 64) / 64) * 64;
            mapSectionHeight = (int)(((graphics.Viewport.Height / 0.4) + 64) / 64) * 64;



            mapSections = new mapSection[(dungeonContainer.Width / mapSectionWidth) + 1, (dungeonContainer.Height / mapSectionHeight) + 1];


            for (var x = 0; x < mapSections.GetLength(0); x++)
            {
                for (var y = 0; y < mapSections.GetLength(1); y++)
                {
                    mapSections[x, y] = new mapSection(new Rectangle(x * mapSectionWidth, y * mapSectionHeight, mapSectionWidth, mapSectionHeight));
                }
            }
        }


        private void setWalls(HashSet<Vector2> positions, HashSet<Vector2> wallPositions, HashSet<Vector2> walls, bool advanced)
        {
            // Sets specific kinds of tiles
            if (advanced)
            {
                Vector2 tempPosition = new Vector2();


                // Sets the Walls and the Corner tiles
                foreach (var r in wallPositions)
                {
                    var inserted = false;

                    int wallCount = 0;

                    if (wallPositions.Contains(new Vector2(r.X - 64, r.Y)))
                        wallCount += 1;

                    if (wallPositions.Contains(new Vector2(r.X + 64, r.Y)))
                        wallCount += 1;

                    if (wallPositions.Contains(new Vector2(r.X, r.Y + 64)))
                        wallCount += 1;

                    if (wallPositions.Contains(new Vector2(r.X, r.Y - 64)))
                        wallCount += 1;


                    if (wallCount == 3 &&
                        (!positions.Contains(new Vector2(r.X + 64, r.Y)) ||
                         !positions.Contains(new Vector2(r.X - 64, r.Y)) ||
                         !positions.Contains(new Vector2(r.X, r.Y + 64)) ||
                         !positions.Contains(new Vector2(r.X, r.Y - 64))))
                    {

                        player.LoadCollider(createTile(r, "rightWall", true));
                        inserted = true;
                    }
                    else {

                        // Top Left
                        if (!positions.Contains(new Vector2(r.X - 64, r.Y - 64)))
                        {
                            if (wallPositions.Contains(new Vector2(r.X, r.Y - 64)) && wallPositions.Contains(new Vector2(r.X - 64, r.Y)))
                            {
                                inserted = true;

                                player.LoadCollider(createTile(r, "bottomRightCorner", true));
                                player.LoadCollider(createTile(new Vector2(r.X, r.Y + 64), "longRightCornerWall", true));
                                player.LoadCollider(createTile(new Vector2(r.X, r.Y + 64 * 2), "longRightCornerWall", true));
                            }
                            else if (wallPositions.Contains(new Vector2(r.X, r.Y + 64)) && wallPositions.Contains(new Vector2(r.X + 64, r.Y)))
                            {
                                inserted = true;

                                player.LoadCollider(createTile(r, "innerBottomRightCorner", true));
                            }
                        }

                        // Top Right
                        if (!inserted && !positions.Contains(new Vector2(r.X + 64, r.Y - 64)))
                        {
                            if (wallPositions.Contains(new Vector2(r.X, r.Y - 64)) && wallPositions.Contains(new Vector2(r.X + 64, r.Y)))
                            {
                                inserted = true;

                                player.LoadCollider(createTile(r, "bottomLeftCorner", true));
                                player.LoadCollider(createTile(new Vector2(r.X, r.Y + 64), "longLeftCornerWall", true));
                                player.LoadCollider(createTile(new Vector2(r.X, r.Y + 64 * 2), "longLeftCornerWall", true));
                            }
                            else if (wallPositions.Contains(new Vector2(r.X, r.Y + 64)) && wallPositions.Contains(new Vector2(r.X - 64, r.Y)))
                            {
                                inserted = true;

                                player.LoadCollider(createTile(r, "innerBottomLeftCorner", true));
                            }
                        }

                        // Bottom Left
                        if (!inserted && !positions.Contains(new Vector2(r.X - 64, r.Y + 64)))
                        {
                            if (wallPositions.Contains(new Vector2(r.X, r.Y + 64)) && wallPositions.Contains(new Vector2(r.X - 64, r.Y)))
                            {
                                inserted = true;

                                player.LoadCollider(createTile(r, "topRightCorner", true));
                            }
                            else if (wallPositions.Contains(new Vector2(r.X, r.Y - 64)) && wallPositions.Contains(new Vector2(r.X + 64, r.Y)))
                            {
                                inserted = true;

                                player.LoadCollider(createTile(r, "innerTopRightCorner", true));
                            }
                        }

                        // Bottom Right
                        if (!inserted && !positions.Contains(new Vector2(r.X + 64, r.Y + 64)))
                        {
                            if (wallPositions.Contains(new Vector2(r.X, r.Y + 64)) && wallPositions.Contains(new Vector2(r.X + 64, r.Y)))
                            {
                                inserted = true;

                                player.LoadCollider(createTile(r, "topLeftCorner", true));
                            }
                            else if (wallPositions.Contains(new Vector2(r.X, r.Y - 64)) && wallPositions.Contains(new Vector2(r.X - 64, r.Y)))
                            {
                                inserted = true;
                                
                                player.LoadCollider(createTile(r, "innerTopLeftCorner", true));
                            }
                        }
                    }


                    if (!inserted)
                    {
                        //player.LoadCollider(createTile(r, "leftWall", true));
                    }













                    /*int cornerCount = 0;

                    if (wallPositions.Contains(new Vector2(r.X - 64, r.Y)))
                        cornerCount += 1;

                    if (wallPositions.Contains(new Vector2(r.X + 64, r.Y)))
                        cornerCount += 1;

                    if (wallPositions.Contains(new Vector2(r.X, r.Y + 64)))
                        cornerCount += 1;

                    if (wallPositions.Contains(new Vector2(r.X, r.Y - 64)))
                        cornerCount += 1;*/


                    // Normal Wall
                    /*if ((!positions.Contains(new Vector2(r.X, r.Y - 64)) ||
                            !positions.Contains(new Vector2(r.X, r.Y + 64)) ||
                            !positions.Contains(new Vector2(r.X - 64, r.Y)) ||
                            !positions.Contains(new Vector2(r.X + 64, r.Y))))
                    {*/





                    /*
                    // Top Bottom Wall
                    if (!positions.Contains(new Vector2(r.X, r.Y + 64)))
                    {
                        if (!positions.Contains(new Vector2(r.X - 64, r.Y))) // Inner Top Right Wall
                        {
                            innerTopRightWalls.Add(r);
                            player.LoadCollider(createTile(r, "innerTopRightCorner", true));
                        }
                        else if (!positions.Contains(new Vector2(r.X + 64, r.Y))) // Inner Top Left Wall
                        {
                            innerTopLeftWalls.Add(r);
                            player.LoadCollider(createTile(r, "innerTopLeftCorner", true));
                        }
                        else // Top Bottom Wall
                        {
                            bottomWalls.Add(r);
                            player.LoadCollider(createTile(r, "topWall", true));
                        }
                    }
                    else if (!positions.Contains(new Vector2(r.X, r.Y - 64)))
                    {
                        if (!positions.Contains(new Vector2(r.X - 64, r.Y))) //  Inner Bottom Right Wall
                        {
                            innerBottomRightWalls.Add(r);
                            player.LoadCollider(createTile(r, "innerBottomRightCorner", true));
                        }
                        else if (!positions.Contains(new Vector2(r.X + 64, r.Y))) // Inner Bottom Left Wall
                        {
                            innerBottomLeftWalls.Add(r);
                            player.LoadCollider(createTile(r, "innerBottomLeftCorner", true));
                        }
                        else // Top Bottom Wall
                        {
                            topWalls.Add(r);
                            player.LoadCollider(createTile(r, "bottomWall", true));
                        }
                    }
                    else // Side Wall
                    {

                        tempPosition = new Vector2(r.X + 64, r.Y);
                        if (!positions.Contains(tempPosition))
                        {
                            player.LoadCollider(createTile(r, "leftWall", true));
                        }
                        else
                        {
                            if (wallPositions.Contains(new Vector2(r.X + 64, r.Y)) && wallPositions.Contains(new Vector2(r.X, r.Y + 64)) && !wallPositions.Contains(new Vector2(r.X + 64, r.Y  - 64)))
                                player.LoadCollider(createTile(r, "innerBottomRightCorner", true));
                            else
                                player.LoadCollider(createTile(r, "rightWall", true));
                        }
                    }*/


                    /*
                    else // Corner
                    {
                        if (!positions.Contains(new Vector2(r.X + 64, r.Y + 64))) // Top Left Corner
                        {
                            topLeftWalls.Add(r);
                            player.LoadCollider(createTile(r, "topLeftCorner", true));
                        }
                        else if (!positions.Contains(new Vector2(r.X - 64, r.Y + 64))) // Top Right Corner
                        {
                            topRightWalls.Add(r);
                            player.LoadCollider(createTile(r, "topRightCorner", true));
                        }
                        else if (!positions.Contains(new Vector2(r.X + 64, r.Y - 64))) // Bottom Left Corner
                        {
                            bottomLeftWalls.Add(r);
                            player.LoadCollider(createTile(r, "bottomLeftCorner", true));
                        }
                        else // Bottom Right Corner
                        {
                            bottomRightWalls.Add(r);
                            player.LoadCollider(createTile(r, "bottomRightCorner", true));
                        }
                    }*/
                }

                /*
                // Adds Long Walls
                foreach (var r in topWalls)
                {
                    if (positions.Contains(new Vector2(r.X, r.Y + 64)) && !wallPositions.Contains(new Vector2(r.X, r.Y + 64)))
                    {
                        tempPosition = new Vector2(r.X, r.Y + 64);
                        longWalls.Add(tempPosition);
                        wallPositions.Add(tempPosition);
                        walls.Add(tempPosition);
                        player.LoadCollider(createTile(tempPosition, "longWall", true));
                    }

                    if (positions.Contains(new Vector2(r.X, r.Y + 128)) && !wallPositions.Contains(new Vector2(r.X, r.Y + 128)))
                    {
                        tempPosition = new Vector2(r.X, r.Y + 128);
                        longWalls.Add(tempPosition);
                        wallPositions.Add(tempPosition);
                        walls.Add(tempPosition);
                        player.LoadCollider(createTile(tempPosition, "longWall", true));
                    }
                }


                // Adds Long Left Corner Walls
                foreach (var r in bottomLeftWalls)
                {
                    if (positions.Contains(new Vector2(r.X, r.Y + 64)) && !wallPositions.Contains(new Vector2(r.X, r.Y + 64)))
                    {
                        tempPosition = new Vector2(r.X, r.Y + 64);
                        longLeftCornerWalls.Add(tempPosition);
                        wallPositions.Add(tempPosition);
                        walls.Add(tempPosition);
                        player.LoadCollider(createTile(tempPosition, "longLeftCornerWall", true));
                    }

                    if (positions.Contains(new Vector2(r.X, r.Y + 128)) && !wallPositions.Contains(new Vector2(r.X, r.Y + 128)))
                    {
                        tempPosition = new Vector2(r.X, r.Y + 128);
                        longLeftCornerWalls.Add(tempPosition);
                        wallPositions.Add(tempPosition);
                        walls.Add(tempPosition);
                        player.LoadCollider(createTile(tempPosition, "longLeftCornerWall", true));
                    }
                }

                // Adds Long Right Corner Walls
                foreach (var r in bottomRightWalls)
                {
                    if (positions.Contains(new Vector2(r.X, r.Y + 64)) && !wallPositions.Contains(new Vector2(r.X, r.Y + 64)))
                    {
                        tempPosition = new Vector2(r.X, r.Y + 64);
                        longRightCornerWalls.Add(tempPosition);
                        wallPositions.Add(tempPosition);
                        walls.Add(tempPosition);
                        player.LoadCollider(createTile(tempPosition, "longRightCornerWall", true));
                    }

                    if (positions.Contains(new Vector2(r.X, r.Y + 128)) && !wallPositions.Contains(new Vector2(r.X, r.Y + 128)))
                    {
                        tempPosition = new Vector2(r.X, r.Y + 128);
                        longRightCornerWalls.Add(tempPosition);
                        wallPositions.Add(tempPosition);
                        walls.Add(tempPosition);
                        player.LoadCollider(createTile(tempPosition, "longRightCornerWall", true));
                    }
                }*/
            }
            // Simply sets what tiles are general walls
            else
            {
                // Sets wall positions from all tile position
                foreach (var r in positions)
                {
                    if (
                        !positions.Contains(new Vector2(r.X - 64, r.Y)) ||
                        !positions.Contains(new Vector2(r.X - 64, r.Y - 64)) ||
                        !positions.Contains(new Vector2(r.X - 64, r.Y + 64)) ||
                        !positions.Contains(new Vector2(r.X + 64, r.Y)) ||
                        !positions.Contains(new Vector2(r.X + 64, r.Y - 64)) ||
                        !positions.Contains(new Vector2(r.X + 64, r.Y + 64)) ||
                        !positions.Contains(new Vector2(r.X, r.Y - 64)) ||
                        !positions.Contains(new Vector2(r.X, r.Y + 64)))
                    {
                        wallPositions.Add(new Vector2(r.X, r.Y));
                        walls.Add(new Vector2(r.X, r.Y));
                    }
                }
            }
        }


        private void setFloors()
        {
            foreach (var r in positions)
            {
                if (!wallPositions.Contains(r) && !wallPositions.Contains(new Vector2(r.X, r.Y - 64)) && !wallPositions.Contains(new Vector2(r.X, r.Y - 128)))
                {
                    floorPositions.Add(r);
                    createTile(r, "floor", false);
                }
            }
        }


        private Tile createTile(Vector2 pos, String type, bool collsion)
        {
            Tile currentTile = new Tile(pos, type, collsion);
            dungeonTiles.Add(currentTile);


            var x = ((int)(pos.X / mapSectionWidth) * mapSectionWidth) / mapSectionWidth;
            var y = ((int)(pos.Y / mapSectionHeight) * mapSectionHeight) / mapSectionHeight;

            mapSections[x, y].sectionTiles.Add(currentTile);

            return currentTile;
        }


        private void createSpawns()
        {
            

            
            Stopwatch stopwatch = new Stopwatch();

            var es = 10;
            var ss = 20;
            var ms = 30;
            var ls = 40;
            var bs = 60;


            Quadtree spawnTree = new Quadtree(dungeonContainer);
            HashSet<Rectangle> allSpawns = new HashSet<Rectangle>();
            HashSet<Rectangle> badSpawns = new HashSet<Rectangle>();


            HashSet<Rectangle> enemySpawns = new HashSet<Rectangle>();
            HashSet<Rectangle> smallSpawns = new HashSet<Rectangle>();
            HashSet<Rectangle> mediumSpawns = new HashSet<Rectangle>();
            HashSet<Rectangle> largeSpawns = new HashSet<Rectangle>();
            HashSet<Rectangle> bossSpawns = new HashSet<Rectangle>();


            foreach (var r in wallPositions)
            {
                spawnTree.insertSpawn(r, 1, 1, badSpawns, spawnTree);
            }

            
            foreach (var r in floorPositions)
            {
                spawnTree.insertSpawn(r, bs, bs, badSpawns, spawnTree);
            }

            foreach (var r in floorPositions)
            {
                spawnTree.insertSpawn(r, ls, ls, badSpawns, spawnTree);
            }

            foreach (var r in floorPositions)
            {
                spawnTree.insertSpawn(r, ms, ms, badSpawns, spawnTree);
            }

            foreach (var r in floorPositions)
            {
                spawnTree.insertSpawn(r, ss, ss, badSpawns, spawnTree);
            }

            foreach (var r in floorPositions)
            {
                spawnTree.insertSpawn(r, es, es, badSpawns, spawnTree);
            }

            spawnTree.makeTiles(debugTiles, badSpawns, es, ss, ms, ls, bs, enemySpawns, smallSpawns, mediumSpawns, largeSpawns, bossSpawns);

            stopwatch.Stop();

            long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            System.Diagnostics.Debug.WriteLine("TOTAL TIME FOR SPAWN CREATIONS: " + (float)elapsedMilliseconds / 1000);
        }


        public HashSet<Vector2> GeneratePointsInRectangle(Rectangle container, int walkLength, int tileDistance, int seed)
        {
            HashSet<Vector2> path = new HashSet<Vector2>();
            Random random = new Random(seed);

            Vector2 previousePosition = new Vector2(((container.X + (container.Width / 2)) / 64) * 64, ((container.Y + (container.Height / 2)) / 64) * 64);
            path.Add(previousePosition);

            for (int i = 0; i < walkLength; i++)
            {
                double angle = random.Next(0, 4) * 90.0; // Randomly choose 0, 90, 180, or 270 degrees

                double distance = 64.0; // Move exactly tileDistance tiles (64 pixels) in the chosen direction

                double newX = previousePosition.X;
                double newY = previousePosition.Y;

                if (angle == 0) // Move right
                {
                    newX += distance;
                }
                else if (angle == 90) // Move up
                {
                    newY -= distance;
                }
                else if (angle == 180) // Move left
                {
                    newX -= distance;
                }
                else if (angle == 270) // Move down
                {
                    newY += distance;
                }

                // Ensure the new point is within the container bounds
                newX = MathHelper.Clamp((float)newX, container.X, container.Right);
                newY = MathHelper.Clamp((float)newY, container.Y, container.Bottom);

                previousePosition = new Vector2((float)newX, (float)newY);
                path.Add(previousePosition);
            }





            HashSet<Vector2> tempPath = new HashSet<Vector2>();
            tempPath.UnionWith(path);

            foreach (var r in tempPath)
            {
                // First layer
                path.Add(new Vector2((int)r.X + 64, (int)r.Y));
                path.Add(new Vector2((int)r.X - 64, (int)r.Y));
                path.Add(new Vector2((int)r.X, (int)r.Y + 64));
                path.Add(new Vector2((int)r.X, (int)r.Y - 64));
                path.Add(new Vector2((int)r.X + 64, (int)r.Y + 64));
                path.Add(new Vector2((int)r.X + 64, (int)r.Y - 64));
                path.Add(new Vector2((int)r.X - 64, (int)r.Y + 64));
                path.Add(new Vector2((int)r.X - 64, (int)r.Y - 64));


                // Second Layer
                path.Add(new Vector2((int)r.X - 128, (int)r.Y));
                path.Add(new Vector2((int)r.X - 128, (int)r.Y + 64));
                path.Add(new Vector2((int)r.X - 128, (int)r.Y + 128));
                path.Add(new Vector2((int)r.X - 128, (int)r.Y - 64));
                path.Add(new Vector2((int)r.X - 128, (int)r.Y - 128));
                path.Add(new Vector2((int)r.X - 64, (int)r.Y - 128));
                path.Add(new Vector2((int)r.X - 64, (int)r.Y + 128));
                path.Add(new Vector2((int)r.X, (int)r.Y - 128));
                path.Add(new Vector2((int)r.X, (int)r.Y + 128));
                path.Add(new Vector2((int)r.X + 64, (int)r.Y - 128));
                path.Add(new Vector2((int)r.X + 64, (int)r.Y + 128));
                path.Add(new Vector2((int)r.X + 128, (int)r.Y));
                path.Add(new Vector2((int)r.X + 128, (int)r.Y + 64));
                path.Add(new Vector2((int)r.X + 128, (int)r.Y + 128));
                path.Add(new Vector2((int)r.X + 128, (int)r.Y - 64));
                path.Add(new Vector2((int)r.X + 128, (int)r.Y - 128));


                // Additional top tiles
                path.Add(new Vector2((int)r.X - 128, (int)r.Y - 192));
                path.Add(new Vector2((int)r.X - 64, (int)r.Y - 192));
                path.Add(new Vector2((int)r.X, (int)r.Y - 192));
                path.Add(new Vector2((int)r.X + 64, (int)r.Y - 192));
                path.Add(new Vector2((int)r.X + 128, (int)r.Y - 192));
                // Additional top tiles
                path.Add(new Vector2((int)r.X - 128, (int)r.Y - 256));
                path.Add(new Vector2((int)r.X - 64, (int)r.Y - 256));
                path.Add(new Vector2((int)r.X, (int)r.Y - 256));
                path.Add(new Vector2((int)r.X + 64, (int)r.Y - 256));
                path.Add(new Vector2((int)r.X + 128, (int)r.Y - 256));

                // Additional bottom tiles
                path.Add(new Vector2((int)r.X - 128, (int)r.Y + 192));
                path.Add(new Vector2((int)r.X - 64, (int)r.Y + 192));
                path.Add(new Vector2((int)r.X, (int)r.Y + 192));
                path.Add(new Vector2((int)r.X + 64, (int)r.Y + 192));
                path.Add(new Vector2((int)r.X + 128, (int)r.Y + 192));

                if (r.X <= least.X)
                {
                    least.X = r.X;
                    leftAnchor = r;
                }

                if (r.Y <= least.Y)
                {
                    least.Y = r.Y;
                    topAnchor = r;
                }

                if (r.X >= most.X)
                {
                    most.X = r.X;
                    rightAnchor = r;
                }

                if (r.Y >= most.Y)
                {
                    most.Y = r.Y;
                    bottomAnchor = r;
                }
            }

            return path;
        }




        private void mapSectionSpawnInsert(Vector2 position, String type)
        {
            var x = ((int)(position.X / mapSectionWidth) * mapSectionWidth) / mapSectionWidth;
            var y = ((int)(position.Y / mapSectionHeight) * mapSectionHeight) / mapSectionHeight;

            switch (type)
            {
                case "enemy":
                    if (!mapSections[x, y].enemySpawnPositions.Contains(position))
                    {
                        mapSections[x, y].enemySpawnPositionsList.Add(position);
                        mapSections[x, y].enemySpawnPositions.Add(position);
                        mapSections[x, y].enemySpawnCount += 1;
                    }
                    break;


                case "small":
                    if (!mapSections[x, y].smallSpawnPositions.Contains(position))
                    {
                        mapSections[x, y].smallSpawnPositionsList.Add(position);
                        mapSections[x, y].smallSpawnPositions.Add(position);
                        mapSections[x, y].smallSpawnCount += 1;
                    }
                    break;


                case "medium":
                    if (!mapSections[x, y].mediumSpawnPositions.Contains(position))
                    {
                        mapSections[x, y].mediumSpawnPositionsList.Add(position);
                        mapSections[x, y].mediumSpawnPositions.Add(position);
                        mapSections[x, y].mediumSpawnCount += 1;
                    }
                    break;


                case "large":
                    if (!mapSections[x, y].largeSpawnPositions.Contains(position))
                    {
                        mapSections[x, y].largeSpawnPositionsList.Add(position);
                        mapSections[x, y].largeSpawnPositions.Add(position);
                        mapSections[x, y].largeSpawnCount += 1;
                    }
                    break;


                case "boss":
                    if (!mapSections[x, y].bossSpawnPositions.Contains(position))
                    {
                        mapSections[x, y].bossSpawnPositionsList.Add(position);
                        mapSections[x, y].bossSpawnPositions.Add(position);
                        mapSections[x, y].bossSpawnCount += 1;
                    }
                    break;


                default:
                    break;
            }
        }
    }



    public static class Direction2D
    {
        public static List<Vector2> cardinalDirectionsList = new List<Vector2>
        {
            new Vector2(0, 64),
            new Vector2(64, 0),
            new Vector2(0, -64),
            new Vector2(-64, 0)
        };

        public static Vector2 RandomCardinalDirection(Random random)
        {
            var rand = random.Next(0, cardinalDirectionsList.Count);

            return cardinalDirectionsList[rand];
        }
    }


    public class Tile
    {
        public Rectangle tileContainer;
        public Vector2 tilePosition;
        public Texture2D tileTexture;

        /*
         * floor
         * north
         * north-east
         * east
         * south-east
         * south
         * south-west
         * west
         * north-west
         */
        public String type;

        public bool collider = false;

        public Tile(Vector2 position, String _type, Boolean collision)
        {
            if (_type == "enemy")
                tileContainer = new Rectangle((int)position.X + 32, (int)position.Y + 32, 640 - 64, 640 - 64);
            else if (_type == "enemy2")
                tileContainer = new Rectangle((int)position.X + 32, (int)position.Y + 32, 640 - 64, 640 - 64);
            else if (_type == "enemy3")
                tileContainer = new Rectangle((int)position.X - (5 * 64) + 32, (int)position.Y - (5 * 64) + 32, 640 - 64, 640 - 64);
            else if (_type != "back")
                tileContainer = new Rectangle((int)position.X, (int)position.Y, 64, 64);
            else
                tileContainer = new Rectangle((int)position.X - 32, (int)position.Y - 32, 128, 128);

            tilePosition = position;

            collider = collision;

            type = _type;
        }


        public Tile(Rectangle position, String _type, Boolean collision)
        {
            tileContainer = position;

            tilePosition = new Vector2(position.X, position.Y);

            collider = collision;

            type = _type;
        }
    }


    public class mapSection
    {
        public Rectangle sectionContainer;
        public Vector2 sectionPosition;
        public HashSet<Tile> sectionTiles = new HashSet<Tile>();
        public HashSet<Vector2> positions = new HashSet<Vector2>();

        public HashSet<Vector2> enemySpawnPositions = new HashSet<Vector2>();
        public HashSet<Vector2> smallSpawnPositions = new HashSet<Vector2>();
        public HashSet<Vector2> mediumSpawnPositions = new HashSet<Vector2>();
        public HashSet<Vector2> largeSpawnPositions = new HashSet<Vector2>();
        public HashSet<Vector2> bossSpawnPositions = new HashSet<Vector2>();

        public List<Vector2> enemySpawnPositionsList = new List<Vector2>();
        public List<Vector2> smallSpawnPositionsList = new List<Vector2>();
        public List<Vector2> mediumSpawnPositionsList = new List<Vector2>();
        public List<Vector2> largeSpawnPositionsList = new List<Vector2>();
        public List<Vector2> bossSpawnPositionsList = new List<Vector2>();

        public int enemySpawnCount = 0;
        public int smallSpawnCount = 0;
        public int mediumSpawnCount = 0;
        public int largeSpawnCount = 0;
        public int bossSpawnCount = 0;

        public mapSection(Rectangle container)
        {
            sectionContainer = container;
            sectionPosition = new Vector2(container.X, container.Y);
        }
    }
    
}
public class PerlinNoise
{
    private int[] permutation;
    private Random random;

    public PerlinNoise(int seed)
    {
        permutation = new int[256];
        random = new Random(seed);

        for (int i = 0; i < 256; i++)
        {
            permutation[i] = i;
        }

        // Shuffle the permutation array
        for (int i = 0; i < 256; i++)
        {
            int j = random.Next(256);
            int temp = permutation[i];
            permutation[i] = permutation[j];
            permutation[j] = temp;
        }
    }

    private double Fade(double t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    private double Grad(int hash, double x, double y)
    {
        int h = hash & 15;
        double grad = 1.0 + (h & 7); // Gradient value from 1.0 to 2.0
        if ((h & 8) != 0) grad = -grad; // Randomly invert the gradient

        return (grad * x) + (h >= 8 ? 0 : y);
    }

    public double Noise(double x, double y)
    {
        int X = (int)Math.Floor(x) & 255;
        int Y = (int)Math.Floor(y) & 255;

        x -= Math.Floor(x);
        y -= Math.Floor(y);

        double u = Fade(x);
        double v = Fade(y);

        int A = permutation[X] + Y;
        int B = permutation[(X + 1) & 255] + Y;
        int AA = permutation[A] & 255;
        int BA = permutation[B] & 255;

        return Grad(permutation[AA], x, y) +
               (u * (Grad(permutation[BA], x - 1, y) - Grad(permutation[AA], x, y))) +
               (v * (Grad(permutation[AA + 1], x, y - 1) - Grad(permutation[AA], x, y)));
    }
}



public class PointGenerator
{
    public List<Vector2> GeneratePointsInRectangle(Rectangle rectangle, int numberOfPoints, double scale, int seed)
    {
        List<Vector2> points = new List<Vector2>();

        // Set up PerlinNoise with the given seed
        PerlinNoise perlinNoise = new PerlinNoise(seed);

        // Generate points using Perlin noise
        for (int i = 0; i < numberOfPoints; i++)
        {
            // Sample Perlin noise in the range [0, 1] for x and y coordinates
            double noiseX = (perlinNoise.Noise(i * scale, 0) + 1) / 2.0;
            double noiseY = (perlinNoise.Noise(0, i * scale) + 1) / 2.0;

            // Map the normalized noise values to the dimensions of the rectangle
            float pointX = rectangle.X + (float)(noiseX * rectangle.Width);
            float pointY = rectangle.Y + (float)(noiseY * rectangle.Height);

            points.Add(new Vector2(pointX, pointY));
        }

        return points;
    }
}