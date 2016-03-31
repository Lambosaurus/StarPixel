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

    public class Intellegence
    {
        public Intellegence()
        {

        }

        public virtual void Process( ShipFacade link )
        {
        }
    }


    public class IntellegenceHuman : Intellegence
    {
        Vector2 target;

        public IntellegenceHuman()
        {

        }

        public void GiveTarget(Vector2 arg_target)
        {
            target = arg_target;
        }

        public override void Process(ShipFacade link)
        {

            KeyboardState key_state = Keyboard.GetState();
            MouseState mouse_state = Mouse.GetState();

            if (link.thrusters != null)
            {
                link.thrusters.output_thrust.X = key_state.IsKeyDown(Keys.W) ? 1.0f : ((key_state.IsKeyDown(Keys.S)) ? -1.0f : 0.0f);
                link.thrusters.output_thrust.Y = key_state.IsKeyDown(Keys.D) ? 1.0f : ((key_state.IsKeyDown(Keys.A)) ? -1.0f : 0.0f);
                link.thrusters.output_torque = key_state.IsKeyDown(Keys.E) ? 1.0f : ((key_state.IsKeyDown(Keys.Q)) ? -1.0f : 0.0f);
            }

            
            bool firing = mouse_state.LeftButton == ButtonState.Pressed;
            float desired_angle = Utility.Angle(target - link.pos);
            

            for (int i = 0; i < link.weapon_count; i++)
            {
                WeaponFacade weapon = link.Weapon(i);
                if (weapon.weapon_present)
                {
                    weapon.fire = firing;
                    weapon.target_angle = desired_angle - link.angle;
                }
            }
        }
    }


    public class IntellegenceHunter : Intellegence
    {
        Ship target;


        PID angle_tracker;
        PID x_tracker;
        PID y_tracker;

        float desired_angle;

        public IntellegenceHunter( Ship arg_target )
        {
            target = arg_target;

            
            angle_tracker = new PID(10f, 0.5f, 10f);
            x_tracker = new PID(0.3f, 0.3f, 1f);
            y_tracker = new PID(0.3f, 0.3f, 1f);
            
        }

        public override void Process(ShipFacade link)
        {

            if (!target.destroyed)
            {

                Vector2 target_pos = target.pos + (Utility.CosSin(target.angle + MathHelper.Pi)*target.hitbox.radius*3);


                Vector2 mov;
                mov.X = x_tracker.Update(target_pos.X - link.pos.X);
                mov.Y = y_tracker.Update(target_pos.Y - link.pos.Y);


                /*
                if (mov.LengthSquared() > (0.5))
                {
                    // Ya, this version is way cooler.

                    desired_angle = Utility.Angle(mov);
                }
                */
                
                
                for (int i = 0; i < link.weapon_count; i++)
                {
                    WeaponFacade weapon = link.Weapon(i);
                    if (weapon.weapon_present)
                    {

                        Vector2 intercept = target.pos + ((target.velocity - link.velocity) * (target.pos - link.pos).Length() / weapon.projectile_velocity);
                        desired_angle = Utility.Angle(intercept - link.pos);
                        float range = (intercept - link.pos).Length();


                        if ( range < (weapon.projectile_life*weapon.projectile_velocity)/2f &&  Utility.AngleDelta(desired_angle, link.angle) < MathHelper.PiOver2)
                        {
                            weapon.fire = true;
                            weapon.target_angle = desired_angle - link.angle;
                        }
                    }
                }
                


                float a_mov = angle_tracker.Update(Utility.AngleDelta(desired_angle, link.angle));


                mov = Utility.Rotate(mov, -link.angle);

                if (link.thrusters != null)
                {
                    link.thrusters.output_thrust.X = Utility.Clamp(mov.X);
                    link.thrusters.output_thrust.Y = Utility.Clamp(mov.Y);
                    link.thrusters.output_torque = Utility.Clamp(a_mov);
                }

            }
            else
            {
                link.thrusters.output_thrust.X = 0.0f;
                link.thrusters.output_thrust.Y = 0.0f;
                link.thrusters.output_torque = 0.0f;
            }
            
        }
    }



    public class IntellegenceRoamer : Intellegence
    {

        float target_range = 1200;
        Vector2 target;
        float target_ok_distance;

        PID angle_tracker;
        PID x_tracker;
        PID y_tracker;

        float desired_angle;

        public IntellegenceRoamer(float gain = 1.0f)
        {
            target = Utility.RandVec(target_range);
            target_ok_distance = 20f;


            angle_tracker = new PID(10f * gain, 0.5f * gain, 10f * gain);
            x_tracker = new PID(0.3f * gain, 0.3f * gain, 1f * gain);
            y_tracker = new PID(0.3f * gain, 0.3f * gain, 1f * gain);

            /*
            angle_tracker = new PID(10f * Utility.random.Next(5, 15) / 10f, 0.5f * Utility.random.Next(5, 15) / 10f, 10f * Utility.random.Next(5, 15) / 10f);
            x_tracker = new PID(0.1f * Utility.random.Next(5, 15) / 10f, 0.2f * Utility.random.Next(5, 15) / 10f, 0.2f * Utility.random.Next(5, 15) / 10f);
            y_tracker = new PID(0.1f * Utility.random.Next(5, 15) / 10f, 0.2f * Utility.random.Next(5, 15) / 10f, 0.2f * Utility.random.Next(5, 15) / 10f);
            */
        }

        public override void Process(ShipFacade link)
        {

            if ((target - link.pos).Length() < target_ok_distance)
            {
                target = Utility.RandVec(target_range);
            }
            

            Vector2 mov;
            mov.X = x_tracker.Update(target.X - link.pos.X);
            mov.Y = y_tracker.Update(target.Y - link.pos.Y);
            

            if (mov.LengthSquared() > (0.5))
            {
                // Ya, this version is way cooler.

                desired_angle = Utility.Angle(mov);
            }

            float a_mov = angle_tracker.Update(Utility.AngleDelta(desired_angle, link.angle));


            mov = Utility.Rotate(mov, -link.angle);

            if (link.thrusters != null)
            {
                link.thrusters.output_thrust.X = Utility.Clamp(mov.X);
                link.thrusters.output_thrust.Y = Utility.Clamp(mov.Y);
                link.thrusters.output_torque = Utility.Clamp(a_mov);
            }
        }
    }
}





