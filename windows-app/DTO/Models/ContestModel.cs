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
using System.Collections;

namespace DAL.Models
{
    /// <summary>
    /// Klasa zawierająca wszystkie metody potrzebne do
    /// zarządzania zawodami
    /// <example>
    /// przykład pokazuje obsługę klasy ContestModel
    /// <code>
    /// ContestModel model = new ContestModel("C\\ProgramFiles\\Orienteering\\Database.sdf");
    /// </code>
    /// </example>
    /// </summary>
    public class ContestModel : BaseModel
    {
        #region Fields & Properties



        #endregion

        #region Constructors

        /// <summary>
        /// Kontruktor klasy ContestModel.
        /// Jedynym dopuszczalnym konstruktorem dla tej klasy jest
        /// konstruktor parametrowy, w którym należy podać
        /// connection string ze ścieżką do lokalnej bazy danych
        /// </summary>
        /// <param name="aConnectionString">ścieżka do lokalnej bazy danych w formacie .sdf</param>
        public ContestModel(string aConnectionString)
            : base(aConnectionString)
        {

        }

        #endregion

        #region Public Methodes

        /// <summary>
        /// Metoda pozwala na pobranie wszystkich obiektów zawodów,
        /// które nie zostały jescze zakończone
        /// </summary>
        /// <returns>lista obiektów zawodów</returns>
        public List<Contest> GetAllActiveContests()
        {
            using (DatabaseContext context = new DatabaseContext(_connectionString))
            {
                try
                {
                    return context.Contests.Where(c => c.IsOpen).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Metoda pozwala na dodanie uczestników do zawodów
        /// </summary>
        /// <param name="aContest">obiekt zawodów</param>
        /// <param name="aUsers">lista uczestników zawodów</param>
        /// <returns>metoda zwraca wartość logiczną:
        /// true - jeżeli operacja się powiodła
        /// false - jeżeli operacja zakończyła się niepowodzeniem</returns>
        public bool AddUsersToContest(Contest aContest, IEnumerable<User> aUsers)
        {
            using (DatabaseContext context = new DatabaseContext(_connectionString))
            {
                try
                {
                    foreach(User user in aUsers)
                    {
                        context.MMUserContests.InsertOnSubmit(
                            new MMUserContest
                            {
                                IDContest = aContest.ID,
                                IDUser = user.ID,
                                Identifier = user.Identifier
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

        /// <summary>
        /// Metoda pozwala na trasy zawodów
        /// </summary>
        /// <param name="aContest">obiekt zawodów</param>
        /// <param name="aControlPoints">lista punktów kontrolnych</param>
        /// <returns>metoda zwraca wartość logiczną:
        /// true - jeżeli operacja się powiodła
        /// false - jeżeli operacja zakończyła się niepowodzeniem</returns>
        public bool AddControlPointsToContest(Contest aContest, IEnumerable<MMControlPointRoute> aControlPoints)
        {
            using (DatabaseContext context = new DatabaseContext(_connectionString))
            {
                try
                {
                    foreach (MMControlPointRoute controlPoint in aControlPoints)
                    {
                        context.MMControlPointContests.InsertOnSubmit(
                            new MMControlPointContest
                            {
                                IDContest = aContest.ID,
                                IDControlPoint = controlPoint.IDControlPoint,
                                Order = controlPoint.Order
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

        /// <summary>
        /// Metoda pozwala na zapis rozpoczętych zawodów do bazy danych
        /// obiekt zawodów zostaje zapisany ze statusem "Open", w bazie
        /// przechowywane są na bierząco dane odnośnie zawodów,
        /// aby w wypadku awarii programu możliwe było odtworzenie stanu poprzedniego
        /// </summary>
        /// <param name="aContest">obiekt zawodów</param>
        /// <returns>metoda zwraca wartość logiczną:
        /// true - jeżeli operacja się powiodła
        /// false - jeżeli operacja zakończyła się niepowodzeniem</returns>
        public bool SaveContest(Contest aContest)
        {
            using (DatabaseContext context = new DatabaseContext(_connectionString))
            {
                try
                {
                    context.Contests.InsertOnSubmit(aContest);
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
        /// Metoda pozwala na zakończenie danych zawodów
        /// </summary>
        /// <param name="aContest">obiekt zawodów</param>
        /// <returns>metoda zwraca wartość logiczną:
        /// true - jeżeli operacja się powiodła
        /// false - jeżeli operacja zakończyła się niepowodzeniem</returns>
        public bool CloseContest(Contest aContest)
        {
            using (DatabaseContext context = new DatabaseContext(_connectionString))
            {
                try
                {
                    Contest selectedContest = context.Contests.Single(c => c.ID == aContest.ID);
                    selectedContest.IsOpen = false;
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
        /// Metoda pozwala na nadanie miejsc poszczególnym uczestnikom zawodów
        /// zgodnie z ich czasami biegu
        /// </summary>
        /// <param name="aContest">obiekt zawodów</param>
        /// <param name="aUsers">lista uczestników</param>
        /// <returns>metoda zwraca wartość logiczną:
        /// true - jeżeli operacja się powiodła
        /// false - jeżeli operacja zakończyła się niepowodzeniem</returns>
        public bool AllocateRankToUsers(Contest aContest, IEnumerable<User> aUsers)
        {
            using (DatabaseContext context = new DatabaseContext(_connectionString))
            {
                try
                {
                    foreach(User user in aUsers)
                    {
                        MMUserContest participant = context.MMUserContests.Single
                            (p => (p.Identifier == user.Identifier
                                    && p.IDContest == aContest.ID
                                    && p.IDUser == user.ID));
                        participant.Rank = user.Rank;
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

        /// <summary>
        /// Metoda pozwala na zapisaniu w bazie danych
        /// czasu danego uczestnika w jakim ukończył on swój bieg
        /// </summary>
        /// <param name="aContest">obiekt zawodów</param>
        /// <param name="aUser">uczestnik zawodów</param>
        /// <returns>metoda zwraca wartość logiczną:
        /// true - jeżeli operacja się powiodła
        /// false - jeżeli operacja zakończyła się niepowodzeniem</returns>
        public bool SaveUserTime(Contest aContest, User aUser)
        {
            using (DatabaseContext context = new DatabaseContext(_connectionString))
            {
                try
                {
                    MMUserContest participant = context.MMUserContests.Single
                        (p => (p.IDContest == aContest.ID
                                && p.IDUser == aUser.ID
                                && p.Identifier == aUser.Identifier));
                    participant.Time = aUser.Time.ToString();

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
        /// Metoda pozwala na zapisanie w bazie danych
        /// statusu ukończenia biegu uczestnika
        /// </summary>
        /// <param name="aContest">obiekt zawodów</param>
        /// <param name="aUser">uczestnik zawodów</param>
        /// <returns>metoda zwraca wartość logiczną:
        /// true - jeżeli operacja się powiodła
        /// false - jeżeli operacja zakończyła się niepowodzeniem</returns>
        public bool SaveUserRunStatus(Contest aContest, User aUser)
        {
            using (DatabaseContext context = new DatabaseContext(_connectionString))
            {
                try
                {
                    MMUserContest participant = context.MMUserContests.Single
                        (p => (p.IDContest == aContest.ID
                                && p.IDUser == aUser.ID
                                && p.Identifier == aUser.Identifier));
                    participant.Status = (int)aUser.Status;

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
        /// Metoda zwraca uczestnikó danych zawodów
        /// </summary>
        /// <param name="aContest">obiekt zawodów</param>
        /// <returns>lista uczestników zawodów</returns>
        public IEnumerable<User> GetContestParticipants(Contest aContest)
        {
            using (DatabaseContext context = new DatabaseContext(_connectionString))
            {
                List<User> contestParticipants = new List<User>();

                foreach (MMUserContest mm in context.MMUserContests)
                {
                    if (mm.IDContest == aContest.ID)
                    {
                        TimeSpan userTime;

                        User u = mm.User;
                        u.Identifier = mm.Identifier;
                        u.Rank = mm.Rank;
                        u.Status = mm.Status != null ? (RunStatus)mm.Status : RunStatus.InProgress;
                        if (TimeSpan.TryParse(mm.Time, out userTime)) { u.Time = userTime; }

                        contestParticipants.Add(u);
                    }
                }

                return contestParticipants;
            }
        }

        /// <summary>
        /// Metoda zwraca punkty kontrolne na trasie danych zawodów
        /// </summary>
        /// <param name="aContest">obiekt zawodów</param>
        /// <returns>lista punktów kontrolnych na trasie zawodów</returns>
        public IEnumerable<MMControlPointContest> GetContestControlPoints(Contest aContest)
        {
            using (DatabaseContext context = new DatabaseContext(_connectionString))
            {
                List<MMControlPointContest> contestControlPoints = new List<MMControlPointContest>();

                foreach (MMControlPointContest mm in context.MMControlPointContests)
                {
                    if (mm.IDContest == aContest.ID)
                    {
                        string[] temp = mm.ControlPoint.Text.Split('/');
                        int id;
                        if (temp.Length == 2 && int.TryParse(temp[1], out id))
                        {
                            mm.Ident = id;
                            contestControlPoints.Add(mm);
                        }
                    }
                }

                return contestControlPoints;
            }
        }

        #endregion

        #region Private Methodes



        #endregion
    }
}
