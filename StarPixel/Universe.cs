﻿using System;
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

        public List<Projectile> projectiles = new List<Projectile>();

        public List<ArtTemporary> art_temp = new List<ArtTemporary>();

        public Universe()
        {

        }

        // creates a new ship from given template name, and adds it into the universe.
        public Ship CreateNewShip( string template_name )
        {
            Ship ship = AssetShipTemplates.ship_templates[template_name].New(this);
            physicals.Add(ship);
            return ship;
        }
        

        public void Start()
        {
            Ship playership = CreateNewShip("F2");
            playership.ai = new IntellegenceHuman();
            playership.MountThruster("better");
            playership.Paint(Color.Red);
            playership.MountShield("default");
            playership.MountWeapon("shooter", 0);
            playership.MountWeapon("shooter", 1);
            playership.MountArmor("default");


            Ship ship0 = CreateNewShip("F2");
            ship0.ai = new IntellegenceRoamer();
            ship0.MountThruster("default");
            ship0.MountShield("default");
            ship0.MountArmor("default");
            ship0.Paint(Color.Blue);
            ship0.pos = Utility.RandVec(400);


            Ship ship1 = CreateNewShip("F2");
            ship1.ai = new IntellegenceRoamer();
            ship1.MountThruster("worse");
            ship1.MountArmor("default");
            ship1.pos = Utility.RandVec(400);
        }

        public void End()
        {
        }

        public void Update()
        {

            foreach (Physical phys in physicals)
            {
                phys.Update();

            }

            foreach (Projectile proj in projectiles)
            {
                proj.Update();

                foreach (Physical phys in physicals)
                {
                    if ( proj.HitCheck(this, phys) )
                    {
                        break;
                    }
                    
                }
            }

            foreach ( ArtTemporary temp in art_temp )
            {
                temp.Update();
            }


            // remove physicals that are good to be removed.
            // this is done separately for a good reason. probably.
            for (int i = physicals.Count - 1; i >= 0; i--)
            {
                if (physicals[i].ReadyForRemoval())
                {
                    physicals.RemoveAt(i);
                }
            }

            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                if (projectiles[i].ReadyForRemoval())
                {
                    projectiles.RemoveAt(i);
                }
            }

            for (int i = art_temp.Count - 1; i >= 0; i--)
            {
                if (art_temp[i].ReadyForRemoval())
                {
                    art_temp.RemoveAt(i);
                }
            }
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


