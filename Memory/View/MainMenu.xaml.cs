using Memory.ViewModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Memory.View
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : UserControl
    {
        public MainMenu()
        {
            InitializeComponent();
        }
        private void Slide_Clicked(object sender, RoutedEventArgs e)
        {
            var game = DataContext as GameViewModel;
            var button = sender as Button;
            game?.ClickedBlock(button!.DataContext);
            Debug.WriteLine("Button was clicked.");
        }

        private void PlayAgain_C(object sender, RoutedEventArgs e)
        {
            var game = DataContext as GameViewModel;
            game?.Restart();
        }
    }
}
