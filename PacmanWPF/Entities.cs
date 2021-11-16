using System;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;

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
        /* Sets direction. First calls wave algorithm, fills empty matrix with it, then find path back from 
         * endpoint - pacman marking way with -1s since all other numbers are positive and depend on area.*/
        public void set_direction(GameArea.GameBoard board, Pacman packman)
        {
            int [,] matrix=new int[board.map.GetLength(0), board.map.GetLength(0)];
            int x_dest = packman.x;
            int y_dest = packman.y;
            matrix[x,y] = 1;
            wave(ref matrix, ref board, this.x, this.y, x_dest, y_dest, 1);
            
            while ((x_dest != x) || (y_dest != y))
            {

                if (y_dest + 1 < matrix.GetLength(1) && matrix[x_dest, y_dest + 1] == matrix[x_dest, y_dest] - 1)
                {
                    matrix[x_dest, y_dest] = -1;
                    y_dest = y_dest + 1;
                    continue;
                }
                if (y_dest > 0 && matrix[x_dest, y_dest - 1] == matrix[x_dest, y_dest] - 1)
                {
                    matrix[x_dest, y_dest] = -1;
                    y_dest = y_dest - 1;
                    continue;
                }
                if (x_dest + 1 < matrix.GetLength(0) && matrix[x_dest + 1, y_dest] == matrix[x_dest, y_dest] - 1)
                {
                    matrix[x_dest, y_dest] = -1;
                    x_dest = x_dest + 1;
                    continue;
                }
                if (x_dest > 0 && matrix[x_dest - 1, y_dest] == matrix[x_dest, y_dest] - 1)
                {
                    matrix[x_dest, y_dest] = -1;
                    x_dest = x_dest - 1;
                    continue;
                }
            }
            
            if (y_dest + 1 < matrix.GetLength(1) && matrix[x_dest, y_dest + 1] == -1)
            {
                this.direction = directions.Right;
            }
            if (y_dest > 0 && matrix[x_dest, y_dest - 1] == -1)
            {
                this.direction = directions.Left;
            }
            if (x_dest + 1 < matrix.GetLength(0) && matrix[x_dest + 1, y_dest] == -1)
            {
                this.direction = directions.Down;
            }
            if (x_dest > 0 && matrix[x_dest - 1, y_dest] == -1)
            {
                this.direction = directions.Up;
            }

        }
        /* Wave algorithm to find the shortest way to pacman*/
        private void wave(ref int[,] matrix, ref GameArea.GameBoard board, int x_org, int y_org, int x_dest, int y_dest, int count)
        {
            if (x_org == x_dest && y_org == y_dest)
                return;

            if (y_org + 1 < matrix.GetLength(1) && board.map[x_org, y_org + 1].type != GameArea.Tile.types.wall && (matrix[x_org, y_org + 1] == 0 || matrix[x_org, y_org + 1] > count + 1))
            {
                matrix[x_org, y_org + 1] = count + 1;
                wave(ref matrix, ref board, x_org, y_org + 1, x_dest, y_dest, count + 1);
            }
            if (y_org > 0 && board.map[x_org, y_org - 1].type != GameArea.Tile.types.wall && (matrix[x_org, y_org - 1] == 0 || matrix[x_org, y_org - 1] > count + 1))
            {
                matrix[x_org, y_org - 1] = count + 1;
                wave(ref matrix, ref board, x_org, y_org - 1, x_dest, y_dest, count + 1);
            }
            if (x_org + 1 < matrix.GetLength(0) && board.map[x_org + 1, y_org].type != GameArea.Tile.types.wall && (matrix[x_org + 1, y_org] == 0 || matrix[x_org + 1, y_org] > count + 1))
            {
                matrix[x_org + 1, y_org] = count + 1;
                wave(ref matrix, ref board, x_org + 1, y_org, x_dest, y_dest, count + 1);
            }
            if (x_org > 0 && board.map[x_org - 1, y_org].type != GameArea.Tile.types.wall && (matrix[x_org - 1, y_org] == 0 || matrix[x_org - 1, y_org] > count + 1))
            {
                matrix[x_org - 1, y_org] = count + 1;
                wave(ref matrix, ref board, x_org - 1, y_org, x_dest, y_dest, count + 1);
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

