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
using Orienteering.Helpers.Commands;
using Orienteering.View;
using System.Collections.ObjectModel;
using DAL;
using System.Windows.Input;
using DAL.Models;
using Orienteering.Helpers;
using System.Windows;

namespace Orienteering.ViewModels
{
    /// <summary>
    /// View Model dla kreatora zawodów
    /// </summary>
    public class ContestViewModel : ViewModelBase
    {
        private Contest contest;
        /// <summary>
        /// obiekt zawodów
        /// </summary>
        public Contest Contest
        {
            get { return contest; }
        }

        private UserModel userModel;
        private RouteModel routeModel;
        private ContestModel contestModel;

        #region Singleton members

        private static ContestViewModel instance;

        private ContestViewModel()
        {
            #region Initialize

            contest = new Contest();

            userModel = new UserModel(DBHelper.ConnectionString);
            routeModel = new RouteModel(DBHelper.ConnectionString);
            contestModel = new ContestModel(DBHelper.ConnectionString);
            IsDataPrepared = false;
            ContestInProgress = false;
            EnableDataModification = true;

            AllRoutes = new ObservableCollection<Route>(routeModel.GetAllRoutes());
            AllUserLists = new ObservableCollection<UsersList>(userModel.GetAllUsersLists());
            Date = DateTime.Today;

            #endregion

            SaveRoute = new RelayCommand(() =>
            {
                if (SelectedRoute != null)
                {
                    SavedRoute = SelectedRoute;

                    if (SavedUserList != null && contest.IsOpen)
                        IsDataPrepared = true;
                }
                else
                {
                    MessageBox.Show("Prosze wybrać trase");
                }
            },
            () =>
            {
                return EnableDataModification;
            });
            SaveUserList = new RelayCommand(() =>
            {
                if (SelectedUserList != null)
                {
                    SavedUserList = SelectedUserList;

                    if (SavedRoute != null && contest.IsOpen)
                        IsDataPrepared = true;
                }
                else
                {
                    MessageBox.Show("Prosze wybrać listę uczestników");
                }
            },
            () =>
            {
                return EnableDataModification;
            });
            SaveData = new RelayCommand(() =>
            {
                if (!String.IsNullOrEmpty(Name)
                    && !String.IsNullOrEmpty(Info)
                    && Date != null)
                {
                    contest.Date = Date;
                    contest.Info = Info;
                    contest.Name = Name;
                    contest.IsOpen = true;

                    if (SaveRoute != null && SavedUserList != null)
                        IsDataPrepared = true;
                }
                else
                {
                    MessageBox.Show("Należy uzupełnić odpowiednie pola");
                }
            },
            () =>
            {
                return EnableDataModification;
            });
            StartStopContest = new RelayCommand(() =>
            {
                if (!ContestInProgress)
                {
                    if ((SavedRoute != null)
                        && (SavedUserList != null)
                        && contest.IsOpen)
                    {
                        contestModel.SaveContest(contest);

                        int i = 1;

                        List<Player> temp = new List<Player>();
                        List<User> contestUsers = userModel.GetUsersForUsersList(SavedUserList).ToList();

                        foreach (User u in contestUsers)
                        {
                            u.Identifier = i;
                            u.Status = RunStatus.InProgress;
                            Player p = new Player(u);

                            temp.Add(p);

                            i++;
                        }

                        Players = temp;

                        contestModel.AddUsersToContest(contest, contestUsers);
                        contestModel.AddControlPointsToContest(contest, routeModel.GetRouteMMControlPoints(SelectedRoute));

                        EnableDataModification = false;
                        ContestInProgress = true;
                    }
                }
                else
                {

                     if (MessageBox.Show("Czy napewno chcesz zakończyć zawody?",
                            "Zakończ zawody", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                     {
                        contestModel.CloseContest(this.contest);
                        CloseContest = true;
                     }
                }
            },
            () =>
            {
                return !CloseContest;
            });
            LoadPlayersData = new RelayCommand(() =>
            {
                LoadQRViewWF window = new LoadQRViewWF(this);
                window.OnAllPlayerDataLoaded += new LoadQRViewWF.AllPlayerDataLoaded(window_OnAllPlayerDataLoaded);
                window.OnPlayerDataLoaded += new LoadQRViewWF.PlayerDataLoaded(window_OnPlayerDataLoaded);
                window.OnCodeLoaded += new LoadQRViewWF.CodeLoaded(window_OnCodeLoaded);
                window.Show();
            },
            () =>
            {
                return !CloseContest;
            });
        }

        void window_OnCodeLoaded(object sender)
        {
            System.Media.SystemSounds.Beep.Play();
        }

        void window_OnPlayerDataLoaded(object sender, Player proceedPlayer)
        {
            contestModel.SaveUserTime(contest, proceedPlayer.User);
            contestModel.SaveUserRunStatus(contest, proceedPlayer.User);
        }

        void window_OnAllPlayerDataLoaded(object sender)
        {
            (sender as LoadQRViewWF).Close();

            Players.Sort((p1, p2) => 
            {
                if (p1.Status == RunStatus.Correct && p2.Status != RunStatus.Correct)
                    return -1;
                else if (p2.Status == RunStatus.Correct && p1.Status != RunStatus.Correct)
                    return 1;
                else 
                    return p1.ElapsedTime.CompareTo(p2.ElapsedTime); 
            });

            int rank = 1;
            foreach (Player p in Players)
            {
                p.User.Rank = rank;
                rank++;
            }

            List<User> contestUsers = new List<User>(Players.Count);
            Players.ForEach(p => contestUsers.Add(p.User));

            contestModel.AllocateRankToUsers(contest, contestUsers);

            List<Player> tempPlayers = new List<Player>(Players);
            Players = tempPlayers;

            MessageBox.Show("dane wszystkich uczestników zostały już wczytane");
        }

        public static ContestViewModel GetInstance()
        {
            if (instance == null)
            {
                instance = new ContestViewModel();
            }
            return instance;
        }

        #endregion

        #region Fields and Properties

        private string _name;
        /// <summary>
        /// nazwa zawodów
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private string _info;
        /// <summary>
        /// opis zawodów
        /// </summary>
        public string Info
        {
            get 
            { 
                return _info; 
            }

            set
            {
                _info = value;
                OnPropertyChanged("Info");
            }
        }

        private DateTime _date;
        /// <summary>
        /// data odbycia zawodów
        /// jezel zawody trwają dłużej niż dobę
        /// liczy się data rozpoczęcia zawodów
        /// data określa dzień, miesiąc i rok
        /// bez uwzględnienia konkretnej godziny
        /// </summary>
        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
                OnPropertyChanged("Date");
            }
        }

        private Route _selectedRoute;
        /// <summary>
        /// wybrana trasa dla danych zawodów
        /// </summary>
        public Route SelectedRoute
        {
            get
            {
                return _selectedRoute;
            }
            set
            {
                _selectedRoute = value;
                OnPropertyChanged("SelectedRoute");
                SelectedRouteControlPoints = new List<ControlPoint>(routeModel.GetRouteControlPoints(value));
            }
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

        private Route _savedRoute;
        /// <summary>
        /// wybrana i zapisana trasa dla danych zawodów
        /// </summary>
        public Route SavedRoute
        {
            get
            {
                return _savedRoute;
            }
            private set
            {
                _savedRoute = value;
                OnPropertyChanged("SavedRoute");
            }
        }

        private ObservableCollection<Route> _allRoutes;
        /// <summary>
        /// wszystkie trasy dsotępne w aplikacji
        /// </summary>
        public ObservableCollection<Route> AllRoutes
        {
            get
            {
                return _allRoutes;
            }
            set
            {
                _allRoutes = value;
                OnPropertyChanged("AllRoutes");
            }
        }

        private UsersList _selectedUserList;
        /// <summary>
        /// wybrana lista użytkowników dla danych zawodów
        /// </summary>
        public UsersList SelectedUserList
        {
            get
            {
                return _selectedUserList;
            }
            set
            {
                _selectedUserList = value;
                OnPropertyChanged("SelectedUserList");
                SelectedUsers = new List<User>(userModel.GetUsersForUsersList(value));
            }
        }

        private IEnumerable<User> _selectedUsers;
        /// <summary>
        /// Lista uczestników aktualnie zaznaczonej listy
        /// </summary>
        public IEnumerable<User> SelectedUsers
        {
            get { return _selectedUsers; }
            set
            {
                _selectedUsers = value;
                OnPropertyChanged("SelectedUsers");
            }
        }

        private UsersList _savedUserList;
        /// <summary>
        /// wybrana i zapisana lista użytkowników dla danych zawodów
        /// </summary>
        public UsersList SavedUserList
        {
            get
            {
                return _savedUserList;
            }
            private set
            {
                _savedUserList = value;
                OnPropertyChanged("SavedUserList");
            }
        }

        private ObservableCollection<UsersList> _allUserLists;
        /// <summary>
        /// wszystkie listy czuestników dostępne w aplikacji
        /// </summary>
        public ObservableCollection<UsersList> AllUserLists
        {
            get
            {
                return _allUserLists;
            }
            set
            {
                _allUserLists = value;
                OnPropertyChanged("AllUserLists");
            }
        }

        private List<Player> _players;
        /// <summary>
        /// lista uczestników gotowych do startu
        /// </summary>
        public List<Player> Players
        {
            get
            {
                return _players;
            }
            set
            {
                _players = value;
                OnPropertyChanged("Players");
            }
        }

        private bool _isDataPrepared;
        /// <summary>
        /// zmienna określa czy wszystkie dane
        /// potrzebne do rozpoczęcia zawodów zostały zdefiniowane
        /// </summary>
        public bool IsDataPrepared
        {
            get
            {
                return _isDataPrepared;
            }
            set
            {
                _isDataPrepared = value;
                OnPropertyChanged("IsDataPrepared");
            }
        }

        private bool _enableDataModification;
        /// <summary>
        /// Zmienna określająca czy w danym momencie
        /// możliwa jest modyfikacja danych zawodów
        /// np. zmiana nazwy, daty, trasy itd.
        /// Po rozpoczęciu zawodów możliwośćmodyfikacji jest automatycznie blokowana
        /// </summary>
        public bool EnableDataModification
        {
            get
            {
                return _enableDataModification;
            }
            set
            {
                _enableDataModification = value;
                OnPropertyChanged("EnableDataModification");
            }
        }

        private bool _contestInProgress;
        /// <summary>
        /// Zmienna określa czy dane zawody zostały rozpoczęte
        /// </summary>
        public bool ContestInProgress
        {
            get
            {
                return _contestInProgress;
            }
            set
            {
                _contestInProgress = value;
                OnPropertyChanged("ContestInProgress");

                if (value)
                    StartStopButtonDisplayValue = "zakończ";
                else
                    StartStopButtonDisplayValue = "rozpocznij";
            }
        }

        private string _startStopButtonDisplayValue;
        /// <summary>
        /// Zmienna określa tekst wyświetlany
        /// na przycisku Rozpocznik/Zakończ zawody
        /// w panelu zarządzania zawodami
        /// </summary>
        public string StartStopButtonDisplayValue
        {
            get
            {
                return _startStopButtonDisplayValue;
            }
            set
            {
                _startStopButtonDisplayValue = value;
                OnPropertyChanged("StartStopButtonDisplayValue");
            }
        }

        /// <summary>
        /// Własność określa czy dane wszystkich zawodników
        /// zostały już zeskanowane, jeżeli tak, to zawody
        /// można uznać za zakończone
        /// </summary>
        public bool AllPlayerDataLoaded { get; set; }

        /// <summary>
        /// Własność określa czy dane zawody zostały już zakończone
        /// </summary>
        public bool CloseContest { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// komenda zapisuje wybraną trase dla danych zawodów
        /// </summary>
        public ICommand SaveRoute { get; private set; }
        /// <summary>
        /// komenda zapisuje wybraną listę uczestników dla danych zawodów
        /// </summary>
        public ICommand SaveUserList { get; private set; }
        /// <summary>
        /// komenda rozpoczyna zawody
        /// </summary>
        public ICommand StartStopContest { get; private set; }
        /// <summary>
        /// komenda wczytuje plik z danymi odnośnie biegu danego uczestnika
        /// </summary>
        public ICommand LoadPlayersData { get; private set; }
        /// <summary>
        /// komenda zapisuje metryke zawodów
        /// </summary>
        public ICommand SaveData { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Metoda, która pozwala na odświerzenie list,
        /// metoda może być wywoływana z zewnątrz w przypadku
        /// jakiejkolwiek zmiany w kateogrii tras i list uczestników
        /// </summary>
        public void RefreshLists()
        {
            AllRoutes = new ObservableCollection<Route>(routeModel.GetAllRoutes());
            AllUserLists = new ObservableCollection<UsersList>(userModel.GetAllUsersLists());
        }

        /// <summary>
        /// Metoda umożliwia wczytanie danych już rozpoczętych wcześniej zawodów,
        /// wykorzystujemy tą metodę w momencie, gdy chcemy kontynuować sesje otwartych zawodów
        /// po uprzednim jej przerwaniu. Przerwanie może nastąpić poprzez wyłączenie aplikacji
        /// bez uprzedniego zakończenia zawodów.
        /// </summary>
        /// <param name="aContest">obiekt otwartych zawodów</param>
        /// <returns>true - jeżęli operacja wykonała się poprawnie i udało się odtworzyć wcześniej przerwane zawody
        /// false - w przypadku, gdy nie udało się z jakichś względów odtworzyć wcześniej przerwanych zawodów</returns>
        public bool LoadOpenContest(Contest aContest)
        {
            this.contest = aContest;

            this.Name = aContest.Name;
            this.Info = aContest.Info;

            List<Player> contestPlayers = new List<Player>();

            ContestInProgress = false;

            foreach (User u in contestModel.GetContestParticipants(this.contest))
            {
                contestPlayers.Add(new Player(u));
            }

            contestPlayers.Sort((p1, p2) =>
            {
                if (p1.Status == RunStatus.Correct && p2.Status != RunStatus.Correct)
                    return -1;
                else if (p2.Status == RunStatus.Correct && p1.Status != RunStatus.Correct)
                    return 1;
                else
                    return p1.ElapsedTime.CompareTo(p2.ElapsedTime);
            });

            Players = contestPlayers;

            EnableDataModification = false;
            ContestInProgress = true;
            IsDataPrepared = true;

            return true;
        }

        #endregion
    }
}
