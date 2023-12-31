﻿using Microsoft.Xna.Framework;
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
            var level = 16;

            var walk = 0;
            if (level <= 100)
                walk = (int)(((int)((level * level) / (4.2f)) + 140) * 71.428);
            else
                walk = (int)(240000 * (1 - Math.Pow(Math.E, -0.05 * (level - 72.4))));

            var scale = 0f;
            if (level < 15)
                scale = 2.4f;
            else if (level < 25)
                scale = 2.0f;
            else if (level < 50)
                scale = 1.8f;
            else if (level < 75)
                scale = 1.6f;
            else
                scale = 1.4f;


            var levelSize = (int)(Math.Sqrt(walk) * scale);


            dungeonContainer = new Rectangle(0, 0, levelSize * 64, levelSize * 64);
            debugRectangle = dungeonContainer;


            // Creates Map Sections
            createMapSections(graphics);


            // Random Walks to set the dungeon tile positions
            positions = FastRandomWalk(dungeonContainer, walk, level + random.Next(0, 100000000));
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


            // Sets general wall positions
            setWalls(positions, wallPositions, walls, false);

            // Sets dungeon container to the appropriate container only fitting the dungeon tiles
            dungeonContainer = new Rectangle((int)least.X, (int)least.Y, (int)most.X - (int)least.X, (int)most.Y - (int)least.Y);

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



        public HashSet<Vector2> FastRandomWalk(Rectangle container, int walkLength, int seed)
        {
            HashSet<Vector2> path = new HashSet<Vector2>();
            HashSet<Vector2> addedPath = new HashSet<Vector2>();
            Random random = new Random(seed);

            Vector2 previousePosition = new Vector2(((container.X + (container.Width / 2)) / 64) * 64, ((container.Y + (container.Height / 2)) / 64) * 64);
            path.Add(previousePosition);

            for (int i = 0; i < walkLength; i++)
            {
                double angle = random.Next(0, 4) * 90.0; // Randomly choose 0, 90, 180, or 270 degrees

                double distance = 64.0; // Move a tile length

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

                // Checks to see if the position has already been added to the collection of positions
                if (path.Contains(previousePosition))
                {
                    i--;
                }
                else // If not a repeat then add additional positions to the additional path collection
                {
                    // First layer
                    addedPath.Add(new Vector2((int)previousePosition.X + 64, (int)previousePosition.Y));
                    addedPath.Add(new Vector2((int)previousePosition.X - 64, (int)previousePosition.Y));
                    addedPath.Add(new Vector2((int)previousePosition.X, (int)previousePosition.Y + 64));
                    addedPath.Add(new Vector2((int)previousePosition.X, (int)previousePosition.Y - 64));
                    addedPath.Add(new Vector2((int)previousePosition.X + 64, (int)previousePosition.Y + 64));
                    addedPath.Add(new Vector2((int)previousePosition.X + 64, (int)previousePosition.Y - 64));
                    addedPath.Add(new Vector2((int)previousePosition.X - 64, (int)previousePosition.Y + 64));
                    addedPath.Add(new Vector2((int)previousePosition.X - 64, (int)previousePosition.Y - 64));


                    // Second Layer
                    addedPath.Add(new Vector2((int)previousePosition.X - 128, (int)previousePosition.Y));
                    addedPath.Add(new Vector2((int)previousePosition.X - 128, (int)previousePosition.Y + 64));
                    addedPath.Add(new Vector2((int)previousePosition.X - 128, (int)previousePosition.Y + 128));
                    addedPath.Add(new Vector2((int)previousePosition.X - 128, (int)previousePosition.Y - 64));
                    addedPath.Add(new Vector2((int)previousePosition.X - 128, (int)previousePosition.Y - 128));
                    addedPath.Add(new Vector2((int)previousePosition.X - 64, (int)previousePosition.Y - 128));
                    addedPath.Add(new Vector2((int)previousePosition.X - 64, (int)previousePosition.Y + 128));
                    addedPath.Add(new Vector2((int)previousePosition.X, (int)previousePosition.Y - 128));
                    addedPath.Add(new Vector2((int)previousePosition.X, (int)previousePosition.Y + 128));
                    addedPath.Add(new Vector2((int)previousePosition.X + 64, (int)previousePosition.Y - 128));
                    addedPath.Add(new Vector2((int)previousePosition.X + 64, (int)previousePosition.Y + 128));
                    addedPath.Add(new Vector2((int)previousePosition.X + 128, (int)previousePosition.Y));
                    addedPath.Add(new Vector2((int)previousePosition.X + 128, (int)previousePosition.Y + 64));
                    addedPath.Add(new Vector2((int)previousePosition.X + 128, (int)previousePosition.Y + 128));
                    addedPath.Add(new Vector2((int)previousePosition.X + 128, (int)previousePosition.Y - 64));
                    addedPath.Add(new Vector2((int)previousePosition.X + 128, (int)previousePosition.Y - 128));


                    // Additional top tiles
                    addedPath.Add(new Vector2((int)previousePosition.X - 128, (int)previousePosition.Y - 192));
                    addedPath.Add(new Vector2((int)previousePosition.X - 64, (int)previousePosition.Y - 192));
                    addedPath.Add(new Vector2((int)previousePosition.X, (int)previousePosition.Y - 192));
                    addedPath.Add(new Vector2((int)previousePosition.X + 64, (int)previousePosition.Y - 192));
                    addedPath.Add(new Vector2((int)previousePosition.X + 128, (int)previousePosition.Y - 192));
                    // Additional top tiles
                    addedPath.Add(new Vector2((int)previousePosition.X - 128, (int)previousePosition.Y - 256));
                    addedPath.Add(new Vector2((int)previousePosition.X - 64, (int)previousePosition.Y - 256));
                    addedPath.Add(new Vector2((int)previousePosition.X, (int)previousePosition.Y - 256));
                    addedPath.Add(new Vector2((int)previousePosition.X + 64, (int)previousePosition.Y - 256));
                    addedPath.Add(new Vector2((int)previousePosition.X + 128, (int)previousePosition.Y - 256));

                    // Additional bottom tiles
                    addedPath.Add(new Vector2((int)previousePosition.X - 128, (int)previousePosition.Y + 192));
                    addedPath.Add(new Vector2((int)previousePosition.X - 64, (int)previousePosition.Y + 192));
                    addedPath.Add(new Vector2((int)previousePosition.X, (int)previousePosition.Y + 192));
                    addedPath.Add(new Vector2((int)previousePosition.X + 64, (int)previousePosition.Y + 192));
                    addedPath.Add(new Vector2((int)previousePosition.X + 128, (int)previousePosition.Y + 192));


                    // List of checks to set the least and most positions
                    if (previousePosition.X <= least.X)
                    {
                        least.X = previousePosition.X;
                        leftAnchor = previousePosition;
                    }

                    if (previousePosition.Y <= least.Y)
                    {
                        least.Y = previousePosition.Y;
                        topAnchor = previousePosition;
                    }

                    if (previousePosition.X >= most.X)
                    {
                        most.X = previousePosition.X;
                        rightAnchor = previousePosition;
                    }

                    if (previousePosition.Y >= most.Y)
                    {
                        most.Y = previousePosition.Y;
                        bottomAnchor = previousePosition;
                    }
                }

                path.Add(previousePosition);
            }

            // Combine the additional positions to the main set of positions
            path.UnionWith(addedPath);

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



            mapSections = new mapSection[(dungeonContainer.Width / mapSectionWidth) + 2, (dungeonContainer.Height / mapSectionHeight) + 2];


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
                Vector2 tempPosition;
                HashSet<Vector2> additions = new HashSet<Vector2>();
                HashSet<Vector2> trios = new HashSet<Vector2>();

                foreach (var r in wallPositions)
                {
                    var wallCounter = 0;

                    // Up
                    if (wallPositions.Contains(new Vector2(r.X, r.Y - 64)))
                        wallCounter += 1;
                    // Down
                    if (wallPositions.Contains(new Vector2(r.X, r.Y + 64)))
                        wallCounter += 1;
                    // Left
                    if (wallPositions.Contains(new Vector2(r.X - 64, r.Y)))
                        wallCounter += 1;
                    // Right
                    if (wallPositions.Contains(new Vector2(r.X + 64, r.Y)))
                        wallCounter += 1;

                    if (wallCounter > 0)
                    {
                        // Normal Wall
                        if ((!positions.Contains(new Vector2(r.X, r.Y - 64)) ||
                             !positions.Contains(new Vector2(r.X, r.Y + 64)) ||
                             !positions.Contains(new Vector2(r.X - 64, r.Y)) ||
                             !positions.Contains(new Vector2(r.X + 64, r.Y))))
                        {

                            // Top Bottom Wall
                            if (!positions.Contains(new Vector2(r.X, r.Y + 64)))
                            {
                                if (!positions.Contains(new Vector2(r.X - 64, r.Y))) // Inner Top Right Wall
                                {
                                    player.LoadCollider(createTile(r, "innerTopRightCorner", true));
                                }
                                else if (!positions.Contains(new Vector2(r.X + 64, r.Y))) // Inner Top Left Wall
                                {
                                    player.LoadCollider(createTile(r, "innerTopLeftCorner", true));
                                }
                                else
                                {
                                    if (wallPositions.Contains(new Vector2(r.X - 64, r.Y)) && wallPositions.Contains(new Vector2(r.X, r.Y - 64)) && !positions.Contains(new Vector2(r.X + 64, r.Y - 64)))
                                        player.LoadCollider(createTile(r, "innerTopLeftCorner", true));
                                    else if (wallPositions.Contains(new Vector2(r.X + 64, r.Y)) && wallPositions.Contains(new Vector2(r.X, r.Y - 64)) && !positions.Contains(new Vector2(r.X - 64, r.Y - 64)))
                                        player.LoadCollider(createTile(r, "innerTopRightCorner", true));
                                    else
                                        player.LoadCollider(createTile(r, "topWall", true));
                                }
                            }
                            else if (!positions.Contains(new Vector2(r.X, r.Y - 64)))
                            {
                                if (!positions.Contains(new Vector2(r.X - 64, r.Y))) //  Inner Bottom Right Wall
                                {
                                    player.LoadCollider(createTile(r, "innerBottomRightCorner", true));
                                }
                                else if (!positions.Contains(new Vector2(r.X + 64, r.Y))) // Inner Bottom Left Wall
                                {
                                    player.LoadCollider(createTile(r, "innerBottomLeftCorner", true));
                                }
                                else // Bottom Wall
                                {
                                    if (wallPositions.Contains(new Vector2(r.X - 64, r.Y)) && wallPositions.Contains(new Vector2(r.X, r.Y + 64)) && !positions.Contains(new Vector2(r.X + 64, r.Y + 64)))
                                        player.LoadCollider(createTile(r, "innerBottomLeftCorner", true));
                                    else if (wallPositions.Contains(new Vector2(r.X + 64, r.Y)) && wallPositions.Contains(new Vector2(r.X, r.Y + 64)) && !positions.Contains(new Vector2(r.X - 64, r.Y + 64)))
                                        player.LoadCollider(createTile(r, "innerBottomRightCorner", true));
                                    else
                                    {
                                        player.LoadCollider(createTile(r, "bottomWall", true));

                                        tempPosition = new Vector2(r.X, r.Y + 64);

                                        if (!wallPositions.Contains(tempPosition))
                                        {
                                            additions.Add(tempPosition);
                                            walls.Add(tempPosition);
                                            player.LoadCollider(createTile(tempPosition, "longWall", true));

                                            tempPosition = new Vector2(r.X, r.Y + 128);

                                            if (!wallPositions.Contains(tempPosition))
                                            {
                                                additions.Add(tempPosition);
                                                walls.Add(tempPosition);
                                                player.LoadCollider(createTile(tempPosition, "longWall", true));
                                            }
                                        }
                                    }
                                }
                            }
                            else // Side Wall
                            {

                                tempPosition = new Vector2(r.X + 64, r.Y);
                                if (!positions.Contains(tempPosition))
                                {
                                    if (wallPositions.Contains(new Vector2(r.X - 64, r.Y)) && wallPositions.Contains(new Vector2(r.X, r.Y - 64)) && !positions.Contains(new Vector2(r.X - 64, r.Y + 64)))
                                        player.LoadCollider(createTile(r, "innerTopLeftCorner", true));
                                    else if (wallPositions.Contains(new Vector2(r.X - 64, r.Y)) && wallPositions.Contains(new Vector2(r.X, r.Y + 64)) && !positions.Contains(new Vector2(r.X - 64, r.Y - 64)))
                                        player.LoadCollider(createTile(r, "innerBottomLeftCorner", true));
                                    else
                                        player.LoadCollider(createTile(r, "leftWall", true));
                                }
                                else
                                {
                                    if (wallPositions.Contains(new Vector2(r.X + 64, r.Y)) && wallPositions.Contains(new Vector2(r.X, r.Y + 64)) && !positions.Contains(new Vector2(r.X + 64, r.Y - 64)))
                                        player.LoadCollider(createTile(r, "innerBottomRightCorner", true));
                                    else if (wallPositions.Contains(new Vector2(r.X + 64, r.Y)) && wallPositions.Contains(new Vector2(r.X, r.Y - 64)) && !positions.Contains(new Vector2(r.X + 64, r.Y + 64)))
                                        player.LoadCollider(createTile(r, "innerTopRightCorner", true));
                                    else
                                        player.LoadCollider(createTile(r, "rightWall", true));
                                }
                            }
                        }
                        else // Corner
                        {
                            if (!positions.Contains(new Vector2(r.X + 64, r.Y + 64))) // Top Left Corner
                            {
                                player.LoadCollider(createTile(r, "topLeftCorner", true));
                            }
                            else if (!positions.Contains(new Vector2(r.X - 64, r.Y + 64))) // Top Right Corner
                            {
                                player.LoadCollider(createTile(r, "topRightCorner", true));
                            }
                            else if (!positions.Contains(new Vector2(r.X + 64, r.Y - 64))) // Bottom Left Corner
                            {
                                player.LoadCollider(createTile(r, "bottomLeftCorner", true));

                                tempPosition = new Vector2(r.X, r.Y + 64);
                                if (!wallPositions.Contains(tempPosition))
                                {
                                    additions.Add(tempPosition);
                                    walls.Add(tempPosition);
                                    player.LoadCollider(createTile(tempPosition, "longLeftCornerWall", true));

                                    tempPosition = new Vector2(r.X, r.Y + 128);

                                    if (!wallPositions.Contains(tempPosition))
                                    {
                                        additions.Add(tempPosition);
                                        walls.Add(tempPosition);
                                        player.LoadCollider(createTile(tempPosition, "longLeftCornerWall", true));
                                    }
                                }
                            }
                            else // Bottom Right Corner
                            {
                                player.LoadCollider(createTile(r, "bottomRightCorner", true));

                                tempPosition = new Vector2(r.X, r.Y + 64);

                                if (!wallPositions.Contains(tempPosition))
                                {
                                    additions.Add(tempPosition);
                                    walls.Add(tempPosition);
                                    player.LoadCollider(createTile(tempPosition, "longRightCornerWall", true));

                                    tempPosition = new Vector2(r.X, r.Y + 128);

                                    if (!wallPositions.Contains(tempPosition))
                                    {
                                        additions.Add(tempPosition);
                                        walls.Add(tempPosition);
                                        player.LoadCollider(createTile(tempPosition, "longRightCornerWall", true));
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        trios.Add(r);
                    }
                }

                foreach (var r in trios)
                {
                    var counter = 0;

                    if (!positions.Contains(new Vector2(r.X - 64, r.Y)))
                        counter += 1;
                    if (!positions.Contains(new Vector2(r.X, r.Y - 64)))
                        counter += 1;
                    if (!positions.Contains(new Vector2(r.X + 64, r.Y)))
                        counter += 1;
                    if (!positions.Contains(new Vector2(r.X, r.Y + 64)))
                        counter += 1;
                    if (!positions.Contains(new Vector2(r.X - 64, r.Y - 64)))
                        counter += 1;
                    if (!positions.Contains(new Vector2(r.X - 64, r.Y + 64)))
                        counter += 1;
                    if (!positions.Contains(new Vector2(r.X + 64, r.Y + 64)))
                        counter += 1;
                    if (!positions.Contains(new Vector2(r.X + 64, r.Y - 64)))
                        counter += 1;

                        player.LoadCollider(createTile(r, "trio", true));
                        System.Diagnostics.Debug.WriteLine("what do i do?");
                }

                foreach (var r in additions)
                {
                    wallPositions.Add(r);
                }
            }
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