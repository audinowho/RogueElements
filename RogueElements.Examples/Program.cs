// <copyright file="Program.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using RogueElements;

namespace RogueElements.Examples
{
    public class Program
    {
        public static void Main()
        {
#if DEBUG
            GenContextDebug.OnInit += ExampleDebug.Init;
            GenContextDebug.OnStep += ExampleDebug.OnStep;
            GenContextDebug.OnStepIn += ExampleDebug.StepIn;
            GenContextDebug.OnStepOut += ExampleDebug.StepOut;
#endif
            ConsoleKey lastKey = ConsoleKey.Enter;
            bool wasNonAction = false;
            bool done = false;
            while (!done)
            {
                ConsoleKey key = ConsoleKey.Enter;
                if (!wasNonAction)
                {
                    if (lastKey != ConsoleKey.Enter)
                    {
                        Console.WriteLine("Press a key 1-8 | F4=Debug");
                        Console.WriteLine("While debugging: F5=Step-In | F6=Step-Out | ESC=Exit Debug");
                    }
                    else
                    {
                        Console.WriteLine("Press a key 1-8:");
                    }
                }

                key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.F2)
                {
                    while (true)
                    {
                        Console.Clear();
                        Console.WriteLine(">Bulk Gen");
                        Console.WriteLine("Specify amount to generate");
                        int amt = GetInt(false);
                        if (amt > -1)
                        {
                            Console.WriteLine("Stress Test WIP.");
                            ConsoleKeyInfo afterKey = Console.ReadKey();
                            if (afterKey.Key == ConsoleKey.Escape)
                                break;
                        }
                        else if (amt == -1)
                        {
                            break;
                        }
                    }
                }
                else if (key == ConsoleKey.F4)
                {
                    ExampleDebug.SteppingIn = true;
                    key = lastKey;
                }

                bool keepKey = true;
                wasNonAction = false;
                switch (key)
                {
                    case ConsoleKey.D1:
                        Ex1_Tiles.Example1.Run();
                        break;
                    case ConsoleKey.D2:
                        Ex2_Rooms.Example2.Run();
                        break;
                    case ConsoleKey.D3:
                        Ex3_Grid.Example3.Run();
                        break;
                    case ConsoleKey.D4:
                        Ex4_Stairs.Example4.Run();
                        break;
                    case ConsoleKey.D5:
                        Ex5_Terrain.Example5.Run();
                        break;
                    case ConsoleKey.D6:
                        Ex6_Items.Example6.Run();
                        break;
                    case ConsoleKey.D7:
                        Ex7_Special.Example7.Run();
                        break;
                    case ConsoleKey.D8:
                        Ex8_Integration.Example8.Run();
                        break;
                    case ConsoleKey.Escape:
                        done = true;
                        break;
                    default:
                        keepKey = false;
                        wasNonAction = true;
                        break;
                }

                if (keepKey)
                    lastKey = key;

                ExampleDebug.SteppingIn = false;
            }

            Console.WriteLine("Bye.");
            Console.ReadKey();
        }

        public static int GetInt(bool includeAmt)
        {
            int result = 0;

            ConsoleKeyInfo key = Console.ReadKey(true);
            while (key.Key != ConsoleKey.Enter)
            {
                if (key.Key == ConsoleKey.Escape)
                    return -1;
                if (includeAmt && key.Key == ConsoleKey.F2)
                    return -2;

                if (key.KeyChar >= '0' && key.KeyChar <= '9')
                {
                    Console.Write(key.KeyChar);
                    result = (result * 10) + key.KeyChar - '0';
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    Console.Write("\b \b");
                    result /= 10;
                }

                key = Console.ReadKey(true);
            }

            Console.WriteLine();
            return result;
        }

        public static void StressTest<T>(MapGen<T> layout, int amount)
            where T : class, IGenContext
        {
            ExampleDebug.Printing = -1;
            ulong structSeed = 0;
            try
            {
                Dictionary<int, int> generatedItems = new Dictionary<int, int>();
                Dictionary<int, int> generatedEnemies = new Dictionary<int, int>();

                var watch = new Stopwatch();
                TimeSpan minTime = TimeSpan.MaxValue;
                TimeSpan maxTime = TimeSpan.MinValue;

                for (int ii = 0; ii < amount; ii++)
                {
                    structSeed = MathUtils.Rand.NextUInt64();

                    TimeSpan before = watch.Elapsed;
                    watch.Start();
                    IGenContext context = layout.GenMap(structSeed);
                    watch.Stop();
                    TimeSpan diff = watch.Elapsed - before;
                    if (diff > maxTime)
                        maxTime = diff;
                    if (diff < minTime)
                        minTime = diff;
                }

                var avgTime = new TimeSpan(watch.Elapsed.Ticks / amount);
                Console.WriteLine($"Completed in {watch.Elapsed.ToString()}.");
                Console.WriteLine($"MIN: {minTime.ToString()}    AVG: {avgTime.ToString()}    MAX: {maxTime.ToString()}");
            }
            catch (Exception ex)
            {
                Debug.Write("ERROR: " + structSeed);
                PrintError(ex);
                throw;
            }
            finally
            {
                ExampleDebug.Printing = 0;
            }
        }

        public static void PrintError(Exception ex)
        {
            Exception innerException = ex;
            int depth = 0;
            while (innerException != null)
            {
                Console.WriteLine("Exception Depth: " + depth);
                Console.WriteLine(innerException.ToString());
                Console.WriteLine();
                innerException = innerException.InnerException;
                depth++;
            }
        }
    }
}
