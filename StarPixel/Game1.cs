using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace StarPixel
{
    /// This is the main type for your game
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Camera camera;
        Camera camera2;

        Universe universe;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        protected override void Initialize()
        {
            base.Initialize();


            universe = new Universe();
            universe.entities.Add(new Ship());

        }


        /// LoadContent will be called once per game
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            camera = new Camera(GraphicsDevice, spriteBatch, 200, 200);
            camera2 = new Camera(GraphicsDevice, spriteBatch, 200, 200);

            ArtManager.Load(Content);

        }

        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        protected override void UnloadContent()
        {

        }
        

        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                this.Exit();
            }


            universe.Update();


            base.Update(gameTime);
        }
        

        /// This is called when the game should draw itself.
        protected override void Draw(GameTime gameTime)
        {
            camera.BeginRender();
            universe.Draw(camera);
            camera.EndRender();

            camera2.scale = 2;
            camera2.pos = new Vector2(20,20);

            camera2.BeginRender();
            universe.Draw(camera2);
            camera2.EndRender();


            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            spriteBatch.Draw(camera.surface, new Vector2(20, 20), Color.White);
            spriteBatch.Draw(camera2.surface, new Vector2(240, 20), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
