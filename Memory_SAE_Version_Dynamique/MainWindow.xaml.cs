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

namespace Memory_SAE_Version_Dynamique
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {



        private Button[,] listeBoutons;
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
                resultatMessageBox = MessageBox.Show("Vous êtes sur le point de quitter le meilleur jeu de Memoire de 2024. Voulez vous vraiment le quitter ?", "Verification Annulation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (resultatMessageBox == MessageBoxResult.Yes)
                {
                    this.Close();
                }
            }   
            difficulteChoisie = ChoixDifficulte.ComboBoxDifficulté.Text;
            //resultatMessageBox = MessageBoxResult.Yes;
            Initialisation(difficulteChoisie);


        }
        public void Initialisation(string difficulteChoisie)
        {
            int nbLigne = 0, nbCartes, numImage=0;

            if (difficulteChoisie == "Facile")
                nbLigne = 4;
            else if (difficulteChoisie == "Intermédiaire")
                nbLigne = 6;
            else
                nbLigne = 8;
            nbCartes = (nbLigne * nbLigne)/2;
            List<string> images = new List<string>();
            for (int c  = 0; c < nbCartes; c++)
            {
                for (int d = 0; d < 2; d++) 
                {
                    images.Add("img/img1 (" + c + ").jpg");
                }
            }
            MelangeImages(images);
#if DEBUG
            Console.WriteLine(images.Count);
#endif
            listeBoutons = new Button[nbLigne, nbLigne];
            for (int i = 0; i < nbLigne; i++)
            {
                ColumnDefinition colDef = new ColumnDefinition();
                GridJeu.ColumnDefinitions.Add(colDef);
                RowDefinition ligDef = new RowDefinition();
                GridJeu.RowDefinitions.Add(ligDef);
                for (int j = 0; j < nbLigne; j++)
                {
#if DEBUG
                    Console.WriteLine(numImage);
#endif
                    ImageBrush initialisation = new ImageBrush();
                    initialisation.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + images[numImage]));
                    listeBoutons[i, j] = new Button() {Background=initialisation } ;
                    GridJeu.Children.Add(listeBoutons[i, j]);
                    Grid.SetColumn(listeBoutons[i, j], i);
                    Grid.SetRow(listeBoutons[i, j],j);
                    numImage++;
                }
            }



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
    }
}
