﻿using Project.Assets.DataClasses;
using Project.Assets.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace Project.Assets.ControlClasses
{
    public class GameControls
    {
        private Canvas GameScreen { get; set; }
        private DispatcherTimer GameTimer = new DispatcherTimer();
        private bool UpKeyPressed, DownKeyPressed, LeftKeyPressed, RightKeyPressed;
        private float SpeedX, SpeedY, Friction = 0.75f, Speed;
        private Point mousePosition;

        private UserControl character1Control;
        private static Player character1;

        private TranslateTransform translateTransform;
        private RotateTransform rotateTransform;
        private TransformGroup combinedTransform;

        private Vector movementDirection;
        private bool jumpAvailable = true;
        private DispatcherTimer JumpTimer = new DispatcherTimer();

        public GameControls(Canvas gameScreen, Player player)
        {
            character1 = player;
            Speed = (float)character1.Speed;
            GameScreen = gameScreen;

            character1Control = new Character1Control(character1);
            GameScreen.Children.Add(character1Control);

            translateTransform = new TranslateTransform();
            rotateTransform = new RotateTransform();
            combinedTransform = new TransformGroup();
            combinedTransform.Children.Add(rotateTransform);
            combinedTransform.Children.Add(translateTransform);
            character1Control.RenderTransform = combinedTransform;

            movementDirection = new Vector(1, 0);

            StartGame();
        }

        public void StartGame()
        {
            GameScreen.KeyDown += KeyboardDown;
            GameScreen.KeyUp += KeyboardUp;
            GameScreen.MouseMove += GameScreen_MouseMove;

            GameTimer.Interval = TimeSpan.FromMilliseconds(16);
            GameTimer.Tick += GameTick;
            GameTimer.Start();

            JumpTimer.Interval = TimeSpan.FromSeconds(3);
            JumpTimer.Tick += JumpTimer_Tick;
        }

        private void KeyboardDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W)
            {
                UpKeyPressed = true;
            }
            if (e.Key == Key.S)
            {
                DownKeyPressed = true;
            }
            if (e.Key == Key.A)
            {
                LeftKeyPressed = true;
            }
            if (e.Key == Key.D)
            {
                RightKeyPressed = true;
            }
            if (e.Key == Key.Space)
            {
                movementDirection = new Vector(SpeedX, SpeedY);
                movementDirection.Normalize();
                Jump();
            }
        }

        private void KeyboardUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W)
            {
                UpKeyPressed = false;
            }
            if (e.Key == Key.S)
            {
                DownKeyPressed = false;
            }
            if (e.Key == Key.A)
            {
                LeftKeyPressed = false;
            }
            if (e.Key == Key.D)
            {
                RightKeyPressed = false;
            }
        }

        private void JumpTimer_Tick(object sender, EventArgs e)
        {
            JumpTimer.Stop();
            jumpAvailable = true;
        }

        private void Jump()
        {
            if (jumpAvailable)
            {
                SpeedY = (float)(character1.JumpLenght * movementDirection.Y);
                SpeedX = (float)(character1.JumpLenght * movementDirection.X);
                jumpAvailable = false;
                JumpTimer.Start();
            }
        }

        private void GameTick(object sender, EventArgs e)
        {
            if (UpKeyPressed)
            {
                SpeedY += Speed;
            }
            if (DownKeyPressed)
            {
                SpeedY -= Speed;
            }
            if (LeftKeyPressed)
            {
                SpeedX -= Speed;
            }
            if (RightKeyPressed)
            {
                SpeedX += Speed;
            }

            var maxX = GameScreen.ActualWidth - character1Control.ActualWidth;
            var maxY = GameScreen.ActualHeight - character1Control.ActualHeight;

            if (translateTransform.X < 0)
            {
                translateTransform.X = 0;
            }
            if (translateTransform.X + character1Control.ActualWidth > maxX)
            {
                translateTransform.X = maxX - character1Control.ActualWidth;
            }
            if (translateTransform.Y < 0)
            {
                translateTransform.Y = 0;
            }
            if (translateTransform.Y + character1Control.ActualHeight > maxY)
            {
                translateTransform.Y = maxY - character1Control.ActualHeight;
            }

            SpeedX = SpeedX * Friction;
            SpeedY = SpeedY * Friction;

            translateTransform.X += SpeedX;
            translateTransform.Y -= SpeedY;

            mousePosition = Mouse.GetPosition(GameScreen);
            RotateCharacterToMouse();
        }

        private void RotateCharacterToMouse()
        {
            var characterPosition = character1Control.TranslatePoint(new Point(character1Control.ActualWidth / 2.0, character1Control.ActualHeight / 2.0), GameScreen);
            var direction = mousePosition - characterPosition;
            var angle = Math.Atan2(direction.Y, direction.X) * 180 / Math.PI;

            rotateTransform.CenterX = character1Control.ActualWidth / 2.0;
            rotateTransform.CenterY = character1Control.ActualHeight / 2.0;

            rotateTransform.Angle = angle;
        }

        private void GameScreen_MouseMove(object sender, MouseEventArgs e)
        {
            mousePosition = e.GetPosition(GameScreen);
        }
    }
}
