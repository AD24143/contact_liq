using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace contact_liq;

public class Contact : INotifyPropertyChanged
{
    private int _id;
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _email = string.Empty;
    private int _age;
    private string _city = string.Empty;

    private int? _categoryId;
    private Category? _category;

    public int Id
    {
        get => _id;
        set => SetField(ref _id, value);
    }

    public string FirstName
    {
        get => _firstName;
        set => SetField(ref _firstName, value);
    }

    public string LastName
    {
        get => _lastName;
        set => SetField(ref _lastName, value);
    }

    public string Email
    {
        get => _email;
        set => SetField(ref _email, value);
    }

    public int Age
    {
        get => _age;
        set => SetField(ref _age, value);
    }

    public string City
    {
        get => _city;
        set => SetField(ref _city, value);
    }

    public int? CategoryId
    {
        get => _categoryId;
        set => SetField(ref _categoryId, value);
    }

    public virtual Category? Category
    {
        get => _category;
        set => SetField(ref _category, value);
    }

    public string FullName => $"{FirstName} {LastName}".Trim();

    public Contact Clone()
    {
        return new Contact
        {
            Id = Id,
            FirstName = FirstName,
            LastName = LastName,
            Email = Email,
            Age = Age,
            City = City,
            CategoryId = CategoryId
        };
    }

    public void ApplyChanges(Contact source)
    {
        FirstName = source.FirstName;
        LastName = source.LastName;
        Email = source.Email;
        Age = source.Age;
        City = source.City;
        CategoryId = source.CategoryId;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        if (propertyName is nameof(FirstName) or nameof(LastName))
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FullName)));
        }

        return true;
    }
}
