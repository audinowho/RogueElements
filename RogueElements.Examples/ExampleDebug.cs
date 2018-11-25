using System;
using System.Collections.Generic;
using System.Text;
using RogueElements;
using System.Diagnostics;

namespace RogueElements.Examples
{
    public static class ExampleDebug
    {
        static IGenContext map;
        static string gridDebugString;
        static string listDebugString;
        static string tileDebugString;

        public static void Init(IGenContext newMap)
        {
            map = newMap;
            gridDebugString = "";
            listDebugString = "";
            tileDebugString = "";
        }
        public static void OnStep()
        {
            updateString(ref gridDebugString, PrintGridRoomHalls());

            updateString(ref listDebugString, PrintListRoomHalls());

            updateString(ref tileDebugString, PrintTiles());
        }

        private static void updateString(ref string targetString, string newString)
        {
            if (targetString != newString)
                Debug.Write(newString);
            targetString = newString;
        }

        public static string PrintTiles()
        {
            ITiledGenContext context = map as ITiledGenContext;
            if (context == null)
                return "";
            if (!context.TilesInitialized)
                return "";

            StringBuilder str = new StringBuilder();

            for (int yy = 0; yy < context.Height; yy++)
            {
                for (int xx = 0; xx < context.Width; xx++)
                {
                    if (context.GetTile(new Loc(xx, yy)).TileEquivalent(context.RoomTerrain))
                        str.Append('.');
                    else if (context.GetTile(new Loc(xx, yy)).TileEquivalent(context.WallTerrain))
                        str.Append('#');
                    else
                    {
                        if (context.TileBlocked(new Loc(xx, yy)))
                            str.Append('+');
                        else
                            str.Append('_');
                    }
                }
                str.Append('\n');
            }
            str.Append('\n');

            return str.ToString();
        }

        public static string PrintListRoomHalls()
        {
            IFloorPlanGenContext context = map as IFloorPlanGenContext;
            if (context == null)
                return "";

            StringBuilder str = new StringBuilder();
            FloorPlan plan = context.RoomPlan;
            if (plan == null)
                return "";

            for (int yy = 0; yy < plan.DrawRect.Bottom; yy++)
            {
                for (int xx = 0; xx < plan.DrawRect.Right; xx++)
                {
                    str.Append(' ');
                }
            }

            for (int ii = 0; ii < plan.RoomCount; ii++)
            {
                char chosenChar = '@';
                if (ii < 26)
                    chosenChar = (char)('A' + ii);
                IRoomGen gen = plan.GetRoom(ii);
                for (int xx = gen.Draw.Left; xx < gen.Draw.Right; xx++)
                {
                    for (int yy = gen.Draw.Top; yy < gen.Draw.Bottom; yy++)
                    {
                        int index = yy * plan.DrawRect.Right + xx;

                        if (str[index] == ' ')
                            str[index] = chosenChar;
                        else
                            str[index] = '!';
                    }
                }
            }
            for (int ii = 0; ii < plan.HallCount; ii++)
            {
                char chosenChar = '#';
                if (ii < 26)
                    chosenChar = (char)('a' + ii);

                IRoomGen gen = plan.GetHall(ii);

                for (int xx = gen.Draw.Left; xx < gen.Draw.Right; xx++)
                {
                    for (int yy = gen.Draw.Top; yy < gen.Draw.Bottom; yy++)
                    {
                        int index = yy * plan.DrawRect.Right + xx;

                        if (str[index] == ' ')
                            str[index] = chosenChar;
                        else if (str[index] >= 'a' && str[index] <= 'z' || str[index] == '#')
                            str[index] = '+';
                        else
                            str[index] = '!';
                    }
                }
            }

            for (int yy = plan.DrawRect.Bottom; yy > 0; yy--)
                str.Insert(plan.DrawRect.Right * yy, '\n');

            str.Append('\n');

            return str.ToString();
        }

        public static string PrintGridRoomHalls()
        {
            IRoomGridGenContext context = map as IRoomGridGenContext;
            if (context == null)
                return "";

            StringBuilder str = new StringBuilder();
            GridPlan plan = context.GridPlan;
            if (plan == null)
                return "";

            for (int yy = 0; yy < plan.GridHeight; yy++)
            {
                for (int xx = 0; xx < plan.GridWidth; xx++)
                {
                    int roomIndex = plan.GetRoomIndex(new Loc(xx, yy));
                    if (roomIndex == -1)
                        str.Append('0');
                    else if (roomIndex < 26)
                        str.Append((char)('A' + roomIndex));
                    else
                        str.Append('@');

                    if (xx < plan.GridWidth - 1)
                    {
                        if (plan.GetHall(new LocRay4(xx, yy, Dir4.Right)) != null)
                            str.Append('#');
                        else
                            str.Append('.');
                    }
                }
                str.Append('\n');

                if (yy < plan.GridHeight - 1)
                {
                    for (int xx = 0; xx < plan.GridWidth; xx++)
                    {
                        if (plan.GetHall(new LocRay4(xx, yy, Dir4.Down)) != null)
                            str.Append('#');
                        else
                            str.Append('.');

                        if (xx < plan.GridWidth - 1)
                        {
                            str.Append(' ');
                        }
                    }
                    str.Append('\n');
                }
            }
            str.Append('\n');

            return str.ToString();
        }
    }
}
