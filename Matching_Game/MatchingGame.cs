﻿using System;
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

        private int timeLeft; // milliseconds
        private bool start;
        private bool lost;
        private bool won;

        // ctor
        public MatchingGame()
        {
            InitializeComponent();

            Setup();
        }
        // setup
        private void Setup()
        {
            LoadCustomFont();
            start = true;
            GameOverPanelColor(Color.FromArgb(67, 67, 67));
            gameoverPanel.BringToFront();
            saveSettingsBtn.Visible = false;
            infoLabel.Text = "Matching Game created by\nDavid Ohayon && Yishai Kehalani";
        }

        // load of custom atari looking font
        private void LoadCustomFont()
        {
            pfc.AddFontFile(@"..\..\Resources\AtariClassic.ttf");
            timeLabel.Font = new Font(pfc.Families[0], timeLabel.Font.Size);
            saveSettingsBtn.Font = new Font(pfc.Families[0], saveSettingsBtn.Font.Size);
            timeInpSettings.Font = new Font(pfc.Families[0], timeInpSettings.Font.Size);
            timeleftInfoLbl.Font = new Font(pfc.Families[0], timeleftInfoLbl.Font.Size);
            infoLabel.Font = new Font(pfc.Families[0], infoLabel.Font.Size);
        }

        // color and visibilty of gameover panel
        private void GameOverPanelColor(Color color)
        {
            gameoverPanel.BackColor = Color.FromArgb(80, color);
            // must do else cant see opacity change of gameoverPanel
            matchingTablePanel.BackgroundImage = Properties.Resources.matchingTable;
            matchingTablePanel.BackgroundImageLayout = ImageLayout.Stretch;
        }
        private void GameOverPanelVisibilityLW()
        {
            gameoverPanel.BringToFront();
            timeInpSettings.Visible = false;
            timeleftInfoLbl.Visible = false;
            saveSettingsBtn.Visible = false;

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
            won = true;
            countdownTimer.Stop();
            timeLabel.Text = "You won!";
            await Task.Delay(700);

            GameOverPanelVisibilityLW();
            GameOverPanelColor(Color.FromArgb(118, 184, 82));
        }
        private void countdownTimer_Tick(object sender, EventArgs e)
        {
            // hasn't lost yet
            if (timeLeft > 800)
            {
                timeLeft -= 100;
                timeLabel.Text = $"{timeLeft / 1000} seconds";
            }
            // lost
            else
            {
                lost = true;
                countdownTimer.Stop();
                timeLabel.Text = "Time's up!";

                GameOverPanelVisibilityLW();
                GameOverPanelColor(Color.FromArgb(255, 75, 43));
            }
        }

        // start
        private async void startGame(object sender, EventArgs e)
        {
            await Task.Delay(250);
            infoLabel.Visible = false;

            start = false;
            lost = false;
            won = false;

            timeLeft = Convert.ToInt32(timeInpSettings.Value) * 1000 + 800;
            timeLabel.Text = $"{timeLeft / 1000} seconds";

            icons = new List<string>()
            {
                "!", "!", "V", "V", ",", ",", "~", "~",
                "b", "b", "v", "v", "w", "w", "z", "z"
            };
            AssignIconsToSquares();

            matchingTablePanel.BackgroundImage = null;
            gameoverPanel.SendToBack();

            countdownTimer.Start();

            settingsBtn.Visible = false;
            quitBtn.Visible = false;
            replayBtn.Visible = true;
        }
        //settings
        private void settingsBtn_Click(object sender, EventArgs e)
        {
            gameoverPanel.BringToFront();
            infoLabel.Visible = false;
            quitBtn.Visible = true;
            timeInpSettings.Visible = true;
            timeleftInfoLbl.Visible = true;
            saveSettingsBtn.Visible = true;
            replayBtn.Visible = false;
            playBtn.Visible = false;
            GameOverPanelColor(Color.FromArgb(67, 67, 67));
        }
        private void timeInpSettings_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar < 48 || e.KeyChar > 57)
                if (e.KeyChar != 8)
                    e.Handled = true;
        }
        private void saveSettingsBtn_Click(object sender, EventArgs e)
        {
            timeLeft = Convert.ToInt32(timeInpSettings.Value) * 1000 + 800;
            timeLabel.Text = $"{timeLeft / 1000} seconds";

            if (start)
            {
                GameOverPanelColor(Color.FromArgb(67, 67, 67));
                infoLabel.Visible = true;
                timeInpSettings.Visible = false;
                timeleftInfoLbl.Visible = false;
                saveSettingsBtn.Visible = false;
                playBtn.Visible = true;
                return;
            }
            if (lost)
            {
                GameOverPanelVisibilityLW();
                GameOverPanelColor(Color.FromArgb(255, 75, 43));
                return;
            }
            if (won)
            {
                GameOverPanelVisibilityLW();
                GameOverPanelColor(Color.FromArgb(118, 184, 82));
                return;
            }

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