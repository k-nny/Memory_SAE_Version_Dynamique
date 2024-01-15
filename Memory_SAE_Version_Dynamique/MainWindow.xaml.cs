using Memory_SAE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
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
        private List<Button> dosCarteCliqueeCeTour = new List<Button> { };
        private List<Button> carteCliqueeCeTour = new List<Button> { };
        private ImageBrush dosCarte = new ImageBrush();
        private int nbLigne;
        public MainWindow()
        {
            InitializeComponent();
            bool resultat;
            string difficulteChoisie;
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
        }

        public Button[,] Initialisation(string difficulteChoisie)
        {
            int nbCartes, numImage = 0;

            if (difficulteChoisie == "Facile")
                nbLigne = 4;
            else if (difficulteChoisie == "Intermédiaire")
                nbLigne = 6;
            else
                nbLigne = 8;
            nbCartes = (nbLigne * nbLigne) / 2;
            List<string> images = new List<string>();
            for (int c = 0; c < nbCartes; c++)
            {
                for (int d = 0; d < 2; d++)
                {
                    images.Add("img/img1 (" + c + ").jpg");
                }
            }
            MelangeImages(images) ;
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
                    Console.WriteLine("Image numéro "+numImage);
#endif
                    ImageBrush initialisation = new ImageBrush();
                    initialisation.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + images[numImage]));
                    listeBoutons[i, j] = new Button() { Background = initialisation };
                    GridJeu.Children.Add(listeBoutons[i, j]);
                    Grid.SetColumn(listeBoutons[i, j], i);
                    Grid.SetRow(listeBoutons[i, j], j);
                    dosCarte.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img/dos_carte.jpg"));
                    listeBoutonsDosCarte[i, j] = new Button() { Background = dosCarte };
                    listeBoutonsDosCarte[i, j].Click += CliqueCarte;
                    GridJeu.Children.Add(listeBoutonsDosCarte[i, j]);
                    Grid.SetColumn(listeBoutonsDosCarte[i, j], i);
                    Grid.SetRow(listeBoutonsDosCarte[i, j], j);
                    numImage++;
                }
            }
            return listeBoutonsDosCarte;
        }

        private void CliqueCarte(object sender, RoutedEventArgs e)
        {
            Button cartecliquee = (Button)sender;
            dosCarteCliqueeCeTour.Add(cartecliquee);
            carteCliqueeCeTour.Add(cartecliquee);
            cartecliquee.Visibility = Visibility.Hidden;
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
        private void Verification()
        {
#if DEBUG
            Console.WriteLine("Nombre de cartes visibles "+dosCarteCliqueeCeTour.Count);
#endif
            if (dosCarteCliqueeCeTour.Count==3)
            {
                if (carteCliqueeCeTour[0].Background == carteCliqueeCeTour[1].Background)
                {
                    //appel de la fonction Score
                }
                else
                {
                    dosCarteCliqueeCeTour[0].Visibility = Visibility.Visible;
                    dosCarteCliqueeCeTour[1].Visibility = Visibility.Visible;
                }
                dosCarteCliqueeCeTour.Remove(carteCliqueeCeTour[1]);
                dosCarteCliqueeCeTour.Remove(carteCliqueeCeTour[0]);
            }
        }
    }
}
