using System;
using System.Collections.Generic;
using System.Text;

namespace RogueElements.Examples.Ex8_Custom
{
    public static class Example8
    {
        public static void Run()
        {
            string title = "8: Custom Blank Map. Ignore.";
            MapGen<MapGenContext> layout = new MapGen<MapGenContext>();

            InitTilesStep<MapGenContext> startStep = new InitTilesStep<MapGenContext>();
            startStep.Width = 30;
            startStep.Height = 25;
            layout.GenSteps.Add(new GenPriority<GenStep<MapGenContext>>(startStep));

            //Run the generator and print
            MapGenContext context = layout.GenMap(MathUtils.Rand.NextUInt64());
            Print(context.Map, title);
        }


        public static void Print(Map map, string title)
        {
            int oldLeft = Console.CursorLeft;
            int oldTop = Console.CursorTop;
            Console.SetCursorPosition(0, 0);
            StringBuilder topString = new StringBuilder("");
            string turnString = title;
            topString.Append(String.Format("{0,-82}", turnString));
            topString.Append('\n');
            for (int i = 0; i < map.Width + 1; i++)
                topString.Append("=");
            topString.Append('\n');

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    Loc loc = new Loc(x, y);
                    char tileChar = ' ';
                    Tile tile = map.Tiles[x][y];
                    if (tile.ID <= 0)//wall
                        tileChar = '#';
                    else if (tile.ID == 1)//floor
                        tileChar = '.';
                    else if (tile.ID == 2)//water
                        tileChar = '~';
                    else
                        tileChar = '?';


                    foreach (StairsUp entrance in map.GenEntrances)
                    {
                        if (entrance.Loc == loc)
                        {
                            tileChar = '<';
                            break;
                        }
                    }
                    foreach (StairsDown entrance in map.GenExits)
                    {
                        if (entrance.Loc == loc)
                        {
                            tileChar = '>';
                            break;
                        }
                    }


                    foreach (Item item in map.Items)
                    {
                        if (item.Loc == loc)
                        {
                            tileChar = (char)item.ID;
                            break;
                        }
                    }

                    topString.Append(tileChar);
                }
                topString.Append('\n');
            }
            Console.Write(topString.ToString());
        }
    }
}
