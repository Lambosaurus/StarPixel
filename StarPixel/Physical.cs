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
    public class Physical : Entity
    {
        //public static Damage collision_dmg_per_force = new Damage(1.0f);

        public float mass;
        public float inertia;
        public float angular_velocity;

        public float radius;

        public Universe universe;

        public Hitbox hitbox;

        public ComponentShield shield = null;
        public ComponentArmor armor = null;

        public Physical(Universe arg_universe) : base()
        {
            universe = arg_universe;
            mass = 10;
            inertia = 50; // things feel right with the current thruster torque model when inertia is about 5x mass.
            radius = 10;

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
            float torque = Utility.Cross(eccentricity, force); // GameConst.forcerad_to_torque;
            this.Push(force, torque);
        }
        

        public bool HitCheck(Physical phys)
        {
            Intersection sect = hitbox.Intersect(phys.hitbox);
            if (sect == null) { return false; }

            Vector2 this_ecc = sect.position - pos;
            Vector2 phys_ecc = sect.position - phys.pos;

            // determine the velocities at the points of impact, this includes the velocity due to radial effects
            Vector2 v1 = velocity + Utility.RotatePos(this_ecc * angular_velocity);
            Vector2 v2 = phys.velocity + Utility.RotatePos(phys_ecc * phys.angular_velocity);

            // get the relative velocity at the impact point
            Vector2 relative_impact_velocity = (v1 - v2);

            // modify the imact velocity in the direction of the surface normal.
            // This is based on the overlap, and functions as a spring.
            relative_impact_velocity += (Utility.CosSin(sect.surface_normal) * (0.2f + (sect.overlap * 0.05f)));

            // calculate the 'bounce' velocity.
            // The velocity which must be created between the two physicals, as a result of the collision
            Vector2 surface_aligned = Utility.Rotate(relative_impact_velocity, -sect.surface_normal);
            surface_aligned.X *= -GameConst.collision_elasticity; // bouncyness
            surface_aligned.Y *= -GameConst.collision_friction; // friction
            Vector2 bounce = Utility.Rotate(surface_aligned, sect.surface_normal);

            float this_mass_ratio = mass / (mass + phys.mass);
            float phys_mass_ratio = phys.mass / (mass + phys.mass);

            // modify the physicals positions, to remove them from the collision.
            // this may cause problems if it pushes a ship into a second collsion. Whatevs.
            this.pos += bounce * 1.5f * phys_mass_ratio;
            phys.pos -= bounce * 1.5f * this_mass_ratio;
            
        
            // calculate the generated foce between each ship required to produce the bounce velocity
            //Vector2 force = ((bounce) / ((1 / mass) + (1 / phys.mass)));


            // Vector2 velocity_this = (force / mass) + Utility.RotatePos(eccentricity_this * Utility.Cross(eccentricity_this, force) / intertia);
            // Vector2 velocity_phys = (force / phys.mass) + Utility.RotatePos(eccentricity_phys * Utility.Cross(eccentricity_phys, force) / phys.intertia)

            float A1 = (1.0f / this.mass) + (1.0f / phys.mass) + (this_ecc.Y * this_ecc.Y / this.inertia) + (phys_ecc.Y * phys_ecc.Y / phys.inertia);
            float B1 = (this_ecc.Y * this_ecc.X / this.inertia) + (phys_ecc.Y * phys_ecc.X / phys.inertia);
            float A2 = (1.0f / this.mass) + (1.0f / phys.mass) + (this_ecc.X * this_ecc.X / this.inertia) + (phys_ecc.X * phys_ecc.X / phys.inertia);
            float B2 = (this_ecc.X * this_ecc.Y / this.inertia) + (phys_ecc.X * phys_ecc.Y / phys.inertia);

            float Fx = ((B1 * bounce.Y / (A1 * A2)) + (bounce.X / A1)) / (1.0f - (B1 * B2 / (A1 * A2)));
            float Fy = ((B2 * Fx) + bounce.Y) / A2;
            
            Vector2 force = new Vector2(Fx, Fy);

            // apply the forces.
            this.Push(force, this_ecc);
            phys.Push(-force, phys_ecc);



            // generate an explosion as the result of the collision.
            // the radius is ish 1/4 of the larger hulls's radius. Maybe look into whether that is smart.
            float explosion_radius = 20;
            Explosion exp = new Explosion(new Damage(120), explosion_radius, ArtManager.explosions["phys_collision"]);
            exp *= (force.Length() / 100f);

            // apply the damage to the phys.
            this.AdsorbExplosion(exp, sect.position);
            phys.AdsorbExplosion(exp, sect.position);

            Vector2 explosion_velocity = phys_mass_ratio * phys.velocity + this_mass_ratio * this.velocity;

            // Inform the universe of the explosion.
            exp.Explode(universe, sect.position, explosion_velocity, Utility.CosSin(sect.surface_normal + MathHelper.PiOver2));
            
            return true; // a collision was indeed serviced.
        }
    }
}
