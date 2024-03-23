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
        private Dictionary<int, Tuple<Point, Point>> skud;

        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            pictureBox = pictureBox1;
            InitializeTimer();
            await UpdateMovements();
            LoadMapImage();
            InitializeSkud();
        }

        private void InitializeSkud()
        {
            //PictureBox размером 964; 411
            skud = new Dictionary<int, Tuple<Point, Point>>
            {
                { 0, Tuple.Create(new Point(424, 5), new Point(488, 91)) },
                { 1, Tuple.Create(new Point(563, 5), new Point(639, 144)) },
                { 2, Tuple.Create(new Point(645, 5), new Point(682, 144)) },
                { 3, Tuple.Create(new Point(689, 5), new Point(727, 144)) },
                { 4, Tuple.Create(new Point(732, 5), new Point(793, 144)) },
                { 5, Tuple.Create(new Point(800, 5), new Point(868, 144)) },
                { 6, Tuple.Create(new Point(873, 5), new Point(959, 144)) },
                { 7, Tuple.Create(new Point(843, 265), new Point(959, 405)) },
                { 8, Tuple.Create(new Point(723, 265), new Point(837, 405)) },
                { 9, Tuple.Create(new Point(655, 265), new Point(718, 405)) },
                { 10, Tuple.Create(new Point(604, 265), new Point(650, 405)) },
                { 11, Tuple.Create(new Point(537, 265), new Point(599, 405)) },
                { 12, Tuple.Create(new Point(423, 265), new Point(531, 405)) },
                { 13, Tuple.Create(new Point(380, 265), new Point(418, 405)) },
                { 14, Tuple.Create(new Point(212, 265), new Point(374, 405)) },
                { 15, Tuple.Create(new Point(81, 265), new Point(206, 405)) },
                { 16, Tuple.Create(new Point(5, 265), new Point(76, 405)) },
                { 17, Tuple.Create(new Point(5, 5), new Point(41, 144)) },
                { 18, Tuple.Create(new Point(47, 5), new Point(85, 144)) },
                { 19, Tuple.Create(new Point(90, 5), new Point(167, 144)) },
                { 20, Tuple.Create(new Point(174, 5), new Point(211, 144)) },
                { 21, Tuple.Create(new Point(217, 5), new Point(330, 144)) },
                { 22, Tuple.Create(new Point(337, 5), new Point(374, 144)) }
            };
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
            mapImage = new Bitmap(pictureBox1.Image);
            // или же mapImage = new Bitmap(@"C:\Users\Admin\Desktop\КЗ по сессиям и ресурсы\Помещения больницы + СКУД.png");
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
            Dictionary<Point, bool> occupiedPositions = new Dictionary<Point, bool>(); // Словарь для отслеживания занятых позиций
            int circleDiameter = 10; // Диаметр круга
            int maxAttempts = 100; // Максимальное количество попыток генерации координат

            foreach (var movement in movements)
            {
                Brush brush = movement.PersonRole == "Сотрудник" ? Brushes.Blue : Brushes.Green;

                // Получаем случайные координаты в пределах границ комнаты
                int roomId = movement.LastSecurityPointNumber;
                if (skud.ContainsKey(roomId))
                {
                    Tuple<Point, Point> roomBounds = skud[roomId];

                    // Переменные для хранения координат нового круга
                    int x = 0;
                    int y = 0;
                    int attempts = 0; // Счетчик попыток генерации координат

                    // Генерируем случайные координаты, пока круг не помещается в комнате или не превышено максимальное количество попыток
                    while (attempts < maxAttempts)
                    {
                        x = random.Next(roomBounds.Item1.X, roomBounds.Item2.X - circleDiameter);
                        y = random.Next(roomBounds.Item1.Y, roomBounds.Item2.Y - circleDiameter); 

                        // Проверяем, помещается ли круг в комнате
                        bool canDrawCircle = true;
                        for (int i = x; i < x + circleDiameter; i++)
                        {
                            for (int j = y; j < y + circleDiameter; j++)
                            {
                                if (occupiedPositions.ContainsKey(new Point(i, j)))
                                {
                                    canDrawCircle = false;
                                    break;
                                }
                            }
                            if (!canDrawCircle) break;
                        }

                        // Если круг помещается в комнате, отмечаем занятые позиции и выходим из цикла
                        if (canDrawCircle)
                        {
                            for (int i = x; i < x + circleDiameter; i++)
                            {
                                for (int j = y; j < y + circleDiameter; j++)
                                {
                                    occupiedPositions[new Point(i, j)] = true;
                                }
                            }
                            break;
                        }
                        attempts++;
                    }

                    // Отрисовываем круг, если он помещается в комнате
                    if (attempts < maxAttempts)
                        g.FillEllipse(brush, x, y, circleDiameter, circleDiameter);
                }
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
