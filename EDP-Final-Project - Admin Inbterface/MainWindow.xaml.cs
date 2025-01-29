using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace EDP_Final_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainFrame.Navigate(new LoginPage());
        }
        //Navigate Page
        public void NavigateToPage(Page page)
        {
            MainFrame.Navigate(page);
        }
        //Navigate Page
        //Custom Header Function
        private void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        //Custom Header Function


        //Minimize Window Function
        private void Minimize_Function(object sender, MouseButtonEventArgs e)
        {
            // Minimize the window
            this.WindowState = WindowState.Minimized;
        }
        private void Minimize_MouseEnter(object sender, MouseEventArgs e)
        {
            // Change the source to Minimize2.png when hovering
            Minimize.Source = new BitmapImage(new Uri("/Minimize2.png", UriKind.Relative));
        }
        private void Minimize_MouseLeave(object sender, MouseEventArgs e)
        {
            // Revert the source back to Minimize.png when hover ends
            Minimize.Source = new BitmapImage(new Uri("/Minimize.png", UriKind.Relative));
        }
        //Minimize Window Function


        //Close Window Function
        private void Close_Function(object sender, MouseButtonEventArgs e)
        {
            // Close the window
            this.Close();
        }
        private void Close_MouseEnter(object sender, MouseEventArgs e)
        {
            // Change the source to Minimize2.png when hovering
            Close.Source = new BitmapImage(new Uri("/Close2.png", UriKind.Relative));
        }
        private void Close_MouseLeave(object sender, MouseEventArgs e)
        {
            // Revert the source back to Minimize.png when hover ends
            Close.Source = new BitmapImage(new Uri("/Close.png", UriKind.Relative));
        }
        //Close Window Function


        //TextField Placeholder functions
        //Lose Textfield focus
        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Check if the focused element is a TextBox or PasswordBox and the click happened outside of it
            if ((Keyboard.FocusedElement is TextBox textBox && !textBox.IsMouseOver) ||
                (Keyboard.FocusedElement is PasswordBox passwordBox && !passwordBox.IsMouseOver))
            {
                // Move focus to the parent element (e.g., Grid or Window) to remove focus
                FocusManager.SetFocusedElement(this, null);
                Keyboard.ClearFocus();
            }
        }
    }

}