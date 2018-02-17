using System;

namespace Mercury3dTest
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (TestApp game = new TestApp())
            {
                game.Run();
            }
        }
    }
#endif
}

