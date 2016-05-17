using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TankGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class TankGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Point screenCenter;
        Point saveMousePoint;
        bool moveMode = false;
        float scrollRate = 1.0f;
        MouseState previousMouse;

        Terrain terrain;
        Effect effect;

        Tank tank;
        float canonRot = 0;
        float turretRot = 0;
        float wheelRot = 0;
        float steelRot = 0;
        float BodyRot = 0;
        Matrix CanonRelTransform, tankTransform;


        Camera camera;
        MouseState LastMouseState;

        public TankGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            screenCenter.X = this.Window.ClientBounds.Width / 2;
            screenCenter.Y = this.Window.ClientBounds.Height / 2;
            this.IsMouseVisible = true;
            previousMouse = Mouse.GetState();
            Mouse.SetPosition(screenCenter.X, screenCenter.Y);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            camera = new Camera (new Vector3(64f, 16f, 64f),MathHelper.ToRadians(-30),0f,32f,192f,128f,GraphicsDevice.Viewport.AspectRatio,0.1f,512f);
            LastMouseState = Mouse.GetState();
     
            terrain = new Terrain(GraphicsDevice,Content.Load<Texture2D>("heightmap"),Content.Load<Texture2D>("erba"), Content.Load<Texture2D>("muschio"), Content.Load<Texture2D>("vetta"),1f,128,128,15f);

            effect = Content.Load<Effect>("terrain");

            tank = new Tank(Content.Load<Model>("tank"), new Vector3(61, terrain.GetHeight(61, 61), 61),
                                new Quaternion(), new Vector3(0.01f, 0.01f, 0.01f), GraphicsDevice);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState KState = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (this.IsActive)
            {
                MouseState mouse = Mouse.GetState();
                if (moveMode)
                {
                    camera.Rotation += MathHelper.ToRadians(
                    (mouse.X - screenCenter.X) / 2f);
                    camera.Elevation += MathHelper.ToRadians(
                    (mouse.Y - screenCenter.Y) / 2f);
                    Mouse.SetPosition(screenCenter.X, screenCenter.Y);
                }
                if (mouse.RightButton == ButtonState.Pressed)
                {
                    if (!moveMode &&
                    previousMouse.RightButton == ButtonState.Released)
                    {
                        if (graphics.GraphicsDevice.Viewport.Bounds.Contains(
                        new Point(mouse.X, mouse.Y)))
                        {
                            moveMode = true;
                            saveMousePoint.X = mouse.X;
                            saveMousePoint.Y = mouse.Y;
                            Mouse.SetPosition(screenCenter.X, screenCenter.Y);
                            this.IsMouseVisible = false;
                        }
                    }
                }
                else
                {
                    if (moveMode)
                    {
                        moveMode = false;
                        Mouse.SetPosition(saveMousePoint.X, saveMousePoint.Y);
                        this.IsMouseVisible = true;
                    }
                }
                if (mouse.ScrollWheelValue - previousMouse.ScrollWheelValue !=
                0)
                {
                    float wheelChange = mouse.ScrollWheelValue -
                    previousMouse.ScrollWheelValue;
                    camera.ViewDistance -= (wheelChange / 120) * scrollRate;
                }
                previousMouse = mouse;
            }

            if (KState.IsKeyDown(Keys.U))
            {
                canonRot -= 0.05f;
                tank.BoneTransform(10, Matrix.CreateRotationX(canonRot));
            }

            if (KState.IsKeyDown(Keys.J))
            {
                canonRot += 0.05f;
                tank.BoneTransform(10, Matrix.CreateRotationX(canonRot));
            }
            if (KState.IsKeyDown(Keys.L))
            {
                turretRot += 0.05f;
                tank.BoneTransform(9, Matrix.CreateRotationY(turretRot));
            }
            if (KState.IsKeyDown(Keys.R))
            {
                turretRot -= 0.05f;
                tank.BoneTransform(9, Matrix.CreateRotationY(turretRot));
            }
            if (KState.IsKeyDown(Keys.Left))
            {
                steelRot += 0.05f;
                if (steelRot > 1.5f)
                    steelRot = 1.5f;
                tank.BoneTransform(3, Matrix.CreateRotationY(steelRot));
                tank.BoneTransform(7, Matrix.CreateRotationY(steelRot));
            }
            if (KState.IsKeyDown(Keys.Right))
            {
                steelRot -= 0.05f;
                if (steelRot < -1.5f)
                    steelRot = -1.5f;
                tank.BoneTransform(3, Matrix.CreateRotationY(steelRot));
                tank.BoneTransform(7, Matrix.CreateRotationY(steelRot));
            }
            if (KState.IsKeyDown(Keys.Down))
            {
                wheelRot -= 0.05f;
                tank.BoneTransform(2, Matrix.CreateRotationX(wheelRot));
                tank.BoneTransform(4, Matrix.CreateRotationX(wheelRot));
                tank.BoneTransform(6, Matrix.CreateRotationX(wheelRot));
                tank.BoneTransform(8, Matrix.CreateRotationX(wheelRot));

                BodyRot -= delta * steelRot;
                tank.BoneTransform(0, Matrix.CreateRotationY(BodyRot));
                tankTransform = GetTransformPaths(tank.Model.Bones[0]);
                Vector3 scale;
                Quaternion rotation;
                Vector3 translation;
                tankTransform.Decompose(out scale, out rotation, out translation);
                Vector3 tankForward = Vector3.Transform(Vector3.UnitZ, rotation);
                tank.Position += translation - tankForward * 0.03f;
                tank.Position = new Vector3(tank.Position.X, terrain.GetHeight(tank.Position.X, tank.Position.Z), tank.Position.Z);
            }
            if (KState.IsKeyDown(Keys.Up))
            {
                wheelRot += 0.05f;
                tank.BoneTransform(2, Matrix.CreateRotationX(wheelRot));
                tank.BoneTransform(4, Matrix.CreateRotationX(wheelRot));
                tank.BoneTransform(6, Matrix.CreateRotationX(wheelRot));
                tank.BoneTransform(8, Matrix.CreateRotationX(wheelRot));


                BodyRot += delta * steelRot;
                tank.BoneTransform(0, Matrix.CreateRotationY(BodyRot));
                tankTransform = GetTransformPaths(tank.Model.Bones[0]);
                Vector3 scale;
                Quaternion rotation;
                Vector3 translation;
                tankTransform.Decompose(out scale, out rotation, out translation);
                Vector3 tankForward = Vector3.Transform(Vector3.UnitZ, rotation);
                tank.Position += translation + tankForward * 0.03f;
                tank.Position = new Vector3(tank.Position.X,terrain.GetHeight(tank.Position.X, tank.Position.Z), tank.Position.Z);
            }


            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            terrain.Draw(camera, effect);
            tank.Draw(camera.View, camera.Projection);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        private Matrix GetTransformPaths(ModelBone bone)
        {
            Matrix result = Matrix.Identity;
            while (bone != null)
            {
                result = result * bone.Transform;
                bone = bone.Parent;
            }
            return result;

        }
    }
}
