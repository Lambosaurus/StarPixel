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
        bool UPSAMPLE_GRAPHICS = true;


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


        public double time_accelleration = 1.0;
        public double time_update_counter = 0.0;
        public bool paused = false;

        KeyboardState old_keys;

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

            old_keys = Keyboard.GetState();

            universe = new Universe();

            universe.Start();
        }


        /// LoadContent will be called once per game
        protected override void LoadContent()
        {
            AssetShipTemplates.GenerateAssets();
            AssetThrusterTemplates.GenerateAssets();
            AssetWeaponTemplates.GenerateAssets();

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);


            if (UPSAMPLE_GRAPHICS)
            {
                camera = new Camera(GraphicsDevice, spriteBatch, window_res_x * 2, window_res_y * 2);
            }
            else
            {
                camera = new Camera(GraphicsDevice, spriteBatch, window_res_x, window_res_y);
            }


            ColorManager.Load(Content); // it may be important to do this before artmanager.Load, in case I make art assets which need colors


            
            ArtManager.Load(Content);

            cursor = Content.Load<Texture2D>("cursor");
        }

        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        protected override void UnloadContent()
        {

            // probably not the spot for this. but where else?
            universe.End();
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


            KeyboardState new_keys = Keyboard.GetState();
            
            if ( new_keys.IsKeyDown(Keys.P) && !old_keys.IsKeyDown(Keys.P) )
            {
                paused = !paused;
                time_accelleration = (paused) ? 0.0 : 1.0;
            }

            if (new_keys.IsKeyDown(Keys.OemMinus) && !old_keys.IsKeyDown(Keys.OemMinus))
            {
                if (paused)
                {
                    paused = false;
                    time_accelleration = 1.0;
                }
                else
                {
                    time_accelleration /= 1.25;
                }
            }

            if (new_keys.IsKeyDown(Keys.OemPlus) && !old_keys.IsKeyDown(Keys.OemPlus))
            {
                if (paused)
                {
                    paused = false;
                    time_accelleration = 1.0;
                }
                else
                {
                    time_accelleration *= 1.25;
                }
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


            time_update_counter += time_accelleration;
            while (time_update_counter > 1.0)
            {
                time_update_counter--;
                universe.Update();
            }

            if (selectedEntity != null)
            {
                camera.pos = selectedEntity.pos;
            }

            old_keys = new_keys;
            base.Update(gameTime);
        }


        /// This is called when the game should draw itself.
        protected override void Draw(GameTime gameTime)
        {
            camera.Draw(universe);

            

            GraphicsDevice.SetRenderTarget(null); // draw to windows now
            GraphicsDevice.Clear(Color.CornflowerBlue); // so we can see what we forgot TODO: remove this.

            // now we write the cameras result to the screen
            spriteBatch.Begin();


            
            if (UPSAMPLE_GRAPHICS)
            {
                spriteBatch.Draw(camera.surface, new Vector2(0, 0), null, Color.White, 0.0f, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.Draw(camera.surface, new Vector2(0, 0), Color.White);
            }

            spriteBatch.Draw(cursor, mouse_pos - new Vector2(7,7), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
