using contact_liq;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace contact_liq
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Contact> _allContacts;
        private ObservableCollection<Contact> _filteredContacts;
        private Contact _selectedContact;
        private string _searchText;
        private string _selectedFilter;
        private string _selectedSort;
        private bool _isAscending;
        private string _selectedProjection;
        private string _selectedQuantifier;
        private string _quantifierResult;
        private string _selectedAggregation;
        private string _aggregationResult;

        public MainViewModel()
        {
            // Инициализация коллекций
            _allContacts = new ObservableCollection<Contact>();
            _filteredContacts = new ObservableCollection<Contact>();

            // Команды
            AddCommand = new RelayCommand(AddContact);
            EditCommand = new RelayCommand(EditContact, () => SelectedContact != null);
            DeleteCommand = new RelayCommand(DeleteContact, () => SelectedContact != null);
            ApplyFiltersCommand = new RelayCommand(ApplyFilters);

            // Добавляем тестовые данные
            LoadTestData();

            // Инициализация опций
            InitializeOptions();

            // Применяем начальные фильтры
            ApplyFilters();
        }

        private void InitializeOptions()
        {
            // Опции для фильтрации
            FilterOptions = new ObservableCollection<string>
            {
                "Все",
                "Старше 30 лет",
                "Младше 30 лет",
                "Из Москвы"
            };
            SelectedFilter = "Все";

            // Опции для сортировки
            SortOptions = new ObservableCollection<string>
            {
                "По имени",
                "По возрасту",
                "По городу"
            };
            SelectedSort = "По имени";
            IsAscending = true;

            // Опции для проекции
            ProjectionOptions = new ObservableCollection<string>
            {
                "Полное имя",
                "Имя и возраст",
                "Контактная информация"
            };
            SelectedProjection = "Полное имя";

            // Опции для кванторов
            QuantifierOptions = new ObservableCollection<string>
            {
                "Есть ли кто из Москвы?",
                "Есть ли кто старше 40 лет?",
                "Есть ли с email?"
            };
            SelectedQuantifier = "Есть ли кто из Москвы?";

            // Опции для агрегации
            AggregationOptions = new ObservableCollection<string>
            {
                "Средний возраст",
                "Максимальный возраст",
                "Минимальный возраст"
            };
            SelectedAggregation = "Средний возраст";
        }

        private void LoadTestData()
        {
            _allContacts.Add(new Contact
            {
                Id = 1,
                FirstName = "Иван",
                LastName = "Петров",
                Email = "ivan@mail.com",
                Age = 25,
                City = "Москва"
            });

            _allContacts.Add(new Contact
            {
                Id = 2,
                FirstName = "Мария",
                LastName = "Сидорова",
                Email = "maria@mail.com",
                Age = 35,
                City = "Санкт-Петербург"
            });

            _allContacts.Add(new Contact
            {
                Id = 3,
                FirstName = "Алексей",
                LastName = "Иванов",
                Email = "alexey@mail.com",
                Age = 42,
                City = "Москва"
            });
        }

        public ObservableCollection<Contact> FilteredContacts
        {
            get => _filteredContacts;
            set
            {
                _filteredContacts = value;
                OnPropertyChanged(nameof(FilteredContacts));
            }
        }

        public Contact SelectedContact
        {
            get => _selectedContact;
            set
            {
                _selectedContact = value;
                OnPropertyChanged(nameof(SelectedContact));
                ((RelayCommand)EditCommand).RaiseCanExecuteChanged();
                ((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
            }
        }

        public ObservableCollection<string> FilterOptions { get; set; }

        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                _selectedFilter = value;
                OnPropertyChanged(nameof(SelectedFilter));
            }
        }

        public ObservableCollection<string> SortOptions { get; set; }

        public string SelectedSort
        {
            get => _selectedSort;
            set
            {
                _selectedSort = value;
                OnPropertyChanged(nameof(SelectedSort));
            }
        }

        public bool IsAscending
        {
            get => _isAscending;
            set
            {
                _isAscending = value;
                OnPropertyChanged(nameof(IsAscending));
            }
        }

        public ObservableCollection<string> ProjectionOptions { get; set; }

        public string SelectedProjection
        {
            get => _selectedProjection;
            set
            {
                _selectedProjection = value;
                OnPropertyChanged(nameof(SelectedProjection));
                UpdateProjection();
            }
        }

        public ObservableCollection<string> QuantifierOptions { get; set; }

        public string SelectedQuantifier
        {
            get => _selectedQuantifier;
            set
            {
                _selectedQuantifier = value;
                OnPropertyChanged(nameof(SelectedQuantifier));
                CheckQuantifier();
            }
        }

        public string QuantifierResult
        {
            get => _quantifierResult;
            set
            {
                _quantifierResult = value;
                OnPropertyChanged(nameof(QuantifierResult));
            }
        }

        public ObservableCollection<string> AggregationOptions { get; set; }

        public string SelectedAggregation
        {
            get => _selectedAggregation;
            set
            {
                _selectedAggregation = value;
                OnPropertyChanged(nameof(SelectedAggregation));
                CalculateAggregation();
            }
        }

        public string AggregationResult
        {
            get => _aggregationResult;
            set
            {
                _aggregationResult = value;
                OnPropertyChanged(nameof(AggregationResult));
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ApplyFiltersCommand { get; }

        private void AddContact()
        {
            var newContact = new Contact
            {
                Id = _allContacts.Count > 0 ? _allContacts.Max(c => c.Id) + 1 : 1,
                FirstName = "Новый",
                LastName = "Контакт",
                Age = 18,
                City = "Город",
                Email = "email@example.com"
            };
            _allContacts.Add(newContact);
            ApplyFilters();
        }

        private void EditContact()
        {
            if (SelectedContact != null)
            {
                // Здесь можно открыть диалоговое окно для редактирования
                // Для простоты пока изменим демо-данные
                SelectedContact.FirstName = $"{SelectedContact.FirstName} (ред.)";
                ApplyFilters();
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

        private void ApplyFilters()
        {
            var query = _allContacts.AsEnumerable();

            // Фильтрация
            switch (SelectedFilter)
            {
                case "Старше 30 лет":
                    query = query.Where(c => c.Age > 30);
                    break;
                case "Младше 30 лет":
                    query = query.Where(c => c.Age < 30);
                    break;
                case "Из Москвы":
                    query = query.Where(c => c.City == "Москва");
                    break;
                default:
                    break;
            }

            // Поиск
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                query = query.Where(c =>
                    c.FirstName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    c.LastName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    c.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            // Сортировка
            switch (SelectedSort)
            {
                case "По имени":
                    query = IsAscending ?
                        query.OrderBy(c => c.FirstName) :
                        query.OrderByDescending(c => c.FirstName);
                    break;
                case "По возрасту":
                    query = IsAscending ?
                        query.OrderBy(c => c.Age) :
                        query.OrderByDescending(c => c.Age);
                    break;
                case "По городу":
                    query = IsAscending ?
                        query.OrderBy(c => c.City) :
                        query.OrderByDescending(c => c.City);
                    break;
            }

            FilteredContacts = new ObservableCollection<Contact>(query);
            UpdateProjection();
            CheckQuantifier();
            CalculateAggregation();
        }

        private void UpdateProjection()
        {
            // Проекция отображается в UI через привязку данных
            // Мы просто вызываем обновление для отображения
            OnPropertyChanged(nameof(FilteredContacts));
        }

        private void CheckQuantifier()
        {
            var query = _allContacts.AsEnumerable();

            bool result = false;
            switch (SelectedQuantifier)
            {
                case "Есть ли кто из Москвы?":
                    result = query.Any(c => c.City == "Москва");
                    break;
                case "Есть ли кто старше 40 лет?":
                    result = query.Any(c => c.Age > 40);
                    break;
                case "Есть ли с email?":
                    result = query.Any(c => !string.IsNullOrEmpty(c.Email));
                    break;
            }

            QuantifierResult = result ? "Да" : "Нет";
        }

        private void CalculateAggregation()
        {
            if (!_allContacts.Any())
            {
                AggregationResult = "Нет данных";
                return;
            }

            double result = 0;
            switch (SelectedAggregation)
            {
                case "Средний возраст":
                    result = _allContacts.Average(c => c.Age);
                    AggregationResult = $"{result:F1} лет";
                    break;
                case "Максимальный возраст":
                    result = _allContacts.Max(c => c.Age);
                    AggregationResult = $"{result} лет";
                    break;
                case "Минимальный возраст":
                    result = _allContacts.Min(c => c.Age);
                    AggregationResult = $"{result} лет";
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // RelayCommand для команд
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

        public void Execute(object parameter) => _execute();

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}