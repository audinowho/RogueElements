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
                    Ex3_Grid.Example3.Run();
                else if (key.Key == ConsoleKey.D4)
                    Ex4_Stairs.Example4.Run();
                else if (key.Key == ConsoleKey.D5)
                    Ex5_Terrain.Example5.Run();
                else if (key.Key == ConsoleKey.D6)
                    Ex6_Items.Example6.Run();
                else if (key.Key == ConsoleKey.D7)
                    Ex7_Integration.Example7.Run();
                else if (key.Key == ConsoleKey.D8)
                    Ex8_CustomSteps.Example8.Run();
                else if (key.Key == ConsoleKey.D9)
                    Ex9_CustomRooms.Example9.Run();
                else
                    break;

            }
            Console.WriteLine("Bye.");
            Console.ReadKey();
        }

    }
}
