using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Matching_Game
{
    public partial class MatchingGame : Form
    {
        // variables
        private readonly PrivateFontCollection pfc = new PrivateFontCollection();
        private readonly Random random = new Random();
        private List<string> icons;

        private Label firstClicked = null;
        private Label secondClicked = null;

        private int timeLeft;

        // ctor
        public MatchingGame()
        {
            InitializeComponent();

            LoadCustomFont();
        }

        // load of custom atari looking font
        private void LoadCustomFont()
        {
            pfc.AddFontFile(@"..\..\Resources\AtariClassic.ttf");
            timeLabel.Font = new Font(pfc.Families[0], timeLabel.Font.Size);
            quitBtn.Font = new Font(pfc.Families[0], quitBtn.Font.Size);
        }

        // assign a random icon to a random card
        private void AssignIconsToSquares()
        {
            foreach (Control control in matchingTableLayoutPanel.Controls)
            {
                if (control is Label iconLabel)
                {
                    int randomNumber = random.Next(icons.Count);
                    iconLabel.Text = icons[randomNumber];
                    iconLabel.ForeColor = iconLabel.BackColor;
                    icons.RemoveAt(randomNumber);
                }
            }
        }

        // game logic
        private void label_Click(object sender, EventArgs e)
        {
            if (choiceTimer.Enabled == true)
                return;

            if (secondClicked != null)
                return;

            if (sender is Label clickedLabel)
            {
                if (clickedLabel.ForeColor == Color.White)
                    return;

                if (firstClicked == null)
                {
                    firstClicked = clickedLabel;
                    firstClicked.ForeColor = Color.White;
                    return;
                }

                secondClicked = clickedLabel;
                secondClicked.ForeColor = Color.White;

                CheckForWinner();

                if (firstClicked.Text == secondClicked.Text)
                {
                    firstClicked = null;
                    secondClicked = null;
                    return;
                }

                choiceTimer.Start();
            }
        }
        private void choiceTimer_Tick(object sender, EventArgs e)
        {
            choiceTimer.Stop();

            firstClicked.ForeColor = firstClicked.BackColor;
            secondClicked.ForeColor = secondClicked.BackColor;

            firstClicked = null;
            secondClicked = null;
        }

        // lose or win check
        private async void CheckForWinner()
        {
            foreach (Control control in matchingTableLayoutPanel.Controls)
            {
                if (control is Label iconLabel)
                {
                    if (iconLabel.ForeColor == iconLabel.BackColor)
                        return;
                }
            }

            countdownTimer.Stop();
            timeLabel.Text = "You won!";
            await Task.Delay(750);
            //playBtn.Visible = true;
            quitBtn.Visible = true;

            //foreach (Control control in matchingTableLayoutPanel.Controls)
            //if (control is Label iconLabel)
            //iconLabel.Text = "";

            matchingTableLayoutPanel.Enabled = false;
            gameOverPanel.BringToFront();
        }
        private void countdownTimer_Tick(object sender, EventArgs e)
        {
            if (timeLeft > 0)
            {
                timeLeft -= 1;
                timeLabel.Text = $"{timeLeft} seconds";
            }
            else
            {
                countdownTimer.Stop();
                timeLabel.Text = "Time's up!";
                //playBtn.Visible = true;
                quitBtn.Visible = true;

                //foreach (Control control in matchingTableLayoutPanel.Controls)
                //if (control is Label iconLabel)
                //iconLabel.Text = "";

                matchingTableLayoutPanel.Enabled = false;
                gameOverPanel.BringToFront();
                gameOverPanel.BackColor = Color.FromArgb(125, Color.Red);
                // must do else cant see opacity change of gameoverPanel
                matchingTablePanel.BackgroundImage = Properties.Resources.matchingTable;
                matchingTablePanel.BackgroundImageLayout = ImageLayout.Stretch;
            }
        }

        // start and quit
        private void startGame(object sender, EventArgs e)
        {
            icons = new List<string>()
            {
                "!", "!", "V", "V", ",", ",", "~", "~",
                "b", "b", "v", "v", "w", "w", "z", "z"
            };
            AssignIconsToSquares();

            matchingTableLayoutPanel.Enabled = true;
            matchingTablePanel.BackgroundImage = null;
            gameOverPanel.SendToBack();

            timeLeft = 5;
            timeLabel.Text = "50 seconds";
            countdownTimer.Start();

            playBtn.Visible = false;
            quitBtn.Visible = false;
        }
        private void exitGame(object sender, EventArgs e)
        {
            Close();
        }
    }
}