using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;


namespace TankGame
{
    /// 

    /// This is a game component that implements IUpdateable.
    /// 

    public class FPSCamera
    {

        public Matrix view { get; protected set; }
        public Matrix projection { get; protected set; }

        public Vector3 cameraPosition { get; protected set; }
        Vector3 cameraDirection;
        Vector3 cameraUp;

        private Vector3 targetPosition = Vector3.Zero;
        private float elevation;
        private float rotation;
        private float minDistance;
        private float maxDistance;
        private float viewDistance = 12f;
        private Vector3 baseCameraReference = new Vector3(0, 0, 1);
        private bool needViewResync = true;
        private Matrix cachedViewMatrix;

        MouseState prevMouseState;
        float speed = 1.5f;


        public FPSCamera(GraphicsDevice GraphicsDevice, Vector3 pos, Vector3 target, Vector3 up)      
        {
            //Inizializzo varaibili di posizione
            cameraPosition = pos;
            cameraDirection = target - pos;
            cameraDirection.Normalize();
            cameraUp = up;

            //Rigenero la variabile view
            FPSCreateLookAt();

            //Inizializzo projection
            projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.PiOver4,
            GraphicsDevice.Viewport.AspectRatio,
            0.1f,
            512f);
            needViewResync = true;
        }

        public void FPSCreateLookAt()
        {
            view = Matrix.CreateLookAt(cameraPosition, cameraPosition + cameraDirection, cameraUp);
        }
    }
}