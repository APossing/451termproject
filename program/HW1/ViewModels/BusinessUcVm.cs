using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using HW1.DAL;
using HW1.Helper;
using HW1.Models;
using ListBoxItem = HW1.Models.ListBoxItem;

namespace HW1.ViewModels
{
    class BusinessUcVm : INotifyPropertyChanged
    {
        private DataAccessLayer _dal;
        private UserUcVm _userVm;
        private List<State> _states;
        private HashSet<string> _checkinCheck;

        public BusinessUcVm(UserUcVm userVm)
        {
            _dal = new DataAccessLayer();
            States = _dal.GetStates();
            _userVm = userVm;
            _checkinCheck = new HashSet<string>();

            FilterByMealBoxes = new List<Checkbox>{
                new Checkbox("Breakfast", this, "breakfast"),
                new Checkbox("Brunch", this, "brunch"),
                new Checkbox("Lunch", this, "lunch"),
                new Checkbox("Dinner", this, "dinner"),
                new Checkbox("Dessert", this, "dessert"),
                new Checkbox("Late Night", this, "latenight")
            };
            FilterByAttributeCheckboxes = new List<Checkbox>
            {
                new Checkbox("Accepts Credit Cards", this,"BusinessAcceptsCreditCards"),
                new Checkbox("Takes Reservation", this, "RestaurantsReservations"),
                new Checkbox("Wheelchair Accesible", this,"WheelchairAccessible"),
                new Checkbox("Outdoor Seating", this, "OutdoorSeating"),
                new Checkbox("Good for Kids", this, "GoodForKids"),
                new Checkbox("Good for Groups", this,"RestaurantsGoodForGroups"),
                new Checkbox("Delivery", this, "RestaurantsDelivery"),
                new Checkbox("Take Out", this, "RestaurantsTakeOut"),
                new Checkbox("Free Wifi", this,"WiFi"),
                new Checkbox("Bike Parking", this, "BikeParking")
            };
            FilterByPrice = new List<Checkbox>
            {
                new Checkbox("$", this, "1"),
                new Checkbox("$$", this, "2"),
                new Checkbox("$$$", this, "3"),
                new Checkbox("$$$$", this, "4")
            };
            SortByChoices = new List<string>
            {
                "Business Name", "Business Address", "City", "St", "Distance", "Review Count", "Review Rating",
                "Num Checkins"
            };
            _selectedChoice = _sortyByChoices[0];
            ReviewText = ReviewText = "Enter Review Text Here!";
        }

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

                Zipcodes = value != null
                    ? new List<string>(_dal.GetZipcodes(SelectedState, SelectedCity))
                    : new List<string>();
                SelectedCategories = new ObservableCollection<ListBoxItem>();
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
                    SelectedCategories =
                        new ObservableCollection<ListBoxItem>(_dal.GetBusinessCategories(SelectedState, SelectedCity,
                            SelectedZipcode));
                    Businesses =
                        new ObservableCollection<Business>(
                            _dal.GetBusinessesWithAttributesSorted(SelectedState, SelectedCity, SelectedZipcode, SelectedChoice, _userVm.SelectedUser));
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
                    BusinessCategories = new List<string>();
                    foreach (var item in selectedItems)
                    {
                        BusinessCategories.Add(item.Item);
                    }

                    Businesses = new ObservableCollection<Business>(_dal.GetBusinessesSorted(SelectedState, SelectedCity,
                        BusinessCategories, SelectedZipcode, SelectedChoice, _userVm.SelectedUser));
                }
                else
                {
                    Businesses =
                        new ObservableCollection<Business>(_dal.GetBusinesses(SelectedState, SelectedCity,
                            SelectedZipcode, SelectedChoice, _userVm.SelectedUser));
                }

                NotifyPropertyChanged("SelectedCategory");
            }
        }

        private List<String> _sortyByChoices;
        public List<String> SortByChoices
        {
            get => _sortyByChoices;
            set
            {
                _sortyByChoices = value;
                NotifyPropertyChanged("SortByChoices");
            }
        }

        private string _selectedChoice;
        public string SelectedChoice
        {
            get => _selectedChoice;

            set
            {
                _selectedChoice = value;
                NotifyPropertyChanged("Businesses");
                NotifyPropertyChanged("SelectedChoice");
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
                if (_businesses != null && SelectedZipcode != null)
                    return new ObservableCollection<Business>(GetFilteredBusinesses());
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
                    SelectedBusinessCategories = _dal.GetBusinessCategories(value.id);
                    SelectedBusinessAttributes = _dal.GetBusinessAttributes(value.id);
                    SelectedBusinessHours = _dal.GetBusinessHours(value.id);
                    Reviews = new ObservableCollection<Review>(_dal.GetReviews(SelectedBusiness));
                    BarGraphBars = new List<Bar>(_dal.GetBarsForBusiness(value.id));
                    if (_userVm.SelectedUser != null)
                        FriendsReviewBusinessRows = new ObservableCollection<FriendReviewBusinessRow>(_dal.GetFriendsReviewingBusiness(SelectedBusiness, _userVm.Friends.ToList()));
                }
                else
                {
                    SelectedBusinessCategories = new List<String>();
                    SelectedBusinessAttributes = new Attributes();
                    SelectedBusinessHours = new Hours();
                    Reviews = new ObservableCollection<Review>();
                    BarGraphBars = new List<Bar>();
                }

                NotifyPropertyChanged("SelectedBusiness");
            }
        }

        private Hours _selectedBusinessHours;

        public Hours SelectedBusinessHours
        {
            get => _selectedBusinessHours;
            set
            {
                _selectedBusinessHours = value;
                NotifyPropertyChanged("SelectedBusinessHours");

            }
        }

        private Attributes _selectedBusinessAttributes;

        public Attributes SelectedBusinessAttributes
        {
            get => _selectedBusinessAttributes;
            set
            {
                _selectedBusinessAttributes = value;
                NotifyPropertyChanged("SelectedBusinessAttributes");

            }
        }

        private List<String> _selectedBusinessCategories;

        public List<String> SelectedBusinessCategories
        {
            get => _selectedBusinessCategories;
            set
            {
                _selectedBusinessCategories = value;
                NotifyPropertyChanged("SelectedBusinessCategories");

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

        private String _reviewText;

        public String ReviewText
        {
            get => _reviewText;
            set
            {
                _reviewText = value;
                NotifyPropertyChanged("ReviewText");
            }
        }

        private String _rating;

        public String Rating
        {
            get => _rating;
            set
            {
                _rating = value;
                NotifyPropertyChanged("Rating");
            }
        }

        private List<FriendReviewBusinessRow> _friendsReviewBusinessRows;

        public ObservableCollection<FriendReviewBusinessRow> FriendsReviewBusinessRows
        {
            get
            {
                if (_userVm.SelectedUser != null)
                    return new ObservableCollection<FriendReviewBusinessRow>(_friendsReviewBusinessRows);
                else
                    return new ObservableCollection<FriendReviewBusinessRow>();
            }
            set
            {
                _friendsReviewBusinessRows = value.ToList();
                NotifyPropertyChanged("FriendsReviewBusinessRows");
            }
        }

        public void CheckinClick()
        {
            string checkinKey = "";
            DateTime dt = DateTime.Now;
            checkinKey = SelectedBusiness.id + _userVm._userId + dt.Date;
            if (_checkinCheck.Contains(checkinKey))
            {
                //do nothing, cant checkin into place more than once a day
            }
            else
            {
                _dal.InsertCheckin(SelectedBusiness);
                Businesses.First(x => x.id == SelectedBusiness.id).numCheckins++;
                _checkinCheck.Add(checkinKey);
            }
            NotifyPropertyChanged("Businesses");
        }

        /********************************************************************************/
        private bool _revPopup;

        public bool RevPopup
        {
            get => _revPopup;
            set
            {
                _revPopup = value;
                NotifyPropertyChanged("RevPopup");
            }
        }

        private bool _checkPopup;

        public bool CheckPopup
        {
            get => _checkPopup;
            set
            {
                _checkPopup = value;
                NotifyPropertyChanged("CheckPopup");
            }
        }
        /********************************************************************************/

        private List<Bar> _barGraphBars;

        public List<Bar> BarGraphBars
        {
            get => _barGraphBars;
            set
            {
                _barGraphBars = value;
                NotifyPropertyChanged("BarGraphBars");
            }
        }

        /********************************************************************************/
        public ICommand ShowReviewsClickCommand => new GeneralCommand(ShowReviewClicked);
        public void ShowReviewClicked()
        {
            RevPopup = true;
        }

        public ICommand ShowCheckinsClickCommand => new GeneralCommand(ShowCheckinClicked);
        public void ShowCheckinClicked()
        {
            CheckPopup = true;
        }
        /********************************************************************************/

        public List<Models.Checkbox> FilterByMealBoxes { get; set; }
        public List<Models.Checkbox> FilterByAttributeCheckboxes { get; set; }
        public List<Models.Checkbox> FilterByPrice { get; set; }

        public List<Business> GetFilteredBusinesses()
        {
            List<String> filterAttributes = new List<string>();
            List<String> filterPrices = new List<string>();
            ObservableCollection<Business> filteredBusinesses;
            foreach (var box in FilterByMealBoxes)
            {
                if (box.IsChecked)
                    filterAttributes.Add(box.Value);
            }
            foreach (var box in FilterByAttributeCheckboxes)
            {
                if (box.IsChecked)
                    filterAttributes.Add(box.Value);
            }
            foreach (var box in FilterByPrice)
            {
                if (box.IsChecked)
                    filterPrices.Add(box.Value);
            }

            if (BusinessCategories != null)
            {
                filteredBusinesses = new ObservableCollection<Business>(_dal.GetBusinessesSortedAttributes(SelectedState, SelectedCity,
                            BusinessCategories, SelectedZipcode, SelectedChoice, filterAttributes, filterPrices, _userVm.SelectedUser));
            }
            else
            {
                filteredBusinesses = new ObservableCollection<Business>(_dal.GetBusinessesSortedAttributes(SelectedState, SelectedCity,
            new List<string>(), SelectedZipcode, SelectedChoice, filterAttributes, filterPrices, _userVm.SelectedUser));
            }
            return filteredBusinesses.ToList();
        }

        public void AddToFavoritesClicked()
        {
            if (_userVm.SelectedUser != null && SelectedBusiness != null)
            {
                _dal.AddToFavorites(SelectedBusiness, _userVm.SelectedUser);
                _userVm.SelectedUser = _userVm.SelectedUser;
            }
        }

        /********************************************************************************/
        public void closeReviewPopupClicked()
        {
            RevPopup = false;
        }

        public void closeCheckinPopupClicked()
        {
            CheckPopup = false;
        }
        /********************************************************************************/

        /*public void CheckinsClicked()
        {
            CheckPopup = false;
        }

        public void CheckinPopupCloseClick()
        {
            CheckPopup = false;
        }*/

        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand CheckinClickCommand => new GeneralCommand(CheckinClick);
        public ICommand AddToFavoritesClickCommand => new GeneralCommand(AddToFavoritesClicked);
        //public ICommand CheckinsClickCommand => new GeneralCommand(CheckinsClicked);
        /********************************************************************************/
        public ICommand CloseReviewPopupClickCommand => new GeneralCommand(closeReviewPopupClicked);
        public ICommand CloseCheckinsPopupClickCommand => new GeneralCommand(closeCheckinPopupClicked);
        /********************************************************************************/
        public ICommand SubmitButtonClickCommand => new GeneralCommand(Submit_Click);
        public void Submit_Click()
        {
            if (SubmitReady())
            {
                _dal.SubmitReview(ReviewText, SelectedBusiness, _userVm.SelectedUser.id, Rating);
                SelectedCategory = null;

                ReviewText = "Enter Review Text Here!";
                Rating = "";
                _userVm._userId = "";
            }
        }

        public bool SubmitReady()
        {
            if (SelectedBusiness != null && !String.IsNullOrEmpty(ReviewText) && _userVm.SelectedUser != null)
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

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
