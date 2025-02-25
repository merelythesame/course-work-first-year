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
		public Player player;
		public Space gameSpace;
		public SavesControls save;
		public SoundControls music;
		public SoundControls sound;
		public BitmapImage bitmapImage;
		public GameScreen GameScreen;
		public DispatcherTimer UIUpdateTimer;
		public DispatcherTimer TimeTimer;
		public GameControls gameControls;
		public EnemyControls enemyControls;
		public GameState gameState;

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
        }

		private void InitializeGame()
		{
			gameSpace = new Space(1, 1920, 1064);
			gameState = new GameState();
			player = new Player();
			save = new SavesControls();
			music = new SoundControls();
			sound = new SoundControls();
			UIUpdateTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(6) };
			UIUpdateTimer.Tick += UIUpdateTimer_Tick;
			UIUpdateTimer.Start();
			TimeTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
			TimeTimer.Tick += (sender, e) => gameState.CurrentTime++;
			InitializeCharacterButtons();
			InitializeDifficultyButtons();
		}

		// Control classes
		#region ButtonFunctions
		private void HandleButtonClick(Action action)
		{
			sound.PlaySound("button-click");
			action?.Invoke();
		}
        private void AdjustVolume(string settingKey, double value)
		{
			Properties.Settings.Default[settingKey] = (int)value;
			Properties.Settings.Default.Save();
		}
        private void NewGame_Click(object sender, RoutedEventArgs e) => HandleButtonClick(() =>
		{
			MainMenu.Visibility = Visibility.Hidden;
			GameSaves.Visibility = Visibility.Visible;
			UpdateAllMaxScores();
		});

		private void Settings_Click(object sender, RoutedEventArgs e) => HandleButtonClick(() =>
		{
			MainMenu.Visibility = Visibility.Hidden;
			SettingsMenu.Visibility = Visibility.Visible;
		});

		private void Exit_Click(object sender, RoutedEventArgs e) => HandleButtonClick(() => Application.Current.Shutdown());
		// Settings menu buttons
		private void Music_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => AdjustVolume("musicVolume", Music_Slider.Value);
		private void Sound_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => AdjustVolume("soundVolume", Sound_Slider.Value);
		private void Return_to_MainMenu_Click(object sender, RoutedEventArgs e) => HandleButtonClick(() =>
		{
			SettingsMenu.Visibility = Visibility.Hidden;
			MainMenu.Visibility = Visibility.Visible;
		});

		private void ChangeLanguage(string languageCode) => HandleButtonClick(() => Properties.Settings.Default.languageCode = languageCode);
        private void Change_to_English_Click(object sender, RoutedEventArgs e) => ChangeLanguage("en");
		private void Change_to_Ukrainian_Click(object sender, RoutedEventArgs e) => ChangeLanguage("uk");

		private void Dont_SaveChanges_Click(object sender, RoutedEventArgs e) => HandleButtonClick(() =>
		{
			SaveSettings.Visibility = Visibility.Hidden;
			SettingsMenu.Visibility = Visibility.Visible;
		});
		private void SaveChanges_Click(object sender, RoutedEventArgs e) => HandleButtonClick(() =>
		{
			Properties.Settings.Default.Save();
			Application.Current.Shutdown();
		});
		private void Save_Settings_Click(object sender, RoutedEventArgs e) => HandleButtonClick(() =>
		{
			SettingsMenu.Visibility = Visibility.Hidden;
			SaveSettings.Visibility = Visibility.Visible;
		});
		//Game save buttons

		private void Load_Save1_Click(object sender, RoutedEventArgs e) => ProcessLoadSave("save1.txt");
        private void Load_Save2_Click(object sender, RoutedEventArgs e) => ProcessLoadSave("save2.txt");
        private void Load_Save3_Click(object sender, RoutedEventArgs e) => ProcessLoadSave("save3.txt");

		private void ProcessLoadSave(string saveFileName)
		{
			HandleButtonClick(() =>
			{
				if (save.CheckSaveExistence(saveFileName))
				{
					gameState.CurrentSave = saveFileName;
					GameSaves.Visibility = Visibility.Hidden;
					save.ReadSaveData(saveFileName);
					CharactersSelectMenu.Visibility = Visibility.Visible;
				}
				else
				{
					gameState.CurrentSave = saveFileName;
					save.CreateSave(saveFileName);
					MessageBox.Show("New save created.");
					GameSaves.Visibility = Visibility.Hidden;
					CharactersSelectMenu.Visibility = Visibility.Visible;
				}
				UpdateAllMaxScores();
			});
		}

		private void Delete_Save1_Click(object sender, RoutedEventArgs e) => ProcessDeleteSave("save1.txt");
        private void Delete_Save2_Click(object sender, RoutedEventArgs e) => ProcessDeleteSave("save2.txt");
        private void Delete_Save3_Click(object sender, RoutedEventArgs e) => ProcessDeleteSave("save3.txt");

		private void ProcessDeleteSave(string saveFileName)
		{
			HandleButtonClick(() =>
			{
				if (save.CheckSaveExistence(saveFileName))
				{
					save.DeleteSave(saveFileName);
					MessageBox.Show("Save deleted successfully.");
				}
				else
				{
					MessageBox.Show("Save not found.");
				}
				UpdateAllMaxScores();
			});
		}

		//Game start menu buttons
		private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            sound.PlaySound("button-click");
            if (gameState.CurrentCharacter != -1 && gameState.CurrentDifficultyMultiplier != -1)
            {
                CharactersSelectMenu.Visibility = Visibility.Hidden;
                Menu.Visibility = Visibility.Hidden;
                InGameUI.Visibility = Visibility.Visible;
                GameScreen = new GameScreen(bitmapImage);
                GameScreen.GameSpaceLoaded += (o, args) => GameScreen.GameSpace.Focus();
                Game.Children.Add(GameScreen);
                GameScreen.Visibility = Visibility.Visible;
                if (gameState.CurrentCharacter == 1)
                {
                    player = new Player(1, "Character1", 100.0, 1.0, 5.0, 1.2, new Vector(960, 532), 0, 100.0, 1, 2.5);
                }
                else if (gameState.CurrentCharacter == 2)
                {
                    player = new Player(2, "Character2", 200.0, 0.75, 3.5, 1.5, new Vector(960, 532), 0, 200, 2, 10);
                }
                gameSpace = new Space(1, 1920, 1064);
                gameControls = new GameControls(GameScreen, player);
                enemyControls = new EnemyControls(gameState.CurrentDifficultyMultiplier, GameScreen);

                player.CurrentHealth = player.CurrentHealth * gameState.CurrentDifficultyMultiplier;
                player.MaxHealth = player.MaxHealth * gameState.CurrentDifficultyMultiplier;

                Canvas.SetZIndex(InGameUI, 1);

                TimeTimer.Start();
                gameControls.StartGame();
                enemyControls.StartEnemySpawning();
            }
            else
            {
                MessageBox.Show("Please select character and difficulty.");
            }
        }

        private List<Button> characterButtons;
        private void InitializeCharacterButtons()
        {
            btnCharacter1.Tag = 1;
            btnCharacter2.Tag = 2;

            characterButtons = new List<Button> { btnCharacter1, btnCharacter2 };

            foreach (var btn in characterButtons)
            {
                btn.Click += CharacterButton_Click;
            }
        }
        private void CharacterButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            sound.PlaySound("button-click");

            foreach (var btn in characterButtons)
            {
                btn.Background = Brushes.Black;
                btn.Foreground = Brushes.White;
            }

            clickedButton.Background = Brushes.White;
            clickedButton.Foreground = Brushes.Black;

            if (clickedButton.Tag is int characterId)
            {
                gameState.CurrentCharacter = characterId;
            }
        }

        private List<Button> difficultyButtons;

        private void InitializeDifficultyButtons()
        {
            difficultyButtons = new List<Button> { btnEasy, btnNormal, btnHard };

            btnEasy.Tag = new DifficultySetting
            {
                Multiplier = 2.0,
                BackgroundUri = "pack://application:,,,/Assets/Textures/background-easy.png",
                TextForeground = Brushes.Black
            };

            btnNormal.Tag = new DifficultySetting
            {
                Multiplier = 1.0,
                BackgroundUri = "pack://application:,,,/Assets/Textures/background-normal.png",
                TextForeground = Brushes.White
            };

            btnHard.Tag = new DifficultySetting
            {
                Multiplier = 0.5,
                BackgroundUri = "pack://application:,,,/Assets/Textures/background-hard.png",
                TextForeground = Brushes.White
            };

            foreach (var btn in difficultyButtons)
            {
                btn.Click += DifficultyButton_Click;
            }
        }
        private void DifficultyButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            sound.PlaySound("button-click");

            foreach (var btn in difficultyButtons)
            {
                btn.Background = Brushes.Black;
                btn.Foreground = Brushes.White;
            }

            clickedButton.Background = Brushes.White;
            clickedButton.Foreground = Brushes.Black;

            if (clickedButton.Tag is DifficultySetting setting)
            {
                gameState.CurrentDifficultyMultiplier = setting.Multiplier;
                bitmapImage = new BitmapImage(new Uri(setting.BackgroundUri));

                Gold.Foreground = setting.TextForeground;
                Score.Foreground = setting.TextForeground;
                timerText.Foreground = setting.TextForeground;
            }
        }

        #endregion

        #region FuntionalMethods
        private string FormatTime(string rawTime)
        {
            int timeInSeconds = int.Parse(rawTime);
            int minutes = timeInSeconds / 60;
            int seconds = timeInSeconds % 60;
            return $"{minutes:D2}:{seconds:D2}";
        }
        private void UpdateMaxScoreAndTime(TextBlock maxScoreTextBlock, string saveFileName, Run textBlockMaxScore, TextBlock maxTimeTextBlock, Run textBlockMaxTime)
        {
            if (save.CheckSaveExistence(saveFileName) == false)
            {
                maxScoreTextBlock.Visibility = Visibility.Hidden;
                maxTimeTextBlock.Visibility = Visibility.Hidden;
            }
            else
            {
                maxScoreTextBlock.Visibility = Visibility.Visible;
                maxTimeTextBlock.Visibility = Visibility.Visible;
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
                            }
                            else if (key == "maxTime")
                            {
                                textBlockMaxTime.Text = FormatTime(value);
                            }
                        }
                    }
                }
            }
        }
		private void UpdateAllMaxScores()
		{
			for (int i = 1; i <= 3; i++)
			{
				UpdateMaxScoreAndTime(
					FindName($"maxScore{i}") as TextBlock,
					$"save{i}.txt",
					FindName($"TextBlockMaxScore{i}") as Run,
					FindName($"maxTime{i}") as TextBlock,
					FindName($"TextBlockMaxTime{i}") as Run);
			}
		}
		private void UIUpdateTimer_Tick(object sender, EventArgs e)
		{
			music.SetMusicVolume();
			PlayerCurrentHealth.Text = Math.Max(0, Math.Round(player.CurrentHealth)).ToString("F0");
			PlayerMaxHealth.Text = player.MaxHealth.ToString();
			pbPlayerHealth.Value = player.CurrentHealth;
			pbPlayerHealth.Maximum = player.MaxHealth;
			GoldCount.Text = player.Gold.ToString();
			ScoreCount.Text = gameState.CurrentScore.ToString();
			timerText.Text = $"{gameState.CurrentTime / 60:00}:{gameState.CurrentTime % 60:00}";
			if (gameState.IsGameEnded) RestartGame();
		}
		public void RestartGame()
		{
			gameState = new GameState();
			TimeTimer.Stop();
			GameScreen.GameSpace.Children.Clear();
			GameScreen.Visibility = Visibility.Hidden;
			InGameUI.Visibility = Visibility.Hidden;
			MainMenu.Visibility = Visibility.Visible;
			Menu.Visibility = Visibility.Visible;
		}
		#endregion
	}
}