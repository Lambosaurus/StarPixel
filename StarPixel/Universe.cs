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
        List<Physical> new_physicals = new List<Physical>();

        public List<Projectile> projectiles = new List<Projectile>();
        List<Projectile> new_projectiles = new List<Projectile>();
        
        List<ArtTemporary> art_temp = new List<ArtTemporary>();
        List<ArtTemporary> new_art_temp = new List<ArtTemporary>();
       

        // creates a new ship from given template name, and adds it into the universe.
        public Ship CreateNewShip( string template_name )
        {
            Ship ship = AssetShipTemplates.ship_templates[template_name].New(this);
            AddPhysical(ship);
            return ship;
        }


        public void AddPhysical( Physical phys )
        {
            new_physicals.Add(phys);
        }
        public void AddProjectile(Projectile proj)
        {
            new_projectiles.Add(proj);
        }
        public void AddArtTemp(ArtTemporary art_temp)
        {
            new_art_temp.Add(art_temp);
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


            for (int i = 0; i < 200; i++)
            {
                Ship ship1 = CreateNewShip("F2");
                ship1.ai = new IntellegenceRoamer();
                ship1.MountThruster( Utility.RandBool() ? "default" : (Utility.RandBool() ? "worse" : "better") );
                ship1.MountShield("green");
                ship1.MountArmor("default");
                ship1.pos = Utility.CosSin(Utility.RandAngle(), Utility.Rand(1000, 3000));

                ship1.MountWeapon("shooter", 0);

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
            
            // Update our things
            foreach (Physical phys in physicals) { phys.Update(); }
            foreach (Projectile proj in projectiles) { proj.Update(); }
            foreach (ArtTemporary art in art_temp) { art.Update(); }


            // Sort our lists for the collison detection.
            //physicals = physicals.OrderBy(x => x.leftmost).ToList();
            //projectiles = projectiles.OrderBy(x => x.pos.X).ToList();
            if (physicals.Count != 0) { physicals.TimSort(Physical.CompareByLeftmost); }
            if (projectiles.Count != 0) { projectiles.TimSort(Projectile.CompareByX); }


            // Our nifty sort based projectile to physical collision detection
            int z = 0;
            for (int i = 0; i < projectiles.Count && z < physicals.Count; i++)
            {
                while (physicals[z].rightmost < projectiles[i].pos.X)
                {
                    if (++z >= physicals.Count)
                    {
                        break;
                    }
                }
                if (z >= physicals.Count) { break; }
                
                // check projectile against all targets.
                for (int k = z; k < physicals.Count; k++)
                {
                    if (projectiles[i].pos.X < physicals[k].leftmost)
                    {
                        break;
                    }

                    if (physicals[k].hitbox.WithinRadius(projectiles[i].pos))
                    {
                        if (projectiles[i].HitCheck(this, physicals[k]))
                        {
                            break; // hit already found. We done here.
                        }
                    }
                }
            }



            for (int i = 0; i < physicals.Count; i++)
            {
                for (int k = i + 1; k < physicals.Count; k++)
                {
                    if (physicals[k].leftmost > physicals[i].rightmost)
                    {
                        break;
                    }
                    if (physicals[i].hitbox.WithinRadius(physicals[k].hitbox)) { physicals[i].HitCheck(physicals[k]); }
                }
            }


            MaintainEntityLists();
        }

        void MaintainEntityLists()
        {
            // clear items that have been flagged as dead.
            for (int i = physicals.Count - 1; i >= 0; i--)
            {
                if (physicals[i].ReadyForRemoval())
                {
                    physicals.RemoveAt(i);
                }
            }

            // If many items are getting removed, its just faster to get a new list.
            List<Projectile> remaining_projectiles = new List<Projectile>();
            foreach (Projectile proj in projectiles)
            {
                if (!proj.ReadyForRemoval())
                {
                    remaining_projectiles.Add(proj);
                }
            }
            projectiles = remaining_projectiles;

            List<ArtTemporary> remaining_art_temp = new List<ArtTemporary>();
            foreach ( ArtTemporary art in art_temp )
            {
                if (!art.ReadyForRemoval())
                {
                    remaining_art_temp.Add(art);
                }
            }
            art_temp = remaining_art_temp;



            // Add items that have been created this frame.
            foreach ( Physical phys in new_physicals )
            {
                phys.NewObjectUpdate();
                physicals.Add(phys);
            }
            new_physicals.Clear();
            
            foreach (Projectile proj in new_projectiles)
            {
                proj.NewObjectUpdate();
                projectiles.Add(proj);
            }
            new_projectiles.Clear();

            foreach (ArtTemporary art in new_art_temp)
            {
                art_temp.Add(art);
            }
            new_art_temp.Clear();
            
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


