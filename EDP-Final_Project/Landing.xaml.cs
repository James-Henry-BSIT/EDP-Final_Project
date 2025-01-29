using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EDP_Final_Project
{
    /// <summary>
    /// Interaction logic for Landing.xaml
    /// </summary>
    public partial class Landing : Page
    {
        int illust_nochar = 1;
        int illust_variant = 1;
        string illust_body = "Full_Body";
        string illust_background = "Simple";
        string commercial_use = "NO";
        int illust_nochar_price = 20;
        int illust_variant_price = 10;
        int illust_body_price = 60;
        int illust_background_price = 10;
        int commercial_fee = 1;
        double illust_total_price;

        private bool _isDragging = false; // Track if the rectangle is being dragged
        private Point _startPoint; // Initial mouse position
        private Thickness _startMargin; // Initial margin of the rectangle

        // Min and max margin constraints
        private readonly Thickness _minMargin = new Thickness(28, 37, 534, 0);
        private readonly Thickness _maxMargin = new Thickness(161, 37, 401, 0);


        public Landing()
        {
            InitializeComponent();
            UpdateTotalPrice();
            LandingUsername();
        }

        //MENU SELECT HOVER FUNCTIONS
        private void HomeLabel_Hover(object sender, MouseEventArgs e)
        {
            Storyboard moveToHome = (Storyboard)MenuSelection.FindResource("MoveToHome");
            moveToHome.Begin();

            Storyboard expandHeight = (Storyboard)MenuSelection.FindResource("ExpandHoverMenuHeight");
            expandHeight.Begin();
        }

        private void UpdatesLabel_Hover(object sender, MouseEventArgs e)
        {
            Storyboard moveToUpdates = (Storyboard)MenuSelection.FindResource("MoveToUpdates");
            moveToUpdates.Begin();

            Storyboard expandHeight = (Storyboard)MenuSelection.FindResource("ExpandHoverMenuHeight");
            expandHeight.Begin();
        }

        private void CommissionLabel_Hover(object sender, MouseEventArgs e)
        {
            Storyboard moveToCommissions = (Storyboard)MenuSelection.FindResource("MoveToCommissions");
            moveToCommissions.Begin();

            Storyboard expandHeight = (Storyboard)MenuSelection.FindResource("ExpandHoverMenuHeight");
            expandHeight.Begin();
        }

        private void NotificationsLabel_Hover(object sender, MouseEventArgs e)
        {
            Storyboard moveToNotifications = (Storyboard)MenuSelection.FindResource("MoveToNotifications");
            moveToNotifications.Begin();

            Storyboard expandHeight = (Storyboard)MenuSelection.FindResource("ExpandHoverMenuHeight");
            expandHeight.Begin();
        }

        private void SocialsLabel_Hover(object sender, MouseEventArgs e)
        {
            Storyboard moveToSocials = (Storyboard)MenuSelection.FindResource("MoveToSocials");
            moveToSocials.Begin();

            Storyboard expandHeight = (Storyboard)MenuSelection.FindResource("ExpandHoverMenuHeight");
            expandHeight.Begin();
        }
        //MENU SELECT HOVER FUNCTIONS

        //MENU SELECT CLICK FUNCTIONS

        private void HomeLabel_Click(object sender, MouseButtonEventArgs e)
        {
            Storyboard moveIndictToHome = (Storyboard)MenuSelection.FindResource("MoveIndictToHome");
            moveIndictToHome.Begin();
        }

        private void UpdatesLabel_Click(object sender, MouseButtonEventArgs e)
        {
            Storyboard moveIndictToUpdates = (Storyboard)MenuSelection.FindResource("MoveIndictToUpdates");
            moveIndictToUpdates.Begin();
        }

        private void CommissionLabel_Click(object sender, MouseButtonEventArgs e)
        {
            Storyboard moveIndictToCommissions = (Storyboard)MenuSelection.FindResource("MoveIndictToCommissions");
            moveIndictToCommissions.Begin();
        }

        private void NotificationsLabel_Click(object sender, MouseButtonEventArgs e)
        {
            Storyboard moveIndictToNotifications = (Storyboard)MenuSelection.FindResource("MoveIndictToNotifications");
            moveIndictToNotifications.Begin();
        }

        private void SocialsLabel_Click(object sender, MouseButtonEventArgs e)
        {
            Storyboard moveIndictToSocials = (Storyboard)MenuSelection.FindResource("MoveIndictToSocials");
            moveIndictToSocials.Begin();
        }
        private void Hover_Retract(object sender, MouseEventArgs e)
        {
            Storyboard collapseHeight = (Storyboard)MenuSelection.FindResource("CollapseHoverMenuHeight");
            collapseHeight.Begin();
        }

        private bool _isExpanded = false; // Track whether AddMenuSelect is expanded

        private void AddMenuSelect_MouseEnter(object sender, MouseEventArgs e)
        {
            // Change the background color on hover using Color.FromRgb
            AddSelectBG.Fill = new SolidColorBrush(Color.FromRgb(65, 56, 77)); // Equivalent to #FF41384D
        }

        private void AddMenuSelect_MouseLeave(object sender, MouseEventArgs e)
        {
            // Revert the background color when hover ends using Color.FromRgb
            AddSelectBG.Fill = new SolidColorBrush(Color.FromRgb(54, 46, 66)); // Equivalent to #FF362E42
        }
        private void AddMenuSelect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Toggle expansion state
            _isExpanded = !_isExpanded;

            // Rotate the arrow and animate the margin change
            var rotation = _isExpanded ? 180 : 0;
            var margin = _isExpanded ? new Thickness(0, 0, 0, 61) : new Thickness(0, 0, 0, 35);

            // Get the RotateTransform applied to AddSelect
            var rotateTransform = (RotateTransform)AddSelect.RenderTransform;
            var rotationAnimation = new DoubleAnimation
            {
                To = rotation,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotationAnimation);

            // Animate the margin change
            var marginAnimation = new ThicknessAnimation
            {
                To = margin,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            LogOutOption.BeginAnimation(FrameworkElement.MarginProperty, marginAnimation);
        }

        private void LandingUsername()
        {
            // Set the username label to the current user's username
            Username.Text = UserSession.Username;
        }
        private void LogOutOption_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Clear sensitive user data (if using SecureUserSession or a similar mechanism)
            // Clear sensitive user data
            UserSession.Username = null;
            UserSession.Email = null;

            // Navigate to Loading.xaml with the logout flag
            NavigationService.Navigate(new Loading(isLogout: true));
        }
        //MENU SELECT CLICK FUNCTIONS


        //MENU ILLUSTRATION FUNCTIONS
        private void UpIllustNoChar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (illust_nochar < 10)
            {
                illust_nochar++;
                IllustNoChar.Text = illust_nochar.ToString();
                illust_nochar_price = 20 * illust_nochar;
                IllustNoCharCost.Text = "+$" + illust_nochar_price.ToString();
                UpdateTotalPrice(); // Update the total price
            }
        }

        private void DownIllustNoChar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (illust_nochar > 1)
            {
                illust_nochar--;
                IllustNoChar.Text = illust_nochar.ToString();
                illust_nochar_price = 20 * illust_nochar;
                IllustNoCharCost.Text = "+$" + illust_nochar_price.ToString();
                UpdateTotalPrice(); // Update the total price
            }
        }
        private void UpIllustVariant_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (illust_variant < 20)
            {
                illust_variant++;
                IllustVariant.Text = illust_variant.ToString();
                illust_variant_price = 10 * illust_variant;
                IllustVariantCost.Text = "+$" + illust_variant_price.ToString();
                UpdateTotalPrice(); // Update the total price
            }
        }

        private void DownIllustVariant_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (illust_variant > 1)
            {
                illust_variant--;
                IllustVariant.Text = illust_variant.ToString();
                illust_variant_price = 10 * illust_variant;
                IllustVariantCost.Text = "+$" + illust_variant_price.ToString();
                UpdateTotalPrice(); // Update the total price
            }
        }

        private void Arrow_MouseEnter(object sender, MouseEventArgs e)
        {
            // Cast the sender to an Image
            var image = sender as Image;
            if (image != null)
            {
                // Change the source based on the current image source
                if (image.Source.ToString().Contains("Down Arrow.png"))
                {
                    image.Source = new BitmapImage(new Uri("/images/Buttons/Hover Down Arrow.png", UriKind.Relative));
                }
                else if (image.Source.ToString().Contains("Up Arrow.png"))
                {
                    image.Source = new BitmapImage(new Uri("/images/Buttons/Hover Up Arrow.png", UriKind.Relative));
                }
            }
        }

        private void Arrow_MouseLeave(object sender, MouseEventArgs e)
        {
            // Cast the sender to an Image
            var image = sender as Image;
            if (image != null)
            {
                // Restore the original source based on the current image source
                if (image.Source.ToString().Contains("Hover Down Arrow.png"))
                {
                    image.Source = new BitmapImage(new Uri("/images/Buttons/Down Arrow.png", UriKind.Relative));
                }
                else if (image.Source.ToString().Contains("Hover Up Arrow.png"))
                {
                    image.Source = new BitmapImage(new Uri("/images/Buttons/Up Arrow.png", UriKind.Relative));
                }
            }
        }

        //BODY SELECT
        private void IllustFullBody_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AnimateOpacity(IllustFullBody, 1);
            AnimateOpacity(IllustHalfBody, 0.5);

            illust_body_price = 60;
            illust_body = "Full_Body";
            IllustBodyCost.Text = "+$" + illust_body_price.ToString();
            UpdateTotalPrice(); // Update the total price
        }

        private void IllustHalfBody_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AnimateOpacity(IllustHalfBody, 1);
            AnimateOpacity(IllustFullBody, 0.5);

            illust_body_price = 30;
            illust_body = "Half_Body";
            IllustBodyCost.Text = "+$" + illust_body_price.ToString();
            UpdateTotalPrice(); // Update the total price
        }
        private void IllustDetailed_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AnimateOpacity(DetailedIllust, 1);
            AnimateOpacity(SimpleIllust, 0.5);

            illust_background_price = 30;
            illust_background = "Detailed";
            IllustBackgroundCost.Text = "+$" + illust_background_price.ToString();
            UpdateTotalPrice(); // Update the total price
        }

        private void IllustSimple_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AnimateOpacity(SimpleIllust, 1);
            AnimateOpacity(DetailedIllust, 0.5);

            illust_background_price = 10;
            illust_background = "Simple";
            IllustBackgroundCost.Text = "+$" + illust_background_price.ToString();
            UpdateTotalPrice(); // Update the total price
        }
        private void IllustCommercialYes_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Animate Half Body to 1 and Full Body to 0.5 using a Storyboard
            AnimateOpacity(CommercialYes, 1);
            AnimateOpacity(CommercialNo, 0.5);

            commercial_fee = 2;
            commercial_use = "YES";
            UpdateTotalPrice();
        }

        private void IllustCommercialNo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Animate Half Body to 1 and Full Body to 0.5 using a Storyboard
            AnimateOpacity(CommercialNo, 1);
            AnimateOpacity(CommercialYes, 0.5);

            commercial_fee = 1;
            commercial_use = "NO";
            UpdateTotalPrice();
        }

        private void UpdateTotalPrice()
        {
            illust_total_price = (illust_nochar_price + illust_variant_price + illust_body_price + illust_background_price) * commercial_fee;
            TotalPrice.Text = "+$" + illust_total_price.ToString();
        }

        private void AnimateOpacity(Label label, double targetOpacity)
        {
            // Create a Storyboard to manage the animation
            Storyboard storyboard = new Storyboard();

            // Create the DoubleAnimation for opacity change
            DoubleAnimation opacityAnimation = new DoubleAnimation
            {
                From = label.Opacity,
                To = targetOpacity,
                Duration = new Duration(TimeSpan.FromSeconds(0.3)), // Short duration for smoothness
                AutoReverse = false
            };

            // Set the target of the animation to the Opacity property of the label
            Storyboard.SetTarget(opacityAnimation, label);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(UIElement.OpacityProperty));

            // Add the animation to the storyboard and begin it
            storyboard.Children.Add(opacityAnimation);
            storyboard.Begin();
        }
        //BODY SELECT


        //SUBMIT COMMISSION
        private int GetClientIdFromDatabase(string username)
        {
            int clientId = -1;

            try
            {
                // Define your connection string
                string connectionString = DecryptConnectionString();

                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    // Query to fetch the user's id_user (client_no) based on the username
                    string query = "SELECT id_user FROM app_users WHERE username = ?"; // Or use email if needed

                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        // Add parameters to avoid SQL injection
                        command.Parameters.AddWithValue("username", username);

                        var result = command.ExecuteScalar(); // Executes the query and gets the first value
                        if (result != null)
                        {
                            clientId = Convert.ToInt32(result); // Set clientId to the value retrieved from the database
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return clientId;
        }

        private void ConfirmSubmitButton_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // Retrieve the username of the logged-in user
                string username = UserSession.Username;

                // Get the client_no (id_user) from the database
                int clientNo = GetClientIdFromDatabase(username);

                // Retrieve user inputs 
                string commissionType = illust_body; // Example value, replace with actual input
                string background = illust_background; // Example value, replace with actual input
                int figures = 2; // Example value, replace with actual input
                int variations = 2; // Example value, replace with actual input
                string description = DescriptionInput.Text; // Example value, replace with actual input
                string commercialUse = commercial_use; // Example value, replace with actual input
                int revisions = 4; // Fixed value for revisions

                // Insert the data into commission_specification table
                string connectionString = DecryptConnectionString();
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    string query = @"INSERT INTO commissions.commission_specification (commission_type, background, figures, variations, description, commercial_use, revisions, client_no)
                                   VALUES (?, ?, ?, ?, ?, ?, ?, ?);";

                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        // Add parameters to prevent SQL injection
                        command.Parameters.AddWithValue("commission_type", commissionType);
                        command.Parameters.AddWithValue("background", background);
                        command.Parameters.AddWithValue("figures", figures);
                        command.Parameters.AddWithValue("variations", variations);
                        command.Parameters.AddWithValue("description", description);
                        command.Parameters.AddWithValue("commercial_use", commercialUse);
                        command.Parameters.AddWithValue("revisions", revisions);
                        command.Parameters.AddWithValue("client_no", clientNo); // Use the client_no (id_user)

                        // Execute the query
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Commission submitted successfully!");
                        }
                        else
                        {
                        }
                    }

                    // Insert into the client_list table
                    string clientListQuery = @"
                INSERT INTO commissions.client_list 
                (client_name, email, payment_date, commercial_use, commission_price)
                VALUES (?, ?, NULL, ?, ?, ?);";

                    using (OdbcCommand command = new OdbcCommand(clientListQuery, connection))
                    {
                        // Add parameters to prevent SQL injection
                        command.Parameters.AddWithValue("client_name", username);
                        command.Parameters.AddWithValue("email", UserSession.Email);
                        command.Parameters.AddWithValue("commercial_use", commercialUse);
                        command.Parameters.AddWithValue("commission_price", illust_total_price); // Example price, can be modified as needed
                        command.Parameters.AddWithValue("acc_id", clientNo);

                        // Execute the query for client_list
                        int rowsAffectedClientList = command.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private static string DecryptConnectionString()
        {
            try
            {
                // Get the configuration of the app.config file associated with the executable
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                // Get the connectionStrings section
                ConfigurationSection section = config.GetSection("connectionStrings");

                if (section != null && section.SectionInformation.IsProtected)
                {
                    // Temporarily decrypt the section in memory
                    section.SectionInformation.UnprotectSection();

                    // Retrieve the decrypted connection string for "CommissionsDB"
                    foreach (ConnectionStringSettings setting in config.ConnectionStrings.ConnectionStrings)
                    {
                        if (setting.Name == "CommissionsDB")
                        {
                            return setting.ConnectionString;
                        }
                    }
                }
                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        //SUBMIT COMMISSION
    }
}
