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
using System.Data.Linq;

namespace DAL.Models
{
    /// <summary>
    /// Klasa zawierająca wszystkie metody potrzebne do
    /// zarządzania użytkownikami w bazie danych
    /// <example>
    /// przykład pokazuje obsługę klasy UserModel
    /// <code>
    /// UserModel model = new UserModel("C\\ProgramFiles\\Orienteering\\Database.sdf");
    /// 
    /// foreach(var item in model.GetAllUsers())
    /// {
    ///     // Porcess data 
    /// }
    /// </code>
    /// </example>
    /// </summary>
    public class UserModel : BaseModel
    {
        #region Fields & Properties



        #endregion

        #region Constructors

        /// <summary>
        /// Kontruktor klasy UserModel.
        /// Jedynym dopuszczalnym konstruktorem dla tej klasy jest
        /// konstruktor parametrowy, w którym należy podać
        /// connection string ze ścieżką do lokalnej bazy danych
        /// </summary>
        /// <param name="aConnectionString">ścieżka do lokalnej bazy danych w formacie .sdf</param>
        public UserModel(string aConnectionString)
            : base(aConnectionString)
        {

        }

        #endregion

        #region Public Methodes

        /// <summary>
        /// Metoda zwraca listę wszystkich użytkowników / zawodników dostępnych w bazie
        /// </summary>
        /// <returns>lista zawodników</returns>
        public IEnumerable<User> GetAllUsers()
        {
            using (DatabaseContext context = new DatabaseContext(_connectionString))
            {
                return context.Users.Where(u => u.IsActive).ToList();
            }
        }

        /// <summary>
        /// Metoda pozwala na pobranie uczestników dla zadanej listy
        /// </summary>
        /// <param name="aUsersList">obiekt listy uczestników</param>
        /// <returns>uczestnicy na danej liście</returns>
        public IEnumerable<User> GetUsersForUsersList(UsersList aUsersList)
        {
            using (DatabaseContext context = new DatabaseContext(_connectionString))
            {
                List<User> users = new List<User>();

                foreach (MMUserList mm in context.MMUserLists)
                {
                    if (mm.IDUsersList == aUsersList.ID)
                        users.Add(mm.User);
                }

                return users;
            }
        }

        /// <summary>
        /// Metoda zapisuje nowego zawodnika do bazy danych
        /// </summary>
        /// <param name="aUser">obiekt nowego zawodnika</param>
        /// <returns>metoda zwraca wartość logiczną:
        /// true - jeżeli operacja się powiodła
        /// false - jeżeli operacja zakończyła się niepowodzeniem</returns>
        public bool AddNewUser(User aUser)
        {
            using (DatabaseContext context = new DatabaseContext(_connectionString))
            {
                try
                {
                    context.Users.InsertOnSubmit(aUser);
                    context.SubmitChanges();

                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Metoda pozwala na usunięcie z bazy wybranego użytkownika
        /// </summary>
        /// <param name="aUser">obiekt użytkownika do usunięcia</param>
        /// <returns>metoda zwraca wartość logiczną:
        /// true - jeżeli operacja się powiodła
        /// false - jeżeli operacja zakończyła się niepowodzeniem</returns>
        public bool DeleteUser(User aUser)
        {
            using (DatabaseContext context = new DatabaseContext(_connectionString))
            {
                try
                {
                    User deleteUser = context.Users.Single(u => u.ID == aUser.ID);
                    deleteUser.IsActive = false;
                    context.SubmitChanges();

                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Metoda umożliwia pobranie wszystkich
        /// list uczestników z bazy danych
        /// </summary>
        /// <returns>lista list uczestników</returns>
        public IEnumerable<UsersList> GetAllUsersLists()
        {
            using (DatabaseContext context = new DatabaseContext(_connectionString))
            {
                return context.UsersLists.ToList();
            }
        }

        /// <summary>
        /// Metoda pozwala na dodanie nowej listy uczestników
        /// </summary>
        /// <param name="aUsersList">obiekt nowej listy uczestników</param>
        /// <returns>metoda zwraca wartość logiczną:
        /// true - jeżeli operacja się powiodła
        /// false - jeżeli operacja zakończyła się niepowodzeniem</returns>
        public bool AddNewUsersList(UsersList aUsersList)
        {
            using (DatabaseContext context = new DatabaseContext(_connectionString))
            {
                try
                {
                    context.UsersLists.InsertOnSubmit(aUsersList);
                    context.SubmitChanges();

                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Metoda pozwala na usunięcie listy użytkowników z bazy
        /// </summary>
        /// <param name="aUsersList">obiekt listy użytkowników do usunięcia</param>
        /// <returns>metoda zwraca wartość logiczną:
        /// true - jeżeli operacja się powiodła
        /// false - jeżeli operacja zakończyła się niepowodzeniem</returns>
        public bool DeleteUsersList(UsersList aUsersList)
        {
            using (DatabaseContext context = new DatabaseContext(_connectionString))
            {
                try
                {
                    UsersList deleteUsersList = context.UsersLists.Single(ul => ul.ID == aUsersList.ID);

                    context.UsersLists.DeleteOnSubmit(deleteUsersList);
                    context.SubmitChanges();

                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Metoda pozwala na dodanie uczestników do listy
        /// </summary>
        /// <param name="aUsersList">lista uczestników</param>
        /// <param name="aUsers">uczestnicy, którzy mają do tej listy zostać dodani</param>
        /// <returns>metoda zwraca wartość logiczną:
        /// true - jeżeli operacja się powiodła
        /// false - jeżeli operacja zakończyła się niepowodzeniem</returns>
        public bool AddUsersToList(UsersList aUsersList, IEnumerable<User> aUsers)
        {
            using (DatabaseContext context = new DatabaseContext(_connectionString))
            {
                try
                {
                    foreach (User user in aUsers)
                    {
                        context.MMUserLists.InsertOnSubmit(
                            new MMUserList
                            {
                                IDUser = user.ID,
                                IDUsersList = aUsersList.ID
                            });
                    }
                    context.SubmitChanges();

                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        #endregion

        #region Private Methodes



        #endregion
    }
}
