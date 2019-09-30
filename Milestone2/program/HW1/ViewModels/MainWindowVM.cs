using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using HW1.DAL;
using HW1.Helper;
using HW1.Models;
using Npgsql;
using ListBoxItem = HW1.Models.ListBoxItem;

namespace HW1.ViewModels
{
    class MainWindowVM : INotifyPropertyChanged, IDataErrorInfo
    {
        private NpgsqlConnection _connection;
        private Page _currentPage;
        private DataAccessLayer _dal;

        public Page CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                NotifyPropertyChanged("CurrentPage");
            }
        }

        private List<State> _states;

        public List<State> States
        {
            get => _states;
            set
            {
                _states = value;
                NotifyPropertyChanged("States");
            }
        }

        private State _selectedState;
        public State SelectedState
        {
            get => _selectedState;
            set
            {
                if (value != _selectedState && value != null)
                {
                    _selectedState = value;
                    SelectedCity = null;
                    Cities = _dal.GetCities(value);
                    NotifyPropertyChanged("SelectedState");
                }
            }
        }

        private City _selectedCity;

        public City SelectedCity
        {
            get => _selectedCity;
            set
            {
                _selectedCity = value;

                Zipcodes = value != null ? new List<string>(_dal.GetZipcodes(SelectedState, SelectedCity)) : new List<string>();
                NotifyPropertyChanged("SelectedCity");
            }
        }

        private String _selectedZipcode;

        public String SelectedZipcode
        {
            get => _selectedZipcode;
            set
            {
                _selectedZipcode = value;
                if (value != null)
                {
                    SelectedCategories = new ObservableCollection<ListBoxItem>(_dal.GetBusinessCategories(SelectedState, SelectedCity, SelectedZipcode));
                    Businesses = new ObservableCollection<Business>(_dal.GetBusinesses(SelectedState, SelectedCity, SelectedZipcode));
                }
                else
                {
                    BusinessCategories = new List<string>();
                    Businesses = new ObservableCollection<Business>();
                }
                NotifyPropertyChanged("SelectedZipcode");
            }
        }

        private ObservableCollection<ListBoxItem> _selectedCategories = new ObservableCollection<ListBoxItem>();

        public ObservableCollection<ListBoxItem> SelectedCategories
        {
            get => _selectedCategories;
            set
            {
                _selectedCategories = value;
                NotifyPropertyChanged("SelectedCategories");
            }
        }

        private String _selectedCategory;

        public String SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                var selectedItems = SelectedCategories.Where(x => x.IsSelected);
                if (selectedItems.Any())
                {
                    List<String> BusinessCategories = new List<string>();
                    foreach (var item in selectedItems)
                    {
                        BusinessCategories.Add(item.Item);
                    }
                    Businesses = new ObservableCollection<Business>(_dal.GetBusinesses(SelectedState, SelectedCity, BusinessCategories, SelectedZipcode));
                }
                else
                {
                    Businesses = new ObservableCollection<Business>(_dal.GetBusinesses(SelectedState, SelectedCity, SelectedZipcode));
                }

                NotifyPropertyChanged("SelectedCategory");
            }
        }

        private List<String> _businessCategories;

        public List<String> BusinessCategories
        {
            get => _businessCategories;
            set
            {
                _businessCategories = value;
                NotifyPropertyChanged("BusinessCategories");
            }
        }

        private List<City> _cities;

        public List<City> Cities
        {
            get => _cities;
            set
            {
                _cities = value;
                NotifyPropertyChanged("Cities");
            }
        }

        private List<String> _zipcodes;

        public List<String> Zipcodes
        {
            get => _zipcodes;
            set
            {
                _zipcodes = value;
                NotifyPropertyChanged("Zipcodes");
            }
        }



        private List<Business> _businesses;

        public ObservableCollection<Business> Businesses
        {
            get
            {
                if (_businesses != null)
                    return new ObservableCollection<Business>(_businesses);
                else
                    return new ObservableCollection<Business>();
            }
            set
            {
                    _businesses = value.ToList();
                    NotifyPropertyChanged("Businesses");
            }
        }

        private Business _selectedBusiness;
        public Business SelectedBusiness
        {
            get => _selectedBusiness;
            set
            {
                _selectedBusiness = value;
                if (value != null)
                {
                    Reviews = new ObservableCollection<Review>(_dal.GetReviews(SelectedBusiness));
                }
                else
                {
                    Reviews = new ObservableCollection<Review>();
                }
                NotifyPropertyChanged("SelectedBusiness");
            }
        }

        private List<Review> _reviews;

        public ObservableCollection<Review> Reviews
        {
            get
            {
                if (_businesses != null)
                    return new ObservableCollection<Review>(_reviews);
                else
                    return new ObservableCollection<Review>();
            }
            set
            {
                _reviews = value.ToList();
                NotifyPropertyChanged("Reviews");
            }
        }

        public MainWindowVM()
        {
            _dal = new DataAccessLayer();
            States = _dal.GetStates();
        }

        private bool SubmitReady()
        {
            if (SelectedBusiness != null && !String.IsNullOrEmpty(ReviewText) && !String.IsNullOrEmpty(UserId))
            {
                try
                {
                    Convert.ToInt32(Rating);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        private void Submit_Click()
        {
            if (SubmitReady())
            {
                _dal.SubmitReview(ReviewText,SelectedBusiness,UserId,Rating);
                SelectedCategory = null;

                ReviewText = "";
                Rating = "";
                UserId = "";
                NotifyPropertyChanged("ReviewText");
                NotifyPropertyChanged("Rating");
                NotifyPropertyChanged("UserId");
            }
        }

        public String ReviewText { get; set; }
        public String Rating { get; set; }
        public String UserId { get; set; }

        public ICommand SubmitButtonClickCommand => new GeneralCommand(Submit_Click);


        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string this[string columnName] => Validate(columnName);

        private string Validate(string propertyName)
        {
            string validationMessage = "";
            switch (propertyName)
            {
                case "SubmitButtonClickCommand":
                    int a = 5;
                    break;
            }

            return validationMessage;
        }

        public string Error { get; }
    }
}
