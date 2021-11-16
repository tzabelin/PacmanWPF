using System.Windows.Input;
using System.Windows.Controls;

namespace PackmanWPF
{
    class Game
    {
        private Entities.Pacman pacman;
        private Entities.Blinky blinky;
        private Entities.Clyde clyde;
        private int timer_count;
        private GameArea.GameBoard board;

        public Game(Canvas GameWindow)
        {
            pacman = new Entities.Pacman(1, 2, GameWindow);
            blinky = new Entities.Blinky(5, 5, GameWindow);
            clyde = new Entities.Clyde(4, 4, GameWindow);
            timer_count = 0;
            board = new GameArea.GameBoard(new int[10, 10]
            { {-1,-1,-1,-1,-1,-1,-1,-1,-1,-1},
                {-1,-3,-2,-2,-2,-2,-2,-2,-3,-1},
                {-1,-2,-1,-1,-2,-2,-1,-1,-2,-1},
                {-1,-2,-1, -2, -2,-2,-2,-1 ,-2,-1},
                {-1,-2,-2,-2,-2,-2,-2,-2,-2,-1},
                {-1,-2,-2,-2,-2,-2,-2,-2,-2,-1},
                {-1,-2,-1,-2,-2,-2,-2,-1,-2,-1},
                {-1,-2,-1,-1,-2,-2,-1,-1,-2,-1},
                {-1,-3,-2,-2,-2,-2,-2,-2,-3,-1},
                {-1,-1,-1,-1,-1,-1,-1,-1,-1,-1}}, GameWindow);
        }

        /* Draws all entities and game board*/
        public void Draw(Canvas Window)
        {
            pacman.Draw(Window, board);
            blinky.Draw(Window, board);
            clyde.Draw(Window, board);
            board.Draw(Window, board);
        }

        /* Represend one move of game. Sets directions and move entities.*/
        public void DoMove()
        {
            if (timer_count == 0 || timer_count == 1)
            {
                blinky.set_direction(board, pacman);
                clyde.set_direction();
                if (board.powerup > 0)
                {
                    blinky.reverse_direction();
                    clyde.reverse_direction();
                    board.powerup--;
                }
            }
            timer_count = (timer_count + 1) % 3;

            pacman.move(board);
            blinky.move(board);
            clyde.move(board);
        }
        /* Checks if maximum score is achived or pacman hits any of ghosts. Then either reset the ghost or end game.*/
        public bool EndGame(Canvas GameWindow)
        {
            Entities.Entity entity = board.CollisionCheck(pacman, blinky, clyde, board, GameWindow);
            if (board.currentScore == board.maxScore)
            { return true; }
            if (entity != null)
            {
                if (board.powerup > 0)
                {
                    entity.Reset(4, 4);
                    return false;
                }
                else { return true; }
            }
            return false;
        }
        public int getCurrentScore()
        { return board.currentScore; }
        public void PacmanDirection(KeyEventArgs e)
        {
            pacman.set_direction(e);
        }
    }
}
