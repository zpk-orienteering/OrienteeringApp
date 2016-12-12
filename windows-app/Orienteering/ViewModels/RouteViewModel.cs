/*      Orienteering software
 * 
 * author:  Mariusz Dobrowolski
 * created: 08.2013
 * 
 * modifications:
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using DAL;
using DAL.Models;
using Orienteering.Helpers.Commands;
using System.Collections.ObjectModel;
using Orienteering.Helpers;
using System.Windows;

namespace Orienteering.ViewModels
{
    /// <summary>
    /// ViewModel dla widoku tras (list punktów kontrolnych)
    /// </summary>
    public class RouteViewModel : ViewModelBase
    {
        private RouteModel model;

        #region Singleton Members

        private static RouteViewModel instance;

        private RouteViewModel() 
        {
            model = new RouteModel(DBHelper.ConnectionString);
            _routes = new ObservableCollection<Route>(model.GetAllRoutes());
            _allControlPoints = new ObservableCollection<ControlPoint>(model.GetControlPoints());

            AddNewRoute = new RelayCommand(() =>
                {
                    Name = "";
                    Info = "";
                    ChosenControlPoints.Clear();
                });
            SaveNewRoute = new RelayCommand(() =>
                {
                    if (!String.IsNullOrEmpty(Name))
                    {
                        Route r = new Route();
                        r.Name = Name;
                        r.Info = Info;

                        Routes.Add(r);
                        model.AddNewRoute(r);
                        model.AddControlPointsToRoute(r, ChosenControlPoints);

                        Name = "";
                        Info = "";
                        ChosenControlPoints.Clear();

                        ContestViewModel.GetInstance().RefreshLists();
                    }
                    else
                    {
                        MessageBox.Show("należy uzupełnić wymagane pola");
                    }
                });
            AddControlPoint = new RelayCommand(() =>
            {
                if(SelectedCpFromAll != null)
                {
                    if (!ChosenControlPoints.Contains(SelectedCpFromAll))
                    {
                        ChosenControlPoints.Add(SelectedCpFromAll);
                    }
                }
            });
            DeleteControlPoint = new RelayCommand(() =>
                {
                    if (SelectedCpFromChosen != null)
                    {
                        ChosenControlPoints.Remove(SelectedCpFromChosen);
                    }
                });
            DeleteRoute = new RelayCommand(() =>
                {
                    if (SelectedRoute != null)
                    {
                        model.DeleteRoute(SelectedRoute);
                        Routes.Remove(SelectedRoute);
                    }
                });
        }

        public static RouteViewModel GetInstance()
        {
            if (instance == null)
            {
                instance = new RouteViewModel();
            }
            return instance;
        }

        #endregion

        #region Fields and Properties

        private ObservableCollection<Route> _routes;
        /// <summary>
        /// lista wszystkich dostępnych tras
        /// </summary>
        public ObservableCollection<Route> Routes
        {
            get { return _routes; }
            set { _routes = value; }
        }

        private IEnumerable<ControlPoint> _selectedRouteControlPoints;
        /// <summary>
        /// Lista punktów kontrolnych aktualnie zaznaczonej trasy
        /// </summary>
        public IEnumerable<ControlPoint> SelectedRouteControlPoints
        {
            get { return _selectedRouteControlPoints; }
            set
            {
                _selectedRouteControlPoints = value;
                OnPropertyChanged("SelectedRouteControlPoints");
            }
        }

        private Route _selectedRoute;
        /// <summary>
        /// aktualnie zaznaczona trasa na liście 
        /// </summary>
        public Route SelectedRoute
        {
            get { return _selectedRoute; }
            set
            {
                _selectedRoute = value;
                OnPropertyChanged("SelectedRoute");

                SelectedRouteControlPoints = new List<ControlPoint>(model.GetRouteControlPoints(value));
            }
        }

        private string _name;
        /// <summary>
        /// nazwa nowej trasy
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private string _info;
        /// <summary>
        /// dodatkowe informacje nt. nowej trasy
        /// </summary>
        public string Info
        {
            get { return _info; }
            set
            {
                _info = value;
                OnPropertyChanged("Info");
            }
        }

        private ObservableCollection<ControlPoint> _allControlPoints = new ObservableCollection<ControlPoint>();
        /// <summary>
        /// lista wszystkich punktów kontrolnych dostępnych w aplikacji
        /// </summary>
        public ObservableCollection<ControlPoint> AllControlPoints
        {
            get { return _allControlPoints; }
            set { _allControlPoints = value; }
        }

        private ControlPoint _selectedCpFromAll;
        /// <summary>
        /// punkt kontrolny wybrany z listy wszystkich dostępnych punktów kontrolnych w aplikacji
        /// </summary>
        public ControlPoint SelectedCpFromAll
        {
            get { return _selectedCpFromAll; }
            set
            {
                _selectedCpFromAll = value;
                OnPropertyChanged("SelectedCpFromAll");
            }
        }

        private ControlPoint _selectedCpFromChosen;
        /// <summary>
        /// punkt kontrolny wybrany z listy wszystkich dostępnych punktów kontrolnych w aplikacji
        /// </summary>
        public ControlPoint SelectedCpFromChosen
        {
            get { return _selectedCpFromChosen; }
            set
            {
                _selectedCpFromChosen = value;
                OnPropertyChanged("SelectedCpFromChosen");
            }
        }

        private ObservableCollection<ControlPoint> _chosenControlPoints = new ObservableCollection<ControlPoint>();
        /// <summary>
        /// lista wybranych przez użytkownika punktów kontrolnych danej trasy
        /// </summary>
        public ObservableCollection<ControlPoint> ChosenControlPoints
        {
            get { return _chosenControlPoints; }
            set { _chosenControlPoints = value; }
        }


        #endregion

        #region Commands

        /// <summary>
        /// komenda odpowiedzialna za przeniesienie
        /// użytkownika do ekranu dodania noewj trasy i
        /// przygotowanie tego ekranu do wypełnienia
        /// </summary>
        public ICommand AddNewRoute { get; private set; }
        /// <summary>
        /// komenda odpowiedzialna za dodanie
        /// nowo zdefiniowanej trasy do listy tras
        /// </summary>
        public ICommand SaveNewRoute { get; private set; }
        /// <summary>
        /// komnda odpowiedzialna za dodanie nowego
        /// punktu kontrolnego do definiowanej trasy
        /// </summary>
        public ICommand AddControlPoint { get; private set; }
        /// <summary>
        /// komenda odpowiedzialna za usunięcie punktu kontrolnego
        /// z nowo definiowanej trasy
        /// </summary>
        public ICommand DeleteControlPoint { get; private set; }
        /// <summary>
        /// komenda odpowiadająca za usuwanie tras z listy
        /// </summary>
        public ICommand DeleteRoute { get; private set; }

        #endregion
    }
}
