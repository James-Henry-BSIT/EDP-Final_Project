using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Configuration;
using System.Data.Odbc;
using System.Net.Http;
using System.Security.Cryptography;
using System.IO;
using Path = System.IO.Path;
using System.Globalization;
using BCrypt.Net;
using System.Runtime.InteropServices;





namespace EDP_Final_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private DispatcherTimer timer; // Timer for the countdown
        private int countdown = 30;   // Initial countdown value in seconds
        private int generatedNumber;
        private string? loggedInEmail = null;
        private string? userName = null;
        private string FindEmail;
        private bool isChangingPassword = false;
        private string? tempass = null;

        public LoginPage()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;


            // Initialize the timer
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); // Trigger every second
            timer.Tick += Timer_Tick;

            // Start the initial countdown
            StartCountdown();
            CodeInput.IsVisibleChanged += CodeInputVisibilityChanged;

        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Start the floating animation for Hex1
            ((Storyboard)FindResource("FloatingAnimation1")).Begin(Hex2, true);
            ((Storyboard)FindResource("FloatingAnimation2")).Begin(Hex2, true);
            ((Storyboard)FindResource("RedGlowOpacityAnimation")).Begin(RedGlow, true);

            LoadRememberedPassword();

        }

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




        //SIGN PAGE PAGE .CS CODE
        //LOGIN FORM LOGIC
        //LoginUsername Functions
        private void LoginUsernameTextField_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text == "USERNAME/EMAIL")
            {
                textBox.Text = string.Empty;
            }

            if (LoginUsernameTextField.Text == "USERNAME/EMAIL")
            {
                LoginUsernameTextField.Text = string.Empty;
            }
        }
        private void LoginUsernameTextField_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = "USERNAME/EMAIL";
            }
        }
        private void LoginUsernameTextField_TextChanged(object sender, TextChangedEventArgs e)
        {

            // Clear the field immediately if the user types "USERNAME/EMAIL"
            if (LoginUsernameTextField.IsFocused && LoginUsernameTextField.Text == "USERNAME/EMAIL")
            {
                LoginUsernameTextField.Text = string.Empty;
            }

            if (LoginUsernameTextField.IsFocused && LoginUsernameTextField.Text != "USERNAME/EMAIL")
            {
                ErrorLog.Visibility = Visibility.Collapsed;
            }
        }
        //LoginUsername Functions


        //Password Functions
        private bool isPasswordVisible = false;
        private void LoginPasswordBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ErrorLog.Visibility = Visibility.Collapsed;
            if (isPasswordVisible)
            {
                LoginPasswordTextField.Text = LoginPasswordBox.Password;
            }
        }
        private void LogPasswordVisToggle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isPasswordVisible = !isPasswordVisible; // Toggle visibility state
            string newImagePath = isPasswordVisible ? "images/buttons/ShowPass.png" : "images/buttons/HidePass.png";

            LogPasswordVisToggle.Source = new BitmapImage(new Uri(newImagePath, UriKind.Relative));

            if (isPasswordVisible)
            {
                // Show plain text (TextBox)
                LoginPasswordTextField.Text = LoginPasswordBox.Password;
                LoginPasswordBox.Visibility = Visibility.Hidden;
                LoginPasswordTextField.Visibility = Visibility.Visible;
                LoginPasswordPlaceholder.Visibility = Visibility.Collapsed; // Hide placeholder when text is visible
                if (string.IsNullOrEmpty(LoginPasswordBox.Password) && string.IsNullOrEmpty(LoginPasswordTextField.Text))
                {
                    LoginPasswordPlaceholder.Visibility = Visibility.Visible;
                }
            }
            else
            {
                // Show masked text (PasswordBox)
                LoginPasswordBox.Password = LoginPasswordTextField.Text;
                LoginPasswordBox.Visibility = Visibility.Visible;
                LoginPasswordTextField.Visibility = Visibility.Hidden;

                if (string.IsNullOrEmpty(LoginPasswordBox.Password) && string.IsNullOrEmpty(LoginPasswordTextField.Text))
                {
                    LoginPasswordPlaceholder.Visibility = Visibility.Visible;
                }
            }
        }
        private void LoginPasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(LoginPasswordBox.Password))
            {
                LoginPasswordPlaceholder.Visibility = Visibility.Collapsed;
            }
        }
        private void LoginPasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(LoginPasswordBox.Password))
            {
                LoginPasswordPlaceholder.Visibility = Visibility.Visible;
            }
        }
        private void LoginPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ErrorLog.Visibility = Visibility.Collapsed;
            if (!string.IsNullOrEmpty(LoginPasswordBox.Password) || !string.IsNullOrEmpty(LoginPasswordTextField.Text))
            {
                LoginPasswordPlaceholder.Visibility = Visibility.Collapsed;
            }
        }
        //Password Functions
        //TextField Placeholder functions



        //Remember Password Function
        private bool RememberPasswordisChecked = false;
        private void RememberCheckbox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Toggle the state
            RememberPasswordisChecked = !RememberPasswordisChecked;

            // Change the image source based on the toggle state
            UpdateRememberCheckboxVisual();
        }
        private void UpdateRememberCheckboxVisual()
        {
            string newImagePath = RememberPasswordisChecked ? "/images/Buttons/Checkbox4.png" : "/images/Buttons/Checkbox1.png";
            RememberCheckbox.Source = new BitmapImage(new Uri(newImagePath, UriKind.Relative));
        }

        private void RememberPassword(string email, string password)
        {
            try
            {
                if (!RememberPasswordisChecked) return;

                // Encryption key (you can generate a unique key per user for added security)
                string encryptionKey = "agv6Namc9VhjArOAJnrD7Bs2uScTM8RE"; // Replace with a secure key
                string encryptedPassword = EncryptPassword(password, encryptionKey);

                // Create a JSON object to store the email, encrypted password, and timestamp
                var data = new
                {
                    Email = email,
                    Password = encryptedPassword,
                    Timestamp = DateTime.Now.ToString("o") // ISO 8601 format
                };

                // Serialize to JSON and save to a file
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);

                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RememberedPassword.json");
                File.WriteAllText(filePath, json);
            }
            catch (Exception)
            {
            }
        }
        private void LoadRememberedPassword()
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RememberedPassword.json");
            string encryptionKey = "agv6Namc9VhjArOAJnrD7Bs2uScTM8RE"; // Match the encryption key used during storage

            // Check if the file exists
            if (!File.Exists(filePath)) return;

            try
            {
                // Read and parse the JSON file
                string json = File.ReadAllText(filePath);
                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

                // Get the timestamp from the JSON data
                string timestampString = data.Timestamp;

                // Check if the timestamp exists and is in a valid format
                if (string.IsNullOrEmpty(timestampString))
                {
                    return;
                }

                // Parse the timestamp as DateTimeOffset (ISO 8601 format)
                if (!DateTimeOffset.TryParse(timestampString, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTimeOffset timestamp))
                {
                    return;
                }

                // Check if the password is still valid (within 7 days)
                if ((DateTimeOffset.Now - timestamp).TotalDays > 7)
                {
                    // Password has expired, delete the file
                    File.Delete(filePath);
                    return;
                }

                // Decrypt the password and prefill the fields
                string email = data.Email;
                string encryptedPassword = data.Password;
                string password = DecryptPassword(encryptedPassword, encryptionKey);

                // Populate the fields
                LoginUsernameTextField.Text = email;
                LoginPasswordBox.Password = password;
                LoginPasswordTextField.Text = password;
                RememberPasswordisChecked = true;
                LoginPasswordPlaceholder.Visibility = Visibility.Collapsed;
                UpdateRememberCheckboxVisual();

            }
            catch (Exception)
            {
            }

        }
        //Remember Password Function



        //Forgot Password Function
        private void ForgotPasswordButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ForgotPasswordButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9F395D"));
        }
        private void ForgotPasswordButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ForgotPasswordButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6F1535"));
        }
        //Forgot Password Function



        //Register Now Function
        private void RegisterNowButton_MouseEnter(object sender, MouseEventArgs e)
        {
            RegisterNowButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B73560"));
        }
        private void RegisterNowButton_MouseLeave(object sender, MouseEventArgs e)
        {
            RegisterNowButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA5A3B2"));
        }
        //Register Now Function
        //LOGIN FORM LOGIC



        //REGISTER FORM LOGIC
        //RegEmail Functions
        private void RegisterEmailTextField_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text == "EMAIL")
            {
                textBox.Text = string.Empty;
            }

            if (RegEmailTextField.Text == "EMAIL")
            {
                RegEmailTextField.Text = string.Empty;
            }
        }
        private void RegisterEmailTextField_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = "EMAIL";
            }
        }
        private void RegisterEmailTextField_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Clear the field immediately if the user types "USERNAME/EMAIL"
            if (RegEmailTextField.IsFocused && RegEmailTextField.Text == "EMAIL")
            {
                RegEmailTextField.Text = string.Empty;
            }
            if (RegEmailTextField.IsFocused && RegEmailTextField.Text != "EMAIL")
            {
                ErrorReg.Visibility = Visibility.Collapsed;
            }
        }
        //RegEmail Functions

        //RegUsername Functions
        private void RegisterUsernameTextField_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text == "USERNAME")
            {
                textBox.Text = string.Empty;
            }

            if (RegUsernameTextField.Text == "USERNAME")
            {
                RegUsernameTextField.Text = string.Empty;
            }
        }
        private void RegisterUsernameTextField_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = "USERNAME";
            }
        }
        private void RegisterUsernameTextField_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Clear the field immediately if the user types "USERNAME/EMAIL"
            if (RegUsernameTextField.IsFocused && RegUsernameTextField.Text == "USERNAME")
            {
                RegUsernameTextField.Text = string.Empty;
            }
            if (RegUsernameTextField.IsFocused && RegUsernameTextField.Text != "USERNAME")
            {
                ErrorReg.Visibility = Visibility.Collapsed;
            }
        }
        //RegUsername Functions


        //RegPassword Functions
        private bool isRegPasswordVisible = false;
        private void RegPasswordBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ErrorReg.Visibility = Visibility.Collapsed;
            if (isRegPasswordVisible)
            {
                RegisterPasswordBox.Password = RegisterPasswordTextField.Text;
            }
        }
        private void RegPasswordVisToggle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isRegPasswordVisible = !isRegPasswordVisible; // Toggle visibility state
            string newImagePath = isRegPasswordVisible ? "images/buttons/ShowPass.png" : "images/buttons/HidePass.png";
            RegPasswordVisToggle.Source = new BitmapImage(new Uri(newImagePath, UriKind.Relative));

            if (isRegPasswordVisible)
            {
                // Show plain text (TextBox)
                RegisterPasswordTextField.Text = RegisterPasswordBox.Password;  // Sync text from PasswordBox
                RegisterPasswordBox.Visibility = Visibility.Hidden;
                RegisterPasswordTextField.Visibility = Visibility.Visible;
                RegisterPasswordPlaceholder.Visibility = Visibility.Collapsed; // Hide placeholder when text is visible

                // Only show placeholder if both password fields are empty
                if (string.IsNullOrEmpty(RegisterPasswordTextField.Text))
                {
                    RegisterPasswordPlaceholder.Visibility = Visibility.Visible;
                }
            }
            else
            {
                // Show masked text (PasswordBox)
                RegisterPasswordBox.Password = RegisterPasswordTextField.Text;  // Sync text to PasswordBox
                RegisterPasswordBox.Visibility = Visibility.Visible;
                RegisterPasswordTextField.Visibility = Visibility.Hidden;

                // Only show placeholder if both password fields are empty
                if (string.IsNullOrEmpty(RegisterPasswordBox.Password))
                {
                    RegisterPasswordPlaceholder.Visibility = Visibility.Visible;
                }
            }
        }
        private void RegPasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(RegisterPasswordBox.Password))
            {
                RegisterPasswordPlaceholder.Visibility = Visibility.Collapsed;
            }
        }
        private void RegisterPasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(RegisterPasswordBox.Password))
            {
                RegisterPasswordPlaceholder.Visibility = Visibility.Visible;
            }
        }
        private void RegisterPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ErrorReg.Visibility = Visibility.Collapsed;
            if (!string.IsNullOrEmpty(RegisterPasswordBox.Password) || !string.IsNullOrEmpty(RegisterPasswordTextField.Text))
            {
                RegisterPasswordPlaceholder.Visibility = Visibility.Collapsed;
            }
        }
        //RegPassword Functions



        //RegConfirmPassword Functions
        private bool isConRegPasswordVisible = false;
        private void ConRegPasswordBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ErrorReg.Visibility = Visibility.Collapsed;
            if (isConRegPasswordVisible)
            {
                ConfirmRegPasswordBox.Password = ConfirmRegPasswordTextField.Text;
            }
        }
        private void ConRegPasswordVisToggle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isConRegPasswordVisible = !isConRegPasswordVisible; // Toggle visibility state
            string newImagePath = isConRegPasswordVisible ? "images/buttons/ShowPass.png" : "images/buttons/HidePass.png";
            ConRegPasswordVisToggle.Source = new BitmapImage(new Uri(newImagePath, UriKind.Relative));

            if (isConRegPasswordVisible)
            {
                // Show plain text (TextBox)
                ConfirmRegPasswordTextField.Text = ConfirmRegPasswordBox.Password;  // Sync text from PasswordBox
                ConfirmRegPasswordBox.Visibility = Visibility.Hidden;
                ConfirmRegPasswordTextField.Visibility = Visibility.Visible;
                ConfirmRegPasswordPlaceholder.Visibility = Visibility.Collapsed; // Hide placeholder when text is visible

                // Only show placeholder if both password fields are empty
                if (string.IsNullOrEmpty(ConfirmRegPasswordTextField.Text))
                {
                    ConfirmRegPasswordPlaceholder.Visibility = Visibility.Visible;
                }
            }
            else
            {
                // Show masked text (PasswordBox)
                ConfirmRegPasswordBox.Password = ConfirmRegPasswordTextField.Text;  // Sync text to PasswordBox
                ConfirmRegPasswordBox.Visibility = Visibility.Visible;
                ConfirmRegPasswordTextField.Visibility = Visibility.Hidden;

                // Only show placeholder if both password fields are empty
                if (string.IsNullOrEmpty(ConfirmRegPasswordBox.Password))
                {
                    ConfirmRegPasswordPlaceholder.Visibility = Visibility.Visible;
                }
            }
        }
        private void ConRegPasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ConfirmRegPasswordBox.Password))
            {
                ConfirmRegPasswordPlaceholder.Visibility = Visibility.Collapsed;
            }
        }
        private void ConfirmRegPasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ConfirmRegPasswordBox.Password))
            {
                ConfirmRegPasswordPlaceholder.Visibility = Visibility.Visible;
            }
        }
        private void ConfirmRegPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ErrorReg.Visibility = Visibility.Collapsed;
            if (!string.IsNullOrEmpty(ConfirmRegPasswordBox.Password) || !string.IsNullOrEmpty(ConfirmRegPasswordTextField.Text))
            {
                ConfirmRegPasswordPlaceholder.Visibility = Visibility.Collapsed;
            }
        }
        //RegConfirmPassword Functions
        //REGISTER FORM LOGIC


        //CHANGE PASSWORD FORM LOGIC
        //Fadeout LoginFOrm Animation
        private void ForgotPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the fade-out animation
            Storyboard fadeOutLoginForm = (Storyboard)FindResource("FadeOutLoginForm");
            LoginForm.IsHitTestVisible = false;
            fadeOutLoginForm.Completed += (s, ev) =>
            {
                // Make LoginForm invisible after fade-out completes
                LoginForm.Visibility = Visibility.Collapsed;

                // Make ChangePassForm visible and start fade-in
                ChangePassForm.Visibility = Visibility.Visible;
                Storyboard fadeInChangePassForm = (Storyboard)FindResource("FadeInChangePassForm");
                fadeInChangePassForm.Completed += (s2, ev2) =>
                {
                    ChangePassForm.IsHitTestVisible = true;
                };

                fadeInChangePassForm.Begin();
            };
            fadeOutLoginForm.Begin();
            // Get the Hex1 transformation animation
            Storyboard transformHex1Storyboard = (Storyboard)FindResource("TransformHex1");

            // Start the Hex1 transformation animation
            transformHex1Storyboard.Begin();


            if (LoginUsernameTextField.Text != "USERNAME/EMAIL")
            {
                LoginUsernameTextField.Text = "USERNAME/EMAIL";
            }

            RememberPasswordisChecked = false;
            UpdateRememberCheckboxVisual();

            LoginPasswordBox.Clear();
            LoginPasswordTextField.Clear();

            // Make the RegisterForm visible
            LoginPasswordPlaceholder.Visibility = Visibility.Visible;
            NewPasswordPlaceholder.Visibility = Visibility.Visible;
            NewConfirmPasswordPlaceholder.Visibility = Visibility.Visible;
            ErrorLog.Visibility = Visibility.Collapsed;
        }
        //Fadeout LoginFOrm Animation

        //Fadeout ChangePassForm Animation
        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            isChangingPassword = false;
            // Fade out the ChangePassForm
            Storyboard fadeOutChangePassForm = (Storyboard)FindResource("FadeOutChangePassForm");
            ChangePassForm.IsHitTestVisible = false;
            fadeOutChangePassForm.Completed += (s, ev) =>
            {
                // Make ChangePassForm invisible after fade-out completes
                ChangePassForm.Visibility = Visibility.Collapsed;

                // Make LoginForm visible and start fade-in
                LoginForm.Visibility = Visibility.Visible;
                Storyboard fadeInLoginForm = (Storyboard)FindResource("FadeInLoginForm");
                fadeInLoginForm.Completed += (s2, ev2) =>
                {
                    LoginForm.IsHitTestVisible = true;
                };
                fadeInLoginForm.Begin();
            };
            fadeOutChangePassForm.Begin();

            // Reset Hex1 margin to its original position
            Storyboard hex1ResetAnimation = (Storyboard)FindResource("Hex1ResetAnimation");
            hex1ResetAnimation.Begin();

            // Clear out the input fields
            if (WhatEmailTextField.Text != "EMAIL")
            {
                WhatEmailTextField.Text = "EMAIL";
            }

            NewPasswordBox.Clear();
            NewPasswordTextField.Clear();
            NewConfirmPasswordBox.Clear();
            NewConfirmPasswordTextField.Clear();

            // Make the RegisterForm visible
            LoginPasswordPlaceholder.Visibility = Visibility.Visible;
            NewPasswordPlaceholder.Visibility = Visibility.Visible;
            NewConfirmPasswordPlaceholder.Visibility = Visibility.Visible;

            WarningNew.Visibility = Visibility.Collapsed;
        }

        private void GoBackButton_MouseEnter(object sender, MouseEventArgs e)
        {
            GoBackButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B73560"));
        }
        private void GoBackButton_MouseLeave(object sender, MouseEventArgs e)
        {
            GoBackButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA5A3B2"));
        }
        //Fadeout ChangePassForm Animation


        //WhatEmail Functions
        private void WhatEmailTextField_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text == "EMAIL")
            {
                textBox.Text = string.Empty;
            }

            if (WhatEmailTextField.Text == "EMAIL")
            {
                WhatEmailTextField.Text = string.Empty;
            }
        }
        private void WhatEmailTextField_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = "EMAIL";
            }
        }
        private void WhatEmailTextField_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Clear the field immediately if the user types "USERNAME/EMAIL"
            if (WhatEmailTextField.IsFocused && WhatEmailTextField.Text == "EMAIL")
            {
                WhatEmailTextField.Text = string.Empty;
            }
            if (WhatEmailTextField.IsFocused && WhatEmailTextField.Text != "EMAIL")
            {
                WarningNew.Visibility = Visibility.Collapsed;
            }
        }
        //WhatEmail Functions


        //RegPassword Functions
        private bool isNewPasswordVisible = false;
        private void NewPasswordBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            WarningNew.Visibility = Visibility.Collapsed;
            if (isNewPasswordVisible)
            {
                NewPasswordBox.Password = NewPasswordTextField.Text;
            }
        }
        private void NewPasswordVisToggle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isNewPasswordVisible = !isNewPasswordVisible; // Toggle visibility state
            string newImagePath = isNewPasswordVisible ? "images/buttons/ShowPass.png" : "images/buttons/HidePass.png";
            NewPasswordVisToggle.Source = new BitmapImage(new Uri(newImagePath, UriKind.Relative));

            if (isNewPasswordVisible)
            {
                // Show plain text (TextBox)
                NewPasswordTextField.Text = NewPasswordBox.Password;  // Sync text from PasswordBox
                NewPasswordBox.Visibility = Visibility.Hidden;
                NewPasswordTextField.Visibility = Visibility.Visible;
                NewPasswordPlaceholder.Visibility = Visibility.Collapsed; // Hide placeholder when text is visible

                // Only show placeholder if both password fields are empty
                if (string.IsNullOrEmpty(NewPasswordTextField.Text))
                {
                    NewPasswordPlaceholder.Visibility = Visibility.Visible;
                }
            }
            else
            {
                // Show masked text (PasswordBox)
                NewPasswordBox.Password = NewPasswordTextField.Text;  // Sync text to PasswordBox
                NewPasswordBox.Visibility = Visibility.Visible;
                NewPasswordTextField.Visibility = Visibility.Hidden;

                // Only show placeholder if both password fields are empty
                if (string.IsNullOrEmpty(NewPasswordBox.Password))
                {
                    NewPasswordPlaceholder.Visibility = Visibility.Visible;
                }
            }
        }
        private void NewPasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NewPasswordBox.Password))
            {
                NewPasswordPlaceholder.Visibility = Visibility.Collapsed;
            }
        }
        private void NewPasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NewPasswordBox.Password))
            {
                NewPasswordPlaceholder.Visibility = Visibility.Visible;
            }
        }
        private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            WarningNew.Visibility = Visibility.Collapsed;
            if (!string.IsNullOrEmpty(NewPasswordBox.Password) || !string.IsNullOrEmpty(NewPasswordTextField.Text))
            {
                NewPasswordPlaceholder.Visibility = Visibility.Collapsed;
            }
        }
        //RegPassword Functions



        //NewPassword Functions
        private bool isNewConfirmPasswordVisible = false;
        private void NewConfirmPasswordBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isNewConfirmPasswordVisible)
            {
                NewConfirmPasswordBox.Password = NewConfirmPasswordTextField.Text;
            }
        }
        private void NewConfirmPasswordVisToggle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isNewConfirmPasswordVisible = !isNewConfirmPasswordVisible; // Toggle visibility state
            string newImagePath = isNewConfirmPasswordVisible ? "images/buttons/ShowPass.png" : "images/buttons/HidePass.png";
            NewConfirmPasswordVisToggle.Source = new BitmapImage(new Uri(newImagePath, UriKind.Relative));

            if (isNewConfirmPasswordVisible)
            {
                // Show plain text (TextBox)
                NewConfirmPasswordTextField.Text = NewConfirmPasswordBox.Password;  // Sync text from PasswordBox
                NewConfirmPasswordBox.Visibility = Visibility.Hidden;
                NewConfirmPasswordTextField.Visibility = Visibility.Visible;
                NewConfirmPasswordPlaceholder.Visibility = Visibility.Collapsed; // Hide placeholder when text is visible

                // Only show placeholder if both password fields are empty
                if (string.IsNullOrEmpty(NewConfirmPasswordTextField.Text))
                {
                    NewConfirmPasswordPlaceholder.Visibility = Visibility.Visible;
                }
            }
            else
            {
                // Show masked text (PasswordBox)
                NewConfirmPasswordBox.Password = NewConfirmPasswordTextField.Text;  // Sync text to PasswordBox
                NewConfirmPasswordBox.Visibility = Visibility.Visible;
                NewConfirmPasswordTextField.Visibility = Visibility.Hidden;

                // Only show placeholder if both password fields are empty
                if (string.IsNullOrEmpty(NewConfirmPasswordBox.Password))
                {
                    NewConfirmPasswordPlaceholder.Visibility = Visibility.Visible;
                }
            }
        }
        private void NewConfirmPasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NewConfirmPasswordBox.Password))
            {
                NewConfirmPasswordPlaceholder.Visibility = Visibility.Collapsed;
            }
        }
        private void NewConfirmPasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NewConfirmPasswordBox.Password))
            {
                NewConfirmPasswordPlaceholder.Visibility = Visibility.Visible;
            }
        }
        private void NewConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(NewConfirmPasswordBox.Password) || !string.IsNullOrEmpty(NewConfirmPasswordTextField.Text))
            {
                NewConfirmPasswordPlaceholder.Visibility = Visibility.Collapsed;
            }
        }
        //NewPassword Functions


        //CHANGE PASSWORD FORM LOGIC


        //Switch Forms Function
        private void RegisterNowButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Make the LoginForm invisible (so it cannot be interacted with)
            LoginForm.Visibility = Visibility.Collapsed;

            // Clear out the input fields
            if (LoginUsernameTextField.Text != "USERNAME/EMAIL")
            {
                LoginUsernameTextField.Text = "USERNAME/EMAIL";
            }

            RememberPasswordisChecked = false;
            UpdateRememberCheckboxVisual();

            LoginPasswordBox.Clear();
            LoginPasswordTextField.Clear();

            // Make the RegisterForm visible
            RegisterForm.Visibility = Visibility.Visible;
        }
        private void SignInButton_MouseEnter(object sender, MouseEventArgs e)
        {
            SignInButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B73560"));
        }
        private void SignInButton_MouseLeave(object sender, MouseEventArgs e)
        {
            SignInButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA5A3B2"));
        }

        private void SignInRegisterNowButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Make the LoginForm invisible (so it cannot be interacted with)
            LoginForm.Visibility = Visibility.Visible;

            // Clear out the input fields
            if (RegEmailTextField.Text != "EMAIL")
            {
                RegEmailTextField.Text = "EMAIL";
            }

            if (RegUsernameTextField.Text != "USERNAME")
            {
                RegUsernameTextField.Text = "USERNAME";
            }

            RegisterPasswordBox.Clear();
            RegisterPasswordTextField.Clear();
            ConfirmRegPasswordBox.Clear();
            ConfirmRegPasswordTextField.Clear();

            // Make the RegisterForm visible
            LoginPasswordPlaceholder.Visibility = Visibility.Visible;
            ConfirmRegPasswordPlaceholder.Visibility = Visibility.Visible;
            RegisterPasswordPlaceholder.Visibility = Visibility.Visible;
            RegisterForm.Visibility = Visibility.Collapsed;
            ErrorReg.Visibility = Visibility.Collapsed;
        }
        //Switch Forms Function


        //CODE VERIFY FORM LOGIC
        //Sent Code Function
        private void ExitCode_MouseEnter(object sender, MouseEventArgs e)
        {
            ExitCode.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B73560"));
        }
        private void ExitCode_MouseLeave(object sender, MouseEventArgs e)
        {
            ExitCode.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA5A3B2"));
        }

        private void ResendCodeButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ResendCodeButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B73560"));
        }
        private void ResendCodeButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ResendCodeButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA5A3B2"));
        }

        private void GenerateRandomNumber()
        {
            Random random = new Random();
            generatedNumber = random.Next(100000, 1000000); // Generates a number between 100000 and 999999
        }


        //Resend Code to email
        private void ExitCode_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            loggedInEmail = string.Empty;
            CodeInput.IsHitTestVisible = false;
            Storyboard FadeOutCode = (Storyboard)FindResource("FadeOutCode");
            Storyboard.SetTarget(FadeOutCode, CodeInput);
            Storyboard stopblurStoryboard = (Storyboard)FindResource("StopBlurStoryboard");

            stopblurStoryboard.Begin();
            FadeOutCode.Begin();
            FadeOutCode.Completed += (s, ev) => CodeInput.Visibility = Visibility.Collapsed;
            stopblurStoryboard.Completed += (s, ev) => CodeInput.Visibility = Visibility.Collapsed;

            loggedInEmail = string.Empty;
            userName = string.Empty;

            Code1.Text = "";
            Code2.Text = "";
            Code3.Text = "";
            Code4.Text = "";
            Code5.Text = "";
            Code6.Text = "";
            tempass = "";
        }
        //Resend Code to email

        // Code input focus
        private async void Code_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (WrongCode.Visibility == Visibility.Visible)
            {
                WrongCode.Visibility = Visibility.Collapsed;
            }

            if (textBox.Text.Length == 1) // When a character is typed
            {
                // Move to the next TextBox
                if (textBox == Code1) Code2.Focus();
                else if (textBox == Code2) Code3.Focus();
                else if (textBox == Code3) Code4.Focus();
                else if (textBox == Code4) Code5.Focus();
                else if (textBox == Code5) Code6.Focus();
            }

            if (!string.IsNullOrEmpty(Code1.Text) &&
                !string.IsNullOrEmpty(Code2.Text) &&
                !string.IsNullOrEmpty(Code3.Text) &&
                !string.IsNullOrEmpty(Code4.Text) &&
                !string.IsNullOrEmpty(Code5.Text) &&
                !string.IsNullOrEmpty(Code6.Text))
            {
                int userInput = GetUserInput();

                // Compare the user input with the generated number
                if (userInput == generatedNumber)
                {
                    // Trigger the FadeOutCode storyboard
                    Storyboard fadeOutStoryboard = (Storyboard)FindResource("FadeOutCode");
                    fadeOutStoryboard.Begin();

                    Storyboard stopBlurStoryboard = (Storyboard)FindResource("StopBlurStoryboard");
                    stopBlurStoryboard.Begin();


                    if (isChangingPassword == true)
                    {
                        // Update the password if the process is for changing password
                        if (!string.IsNullOrEmpty(FindEmail)) // Ensure the email is found and valid
                        {
                            UpdatePassword(FindEmail); // Update the password
                        }
                    }


                    if (isChangingPassword == false)
                    {
                        // Update the verification status in the database asynchronously
                        if (!string.IsNullOrEmpty(loggedInEmail)) // Only update if the email is stored (meaning the user has successfully registered or logged in)
                        {
                            await UpdateVerificationStatus(loggedInEmail);  // This is now awaited
                            LoginForm.IsHitTestVisible = false;
                            RegisterForm.IsHitTestVisible = false;

                            if (RememberPasswordisChecked)
                            {
                                // Remember the password securely
                                RememberPassword(loggedInEmail, tempass);
                            }
                            UserSession.Email = loggedInEmail;
                            UserSession.Username = userName;
                            var mainWindow = (MainWindow)Application.Current.MainWindow;
                            mainWindow.NavigateToPage(new Loading());
                        }

                    }
                    Code1.Text = "";
                    Code2.Text = "";
                    Code3.Text = "";
                    Code4.Text = "";
                    Code5.Text = "";
                    Code6.Text = "";
                }
                else
                {
                    WrongCode.Visibility = Visibility.Visible;
                }
            }
        }


        private void Code_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;

            if (e.Key == Key.Back && string.IsNullOrEmpty(textBox.Text)) // Backspace when empty
            {
                // Move to the previous TextBox
                if (textBox == Code2) Code1.Focus();
                else if (textBox == Code3) Code2.Focus();
                else if (textBox == Code4) Code3.Focus();
                else if (textBox == Code5) Code4.Focus();
                else if (textBox == Code6) Code5.Focus();
            }

            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                e.Handled = false;
            }
            else if (!char.IsDigit((char)KeyInterop.VirtualKeyFromKey(e.Key)))
            {
                e.Handled = true; // Prevent non-numeric input
            }
        }
        //Code input focus

        private void StartCountdown()
        {
            countdown = 30; // Reset countdown
            ResendCodeTimer.Content = $"RESEND IN {countdown}s";
            ResendCodeTimer.Visibility = Visibility.Visible;
            ResendCodeButton.Visibility = Visibility.Hidden;

            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (countdown > 0)
            {
                countdown--;
                ResendCodeTimer.Content = $"RESEND IN {countdown}s";
            }
            else
            {
                timer.Stop();
                ResendCodeTimer.Visibility = Visibility.Hidden;
                ResendCodeButton.Visibility = Visibility.Visible;
            }
        }

        private async void ResendCodeButton_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ResendCodeButton.Visibility = Visibility.Hidden;
            await SendVerificationEmail(loggedInEmail, generatedNumber.ToString());
            StartCountdown();
            GenerateRandomNumber();
        }
        private int GetUserInput()
        {
            string input = $"{Code1.Text}{Code2.Text}{Code3.Text}{Code4.Text}{Code5.Text}{Code6.Text}";
            return int.TryParse(input, out int result) ? result : -1; // Returns -1 if input is invalid
        }
        //Sent Code Function

        private void TriggerBlurEffect()
        {
            // Find the Storyboard resource
            Storyboard blurStoryboard = (Storyboard)FindResource("BlurStoryboard");

            if (blurStoryboard != null)
            {
                // Start the Storyboard
                blurStoryboard.Begin(this);
            }
        }

        private void CodeInputVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Check if CodeInput is now visible
            if (CodeInput.Visibility == Visibility.Visible)
            {
                TriggerBlurEffect();
            }
        }
        //CODE VERIFY FORM LOGIC

        //SQL LOGIC
        //SQL REGISTER LOGIC
        private async Task RegisterUser()
        {
            try
            {
                if (RegEmailTextField == null || RegUsernameTextField == null || RegisterPasswordBox == null || ConfirmRegPasswordBox == null)
                {
                    throw new Exception("One or more required controls are not initialized.");
                }

                string email = RegEmailTextField.Text.Trim();
                string username = RegUsernameTextField.Text.Trim();
                string password = RegisterPasswordBox.Password.Trim();
                string confirmPassword = ConfirmRegPasswordBox.Password.Trim();
                DateTime creationDate = DateTime.Now;
                string verificationStatus = "unverified";

                // **Validation for Invalid Inputs**
                if (email.Equals("EMAIL", StringComparison.OrdinalIgnoreCase) ||
                    !email.Contains("@") ||                // Check for email domain
                    !(email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase) ||
                      email.EndsWith("@yahoo.com", StringComparison.OrdinalIgnoreCase)) ||
                    username.Equals("USERNAME", StringComparison.OrdinalIgnoreCase) ||
                    password.Equals("PASSWORD", StringComparison.OrdinalIgnoreCase) ||
                    confirmPassword.Equals("CONFIRM PASSWORD", StringComparison.OrdinalIgnoreCase))
                {
                    ErrorReg.Content = "Invalid entries, please check your inputs again";
                    ErrorReg.Visibility = Visibility.Visible;
                    return;
                }

                // **Validation for Password Mismatch**
                if (password != confirmPassword)
                {
                    ErrorReg.Content = "Password confirmation mismatch";
                    ErrorReg.Visibility = Visibility.Visible;
                    return;
                }
                string connectionString = DecryptConnectionString();
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    // Check if the email or username already exists
                    string checkQuery = "SELECT COUNT(*) FROM app_users WHERE email = ? OR username = ?";
                    using (var checkCommand = new OdbcCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.Add(new OdbcParameter { Value = email, OdbcType = OdbcType.VarChar });
                        checkCommand.Parameters.Add(new OdbcParameter { Value = username, OdbcType = OdbcType.VarChar });

                        int count = Convert.ToInt32(checkCommand.ExecuteScalar());
                        if (count > 0)
                        {
                            ErrorReg.Content = "User already exists";
                            ErrorReg.Visibility = Visibility.Visible;
                            return;
                        }
                    }

                    // If no existing user, proceed with the registration
                    string insertQuery = "INSERT INTO app_users (email, username, user_password, creation_date, verification) VALUES (?, ?, ?, ?, ?)";
                    using (var insertCommand = new OdbcCommand(insertQuery, connection))
                    {
                        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                        insertCommand.Parameters.Add(new OdbcParameter { Value = email, OdbcType = OdbcType.VarChar });
                        insertCommand.Parameters.Add(new OdbcParameter { Value = username, OdbcType = OdbcType.VarChar });
                        insertCommand.Parameters.Add(new OdbcParameter { Value = hashedPassword, OdbcType = OdbcType.VarChar });
                        insertCommand.Parameters.Add(new OdbcParameter { Value = creationDate, OdbcType = OdbcType.DateTime });
                        insertCommand.Parameters.Add(new OdbcParameter { Value = verificationStatus, OdbcType = OdbcType.VarChar });

                        int rowsAffected = insertCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            ErrorReg.Content = "Registration successful!";
                            ErrorReg.Visibility = Visibility.Visible;

                            loggedInEmail = email;
                            userName = username;

                            GenerateRandomNumber();
                            StartCountdown();

                            Code1.Focus();

                            Storyboard FadeInCode = (Storyboard)FindResource("FadeInCode");
                            CodeInput.Visibility = Visibility.Visible;
                            Storyboard.SetTarget(FadeInCode, CodeInput);
                            FadeInCode.Begin();
                            FadeInCode.Completed += (s2, ev2) =>
                            {
                                CodeInput.IsHitTestVisible = true;
                            };

                            // Blur effect
                            Storyboard blurStoryboard = (Storyboard)FindResource("BlurStoryboard");
                            blurStoryboard.Begin();
                            blurStoryboard.Completed += (s1, ev1) =>
                            {
                                CodeInput.IsHitTestVisible = true;
                            };


                            await SendVerificationEmail(email, generatedNumber.ToString());
                        }
                        else
                        {
                            ErrorReg.Content = "Registration failed!";
                            ErrorReg.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
            catch (Exception)
            {
                ErrorReg.Content = "No Connection";
                ErrorReg.Visibility = Visibility.Visible;
            }
        }
        //SQL REGISTER LOGIC

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            _ = RegisterUser();
            CodeInput.IsHitTestVisible = true;
        }
        //SQL REGISTER LOGIC

        //SQL LOGIN LOGIC
        private async Task LoginUser()
        {
            try
            {
                if (LoginUsernameTextField == null || LoginPasswordBox == null)
                {
                    throw new Exception("One or more required controls are not initialized.");
                }

                string loginUsername = LoginUsernameTextField.Text.Trim();
                string loginPassword = LoginPasswordBox.Password.Trim();

                // Check if username/email and password are provided
                if (string.IsNullOrEmpty(loginUsername) || string.IsNullOrEmpty(loginPassword))
                {
                    ErrorLog.Content = "Please enter both username/email and password.";
                    ErrorLog.Visibility = Visibility.Visible;
                    return;
                }

                string connectionString = DecryptConnectionString();
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    // Check if the username or email exists and fetch their verification status and password
                    string checkQuery = "SELECT email, username, user_password, verification FROM app_users WHERE email = ? OR username = ?";
                    using (var checkCommand = new OdbcCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.Add(new OdbcParameter { Value = loginUsername, OdbcType = OdbcType.VarChar });
                        checkCommand.Parameters.Add(new OdbcParameter { Value = loginUsername, OdbcType = OdbcType.VarChar });

                        using (OdbcDataReader reader = checkCommand.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read(); // Read the result

                                string storedEmail = reader["email"].ToString();
                                string storedUsername = reader["username"].ToString();
                                string storedPasswordHash = reader["user_password"].ToString();
                                string verificationStatus = reader["verification"].ToString();

                                // Check if verification status is "Verified"
                                if (verificationStatus.Equals("Verified", StringComparison.OrdinalIgnoreCase))
                                {
                                    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginPassword, storedPasswordHash);
                                    // Check if the password matches
                                    if (isPasswordValid)
                                    {
                                        // If everything matches, proceed with login
                                        ErrorLog.Content = "Sign In successful!";
                                        ErrorLog.Visibility = Visibility.Visible;

                                        loggedInEmail = storedEmail;
                                        userName = storedUsername;

                                        // Generate the verification number and send email
                                        GenerateRandomNumber();
                                        StartCountdown();

                                        Code1.Focus();
                                        // If the login was done via username, use the stored email for verification
                                        string emailToUse = storedEmail;

                                        tempass = loginPassword;
                                        // Fade out and show verification code input
                                        Storyboard FadeInCode = (Storyboard)FindResource("FadeInCode");
                                        CodeInput.Visibility = Visibility.Visible;
                                        Storyboard.SetTarget(FadeInCode, CodeInput);
                                        FadeInCode.Begin();
                                        FadeInCode.Completed += (s2, ev2) =>
                                        {
                                            CodeInput.IsHitTestVisible = true;
                                        };

                                        // Blur effect
                                        Storyboard blurStoryboard = (Storyboard)FindResource("BlurStoryboard");
                                        blurStoryboard.Begin();
                                        blurStoryboard.Completed += (s1, ev1) =>
                                        {
                                            CodeInput.IsHitTestVisible = true;
                                        };


                                        await SendVerificationEmail(emailToUse, generatedNumber.ToString());
                                        // Send the verification email after successful login

                                    }
                                    else
                                    {
                                        ErrorLog.Content = "Wrong credentials, please try again!";
                                        ErrorLog.Visibility = Visibility.Visible;
                                    }
                                }
                                else
                                {
                                    ErrorLog.Content = "Account not verified";
                                    ErrorLog.Visibility = Visibility.Visible;
                                }
                            }
                            else
                            {
                                ErrorLog.Content = "Wrong credentials, please try again!";
                                ErrorLog.Visibility = Visibility.Visible;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                ErrorLog.Content = "No Connection";
                ErrorLog.Visibility = Visibility.Visible;
            }
        }

        //SQL LOGIN LOGIC

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            _ = LoginUser();
            CodeInput.IsHitTestVisible = true;
        }
        //SQL REGISTER LOGIC


        //SQL CHANGE PASSWORD LOGIC
        private async Task ChangePassword()
        {
            try
            {
                if (WhatEmailTextField == null || NewConfirmPasswordBox == null)
                {
                    throw new Exception("One or more required controls are not initialized.");
                }

                string email = WhatEmailTextField.Text.Trim();
                string newPassword = NewConfirmPasswordBox.Password.Trim();

                // Check if the email exists in the app_users table
                string connectionString = DecryptConnectionString();
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    // Query to check if the email exists and if the Verification status is valid
                    string checkQuery = "SELECT email, verification FROM app_users WHERE email = ?";
                    using (var checkCommand = new OdbcCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.Add(new OdbcParameter { Value = email, OdbcType = OdbcType.VarChar });

                        using (OdbcDataReader reader = checkCommand.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read(); // Read the result

                                string verificationStatus = reader["verification"].ToString();

                                // Check if the verification status is either "Verified" or "Unverified"
                                if (verificationStatus.Equals("Verified", StringComparison.OrdinalIgnoreCase) || verificationStatus.Equals("Unverified", StringComparison.OrdinalIgnoreCase))
                                {
                                    // Store the email temporarily for later use
                                    FindEmail = email;

                                    isChangingPassword = true;

                                    // Generate the verification code
                                    GenerateRandomNumber();
                                    StartCountdown();

                                    Code1.Focus();
                                    // Fade out and show verification code input
                                    Storyboard FadeInCode = (Storyboard)FindResource("FadeInCode");
                                    CodeInput.Visibility = Visibility.Visible;
                                    Storyboard.SetTarget(FadeInCode, CodeInput);
                                    FadeInCode.Begin();
                                    FadeInCode.Completed += (s2, ev2) =>
                                    {
                                        CodeInput.IsHitTestVisible = true;
                                    };

                                    // Blur effect
                                    Storyboard blurStoryboard = (Storyboard)FindResource("BlurStoryboard");
                                    blurStoryboard.Begin();
                                    blurStoryboard.Completed += (s1, ev1) =>
                                    {
                                        CodeInput.IsHitTestVisible = true;
                                    };


                                    // Send the verification email
                                    await SendVerificationEmail(FindEmail, generatedNumber.ToString());
                                }
                                else
                                {
                                    WarningNew.Content = "Email is not found";
                                    WarningNew.Visibility = Visibility.Visible;
                                }
                            }
                            else
                            {
                                // Email not found
                                WarningNew.Content = "Email is not found";
                                WarningNew.Visibility = Visibility.Visible;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WarningNew.Content = $"Error: {ex.Message}";
                WarningNew.Visibility = Visibility.Visible;
            }
        }
        //SQL CHANGE PASSWORD LOGIC

        //SQL UPDATE PASSWORD FUNCTION
        private void UpdatePassword(string email)
        {
            isChangingPassword = false;
            try
            {
                if (NewConfirmPasswordBox == null)
                {
                    throw new Exception("Password input field is not initialized.");
                }

                string newPassword = NewConfirmPasswordBox.Password.Trim();

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

                string connectionString = DecryptConnectionString();
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    // Update the user's password in the database
                    string updateQuery = "UPDATE app_users SET user_password = ? WHERE email = ?";
                    using (var updateCommand = new OdbcCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.Add(new OdbcParameter { Value = hashedPassword, OdbcType = OdbcType.VarChar });
                        updateCommand.Parameters.Add(new OdbcParameter { Value = email, OdbcType = OdbcType.VarChar });

                        int rowsAffected = updateCommand.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            WarningNew.Visibility = Visibility.Visible;
                            WarningNew.Content = "Password successfully updated.";
                        }
                        else
                        {
                            WarningNew.Visibility = Visibility.Visible;
                            WarningNew.Content = "Error updating password.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WarningNew.Visibility = Visibility.Visible;
                WarningNew.Content = "Error updating password: {ex.Message}";
            }
        }
        //SQL UPDATE PASSWORD FUNCTION

        private void ConfirmPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            _ = ChangePassword();
            CodeInput.IsHitTestVisible = true;
        }
        //SQL CHANGE PASSWORD LOGIC

        //SQL CODE SEND LOGIC
        private async Task SendVerificationEmail(string email, string generatedNumber)
        {
            try
            {
                string url = "https://clickteam-host.vercel.app/send.php"; // Your PHP endpoint

                using (HttpClient client = new HttpClient())
                {
                    // Create the POST data
                    var postData = new Dictionary<string, string>
            {
                { "email", email },
                { "subject", "Verification Code" },
                { "code", generatedNumber }
            };

                    // Send the POST request
                    var content = new FormUrlEncodedContent(postData);
                    var response = await client.PostAsync(url, content);

                }
            }
            catch (Exception)
            {
                WrongCode.Content = "An error occurred while sending email";
                WrongCode.Visibility = Visibility.Visible;
            }
        }


        //VERIFY EMAIL FUNCTION
        private async Task UpdateVerificationStatus(string email)
        {
            try
            {
                string connectionString = DecryptConnectionString();
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    // Update the verification status of the user to "Verified"
                    string updateQuery = "UPDATE app_users SET verification = ? WHERE email = ?";
                    using (var updateCommand = new OdbcCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.Add(new OdbcParameter { Value = "Verified", OdbcType = OdbcType.VarChar });
                        updateCommand.Parameters.Add(new OdbcParameter { Value = email, OdbcType = OdbcType.VarChar });

                        int rowsAffected = updateCommand.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            // Optionally, handle further logic here after successfully updating the verification status
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        //VERIFY EMAIL FUNCTION
        //SQL CODE SEND LOGIC

        //SQL LOGIC

        //ENCRYPTION DECRYPIION LOGIC
        private string EncryptPassword(string plainText, string key)
        {
            try
            {
                if (key.Length != 16 && key.Length != 24 && key.Length != 32)
                {
                    throw new ArgumentException("Encryption key must be 16, 24, or 32 characters long.");
                }

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(key);
                    aes.IV = new byte[16]; // Use an empty IV for simplicity; ideally, use a unique IV per user

                    using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    {
                        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                        byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                        return Convert.ToBase64String(encryptedBytes);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string DecryptPassword(string encryptedText, string key)
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(key);
                    aes.IV = new byte[16]; // Match the IV used during encryption

                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    {
                        byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                        byte[] plainBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                        return Encoding.UTF8.GetString(plainBytes);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        //RETRIEVE AND SAVE FUNCTION
        //SIGN PAGE PAGE .CS CODE

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


    }

}