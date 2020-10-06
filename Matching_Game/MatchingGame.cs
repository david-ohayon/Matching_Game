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
            saveSettings.Font = new Font(pfc.Families[0], saveSettings.Font.Size);
            timeSettings.Font = new Font(pfc.Families[0], timeSettings.Font.Size);
            timeleftInfo.Font = new Font(pfc.Families[0], timeleftInfo.Font.Size);
        }

        // color and visibilty of gameover panel
        private void GameOverPanelColor(Color color)
        {
            gameoverPanel.BackColor = Color.FromArgb(80, color);
            // must do else cant see opacity change of gameoverPanel
            matchingTablePanel.BackgroundImage = Properties.Resources.matchingTable;
            matchingTablePanel.BackgroundImageLayout = ImageLayout.Stretch;
        }
        private void GameOverPanelVisibility()
        {
            gameoverPanel.BringToFront();
            matchingTableLayoutPanel.Enabled = false;
            timeSettings.Visible = false;
            timeleftInfo.Visible = false;
            timeLabel.Visible = false;
            saveSettings.Visible = false;

            settingsBtn.Visible = true;
            quitBtn.Visible = true;
            replayBtn.Visible = true;
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
            // hasn't won yet
            foreach (Control control in matchingTableLayoutPanel.Controls)
            {
                if (control is Label iconLabel)
                {
                    if (iconLabel.ForeColor == iconLabel.BackColor)
                        return;
                }
            }

            // won
            countdownTimer.Stop();
            timeLabel.Text = "You won!";
            await Task.Delay(700);

            GameOverPanelColor(Color.FromArgb(118, 184, 82));
            GameOverPanelVisibility();
        }
        private void countdownTimer_Tick(object sender, EventArgs e)
        {
            // hasn't lost yet
            if (timeLeft > 1)
            {
                timeLeft -= 1;
                timeLabel.Text = $"{timeLeft} seconds";
            }
            // lost
            else
            {
                countdownTimer.Stop();
                timeLabel.Text = "Time's up!";

                GameOverPanelColor(Color.FromArgb(255, 75, 43));
                GameOverPanelVisibility();
            }
        }

        // start
        private void startGame(object sender, EventArgs e)
        {
            timeLeft = (int)timeSettings.Value;

            icons = new List<string>()
            {
                "!", "!", "V", "V", ",", ",", "~", "~",
                "b", "b", "v", "v", "w", "w", "z", "z"
            };
            AssignIconsToSquares();

            matchingTableLayoutPanel.Enabled = true;
            matchingTablePanel.BackgroundImage = null;
            gameoverPanel.SendToBack();

            countdownTimer.Start();

            playBtn.Visible = false;
            settingsBtn.Visible = false;
            quitBtn.Visible = false;
        }
        //settings
        private void settingsBtn_Click(object sender, EventArgs e)
        {
            gameoverPanel.BringToFront();
            quitBtn.Visible = true;
            timeSettings.Visible = true;
            timeleftInfo.Visible = true;
            replayBtn.Visible = false;
            GameOverPanelColor(Color.FromArgb(67, 67, 67));
        }
        private void timeSettings_ValueChanged(object sender, EventArgs e)
        {
            timeLeft = (int)timeSettings.Value;
            timeLabel.Text = $"{timeLeft} seconds";
        }
        private void timeSettings_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar < 48 || e.KeyChar > 57)
                if (e.KeyChar != 8)
                    e.Handled = true;
        }
        private void saveSettings_Click(object sender, EventArgs e)
        {
            quitBtn.Visible = false;
            gameoverPanel.SendToBack();
        }
        // quit
        private void quitBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}