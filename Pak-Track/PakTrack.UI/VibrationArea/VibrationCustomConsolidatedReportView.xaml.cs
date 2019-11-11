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
using System.Text.RegularExpressions;

namespace PakTrack.UI.VibrationArea
{
    /// <summary>
    /// Interaction logic for VibrationCustomConsolidatedReportView.xaml
    /// </summary>
    public partial class VibrationCustomConsolidatedReportView : UserControl
    {
        private static readonly Regex Regex = new Regex("[^0-9.-]+");
        public VibrationCustomConsolidatedReportView()
        {
            InitializeComponent();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text);
        }
    }
}
