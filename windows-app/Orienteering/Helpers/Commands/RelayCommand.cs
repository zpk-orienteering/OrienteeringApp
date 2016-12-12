using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace Orienteering.Helpers.Commands
{
    /// <summary>
    /// Klasa reprezentująca komendy, implementuje interfejs ICommand
    /// </summary>
    public class RelayCommand : ICommand
    {
        #region private fields

        private readonly Action execute;
        private readonly Func<bool> canExecute;

        #endregion



        public event EventHandler CanExecuteChanged
        {
            // wire the CanExecutedChanged event only if the canExecute func
            // is defined (that improves perf when canExecute is not used)
            add
            {
                if (this.canExecute != null)
                    CommandManager.RequerySuggested += value;
            }

            remove
            {
                if (this.canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }

        }



        /// <summary>
        /// Initializes a new instance of the KSCommand class
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action execute)
            : this(execute, null)
        {

        }



        /// <summary>
        /// Initializes a new instance of the KSCommand class
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            this.execute = execute;
            this.canExecute = canExecute;
        }

        public void Execute(object parameter)
        {
            if (DBHelper.IsSelfTestCompleted)
            {
                this.execute();
            }
            else
            {
                MessageBox.Show("Akcja niemożliwa do wykonania");
            }
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute == null ? true : this.canExecute();
        }
    }

    public class RelayCommand<T> : ICommand
    {
        #region Fields

        readonly Action<T> _execute = null;
        readonly Predicate<T> _canExecute = null;

        #endregion // Fields

        #region Constructors

        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion // Constructors

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            if (DBHelper.IsSelfTestCompleted)
            {
                _execute((T)parameter);
            }
            else
            {
                MessageBox.Show("Akcja niemożliwa do wykonania");
            }
        }

        #endregion // ICommand Members
    }
}
