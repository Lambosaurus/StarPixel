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
        public List<Entity> entities = new List<Entity>();

        public List<Projectile> projectiles = new List<Projectile>();


        public Universe()
        {

        }

        // creates a new ship from given template name, and adds it into the universe.
        public Ship CreateNewShip( string template_name )
        {
            Ship ship = AssetShipTemplates.ship_templates[template_name].New(this);
            entities.Add(ship);
            return ship;
        }
        

        public void Start()
        {


            Ship playership = CreateNewShip("F2");
            playership.ai = new IntellegenceHuman();
            playership.thrusters.ApplyTemplate("better");
            playership.Paint(Color.Red);

            playership.MountWeapon("shooter", 0);
            playership.MountWeapon("shooter", 1);


            Ship othership = CreateNewShip("F2");
            othership.ai = new IntellegenceRoamer();

            othership.pos = Utility.RandVec(1000);

            Ship othership2 = CreateNewShip("F2");
            othership2.ai = new IntellegenceHunter(othership);
            othership2.thrusters.ApplyTemplate("worse");
            othership2.Paint(Color.Blue);

            othership2.pos = Utility.RandVec(1000);


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

            foreach (Projectile proj in projectiles)
            {
                proj.Update();

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

            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                if (projectiles[i].destroyed)
                {
                    projectiles.RemoveAt(i);
                }
            }
        }

        public Entity OnClick(Vector2 pos)
        {
            foreach (Entity ent in entities)
            {
                if (Utility.Window(ent.pos, pos, 20))
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

            foreach (Projectile proj in projectiles)
            {
                proj.Draw(camera);
            }
        }
    }
}


