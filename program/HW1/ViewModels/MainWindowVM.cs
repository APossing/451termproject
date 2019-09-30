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
    class MainWindowVM
    {
        public BusinessUcVm BusinessUcVm { get; private set; }
        public UserUcVm UserUcVm { get; private set; }

        public MainWindowVM()
        {
            UserUcVm = new UserUcVm();
            BusinessUcVm = new BusinessUcVm(UserUcVm);
        }

    }
}
