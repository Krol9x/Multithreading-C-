using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Gra
{
    public partial class Form1 : Form
    {
        bool goLeft, goRight, shooting, isGameOver;
        int score;
        int playerSpeed = 12;
        int enemySpeed;
        int bulletSpeed;
        Random rnd = new Random();

        private SemaphoreSlim semaphore; // Deklaracja semafora slim

        public Form1()
        {
            InitializeComponent();
            resetGame();

            // Inicjalizacja semafora slim z wartością 1
            semaphore = new SemaphoreSlim(1, 1);

            // Uruchomienie puli wątków
            ThreadPool.QueueUserWorkItem(EnemyThread, enemyOne);
            ThreadPool.QueueUserWorkItem(EnemyThread, enemyTwo);
            ThreadPool.QueueUserWorkItem(EnemyThread, enemyThree);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void mainGameTimerEvent(object sender, EventArgs e)
        {
            txtScore.Text = score.ToString();

            enemyOne.Top += enemySpeed;
            enemyTwo.Top += enemySpeed;
            enemyThree.Top += enemySpeed;

            if (enemyOne.Top > 710 || enemyTwo.Top > 710 || enemyThree.Top > 710)
            {
                gameOver();
            }

            if (goLeft && player.Left > 0)
            {
                player.Left -= playerSpeed;
            }
            if (goRight && player.Left < 618)
            {
                player.Left += playerSpeed;
            }

            if (shooting)
            {
                bulletSpeed = 10;
                bullet.Top -= bulletSpeed;
            }
            else
            {
                bullet.Left = -100;
                bulletSpeed = 0;
            }

            if (bullet.Top < -30)
            {
                shooting = false;
            }
            if (bullet.Bounds.IntersectsWith(enemyOne.Bounds))
            {
                score += 1;
                enemyOne.Top = -450;
                enemyOne.Left = rnd.Next(20, 600);
                shooting = false;
            }
            if (bullet.Bounds.IntersectsWith(enemyTwo.Bounds))
            {
                score += 1;
                enemyTwo.Top = -650;
                enemyTwo.Left = rnd.Next(20, 600);
                shooting = false;
            }
            if (bullet.Bounds.IntersectsWith(enemyThree.Bounds))
            {
                score += 1;
                enemyThree.Top = -750;
                enemyThree.Left = rnd.Next(20, 600);
                shooting = false;
            }
        }

        private void keyisdown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = true;
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = true;
            }
        }

        private void keyisup(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }
            if (e.KeyCode == Keys.Space && !shooting)
            {
                shooting = true;
                bullet.Top = player.Top + 30;
                bullet.Left = player.Left + (player.Width / 2);
            }
            if (e.KeyCode == Keys.Enter && isGameOver)
            {
                resetGame();
            }
        }

        private void resetGame()
        {
            gameTimer.Start();
            enemySpeed = 6;

            enemyOne.Left = rnd.Next(20, 600);
            enemyTwo.Left = rnd.Next(20, 600);
            enemyThree.Left = rnd.Next(20, 600);

            enemyOne.Top = rnd.Next(20, 200) * -1;
            enemyTwo.Top = rnd.Next(20, 500) * -1;
            enemyThree.Top = rnd.Next(20, 900) * -1;

            score = 0;
            bulletSpeed = 0;
            bullet.Left = -300;
            shooting = false;
            isGameOver = false;
        }

        private void gameOver()
        {
            isGameOver = true;
            gameTimer.Stop();
            txtScore.Text += Environment.NewLine + "GAME OVER! (Click ENTER)";
        }

        private void txtScore_Click(object sender, EventArgs e)
        {

        }


        private void player_Click(object sender, EventArgs e)
        {

        }
        private void bullet_Click(object sender, EventArgs e)
        {

        }

        private void EnemyThread(object state)
        {
            var enemy = (PictureBox)state;

            while (!isGameOver)
            {
                semaphore.Wait(); // Zajęcie semafora

                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        enemy.Top += enemySpeed;
                    });
                }
                else
                {
                    enemy.Top += enemySpeed;
                }

                semaphore.Release(); // Zwolnienie semafora

                Thread.Sleep(50);
            }
        }
    }
}
