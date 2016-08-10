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
        public List<Physical> physicals = new List<Physical>();
        int physcount = 0;

        public List<Projectile> projectiles = new List<Projectile>();
        int projcount = 0;

        public List<ArtTemporary> art_temp = new List<ArtTemporary>();
        int tempcount = 0;
       

        // creates a new ship from given template name, and adds it into the universe.
        public Ship CreateNewShip( string template_name )
        {
            Ship ship = AssetShipTemplates.ship_templates[template_name].New(this);
            physicals.Add(ship);
            return ship;
        }
        
        public Physical PhysAtPoint( Vector2 point )
        {
            foreach ( Physical phys in physicals )
            {
                if ( (point - phys.pos).Length() < phys.radius )
                {
                    return phys;
                }
            }
            return null;
        }

        public void Start()
        {
            /*
            Ship playership = CreateNewShip("F2");
            playership.ai = new IntellegenceHuman();
            playership.MountThruster("better");
            playership.Paint(Color.Red);
            playership.MountShield("default");
            playership.MountWeapon("shooter", 0);
            playership.MountWeapon("shooter", 1);
            playership.MountArmor("default");

            
            Ship ship0 = CreateNewShip("F2");
            ship0.ai = new IntellegenceHunter(playership);
            ship0.MountThruster("default");
            ship0.MountShield("default");
            ship0.MountArmor("default");
            ship0.Paint(Color.Blue);
            ship0.pos = Utility.CosSin( Utility.RandAngle(),  Utility.Rand(200,300)  );
            ship0.MountWeapon("shooter", 0);
            ship0.MountWeapon("shooter", 1);
            */

            Ship broship = CreateNewShip("F2");
            broship.ai = new IntellegenceRoamer();
            broship.MountThruster("better");
            broship.Paint(Color.Red);
            broship.MountShield("default");
            broship.MountArmor("default");


            for (int i = 0; i < 100; i++)
            {
                Ship ship1 = CreateNewShip("F2");
                ship1.ai = new IntellegenceRoamer();
                ship1.MountThruster( Utility.RandBool() ? "default" : (Utility.RandBool() ? "worse" : "better") );
                ship1.MountShield("green");
                ship1.MountArmor("default");
                ship1.pos = Utility.CosSin(Utility.RandAngle(), Utility.Rand(1000, 3000));

                ship1.Paint(new Color(Utility.Rand(0.0f), Utility.Rand(1.0f), Utility.Rand(1.0f)));
            }

            for (int i = 0; i < 25; i++)
            {
                Ship ship2 = CreateNewShip("CG1");
                ship2.ai = new IntellegenceRoamer(0.2f);
                ship2.MountThruster(Utility.RandBool() ? "default" : (Utility.RandBool() ? "worse" : "better"));
                ship2.MountShield("default");
                ship2.MountArmor("default");
                ship2.Paint( new Color(0.0f, Utility.Rand(1.0f), Utility.Rand(1.0f) ));
                ship2.pos = Utility.CosSin(Utility.RandAngle(), Utility.Rand(3000, 4000));
            }
        }

        public void End()
        {
        }

        public void Update()
        {
            // we use physcount, not the actual physicals.count, so we dont update newly added objects
            for( int i = 0; i < physcount; i++ )
            {
                physicals[i].Update();
            }

            for (int i = 0; i < projcount; i++)
            {
                projectiles[i].Update();

                // check projectile against all targets.
                for (int k = 0; k < physcount; k++)
                {
                    if (projectiles[i].HitCheck(this, physicals[k]))
                    {
                        break; // hit already found. We done here.
                    }
                }
            }

            // check each physical for collisions
            for (int i = 0; i < physcount; i++)
            {
                // dont check physicals that have already check you.
                for (int k = i + 1; k < physcount; k++)
                {
                    physicals[i].HitCheck(physicals[k]);
                }
            }
            
            for (int i = 0; i < tempcount; i++)
            {
                art_temp[i].Update();
            }

            this.WelcomeNewEntities();
            this.MaintainEntityLists();   
        }


        void WelcomeNewEntities()
        {
            for ( int i = physcount; i < physicals.Count; i++)
            {
                physicals[i].NewObjectUpdate();
            }
            
            for (int i = projcount; i < projectiles.Count; i++)
            {
                projectiles[i].NewObjectUpdate();
            }
        }

        // maintains the lists of entities
        // culling entities that are ready to die, and updating counts
        void MaintainEntityLists()
        {
            // remove physicals that are good to be removed.
            // this is done separately for a good reason. probably.
            for (int i = physicals.Count - 1; i >= 0; i--)
            {
                if (physicals[i].ReadyForRemoval())
                {
                    physicals.RemoveAt(i);
                }
            }
            physcount = physicals.Count;


            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                if (projectiles[i].ReadyForRemoval())
                {
                    projectiles.RemoveAt(i);
                }
            }
            projcount = projectiles.Count;


            for (int i = art_temp.Count - 1; i >= 0; i--)
            {
                if (art_temp[i].ReadyForRemoval())
                {
                    art_temp.RemoveAt(i);
                }
            }
            tempcount = art_temp.Count;

        }

        public Entity OnClick(Vector2 pos)
        {
            foreach (Physical phys in physicals)
            {
                if (Utility.Window(phys.pos, pos, 20))
                {
                    phys.OnClick();

                    return phys;
                }
                else
                {
                    phys.Selected = false;
                }

            }

            return null;

        }



        public void Draw( Camera camera)
        {
           

            foreach (Physical phys in physicals)
            {
                phys.Draw(camera);
            }

            foreach (Projectile proj in projectiles)
            {
                proj.Draw(camera);
            }

            foreach (ArtTemporary temp in art_temp)
            {
                temp.Draw(camera);
            }

            if ( camera.DRAW_HITBOXES )
            {
                foreach (Physical phys in physicals)
                {
                    phys.hitbox.Draw(camera);
                }
            }
        }
    }
}


