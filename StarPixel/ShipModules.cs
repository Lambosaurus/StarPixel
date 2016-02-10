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
    public class Component
    {
        // do i have component parent classes?
        // I dunno. It gets messy, but i feel like i need them to abstract out component generation
        public float max_hp = 100;
        public float hp;
        public float mass;
        int max_size;
        public float max_usage;
        public float usage;

        public bool destroyed;

        public Ship ship;

        public Component( Ship arg_ship )
        {
            destroyed = false;
            hp = max_hp;

            ship = arg_ship;
        }

        public virtual void Damage(float damage)
        {
            hp -= damage;
            
            if (hp <= 0)
            {
                if (!destroyed)
                {
                    destroyed = true;
                    Destroy();
                }
            }
        }

        public virtual void Update()
        {
        }

        public virtual void Destroy()
        {
        }
        public virtual void CalculateUsage()
        {
    }
    
    }



    public struct ThrusterNozzle
    {
        public Vector2 position;
        public float angle;
        public ArtVent vent;

        public float kx;
        public float ky;
        public float kt;

        public ThrusterNozzle( Vector2 pos, float arg_angle, float a_kx, float a_ky, float a_kt, ArtVent arg_vent )
        {
            position = pos;
            angle = arg_angle;
            vent = arg_vent;

            kx = a_kx;
            ky = a_ky;
            kt = a_kt;
        }
    }


    public class Thrusters : Component
    {
        public float main_thrust = 4;
        public float manouvering_thrust = 2;
        public float torque = 0.5f;

        public Vector2 control_thrust_vector;
        public float control_torque_scalar;
        public float thrust_temperature = 5000;

        public List<ThrusterNozzle> nozzles = new List<ThrusterNozzle>();

        ArtVent sparkles;
        
        public float efficiency;
        
        public Thrusters(Ship ship) : base(ship)
        {
            control_thrust_vector = new Vector2(0, 0);
            control_torque_scalar = 0;

            efficiency = 1.0f;

            
            float scale = 0.75f;

            sparkles = ArtManager.NewArtVent("sparkles", 0.75f);


            nozzles.Add(new ThrusterNozzle(new Vector2(-10, 0), MathHelper.Pi, 1, 0, 0, ArtManager.NewArtVent("", scale*1.2f)));

            nozzles.Add(new ThrusterNozzle(new Vector2(8, 4), MathHelper.PiOver2, 0, -0.25f, -0.25f, ArtManager.NewArtVent("", scale)));
            nozzles.Add(new ThrusterNozzle(new Vector2(-8, 7), MathHelper.PiOver2, 0, -0.25f, 0.25f, ArtManager.NewArtVent("", scale)));
            nozzles.Add(new ThrusterNozzle(new Vector2(8, -4), -MathHelper.PiOver2, 0, 0.25f,  0.25f, ArtManager.NewArtVent("", scale)));
            nozzles.Add(new ThrusterNozzle(new Vector2(-8, -7), -MathHelper.PiOver2, 0, 0.25f, -0.25f, ArtManager.NewArtVent("", scale)));

            nozzles.Add(new ThrusterNozzle(new Vector2(-6, 11), 0.2f, -0.25f, 0, 0, ArtManager.NewArtVent("", scale)));
            nozzles.Add(new ThrusterNozzle(new Vector2(-6, -11), -0.2f, -0.25f, 0, 0, ArtManager.NewArtVent("", scale)));


            /*
            nozzles[(int)PortDirections.Rear] = new ThrusterNozzle(new Vector2(-10, 0), new Vector2(-1, 0));
            nozzles[(int)PortDirections.Front] = new ThrusterNozzle(new Vector2(10, 0), new Vector2(0.75f, 0));

            nozzles[(int)PortDirections.LeftFront] = new ThrusterNozzle(new Vector2(8, -4), new Vector2(0, -0.75f));
            nozzles[(int)PortDirections.LeftRear] = new ThrusterNozzle(new Vector2(-8, -6), new Vector2(0, -0.75f));
            nozzles[(int)PortDirections.RightFront] = new ThrusterNozzle(new Vector2(8, 4), new Vector2(0, 0.75f));
            nozzles[(int)PortDirections.RightRear] = new ThrusterNozzle(new Vector2(-8, -6), new Vector2(0, 0.75f));
            */

            max_usage = 4.0f;
            usage = 0.0f;
        }

        public override void CalculateUsage()
        {
            usage = (control_thrust_vector.X > 0.0f ? (control_thrust_vector.X * main_thrust) : (Utility.Abs(control_thrust_vector.X) * control_torque_scalar)) +
                (control_thrust_vector.Y * manouvering_thrust) +
                (control_torque_scalar * manouvering_thrust);
        }

        public override void Update()
        {
            float control_x = Utility.Clamp(control_thrust_vector.X);
            float control_y = Utility.Clamp(control_thrust_vector.Y);

            float control_t = Utility.Clamp(control_torque_scalar);

            /*
            /+/ Torque and thrust use all use the side thrusters, so they conflict a little.
            float side_thruster_sum = Utility.Abs(control_y) + Utility.Abs(control_t);
            if (side_thruster_sum > 1.0f )
            {
                // we set them in proportion to their ratio
                control_t = control_t / (side_thruster_sum);
                control_y = control_y / (side_thruster_sum);

                // in the simplest case, if control_x is full and torque is full, then they both get reduced to half.
            }
            */

            float output_torque = control_t * torque;
            float output_thrust_y = control_y * manouvering_thrust;
            float output_thrust_x = control_x * ((control_x > 0) ? main_thrust : manouvering_thrust);


            foreach (ThrusterNozzle nozzle in nozzles)
            {
                float strength = (nozzle.kx * control_x) + (nozzle.ky * control_y) + (nozzle.kt * control_t);
                if (strength > 0)
                {
                    nozzle.vent.Generate(ship.pos + Utility.Rotate(nozzle.position, ship.angle), ship.velocity, nozzle.angle + ship.angle, strength);
                }
                nozzle.vent.Update();
            }
            

            //particles.Generate(ship.pos + Utility.Rotate(nozzles[0].position, ship.angle), ship.velocity, ship.angle + MathHelper.Pi, nozzles[(int)PortDirections.Rear].particle_gen);
            
            if( control_x > 0 && Utility.Randf(1.0f) > 0.94f / control_x ) {
                sparkles.Generate(ship.pos + Utility.Rotate(nozzles[0].position, ship.angle), ship.velocity, ship.angle + MathHelper.Pi, 1);
            }
            

            

            /*
            for ( int i = 0; i < 6; i++ )
            {
                while (nozzles[i].particle_gen > 1)
                {
                    nozzles[i].particle_gen--;

                    Vector2 thrust_vector = Utility.Rotate(nozzles[i].velocity + Utility.Rand(0.1f), ship.angle);


                    particles.Add(ship.pos + Utility.Rotate(nozzles[i].position, ship.angle), ship.velocity + thrust_vector, thrust_temperature, Utility.Angle(thrust_vector), thrust_vector.Length()*2 );


                    if ( (i == (int)PortDirections.Rear)  && (Utility.random.Next(0,10) == 0) )
                    {
                        particles2.Add(ship.pos + Utility.Rotate(nozzles[i].position, ship.angle), ship.velocity + thrust_vector, thrust_temperature, Utility.Angle(thrust_vector), thrust_vector.Length() * 2);
                    }
                }
            }
            */

            //particles.Update();
            sparkles.Update();

            // out thrust vector we have calculated needs to be rotated by the ships angle.
            ship.Push( Utility.Rotate( new Vector2(output_thrust_x, output_thrust_y), ship.angle ) , output_torque);
        }

        public void Draw(Camera camera)
        {
            sparkles.Draw(camera);

            foreach ( ThrusterNozzle nozzle in nozzles)
            {
                nozzle.vent.Draw(camera);
            }

        }
    }
    enum ReactorType
    {
        URANIUM, THORIUM, PLUTONIUM
    }


    public class Reactor : Component
    {
        float Max_output;
        float fuel_efficiency;
        ReactorType fuel_type;
        float power_usage;
        List<Component> connected;

        public Reactor(Ship ship) : base(ship)
        {
            Max_output = 3.0f;
            connected.Add(ship.thrusters);
        }

        public override void Update()
        {
            foreach (Component comp in connected)
            {
                
            }
        }
    }

}




