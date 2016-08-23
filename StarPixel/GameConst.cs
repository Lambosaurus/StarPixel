
namespace StarPixel
{
    public static class GameConst
    {
        public const int framerate = 60;
        
        public const float collision_friction = 0.2f;
        public const float collision_elasticity = 0.9f;

        public const bool screensaver = false;

        public static float C(int c)
        {
            float size = 1.0f;
            while (c-- > 1)
            {
                size *= 1.5f;
            }
            return size;
        }

    }
}