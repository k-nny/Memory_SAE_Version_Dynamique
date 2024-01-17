using Memory_SAE;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        private Rectangle[,] listeBoutonsDosCarte;
        private Button premierBouton, secondBouton;
        private List<Rectangle> dosCarteCliqueeCeTour = new List<Rectangle> { };
        private List<string> carteCliqueeCeTour = new List<string> { };
        private ImageBrush dosCarte = new ImageBrush();
        private DispatcherTimer timer;
        private TimeSpan elapsedTime;
        private bool isTimerRunning, menuFin, verifier = false;
        private int moves;
        private int nbLigne, nbCartes;
        private double score = 1000;
        private List<string> images = new List<string>();
        private List<string> pairesCorrecte = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
            bool resultat;
            string difficulteChoisie;
            moves = 0;
            MessageBoxResult resultatMessageBox = MessageBoxResult.No;
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

        public Rectangle[,] Initialisation(string difficulteChoisie)
        {
            ImageBrush pause = new ImageBrush();
            pause.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img/pause.jpg"));
            this.ButPause.Background = pause;
            int numImage = 0;
            if (difficulteChoisie == "Facile")
                nbLigne = 4;
            else if (difficulteChoisie == "Intermédiaire")
                nbLigne = 6;
            else
            {
                nbLigne = 8;
                score = 1500;
            }
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
            listeBoutonsDosCarte = new Rectangle[nbLigne, nbLigne];
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
                    listeBoutonsDosCarte[i, j] = new Rectangle() { Name = "DosBouton" + numImage };
                    listeBoutonsDosCarte[i, j].Fill = dosCarte;
                    listeBoutonsDosCarte[i, j].MouseLeftButtonDown += CliqueCarte;
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
            Rectangle cartecliquee = (Rectangle)sender;
            cartecliquee.Visibility = Visibility.Hidden;
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

#if DEBUG
            Console.WriteLine(NomImageCarte);
#endif
            Verification();
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

        private void ButPause_Click(object sender, RoutedEventArgs e)
        {
            StopTimer();
            MenuPause menuPause = new MenuPause();
            bool menuPauseResultat = (bool)menuPause.ShowDialog();
            if (menuPauseResultat == false)
            {
                menuPause.Close();
                StartTimer();
            }
            else
            {
                this.Visibility = Visibility.Hidden;
                MainWindow mainWindow = new MainWindow();
                this.Close();
                mainWindow.Show();
            }
        }

        private void Verification()
        {
#if DEBUG
            Console.WriteLine("Nombre de cartes visibles " + dosCarteCliqueeCeTour.Count);
            for (int i = 0; i < carteCliqueeCeTour.Count; i++)
                Console.WriteLine("La carte cliquée en position " + i + " est : " + carteCliqueeCeTour[i]);
#endif
            if (pairesCorrecte.Count == nbCartes * 2 - 2 && carteCliqueeCeTour.Count == 2)
            {
                carteCliqueeCeTour.Insert(2, "");
                dosCarteCliqueeCeTour.Add(RectVerifFin);
            }

            if (carteCliqueeCeTour.Count == 3)
            {
                Thread.Sleep(200);
                if (carteCliqueeCeTour[0] == carteCliqueeCeTour[1])
                {
                    score += 10;
                    pairesCorrecte.Add(carteCliqueeCeTour[0]);
                    pairesCorrecte.Add(carteCliqueeCeTour[1]);
                }

                else
                {
                    dosCarteCliqueeCeTour[0].Visibility = Visibility.Visible;
                    dosCarteCliqueeCeTour[1].Visibility = Visibility.Visible;
                    moves++;
                    CalculScore();
                }
                dosCarteCliqueeCeTour[2].Visibility = Visibility.Visible;
                dosCarteCliqueeCeTour.Clear();
                carteCliqueeCeTour.Clear();
#if DEBUG
                Console.WriteLine("Nombre de cartes visibles " + dosCarteCliqueeCeTour.Count);
                for (int i = 0; i < dosCarteCliqueeCeTour.Count; i++)
                {
                    Console.WriteLine(dosCarteCliqueeCeTour[i]);
                }
                Console.WriteLine("Score : " + score);
#endif 
            }
            VictoireDefaite Fin = new VictoireDefaite();

            if (pairesCorrecte.Count == nbCartes * 2)
            {
                StopTimer();
                Fin.LabResultat.Text = "Vous avez gagnez !!!";
                this.Close();
                verifier = true;
            }
            else if (score <= 0)
            {
                StopTimer();
                Fin.LabResultat.Text = "Vous avez perdu car votre score est inférieur à zéro !!!";
                this.Close();
                verifier = true;
            }
            if (verifier == true)
            {
                Fin.LabResultatScore.Text = "Votre score est de " + Math.Round(score);
                menuFin = (bool)Fin.ShowDialog();
                if (menuFin == false)
                {
                    Fin.Close();
                    MessageBoxResult resultatMessageBoxFin = MessageBoxResult.No;
                    resultatMessageBoxFin = MessageBox.Show("Vous venez de quitter le meilleur jeu de mémoire de 2024", "Information Annulation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    if (resultatMessageBoxFin == MessageBoxResult.OK)
                    {
                        this.Close();
                    }
                }
                else
                {
                    MainWindow mainWindowRejouer = new MainWindow();
                    this.Close();
                    mainWindowRejouer.ShowDialog();
                }
            }
        }
        private double CalculScore()
        {
            string txtTemps = txtTimer.Text.Substring(3, 2);
            double coeff, resultat;
            double.TryParse(txtTemps, out resultat);
            if (resultat < 10)
                coeff = 0.25;
            else if (resultat < 20)
                coeff = 0.4;
            else if (resultat < 30)
                coeff = 0.6;
            else coeff = 0.8;

            score = score - (moves * coeff);
            txtScore.Text = Math.Round(score).ToString();
            return score;
        }
    }
}
