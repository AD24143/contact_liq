using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace contact_liq
{
    /// <summary>
    /// Represents a single contact with name, phone, email, and favorite status.
    /// Implements INotifyPropertyChanged for WPF data binding support.
    /// </summary>
    public class Contact : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private string _phone = string.Empty;
        private string _email = string.Empty;
        private bool _isFavorite;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string Phone
        {
            get => _phone;
            set { _phone = value; OnPropertyChanged(); }
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public bool IsFavorite
        {
            get => _isFavorite;
            set { _isFavorite = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString() => Name;
    }
}
