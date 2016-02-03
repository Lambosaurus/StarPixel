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
    public class Universe
    {
        public List<Entity> entities;

        public Universe()
        {
            entities = new List<Entity>();
        }

        public void Start()
        {
        }

        public void End()
        {
        }

        public void Update()
        {
            foreach (Entity ent in entities)
            {
                ent.Update();
            }


            // remove entities that are good to be removed.
            // The reason this should be done in a separate
            for (int i = entities.Count - 1; i >= 0; i--)
            {
                if ( entities[i].destroyed)
                {
                    entities.RemoveAt(i);
                }
            }
        }

        public void Draw( Camera camera )
        {
            foreach (Entity ent in entities)
            {
                ent.Draw(camera);
            }
        }
    }
}


