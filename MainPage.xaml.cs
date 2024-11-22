using System;
using Microsoft.Maui.Controls;
using Plugin.Maui.Audio;

namespace TicTacToeMaui
{
    public partial class MainPage : ContentPage
    {
        private IAudioPlayer _player;
        private string currentPlayer;
        private string[,] board;
        private bool gameEnded;

        public MainPage()
        {
            InitializeComponent();
            StartNewGame();
        }

        private void StartNewGame()
        {
            currentPlayer = "🍏";
            board = new string[3, 3];
            gameEnded = false;
            statusLabel.Text = "Player 🍏's turn";
            ResetButtons();
            AnimateTitle();
        }

        private async void OnButtonClick(object sender, EventArgs e)
        {
            if (gameEnded) return;
            // Load the audio file from the app package
            if (_player == null)
            {
                var audioManager = AudioManager.Current;
                var audioFile = await FileSystem.OpenAppPackageFileAsync("appleslice.mp3");
                _player = audioManager.CreatePlayer(audioFile);
            }

            // Play the sound
            _player.Play();
        }

        private void ResetButtons()
        {
            btn00.Text = btn01.Text = btn02.Text = "";
            btn10.Text = btn11.Text = btn12.Text = "";
            btn20.Text = btn21.Text = btn22.Text = "";
            btn00.IsEnabled = btn01.IsEnabled = btn02.IsEnabled = true;
            btn10.IsEnabled = btn11.IsEnabled = btn12.IsEnabled = true;
            btn20.IsEnabled = btn21.IsEnabled = btn22.IsEnabled = true;
        }

        private void OnButtonClicked(object sender, EventArgs e)
        {
            if (gameEnded) return;

            var button = (Button)sender;
            int row = Grid.GetRow(button);
            int col = Grid.GetColumn(button);

            if (string.IsNullOrEmpty(board[row, col]))
            {
                board[row, col] = currentPlayer;
                button.Text = currentPlayer;

                if (CheckForWinner())
                {
                    statusLabel.Text = $"Player {currentPlayer} wins!";
                    gameEnded = true;
                    DisableAllButtons();
                }
                else if (CheckForDraw())
                {
                    statusLabel.Text = "It's a draw!";
                    gameEnded = true;
                }
                else
                {
                    SwitchPlayer();
                }
            }
        }

        private void SwitchPlayer()
        {
            currentPlayer = currentPlayer == "🍏" ? "🍎" : "🍏";
            statusLabel.Text = $"Player {currentPlayer}'s turn";
        }

        async void AnimateTitle()
        {
            // Example: scale, and rotate the title
            await Task.WhenAll(
            AppleTitle.ScaleTo(.5, 1000), // Scale down
            AppleTitle.RotateTo(360, 1000) // Rotate 360 degrees
            );

            // Reverse animation
            await Task.WhenAll(
            AppleTitle.ScaleTo(1, 1000), // Scale back to normal
            AppleTitle.RotateTo(0, 1000) // Reset rotation
            );
        }

        private bool CheckForWinner()
        {
            // Check rows, columns, and diagonals for a win
            for (int i = 0; i < 3; i++)
            {
                if (!string.IsNullOrEmpty(board[i, 0]) && board[i, 0] == board[i, 1] && board[i, 1] == board[i, 2])
                    return true;
                if (!string.IsNullOrEmpty(board[0, i]) && board[0, i] == board[1, i] && board[1, i] == board[2, i])
                    return true;
            }
            if (!string.IsNullOrEmpty(board[0, 0]) && board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2])
                return true;
            if (!string.IsNullOrEmpty(board[0, 2]) && board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0])
                return true;

            return false;
        }

        private bool CheckForDraw()
        {
            foreach (var cell in board)
            {
                if (string.IsNullOrEmpty(cell))
                    return false;
            }
            return true;
        }

        private void DisableAllButtons()
        {
            btn00.IsEnabled = btn01.IsEnabled = btn02.IsEnabled = false;
            btn10.IsEnabled = btn11.IsEnabled = btn12.IsEnabled = false;
            btn20.IsEnabled = btn21.IsEnabled = btn22.IsEnabled = false;
        }

        private void OnRestartClicked(object sender, EventArgs e)
        {
            StartNewGame();
        }
    }
}