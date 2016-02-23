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



            Ship playership = CreateNewShip("F2");
            playership.ai = new IntellegenceHuman();
            playership.thrusters.ApplyTemplate("better");
            playership.Paint(Color.Red);


            Ship othership = CreateNewShip("F2");
            othership.ai = new IntellegenceHunter(playership);

            othership.pos = Utility.RandVec(1000);

        }

        // creates a new ship from given template name, and adds it into the universe.
        public Ship CreateNewShip( string template_name )
        {
            Ship ship = AssetShipTemplates.ship_templates[template_name].New();
            entities.Add(ship);
            return ship;
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
                if (Utility.Window(ent.pos, pos))
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


