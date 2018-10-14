using System;
using System.Collections.Generic;
using System.Text;
using RogueElements;

namespace RogueElements.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WindowWidth = Console.LargestWindowWidth;
            Console.WindowHeight = Console.LargestWindowHeight;
            while (true)
            {
                Console.WriteLine("Press a key 1-9:");
                ConsoleKeyInfo key = Console.ReadKey();
                Console.Clear();
                if (key.Key == ConsoleKey.D1)
                    Ex1_Tiles.Example1.Run();
                else if (key.Key == ConsoleKey.D2)
                    Ex2_Rooms.Example2.Run();
                else if (key.Key == ConsoleKey.D3)
                {

                }
                else if (key.Key == ConsoleKey.D4)
                {

                }
                else if (key.Key == ConsoleKey.D5)
                {

                }
                else if (key.Key == ConsoleKey.D6)
                {

                }
                else if (key.Key == ConsoleKey.D7)
                {

                }
                else if (key.Key == ConsoleKey.D8)
                {

                }
                else if (key.Key == ConsoleKey.D9)
                {

                }
                else
                    break;

            }
            Console.WriteLine("Bye.");
            Console.ReadKey();
        }

    }
}
