using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HW1.Helper
{
    public delegate void CallBack();

    public class GeneralCommand : ICommand
    {
        public GeneralCommand(CallBack call)
        {
            _CBack = call;
        }

        private CallBack _CBack;
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _CBack.Invoke();
        }
    }
}
