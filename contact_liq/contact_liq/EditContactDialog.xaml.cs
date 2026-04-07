using System.Windows;

namespace contact_liq
{
    public partial class EditContactDialog : Window
    {
        private Contact _contact;

        public EditContactDialog(Contact contact)
        {
            InitializeComponent();
            _contact = contact;
            DataContext = _contact;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}