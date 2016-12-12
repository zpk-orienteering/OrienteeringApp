using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;

namespace Orienteering.ViewModels
{
    /// <summary>
    /// Bazowa klasa dla wszystkich ViewModeli w aplikacji
    /// Implementuje interfejs INotifyPropertyChanged 
    /// Posiada pole DisplayName. Jest to klasa abstrakcyna
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        #region Constructor

        protected ViewModelBase()
        {
        }

        #endregion // Constructor

        #region DisplayName

        /// <summary>
        /// Zwraca użytkownikowi nazwe objektu
        /// Klasy dziedziczące mogą ustawić nową wartość tego pola
        /// lub nadpisać je aby zdeterminować wartość na rządanie
        /// </summary>
        public virtual string DisplayName { get; protected set; }

        #endregion // DisplayName

        #region Debugging Aides

        /// <summary>
        /// Ostrzega developera jeżeli objekt nie posiada
        /// publicznej własności (property) o zdefiniowanej w argumęcie funkcji nazwie
        /// Ta metoda nie compiluje się w Release build
        /// </summary>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            // Weryfikuje czy własność (property) odpowiada prawdziwej  
            // publicznej własności w tym objekcie
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;

                if (this.ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
                else
                    Debug.Fail(msg);
            }
        }

        /// <summary>
        /// Returns whether an exception is thrown, or if a Debug.Fail() is used
        /// when an invalid property name is passed to the VerifyPropertyName method.
        /// The default value is false, but subclasses used by unit tests might 
        /// override this property's getter to return true.
        /// </summary>
        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

        #endregion // Debugging Aides

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Odpala się gdy własności tego objektu przypiszemy nową wartość
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Odpala PropertyChanged event tego objektu
        /// </summary>
        /// <param name="propertyName">Własność (property) której nadano nowa wartość</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);

            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion // INotifyPropertyChanged Members

        #region IDisposable Members

        /// <summary>
        /// Wywoływane, gdy objekt jest usuwany z aplikacji
        /// i przechwytywany przez garbage collector
        /// </summary>
        public void Dispose()
        {
            this.OnDispose();
        }

        /// <summary>
        /// Klasy potomne mogą nadpisywać tą metode aby wykonać 
        /// czyszczenie, np. usuwanie event handlerów
        /// </summary>
        protected virtual void OnDispose()
        {
        }

#if DEBUG
        /// <summary>
        /// Dobra praktyka aby sprawdzić czy ViewModel jest poprawnie usuwany przez garbage collector
        /// </summary>
        ~ViewModelBase()
        {
            string msg = string.Format("{0} ({1}) ({2}) Finalized", this.GetType().Name, this.DisplayName, this.GetHashCode());
            System.Diagnostics.Debug.WriteLine(msg);
        }
#endif

        #endregion // IDisposable Members
    }
}
