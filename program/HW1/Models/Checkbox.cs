using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HW1.Annotations;
using HW1.ViewModels;

namespace HW1.Models
{
    class Checkbox : INotifyPropertyChanged
    {
        public Checkbox(string displayName, BusinessUcVm vm, string value = "")
        {
            DisplayName = displayName;
            _cheating = vm;
            Value = value == "" ? DisplayName : value;
        }
        public String Value { get; set; }
        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value; 
                OnPropertyChanged("IsChecked");
                _cheating.NotifyPropertyChanged("Businesses");
            }
        }

        private BusinessUcVm _cheating;

        public string DisplayName { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
