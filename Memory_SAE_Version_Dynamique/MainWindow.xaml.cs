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
            MessageBoxResult resultatMessageBox = MessageBoxResult.No;


            InitializeComponent();

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
            //resultatMessageBox = MessageBoxResult.Yes;
            Initialisation("Facile");


        }
        public void Initialisation(string difficulteChoisie)
        {
            int nbLigne = 0;
           
            if (difficulteChoisie == "Facile")
                nbLigne = 4;
            else if (difficulteChoisie == "Intermédiaire")
                nbLigne = 6;
            else
                nbLigne = 8;
            listeBoutons = new Button[nbLigne, nbLigne];
            for (int i = 0; i < nbLigne; i++)
            {
                ColumnDefinition colDef = new ColumnDefinition();
                GridJeu.ColumnDefinitions.Add(colDef);
                RowDefinition ligDef = new RowDefinition();
                GridJeu.RowDefinitions.Add(ligDef);
                for (int j = 0; j < nbLigne; j++)
                {
                    listeBoutons[i,j]=new Button();

                }
            }
           
                for (int c = 0; c < listeBoutons.Length; c++)
                {
                    for (int l = 0; l < listeBoutons.Length; l++)
                    {
                    Grid.SetColumn(listeBoutons[c,l], c);
                    Grid.SetRow(listeBoutons[c,l], l);
                    }
                }
            
#if DEBUG
         /*   for (int i = 0; i < listeBoutons.Count; i++)
            {
                Console.WriteLine("Debug version \n " + listeBoutons[i] + "\nbouton " + i);
            }*/
#endif
        }
    }
}
