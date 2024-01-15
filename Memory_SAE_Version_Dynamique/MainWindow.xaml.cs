using Memory_SAE;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Policy;
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
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace Memory_SAE_Version_Dynamique
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {



        private Button[,] listeBoutons;
        private Button[,] listeBoutonsDosCarte;
        private Button premierBouton, secondBouton;
        private List<Button> dosCarteCliqueeCeTour = new List<Button> { };
        private List<string> carteCliqueeCeTour = new List<string> { };
        private ImageBrush dosCarte = new ImageBrush();
        private DispatcherTimer timer;
        private TimeSpan elapsedTime;
        private bool isTimerRunning;
        private int moves;
        private Score currentScore;
        private int nbLigne, nbCartes;
        private List<string> images = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
            bool resultat;
            string difficulteChoisie;
            moves = 0;
            currentScore = new Score();
            UpdateScoreText();
            MessageBoxResult resultatMessageBox = MessageBoxResult.No;

            //Creation du menu des difficultés
            MenuDifficulte ChoixDifficulte = new MenuDifficulte();

            resultat = (bool)ChoixDifficulte.ShowDialog();
            if (resultat == false)
            {
                resultatMessageBox = MessageBox.Show("Vous venez de quitter le meilleur jeu de mémoire de 2024", "Information Annulation", MessageBoxButton.OK, MessageBoxImage.Warning);
                if (resultatMessageBox == MessageBoxResult.OK)
                {
                    this.Close();
                }
            }
            difficulteChoisie = ChoixDifficulte.ComboBoxDifficulté.Text;
            //resultatMessageBox = MessageBoxResult.Yes;
            listeBoutonsDosCarte = Initialisation(difficulteChoisie);
            StartTimer();
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(1));
            UpdateTimerText();
        }
        private void StartTimer()
        {
            timer.Start();
            isTimerRunning = true;
        }

        private void StopTimer()
        {
            timer.Stop();
            isTimerRunning = false;
        }

        private void UpdateTimerText()
        {
            txtTimer.Text = $"{elapsedTime:mm\\:ss}";
        }
        private void UpdateScoreText()
        {

            txtScore.Text = $"Score : {currentScore.CalculateScore()}";
        }
        private void StartPauseTimer_Click(object sender, RoutedEventArgs e)
        {
            if (isTimerRunning)
            {
                StopTimer();
            }
            else
            {
                StartTimer();
            }
        }
        public Button[,] Initialisation(string difficulteChoisie)
        {
            int numImage = 0;
            if (difficulteChoisie == "Facile")
                nbLigne = 4;
            else if (difficulteChoisie == "Intermédiaire")
                nbLigne = 6;
            else
                nbLigne = 8;
            nbCartes = (nbLigne * nbLigne) / 2;
            
            for (int c = 0; c < nbCartes; c++)
            {
                for (int d = 0; d < 2; d++)
                {
                    images.Add("img/img1 (" + c + ").jpg");
                }
            }
            MelangeImages(images);
#if DEBUG
            Console.WriteLine("Nombre d'Images : " + images.Count);
#endif
            listeBoutons = new Button[nbLigne, nbLigne];
            listeBoutonsDosCarte = new Button[nbLigne, nbLigne];
            for (int i = 0; i < nbLigne; i++)
            {
                ColumnDefinition colDef = new ColumnDefinition();
                GridJeu.ColumnDefinitions.Add(colDef);
                RowDefinition ligDef = new RowDefinition();
                GridJeu.RowDefinitions.Add(ligDef);
                for (int j = 0; j < nbLigne; j++)
                {
#if DEBUG
                    Console.WriteLine("Image numéro " + numImage);
#endif
                    ImageBrush initialisation = new ImageBrush();
                    initialisation.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + images[numImage]));
                    listeBoutons[i, j] = new Button() { Background = initialisation, Name = "Bouton" + numImage };
                    GridJeu.Children.Add(listeBoutons[i, j]);
                    Grid.SetColumn(listeBoutons[i, j], i);
                    Grid.SetRow(listeBoutons[i, j], j);
                    dosCarte.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img/dos_carte.jpg"));
                    listeBoutonsDosCarte[i, j] = new Button() { Background = dosCarte, Name = "DosBouton" + numImage };
                    listeBoutonsDosCarte[i, j].Click += CliqueCarte;
                    GridJeu.Children.Add(listeBoutonsDosCarte[i, j]);
                    Grid.SetColumn(listeBoutonsDosCarte[i, j], i);
                    Grid.SetRow(listeBoutonsDosCarte[i, j], j);
                    numImage++;
#if DEBUG
                    Console.WriteLine("Nom du bouton numéro " + numImage + " : " + listeBoutons[i, j].Name + "\nNom du bouton du dos numéro " + numImage + " : " + listeBoutonsDosCarte[i, j].Name);
#endif
                }
            }
            return listeBoutonsDosCarte;
        }

        private void CliqueCarte(object sender, RoutedEventArgs e)
        {
            string NomImageCarte = "";
            Button cartecliquee = (Button)sender;
            dosCarteCliqueeCeTour.Add(cartecliquee);
            string NomBouton = cartecliquee.Name;
            for (int i = 0; i < nbCartes * 2; i++)
            {
                if (NomBouton == "DosBouton" + i)
                {
                    NomImageCarte = images[i];
                }
            }
            carteCliqueeCeTour.Add(NomImageCarte);
            cartecliquee.Visibility = Visibility.Hidden;
#if DEBUG
            Console.WriteLine(NomImageCarte);
#endif
            Verification();
            moves++;
        }

        private void MelangeImages(List<string> images)
        {
            Random random = new Random();
            int n = images.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                string value = images[k];
                images[k] = images[n];
                images[n] = value;
            }
        }
        private void Verification()
        {
#if DEBUG
            Console.WriteLine("Nombre de cartes visibles " + dosCarteCliqueeCeTour.Count);
            for (int i=0;i<carteCliqueeCeTour.Count;i++ )
                Console.WriteLine("La carte cliquée en position "+i+" est : "+carteCliqueeCeTour[i]);
#endif

            if (dosCarteCliqueeCeTour.Count == 2)
            {
                if (carteCliqueeCeTour[0] == carteCliqueeCeTour[1])
                {
                    //Score++
                }

                else
                {
                    dosCarteCliqueeCeTour[0].Visibility = Visibility.Visible;
                    dosCarteCliqueeCeTour[1].Visibility = Visibility.Visible;
                }
                Thread.Sleep(1000);
                dosCarteCliqueeCeTour.Clear();
                carteCliqueeCeTour.Clear();
#if DEBUG
                Console.WriteLine("Nombre de cartes visibles " + dosCarteCliqueeCeTour.Count);
                for (int i = 0; i < dosCarteCliqueeCeTour.Count; i++)
                {
                    Console.WriteLine(dosCarteCliqueeCeTour[i]);
                }
#endif
            }
        }
        public class Score
        {
            public TimeSpan Time { get; set; }
            public int Moves { get; set; }

            public int CalculateScore()
            {
                int timeScore = (int)(10000 / Time.TotalSeconds);

                int movesScore = 1000 - Moves;

                return (int)(0.7 * timeScore + 0.3 * movesScore);
            }
        }
    }
}
