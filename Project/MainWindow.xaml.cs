﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Xml;
using Project.Assets.DataClasses;
using System.IO;
using Project.Assets.ControlClasses;
using Project.Assets.UserControls;
using System.Windows.Threading;

namespace Project
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
            Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            music.PlayMusic();
            UpdateAllMaxScores();
            GameControls gameControls = new GameControls(GameScreen, charapter1);
            gameControls.StartGame();
        }

        private void InitializeGame()
        {
            gameSpace = new Space(1, 1980, 1100, false, false, new List<Enemy>());
            DataContext = gameSpace;
        }

        // Control classes
        Player charapter1 = new Player(1, "Character1", 100.0, 1.0, 1.0, 1.0, new Vector(800, 400), 0, new List<Item>(), 40.0f);
        Space gameSpace = new Space();
        SavesControls save = new SavesControls();
        SoundControls music = new SoundControls();
        SoundControls sound = new SoundControls();
        //Data Storage
        protected string currentSave;
        protected int currentDifficulty = -1;
        protected int currentCharacter = -1;

        #region ButtonFunctions
        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            sound.PlaySound("button-click");
            MainMenu.Visibility = Visibility.Hidden;
            GameSaves.Visibility = Visibility.Visible;
            UpdateAllMaxScores();
        }
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            sound.PlaySound("button-click");
            MainMenu.Visibility = Visibility.Hidden;
            SettingsMenu.Visibility = Visibility.Visible;
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            sound.PlaySound("button-click");
            Application.Current.Shutdown();
        }
        // Settings menu buttons
        private void Music_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Properties.Settings.Default.musicVolume = (int)Music_Slider.Value;
        }
        private void Sound_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Properties.Settings.Default.soundVolume = (int)Sound_Slider.Value;
        }
        private void Return_to_MainMenu_Click(object sender, RoutedEventArgs e)
        {
            sound.PlaySound("button-click");
            SettingsMenu.Visibility = Visibility.Hidden;
            MainMenu.Visibility = Visibility.Visible;
        }
        private void Change_to_English_Click(object sender, RoutedEventArgs e)
        {
            sound.PlaySound("button-click");
            Properties.Settings.Default.languageCode = "en";
        }
        private void Change_to_Ukrainian_Click(object sender, RoutedEventArgs e)
        {
            sound.PlaySound("button-click");
            Properties.Settings.Default.languageCode = "uk";
        }
        private void Dont_SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            sound.PlaySound("button-click");
            SaveSettings.Visibility = Visibility.Hidden;
            SettingsMenu.Visibility = Visibility.Visible;
        }
        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            sound.PlaySound("button-click");
            Properties.Settings.Default.Save();
            Application.Current.Shutdown();
        }
        private void Save_Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsMenu.Visibility = Visibility.Hidden;
            sound.PlaySound("button-click");
            SaveSettings.Visibility = Visibility.Visible;
        }
        //Game save buttons
        private void Load_Save1_Click(object sender, RoutedEventArgs e)
        {
            sound.PlaySound("button-click");
            if (save.CheckSaveExistence("save1.txt") == true)
            {
                GameSaves.Visibility = Visibility.Hidden;
                save.ReadSaveData("save1.txt");
                CharactersSelectMenu.Visibility = Visibility.Visible;
                currentSave = "save1.txt";
            }
            else
            {
                save.CreateSave("save1.txt");
                MessageBox.Show("New save created.");
                GameSaves.Visibility = Visibility.Hidden;
                CharactersSelectMenu.Visibility = Visibility.Visible;
            }
            UpdateAllMaxScores();
        }
        private void Delete_Save1_Click(object sender, RoutedEventArgs e)
        {
            sound.PlaySound("button-click");
            if (save.CheckSaveExistence("save1.txt") == true)
            {
                save.DeleteSave("save1.txt");
                MessageBox.Show("Save deleted successfully.");
            }
            else
            {
                MessageBox.Show("Save not found.");
            }
            UpdateAllMaxScores();
        }
        private void Load_Save2_Click(object sender, RoutedEventArgs e)
        {
            sound.PlaySound("button-click");
            if (save.CheckSaveExistence("save2.txt") == true)
            {
                GameSaves.Visibility = Visibility.Hidden;
                save.ReadSaveData("save2.txt");
                CharactersSelectMenu.Visibility = Visibility.Visible;
                currentSave = "save2.txt";
            }
            else
            {
                save.CreateSave("save2.txt");
                MessageBox.Show("New save created.");
                GameSaves.Visibility = Visibility.Hidden;
                CharactersSelectMenu.Visibility = Visibility.Visible;
            }
            UpdateAllMaxScores();
        }
        private void Delete_Save2_Click(object sender, RoutedEventArgs e)
        {
            sound.PlaySound("button-click");
            if (save.CheckSaveExistence("save2.txt") == true)
            {
                save.DeleteSave("save2.txt");
                MessageBox.Show("Save deleted successfully.");
            }
            else
            {
                MessageBox.Show("Save not found.");
            }
            UpdateAllMaxScores();
        }
        private void Load_Save3_Click(object sender, RoutedEventArgs e)
        {
            sound.PlaySound("button-click");
            if (save.CheckSaveExistence("save3.txt") == true)
            {
                GameSaves.Visibility = Visibility.Hidden;
                save.ReadSaveData("save3.txt");
                CharactersSelectMenu.Visibility = Visibility.Visible;
                currentSave = "save3.txt";
            }
            else
            {
                save.CreateSave("save3.txt");
                MessageBox.Show("New save created.");
                GameSaves.Visibility = Visibility.Hidden;
                CharactersSelectMenu.Visibility = Visibility.Visible;
            }
            UpdateAllMaxScores();
        }
        private void Delete_Save3_Click(object sender, RoutedEventArgs e)
        {
            sound.PlaySound("button-click");
            if (save.CheckSaveExistence("save3.txt") == true)
            {
                save.DeleteSave("save3.txt");
                MessageBox.Show("Save deleted successfully.");
            }
            else
            {
                MessageBox.Show("Save not found.");
            }
            UpdateAllMaxScores();
        }

        //Game start menu buttons
        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            sound.PlaySound("button-click");
            if (currentCharacter != -1 && currentDifficulty != -1)
            {
                GameSaves.Visibility = Visibility.Hidden;
                CharactersSelectMenu.Visibility = Visibility.Hidden;
                Menu.Visibility = Visibility.Hidden;
                GameScreen.Visibility = Visibility.Visible;
                GameScreen.Focus();
            }
            else
            {
                MessageBox.Show("Please select character and difficulty.");
            }
        }
        private void Choose_Character1_Click(object sender, RoutedEventArgs e)
        {
            sound.PlaySound("button-click");
            btnCharacter1.Background = Brushes.White;
            btnCharacter1.Foreground = Brushes.Black;
            currentCharacter = 1;
        }
        private void Easy_Difficulty_Selected_Click(object sender, RoutedEventArgs e)
        {
            sound.PlaySound("button-click");
            btnEasy.Background = Brushes.White;
            btnEasy.Foreground = Brushes.Black;
            btnNormal.Background = Brushes.Black;
            btnNormal.Foreground = Brushes.White;
            btnHard.Background = Brushes.Black;
            btnHard.Foreground = Brushes.White;
            currentDifficulty = 0;
        }
        private void Normal_Difficulty_Selected_Click(object sender, RoutedEventArgs e)
        {
            sound.PlaySound("button-click");
            btnEasy.Background = Brushes.Black;
            btnEasy.Foreground = Brushes.White;
            btnNormal.Background = Brushes.White;
            btnNormal.Foreground = Brushes.Black;
            btnHard.Background = Brushes.Black;
            btnHard.Foreground = Brushes.White;
            currentDifficulty = 1;
        }
        private void Hard_Difficulty_Selected_Click(object sender, RoutedEventArgs e)
        {
            sound.PlaySound("button-click");
            btnEasy.Background = Brushes.Black;
            btnEasy.Foreground = Brushes.White;
            btnNormal.Background = Brushes.Black;
            btnNormal.Foreground = Brushes.White;
            btnHard.Background = Brushes.White;
            btnHard.Foreground = Brushes.Black;
            currentDifficulty = 2;
        }
        #endregion

        //Functional methods
        private void UpdateMaxScore(TextBlock maxScoreTextBlock, string saveFileName, Run textBlockMaxScore)
        {
            if (save.CheckSaveExistence(saveFileName) == false)
            {
                maxScoreTextBlock.Visibility = Visibility.Hidden;
            }
            else
            {
                maxScoreTextBlock.Visibility = Visibility.Visible;
                string saveData = save.ReadSaveData(saveFileName);
                if (saveData != null)
                {
                    string[] saveParts = saveData.Split(';');
                    foreach (string part in saveParts)
                    {
                        string[] keyValue = part.Split(':');
                        if (keyValue.Length == 2)
                        {
                            string key = keyValue[0].Trim();
                            string value = keyValue[1].Trim();
                            if (key == "maxScore")
                            {
                                textBlockMaxScore.Text = value;
                                break;
                            }
                        }
                    }
                }
            }
        }
        private void UpdateAllMaxScores()
        {
            UpdateMaxScore(maxScore1, "save1.txt", TextBlockMaxScore1);
            UpdateMaxScore(maxScore2, "save2.txt", TextBlockMaxScore2);
            UpdateMaxScore(maxScore3, "save3.txt", TextBlockMaxScore3);
        }
    }
}