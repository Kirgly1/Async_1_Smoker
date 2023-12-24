using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace CigaretteSmokersProblem
{
    public partial class MainForm : Form
    {
        private readonly object lockObject = new object();
        private bool tobaccoOnTable, paperOnTable, matchesOnTable;
        private bool smokerWithTobaccoIsSmoking, smokerWithPaperIsSmoking, smokerWithMatchesIsSmoking;

        private Label statusLabel;
        private Panel smokerWithTobaccoPanel;
        private Panel smokerWithPaperPanel;
        private Panel smokerWithMatchesPanel;

        public MainForm()
        {
            InitializeComponent();
            InitializeSmokers();
            StartThreads();
        }

        private void InitializeComponent()
        {
            // Инициализация компонентов формы
            statusLabel = new Label();
            smokerWithTobaccoPanel = new Panel();
            smokerWithPaperPanel = new Panel();
            smokerWithMatchesPanel = new Panel();

            this.SuspendLayout();

            // Инициализация формы
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void InitializeSmokers()
        {
            // Инициализация панели для курильщиков с табаком
            smokerWithTobaccoPanel.BackColor = Color.Gray;
            smokerWithTobaccoPanel.Location = new Point(50, 100);
            smokerWithTobaccoPanel.Size = new Size(50, 50);
            Controls.Add(smokerWithTobaccoPanel);

            //Инициализация панели для курильщиков с бумагой
            smokerWithPaperPanel.BackColor = Color.Gray;
            smokerWithPaperPanel.Location = new Point(150, 100);
            smokerWithPaperPanel.Size = new Size(50, 50);
            Controls.Add(smokerWithPaperPanel);

            //Инициализация панели для курильщиков со спичками
            smokerWithMatchesPanel.BackColor = Color.Gray;
            smokerWithMatchesPanel.Location = new Point(250, 100);
            smokerWithMatchesPanel.Size = new Size(50, 50);
            Controls.Add(smokerWithMatchesPanel);
        }

        private void StartThreads()
        {
            // Запуск потоков поставщика и курильщиков
            Thread supplierThread = new Thread(SupplierThread);
            Thread smokerWithTobaccoThread = new Thread(SmokerWithTobaccoThread);
            Thread smokerWithPaperThread = new Thread(SmokerWithPaperThread);
            Thread smokerWithMatchesThread = new Thread(SmokerWithMatchesThread);

            supplierThread.Start();
            smokerWithTobaccoThread.Start();
            smokerWithPaperThread.Start();
            smokerWithMatchesThread.Start();
        }

        private void SupplierThread()
        {
            Random random = new Random();

            while (true)
            {
                Thread.Sleep(random.Next(1000, 3000)); // Имитация времени на подготовку ингредиентов

                lock (lockObject)
                {
                    // Если какой-то из ингредиентов отсутствует на столе, добавляем их случайным образом
                    if (!tobaccoOnTable || !paperOnTable || !matchesOnTable)
                    {
                        int ingredient1 = random.Next(1, 4);
                        int ingredient2 = random.Next(1, 4);

                        // Убеждаемся, что ingredient2 отличается от ingredient1
                        while (ingredient2 == ingredient1)
                        {
                            ingredient2 = random.Next(1, 4);
                        }

                        // Размещаем ингредиенты на столе
                        PlaceIngredientsOnTable(ingredient1, ingredient2);
                    }
                }
            }
        }

        private void PlaceIngredientsOnTable(int ingredient1, int ingredient2)
        {
            tobaccoOnTable = paperOnTable = matchesOnTable = false;

            // Размещаем первый ингредиент
            switch (ingredient1)
            {
                case 1:
                    tobaccoOnTable = true;
                    break;
                case 2:
                    paperOnTable = true;
                    break;
                case 3:
                    matchesOnTable = true;
                    break;
            }

            // Размещаем второй ингредиент
            switch (ingredient2)
            {
                case 1:
                    tobaccoOnTable = true;
                    break;
                case 2:
                    paperOnTable = true;
                    break;
                case 3:
                    matchesOnTable = true;
                    break;
            }

            // Обновляем статус
            UpdateStatusLabel();

            // Сигнализируем всем потокам, что таблица обновлена
            Monitor.PulseAll(lockObject);
        }

        private void SmokerWithTobaccoThread()
        {
            while (true)
            {
                lock (lockObject)
                {
                    // Пока не все нужные ингредиенты на столе или кто-то уже курит, ждем
                    while (!(paperOnTable && matchesOnTable) || smokerWithTobaccoIsSmoking)
                    {
                        // Ожидаем сигнала о том, что таблица обновлена
                        Monitor.Wait(lockObject);
                    }

                    // Курильщик с табаком начинает курить
                    smokerWithTobaccoIsSmoking = true;
                    SetSmokerPanelColor(smokerWithTobaccoPanel, Color.Green);
                    MakeCigaretteAndSmoke("Smoker with Tobacco is smoking");
                    SetSmokerPanelColor(smokerWithTobaccoPanel, Color.Gray);
                    smokerWithTobaccoIsSmoking = false;
                }
            }
        }

        private void SmokerWithPaperThread()
        {
            while (true)
            {
                lock (lockObject)
                {
                    // Пока не все нужные ингредиенты на столе или кто-то уже курит, ждем
                    while (!(tobaccoOnTable && matchesOnTable) || smokerWithPaperIsSmoking)
                    {
                        Monitor.Wait(lockObject);
                    }

                    // Курильщик с бумагой начинает курить
                    smokerWithPaperIsSmoking = true;
                    SetSmokerPanelColor(smokerWithPaperPanel, Color.Blue);
                    MakeCigaretteAndSmoke("Smoker with Paper is smoking");
                    SetSmokerPanelColor(smokerWithPaperPanel, Color.Gray);
                    smokerWithPaperIsSmoking = false;
                }
            }
        }

        private void SmokerWithMatchesThread()
        {
            while (true)
            {
                lock (lockObject)
                {
                    // Пока не все нужные ингредиенты на столе или кто-то уже курит, ждем
                    while (!(tobaccoOnTable && paperOnTable) || smokerWithMatchesIsSmoking)
                    {
                        Monitor.Wait(lockObject);
                    }

                    // Курильщик с спичками начинает курить
                    smokerWithMatchesIsSmoking = true;
                    SetSmokerPanelColor(smokerWithMatchesPanel, Color.Red);
                    MakeCigaretteAndSmoke("Smoker with Matches is smoking");
                    SetSmokerPanelColor(smokerWithMatchesPanel, Color.Gray);
                    smokerWithMatchesIsSmoking = false;
                }
            }
        }

        private void SetSmokerPanelColor(Panel panel, Color color)
        {
            if (InvokeRequired)
            {
                // Используем Invoke, чтобы изменения в UI выполнялись в правильном потоке
                Invoke(new MethodInvoker(() => { panel.BackColor = color; }));
            }
            else
            {
                panel.BackColor = color;
            }
        }

        private void MakeCigaretteAndSmoke(string message)
        {
            Thread.Sleep(2000); // Имитация времени на изготовление и курение сигареты

            // Обновляем статус после завершения курения
            UpdateStatusLabel();

            // Выводим сообщение
            MessageBox.Show(message);
        }

        private void UpdateStatusLabel()
        {
            string status = "Tobacco: " + (tobaccoOnTable ? "On Table" : "Not On Table") + " | " +
                            "Paper: " + (paperOnTable ? "On Table" : "Not On Table") + " | " +
                            "Matches: " + (matchesOnTable ? "On Table" : "Not On Table");

            if (InvokeRequired)
            {
                // Используем Invoke, чтобы изменения в UI выполнялись в правильном потоке
                Invoke(new MethodInvoker(() => { statusLabel.Text = status; }));
            }
            else
            {
                statusLabel.Text = status;
            }
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
