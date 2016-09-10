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
        float maximum_physical_radius = 100;

        public List<Physical> physicals = new List<Physical>();
        List<Physical> new_physicals = new List<Physical>();

        public List<Physical> physical_leftmost = new List<Physical>();

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
            if (phys.radius > maximum_physical_radius)
            {
                maximum_physical_radius = phys.radius;
            }
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
            float upper_search_pos = point.X + maximum_physical_radius;
            float lower_search_pos = point.X - maximum_physical_radius;

            int i = Utility.BinarySearch(physicals, lower_search_pos, x => x.pos.X);
            
            while (i < physicals.Count && physicals[i].pos.X < upper_search_pos)
            {
                Physical phys = physicals[i++];
                if ((point - phys.pos).LengthSquared() < phys.radius_sq) { return phys; }
            }
            
            return null;
        }
        
        public List<Physical> PhysInBox(Vector2 lower, Vector2 upper)
        {
            List<Physical> in_box = new List<Physical>();

            int i = Utility.BinarySearch(physicals, lower.X, x => x.pos.X);
            
            while ( i < physicals.Count && physicals[i].pos.X < upper.X )
            {
                Physical phys = physicals[i++];
                if (phys.pos.Y > lower.Y && phys.pos.Y < upper.Y) { in_box.Add(phys); }
            }
            return in_box;
        }

        public List<Physical> PhysicalsInRadius(Vector2 point, float radius)
        {
            float upper_search_pos = point.X + radius;
            float lower_search_pos = point.X - radius;
            float radius_sq = radius* radius;

            List<Physical> in_rad = new List<Physical>();

            int i = Utility.BinarySearch(physicals, lower_search_pos, x => x.pos.X);

            while (i < physicals.Count && physicals[i].pos.X < upper_search_pos)
            {
                Physical phys = physicals[i++];
                if ((point - phys.pos).LengthSquared() < radius_sq) { in_rad.Add(phys); }
            }

            return in_rad;
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

            Ship broship = CreateNewShip("CG1");
            broship.MountThruster("default");
            broship.Paint(Color.Red);
            broship.MountShield("default");
            broship.MountArmor("default");

            broship.MountWeapon("shooter", 0);
            broship.MountWeapon("shooter", 1);
            broship.MountWeapon("shooter", 2);
            broship.MountWeapon("shooter", 3);
            broship.MountWeapon("shooter", 4);
            broship.MountWeapon("shooter", 5);


            Ship ship2 = null;
            for (int i = 0; i < 80; i++)
            {
                Ship ship1 = CreateNewShip("F2");
                ship1.ai = new IntellegenceRoamer();
                ship1.MountThruster( Utility.RandBool() ? "default" : (Utility.RandBool() ? "worse" : "better") );
                ship1.MountShield("default");
                ship1.MountArmor("default");
                ship1.pos = Utility.CosSin(Utility.RandAngle(), Utility.Rand(1000, 3000));
                
                ship1.Paint(new Color(Utility.Rand(0.0f), Utility.Rand(1.0f), Utility.Rand(1.0f)));
             
                ship2 = CreateNewShip("F2");
                ship2.ai = new IntellegenceHunter(ship1);
                ship2.MountThruster(Utility.RandBool() ? "default" : (Utility.RandBool() ? "worse" : "better"));
                ship2.MountShield("green");
                ship2.MountArmor("default");
                ship2.pos = Utility.CosSin(Utility.RandAngle(), Utility.Rand(500, 2000));

                ship2.MountWeapon("shooter", 0);
                if (Utility.RandBool()) { ship2.MountWeapon("shooter", 1); }
                
                ship2.Paint(new Color(Utility.Rand(0.0f), Utility.Rand(1.0f), Utility.Rand(1.0f)));
            }
            
            broship.ai = new IntellegenceHunter( ship2 );

            for (int i = 0; i < 20; i++)
            {
                ship2 = CreateNewShip("CG1");
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
            
            CollisionChecks();
   
            MaintainEntityLists();
        }

        void CollisionChecks()
        {
            // Sort our lists for the collison detection.
            if (physicals.Count != 0) { physicals.TimSort(Physical.CompareByX); }
            physical_leftmost = physicals.OrderBy(x => x.leftmost).ToList();

            if (projectiles.Count != 0) { projectiles.TimSort(Projectile.CompareByX); }
            
            
            // Our nifty sort based projectile to physical collision detection
            int z = 0;
            foreach (Projectile proj in projectiles)
            {
                // This particle (and therefore all particles past this point) cannot collide
                // if rightmost point is to the left of us.
                while (physical_leftmost[z].rightmost < proj.pos.X)
                {
                    if (++z >= physical_leftmost.Count) // We are out of collide targets. Stop
                    {
                        break;
                    }
                }
                if (z >= physical_leftmost.Count) { break; } // THats it, keep stopping.


                // we start checking physicals from z onwards
                for (int k = z; k < physical_leftmost.Count; k++)
                {
                    Physical phys = physical_leftmost[k];

                    // if the leftmost point is to our right, then this and future phys cannot collide with us
                    if (proj.pos.X < phys.leftmost)
                    {
                        break;
                    }

                    if ( (phys.pos - proj.pos).LengthSquared() < phys.radius_sq ) // a prelim check before we open up the abstraction
                    {
                        if (proj.HitCheck(this, phys))
                        {
                            break; // hit already found. We done here.
                        }
                    }
                }
            }



            for (int i = 0; i < physical_leftmost.Count; i++)
            {
                Physical phys_a = physical_leftmost[i];

                // only check forwards in the list. Checking backwards will double up our comparasons.
                for (int k = i + 1; k < physical_leftmost.Count; k++)
                {
                    Physical phys_b = physical_leftmost[k];

                    // if the leftmost point is to our right, then this and future phys cannot collide with us
                    if (phys_a.rightmost < phys_b.leftmost)
                    {
                        break;
                    }
                    if (phys_a.hitbox.WithinRadius(phys_b.hitbox)) // a prelim check before we open up the abstraction
                    {
                        phys_a.HitCheck(phys_b);
                    }
                }
            }
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
            remaining_projectiles.Capacity = projectiles.Capacity;
            foreach (Projectile proj in projectiles)
            {
                if (!proj.ReadyForRemoval())
                {
                    remaining_projectiles.Add(proj);
                }
            }
            projectiles = remaining_projectiles;

            List<ArtTemporary> remaining_art_temp = new List<ArtTemporary>();
            remaining_art_temp.Capacity = art_temp.Capacity;
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
                    phys.DrawHitbox(camera);
                }
            }
        }
    }
}


