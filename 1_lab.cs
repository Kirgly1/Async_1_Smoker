using System;
using System.Threading;
using System.Windows.Forms;

namespace CigaretteSmokersProblem
{
    public partial class MainForm : Form
    {
        private readonly object lockObject = new object();
        private bool tobaccoOnTable, paperOnTable, matchesOnTable;
        private bool smokerWithTobaccoIsSmoking, smokerWithPaperIsSmoking, smokerWithMatchesIsSmoking;

        public MainForm()
        {
            InitializeComponent();
            InitializeSmokers();
        }

        private void InitializeSmokers()
        {
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
                Thread.Sleep(random.Next(1000, 3000)); // Simulating time to prepare ingredients

                lock (lockObject)
                {
                    if (!tobaccoOnTable || !paperOnTable || !matchesOnTable)
                    {
                        int ingredient1 = random.Next(1, 4);
                        int ingredient2 = random.Next(1, 4);

                        while (ingredient2 == ingredient1)
                        {
                            ingredient2 = random.Next(1, 4);
                        }

                        PlaceIngredientsOnTable(ingredient1, ingredient2);
                    }
                }
            }
        }

        private void PlaceIngredientsOnTable(int ingredient1, int ingredient2)
        {
            tobaccoOnTable = paperOnTable = matchesOnTable = false;

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

            UpdateStatusLabel();
        }

        private void SmokerWithTobaccoThread()
        {
            while (true)
            {
                lock (lockObject)
                {
                    if (paperOnTable && matchesOnTable && !smokerWithTobaccoIsSmoking)
                    {
                        smokerWithTobaccoIsSmoking = true;
                        MakeCigaretteAndSmoke("Smoker with Tobacco is smoking");
                    }
                }
            }
        }

        private void SmokerWithPaperThread()
        {
            while (true)
            {
                lock (lockObject)
                {
                    if (tobaccoOnTable && matchesOnTable && !smokerWithPaperIsSmoking)
                    {
                        smokerWithPaperIsSmoking = true;
                        MakeCigaretteAndSmoke("Smoker with Paper is smoking");
                    }
                }
            }
        }

        private void SmokerWithMatchesThread()
        {
            while (true)
            {
                lock (lockObject)
                {
                    if (tobaccoOnTable && paperOnTable && !smokerWithMatchesIsSmoking)
                    {
                        smokerWithMatchesIsSmoking = true;
                        MakeCigaretteAndSmoke("Smoker with Matches is smoking");
                    }
                }
            }
        }

        private void MakeCigaretteAndSmoke(string message)
        {
            Thread.Sleep(2000); // Simulating time to make and smoke a cigarette

            smokerWithTobaccoIsSmoking = smokerWithPaperIsSmoking = smokerWithMatchesIsSmoking = false;
            UpdateStatusLabel();

            MessageBox.Show(message);
        }

        private void UpdateStatusLabel()
        {
            string status = "Tobacco: " + (tobaccoOnTable ? "On Table" : "Not On Table") + " | " +
                            "Paper: " + (paperOnTable ? "On Table" : "Not On Table") + " | " +
                            "Matches: " + (matchesOnTable ? "On Table" : "Not On Table");

            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => { statusLabel.Text = status; }));
            }
            else
            {
                statusLabel.Text = status;
            }
        }
    }
}
