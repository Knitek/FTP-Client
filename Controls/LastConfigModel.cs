using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FTP_Client.Model;

namespace FTP_Client.Controls
{
    public class LastConfigModel : ICommand
    {
        Action _TargetExecuteMethod;
        //Func<bool> _TargetCanExecuteMethod;

        public ConfigModel Config {get;set;}
        public LastConfigModel Command { get { return this; } }

        public LastConfigModel(object executeMethod)
        {
            _TargetExecuteMethod = executeMethod as Action;
        }

        public LastConfigModel(Action executeMethod)
        {
            _TargetExecuteMethod = executeMethod;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        bool ICommand.CanExecute(object parameter)
        {           
            if (_TargetExecuteMethod != null)
            {
                return true;
            }
            return false;
        }

        public event EventHandler CanExecuteChanged = delegate { };

        void ICommand.Execute(object parameter)
        {
            if (_TargetExecuteMethod != null)
            {
                _TargetExecuteMethod();
            }
        }
    }
}
