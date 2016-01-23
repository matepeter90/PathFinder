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
        public int Total { get { return Heuristic + Cost; } }
        public int Heuristic { get; set; }
        public int Cost { get; set; }

        public Node(MapCell cell, Node parent, int cost, int heur)
        {
            Cell = cell;
            Heuristic = heur;
            Parent = parent;
            Cost = cost;
        }

        public int getTotal()
        {
            return Heuristic + Cost;
        }

    }

    class Map
    {
        public List<MapRow> Rows = new List<MapRow>();
        private Texture2D mouseMap;
        private Texture2D slopeMaps;
        public int MapWidth = 50;
        public int MapHeight = 50;
        public int prevHeightOffset = 0;

        public Map(Texture2D mouseMap, Texture2D slopeMaps)
        {
            this.mouseMap = mouseMap;
            this.slopeMaps = slopeMaps;
            for (int y = 0; y < MapHeight; y++)
            {
                MapRow thisRow = new MapRow();
                for (int x = 0; x < MapWidth; x++)
                {
                    thisRow.Columns.Add(new MapCell(x,y,0));
                }
                Rows.Add(thisRow);
            }

            Rows[0].Columns[3].TileID = 3;
            Rows[0].Columns[4].TileID = 3;
            Rows[0].Columns[5].TileID = 1;
            Rows[0].Columns[6].TileID = 1;
            Rows[0].Columns[7].TileID = 1;

            Rows[1].Columns[3].TileID = 3;
            Rows[1].Columns[4].TileID = 1;
            Rows[1].Columns[5].TileID = 1;
            Rows[1].Columns[6].TileID = 1;
            Rows[1].Columns[7].TileID = 1;

            Rows[2].Columns[2].TileID = 3;
            Rows[2].Columns[3].TileID = 1;
            Rows[2].Columns[4].TileID = 1;
            Rows[2].Columns[5].TileID = 1;
            Rows[2].Columns[6].TileID = 1;
            Rows[2].Columns[7].TileID = 1;

            Rows[3].Columns[2].TileID = 3;
            Rows[3].Columns[3].TileID = 1;
            Rows[3].Columns[4].TileID = 1;
            Rows[3].Columns[5].TileID = 2;
            Rows[3].Columns[6].TileID = 2;
            Rows[3].Columns[7].TileID = 2;

            Rows[4].Columns[2].TileID = 3;
            Rows[4].Columns[3].TileID = 1;
            Rows[4].Columns[4].TileID = 1;
            Rows[4].Columns[5].TileID = 2;
            Rows[4].Columns[6].TileID = 2;
            Rows[4].Columns[7].TileID = 2;

            Rows[5].Columns[2].TileID = 3;
            Rows[5].Columns[3].TileID = 1;
            Rows[5].Columns[4].TileID = 1;
            Rows[5].Columns[5].TileID = 2;
            Rows[5].Columns[6].TileID = 2;
            Rows[5].Columns[7].TileID = 2;

            Rows[16].Columns[4].AddHeightTile(54);

            Rows[17].Columns[3].AddHeightTile(54);

            Rows[15].Columns[3].AddHeightTile(54);
            Rows[16].Columns[3].AddHeightTile(53);

            Rows[15].Columns[4].AddHeightTile(54);
            Rows[15].Columns[4].AddHeightTile(54);
            Rows[15].Columns[4].AddHeightTile(51);

            Rows[18].Columns[3].AddHeightTile(51);
            Rows[19].Columns[3].AddHeightTile(50);
            Rows[18].Columns[4].AddHeightTile(55);

            Rows[14].Columns[4].AddHeightTile(54);

            Rows[14].Columns[5].AddHeightTile(62);
            Rows[14].Columns[5].AddHeightTile(61);
            Rows[14].Columns[5].AddHeightTile(63);

            Rows[17].Columns[4].AddTopperTile(114);
            Rows[16].Columns[5].AddTopperTile(115);
            Rows[14].Columns[4].AddTopperTile(125);
            Rows[15].Columns[5].AddTopperTile(91);
            Rows[16].Columns[6].AddTopperTile(94);
            Rows[15].Columns[5].Walkable = false;
            Rows[16].Columns[6].Walkable = false;

            Rows[12].Columns[9].AddHeightTile(34);
            Rows[11].Columns[9].AddHeightTile(34);
            Rows[11].Columns[8].AddHeightTile(34);
            Rows[10].Columns[9].AddHeightTile(34);

            Rows[12].Columns[8].AddTopperTile(31);
            Rows[12].Columns[8].SlopeMap = 0;
            Rows[13].Columns[8].AddTopperTile(31);
            Rows[13].Columns[8].SlopeMap = 0;

            Rows[12].Columns[10].AddTopperTile(32);
            Rows[12].Columns[10].SlopeMap = 1;
            Rows[13].Columns[9].AddTopperTile(32);
            Rows[13].Columns[9].SlopeMap = 1;

            Rows[14].Columns[9].AddTopperTile(30);
            Rows[14].Columns[9].SlopeMap = 4;
        }

        public Dictionary<String,MapCell> GetNeighbours(MapCell selectedcell)
        {
            Dictionary<String, MapCell> neighbours = new Dictionary<string, MapCell>();
            Point cell = new Point(selectedcell.X, selectedcell.Y);
            if (cell.Y % 2 == 1)
                cell.X += 1;
            if (cell.Y > 0 && cell.X > 0)
                neighbours.Add("N", Rows[cell.Y - 1].Columns[cell.X - 1]);
            if (cell.Y > 1 && cell.X <= MapWidth)
                neighbours.Add("NE", Rows[cell.Y - 2].Columns[cell.X]);
            if (cell.Y > 0 && cell.X <= MapWidth)
                neighbours.Add("E", Rows[cell.Y - 1].Columns[cell.X]);
            if (cell.Y < MapWidth && cell.X < MapWidth)
                neighbours.Add("SE", Rows[cell.Y].Columns[cell.X + 1]);
            if (cell.Y < MapHeight && cell.X <= MapWidth)
                neighbours.Add("S", Rows[cell.Y + 1].Columns[cell.X]);
            if (cell.Y < (MapWidth - 1) && cell.X <= MapWidth)
                neighbours.Add("SW", Rows[cell.Y + 2].Columns[cell.X]);
            if (cell.Y < MapHeight && cell.X > 0)
                neighbours.Add("W", Rows[cell.Y + 1].Columns[cell.X - 1]);
            if (cell.Y < MapHeight && cell.X > 0)
                neighbours.Add("NW", Rows[cell.Y].Columns[cell.X - 1]);
            return neighbours;
        }

        public int GetSlopeMapHeight(Point localPixel, int slopeMap)
        {
            Point texturePoint = new Point(slopeMap * mouseMap.Width + localPixel.X, localPixel.Y);
            Color[] slopeColor = new Color[1];
            int offset = 0;
            if (new Rectangle(0, 0, slopeMaps.Width, slopeMaps.Height).Contains(texturePoint.X, texturePoint.Y))
            {
                slopeMaps.GetData(0, new Rectangle(texturePoint.X, texturePoint.Y, 1, 1), slopeColor, 0, 1);
                if (slopeColor[0].R == slopeColor[0].G && slopeColor[0].R == slopeColor[0].B)
                {
                    offset = (int)(((float)(255 - slopeColor[0].G) / 255f) * Tile.HeightOffset);
                    prevHeightOffset = offset;
                }
                else
                    offset = prevHeightOffset;
            }
            return offset;
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

        public int GetSlopeHeightAtWorldPoint(Point worldPoint)
        {
            Point localPoint;
            Point mapPoint = WorldToMapCell(worldPoint, out localPoint);
            int slopeMap = Rows[mapPoint.Y].Columns[mapPoint.X].SlopeMap;

            return GetSlopeMapHeight(localPoint, slopeMap);
        }

        public int GetSlopeHeightAtWorldPoint(Vector2 worldPoint)
        {
            return GetSlopeHeightAtWorldPoint(new Point((int)worldPoint.X, (int)worldPoint.Y));
        }
        public int GetOverallHeight(Point worldPoint)
        {
            Point mapCellPoint = WorldToMapCell(worldPoint);
            int height = Rows[mapCellPoint.Y].Columns[mapCellPoint.X].HeightTiles.Count * Tile.HeightOffset;
            height += GetSlopeHeightAtWorldPoint(worldPoint);

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
        public int distance(MapCell cell, MapCell target)
        {
            return (int)Math.Sqrt(Math.Pow(cell.X - target.X, 2) + Math.Pow(cell.Y - target.Y, 2));
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
                foreach (var neighbour in GetNeighbours(currentCell.Cell))
                {
                    if (neighbour.Value == null)
                        continue;
                    if (neighbour.Value.Walkable &&
                        Math.Abs(GetCellHeight(currentCell.Cell) 
                        - GetCellHeight(neighbour.Value)) < 16)
                    { 
                        if (closedList.Any(x => x.Cell == neighbour.Value))
                            continue;
                        Node element = openList.Where(x => x.Cell == neighbour.Value).FirstOrDefault();
                        if (element == null)
                        {
                            openList.Add(new Node(
                                    neighbour.Value,
                                    currentCell,
                                    currentCell.Cost + 1,
                                    distance(neighbour.Value, targetCell)));
                        }
                        else
                        {
                            int cost = currentCell.Cost + 1 + distance(neighbour.Value, targetCell);
                            if (cost < element.getTotal())
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
