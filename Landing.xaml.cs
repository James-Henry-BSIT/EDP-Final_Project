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
        public Landing()
        {
            InitializeComponent();
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

        private void HomeLabel_Click(object sender, MouseEventArgs e)
        {

        }

        private void UpdatesLabel_Click(object sender, MouseEventArgs e)
        {

        }

        private void CommissionLabel_Click(object sender, MouseEventArgs e)
        {

        }

        private void NotificationsLabel_Click(object sender, MouseEventArgs e)
        {

        }

        private void SocialsLabel_Click(object sender, MouseEventArgs e)
        {

        }
        //MENU SELECT CLICK FUNCTIONS
        private void Hover_Retract(object sender, MouseEventArgs e)
        {
            Storyboard collapseHeight = (Storyboard)MenuSelection.FindResource("CollapseHoverMenuHeight");
            collapseHeight.Begin();
        }

    }
}
