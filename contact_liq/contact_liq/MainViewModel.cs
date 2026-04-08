using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace contact_liq;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly ObservableCollection<Contact> _allContacts = [];
    private Contact? _selectedContact;
    private string? _selectedFilter;
    private string? _selectedSort;
    private string? _selectedProjection;
    private string? _selectedQuantifier;
    private string? _selectedAggregation;
    private string _filterValue = string.Empty;
    private string _projectionResult = "Select a projection to see transformed data.";
    private string _quantifierResult = "Select a check to evaluate the collection.";
    private string _aggregationResult = "Select an aggregation to calculate a value.";
    private string _statusMessage = "Base version loaded with an in-memory collection.";

    public MainViewModel()
    {
        FilterOptions = ["City contains"];
        SortOptions = ["First name ascending"];
        ProjectionOptions = ["Full name and city"];
        QuantifierOptions = ["Any contact older than 30"];
        AggregationOptions = ["Average age"];

        AddCommand = new RelayCommand(AddContact);
        EditCommand = new RelayCommand(EditContact, () => SelectedContact is not null);
        DeleteCommand = new RelayCommand(DeleteContact, () => SelectedContact is not null);
        ApplyFiltersCommand = new RelayCommand(RefreshResults);

        SeedContacts();
        SelectedFilter = FilterOptions[0];
        SelectedSort = SortOptions[0];
        SelectedProjection = ProjectionOptions[0];
        SelectedQuantifier = QuantifierOptions[0];
        SelectedAggregation = AggregationOptions[0];
        RefreshResults();
    }

    public ObservableCollection<Contact> FilteredContacts { get; } = [];

    public IReadOnlyList<string> FilterOptions { get; }

    public IReadOnlyList<string> SortOptions { get; }

    public IReadOnlyList<string> ProjectionOptions { get; }

    public IReadOnlyList<string> QuantifierOptions { get; }

    public IReadOnlyList<string> AggregationOptions { get; }

    public Contact? SelectedContact
    {
        get => _selectedContact;
        set
        {
            if (SetField(ref _selectedContact, value))
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }

    public string? SelectedFilter
    {
        get => _selectedFilter;
        set
        {
            if (SetField(ref _selectedFilter, value))
            {
                RefreshResults();
            }
        }
    }

    public string? SelectedSort
    {
        get => _selectedSort;
        set
        {
            if (SetField(ref _selectedSort, value))
            {
                RefreshResults();
            }
        }
    }

    public string? SelectedProjection
    {
        get => _selectedProjection;
        set
        {
            if (SetField(ref _selectedProjection, value))
            {
                RefreshResults();
            }
        }
    }

    public string? SelectedQuantifier
    {
        get => _selectedQuantifier;
        set
        {
            if (SetField(ref _selectedQuantifier, value))
            {
                RefreshResults();
            }
        }
    }

    public string? SelectedAggregation
    {
        get => _selectedAggregation;
        set
        {
            if (SetField(ref _selectedAggregation, value))
            {
                RefreshResults();
            }
        }
    }

    public string FilterValue
    {
        get => _filterValue;
        set
        {
            if (SetField(ref _filterValue, value))
            {
                RefreshResults();
            }
        }
    }

    public string ProjectionResult
    {
        get => _projectionResult;
        private set => SetField(ref _projectionResult, value);
    }

    public string QuantifierResult
    {
        get => _quantifierResult;
        private set => SetField(ref _quantifierResult, value);
    }

    public string AggregationResult
    {
        get => _aggregationResult;
        private set => SetField(ref _aggregationResult, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetField(ref _statusMessage, value);
    }

    public ICommand AddCommand { get; }

    public ICommand EditCommand { get; }

    public ICommand DeleteCommand { get; }

    public ICommand ApplyFiltersCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void SeedContacts()
    {
        _allContacts.Add(new Contact { Id = 1, FirstName = "Alice", LastName = "Johnson", Email = "alice.johnson@example.com", Age = 24, City = "Warsaw" });
        _allContacts.Add(new Contact { Id = 2, FirstName = "Brian", LastName = "Taylor", Email = "brian.taylor@example.com", Age = 31, City = "Krakow" });
        _allContacts.Add(new Contact { Id = 3, FirstName = "Clara", LastName = "Evans", Email = "clara.evans@example.com", Age = 28, City = "Warsaw" });
        _allContacts.Add(new Contact { Id = 4, FirstName = "David", LastName = "Miller", Email = "david.miller@example.com", Age = 36, City = "Gdansk" });
        _allContacts.Add(new Contact { Id = 5, FirstName = "Eva", LastName = "Nowak", Email = "eva.nowak@example.com", Age = 29, City = "Wroclaw" });
    }

    private void RefreshResults()
    {
        IEnumerable<Contact> query = _allContacts;

        if (SelectedFilter == "City contains" && !string.IsNullOrWhiteSpace(FilterValue))
        {
            query = query.Where(contact => contact.City.Contains(FilterValue, StringComparison.OrdinalIgnoreCase));
        }

        if (SelectedSort == "First name ascending")
        {
            query = query.OrderBy(contact => contact.FirstName);
        }

        var resultList = query.ToList();
        ReplaceContacts(resultList);
        UpdateProjection(resultList);
        UpdateQuantifier();
        UpdateAggregation();
        StatusMessage = $"{resultList.Count} contact(s) shown from {_allContacts.Count} total.";
    }

    private void UpdateProjection(IEnumerable<Contact> resultList)
    {
        ProjectionResult = SelectedProjection == "Full name and city"
            ? string.Join(Environment.NewLine, resultList.Select(contact => $"{contact.FullName} - {contact.City}"))
            : "Select a projection to see transformed data.";
    }

    private void UpdateQuantifier()
    {
        QuantifierResult = SelectedQuantifier == "Any contact older than 30"
            ? (_allContacts.Any(contact => contact.Age > 30) ? "Yes" : "No")
            : "Select a check to evaluate the collection.";
    }

    private void UpdateAggregation()
    {
        AggregationResult = SelectedAggregation == "Average age"
            ? _allContacts.Average(contact => contact.Age).ToString("F1")
            : "Select an aggregation to calculate a value.";
    }

    private void ReplaceContacts(IEnumerable<Contact> contacts)
    {
        FilteredContacts.Clear();
        foreach (var contact in contacts)
        {
            FilteredContacts.Add(contact);
        }
    }

    private void AddContact()
    {
        var draft = new Contact
        {
            Id = _allContacts.Count == 0 ? 1 : _allContacts.Max(contact => contact.Id) + 1,
            FirstName = "New",
            LastName = "Contact",
            Email = "new.contact@example.com",
            Age = 18,
            City = "Warsaw"
        };

        var dialog = new EditContactDialog(draft, isNewContact: true);
        if (dialog.ShowDialog() == true)
        {
            _allContacts.Add(draft);
            RefreshResults();
            SelectedContact = draft;
        }
    }

    private void EditContact()
    {
        if (SelectedContact is null)
        {
            return;
        }

        var dialog = new EditContactDialog(SelectedContact, isNewContact: false);
        if (dialog.ShowDialog() == true)
        {
            RefreshResults();
        }
    }

    private void DeleteContact()
    {
        if (SelectedContact is null)
        {
            return;
        }

        _allContacts.Remove(SelectedContact);
        SelectedContact = null;
        RefreshResults();
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}
