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

        public virtual List<UIMarker> GetUiMarkers()
        {
            return new List<UIMarker>();
        }
    }


    public class IntellegenceHuman : Intellegence
    {
        public bool firing;
        public Vector2 weapon_target;

        public Vector2 thrust;
        public float torque;

        public IntellegenceHuman()
        {

        }
        
        public override void Process(ShipFacade link)
        {

            if (link.thrusters != null)
            {
                link.thrusters.output_thrust = thrust;
                link.thrusters.output_torque = torque;
            }

            
            float desired_angle = Utility.Angle(weapon_target - link.pos);
            

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
        Vector2 pos;

        float target_range = 1500;
        Vector2 target;
        float target_ok_distance;
        
        List<Vector2> targets;

        PID angle_tracker;
        PID x_tracker;
        PID y_tracker;

        float desired_angle;

        int k = 0;

        public IntellegenceRoamer(float gain = 1.0f)
        {
            targets = new List<Vector2>();

            for (int i = 0; i < 4; i++)
            {
                targets.Add(Utility.RandVec(target_range));
            }
            target = targets[0];
            targets.RemoveAt(0);
            target_ok_distance = 20f;


            angle_tracker = new PID(10f * gain, 0.5f * gain, 10f * gain);
            x_tracker = new PID(0.6f * gain, 0.3f * gain, 1f * gain);
            y_tracker = new PID(0.6f * gain, 0.3f * gain, 1f * gain);

            /*
            angle_tracker = new PID(10f * Utility.random.Next(5, 15) / 10f, 0.5f * Utility.random.Next(5, 15) / 10f, 10f * Utility.random.Next(5, 15) / 10f);
            x_tracker = new PID(0.1f * Utility.random.Next(5, 15) / 10f, 0.2f * Utility.random.Next(5, 15) / 10f, 0.2f * Utility.random.Next(5, 15) / 10f);
            y_tracker = new PID(0.1f * Utility.random.Next(5, 15) / 10f, 0.2f * Utility.random.Next(5, 15) / 10f, 0.2f * Utility.random.Next(5, 15) / 10f);
            */
        }

        public override void Process(ShipFacade link)
        {
            pos = link.pos;

            if ((target - link.pos).Length() < target_ok_distance)
            {
                targets.Add(Utility.RandVec(target_range));
                target = targets[0];
                targets.RemoveAt(0);
                k = (k + 1) % 24;
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

        public override List<UIMarker> GetUiMarkers()
        {
            MarkerCircle t1 = new MarkerCircle(target, target_ok_distance, Color.Red);
            t1.dashing = ArtPrimitive.CircleDashing.Moderate;
            t1.icons_top.Add(new MarkerIcon((Symbols.GreekL)k, Color.Red));
            

            MarkerCircle t2 = new MarkerCircle(targets[0], target_ok_distance, Color.Red*0.6f);
            t2.dashing = ArtPrimitive.CircleDashing.Moderate;
            t2.icons_top.Add(new MarkerIcon((Symbols.GreekL)((k+1)%24), Color.Red * 0.6f));


            MarkerCircle t3 = new MarkerCircle(targets[1], target_ok_distance, Color.Red*0.4f);
            t3.dashing = ArtPrimitive.CircleDashing.Moderate;
            t3.icons_top.Add(new MarkerIcon((Symbols.GreekL)((k + 2) % 24), Color.Red * 0.4f));

            MarkerCircle t4 = new MarkerCircle(targets[2], target_ok_distance, Color.Red * 0.2f);
            t4.dashing = ArtPrimitive.CircleDashing.Moderate;
            t4.icons_top.Add(new MarkerIcon((Symbols.GreekL)((k + 3) % 24), Color.Red * 0.2f));

            MarkerCircle zone = new MarkerCircle(new Vector2(0, 0), target_range, Color.Yellow);
            zone.fill_color = Color.Transparent;
            zone.line_thickness = 1;

            zone.icons_bot.Add(new MarkerIcon(Symbols.GreekU.Beta, Color.Orange, 0.5f));
            zone.icons_bot.Add(new MarkerIcon(Symbols.GreekU.Alpha, Color.Orange, 0.5f));
            zone.icons_bot.Add(new MarkerIcon(Symbols.GreekU.Tau, Color.Orange, 0.5f));
            zone.icons_bot.Add(new MarkerIcon(Symbols.GreekU.Mu, Color.Orange, 0.5f));
            zone.icons_bot.Add(new MarkerIcon(Symbols.GreekU.Alpha, Color.Orange, 0.5f));
            zone.icons_bot.Add(new MarkerIcon(Symbols.GreekU.Nu, Color.Orange, 0.5f));

            Color lblue = new Color(0.15f, 0.3f, 1.0f);

           
            MarkerQuad t0 = new MarkerQuad(pos, 30, lblue, MarkerQuad.QuadType.Diamond);
            t0.dashing = ArtPrimitive.ShapeDashing.One;
            t0.fill_color = Color.Transparent;

            MarkerLine heading = new MarkerLine(t0, new MarkerPoint( pos + new Vector2(x_tracker.Value(), y_tracker.Value()), 4.0f, lblue));
            heading.draw_startpoint = false;

            MarkerLine target1 = new MarkerLine(t0, t1);
            target1.line_color *= 0.7f;

            MarkerLine target2 = new MarkerLine(t1, t2);
            target2.draw_startpoint = false;
            target2.line_color *= 0.7f;

            MarkerLine target3 = new MarkerLine(t2,t3);
            target3.draw_startpoint = false;
            target3.line_color *= 0.7f;

            MarkerLine target4 = new MarkerLine(t3, t4);
            target4.draw_startpoint = false;
            target4.line_color *= 0.7f;

            List<UIMarker> markers = new List<UIMarker>();
            markers.Add(target1);
            markers.Add(target2);
            markers.Add(target3);
            markers.Add(target4);
            markers.Add(zone);
            markers.Add(heading);
                   
            return markers;
        }
    }
}





