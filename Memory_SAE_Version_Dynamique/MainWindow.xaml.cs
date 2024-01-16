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
        private Button[,] listeBoutonsDosCarte;
        private Button premierBouton, secondBouton;
        private List<Button> dosCarteCliqueeCeTour = new List<Button> { };
        private List<string> carteCliqueeCeTour = new List<string> { };
        private ImageBrush dosCarte = new ImageBrush();
        private DispatcherTimer timer;
        private TimeSpan elapsedTime;
        private bool isTimerRunning, menuFin, verifier = false;
        private int moves;
        private int nbLigne, nbCartes;
        private double score=1000;
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
                    listeBoutonsDosCarte[i, j] = new Button() { Background = dosCarte, Name = "DosBouton" + numImage};
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
            CalculScore();
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
            if (menuPauseResultat == true )
            {
                menuPause.Close();
                StartTimer();
            }
            else
            {     
                MainWindow mainWindow = new MainWindow();
                this.Close();
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
                    score += 10; 
                    pairesCorrecte.Add(carteCliqueeCeTour[0]);
                    pairesCorrecte.Add(carteCliqueeCeTour[1]);
                }

                else
                {
                    dosCarteCliqueeCeTour[0].Visibility = Visibility.Visible;
                    dosCarteCliqueeCeTour[1].Visibility = Visibility.Visible;
                    moves++;
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
                Console.WriteLine("Score : " + CalculScore());
#endif 
            }
            VictoireDefaite Fin = new VictoireDefaite();
            
            if (pairesCorrecte.Count == nbCartes * 2)
            {
                StopTimer();
                Fin.LabResultat.Text = "Vous avez gagnez !!!";
                this.Close();
                menuFin = (bool)Fin.ShowDialog();
                verifier = true;
            }
            else if (CalculScore() <= 0)
            {
                StopTimer();
                Fin.LabResultat.Text = "Vous avez perdu car votre score est inférieur à zéro !!!";
                this.Close();
                menuFin = (bool)Fin.ShowDialog();
                verifier = true;
            }
            if (verifier ==  true)
            {
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
            score = score - (moves * 0.3);
            txtScore.Text = Math.Round(score,2).ToString();
            return score;
        }
    }
}
