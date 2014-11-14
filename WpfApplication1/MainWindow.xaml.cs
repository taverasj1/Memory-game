using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace WpfApplication1
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow 
    {

        public MainWindow()
        {
            InitializeComponent();
        
        }
        private void GridMousedDown (object sender, MouseButtonEventArgs e)
        {
            e.Handled = true; // prevents clicking of several layers.
           
        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            //Create the game window
            var game = new GameWindow();
            game.Show(); //Makes the window visible

        }
    }
}