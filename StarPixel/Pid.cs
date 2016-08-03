using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarPixel
{
    public class PID
    {

        float p;
        float i;
        float d;

        float integral = 0;
        float maximum_integral;

        float last_value = 0;
        float delta = 0;

        public PID( float arg_p, float arg_i, float arg_d, float update_rate = GameConst.framerate, float maximum_i = 10f )
        {
            p = arg_p;
            i = arg_i / update_rate;
            d = arg_d * update_rate;
            maximum_integral = maximum_i / i;
        }

        public float Update( float value )
        {
            delta = value - last_value;
            last_value = value;

            integral = Utility.Clamp(integral + value, -maximum_integral, maximum_integral);

            return Value();
        }

        public float Value()
        {
            return (p * last_value) + (d * delta) + (i * integral);
        }

        

    }
}
