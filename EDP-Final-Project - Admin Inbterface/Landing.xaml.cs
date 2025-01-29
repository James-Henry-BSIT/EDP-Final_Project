using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xml;
using OfficeOpenXml;
using static EDP_Final_Project.Landing;

namespace EDP_Final_Project
{
    public partial class Landing : Page
    {
        // Observable collection for binding commissions to UI
        public ObservableCollection<Commission> commissions { get; set; }

        public Landing()
        {
            InitializeComponent();
            LandingUsername();

            commissions = new ObservableCollection<Commission>();
            UserCommissionTabs.DataContext = commissions; // Bind the ItemsControl to the collection
            LoadCommissions(); // Load data into the collection
        }

        public ObservableCollection<Commission> Commissions
        {
            get { return commissions; }
            set { commissions = value; }
        }

        // Current page for pagination
        private int currentPage = 1;

        // Commission class for data representation
        public class Commission
        {
            public int CommissionId { get; set; }
            public string ClientName { get; set; }
            public int ClientNo { get; set; }
            public string CommissionType { get; set; }
            public string Background { get; set; }
            public int Figures { get; set; }
            public int Variations { get; set; }
            public string CommercialUse { get; set; }
            public int Revisions { get; set; }
            public string PaymentDate { get; set; }
            public decimal CommissionPrice { get; set; }
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
            AddSelectBG.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(65, 56, 77)); // Equivalent to #FF41384D
        }

        private void AddMenuSelect_MouseLeave(object sender, MouseEventArgs e)
        {
            // Revert the background color when hover ends using Color.FromRgb
            AddSelectBG.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(54, 46, 66)); // Equivalent to #FF362E42
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


        // Load commissions for the current page
        private void LoadCommissions()
        {
            try
            {
                List<Commission> fetchedCommissions = GetCommissionsFromDatabase(currentPage);
                if (fetchedCommissions.Count == 0 && currentPage > 1)
                {
                    currentPage--;
                }
                else
                {
                    BindDataToUI(fetchedCommissions);
                }
            }
            catch (Exception ex)
            {
            }
        }

        // Bind fetched data to the UI
        private void BindDataToUI(List<Commission> fetchedCommissions)
        {
            commissions.Clear(); // Clear existing data
            foreach (var commission in fetchedCommissions)
            {
                commissions.Add(commission); // Add new data to ObservableCollection
            }
        }
        // Bind fetched data to the UI


        // Fetch commissions from the database
        private List<Commission> GetCommissionsFromDatabase(int page)
        {
            List<Commission> commissionList = new List<Commission>();
            try
            {
                string connectionString = DecryptConnectionString();
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    string query = @"SELECT cs.commission_id, cs.commission_type, cs.background, cs.figures, cs.variations, cs.commercial_use, 
                                    cs.revisions, cs.client_no, cl.client_name, cl.payment_date, cl.commission_price
                             FROM commissions.commission_specification cs
                             JOIN commissions.client_list cl ON cs.client_no = cl.id
                             LIMIT 3 OFFSET ?;";

                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("offset", (page - 1) * 3);

                        using (OdbcDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                            }

                            while (reader.Read())
                            {

                                commissionList.Add(new Commission
                                {
                                    CommissionId = Convert.ToInt32(reader["commission_id"]),
                                    ClientName = reader["client_name"].ToString(),
                                    ClientNo = Convert.ToInt32(reader["client_no"]),
                                    CommissionType = ConvertCommissionType(reader["commission_type"].ToString()),
                                    Background = reader["background"].ToString(),
                                    Figures = Convert.ToInt32(reader["figures"]),
                                    Variations = Convert.ToInt32(reader["variations"]),
                                    CommercialUse = reader["commercial_use"].ToString(),
                                    Revisions = Convert.ToInt32(reader["revisions"]),
                                    PaymentDate = reader["payment_date"] == DBNull.Value ? "N/A" : Convert.ToDateTime(reader["payment_date"]).ToString("MM-dd-yyyy"),
                                    CommissionPrice = Convert.ToDecimal(reader["commission_price"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching commissions: {ex.Message}");
            }

            return commissionList;
        }


        // Convert commission type for display
        private string ConvertCommissionType(string type)
        {
            return type switch
            {
                "Full_Body" => "Full Body",
                "Half_Body" => "Half Body",
                _ => type
            };
        }


        // Handle "Next" button click for pagination
        private void NextListButton_Click(object sender, RoutedEventArgs e)
        {
            currentPage++;
            LoadCommissions();
        }

        // Handle "Previous" button click for pagination
        private void PreviousListButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                LoadCommissions();
            }
        }

        // Log out the user
        private void LogOutOption_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            UserSession.Username = null;
            UserSession.Email = null;

            NavigationService.Navigate(new Loading(isLogout: true));
        }

        //PAID COMMISSION
        private void OnPaidButtonClick(object sender, MouseButtonEventArgs e)
        {
            // Get the bound data context for this row
            var commission = (sender as FrameworkElement)?.DataContext as Commission;

            if (commission == null)
                return;

            // Extract necessary details
            int commissionId = commission.CommissionId;
            string clientName = commission.ClientName;
            int clientNo = commission.ClientNo;
            string commissionType = commission.CommissionType;
            decimal commissionPrice = commission.CommissionPrice;
            int option = 0;


            // Update database
            UpdateClientHistory(clientName, commissionType, commissionPrice, commissionId, clientNo, option);
        }

        private void DeleteCommission(object sender, MouseButtonEventArgs e)
        {
            // Get the bound data context for this row
            var commission = (sender as FrameworkElement)?.DataContext as Commission;

            if (commission == null)
                return;

            // Extract necessary details
            int commissionId = commission.CommissionId;
            string clientName = commission.ClientName;
            int clientNo = commission.ClientNo;
            string commissionType = commission.CommissionType;
            decimal commissionPrice = commission.CommissionPrice;
            int option = 1;


            // Update database
            UpdateClientHistory(clientName, commissionType, commissionPrice, commissionId, clientNo, option);
        }

        private void UpdateClientHistory(string clientName, string commissionType, decimal commissionPrice, int commissionId, int clientNo, int option)
        {
            string connectionString = DecryptConnectionString();
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                connection.Open();

                // Step 1: Check if the client exists in client_history
                string checkQuery = "SELECT COUNT(*) FROM client_history WHERE client_name = ?";
                int userExists;

                using (OdbcCommand checkCommand = new OdbcCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@ClientName", clientName);
                    userExists = Convert.ToInt32(checkCommand.ExecuteScalar());
                }

                if (option == 0)
                {
                    if (userExists > 0)
                    {
                        // Step 2a: Update the existing record
                        string updateQuery = @"UPDATE client_history SET total_payment = total_payment + ?, full_illustrations = full_illustrations
                                           + (CASE WHEN ? = 'Full Body' THEN 1 ELSE 0 END), half_illustrations = half_illustrations +
                                           (CASE WHEN ? = 'Half Body' THEN 1 ELSE 0 END) WHERE client_name = ?";
                        using (var updateCommand = new OdbcCommand(updateQuery, connection))
                        {
                            updateCommand.Parameters.AddWithValue("@CommissionPrice", commissionPrice);
                            updateCommand.Parameters.AddWithValue("@CommissionTypeFull", commissionType);
                            updateCommand.Parameters.AddWithValue("@CommissionTypeHalf", commissionType);
                            updateCommand.Parameters.AddWithValue("@ClientName", clientName);
                            updateCommand.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // Step 2b: Insert a new record
                        string insertQuery = @"INSERT INTO client_history (client_name, total_payment, full_illustrations, half_illustrations)
                                           VALUES (?, ?, (CASE WHEN ? = 'Full Body' THEN 1 ELSE 0 END), (CASE WHEN ? = 'Half Body' THEN 1
                                           ELSE 0 END))";
                        using (var insertCommand = new OdbcCommand(insertQuery, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@ClientName", clientName);
                            insertCommand.Parameters.AddWithValue("@CommissionPrice", commissionPrice);
                            insertCommand.Parameters.AddWithValue("@CommissionTypeFull", commissionType);
                            insertCommand.Parameters.AddWithValue("@CommissionTypeHalf", commissionType);
                            insertCommand.ExecuteNonQuery();
                        }
                    }


                    // Update the payment_date column in the client_list table
                    string updatePaymentDateQuery = @"UPDATE client_list 
                                  SET payment_date = ? 
                                  WHERE client_name = ?";
                    using (var updatePaymentDateCommand = new OdbcCommand(updatePaymentDateQuery, connection))
                    {
                        updatePaymentDateCommand.Parameters.AddWithValue("@PaymentDate", DateTime.Now.Date);
                        updatePaymentDateCommand.Parameters.AddWithValue("@ClientName", clientName);
                        updatePaymentDateCommand.ExecuteNonQuery();
                    }
                }
                // Step 3: Delete commission rows based on conditions
                try
                {
                    // Step 1: Delete the specific commission by Commission_id
                    string deleteSpecificCommissionQuery = @"DELETE FROM commission_specification WHERE Commission_id = ?";
                    int rowsAffected;
                    using (var deleteSpecificCommissionCommand = new OdbcCommand(deleteSpecificCommissionQuery, connection))
                    {
                        deleteSpecificCommissionCommand.Parameters.AddWithValue("@CommissionId", commissionId);

                        rowsAffected = deleteSpecificCommissionCommand.ExecuteNonQuery();
                    }

                    // Debugging: Check if the commission was deleted
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Specific commission successfully deleted!");
                    }
                    else
                    {
                        MessageBox.Show("No commission was deleted. Verify the Commission_id.");
                        return;
                    }

                    // Step 2: Check if there are any remaining commissions for the client
                    string checkRemainingQuery = @"SELECT COUNT(*) FROM commission_specification WHERE client_no = (
                                        SELECT client_no FROM commission_specification WHERE client_no = ? 
                                        LIMIT 1) AND client_no IS NOT NULL;";
                    int remainingCommissions;
                    using (var checkRemainingCommand = new OdbcCommand(checkRemainingQuery, connection))
                    {
                        checkRemainingCommand.Parameters.AddWithValue("@client_no", clientNo);
                        remainingCommissions = Convert.ToInt32(checkRemainingCommand.ExecuteScalar());
                    }

                    // Debugging: Check the number of remaining commissions
                    MessageBox.Show($"Remaining commissions for the client: {remainingCommissions}");

                    // Step 3: Delete the client from client_list if no commissions remain
                    if (remainingCommissions == 0)
                    {
                        string deleteClientListQuery = "DELETE FROM client_list WHERE client_name = ?";

                        using (var deleteClientListCommand = new OdbcCommand(deleteClientListQuery, connection))
                        {
                            deleteClientListCommand.Parameters.AddWithValue("@ClientName", clientName);
                            deleteClientListCommand.ExecuteNonQuery();
                        }
                        MessageBox.Show("Client successfully removed as they have no remaining commissions!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }

            }

        }
        //PAID COMMISSION

        private void GenerateSpreadsheet(object sender, MouseButtonEventArgs e)
        {
            string templatePath = @"C:\Users\SC136\Documents\3rd Year Outputs\EDP\EDP-Final-Project - Admin Inbterface\CommissionSummary_Template.xlsx"; // Path to the template file

            string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

            // Generate a unique file name
            string savePath = Path.Combine(downloadsPath, $"CommissionSummary_{DateTime.Now:yyyyMMddHHmmss}.xlsx");

            try
            {
                // Load data for the current month
                var commissions = GetMonthlyCommissions();

                if (commissions == null || commissions.Count == 0)
                {
                    MessageBox.Show("No commission data found for the current month.");
                    return;
                }

                // Load the template
                using (var package = new ExcelPackage(new FileInfo(templatePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Assuming data is in the first worksheet

                    int rowStart = 3; // Start appending from row 3
                    decimal monthlyEarnings = 0;

                    // Populate rows with commission data
                    foreach (var commission in commissions)
                    {
                        worksheet.Cells[rowStart, 1].Value = commission.ClientName;
                        worksheet.Cells[rowStart, 2].Value = commission.CommissionType;
                        worksheet.Cells[rowStart, 3].Value = commission.PaymentDate != "N/A"
                            ? DateTime.Parse(commission.PaymentDate).ToString("yyyy-MM-dd")
                            : "N/A";
                        worksheet.Cells[rowStart, 4].Value = commission.CommissionPrice;

                        // Preserve cell formatting
                        worksheet.Cells[rowStart, 1, rowStart, 3].StyleID = worksheet.Cells[3, 1, 3, 4].StyleID;
                        worksheet.Cells[rowStart, 4].StyleID = worksheet.Cells[3, 1, 3, 4].StyleID;
                        monthlyEarnings += commission.CommissionPrice;
                        rowStart++;
                    }

                    // Preserve "Month Total Earnings" row style
                    worksheet.Cells[rowStart, 1].Value = "Month Total Earnings";
                    worksheet.Cells[rowStart, 1].Style.Font.Bold = true;
                    worksheet.Cells[rowStart, 1].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                    worksheet.Cells[rowStart, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[rowStart, 1].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#13ED90"));

                    worksheet.Cells[rowStart, 4].Value = monthlyEarnings;
                    worksheet.Cells[rowStart, 4].Style.Numberformat.Format = "#,##0.00";
                    // Apply styling for B to D (2nd to 4th cells)
                    worksheet.Cells[rowStart, 2, rowStart, 4].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                    worksheet.Cells[rowStart, 2, rowStart, 4].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[rowStart, 2, rowStart, 4].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#C6EFCE")); // Copy green row style

                    rowStart++;
                    worksheet.Cells[rowStart, 1].Value = "Spreadsheet Date";
                    worksheet.Cells[rowStart, 1].Style.Font.Bold = true;
                    worksheet.Cells[rowStart, 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
                    worksheet.Cells[rowStart, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[rowStart, 1].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#595959"));

                    worksheet.Cells[rowStart, 4].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    // Apply styling for B to D (2nd to 4th cells)
                    worksheet.Cells[rowStart, 2, rowStart, 4].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                    worksheet.Cells[rowStart, 2, rowStart, 4].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[rowStart, 2, rowStart, 4].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D9D9D9"));




                    var commissions1 = GetMonthlyCommissions();

                    if (commissions1 == null || commissions1.Count == 0)
                    {
                        MessageBox.Show("No commission data found for the current month.");
                        return;
                    }

                    // Load data for client_history
                    var clientHistory = GetClientHistory();

                    var worksheet3 = package.Workbook.Worksheets[1]; // Assuming data is in the second worksheet



                    int clientHistoryRowStart = 3;

                    foreach (var client in clientHistory)
                    {
                        worksheet3.Cells[clientHistoryRowStart, 1].Value = client.ClientName;
                        worksheet3.Cells[clientHistoryRowStart, 2].Value = client.FullIllustrations;
                        worksheet3.Cells[clientHistoryRowStart, 3].Value = client.HalfIllustrations;
                        worksheet3.Cells[clientHistoryRowStart, 4].Value = client.TotalPayment;
                        worksheet3.Cells[clientHistoryRowStart, 1, clientHistoryRowStart, 4].StyleID = worksheet3.Cells[3, 1, 3, 4].StyleID;

                        clientHistoryRowStart++;
                    }

                    worksheet3.Cells[clientHistoryRowStart, 1].Value = "Spreadsheet Date";
                    worksheet3.Cells[clientHistoryRowStart, 1].Style.Font.Bold = true;
                    worksheet3.Cells[clientHistoryRowStart, 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
                    worksheet3.Cells[clientHistoryRowStart, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet3.Cells[clientHistoryRowStart, 1].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#595959"));

                    worksheet3.Cells[clientHistoryRowStart, 4].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    // Apply styling for B to D (2nd to 4th cells)
                    worksheet3.Cells[clientHistoryRowStart, 2, clientHistoryRowStart, 4].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                    worksheet3.Cells[clientHistoryRowStart, 2, clientHistoryRowStart, 4].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet3.Cells[clientHistoryRowStart, 2, clientHistoryRowStart, 4].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D9D9D9"));



                    var worksheet2 = package.Workbook.Worksheets.Add("Earnings Chart");

                    // Set chart title
                    worksheet2.Cells[1, 1].Value = "Monthly Earnings Chart";
                    worksheet2.Cells[1, 1].Style.Font.Bold = true;

                    // Copy earnings data to the second sheet
                    worksheet2.Cells[3, 1].Value = "Date";
                    worksheet2.Cells[3, 2].Value = "Earnings";

                    int chartRowStart = 4, dataRow = 3;
                    foreach (var commission in commissions)
                    {
                        if (commission.PaymentDate != "N/A")
                        {
                            worksheet2.Cells[chartRowStart, 1].Value = DateTime.Parse(commission.PaymentDate).ToString("yyyy-MM-dd");
                            worksheet2.Cells[chartRowStart, 2].Value = commission.CommissionPrice;
                            chartRowStart++;
                        }
                    }

                    // Create a chart
                    var chart = worksheet2.Drawings.AddChart("EarningsChart", OfficeOpenXml.Drawing.Chart.eChartType.Line);
                    chart.Title.Text = "Monthly Earnings Trend";
                    chart.SetPosition(1, 0, 4, 0); // Position chart
                    chart.SetSize(600, 400); // Chart size

                    // Set chart data range
                    var series = chart.Series.Add(worksheet2.Cells[4, 2, chartRowStart - 1, 2], worksheet2.Cells[4, 1, chartRowStart - 1, 1]);
                    series.Header = "Earnings";

                    chart.XAxis.Title.Text = "Date";
                    chart.YAxis.Title.Text = "Earnings ($)";
                    // Save the filled spreadsheet

                        // Save the filled spreadsheet
                        package.SaveAs(new FileInfo(savePath));
                    }

                MessageBox.Show($"Spreadsheet generated successfully! at Downloads folder");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating spreadsheet: {ex.Message}");
            }
        }

        private List<Commission> GetMonthlyCommissions()
        {
            List<Commission> commissionList = new List<Commission>();
            try
            {
                string connectionString = DecryptConnectionString();
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    string query = @"SELECT cs.commission_id, cs.commission_type, cs.background, cs.figures, cs.variations, cs.commercial_use, 
                                cs.revisions, cl.client_name, cl.payment_date, cl.commission_price
                         FROM commissions.commission_specification cs
                         JOIN commissions.client_list cl ON cs.client_no = cl.id
                         WHERE MONTH(cl.payment_date) = ? AND YEAR(cl.payment_date) = ?";

                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("month", DateTime.Now.Month);
                        command.Parameters.AddWithValue("year", DateTime.Now.Year);

                        using (OdbcDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                commissionList.Add(new Commission
                                {
                                    CommissionId = Convert.ToInt32(reader["commission_id"]),
                                    ClientName = reader["client_name"].ToString(),
                                    CommissionType = reader["commission_type"].ToString(),
                                    PaymentDate = reader["payment_date"] == DBNull.Value
                                        ? "N/A"
                                        : Convert.ToDateTime(reader["payment_date"]).ToString("yyyy-MM-dd"),
                                    CommissionPrice = reader["commission_price"] != DBNull.Value
                                        ? Convert.ToDecimal(reader["commission_price"])
                                        : 0,
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching commissions: {ex.Message}");
            }

            return commissionList;
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
                            // Show the decrypted connection string in a MessageBox

                            return setting.ConnectionString;
                        }
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                // Show an error message if something goes wrong
                MessageBox.Show("Error decrypting connection string: " + ex.Message, "Decryption Error");
                return string.Empty;
            }
        }








        // Method to fetch client history data from the database
        private List<ClientHistory> GetClientHistory()
        {
            List<ClientHistory> clientHistoryList = new List<ClientHistory>();

            try
            {
                string connectionString = DecryptConnectionString();
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    string query = @"SELECT client_name, full_illustrations, half_illustrations, total_payment
                             FROM client_history";

                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    using (OdbcDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            clientHistoryList.Add(new ClientHistory
                            {
                                ClientName = reader["client_name"].ToString(),
                                FullIllustrations = Convert.ToInt32(reader["full_illustrations"]),
                                HalfIllustrations = Convert.ToInt32(reader["half_illustrations"]),
                                TotalPayment = Convert.ToDecimal(reader["total_payment"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return clientHistoryList;
        }

        // Class for client history
        public class ClientHistory
        {
            public string ClientName { get; set; }
            public int FullIllustrations { get; set; }
            public int HalfIllustrations { get; set; }
            public decimal TotalPayment { get; set; }
        }

    }
}
