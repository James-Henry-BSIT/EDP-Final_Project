using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace EDP_Final_Project
{
    /// <summary>
    /// Interaction logic for Loading.xaml
    /// </summary>
    public partial class Loading : Page
    {
        private bool _isLogout = false; // Flag to check if it's a logout event

        public Loading(bool isLogout = false)
        {
            InitializeComponent();

            _isLogout = isLogout;

            // Simulate loading and navigate accordingly
            _ = LoadAndNavigateAsync();
        }

        private async Task LoadAndNavigateAsync()
        {
            await Task.Delay(2000); // Simulate loading delay

            if (_isLogout)
            {
                // Navigate to LoginPage.xaml if returning from logout
                NavigationService.Navigate(new LoginPage());
            }
            else
            {
                // Navigate to Landing.xaml for normal flow
                NavigationService.Navigate(new Landing());
            }
        }
    }
}
