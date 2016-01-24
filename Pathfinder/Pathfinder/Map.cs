using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pathfinder
{
    class MapRow
    {
        public List<MapCell> Columns = new List<MapCell>();
    }

    class Node
    {
        public MapCell Cell { get; set; }
        public Node Parent { get; set; }
        public float Total { get { return Heuristic + Cost; } }
        public float Heuristic { get; set; }
        public float Cost { get; set; }

        public Node(MapCell cell, Node parent, float cost, float heur)
        {
            Cell = cell;
            Heuristic = heur;
            Parent = parent;
            Cost = cost;
        }

        public float getTotal()
        {
            return Heuristic + Cost;
        }

    }

    class Map
    {
        public List<MapRow> Rows = new List<MapRow>();
        private Texture2D mouseMap;
        public int MapWidth = 50;
        public int MapHeight = 50;
        public int prevHeightOffset = 0;

        public Map(Texture2D mouseMap)
        {
            this.mouseMap = mouseMap;
            Random r = new Random(4);
            for (int y = 0; y < MapHeight; y++)
            {
                MapRow thisRow = new MapRow();
                for (int x = 0; x < MapWidth; x++)
                {
                    thisRow.Columns.Add(new MapCell(x,y,0));
                    if (r.Next(0, 100) < 35)
                        thisRow.Columns[x].AddHeightTile(54);
                    else if (r.Next(0, 100) < 15)
                        thisRow.Columns[x].AddTopperTile(110);
                }
                Rows.Add(thisRow);
            }
        }

        public Dictionary<string, MapCell> GetNeighbours(MapCell selectedcell)
        {
            Dictionary<String, MapCell> neighbours = new Dictionary<string, MapCell>();
            Point cell = new Point(selectedcell.X, selectedcell.Y);
            int newX = cell.X;
            if (cell.Y % 2 == 1)
               newX += 1;
            if (cell.Y > 0 && newX > 0)
                neighbours.Add("N", Rows[cell.Y - 1].Columns[newX - 1]);
            if (cell.Y > 1 && cell.X < MapWidth)
                neighbours.Add("NE", Rows[cell.Y - 2].Columns[cell.X]);
            if (cell.Y > 0 && newX < MapWidth)
                neighbours.Add("E", Rows[cell.Y - 1].Columns[newX]);
            if (cell.Y < MapWidth && cell.X < (MapWidth - 1))
                neighbours.Add("SE", Rows[cell.Y].Columns[cell.X + 1]);
            if (cell.Y < (MapHeight-1) && newX < MapWidth)
                neighbours.Add("S", Rows[cell.Y + 1].Columns[newX]);
            if (cell.Y < (MapWidth - 2) && cell.X < MapWidth)
                neighbours.Add("SW", Rows[cell.Y + 2].Columns[cell.X]);
            if (cell.Y < (MapHeight - 1) && newX > 0)
                neighbours.Add("W", Rows[cell.Y + 1].Columns[newX - 1]);
            if (cell.Y < MapHeight && cell.X > 0)
                neighbours.Add("NW", Rows[cell.Y].Columns[cell.X - 1]);
            return neighbours;
        }

        public Point WorldToMapCell(Point worldPoint)
        {
            Point dummy;
            return WorldToMapCell(worldPoint, out dummy);
        }
        public Point WorldToMapCell(Point worldPoint, out Point localPoint)
        {
            Point mapCell = new Point(
               (int)(worldPoint.X / mouseMap.Width),
               ((int)(worldPoint.Y / mouseMap.Height)) * 2
               );

            int localPointX = worldPoint.X % mouseMap.Width;
            int localPointY = worldPoint.Y % mouseMap.Height;

            int dx = 0;
            int dy = 0;

            Color[] mouseColor = new Color[1];

            if (new Rectangle(0, 0, mouseMap.Width, mouseMap.Height).Contains(localPointX, localPointY))
            {
                mouseMap.GetData(0, new Rectangle(localPointX, localPointY, 1, 1), mouseColor, 0, 1);

                if (mouseColor[0] == Color.Red)
                {
                    dx = -1;
                    dy = -1;
                    localPointX = localPointX + (mouseMap.Width / 2);
                    localPointY = localPointY + (mouseMap.Height / 2);
                }

                if (mouseColor[0] == Color.Green)
                {
                    dx = -1;
                    localPointX = localPointX + (mouseMap.Width / 2);
                    dy = 1;
                    localPointY = localPointY - (mouseMap.Height / 2);
                }

                if (mouseColor[0] == Color.Yellow)
                {
                    dy = -1;
                    localPointX = localPointX - (mouseMap.Width / 2);
                    localPointY = localPointY + (mouseMap.Height / 2);
                }

                if (mouseColor[0] == Color.Blue)
                {
                    dy = +1;
                    localPointX = localPointX - (mouseMap.Width / 2);
                    localPointY = localPointY - (mouseMap.Height / 2);
                }
            }

            mapCell.X += dx;
            mapCell.Y += dy - 2;

            localPoint = new Point(localPointX, localPointY);

            return mapCell;
        }

        public Point WorldToMapCell(Vector2 worldPoint)
        {
            return WorldToMapCell(new Point((int)worldPoint.X, (int)worldPoint.Y));
        }

        public MapCell GetCellAtWorldPoint(Point worldPoint)
        {
            Point cell = WorldToMapCell(worldPoint);
            if (cell.Y >= 0 && cell.X >= 0 && cell.Y <= MapHeight && cell.X <= MapWidth)
                return Rows[cell.Y].Columns[cell.X];
            return null;
        }

        public MapCell GetCellAtWorldPoint(Vector2 worldPoint)
        {
            return GetCellAtWorldPoint(new Point((int)worldPoint.X, (int)worldPoint.Y));
        }

        public int GetOverallHeight(Point worldPoint)
        {
            Point mapCellPoint = WorldToMapCell(worldPoint);
            int height = Rows[mapCellPoint.Y].Columns[mapCellPoint.X].HeightTiles.Count * Tile.HeightOffset;

            return height;
        }

        public int GetCellHeight(MapCell cell)
        {
            return Rows[cell.Y].Columns[cell.X].HeightTiles.Count * Tile.HeightOffset;
        }

        public int GetOverallHeight(Vector2 worldPoint)
        {
            return GetOverallHeight(new Point((int)worldPoint.X, (int)worldPoint.Y));
        }
        public float distance(MapCell cell, MapCell target)
        {
            return (float)Math.Sqrt(Math.Pow(cell.X - target.X, 2) + Math.Pow(cell.Y - target.Y, 2));
        }
        public List<Point> getPath(Point start, Point target)
        {
            List<Node> closedList = new List<Node>();
            List<Node> openList = new List<Node>();
            MapCell startCell = GetCellAtWorldPoint(start);
            MapCell targetCell = GetCellAtWorldPoint(target);
            openList.Add(new Node(startCell, null, 0, distance(startCell, targetCell)));
            do
            {
                var currentCell = openList.Aggregate((curMin, x) => (x.getTotal() < curMin.getTotal()) ? x : curMin);
                closedList.Add(currentCell);
                openList.Remove(currentCell);
                if (closedList.Any(x => x.Cell == targetCell))
                    break;
                var neighbours = GetNeighbours(currentCell.Cell);
                foreach (var neighbour in neighbours)
                {
                    if (neighbour.Value == null)
                        continue;
                    bool canWalkthrough = true;
                    if(neighbour.Key.Length == 2)
                    {
                        try
                        {
                            MapCell a = neighbours[neighbour.Key[0].ToString()];
                            MapCell b = neighbours[neighbour.Key[1].ToString()];
                            if (a.HeightTiles.Count > 0 && b.HeightTiles.Count > 0)
                                canWalkthrough = false;
                        }
                        catch { };
                    }
                    if (neighbour.Value.Walkable && canWalkthrough &&
                        Math.Abs(GetCellHeight(currentCell.Cell) - GetCellHeight(neighbour.Value)) < 16)
                    { 
                        if (closedList.Any(x => x.Cell == neighbour.Value))
                            continue;
                        Node element = openList.Where(x => x.Cell == neighbour.Value).FirstOrDefault();
                        float cost = currentCell.Cost + 1;
                        if (neighbour.Key.Length == 2)
                            cost += 0.2f;
                        if (element == null)
                        {
                            if (neighbour.Key.Length == 2)
                                cost += 0.2f;
                            openList.Add(new Node(
                                    neighbour.Value,
                                    currentCell,
                                    cost,
                                    distance(neighbour.Value, targetCell)));
                        }
                        else
                        {
                            float newCost = cost + distance(neighbour.Value, targetCell);
                            if (newCost < element.getTotal())
                            {
                                element.Parent = currentCell;
                            }
                        }
                    }
                }

            } while (openList.Count > 0);
            var node = closedList.Where(x => x.Cell == targetCell).FirstOrDefault();
            List<Point> path = new List<Point>();
            while (node != null)
            {
                path.Add(new Point(node.Cell.X,node.Cell.Y));
                node = node.Parent;
            };
            path.Reverse();
            return path;
        }
    }
}
