using System;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;

namespace GameArea
{
    class GameBoard
    {
        public GameArea.Tile[,] map { get; private set; }
        public int powerup { get; set; }
        public int currentScore { get; private set; }
        public int maxScore { get; private set; }

        /* Constructor of GameBoard. Matrix represents game area where 0 are empty spaces, -1 walls, -2 food,
         * -3 powerups*/
        public GameBoard(int[,] matrix, Canvas Window)
        {
            currentScore = 0;
            maxScore = 0;
            map = new GameArea.Tile[matrix.GetLength(0), matrix.GetLength(1)];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    switch (matrix[i, j])
                    {
                        case 0:
                            map[i, j] = new GameArea.Tile(Window, GameArea.Tile.types.passage);
                            break;
                        case -1:
                            map[i, j] = new GameArea.Tile(Window, GameArea.Tile.types.wall);
                            break;
                        case -2:
                            map[i, j] = new GameArea.Tile(Window, GameArea.Tile.types.food);
                            maxScore++;
                            break;
                        case -3:
                            map[i, j] = new GameArea.Tile(Window, GameArea.Tile.types.powerup);
                            break;
                    }

                }
            }
            powerup = 0;
        }

        public void Draw(Canvas Board, GameArea.GameBoard board)
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    map[i, j].Draw(Board, j, i, board);
                }
            }
        }

        /* Cheks for collisions between pacman and ghosts and return the ghost if there are a collision.
         * Also checks for food or powerups and process their consumption.*/
        public Entities.Entity CollisionCheck(Entities.Pacman pacman, Entities.Blinky blinky, Entities.Clyde clyde, GameBoard board, Canvas GameWindow)
        {
            if (pacman.x == blinky.x && pacman.y == blinky.y)
            { return blinky; }
            if (pacman.x == clyde.x && pacman.y == clyde.y)
            { return clyde; }

            if (board.map[pacman.x, pacman.y].type == GameArea.Tile.types.food)
            {
                board.map[pacman.x, pacman.y].Remove(GameWindow);
                board.map[pacman.x, pacman.y].type = GameArea.Tile.types.passage;
                currentScore++;
            }
            if (board.map[pacman.x, pacman.y].type == GameArea.Tile.types.powerup)
            {
                board.map[pacman.x, pacman.y].Remove(GameWindow);
                board.map[pacman.x, pacman.y].type = GameArea.Tile.types.passage;
                board.powerup = 10;
            }

            return null;
        }
    }

    class Tile
    {
        public enum types : int { passage = 0, wall = -1, food = -2, powerup = -3 };
        public types type;
        private Shape sprite;

        public Tile(Canvas Window, types t)
        {
            switch (t)
            {
                case types.passage:
                    type = types.passage;
                    break;
                case types.wall:
                    type = types.wall;
                    sprite = new Rectangle
                    {
                        Fill = Brushes.Black
                    };
                    Window.Children.Add(sprite);
                    break;
                case types.food:
                    type = types.food;
                    sprite = new Rectangle()
                    {
                        Fill = Brushes.Pink
                    };
                    Window.Children.Add(sprite);
                    break;
                case types.powerup:
                    type = types.powerup;
                    sprite = new Ellipse()
                    {
                        Fill = Brushes.Blue
                    };
                    Window.Children.Add(sprite);
                    break;
            }
        }

        /* Draws all tiles according to their types*/
        public void Draw(Canvas Window,int x,int y, GameArea.GameBoard board)
        {
            if (type == types.wall)
            {
                sprite.Width = Window.ActualWidth / board.map.GetLength(0);
                sprite.Height = Window.ActualHeight / board.map.GetLength(1);
                Canvas.SetTop(sprite, y * Window.ActualHeight / board.map.GetLength(1));
                Canvas.SetLeft(sprite, x * Window.ActualWidth / board.map.GetLength(0));
            }
            if (type == types.food)
            {
                sprite.Width = Window.ActualWidth / board.map.GetLength(0) / 5;
                sprite.Height = Window.ActualHeight / board.map.GetLength(1) / 5;
                Canvas.SetTop(sprite, y * Window.ActualHeight / board.map.GetLength(1) + 2 * Window.ActualHeight / board.map.GetLength(1) / 5);
                Canvas.SetLeft(sprite, x * Window.ActualWidth / board.map.GetLength(0) + 2 * Window.ActualWidth / board.map.GetLength(0) / 5);
            }
            if (type == types.powerup)
            {
                sprite.Width = Window.ActualWidth / board.map.GetLength(0) / 2;
                sprite.Height = Window.ActualHeight / board.map.GetLength(1) / 2;
                Canvas.SetTop(sprite, y * Window.ActualHeight / board.map.GetLength(1) + Window.ActualHeight / board.map.GetLength(1) / 4);
                Canvas.SetLeft(sprite, x * Window.ActualWidth / board.map.GetLength(0) + Window.ActualWidth / board.map.GetLength(0) / 4);
            }
        }

        /* Tries to remove the tile from canvas. Catches an exception if it was not there*/
        public void Remove(Canvas Window)
        {
            try
            { Window.Children.Remove(sprite); }
            catch (Exception e)
            { Console.WriteLine(e.Message); }
        }
    }
}
