using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;

namespace contact_liq;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly AppDbContext _dbContext;
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
    private string _statusMessage = "Database loaded successfully.";

    public MainViewModel()
    {
        _dbContext = new AppDbContext();
        _dbContext.Database.EnsureCreated();

        FilterOptions = ["City contains", "Age is greater than", "First name starts with"];
        SortOptions = ["First name ascending", "First name descending", "Age ascending", "Age descending"];
        ProjectionOptions = ["Full name and city", "Only Emails", "Name and Age"];
        QuantifierOptions = ["Any contact older than 30", "All contacts have email", "Any from Warsaw"];
        AggregationOptions = ["Average age", "Max age", "Total contacts count"];

        AddCommand = new RelayCommand(AddContact);
        EditCommand = new RelayCommand(EditContact, () => SelectedContact is not null);
        DeleteCommand = new RelayCommand(DeleteContact, () => SelectedContact is not null);
        ApplyFiltersCommand = new RelayCommand(RefreshResults);

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

    private void RefreshResults()
    {
        var allContacts = _dbContext.Contacts.Include(c => c.Category).ToList();
        IEnumerable<Contact> query = allContacts;

        if (!string.IsNullOrWhiteSpace(FilterValue))
        {
            if (SelectedFilter == "City contains")
            {
                query = query.Where(contact => contact.City.Contains(FilterValue, StringComparison.OrdinalIgnoreCase));
            }
            else if (SelectedFilter == "Age is greater than" && int.TryParse(FilterValue, out int ageVal))
            {
                query = query.Where(contact => contact.Age > ageVal);
            }
            else if (SelectedFilter == "First name starts with")
            {
                query = query.Where(contact => contact.FirstName.StartsWith(FilterValue, StringComparison.OrdinalIgnoreCase));
            }
        }

        if (SelectedSort == "First name ascending")
        {
            query = query.OrderBy(contact => contact.FirstName);
        }
        else if (SelectedSort == "First name descending")
        {
            query = query.OrderByDescending(contact => contact.FirstName);
        }
        else if (SelectedSort == "Age ascending")
        {
            query = query.OrderBy(contact => contact.Age);
        }
        else if (SelectedSort == "Age descending")
        {
            query = query.OrderByDescending(contact => contact.Age);
        }

        var resultList = query.ToList();
        ReplaceContacts(resultList);
        UpdateProjection(resultList);
        UpdateQuantifier(allContacts);
        UpdateAggregation(allContacts);
        StatusMessage = $"{resultList.Count} contact(s) shown from {allContacts.Count} total.";
    }

    private void UpdateProjection(IEnumerable<Contact> resultList)
    {
        if (SelectedProjection == "Full name and city")
            ProjectionResult = string.Join(Environment.NewLine, resultList.Select(contact => $"{contact.FullName} - {contact.City}"));
        else if (SelectedProjection == "Only Emails")
            ProjectionResult = string.Join(Environment.NewLine, resultList.Select(contact => contact.Email));
        else if (SelectedProjection == "Name and Age")
            ProjectionResult = string.Join(Environment.NewLine, resultList.Select(contact => $"{contact.FullName} ({contact.Age})"));
        else
            ProjectionResult = "Select a projection to see transformed data.";
    }

    private void UpdateQuantifier(List<Contact> allContacts)
    {
        if (SelectedQuantifier == "Any contact older than 30")
            QuantifierResult = allContacts.Any(contact => contact.Age > 30) ? "Yes" : "No";
        else if (SelectedQuantifier == "All contacts have email")
            QuantifierResult = allContacts.All(contact => !string.IsNullOrWhiteSpace(contact.Email)) ? "Yes" : "No";
        else if (SelectedQuantifier == "Any from Warsaw")
            QuantifierResult = allContacts.Any(contact => contact.City.Equals("Warsaw", StringComparison.OrdinalIgnoreCase)) ? "Yes" : "No";
        else
            QuantifierResult = "Select a check to evaluate the collection.";
    }

    private void UpdateAggregation(List<Contact> allContacts)
    {
        if (allContacts.Count == 0)
        {
            AggregationResult = "No data available.";
            return;
        }

        if (SelectedAggregation == "Average age")
            AggregationResult = allContacts.Average(contact => contact.Age).ToString("F1");
        else if (SelectedAggregation == "Max age")
            AggregationResult = allContacts.Max(contact => contact.Age).ToString();
        else if (SelectedAggregation == "Total contacts count")
            AggregationResult = allContacts.Count.ToString();
        else
            AggregationResult = "Select an aggregation to calculate a value.";
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
            FirstName = "New",
            LastName = "Contact",
            Email = "new.contact@example.com",
            Age = 18,
            City = "Warsaw",
            CategoryId = 1 // default
        };

        var dialog = new EditContactDialog(draft, isNewContact: true);
        if (dialog.ShowDialog() == true)
        {
            _dbContext.Contacts.Add(draft);
            _dbContext.SaveChanges();
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
            _dbContext.Contacts.Update(SelectedContact);
            _dbContext.SaveChanges();
            RefreshResults();
        }
    }

    private void DeleteContact()
    {
        if (SelectedContact is null)
        {
            return;
        }

        _dbContext.Contacts.Remove(SelectedContact);
        _dbContext.SaveChanges();
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
