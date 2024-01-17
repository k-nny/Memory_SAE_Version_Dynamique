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
    /// Logique d'interaction pour MenuPause.xaml
    /// </summary>
    public partial class MenuPause : Window
    {
        public MenuPause()
        {
            InitializeComponent();
            ImageBrush logo = new ImageBrush();
            logo.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img/Memory_Card_Game_Logo.jpg"));
            rectLogo.Fill = logo;
        }

        private void ButReprendre_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void ButMenu_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
