using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Text;

using TiledSharp;

namespace NotAnMMO
{
    class MISC
    {
        public LevelPiece head;
        public int numRooms = 10;
        public int looseEnds = 0;
        public static Random rand = new Random();
        /*
        public MISC(LevelLoader rooms)
        {
            head = rooms.grandRooms[rand.Next(0, rooms.rooms.Count)];
            numRooms -= 1;
            looseEnds += head.map.transitions.Count;

            GenerateRooms(rooms, head);

            System.Diagnostics.Debug.WriteLine(head.map.transitions.Count + ", "+ head.children.Count);
        }*/

        private void GenerateRooms(LevelLoader rooms, LevelPiece currentRoom)
        {
            if (numRooms <= 0)
                return;

            LevelPiece tempRoom;

            for (var i = 0; i < currentRoom.map.transitions.Count; i++)
            {
                numRooms -= 1;
                tempRoom = rooms.grandRooms[rand.Next(0, rooms.rooms.Count)];
                if (tempRoom.map.transitions.Count + looseEnds - 1 <= numRooms)
                {
                    currentRoom.children.Add(tempRoom);
                    looseEnds += tempRoom.map.transitions.Count - 1;
                }
                else
                {
                    i--;
                }
            }


        }

    }

    class LevelLoader
    {
        public List<LevelPiece> rooms = new List<LevelPiece>();
        public List<LevelPiece> hallways = new List<LevelPiece>();
        public List<LevelPiece> grandRooms = new List<LevelPiece>();
        public List<LevelPiece> bossRooms = new List<LevelPiece>();

        public void Load(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            LoadRooms(Content);
            LoadHallways(Content);
            LoadGrandRooms(Content);
            LoadBoss(Content);
        }

        public void LoadRooms(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            rooms.Add(new LevelPiece(Content, "room", "Room1"));
            rooms.Add(new LevelPiece(Content, "room", "Room2"));
        }

        public void LoadHallways(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            hallways.Add(new LevelPiece(Content, "hallway", "Hallway1"));
            hallways.Add(new LevelPiece(Content, "hallway", "Hallway2"));
            hallways.Add(new LevelPiece(Content, "hallway", "Hallway3"));
            hallways.Add(new LevelPiece(Content, "hallway", "Hallway4"));
        }
        public void LoadGrandRooms(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            grandRooms.Add(new LevelPiece(Content, "grandRoom", "Grand1"));
        }

        public void LoadBoss(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            bossRooms.Add(new LevelPiece(Content, "boss", "Boss1"));
        }
    }

    class LevelPiece
    {
        public string type;
        public Texture2D baseLevel;
        public Tilemap map;
        public List<LevelPiece> children = new List<LevelPiece>();


        public LevelPiece(Microsoft.Xna.Framework.Content.ContentManager Content, String _type, String name)
        {
            type = _type;
            baseLevel = Content.Load<Texture2D>("WorldArt/MapPictures/" + name);
            map = new Tilemap(new TmxMap("Content/WorldArt/Maps/" + name + ".tmx"), Content.Load<Texture2D>("WorldArt/Tilesets/" + name));
        }
    }
}









/*

private readonly Random random = new Random();
public HashSet<Rectangle> floors = new HashSet<Rectangle>();
public Rectangle dungeonContainer = new Rectangle(0, 0, 64, 64);
public Rectangle wholeSpace = new Rectangle();
public List<Room> dungeonRooms = new List<Room>();
private int currentID = 0;


/**
 * Constructor for a Dungeon
 *//*
public Dungeon(int minWidth, int minHeight, int minRooms, int maxRooms)
{
    floors = GenerateRooms(dungeonContainer, minWidth, minHeight, minRooms, maxRooms);
    //floors = ConnectRooms(floors);
    //floors = GenerateCorridors(floors);
}


/**
 * Creates rooms, and adds each room to the dungeon's rooms list, and splits the rooms into tiles as well for the floor list
 *//*
private HashSet<Rectangle> GenerateRooms(Rectangle spaceToSplit, int minWidth, int minHeight, int minRooms, int maxRooms)
{
    // Random Walk to give rooms more of a random shape
    HashSet<Rectangle> randomWalkTiles = new HashSet<Rectangle>();

    randomWalkTiles = SimpleRandomWalk(new Vector2(0, 0), 640000);

    foreach (var r in randomWalkTiles)
    {
        floors.Add(r);
    }

    return floors;
}


private HashSet<Rectangle> SimpleRandomWalk(Vector2 startPosition, int walkLength)
{
    HashSet<Rectangle> path = new HashSet<Rectangle>();

    path.Add(new Rectangle((int)startPosition.X, (int)startPosition.Y, 64, 64));

    var previousePosition = startPosition;

    for (int i = 0; i < walkLength; i++)
    {
        var newPosition = previousePosition + Direction2D.RandomCardinalDirection(random);
        path.Add(new Rectangle((int)newPosition.X + 64, (int)newPosition.Y, 64, 64));
        path.Add(new Rectangle((int)newPosition.X - 64, (int)newPosition.Y, 64, 64));
        path.Add(new Rectangle((int)newPosition.X, (int)newPosition.Y + 64, 64, 64));
        path.Add(new Rectangle((int)newPosition.X, (int)newPosition.Y - 64, 64, 64));
        path.Add(new Rectangle((int)newPosition.X + 64, (int)newPosition.Y + 64, 64, 64));
        path.Add(new Rectangle((int)newPosition.X + 64, (int)newPosition.Y - 64, 64, 64));
        path.Add(new Rectangle((int)newPosition.X - 64, (int)newPosition.Y + 64, 64, 64));
        path.Add(new Rectangle((int)newPosition.X - 64, (int)newPosition.Y - 64, 64, 64));


        path.Add(new Rectangle((int)newPosition.X, (int)newPosition.Y, 64, 64));
        previousePosition = newPosition;
    }

    return path;
}


/**
 * Randomly connects rooms together and assigns rooms a reference to the room(s) they are connected to
 *//*
private HashSet<Rectangle> ConnectRooms(HashSet<Rectangle> floors)
{
    // Rooms yet to be connected
    Queue<Room> rooms = new Queue<Room>();

    // Queues all rooms
    foreach (var r in dungeonRooms)
    {
        rooms.Enqueue(r);
    }

    // Vars to keep track of connecting rooms
    Random random = new Random();
    Room currentRoom;
    Room randomRoom;

    // Runs until each room has been connected
    while (rooms.Count > 0)
    {
        currentRoom = rooms.Dequeue();

        // Randomly picks room to connect to
        randomRoom = dungeonRooms[random.Next(0, dungeonRooms.Count)];

        // Makes sure the random room is not the currentRoom selected
        if (currentRoom.ID == randomRoom.ID)
        {
            rooms.Enqueue(currentRoom);
        }
        else
        {
            // Connects the rooms
            if (!currentRoom.connectedRooms.Contains(randomRoom))
            {
                currentRoom.connectedRooms.Add(randomRoom);
                randomRoom.connectedRooms.Add(currentRoom);
            }
            else
            {
                rooms.Enqueue(currentRoom);
            }


            Vector2 currentPosition = currentRoom.center;
            var moveX = true;
            var moveY = true;
            Rectangle current = new Rectangle();

            if (currentPosition.X % 64 != 0)
            {
                currentPosition.X = (int)(currentPosition.X / 64) * 64;
            }

            if (currentPosition.Y % 64 != 0)
            {
                currentPosition.Y = (int)(currentPosition.Y / 64) * 64;
            }

            if (randomRoom.center.X % 64 != 0)
            {
                randomRoom.center.X = (int)(randomRoom.center.X / 64) * 64;
            }

            if (randomRoom.center.Y % 64 != 0)
            {
                randomRoom.center.Y = (int)(randomRoom.center.Y / 64) * 64;
            }


            while (true)
            {
                if (current.Intersects(randomRoom.roomContainer))
                {
                    break;
                }

                foreach (var r in dungeonRooms)
                {
                    if (current.Intersects(r.roomContainer) && !r.connectedRooms.Contains(currentRoom) && r.ID != currentRoom.ID)
                    {
                        r.connectedRooms.Add(currentRoom);
                    }
                }

                var rand = random.Next(0, 2);

                // Move X
                if (rand == 0 && moveX)
                {
                    // Move Right
                    if (currentPosition.X < randomRoom.center.X)
                    {
                        /*for (int i = -2; i <= 2; i++)
                        {
                            current = new Rectangle((int)currentPosition.X, (int)currentPosition.Y + (i * 64), 64, 64);
                            floors.Add(current);
                            current = new Rectangle((int)currentPosition.X + (i * 64), (int)currentPosition.Y, 64, 64);
                            floors.Add(current);
                        }*//*

current = new Rectangle((int)currentPosition.X, (int)currentPosition.Y, 64, 64);
                        floors.Add(current);

                        currentPosition.X += 64;
                    }
                    // Move Left
                    else if (currentPosition.X > randomRoom.center.X)
                    {
                        /*for (int i = -2; i <= 2; i++)
                        {
                            current = new Rectangle((int)currentPosition.X, (int)currentPosition.Y + (i * 64), 64, 64);
                            floors.Add(current);
                            current = new Rectangle((int)currentPosition.X + (i * 64), (int)currentPosition.Y, 64, 64);
                            floors.Add(current);
                        }*//*

current = new Rectangle((int)currentPosition.X, (int)currentPosition.Y, 64, 64);
                        floors.Add(current);

                        currentPosition.X -= 64;
                    }
                    else
                    {
                        moveX = false;
                    }


                }
                // Move Y

                if (rand == 1 && moveY)
                {
                    // Move Down
                    if (currentPosition.Y < randomRoom.center.Y)
                    {
                        /*for (int i = -2; i <= 2; i++)
                        {
                            current = new Rectangle((int)currentPosition.X + (i * 64), (int)currentPosition.Y, 64, 64);
                            floors.Add(current);
                            current = new Rectangle((int)currentPosition.X, (int)currentPosition.Y + (i * 64), 64, 64);
                            floors.Add(current);
                        }*//*

current = new Rectangle((int)currentPosition.X, (int)currentPosition.Y, 64, 64);
                        floors.Add(current);

                        currentPosition.Y += 64;
                    }
                    // Move Up
                    else if (currentPosition.Y > randomRoom.center.Y)
                    {
                        /*for (int i = -2; i <= 2; i++)
                         {
                             current = new Rectangle((int)currentPosition.X + (i * 64), (int)currentPosition.Y, 64, 64);
                             floors.Add(current);
                             current = new Rectangle((int)currentPosition.X, (int)currentPosition.Y + (i * 64), 64, 64);
                             floors.Add(current);
                         }*//*

current = new Rectangle((int)currentPosition.X, (int)currentPosition.Y, 64, 64);
                        floors.Add(current);

                        currentPosition.Y -= 64;
                    }
                    else
                    {
                        moveY = false;
                    }
                }
            }
        }
    }

    return floors;
}

/**
 * Generates corridors between rooms
 *//*
private void GenerateCorridors(int count, Room currentRoom)
{
    if (count <= 1)
    {
        return;
    }

    Vector2 currentPosition = currentRoom.center;
    var moveX = true;
    var moveY = true;
    Rectangle current = new Rectangle();

    if (currentPosition.X % 64 != 0)
    {
        currentPosition.X = (int)(currentPosition.X / 64) * 64;
    }

    if (currentPosition.Y % 64 != 0)
    {
        currentPosition.Y = (int)(currentPosition.Y / 64) * 64;
    }


    Room randomRoom = dungeonRooms[random.Next(0, dungeonRooms.Count)];


    if (randomRoom.center.X % 64 != 0)
    {
        randomRoom.center.X = (int)(randomRoom.center.X / 64) * 64;
    }

    if (randomRoom.center.Y % 64 != 0)
    {
        randomRoom.center.Y = (int)(randomRoom.center.Y / 64) * 64;
    }

    System.Diagnostics.Debug.WriteLine("Current ID: " + currentRoom.ID + " random ID: " + randomRoom.ID);
    while (true)
    {
        if (current.Intersects(randomRoom.roomContainer))
        {
            break;
        }

        foreach (var r in dungeonRooms)
        {
            if (current.Intersects(r.roomContainer) && !r.connectedRooms.Contains(currentRoom) && r.ID != currentRoom.ID)
            {
                r.connectedRooms.Add(currentRoom);
            }
        }

        var rand = random.Next(0, 2);

        // Move X
        if (rand == 0 && moveX)
        {
            // Move Right
            if (currentPosition.X < randomRoom.center.X)
            {
                current = new Rectangle((int)currentPosition.X, (int)currentPosition.Y, 64, 64);
                floors.Add(current);
                currentPosition.X += 64;
            }
            // Move Left
            else if (currentPosition.X > randomRoom.center.X)
            {
                current = new Rectangle((int)currentPosition.X, (int)currentPosition.Y, 64, 64);
                floors.Add(current);
                currentPosition.X -= 64;
            }
            else
            {
                moveX = false;
            }


        }
        // Move Y

        if (rand == 1 && moveY)
        {
            // Move Down
            if (currentPosition.Y < randomRoom.center.Y)
            {
                current = new Rectangle((int)currentPosition.X, (int)currentPosition.Y, 64, 64);
                floors.Add(current);
                currentPosition.Y += 64;
            }
            // Move Up
            else if (currentPosition.Y > randomRoom.center.Y)
            {
                current = new Rectangle((int)currentPosition.X, (int)currentPosition.Y, 64, 64);
                floors.Add(current);
                currentPosition.Y -= 64;
            }
            else
            {
                moveY = false;
            }
        }
    }

    count--;
    System.Diagnostics.Debug.WriteLine("count: " + count);
    GenerateCorridors(count, dungeonRooms[count-1]);
}
*/




/**
 * Splits large rectangle into smaller ones randomly, creating rooms
 *//*
private List<Rectangle> BinarySpacePartioning(Rectangle spaceToSplit, int minWidth, int minHeight, int maxRooms)
{
    // Creates a queue for the rooms to still be split, and the actuall list for rooms
    Queue<Rectangle> roomsQueue = new Queue<Rectangle>();
    List<Rectangle> roomsList = new List<Rectangle>();

    roomsQueue.Enqueue(spaceToSplit);


    // Runs until all the rooms are generated
    while (roomsQueue.Count > 0)
    {
        var room = roomsQueue.Dequeue();
        if (room.Height >= minHeight && room.Width >= minWidth)
        {
            if (random.Next(0, 2) > 1)
            {
                // Checks to see if the room can still be split more
                if (room.Height >= minHeight * 2)
                {
                    SplitHorizontally(minWidth, minHeight, roomsQueue, room);
                }
                else if (room.Width >= minWidth * 2)
                {
                    SplitVertically(minWidth, minHeight, roomsQueue, room);
                }
                else
                {
                    room.Width = ((int)(room.Width / 64)) * 64;
                    room.Height = ((int)(room.Height / 64)) * 64;

                    roomsList.Add(room);
                }
            }
            else
            {
                // Checks to see if the room can still be split more
                if (room.Width >= minWidth * 2)
                {
                    SplitVertically(minWidth, minHeight, roomsQueue, room);
                }
                else if (room.Height >= minHeight * 2)
                {
                    SplitHorizontally(minWidth, minHeight, roomsQueue, room);
                }
                else
                {
                    room.Width = ((int)(room.Width / 64)) * 64;
                    room.Height = ((int)(room.Height / 64)) * 64;

                    roomsList.Add(room);
                }
            }
        }
    }
    // Makes sure that the generated number of rooms is at least 4 times the ammount of rooms that is required
    if (roomsList.Count >= maxRooms * 4)
    {
        wholeSpace = spaceToSplit;
        return roomsList;
    }
    else
    {
        // If generated rooms is not enough, increase the space's width and height and recursively calls the function again
        spaceToSplit.Width += (int)(spaceToSplit.Width * 0.25);
        spaceToSplit.Height += (int)(spaceToSplit.Height * 0.25);
        return BinarySpacePartioning(spaceToSplit, minWidth, minHeight, maxRooms);
    }
}


/**
 * Horizontally splits a rectangle into 2 randomly sized rectangles
 *//*
private static void SplitHorizontally(int minWidth, int minHeight, Queue<Rectangle> roomsQueue, Rectangle room)
{
    Random rand = new Random();

    int splitSize = rand.Next(0, room.Height);

    roomsQueue.Enqueue(new Rectangle(room.X, room.Y, room.Width, splitSize));

    roomsQueue.Enqueue(new Rectangle(room.X, room.Y + splitSize, room.Width, room.Height - splitSize));
}


/**
 * Vertically splits a rectangle into 2 randomly sized rectangles
 *//*
private static void SplitVertically(int minWidth, int minHeight, Queue<Rectangle> roomsQueue, Rectangle room)
{
    Random rand = new Random();

    int splitSize = rand.Next(0, room.Width);

    roomsQueue.Enqueue(new Rectangle(room.X, room.Y, splitSize, room.Height));

    roomsQueue.Enqueue(new Rectangle(room.X + splitSize, room.Y, room.Width - splitSize, room.Height));
}


/**
 * Takes a list of rectangles, randomly picks a set ammount, then shifts them around randomly.
 *//*
private List<Rectangle> PickAndShift(List<Rectangle> roomsList, Rectangle spaceToSplit, int padding, int minRooms, int maxRooms)
{
    // Sets the roomList list to the temporary one
    roomsList = Pick(roomsList, spaceToSplit, minRooms, maxRooms);

    // Sorts rooms by X
    roomsList.Sort((rect1, rect2) => rect1.X.CompareTo(rect2.X));

    // Shifts the rooms by X using a temporary rectangle
    var shifter = 0;
    Rectangle temp = new Rectangle();

    // Shifts all rectangles in temp on the X axis, with added padding
    for (var i = 0; i < roomsList.Count; i++)
    {
        shifter += 64 * padding;
        temp = roomsList[i];
        temp.X += shifter;
        roomsList[i] = temp;
    }

    // Updates the smallest X, for the room container
    dungeonContainer.X = roomsList[0].X;
    dungeonContainer.Width = (roomsList[roomsList.Count - 1].X + roomsList[roomsList.Count - 1].Width) - roomsList[0].X;

    // Sorts rooms by Y
    roomsList.Sort((rect1, rect2) => rect1.Y.CompareTo(rect2.Y));

    // Shifts the rooms by Y using a temporary rectangle
    shifter = 0;

    // Shifts all rectangles in temp on the Y axis, with added padding
    for (var i = 0; i < roomsList.Count; i++)
    {
        shifter += 64 * padding;
        temp = roomsList[i];
        temp.Y += shifter;
        roomsList[i] = temp;
    }

    // Updates the smallest X, for the room container
    dungeonContainer.Y = roomsList[0].Y;
    dungeonContainer.Height = (roomsList[roomsList.Count - 1].Y + roomsList[roomsList.Count - 1].Height) - roomsList[0].Y;

    var shiftX = 0 - dungeonContainer.X;
    var shiftY = 0 - dungeonContainer.Y;


    // Gets the offset to move the dungeonContainer back to 0,0
    dungeonContainer.X = 0;
    dungeonContainer.Y = 0;

    // Shifts all rooms according to the dungeon container
    for (var i = 0; i < roomsList.Count; i++)
    {
        temp = roomsList[i];
        temp.X += shiftX;
        temp.Y += shiftY;
        roomsList[i] = temp;
    }


    List<Rectangle> tempRoomList = new List<Rectangle>();

    // Adjust all rooms so that they are aligned correctly
    for (int i = 0; i < roomsList.Count; i++)
    {
        tempRoomList.Add(new Rectangle(((int)(roomsList[i].X / 64)) * 64, ((int)(roomsList[i].Y / 64)) * 64, roomsList[i].Width, roomsList[i].Height));
    }

    return tempRoomList;
}


/**
 * Randomly picks rectangles out of a set in a specified area
 */ 
/*private List<Rectangle> Pick(List<Rectangle> roomsList, Rectangle spaceToSplit, int minRooms, int maxRooms)
{
    Random random = new Random();
    // Randomly picks a handful of generated rooms
    dungeonContainer.X = random.Next(0, spaceToSplit.Width - dungeonContainer.Width);
    dungeonContainer.Y = random.Next(0, spaceToSplit.Height - dungeonContainer.Height);

    // Temporary list to hold handful of rooms
    List<Rectangle> tempList = new List<Rectangle>();
    var count = 0;

    // Runs until the amount of rooms is the specified ammount, or until it has run a certain ammount of time
    while (!(tempList.Count >= minRooms && tempList.Count <= maxRooms) && count < 6)
    {
        tempList.Clear();

        // Checks to see if selected rooms are in the dungeon container
        foreach (var r in roomsList)
        {
            if (r.Intersects(dungeonContainer))
            {
                tempList.Add(r);
            }
        }

        // Checks to see if the number of rooms selected are good
        if ((tempList.Count >= minRooms && tempList.Count <= maxRooms))
        {
            return tempList;
        }

        // Either shrinks or grows the container based on the ammount of rooms created
        if (tempList.Count < minRooms)
        {
            dungeonContainer.X += 64 * 2;
            dungeonContainer.Y += 64 * 2;
            dungeonContainer.Width += 64;
            dungeonContainer.Height += 64;
        }
        else if (tempList.Count > maxRooms)
        {
            dungeonContainer.X -= 64 * 2;
            dungeonContainer.Y -= 64 * 2;
            dungeonContainer.Width -= 64;
            dungeonContainer.Height -= 64;
        }

        count++;
    }



    // If the selected number of roomsis the specified number of rooms, return the list
    if ((tempList.Count >= minRooms && tempList.Count <= maxRooms))
    {
        return tempList;
    }
    // Else recursively call to pick different set of rooms
    else
    {
        return Pick(roomsList, spaceToSplit, minRooms, maxRooms);
    }
}

        
    }


public class Room
{
    public Rectangle roomContainer = new Rectangle();
    public Vector2 center = new Vector2();
    public List<Room> connectedRooms = new List<Room>();
    public int ID;

    public Room(Rectangle room, int id)
    {
        roomContainer = room;
        ID = id;
        center = new Vector2(room.X + room.Width / 2, room.Y + room.Height / 2);
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

    public static Rectangle NextFloor(bool xAxis, Rectangle lastFloor, int step)
    {
        // X axis movement
        if (xAxis)
        {
            if (step > 0)
                return new Rectangle(lastFloor.X + 64, lastFloor.Y, 64, 64);
            else
                return new Rectangle(lastFloor.X - 64, lastFloor.Y, 64, 64);
        }
        // Y axis movement
        else
        {
            if (step > 0)
                return new Rectangle(lastFloor.X, lastFloor.Y + 64, 64, 64);
            else
                return new Rectangle(lastFloor.X, lastFloor.Y - 64, 64, 64);
        }
    }
}




public static class WallGenerator
{
    public static HashSet<Vector2> CreateWalls(HashSet<Vector2> floorPositions)
    {
        HashSet<Vector2> wallPositions = new HashSet<Vector2>();

        foreach (var pos in floorPositions)
        {
            foreach (var direction in Direction2D.cardinalDirectionsList)
            {
                var neighborhoodPosition = pos + direction;
                if (floorPositions.Contains(neighborhoodPosition) == false) { wallPositions.Add(neighborhoodPosition); }
            }
        }

        return wallPositions;
    }
}












*/