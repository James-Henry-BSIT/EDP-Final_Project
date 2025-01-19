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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EDP_Final_Project
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Loading : Page
    {
        public Loading()
        {
            InitializeComponent();

            // Simulate loading and navigate to LandingPage
            _ = LoadAndNavigateAsync();
        }
        private async Task LoadAndNavigateAsync()
        {
            await Task.Delay(2000); // Simulate loading delay
            NavigationService.Navigate(new Landing());
        }
    }
}
