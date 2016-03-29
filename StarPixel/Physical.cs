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
    public class Physical : Entity
    {
        //public static Damage collision_dmg_per_force = new Damage(1.0f);

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

        public void Push(Vector2 force, float torque)
        {
            velocity += force / mass;
            angular_velocity += torque / inertia;
        }

        public void Push(Vector2 force, Vector2 eccentricity)
        {
            float torque = ((force.Y * eccentricity.X) - (force.X * eccentricity.Y)) * GameConst.forcerad_to_torque;
            this.Push(force, torque);
        }

        public virtual ComponentShield GetActiveShield()
        {
            return null;
        }

        public bool HitCheck(Physical phys)
        {
            Intersection sect = hitbox.Intersect(phys.hitbox);
            if (sect == null) { return false; }

            // circular approximation for surface normal. This is much worse, but was sometimes more stable
            //float circular_normal = Utility.Angle(phys.pos - pos);
            //sect.surface_normal = circular_normal;

            // determine the velocities at the points of impact, this includes the velocity due to radial effects
            Vector2 v1 = velocity + Utility.Rotate((sect.position - pos) * angular_velocity, MathHelper.PiOver2);
            Vector2 v2 = phys.velocity + Utility.Rotate((sect.position - phys.pos) * phys.angular_velocity, MathHelper.PiOver2);


            // get the relative velocity at the impact point
            Vector2 relative_impact_velocity = (v1 - v2);

            // modify the imact velocity in the direction of the surface normal.
            // this stops a rare case where the impact velocity points the wrong way, and causes huge problems.
            relative_impact_velocity += (Utility.CosSin(sect.surface_normal) * 0.1f);

            // calculate the 'bounce' velocity.
            // The velocity which must be created between the two physicals, as a result of the collision
            Vector2 surface_aligned = Utility.Rotate(relative_impact_velocity, -sect.surface_normal);
            surface_aligned.X *= -1.0f; // bouncyness
            surface_aligned.Y *= -0.25f; // friction
            Vector2 bounce = Utility.Rotate(surface_aligned, sect.surface_normal);

            // modify the physicals positions, to remove them from the collision.
            // this may cause problems if it pushes a ship into a second collsion.
            this.pos += bounce * 1.5f * (phys.mass / (mass + phys.mass));
            phys.pos -= bounce * 1.5f * (mass / (mass + phys.mass));

            // calculate the generated foce between each ship required to produce the bounce velocity
            Vector2 force = ((bounce) / ((1 / mass) + (1 / phys.mass)));

            // apply the forces.
            this.Push(force, sect.position - pos);
            phys.Push(-force, sect.position - phys.pos);



            // generate an explosion as the result of the collision.
            // the radius is ish 1/4 of the larger hulls's radius. Maybe look into whether that is smart.
            float explosion_radius = 20;
            Explosion exp = new Explosion(new Damage(120), explosion_radius, ArtManager.explosions["phys_collision"]);
            exp *= (force.Length()/100f);

            // apply the damage to the phys.
            this.AdsorbExplosion(exp, sect.position);
            phys.AdsorbExplosion(exp, sect.position);

            // Inform the universe of the explosion.
            // I need to create art for this to work.
            exp.Explode(universe, sect.position, (velocity + phys.velocity) / 2,  Utility.CosSin(sect.surface_normal + MathHelper.PiOver2) );
            exp.Explode(universe, sect.position, (velocity + phys.velocity) / 2, Utility.CosSin(sect.surface_normal - MathHelper.PiOver2));

            return true; // a collision was indeed serviced.
        }
    }
}
