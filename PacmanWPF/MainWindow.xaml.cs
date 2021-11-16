using System;
using System.Windows;
using System.Windows.Input;

namespace PackmanWPF
{
    
    public partial class MainWindow : Window
    {
        private System.Windows.Threading.DispatcherTimer gameTickTimer = new System.Windows.Threading.DispatcherTimer();
        Game game;
        public MainWindow()
        {
            InitializeComponent();
            game = new Game(GameWindow);
            this.Title = "Score: " + game.getCurrentScore();
            gameTickTimer.Tick += GameTickTimer_Tick;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            game.Draw(GameWindow);
            gameTickTimer.Interval = TimeSpan.FromMilliseconds(300);
            gameTickTimer.IsEnabled = true;
        }

        private void GameTickTimer_Tick(object sender, EventArgs e)
        {
            game.DoMove();
            game.Draw(GameWindow);
            this.Title = "Score: " + game.getCurrentScore();
            if (game.EndGame(GameWindow))
            {
                gameTickTimer.IsEnabled = false;
                this.Title = "Score: " + game.getCurrentScore();
                GameOver end = new GameOver();
                end.Show();
            }
        }



        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            game.PacmanDirection(e);
        }
    }
}
