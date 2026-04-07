using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;

namespace contact_liq
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Contact> _allContacts;
        private ObservableCollection<Contact> _filteredContacts;
        private Contact _selectedContact;
        private string _selectedFilter;
        private string _selectedSort;
        private string _selectedProjection;
        private string _selectedQuantifier;
        private string _selectedAggregation;
        private string _filterValue;
        private string _quantifierResult;
        private string _aggregationResult;
        private string _projectionResult;

        public MainViewModel()
        {
            _allContacts = new ObservableCollection<Contact>();
            _filteredContacts = new ObservableCollection<Contact>();

            // Добавляем тестовые данные
            LoadSampleData();

            // Инициализация команд
            AddCommand = new RelayCommand(AddContact);
            EditCommand = new RelayCommand(EditContact, () => SelectedContact != null);
            DeleteCommand = new RelayCommand(DeleteContact, () => SelectedContact != null);
            ApplyFiltersCommand = new RelayCommand(ApplyFilters);

            // Опции для комбобоксов
            FilterOptions = new List<string> { "По возрасту (старше)", "По городу", "По имени" };
            SortOptions = new List<string> { "По имени (А-Я)", "По имени (Я-А)", "По возрасту (возр.)", "По возрасту (убыв.)" };
            ProjectionOptions = new List<string> { "Имя и возраст", "Имя и email", "Инициалы и город" };
            QuantifierOptions = new List<string> { "Есть ли старше 30?", "Все ли из Москвы?", "Содержит ли email @" };
            AggregationOptions = new List<string> { "Средний возраст", "Максимальный возраст", "Количество контактов" };

            ApplyFilters();
        }

        private void LoadSampleData()
        {
            _allContacts.Add(new Contact { Id = 1, FirstName = "Иван", LastName = "Петров", Email = "ivan@mail.com", Age = 25, City = "Москва" });
            _allContacts.Add(new Contact { Id = 2, FirstName = "Мария", LastName = "Иванова", Email = "maria@mail.com", Age = 32, City = "СПб" });
            _allContacts.Add(new Contact { Id = 3, FirstName = "Петр", LastName = "Сидоров", Email = "petr@mail.com", Age = 28, City = "Москва" });
            _allContacts.Add(new Contact { Id = 4, FirstName = "Анна", LastName = "Кузнецова", Email = "anna@mail.com", Age = 35, City = "Казань" });
            _allContacts.Add(new Contact { Id = 5, FirstName = "Сергей", LastName = "Волков", Email = "sergey@mail.com", Age = 29, City = "Москва" });
        }

        public ObservableCollection<Contact> FilteredContacts
        {
            get => _filteredContacts;
            set { _filteredContacts = value; OnPropertyChanged(nameof(FilteredContacts)); }
        }

        public Contact SelectedContact
        {
            get => _selectedContact;
            set { _selectedContact = value; OnPropertyChanged(nameof(SelectedContact)); }
        }

        public List<string> FilterOptions { get; set; }
        public List<string> SortOptions { get; set; }
        public List<string> ProjectionOptions { get; set; }
        public List<string> QuantifierOptions { get; set; }
        public List<string> AggregationOptions { get; set; }

        public string SelectedFilter
        {
            get => _selectedFilter;
            set { _selectedFilter = value; OnPropertyChanged(nameof(SelectedFilter)); ApplyFilters(); }
        }

        public string SelectedSort
        {
            get => _selectedSort;
            set { _selectedSort = value; OnPropertyChanged(nameof(SelectedSort)); ApplyFilters(); }
        }

        public string SelectedProjection
        {
            get => _selectedProjection;
            set { _selectedProjection = value; OnPropertyChanged(nameof(SelectedProjection)); ApplyFilters(); }
        }

        public string SelectedQuantifier
        {
            get => _selectedQuantifier;
            set { _selectedQuantifier = value; OnPropertyChanged(nameof(SelectedQuantifier)); ApplyQuantifier(); }
        }

        public string SelectedAggregation
        {
            get => _selectedAggregation;
            set { _selectedAggregation = value; OnPropertyChanged(nameof(SelectedAggregation)); ApplyAggregation(); }
        }

        public string FilterValue
        {
            get => _filterValue;
            set { _filterValue = value; OnPropertyChanged(nameof(FilterValue)); ApplyFilters(); }
        }

        public string QuantifierResult
        {
            get => _quantifierResult;
            set { _quantifierResult = value; OnPropertyChanged(nameof(QuantifierResult)); }
        }

        public string AggregationResult
        {
            get => _aggregationResult;
            set { _aggregationResult = value; OnPropertyChanged(nameof(AggregationResult)); }
        }

        public string ProjectionResult
        {
            get => _projectionResult;
            set { _projectionResult = value; OnPropertyChanged(nameof(ProjectionResult)); }
        }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand ApplyFiltersCommand { get; set; }

        private void ApplyFilters()
        {
            var query = _allContacts.AsEnumerable();

            // Фильтрация
            if (!string.IsNullOrEmpty(SelectedFilter) && !string.IsNullOrEmpty(FilterValue))
            {
                switch (SelectedFilter)
                {
                    case "По возрасту (старше)":
                        if (int.TryParse(FilterValue, out int age))
                            query = query.Where(c => c.Age > age);
                        break;
                    case "По городу":
                        query = query.Where(c => c.City.Contains(FilterValue));
                        break;
                    case "По имени":
                        query = query.Where(c => c.FirstName.Contains(FilterValue));
                        break;
                }
            }

            // Сортировка
            if (!string.IsNullOrEmpty(SelectedSort))
            {
                switch (SelectedSort)
                {
                    case "По имени (А-Я)":
                        query = query.OrderBy(c => c.FirstName);
                        break;
                    case "По имени (Я-А)":
                        query = query.OrderByDescending(c => c.FirstName);
                        break;
                    case "По возрасту (возр.)":
                        query = query.OrderBy(c => c.Age);
                        break;
                    case "По возрасту (убыв.)":
                        query = query.OrderByDescending(c => c.Age);
                        break;
                }
            }

            // Проекция (трансформация)
            if (!string.IsNullOrEmpty(SelectedProjection))
            {
                switch (SelectedProjection)
                {
                    case "Имя и возраст":
                        ProjectionResult = string.Join(", ", query.Select(c => $"{c.FirstName} ({c.Age} лет)"));
                        break;
                    case "Имя и email":
                        ProjectionResult = string.Join(", ", query.Select(c => $"{c.FirstName}: {c.Email}"));
                        break;
                    case "Инициалы и город":
                        ProjectionResult = string.Join(", ", query.Select(c => $"{c.FirstName[0]}.{c.LastName[0]}. - {c.City}"));
                        break;
                }
            }

            FilteredContacts = new ObservableCollection<Contact>(query);
        }

        private void ApplyQuantifier()
        {
            if (string.IsNullOrEmpty(SelectedQuantifier)) return;

            switch (SelectedQuantifier)
            {
                case "Есть ли старше 30?":
                    QuantifierResult = _allContacts.Any(c => c.Age > 30) ? "Да" : "Нет";
                    break;
                case "Все ли из Москвы?":
                    QuantifierResult = _allContacts.All(c => c.City == "Москва") ? "Да" : "Нет";
                    break;
                case "Содержит ли email @":
                    QuantifierResult = _allContacts.Any(c => c.Email.Contains("@")) ? "Да" : "Нет";
                    break;
            }
        }

        private void ApplyAggregation()
        {
            if (string.IsNullOrEmpty(SelectedAggregation)) return;

            switch (SelectedAggregation)
            {
                case "Средний возраст":
                    AggregationResult = _allContacts.Average(c => c.Age).ToString("F2");
                    break;
                case "Максимальный возраст":
                    AggregationResult = _allContacts.Max(c => c.Age).ToString();
                    break;
                case "Количество контактов":
                    AggregationResult = _allContacts.Count().ToString();
                    break;
            }
        }

        private void AddContact()
        {
            var newContact = new Contact
            {
                Id = _allContacts.Max(c => c.Id) + 1,
                FirstName = "Новый",
                LastName = "Контакт",
                Email = "new@mail.com",
                Age = 20,
                City = "Город"
            };
            _allContacts.Add(newContact);
            ApplyFilters();
        }

        private void EditContact()
        {
            if (SelectedContact != null)
            {
                // Открыть диалог редактирования
                var dialog = new EditContactDialog(SelectedContact);
                if (dialog.ShowDialog() == true)
                {
                    ApplyFilters();
                }
            }
        }

        private void DeleteContact()
        {
            if (SelectedContact != null)
            {
                _allContacts.Remove(SelectedContact);
                ApplyFilters();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}