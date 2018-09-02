using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace AStar
{
    class Node
    {
        private Point location;
        private blockTypes blockType;
        private int moveValue;
        private int index;
        private bool isVisited;
        private double distanceToTarget = -1;
        private Color nodeColor;
        Dictionary<Node, int> neighbors;
        Node parent = null;

        
        public Node(Point location, blockTypes type, Color col, int value, int index)
        {
            
            this.location = location;
            blockType = type;
            nodeColor = col;
            moveValue = value;
            this.index = index;
        }

        public Point Location { get => location; }
        public blockTypes BlockType { get => blockType; }
      
        public void getNeighbors(Queue<Node> nodes)
        {
            int size = Form1.TILE_SIZE;
            neighbors = new Dictionary<Node, int>();
            foreach(Node node in nodes)
            {
                if (node.BlockType != blockTypes.BORDER && BlockType != blockTypes.BORDER)
                {
                    if ((Math.Abs(node.location.X - location.X) == 1 || node.Location.X == location.X) &&
                        (Math.Abs(node.location.Y - location.Y) == 1 || node.Location.Y == location.Y ||
                        (location.Y == node.Location.Y && location.X != node.Location.X)))
                    {
                        if (node.Location != location) {
                        if ((node.Location.X != location.X) &&
                            (node.Location.Y != location.Y))
                            neighbors.Add(node, 14 + node.MoveValue);
                        else
                        {
                            neighbors.Add(node, 10 + node.MoveValue);
                        }
                    }
                    }          
                }
            }
        }
        public int Index { get => index; }
        public Dictionary<Node, int> Neighbors { get => neighbors; }
        public bool IsVisited { get => isVisited; set => isVisited = value; }
        public double DistanceToTarget { get => distanceToTarget; set => distanceToTarget = value; }
        public Color NodeColor { get => nodeColor; set => nodeColor = value; }
        internal Node Parent { get => parent; set => parent = value; }
        public int MoveValue { get => moveValue; set => moveValue = value; }
    }
}
