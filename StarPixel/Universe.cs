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


            
            Ship playership = new Ship();
            playership.ai = new IntellegenceHuman();
            
            entities.Add(playership);

            Ship othership = new Ship();
            othership.ai = new IntellegenceHunter(playership);

            othership.pos = Utility.Rand(1000);

            // these give them some speed advantage...
            othership.thrusters.thrust_temperature = Utility.random.Next(500,2000);

            float advantage = 1f + (othership.thrusters.thrust_temperature / 10000f);
            othership.thrusters.manouvering_thrust *= advantage;
            othership.thrusters.main_thrust *= advantage;

            entities.Add(othership);


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
            // this is done separately for a good reason. probably.
            for (int i = entities.Count - 1; i >= 0; i--)
            {
                if ( entities[i].destroyed)
                {
                    entities.RemoveAt(i);
                }
            }
        }

        public Entity OnClick(Vector2 pos)
        {
            foreach (Entity ent in entities)
            {
                if (Utility.CompareVector2(ent.pos, pos))
                {
                    ent.OnClick();

                    return ent;
                }
                else
                {
                    ent.Selected = false;
                }

            }

            return null;

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


