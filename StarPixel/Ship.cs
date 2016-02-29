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
        public string hull_art_resource;
        public string paint_art_resource;
        public string heat_art_resource;

        public float base_mass;
        public float base_intertia;

        public string description;

        public float component_thruster_size = 1.0f;
        public List<ThrusterPort> thruster_ports = new List<ThrusterPort>();
        public List<WeaponPort> weapon_ports = new List<WeaponPort>();

        public Hitbox hitbox;

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
        
        public Thruster thrusters;

        
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
            
            // I do not like this sam i am. I do not like this one bit.
            // Thrusters should follow the model set out by weapons, and should be null
            // then we set them separeately
            thrusters = new Thruster(this, template.component_thruster_size);
            thrusters.ApplyTemplate("default");

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

                thrusters.control_thrust_vector = orders.control_thrust;
                thrusters.control_torque_scalar = orders.control_torque;

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

            
            thrusters.Update();


            foreach (ComponentWeapon weapon in weapons)
            {
                if (weapon != null)
                {
                    weapon.Update();
                }
            }


            base.Update();


            hull_sprite.Update(pos, angle);
            if ( paint_sprite != null )
            {
                paint_sprite.Update(pos, angle);
            }
        }

        public override void Draw(Camera camera)
        {
            thrusters.Draw(camera);
            hull_sprite.Draw(camera);
            if ( paint_sprite != null)
            {
                paint_sprite.Draw(camera);
            }
            
        }
    }
}
