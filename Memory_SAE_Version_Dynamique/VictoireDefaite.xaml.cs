using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Memory_SAE_Version_Dynamique
{
    /// <summary>
    /// Logique d'interaction pour VictoireDefaite.xaml
    /// </summary>
    public partial class VictoireDefaite : Window
    {
        public VictoireDefaite()
        {
            InitializeComponent();
            ImageBrush logo = new ImageBrush();
            logo.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img/Memory_Card_Game_Logo.jpg"));
            rectLogo.Fill = logo;
            ImageBrush menu = new ImageBrush();
            menu.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img/menu.png"));
            ButRejouer.Fill = menu;
            ImageBrush quitter = new ImageBrush();
            quitter.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img/quitter.png"));
            ButMenu.Fill = quitter;
        }


        private void ButRejouer_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ButMenu_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
