﻿using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace c_project_mastermind_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int randomIndex;
        private string randomColor;
        int attempts;
        int maxAttemps;
        int score = 0;
        private string userName;
        string[] colors = { "Red", "Yellow", "Orange", "White", "Green", "Blue" };
        string[] highScores = new string[15];
        List<string> secretCode = new List<string>();
        private DispatcherTimer timer = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            Title = $"MasterMind - poging {attempts}";
            string randomColorOne = GenerateRandomColor();
            string randomColorTwo = GenerateRandomColor();
            string randomColorThree = GenerateRandomColor();
            string randomColorFour = GenerateRandomColor();

            secretCode.Add(randomColorOne);
            secretCode.Add(randomColorTwo);
            secretCode.Add(randomColorThree);
            secretCode.Add(randomColorFour);

            secretCodeTextBox.Text = string.Join(", ", secretCode);

            foreach (string color in colors)
            {
                comboBoxOne.Items.Add(color);
                comboBoxTwo.Items.Add(color);
                comboBoxThree.Items.Add(color);
                comboBoxFour.Items.Add(color);
            }
            StartCountdown();
        }
        public string GenerateRandomColor()
        {
            Random rnd = new Random();
            randomIndex = rnd.Next(colors.Length);
            randomColor = colors[randomIndex];
            return randomColor;
        }
        private void ComboBox_SelectionChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            if (comboBox != null && comboBox.SelectedItem != null)
            {
                string selectedColor = comboBox.SelectedItem.ToString();
                SolidColorBrush brush = GetBrushFromColorName(selectedColor);

                if (comboBox == comboBoxOne)
                {
                    labelOne.Content = selectedColor;
                    labelOne.Background = brush;
                }
                else if (comboBox == comboBoxTwo)
                {
                    labelTwo.Content = selectedColor;
                    labelTwo.Background = brush;
                }
                else if (comboBox == comboBoxThree)
                {
                    labelThree.Content = selectedColor;
                    labelThree.Background = brush;
                }
                else if (comboBox == comboBoxFour)
                {
                    labelFour.Content = selectedColor;
                    labelFour.Background = brush;
                }
            }
        }
        private SolidColorBrush GetBrushFromColorName(string colorName)
        {
            try
            {
                return (SolidColorBrush)new BrushConverter().ConvertFromString(colorName);
            }
            catch
            {
                return Brushes.Transparent;
            }
        }
        private void CheckCodeButton_Click(object sender, RoutedEventArgs e)
        {
            StopCountdown(CheckCodeButton, e);
            List<string> userColors = new List<string>();
            userColors.Add(comboBoxOne.SelectedItem?.ToString());
            userColors.Add(comboBoxTwo.SelectedItem?.ToString());
            userColors.Add(comboBoxThree.SelectedItem?.ToString());
            userColors.Add(comboBoxFour.SelectedItem?.ToString());
            if (attempts <= maxAttemps)
            {
                StringBuilder feedback = new StringBuilder();
                bool codeCracked = true;

                for (int i = 0; i < userColors.Count; i++)
                {
                    if (userColors[i] == secretCode[i])
                    {
                        feedback.Append($"{userColors[i]} (R) ");
                        SetLabelBorder(i, Colors.DarkRed);
                    }
                    else if (secretCode.Contains(userColors[i]))
                    {
                        feedback.Append($"{userColors[i]} (W) ");
                        SetLabelBorder(i, Colors.Wheat);
                        score += 1;
                        codeCracked = false;
                    }
                    else
                    {
                        feedback.Append($"{userColors[i]} (-) ");
                        score += 2;
                        codeCracked = false;
                    }
                }

                scoreLabel.Content = $"Score: {score}";
                string attemptFeedback = $"poging {attempts}: {feedback}";
                attemptsListBox.Items.Add(attemptFeedback);

                if (codeCracked)
                {
                    MessageBox.Show("Gefeliciteerd! Je hebt de code gekraakt!", "je hebt gewonnen!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    highScores.Append($"{userName} - {attempts} pogingen - {score}/100");
                    ResetGame();
                }
            }
            else
            {
                MessageBox.Show($"Game over! Je hebt de code niet gekraakt binnen 10 pogingen. De code was: {string.Join(", ", secretCode)}", "Game over!", MessageBoxButton.OK, MessageBoxImage.Error);
                ResetGame();
            }
        }
        private void ResetGame()
        {
            attempts = 0;
            score = 0;
            scoreLabel.Content = "Score: 0";

            attemptsListBox.Items.Clear();

            secretCode.Clear();
            string randomColorOne = GenerateRandomColor();
            string randomColorTwo = GenerateRandomColor();
            string randomColorThree = GenerateRandomColor();
            string randomColorFour = GenerateRandomColor();

            secretCode.Add(randomColorOne);
            secretCode.Add(randomColorTwo);
            secretCode.Add(randomColorThree);
            secretCode.Add(randomColorFour);

            comboBoxOne.SelectedItem = null;
            comboBoxTwo.SelectedItem = null;
            comboBoxThree.SelectedItem = null;
            comboBoxFour.SelectedItem = null;

            labelOne.Background = Brushes.Transparent;
            labelTwo.Background = Brushes.Transparent;
            labelThree.Background = Brushes.Transparent;
            labelFour.Background = Brushes.Transparent;

            labelOne.BorderBrush = Brushes.Transparent;
            labelTwo.BorderBrush = Brushes.Transparent;
            labelThree.BorderBrush = Brushes.Transparent;
            labelFour.BorderBrush = Brushes.Transparent;

            labelOne.BorderThickness = new Thickness(1);
            labelTwo.BorderThickness = new Thickness(1);
            labelThree.BorderThickness = new Thickness(1);
            labelFour.BorderThickness = new Thickness(1);

            labelOne.Content = "";
            labelTwo.Content = "";
            labelThree.Content = "";
            labelFour.Content = "";

            Title = $"MasterMind - poging {attempts}/{maxAttemps}";
            StartCountdown();
        }
        private void SetLabelBorder(int index, Color borderColor)
        {
            SolidColorBrush brush = new SolidColorBrush(borderColor);
            switch (index)
            {
                case 0:
                    labelOne.BorderBrush = brush;
                    labelOne.BorderThickness = new Thickness(3);
                    break;
                case 1:
                    labelTwo.BorderBrush = brush;
                    labelTwo.BorderThickness = new Thickness(3);
                    break;
                case 2:
                    labelThree.BorderBrush = brush;
                    labelThree.BorderThickness = new Thickness(3);
                    break;
                case 3:
                    labelFour.BorderBrush = brush;
                    labelFour.BorderThickness = new Thickness(3);
                    break;
            }
        }
        // de ToggleDebug maakt de secretcodetextbox zichtbaar bij het indrukken van de control- en f12-toetsen
        // met deze code gebeurt dit echter niet wanneer ik deze toetsen gebruik (ik vermoed mogelijks door de 'fn'-toets maar vind hier momenteel geen oplossing voor.
        private void ToggleDebug(object sender, KeyEventArgs e)
        {
            if( e.Key == Key.F12 && Keyboard.Modifiers == ModifierKeys.Control)
            {
                secretCodeTextBox.Visibility = Visibility.Visible;
            }
        }
        // De StartCountdown activeert stopCountdown na het verstrijken van 10 seconden
        // Eerst wordt gedefinieerd welke method gebruikt gaat worden en na welke interval, daarna wordt de timer geactiveerd
        private void StartCountdown()
        {
            timer.Tick += StopCountdown;
            timer.Interval = new TimeSpan(0, 0, 10);
            timer.Start();
        }
        // De StopCountdown stopt de lopende timer, verhoogt de pogingen met 1 en updatetet dit ook in de titel, vervolgens start het de volgende timer op.
        // Aangezien deze functie wordt opgeroepen wanneer er op de checkbutton wordt geklikt of wanneer er 10 seconden zijn verstreken is dit de enige plaats waar het aantal pogingen en de titel worden aangepast na hun definiëring bij het opstarten van het window.
        private void StopCountdown(object sender, EventArgs e)
        {
            timer.Stop();
            attempts++;
            Title = $"MasterMind - poging {attempts}/{maxAttemps}";
            timer.Start();
        }
        public void WindowClosed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var result = MessageBox.Show("Weet je zeker dat je het spel wilt afsluiten? Je verliest je voortgang.",
                                         "Bevestigen", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        private void mnuClose_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Weet je zeker dat je het spel wilt afsluiten? Je verliest je voortgang.",
                                         "Bevestigen", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                this.Close();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }
        private void mnuHighScore_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"{highScores}");
        }
        private int mnuAmountOfAttemps_Click(object sender, RoutedEventArgs e)
        {
            string userMaxAttempts = Interaction.InputBox("geef het maximaal aantal pogingen op tussen 3 en 20", "Maximale pogingen", "10");
            if (string.IsNullOrEmpty(userMaxAttempts) )
            {
                bool result = int.TryParse(userMaxAttempts, out int maxattempts);
                if (result == true && maxAttemps >= 3 && maxAttemps <= 20)
                {
                    return maxAttemps;
                }
                else
                {
                    MessageBox.Show("foutieve invoer, voer een correcte waarde in");
                    return 0;
                }
            }
            else
            {
                MessageBox.Show("foutieve invoer, voer een correcte waarde in");
                return 0;
            }
        }
    }
}