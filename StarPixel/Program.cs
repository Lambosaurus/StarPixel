using System;

namespace StarPixel
{
    static class Program
    {
        // The main entry point for the application.
        static void Main(string[] args)
        {
            using (Game1 game = new Game1())
            {
                game.Run();
            }
        }
    }
}

