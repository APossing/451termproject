using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
    class UserUcVm : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private DataAccessLayer _dal;


        public UserUcVm()
        {
            _dal = new DataAccessLayer();
            _notEditable = true;
        }
        public String _userId; // uh ohs, must be a way around this lol
        public String UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                NotifyPropertyChanged("UserId");
            }
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private String _selectedUserName;
        public String SelectedUserName
        {
            get => _selectedUserName;
            set
            {
                if (value != _selectedUserName)
                {
                    _selectedUserName = value;
                    _selectedUser = null;
                    Users = !string.IsNullOrEmpty(value) ? _dal.GetUsers(value) : new List<User>();
                    NotifyPropertyChanged("SelectedUserName");
                }
            }
        }

        private List<User> _users;

        public List<User> Users
        {
            get => _users;
            set
            {
                _users = value;
                NotifyPropertyChanged("Users");
            }
        }

        private User _selectedUser;

        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                if (value != null)
                {
                    Friends = new ObservableCollection<User>(_dal.GetFriends(SelectedUser.id));
                    FavoriteBusinesses = new ObservableCollection<Business>(_dal.GetFavoriteBusinesses(SelectedUser.id));
                    FriendsReviewBusinessRows = new ObservableCollection<FriendsReviewBusinessRow>(_dal.GetFriendsReviewing(_friends));
                }
                else
                {
                    Friends = new ObservableCollection<User>();
                    FavoriteBusinesses = new ObservableCollection<Business>();
                    FriendsReviewBusinessRows = new ObservableCollection<FriendsReviewBusinessRow>();
                }
                NotifyPropertyChanged("SelectedUser");
            }
        }

        private List<User> _friends;

        public ObservableCollection<User> Friends
        {
            get
            {
                if (_selectedUser != null)
                    return new ObservableCollection<User>(_friends);
                else
                    return new ObservableCollection<User>();
            }
            set
            {
                _friends = value.ToList();
                NotifyPropertyChanged("Friends");
            }
        }

        private List<Business> _favoriteBusinesses;

        public ObservableCollection<Business> FavoriteBusinesses
        {
            get
            {
                if (_selectedUser != null)
                    return new ObservableCollection<Business>(_favoriteBusinesses);
                else
                    return new ObservableCollection<Business>();
            }
            set
            {
                _favoriteBusinesses = value.ToList();
                NotifyPropertyChanged("FavoriteBusinesses");
            }
        }

        private Business _selectedFavoriteBusiness;

        public Business SelectedFavoriteBusiness
        {
            get => _selectedFavoriteBusiness;
            set
            {
                _selectedFavoriteBusiness = value;
                NotifyPropertyChanged("SelectedFavoriteBusiness");
            }
        }

        private List<FriendsReviewBusinessRow> _friendsReviewBusinessRows;

        public ObservableCollection<FriendsReviewBusinessRow> FriendsReviewBusinessRows
        {
            get
            {
                if (_selectedUser != null)
                    return new ObservableCollection<FriendsReviewBusinessRow>(_friendsReviewBusinessRows);
                else
                    return new ObservableCollection<FriendsReviewBusinessRow>();
            }
            set
            {
                _friendsReviewBusinessRows = value.ToList();
                NotifyPropertyChanged("FriendsReviewBusinessRows");
            }
        }



        private void RandomUser_Click()
        {
            UserId = _dal.getRandomUser();
        }

        private bool _notEditable;

        public bool NotEditable
        {
            get => _notEditable;
            set
            {
                if (value != _notEditable)
                {
                    _notEditable = value;
                    NotifyPropertyChanged("NotEditable");
                }
            }
        }

        public bool NotEditableGrayOut
        {
            get => !_notEditable;
        }

        public void EditButtonClicked()
        {
            NotEditable = !NotEditable;
        }

        public void UpdateButtonClicked()
        {
            _dal.UpdateUserLocation(SelectedUser);
        }

        public void RemoveFromFavorites()
        {
            if (SelectedFavoriteBusiness != null)
            {
                _dal.RemoveFavoriteBusiness(SelectedFavoriteBusiness, SelectedUser);
                FavoriteBusinesses = new ObservableCollection<Business>(_favoriteBusinesses.FindAll(x => x.id != SelectedFavoriteBusiness.id));
            }
        }
        public ICommand UpdateButtonClickCommand => new GeneralCommand(UpdateButtonClicked);
        public ICommand EditButtonClickCommand => new GeneralCommand(EditButtonClicked);

        public ICommand RemoveFromFavoritesClickCommand => new GeneralCommand(RemoveFromFavorites);
        public ICommand RandomUserClickCommand => new GeneralCommand(RandomUser_Click);
    }
}
