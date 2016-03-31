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



    public class Entity
    {
        public Vector2 pos;
        public Vector2 velocity;
        public float angle;
        private bool selected = false;

        bool newly_spawned;

        private void SelectMe(object sender, EventArgs eventArgs)
        {
            selected = !selected;
        }

        public bool destroyed;

        public bool Selected
        {
            set
            {
                selected = false;
            }
        }

        public event EventHandler Clicked;

        public Entity()
        {
            pos = new Vector2(0, 0);
            velocity = new Vector2(0, 0);
            angle = 0.0f;

            Clicked += SelectMe;
            destroyed = false;

            newly_spawned = true;
        }

        public void OnClick()
        {
            if (Clicked != null)
            {
                Clicked(this, EventArgs.Empty);
            }
            
        }

        public void NewObjectUpdate()
        {
            pos -= velocity;
            this.Update();
        }
        
        public virtual void Update()
        {
            pos += velocity;
        }

        public virtual void Draw(Camera camera)
        {
        }

        public void Destory( )
        {
            if ( ! destroyed )
            {

            }

            destroyed = true;
        }

        public bool ReadyForRemoval()
        {
            return destroyed;
        }
    }
}



