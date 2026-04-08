using System.Windows;

namespace contact_liq;

public partial class EditContactDialog : Window
{
    private readonly Contact _targetContact;

    public EditContactDialog(Contact contact, bool isNewContact)
    {
        InitializeComponent();
        _targetContact = contact;
        EditableContact = contact.Clone();
        DialogTitle = isNewContact ? "Add Contact" : "Edit Contact";
        DataContext = this;
    }

    public Contact EditableContact { get; }

    public string DialogTitle { get; }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        _targetContact.ApplyChanges(EditableContact);
        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
