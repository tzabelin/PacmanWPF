using System;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;

namespace Entities
{
    abstract class Entity
    {
        public int x { get; private set; }
        public int y { get; private set; }
        protected enum directions:int { Left=1, Right=-1, Up=2, Down=-2 };
        protected directions direction;
        public abstract void Draw(Canvas Board, GameArea.GameBoard board);
        protected Shape sprite;
        public Entity(int _x, int _y)
        {
            this.x = _x;
            this.y = _y;
            direction = directions.Right;
        }
        public void reverse_direction()
        {
            direction = (directions) (- 1 * (int)direction);

        }
        public void move(GameArea.GameBoard Board)
        {
            switch (direction)
            {
                case directions.Right:
                    if ((y < Board.map.GetLength(1)) && (Board.map[this.x, this.y + 1]).type != GameArea.Tile.types.wall) { this.y++; }
                    break;
                case directions.Left:
                    if ((y > 0) && (Board.map[this.x, this.y -1]).type != GameArea.Tile.types.wall) { this.y--; }
                    break;
                case directions.Down:
                    if ((x < Board.map.GetLength(0)) && (Board.map[this.x+1, this.y]).type != GameArea.Tile.types.wall) { this.x++; }
                    break;
                case directions.Up:
                    if ((x > 0) && (Board.map[this.x-1, this.y]).type != GameArea.Tile.types.wall) { this.x--; }
                    break;
            }
        }
        /* Resets entity and sends it to specified location on board*/
        public void Reset(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.direction = directions.Right;
        }
    }

    class Pacman : Entity
    {
        public Pacman(int _x, int _y, Canvas Board) : base(_x, _y)
        {
            sprite = new Ellipse()
            {
                Fill = Brushes.Yellow
            };
            Board.Children.Add(sprite);
        }
        public void set_direction(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.D:
                case Key.Right:
                    this.direction = directions.Right;
                    break;
                case Key.A:
                case Key.Left:
                    this.direction = directions.Left;
                    break;
                case Key.S:
                case Key.Down:
                    this.direction = directions.Down;
                    break;
                case Key.W:
                case Key.Up:
                    this.direction = directions.Up;
                    break;
            }
        }

        override public void Draw(Canvas Board, GameArea.GameBoard board)
        {
            sprite.Width = Board.ActualWidth / board.map.GetLength(0);
            sprite.Height = Board.ActualWidth / board.map.GetLength(1);
            Canvas.SetTop(sprite, x* Board.ActualWidth / board.map.GetLength(1));
            Canvas.SetLeft(sprite, y* Board.ActualWidth / board.map.GetLength(0));
        }
    }

    class Blinky:Entity
    {
        public Blinky(int _x, int _y, Canvas Board) : base(_x, _y)
        {
            sprite = new Ellipse()
            {
                Fill = Brushes.Red
            };
            Board.Children.Add(sprite);
        }

        #region Direction
        /* Sets direction. Algorithm adds all vertex's neighbors in queue, that adds their neighbors and so on. 
         It starts at the end point - pacman. Length of the route increases by 1 per iteration. When it reaches blinky
         the shortest way is found and it returns.*/

        private class Vertex
        {
            public Vertex(int _x, int _y)
            {
                x = _x;
                y = _y;
            }
            public int x { get; set; }
            public int y { get; set; }
        }

        /*Cheks if a vertex is valid for the route. Also chaks that no vertex is visited twice.*/
        bool[,] DirectionCheker;
        private bool AddNewVertex(int x, int y, GameArea.GameBoard board)
        {
            if (y < board.map.GetLength(1) && y > 0 && x < board.map.GetLength(0) && x > 0
                && board.map[x, y].type != GameArea.Tile.types.wall && !DirectionCheker[x, y])
            {
                DirectionCheker[x, y] = true;
                return true;
            }
            return false;
        }

        public void set_direction(GameArea.GameBoard board, Pacman pacman)
        {
            DirectionCheker = new bool[board.map.GetLength(0), board.map.GetLength(1)];
            if (this.y == pacman.y && this.x == pacman.x)
            { return; }

            Queue <Vertex> vertices=new Queue<Vertex>();
            vertices.Enqueue(new Vertex(pacman.x, pacman.y));

            while(vertices.Count>0)
            {
                Vertex v = vertices.Dequeue();
                if (AddNewVertex(v.x, v.y + 1, board))
                {
                    if (v.y + 1 == this.y && v.x == this.x)
                    {
                        this.direction = directions.Left;
                        vertices.Clear();
                        return;
                    }
                    vertices.Enqueue(new Vertex(v.x, v.y + 1));
                }
                if (AddNewVertex(v.x, v.y - 1, board))
                {
                    if (v.y - 1 == this.y && v.x == this.x)
                    {
                        this.direction = directions.Right;
                        vertices.Clear();
                        return;
                    }
                    vertices.Enqueue(new Vertex(v.x, v.y - 1));
                }
                if (AddNewVertex(v.x + 1, v.y, board))
                {
                    if (v.y == this.y && v.x+1 == this.x)
                    {
                        this.direction = directions.Up;
                        vertices.Clear();
                        return;
                    }
                    vertices.Enqueue(new Vertex(v.x + 1, v.y ));
                }
                if (AddNewVertex(v.x - 1, v.y,  board))
                {
                    if (v.y == this.y && v.x-1 == this.x)
                    {
                        this.direction = directions.Down;
                        vertices.Clear();
                        return;
                    }
                    vertices.Enqueue(new Vertex(v.x - 1, v.y));
                }
            }
        }
        #endregion
       
        override public void Draw(Canvas Board, GameArea.GameBoard board)
        {
            sprite.Width = Board.ActualWidth / board.map.GetLength(0);
            sprite.Height = Board.ActualWidth / board.map.GetLength(1);
            Canvas.SetTop(sprite, x * Board.ActualWidth / board.map.GetLength(1));
            Canvas.SetLeft(sprite, y * Board.ActualWidth / board.map.GetLength(0));
        }
    }
    class Clyde : Entity
    {
        public Clyde(int _x, int _y, Canvas Board) : base(_x, _y)
        {
            sprite = new Ellipse()
            {
                Fill = Brushes.Orange
            };
            Board.Children.Add(sprite);
        }
        public void set_direction()
        {
            switch ((new Random()).Next() % 4)
            {
                case 0:
                    this.direction = directions.Right;
                    break;
                case 1:
                    this.direction = directions.Left;
                    break;
                case 2:
                    this.direction = directions.Down;
                    break;
                case 3:
                    this.direction = directions.Up;
                    break;
            }
        }

        override public void Draw(Canvas Board, GameArea.GameBoard board)
        {
            sprite.Width = Board.ActualWidth / board.map.GetLength(0);
            sprite.Height = Board.ActualWidth / board.map.GetLength(1);
            Canvas.SetTop(sprite, x * Board.ActualWidth / board.map.GetLength(1));
            Canvas.SetLeft(sprite, y * Board.ActualWidth / board.map.GetLength(0));
        }
    }
}

