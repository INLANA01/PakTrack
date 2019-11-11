using System.Windows;
using System.Windows.Controls;

namespace PakTrack.UI.VibrationArea
{
    /// <summary>
    /// Interaction logic for VibrationView
    /// </summary>
    public partial class VibrationView : UserControl
    {
        public VibrationView()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if(btn != null && btn.Content.Equals("Add Report"))
                btn.Content = "Remove";
            else
                btn.Content = "Add Report";
        }
    }
}
