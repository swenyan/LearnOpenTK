using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;

namespace LearnOpenGL_TK
{
    class Program
    {
        static void Main(string[] args)
        {
            Game window = new Game(800, 600, "Test TK");
            Console.WriteLine("Let's do this.");
            window.Run(60.0);
            Console.WriteLine("Good Bye");
            Console.ReadKey();
        }
    }
}
