using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Text;

namespace NotAnMMO
{
    class Collider
    {
        // Vars for the Collider object
        private Vector2 colliderPos;

        // Circle Collider Vars
        private int colliderRadius;
        
        // Rectangle Collider Vars
        public Rectangle colliderRectangle;


        /**
         * Collider Constructor, constructs either a rectangle or a circle collider depending on whether it is
         * a being (player/enemy) or not.
         */
        public Collider(bool being, Vector2 pos, int radius, int h, int w)
        {
            if (being)
            {
                colliderPos = pos;
                colliderRadius = radius;
                colliderRectangle = new Rectangle(0, 0, 0, 0);
            }
            else
            {
                colliderPos = pos;
                colliderRadius = 0;
                colliderRectangle = new Rectangle((int)pos.X, (int)pos.Y, w, h);
            }
        }

        /**
         * Position setter
         */
        public Vector2 Position
        {
            set { colliderPos = value; }
        }


        /**
         * Rectangle getter
         */
        public Rectangle Rectangle
        {
            get { return colliderRectangle; }
        }


        /**
         * Returns true if there is collision between the circle and the rectangle, else returns false
         */
        public bool collision(Vector2 circleCenter, Rectangle rectangle)
        {
            // Calculate the closest point on the rectangle to the center of the circle
            float closestX = MathHelper.Clamp(circleCenter.X, rectangle.Left, rectangle.Right);
            float closestY = MathHelper.Clamp(circleCenter.Y, rectangle.Top, rectangle.Bottom);

            // Calculate the distance between the closest point and the center of the circle
            float distanceX = circleCenter.X - closestX;
            float distanceY = circleCenter.Y - closestY;
            float distanceSquared = (distanceX * distanceX) + (distanceY * distanceY);

            // Check if the distance is less than or equal to the circle's radius squared
            float radiusSquared = this.colliderRadius * this.colliderRadius;
            return distanceSquared <= radiusSquared;
        }

        /**
         * Adjusts the given position to remove collision between a circle and a rectangle, by calculating the depth of the collision
         */
        public Vector2 ResolveCollision(Vector2 newPosition, Rectangle rectangle)
        {
            // Calculate the intersection depth between the circle and the rectangle
            Vector2 intersectionDepth = CalculateIntersectionDepth(this.colliderPos, newPosition, this.colliderRadius, rectangle);

            // Adjust the new position by the intersection depth to resolve the collision
            newPosition += intersectionDepth;

            return newPosition;
        }

        /**
         * Calculates the depth of collision between a circle and a rectangle
         */
        private Vector2 CalculateIntersectionDepth(Vector2 currentPosition, Vector2 newPosition, float radius, Rectangle rectangle)
        {
            // Calculate the half dimensions of the rectangle
            float halfWidth = rectangle.Width / 2f;
            float halfHeight = rectangle.Height / 2f;

            // Calculate the center of the rectangle
            Vector2 center = new Vector2(rectangle.X + halfWidth, rectangle.Y + halfHeight);

            // Calculate the closest point on the rectangle to the current position
            float closestX = MathHelper.Clamp(currentPosition.X, rectangle.Left, rectangle.Right);
            float closestY = MathHelper.Clamp(currentPosition.Y, rectangle.Top, rectangle.Bottom);

            // Check if the current position is inside the rectangle
            bool inside = currentPosition.X >= rectangle.Left && currentPosition.X <= rectangle.Right &&
                          currentPosition.Y >= rectangle.Top && currentPosition.Y <= rectangle.Bottom;

            if (inside)
            {
                // If the current position is inside the rectangle, use the previous position to calculate intersection depth
                Vector2 previousClosest = new Vector2(MathHelper.Clamp(newPosition.X, rectangle.Left, rectangle.Right),
                                                      MathHelper.Clamp(newPosition.Y, rectangle.Top, rectangle.Bottom));

                float depthX = Math.Abs(previousClosest.X - currentPosition.X);
                float depthY = Math.Abs(previousClosest.Y - currentPosition.Y);

                if (depthX < radius && depthY < radius)
                {
                    float dx = radius - depthX;
                    float dy = radius - depthY;

                    if (dx < dy)
                    {
                        return new Vector2(Math.Sign(newPosition.X - currentPosition.X) * dx, 0);
                    }
                    else
                    {
                        return new Vector2(0, Math.Sign(newPosition.Y - currentPosition.Y) * dy);
                    }
                }
            }
            else
            {
                // Calculate the distance between the closest point and the current position
                float distanceSquared = (currentPosition.X - closestX) * (currentPosition.X - closestX) +
                                        (currentPosition.Y - closestY) * (currentPosition.Y - closestY);

                if (distanceSquared < radius * radius)
                {
                    float distance = (float)Math.Sqrt(distanceSquared);
                    float penetration = radius - distance;

                    return new Vector2(penetration * (currentPosition.X - closestX) / distance,
                                       penetration * (currentPosition.Y - closestY) / distance);
                }
            }

            return Vector2.Zero; // No intersection
        }
    }
}
