﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Matching_Game
{
    public partial class MatchingGame : Form
    {
        private readonly Random random = new Random();
        private List<string> icons;

        private Label firstClicked = null;
        private Label secondClicked = null;

        private int timeLeft;

        public MatchingGame()
        {
            InitializeComponent();

            LoadCustomFont();
        }

        private void LoadCustomFont()
        {
            //Create your private font collection object.
            PrivateFontCollection pfc = new PrivateFontCollection();
            //Select your font from the resources.
            int fontLength = Properties.Resources.Atari_Classic.Length;
            // create a buffer to read in to
            byte[] fontdata = Properties.Resources.Atari_Classic;
            // create an unsafe memory block for the font data
            IntPtr data = Marshal.AllocCoTaskMem(fontLength);
            // copy the bytes to the unsafe memory block
            Marshal.Copy(fontdata, 0, data, fontLength);
            // pass the font to the font collection
            pfc.AddMemoryFont(data, fontLength);

            timeLabel.Font = new Font(pfc.Families[0], timeLabel.Font.Size);
            playBtn.Font = new Font(pfc.Families[0], playBtn.Font.Size);
            quitBtn.Font = new Font(pfc.Families[0], quitBtn.Font.Size);
        }

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

        private void CheckForWinner()
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
            playBtn.Visible = true;
            quitBtn.Visible = true;

            foreach (Control control in matchingTableLayoutPanel.Controls)
                if (control is Label iconLabel)
                    iconLabel.Text = "c";

            matchingTableLayoutPanel.Enabled = false;
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
                playBtn.Visible = true;
                quitBtn.Visible = true;

                foreach (Control control in matchingTableLayoutPanel.Controls)
                    if (control is Label iconLabel)
                        iconLabel.Text = "c";

                matchingTableLayoutPanel.Enabled = false;
            }
        }

        private void startGame(object sender, EventArgs e)
        {
            icons = new List<string>()
            {
                "!", "!", "V", "V", ",", ",", "~", "~",
                "b", "b", "v", "v", "w", "w", "z", "z"
            };
            AssignIconsToSquares();

            matchingTableLayoutPanel.Enabled = true;

            timeLeft = 50;
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