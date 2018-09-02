using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace AStar
{

    public partial class Form1 : Form
    {
        public const int WIDTH = 400;
        public const int HEIGHT = 400;
        public static int TILE_SIZE = 20;
        Image buf = null;
        Pen pen;
        
        SolidBrush brush;
        private Graphics g;

        private Queue<Node> nodes;
        private List<int> openList;
        private List<int> closeList;
        private List<Node> path;
        Node startNode, endNode, currentNode;
        Point startPoint;
        Point endPoint;
        
        public Form1()
        {

            InitializeComponent();
            button1.Text = "Generate level";
            button2.Text = "Calculate optimal way";
            button2.Enabled = false;
            groupBox1.Text = "Show parameters";
            radioButton1.Text = "Price";
            radioButton1.CheckedChanged += RadioButton_CheckedChanged;
            radioButton2.Text = "Distances";
            radioButton2.CheckedChanged += RadioButton_CheckedChanged;
            radioButton3.Text = "Neighbors";
            radioButton3.CheckedChanged += RadioButton_CheckedChanged;
            radioButton4.Text = "Indices";
            radioButton4.CheckedChanged += RadioButton_CheckedChanged;
            pictureBox1.Width = WIDTH;
            pictureBox1.Height = HEIGHT;
            pen = new Pen(Color.Black, 1);
            g = Graphics.FromHwnd(pictureBox1.Handle);
            brush = new SolidBrush(Color.Beige);
            nodes = new Queue<Node>();
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (!rb.Checked) return;
          
            if (rb.Tag.ToString() == "100")
            {
                drawWeights();
                Invalidate();
            }
            if(rb.Tag.ToString() == "101")
            {
                drawDistances();
                Invalidate();
            }
            if(rb.Tag.ToString() == "102")
            {
                drawNeighbors();
                Invalidate();
            }
            if(rb.Tag.ToString() == "103")
            {
                drawIndeces();
                Invalidate();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            createLevel();
            calculateDistances();
            button2.Enabled = true;
            if (radioButton1.Checked) drawWeights();
            if (radioButton2.Checked) drawDistances();
            if (radioButton3.Checked) drawNeighbors();
            if (radioButton4.Checked) drawIndeces();
            

        }
        private void drawDistances()
        {
            drawLevel();
            foreach (Node node in nodes)
            {
                g.DrawString(node.DistanceToTarget.ToString(), new Font(FontFamily.GenericMonospace, TILE_SIZE / 2, FontStyle.Regular),
                       new SolidBrush(Color.Black), new Point(node.Location.X * TILE_SIZE, node.Location.Y * TILE_SIZE));
            }
        }
        private void drawWeights()
        {
            drawLevel();
            foreach (Node node in nodes)
            {
                g.DrawString(node.MoveValue.ToString(), new Font(FontFamily.GenericSansSerif, TILE_SIZE / 2, FontStyle.Regular),
                       new SolidBrush(Color.Black), new Point(node.Location.X * TILE_SIZE, node.Location.Y * TILE_SIZE));
            }
        }
        private void drawIndeces()
        {
            drawLevel();
            foreach (Node node in nodes)
            {
                g.DrawString(node.Index.ToString(), new Font(FontFamily.GenericMonospace, TILE_SIZE / 3, FontStyle.Regular),
                       new SolidBrush(Color.Black), new Point(node.Location.X * TILE_SIZE, node.Location.Y * TILE_SIZE));
            }
        }



        private Point generateRanPoint(Random rand)
        {
            int ranX = rand.Next(pictureBox1.Width / TILE_SIZE);
            int ranY = rand.Next(pictureBox1.Height / TILE_SIZE);
            if (ranX >= pictureBox1.Width / TILE_SIZE - 1) ranX--;
            if (ranY >= pictureBox1.Height / TILE_SIZE - 1) ranY--;
            if (ranX <= 0) ranX = 1;
            if (ranY <= 0) ranY = 1;
            return new Point(ranX, ranY);
        }

        private void createLevel()
        {
            nodes = new Queue<Node>();
            openList = new List<int>();
            closeList = new List<int>();
            blockTypes bt = blockTypes.NONE;
            Random rand = new Random();
            int moveValue = 0;
            int num = 0;

            startPoint = generateRanPoint(rand);
            endPoint = generateRanPoint(rand);
            int maxI = pictureBox1.Width / TILE_SIZE;
            int maxJ = pictureBox1.Height / TILE_SIZE;
            while (startPoint == endPoint)
            {
                endPoint = generateRanPoint(rand);
            }
            int index = 0;

            Color col;
            for (int i = 0; i < maxI;i++)
            {
                for (int j = 0; j < maxJ;j++)
                {
                    if (i == 0 || j == 0 || i == maxI - 1 || j == maxJ - 1)
                    {
                        bt = blockTypes.BORDER;
                        col = Color.DarkBlue;
                        moveValue = -1;
                        
                    }
                    else
                    {
                        num = 0;
                        num = rand.Next(4) + 2;
                       
                        switch (num)
                        {
                            case 2:
                                bt = blockTypes.FIELD;
                                col = Color.Wheat;
                                moveValue = 5;
                                break;
                            case 3:
                                bt = blockTypes.FOREST;
                                col = Color.ForestGreen;
                                moveValue = 15;
                                break;
                            case 4:
                                bt = blockTypes.MOUNTAIN;
                                col = Color.SaddleBrown;
                                moveValue = 35;
                                break;
                            default:
                                bt = blockTypes.VOID;
                                col = Color.Purple;
                                moveValue = 50;
                                break;


                        }
                    }
                    if (bt != blockTypes.NONE)
                    {
                        Node node = new Node(new Point(i, j), bt, col, moveValue, index);
                        
                        nodes.Enqueue(node);
                        if( i == startPoint.X && j== startPoint.Y)
                        {
                            startNode = node;
                        }
                        if(i == endPoint.X && j == endPoint.Y)
                        {
                            endNode = node;
                        }
                        
                    }
                    
                    index++;
                }
            }
            int startIndex = 0;
            int endIndex = 0;
            drawLevel();
            //NEIGHBORS
            foreach (Node node in nodes)
            {
                node.getNeighbors(nodes);
                if (node.Location == startPoint) startIndex = node.Index;
                if (node.Location == endPoint) endIndex = node.Index;
                
            }
            


        }
        private void drawLevel()
        {
            SolidBrush br;
            foreach(Node node in nodes)
            {
                br = new SolidBrush(node.NodeColor);
                if (node.Location == startNode.Location)
                {
                    br = new SolidBrush(Color.Goldenrod);
                }
                if (node.Location == endNode.Location)
                {
                    br = new SolidBrush(Color.DarkGray);
                }

                g.FillRectangle(br, node.Location.X * TILE_SIZE, node.Location.Y * TILE_SIZE,
                    TILE_SIZE, TILE_SIZE);
            }
        }
        private void calculateDistances()
        {
            foreach(Node node in nodes)
            {
                if (node.BlockType != blockTypes.BORDER)
                {
                    node.DistanceToTarget = heuristic(node.Location, endNode.Location);
                }
            }
        }
       

        private void Form1_Resize(object sender, EventArgs e)
        {

            if (this.Width <= pictureBox1.Width + 50) this.Width = pictureBox1.Width + 100;
            if (this.Height <= pictureBox1.Height + 50) this.Height = pictureBox1.Height + 100;
            button1_Click(sender, e);
            Invalidate();

        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            TILE_SIZE = (int)(Math.Round(hScrollBar1.Value * 10.0) / 10.0);
            button1_Click(sender, e);
            Invalidate();

        }
        private void drawNeighbors()
        {
            
            Color[] colors = { Color.Red, Color.PaleGoldenrod, Color.Purple, Color.Orchid, Color.Orange };
            int i = 0;
            foreach (Node node in nodes)
            {
                g.FillRectangle(new SolidBrush(Color.White), node.Location.X * TILE_SIZE, node.Location.Y * TILE_SIZE, TILE_SIZE, TILE_SIZE);
                foreach (Node sub in node.Neighbors.Keys)
                {
                    g.FillRectangle(new SolidBrush(colors[i]), sub.Location.X * TILE_SIZE, sub.Location.Y * TILE_SIZE, TILE_SIZE, TILE_SIZE);
                    Thread.Sleep(1);
                }
                i++;
                if (i > 4) i = 0;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            findDistance(startNode);
            path = new List<Node>();
            if(endNode.Parent !=null)
                createPath(endNode);
            drawLevel();
            if (radioButton1.Checked) drawWeights();
            if (radioButton2.Checked) drawDistances();
            if (radioButton3.Checked) drawNeighbors();
            if (radioButton4.Checked) drawIndeces();

        }
        private void createPath(Node node)
        {
            Node n = node.Parent;
            if(n == startNode)
            {
                MessageBox.Show("PATH CREATED");
                return;
            }
            path.Add(n);
            n.NodeColor = Color.Tomato;
           // MessageBox.Show(n.Index.ToString());
            createPath(n);
        }
       
        private void findDistance(Node n)
        {
            int minVal = 65535;
            Node bestOption = null;
            currentNode = n;
            //CHECK IF ONE OF THE NODES IS MORE OPTIMAL
            if(currentNode != startNode) {
            float curNodeVal = currentNode.MoveValue + currentNode.Parent.MoveValue; //+

            foreach(var nodo in nodes)
            {
                Node buffer = null;
                if (openList.Contains(nodo.Index))
                {
                    buffer = nodo;
                    if (buffer.MoveValue + buffer.Parent.MoveValue < curNodeVal) {
                     //   MessageBox.Show(buffer.MoveValue + " " + buffer.Parent.MoveValue + " " + curNodeVal);
                    currentNode = buffer;
                }
                }
            }
            }
            //is checked
            closeList.Add(currentNode.Index);
            openList.Remove(currentNode.Index);
            //Check all the neightbors
            foreach (var neighbor in currentNode.Neighbors)
            {
                Node node = neighbor.Key;
                int moveValue = neighbor.Value;
                //already checked
                if (closeList.Contains(node.Index) || neighbor.Key.MoveValue == -1) { continue; }
                //add new
                if (!openList.Contains(node.Index)) { openList.Add(node.Index);}
                //CHECK IF TARGET NODE IS NEXT TO THE CURRENT NODE
                if (neighbor.Key.Index == endNode.Index)
                {
                    //FOUND!
                    neighbor.Key.Parent = currentNode;
                    MessageBox.Show("PATH FOUND!");
                    //Job's done
                    return;
                }
                //NOT FOUND
                else
                {
                    //REPARENTING
                    if (node.Parent == null)
                        node.Parent = currentNode;
                    else
                    {
                        int oldValue = node.Parent.MoveValue;
                        foreach(var o in node.Parent.Neighbors)
                        {
                            if(o.Key.Index == node.Index)
                            {
                                oldValue += o.Value;
                            }
                        }
                        //MAKE REPARENTING
                        if (currentNode.MoveValue + moveValue < 
                            oldValue)
                            node.Parent = currentNode;
                    }
                    //REPARENTING DONE, CHECK THE BEST OPTION
                    if (moveValue + currentNode.MoveValue < minVal)
                    {
                        minVal = moveValue + currentNode.MoveValue;
                        bestOption = node;
                    }
                }

            }
            if(bestOption != null)
            {
                findDistance(bestOption);
            }
            
        }


        private double heuristic(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

       
    }
}

