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

        // meh. Its a semi comfortable res for developing in.
        int window_res_x = 1200;
        int window_res_y = 800;

        Camera camera;
        int scrollVal = 0;
        Vector2 mouse_pos = new Vector2(0, 0);
        Texture2D cursor;
        ButtonState mouse_left = Mouse.GetState().LeftButton;
        Universe universe;
        Entity selectedEntity = null;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = window_res_y;
            graphics.PreferredBackBufferWidth = window_res_x;

            Content.RootDirectory = "Content";
        }

        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        protected override void Initialize()
        {
            base.Initialize();


            universe = new Universe();

        }


        /// LoadContent will be called once per game
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            camera = new Camera(GraphicsDevice, spriteBatch, window_res_x, window_res_y);

            ColorManager.Load(Content); // it may be important to do this before artmanager.Load, in case I make art assets which need colors

            ArtManager.sprites.Add("ship", new ArtSpriteResource("ship", 0.2f));

            ArtVentResource sparkles = new ArtVentResource("particle");
            sparkles.std_ejection_temperature = 2000;
            sparkles.std_particle_count = 30;
            sparkles.std_particle_length = 0.75f;
            sparkles.std_particle_stretch = 4f;
            sparkles.std_particle_life = 5f;
            sparkles.std_particle_width = 0.75f;
            sparkles.std_temperature_scatter = 0;
            sparkles.std_temp_halflife = 3f;
            sparkles.std_velocity_scatter = 0.1f;
            sparkles.std_ejection_velocity = 1f;

            ArtManager.vents.Add("sparkles", sparkles );

            
            ArtManager.Load(Content);

            cursor = Content.Load<Texture2D>("cursor");
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
            //if scroll has been used, zoom in/out
            if (Mouse.GetState().ScrollWheelValue > scrollVal)
            {
                camera.scale *= 1.25f;
            }
            else if(Mouse.GetState().ScrollWheelValue < scrollVal)
            {
                camera.scale /= 1.25f;
            }
            scrollVal = Mouse.GetState().ScrollWheelValue;

            Vector2 new_pos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            //if right button is pressed, move camera
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                
                if (new_pos != mouse_pos)
                {
                    Vector2 delta = (mouse_pos - new_pos) / camera.scale;
                    camera.pos += delta;
                }
            }
            mouse_pos = new_pos;

            if (mouse_left == ButtonState.Pressed && Mouse.GetState().LeftButton == ButtonState.Released)
            {
                selectedEntity = universe.OnClick(camera.InverseMap(new Vector2(Mouse.GetState().X, Mouse.GetState().Y)));
            }
            mouse_left = Mouse.GetState().LeftButton;
            universe.Update();
            if (selectedEntity != null)
            {
                camera.pos = selectedEntity.pos;
            }

            base.Update(gameTime);
        }

        int time = 0;

        /// This is called when the game should draw itself.
        protected override void Draw(GameTime gameTime)
        {
            camera.Draw(universe);

            

            GraphicsDevice.SetRenderTarget(null); // draw to windows now
            GraphicsDevice.Clear(Color.CornflowerBlue); // so we can see what we forgot TODO: remove this.

            // now we write the cameras result to the screen
            spriteBatch.Begin();

            spriteBatch.Draw(camera.surface, new Vector2(0, 0), Color.White);

            spriteBatch.Draw(cursor, mouse_pos - new Vector2(7,7), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
