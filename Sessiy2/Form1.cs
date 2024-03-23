using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebApplicationHospital;

namespace Sessiy2
{
    public partial class Form1 : Form
    {
        private PictureBox pictureBox;
        private Timer timer;
        private List<PersonMovement> movements;
        private Random random = new Random();
        private Bitmap mapImage;

        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            pictureBox = pictureBox1;
            InitializeTimer();
            await UpdateMovements();
            LoadMapImage(); // Загрузка изображения карты
        }

        private void InitializeTimer()
        {
            timer = new Timer();
            timer.Interval = 3000; // Обновление каждые 3 секунды
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            await UpdateMovements();
            RedrawMap();
        }

        private async Task UpdateMovements()
        {
            movements = await APIReader.getPersonMovements();
        }

        private void LoadMapImage()
        {
            // Загрузите изображение карты из файла или ресурсов
            // Пример:
            mapImage = new Bitmap(pictureBox1.Image);
        }

        private void RedrawMap()
        {
            using (Graphics g = pictureBox.CreateGraphics())
            {
                // Очищаем предыдущее изображение
                g.Clear(Color.White);

                // Отображаем изображение карты
                g.DrawImage(mapImage, 0, 0, pictureBox.Width, pictureBox.Height);

                // Рисуем перемещения клиентов и персонала поверх карты
                DrawMovements(g);
            }
        }

        private void DrawMovements(Graphics g)
        {
            foreach (var movement in movements)
            {
                Brush brush = movement.PersonRole == "Сотрудник" ? Brushes.Blue : Brushes.Green;
                int x = random.Next(0, pictureBox.Width - 10); // Учитываем размер кружка
                int y = random.Next(0, pictureBox.Height - 10); // Учитываем размер кружка
                g.FillEllipse(brush, x, y, 10, 10);
            }
        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;

            Console.WriteLine($"X: {x}, Y: {y}");
        }
    }
}
