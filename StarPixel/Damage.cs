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
    public class Explosion
    {
        public Damage dmg { get; private set; }
        public float radius { get; private set; }

        public ArtExplosionResource art_cloud_resource { get; private set; }
        
        public float art_scale { get; private set; }

        public Explosion(Damage arg_dmg, float arg_radius, ArtExplosionResource arg_cloud_art, float arg_art_scale = 1.0f)
        {
            dmg = arg_dmg;
            radius = arg_radius;
            art_cloud_resource = arg_cloud_art;
            art_scale = arg_art_scale;
        }

        public static Explosion operator *(Explosion exp, float scale)
        {
            return new Explosion(exp.dmg * scale, exp.radius * Utility.Sqrt(scale), exp.art_cloud_resource, scale * exp.art_scale );
        }

        public void Explode(Universe universe, Vector2 pos, Vector2 velocity, Vector2 skew)
        {
            universe.AddArtTemp(art_cloud_resource.New(art_scale, pos, velocity, skew));
        }
    }

    public class Damage
    {
        public float kinetic { get; private set; }
        public float thermal { get; private set; }
        public float electro { get; private set; }
        public float mining { get; private set; }

        public Damage(float k, float t = 0.0f, float e = 0.0f, float m = 0.0f)
        {
            kinetic = k;
            thermal = t;
            electro = e;
            mining = m;
        }


        public static Damage operator * ( Damage dmg, float scalar )
        {
            return new Damage(dmg.kinetic * scalar, dmg.thermal * scalar, dmg.electro * scalar, dmg.mining * scalar);
        }

        public static Damage operator / (Damage dmg, float scalar)
        {
            float inverse = 1.0f / scalar;
            return new Damage(dmg.kinetic * inverse, dmg.thermal * inverse, dmg.electro * inverse, dmg.mining * inverse);
        }
    }

    public class Resistance
    {
        public static Resistance Zero { get; private set; } = new Resistance(0,0,0,0);


        // these are not resistances, these are vulnerabilities
        public float thermal { get; private set; }
        public float kinetic { get; private set; }
        public float electro { get; private set; }
        public float mining { get; private set; }

        public Resistance(float k, float t, float e, float m)
        {
            thermal = 1.0f - t;
            kinetic = 1.0f - k;
            electro = 1.0f - e;
            mining = 1.0f - m;
        }

        public static Resistance operator * ( Resistance one, Resistance two )
        {
            return new Resistance(
                1.0f - (one.kinetic * two.kinetic),
                1.0f - (one.thermal * two.thermal),
                1.0f - (one.electro * two.electro),
                1.0f - (one.mining * two.mining)
                );
        }

        public float EvaluateDamage(Damage dmg)
        {
            return (dmg.kinetic * kinetic) + (dmg.thermal * thermal) + (dmg.electro * electro) + (dmg.mining * mining);
        }

        public Damage RemainingDamage(float hp, Damage dmg)
        {
            float max_damage = this.EvaluateDamage(dmg);
            return dmg * (max_damage / hp);
        }
    }
}
