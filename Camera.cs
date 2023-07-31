using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Text;

namespace NotAnMMO
{
    public class Camera
    {
        private Matrix transform;
        public Matrix Transform
        {
            get { return transform; }
        }

        private Vector2 center;
        private Viewport viewport;

        //private float zoom = 0.8f;
        private float zoom = 0.5f;
        private float rotation = 0;

        public float X
        {
            get { return center.X; }
        }

        public float Y
        {
            get { return center.Y; }
        }

        public float Zoom
        {
            get { return zoom; }
            set
            {
                zoom = value;
                /*if (zoom < 0.1f)
                    zoom = 0.1f;*/
            }
        }

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public Viewport CameraView
        {
            get { return viewport; }
        }


        public Camera(Viewport newViewport)
        {
            viewport = newViewport;
        }

        public void Update(Vector2 position, GraphicsDevice graphics)
        {
            center = new Vector2(position.X, position.Y);

            transform = Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0)) * 
                        //Matrix.CreateRotationZ(Rotation) *
                        Matrix.CreateScale(new Vector3(Zoom, Zoom, 0)) * 
                        Matrix.CreateTranslation(new Vector3(viewport.Width / 2, viewport.Height / 2, 0));
        }
    }
}
