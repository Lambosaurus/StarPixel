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

    public class ShipTemplate
    {
        // lore data
        public string description; // FILL IT OUT, THE LORE MUST BE DEEP

        // hull data
        public string hull_art_resource;
        public string paint_art_resource;
        public string heat_art_resource;

        public Hitbox hitbox;

        public float base_mass;
        public float base_intertia;


        // thruster data
        public float component_thruster_size = 1.0f;
        public List<ThrusterPort> thruster_ports = new List<ThrusterPort>();

        // weapon data
        public List<WeaponPort> weapon_ports = new List<WeaponPort>();

        // shield data
        public float component_shield_size = 1.0f;
        public float shield_radius;

        // armor data
        public float component_armor_size = 1.0f;
        public int armor_segment_count = 4;
        public bool armor_seam_on_rear = false;


        public Ship New(Universe universe)
        {
            Ship ship = new Ship(this, universe);
            return ship;
        }

        public void AddThrusterPort(Vector2 arg_pos, float arg_angle, float arg_size, float x_response, float y_response, float t_response)
        {
            thruster_ports.Add(new ThrusterPort(arg_pos, arg_angle, arg_size, x_response, y_response, t_response));
        }

        public void AddWeaponPort( Vector2 arg_pos, float arg_size, float arc_center, float arc_width)
        {
            weapon_ports.Add(new WeaponPort(arg_pos, arg_size, arc_center, arc_width));
        }
    }





    public class Ship : Physical
    {
        public ShipTemplate template;

        public ArtSprite hull_sprite;
        public ArtSprite paint_sprite = null;
        
        public Thruster thrusters = null;

        public ComponentShield shield = null;
        public ComponentArmor armor = null;
        
        public Intellegence ai;
        public IntInputs ai_inputs = new IntInputs();


        public ComponentWeapon[] weapons;

        public Ship(ShipTemplate arg_template, Universe arg_universe ) : base(arg_universe)
        {
            template = arg_template;

            hitbox = template.hitbox.Copy();

            mass = template.base_mass;
            inertia = template.base_intertia;

            hull_sprite = ArtManager.GetSpriteResource( template.hull_art_resource ).New();
           
            weapons = new ComponentWeapon[template.weapon_ports.Count];
        }

        public void MountWeapon( string template_name, int port_number )
        {
            if (port_number >= 0 && port_number <= template.weapon_ports.Count )
            {

                if (template_name == null)
                {
                    weapons[port_number] = null;
                }
                else
                {
                    weapons[port_number] = AssetWeaponTemplates.weapon_templates[template_name].New(this, template.weapon_ports[port_number]);
                }

            }
        }

        public void MountShield(string template_name)
        {
            shield = AssetShieldTemplates.shield_templates[template_name].New(this);
        }

        public void MountArmor(string template_name)
        {
            armor = AssetArmorTemplates.armor_templates[template_name].New(this);
        }


        public override ComponentShield GetActiveShield()
        {
            if (shield != null)
            {
                if (shield.active)
                {
                    return shield;
                }
            }
            return null;
        }

        public void MountThruster(string template_name)
        {
            thrusters = new Thruster(template_name, this, template.component_thruster_size);
        }

        public void Paint( Color color )
        {
            if (color != null)
            {
                paint_sprite = ArtManager.GetSpriteResource(template.paint_art_resource).New();
                paint_sprite.color = color;
            }
            else
            {
                paint_sprite = null;
            }
        }

        public override void Update()
        {
            
            if (ai != null)
            {
                ai_inputs.pos = pos;
                ai_inputs.angle = angle;
                IntOutputs orders = ai.Process(ai_inputs);

                if (thrusters != null)
                {
                    thrusters.control_thrust_vector = orders.control_thrust;
                    thrusters.control_torque_scalar = orders.control_torque;
                }

                if( orders.firing )
                {
                    foreach (ComponentWeapon weapon in weapons)
                    {
                        if (weapon != null)
                        {
                            if (weapon.ReadyToFire())
                            {
                                weapon.Fire(0.0f);
                            }
                        }
                    }
                }
                
            }


            if (thrusters != null) { thrusters.Update(); }

            if (shield != null) { shield.Update(); }

            if (armor != null) { armor.Update(); }

            foreach (ComponentWeapon weapon in weapons)
            {
                if (weapon != null) { weapon.Update(); }
            }


            base.Update();

            hull_sprite.Update(pos, angle);
            if ( paint_sprite != null ) { paint_sprite.Update(pos, angle); }
        }



        public override void Draw(Camera camera)
        {
            if (thrusters != null) { thrusters.Draw(camera); }
            
            hull_sprite.Draw(camera);
            if ( paint_sprite != null) { paint_sprite.Draw(camera); }

            if (shield != null) { shield.Draw(camera); }
        }
    }
}







