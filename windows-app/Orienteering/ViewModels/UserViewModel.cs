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
    /// Klasa View Model dla widoku użytkowników oraz list użytkowników.
    /// Klasa UserViewModel jest singletonem, co oznacza, że w aplikacji
    /// może być utworzona tylko jedna jej instancja.
    /// </summary>
    public class UserViewModel : ViewModelBase
    {
        #region Singleton members

        private static UserViewModel instance;
        private UserModel model;

        private UserViewModel() 
        {
            model = new UserModel(DBHelper.ConnectionString);

            Users = new ObservableCollection<User>(model.GetAllUsers());
            UsersLists = new ObservableCollection<UsersList>(model.GetAllUsersLists());

            SaveNewUser = new RelayCommand(() =>
                {
                    if (!String.IsNullOrEmpty(NewUserName) && !String.IsNullOrEmpty(NewUserSurname))
                    {
                        User u = new User();
                        u.Name = NewUserName;
                        u.Surname = NewUserSurname;
                        u.Info = NewUserInfo;
                        u.IsActive = true;

                        Users.Add(u);
                        model.AddNewUser(u);

                        NewUserName = "";
                        NewUserSurname = "";
                        NewUserInfo = "";
                    }
                    else
                    {
                        MessageBox.Show("należy uzupełnić wymagane pola");
                    }
                });
            SaveNewUsersList = new RelayCommand(() =>
                {
                    if (!String.IsNullOrEmpty(NewUsersListName))
                    {
                        UsersList ul = new UsersList();
                        ul.Name = NewUsersListName;
                        ul.Info = NewUsersListInfo;

                        UsersLists.Add(ul);
                        model.AddNewUsersList(ul);
                        model.AddUsersToList(ul, ChosenUsers);

                        NewUsersListName = "";
                        NewUsersListInfo = "";

                        ChosenUsers.Clear();

                        ContestViewModel.GetInstance().RefreshLists();
                    }
                    else
                    {
                        MessageBox.Show("należy uzupełnić wymagane pola");
                    }
                });
            AddUserToChosensList = new RelayCommand(() =>
                {
                    if (SelectedUserFromAll != null)
                    {
                        ChosenUsers.Add(SelectedUserFromAll);
                    }
                });
            DeleteUserFromChosensList = new RelayCommand(() =>
                {
                    if (SelectedUserFromChosens != null)
                    {
                        ChosenUsers.Remove(SelectedUserFromChosens);
                    }
                });
            DeleteUser = new RelayCommand(() =>
                {
                    if (SelectedUser != null)
                    {
                        model.DeleteUser(SelectedUser);
                        Users.Remove(SelectedUser);                       
                    }
                });
            DeleteUsersList = new RelayCommand(() =>
                {
                    if (SelectedUsersList != null)
                    {
                        model.DeleteUsersList(SelectedUsersList);
                        UsersLists.Remove(SelectedUsersList);
                    }
                });
        }

        public static UserViewModel GetInstance()
        {
            if (instance == null)
            {
                instance = new UserViewModel();
            }
            return instance;
        }

        #endregion

        #region Fields and Properties

        private ObservableCollection<UsersList> _usersLists = new ObservableCollection<UsersList>();
        /// <summary>
        /// lista list uczetników
        /// </summary>
        public ObservableCollection<UsersList> UsersLists
        {
            get { return _usersLists; }
            set { _usersLists = value; }
        }

        private UsersList _selectedUsersList;
        /// <summary>
        /// aktualnie zaznaczona list auczestników
        /// </summary>
        public UsersList SelectedUsersList
        {
            get { return _selectedUsersList; }
            set
            {
                _selectedUsersList = value;
                OnPropertyChanged("SelectedUsersList");
                SelectedUsers = new List<User>(model.GetUsersForUsersList(value));
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

        private string _newUsersListName;

        public string NewUsersListName
        {
            get { return _newUsersListName; }
            set
            {
                _newUsersListName = value;
                OnPropertyChanged("NewUsersListName");
            }
        }

        private string _newUsersListInfo;

        public string NewUsersListInfo
        {
            get { return _newUsersListInfo; }
            set
            {
                _newUsersListName = value;
                OnPropertyChanged("NewUsersListInfo");
            }
        }

        private ObservableCollection<User> _users = new ObservableCollection<User>();
        /// <summary>
        /// lista wszystkich dostępnych w aplikacji uczestników
        /// </summary>
        public ObservableCollection<User> Users
        {
            get { return _users; }
            set { _users = value; }
        }

        private User _selectedUser;
        /// <summary>
        /// obiekt aktualnie zaznaczonego na liście uczestnika
        /// </summary>
        public User SelectedUser
        {
            get { return _selectedUser; }
            set 
            {
                _selectedUser = value;
                OnPropertyChanged("SelectedUser");
            }
        }

        private string _newUserName;
        /// <summary>
        /// imie nowo definiowanego uczestniku
        /// </summary>
        public string NewUserName
        {
            get { return _newUserName; }
            set
            {
                _newUserName = value;
                OnPropertyChanged("NewUserName");
            }
        }

        private string _newUserSurname;
        /// <summary>
        /// nazwisko nowo definiowanego uczestniku
        /// </summary>
        public string NewUserSurname
        {
            get { return _newUserSurname; }
            set
            {
                _newUserSurname = value;
                OnPropertyChanged("NewUserSurname");
            }
        }

        private string _newUserInfo;
        /// <summary>
        /// dodatkowe informacje o nowo definiowanym uczestniku
        /// </summary>
        public string NewUserInfo
        {
            get { return _newUserInfo; }
            set
            {
                _newUserInfo = value;
                OnPropertyChanged("NewUserInfo");
            }
        }

        private ObservableCollection<User> _chosenUsers = new ObservableCollection<User>();
        /// <summary>
        /// lista wybranych uczestników dla danej listy
        /// </summary>
        public ObservableCollection<User> ChosenUsers
        {
            get { return _chosenUsers; }
            set { _chosenUsers = value; }
        }

        private User _selectedUserFromChosens;
        /// <summary>
        /// zaznaczony uczestnik z listy wybranch czestników
        /// </summary>
        public User SelectedUserFromChosens
        {
            get { return _selectedUserFromChosens; }
            set
            {
                _selectedUserFromChosens = value;
                OnPropertyChanged("SelectedUserFromChosens");
            }
        }

        private User _selectedUserFromAll;
        /// <summary>
        /// zaznaczony uczestnik z globalnej listy wszystkich uczestników dostepnych w aplikacji
        /// </summary>
        public User SelectedUserFromAll
        {
            get { return _selectedUserFromAll; }
            set
            {
                _selectedUserFromAll = value;
                OnPropertyChanged("SelectedUserFromAll");
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Komenda przenosi do ekranu dodania nowego użytkownika
        /// oraz prszygotowuje pola do wypełnienia
        /// </summary>
        public ICommand AddNewUser { get; private set; }
        /// <summary>
        /// Komenda zapisuje dane użytkownika
        /// </summary>
        public ICommand SaveNewUser { get; private set; }
        /// <summary>
        /// Komenda przenosi do ekranu dodania nowej listy użytkowników
        /// oraz przygotowuje odpowiednie pola do wypełnienia
        /// </summary>
        public ICommand AddNewUsersList { get; private set; }
        /// <summary>
        /// Komenda pozwala na zapisanie nowo zdefiniowanej listy użytkowników
        /// </summary>
        public ICommand SaveNewUsersList { get; private set; }
        /// <summary>
        /// Komenda dodaje danego użytkownika do listy użytkowników
        /// </summary>
        public ICommand AddUserToChosensList { get; private set; }
        /// <summary>
        /// Komenda usuwa danego użytkownika z listy użytkowników
        /// </summary>
        public ICommand DeleteUserFromChosensList { get; private set; }
        /// <summary>
        /// Komenda usuwa użytkownika z bazy danych 
        /// (użytkownik jest oznaczany jako nieaktywny - soft delete)
        /// </summary>
        public ICommand DeleteUser { get; private set; }
        /// <summary>
        /// Komenda usuwa listę użytkowników z bazy danych
        /// </summary>
        public ICommand DeleteUsersList { get; set; }

        #endregion
    }
}
