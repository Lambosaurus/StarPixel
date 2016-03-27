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



    public class Entity
    {
        public Vector2 pos;
        public Vector2 velocity;
        public float angle;
        private bool selected = false;

        private void SelectMe(object sender, EventArgs eventArgs)
        {
            selected = !selected;
        }

        public bool destroyed;

        public bool Selected
        {
            set
            {
                selected = false;
            }
        }

        public event EventHandler Clicked;

        public Entity()
        {
            pos = new Vector2(0, 0);
            velocity = new Vector2(0, 0);
            angle = 0.0f;

            Clicked += SelectMe;
            destroyed = false;
        }

        public void OnClick()
        {
            if (Clicked != null)
            {
                Clicked(this, EventArgs.Empty);
            }
            
        }

        public virtual void Update()
        {
            pos += velocity;
        }

        public virtual void Draw(Camera camera)
        {
        }

        public void Destory( )
        {
            if ( ! destroyed )
            {

            }

            destroyed = true;
        }

        public bool ReadyForRemoval()
        {
            return destroyed;
        }
    }

    public class Physical : Entity
    {
        public float mass;
        public float inertia;
        public float angular_velocity;

        public Universe universe;

        public Hitbox hitbox;


        public Physical(Universe arg_universe) : base()
        {
            universe = arg_universe;
            mass = 10;
            inertia = 50; // things feel right with the current thruster torque model when inertia is about 5x mass.

            angular_velocity = 0.0f;
        }

        public override void Update()
        {
            angular_velocity = Utility.Clamp(angular_velocity, -MathHelper.PiOver4, MathHelper.PiOver4);

            angle += angular_velocity;
            angle = Utility.WrapAngle(angle);

            base.Update();

            hitbox.Update(pos, angle);
        }

        public virtual void AdsorbExplosion(Explosion exp, Vector2 position)
        {

        }

        public void Push( Vector2 force, float torque )
        {
            velocity += force / mass;
            angular_velocity += torque / inertia;
        }
        
        public void Push(Vector2 force, Vector2 eccentricity)
        {
            float torque = ((force.Y * eccentricity.X) - (force.X * eccentricity.Y))/50;
            this.Push(force, torque);
        }

        public virtual ComponentShield GetActiveShield(  )
        {
            return null;
        }

        public bool HitCheck( Physical phys )
        {
            // GET BOUNCE AXIS OF COLLISION, AND ONLY TRANSFER THAT.


            Intersection sect = hitbox.Intersect(phys.hitbox);
            if (sect == null) { return false; }

            float circular_normal = Utility.Angle(phys.pos - pos);
            sect.surface_normal = circular_normal;

            
            Vector2 v1 = velocity + Utility.Rotate((sect.position - pos) * angular_velocity, MathHelper.PiOver2);
            Vector2 v2 = phys.velocity + Utility.Rotate((sect.position - phys.pos) * phys.angular_velocity, MathHelper.PiOver2);


            Vector2 relative_impact_velocity = v1 - v2;
            Vector2 surface_aligned = Utility.Rotate(relative_impact_velocity, -sect.surface_normal);
            surface_aligned.X *= -2.0f; // bouncyness
            surface_aligned.Y *= -0.25f; // friction
            Vector2 bounce = Utility.Rotate(surface_aligned, sect.surface_normal);



            /*
            if ( Utility.AngleDelta(sect.surface_normal, circular_normal) > MathHelper.PiOver2 )
            {
                sect.surface_normal = circular_normal;
                //sect.surface_normal -= MathHelper.Pi;
            }
            */

            /*
            Vector2 v1 = velocity + Utility.Rotate((sect.position - pos)*angular_velocity, MathHelper.PiOver2);
            Vector2 v2 = phys.velocity + Utility.Rotate((sect.position - phys.pos) * phys.angular_velocity, MathHelper.PiOver2);

            
            Vector2 bounce = Utility.Bounce(v1 - v2, sect.surface_normal);


            */


            
            this.pos += bounce * 2f * (phys.mass / (mass + phys.mass));
            phys.pos -= bounce * 2f * (mass / (mass + phys.mass));

            Vector2 b2 = ((bounce) / ((phys.mass / mass) + 1));
            Vector2 b1 =  b2 / (phys.mass / mass);

            this.Push(b2*mass, sect.position - pos);
            phys.Push(-b1*phys.mass, sect.position - phys.pos);
            

            return true;
        }

    }



}



