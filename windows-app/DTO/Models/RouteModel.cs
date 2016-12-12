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
    /// zarządzania trasami w bazie danych
    /// <example>
    /// przykład pokazuje obsługę klasy RouteModel
    /// <code>
    /// RouteModel model = new RouteModel("C\\ProgramFiles\\Orienteering\\Database.sdf");
    /// 
    /// foreach(var item in model.GetAllRoutes())
    /// {
    ///     // Porcess data 
    /// }
    /// </code>
    /// </example>
    /// </summary>
    public class RouteModel :BaseModel
    {
        #region Constructors

        /// <summary>
        /// Kontruktor klasy RouteModel.
        /// Jedynym dopuszczalnym konstruktorem dla tej klasy jest
        /// konstruktor parametrowy, w którym należy podać
        /// connection string ze ścieżką do lokalnej bazy danych
        /// </summary>
        /// <param name="aConnectionString">ścieżka do lokalnej bazy danych w formacie .sdf</param>
        public RouteModel(string aConnectionString)
            : base(aConnectionString)
        {

        }

        #endregion

        #region Public Methodes

        /// <summary>
        /// Metoda pozwala na pobranie wszystkich tras znajdujących się w bazie danych
        /// </summary>
        /// <returns>lista tras</returns>
        public IEnumerable<Route> GetAllRoutes()
        {
            using (DatabaseContext context = new DatabaseContext(_connectionString))
            {
                return context.Routes.ToList();
            }
        }

        /// <summary>
        /// Metoda pozwala na pobranie listy punktów kontrolnych
        /// bez uwzględnienia punktów startowych i końcowych
        /// </summary>
        /// <returns>lista punktów kontrolnych</returns>
        public IEnumerable<ControlPoint> GetControlPoints()
        {
            using (DatabaseContext context = new DatabaseContext(_connectionString))
            {
                return context.ControlPoints.Where(cp => (cp.Text.StartsWith(@"CH/") && cp.IsActive)).ToList();
            }
        }

        /// <summary>
        /// Metoda pozwala na pobranie listy punktów kontrolnych dla danej trasy
        /// </summary>
        /// <param name="aRoute">obiekt trasy</param>
        /// <returns>lista punktów kontrolnych danej trasy</returns>
        public IEnumerable<ControlPoint> GetRouteControlPoints(Route aRoute)
        {
            using (DatabaseContext context = new DatabaseContext(_connectionString))
            {
                List<ControlPoint> routeControlPoints = new List<ControlPoint>();

                foreach (MMControlPointRoute mm in context.MMControlPointRoutes)
                {
                    if (mm.IDRoute == aRoute.ID)
                        routeControlPoints.Add(mm.ControlPoint);
                }

                return routeControlPoints;
            }
        }

        /// <summary>
        /// Metoda pozwala na pobranie listy punktów kontrolnych dla danej trasy
        /// </summary>
        /// <param name="aRoute">obiekt trasy</param>
        /// <returns>lista punktów kontrolnych danej trasy</returns>
        public IEnumerable<MMControlPointRoute> GetRouteMMControlPoints(Route aRoute)
        {
            using (DatabaseContext context = new DatabaseContext(_connectionString))
            {
                List<MMControlPointRoute> routeControlPoints = new List<MMControlPointRoute>();

                foreach (MMControlPointRoute mm in context.MMControlPointRoutes)
                {
                    if (mm.IDRoute == aRoute.ID)
                        routeControlPoints.Add(mm);
                }

                return routeControlPoints;
            }
        }

        /// <summary>
        /// Metoda pozwala na dodanie nowej trasy do bazy danych
        /// </summary>
        /// <param name="aRoute">obiekt nowej trasy</param>
        /// <returns>metoda zwraca wartość logiczną:
        /// true - jeżeli operacja się powiodła
        /// false - jeżeli operacja zakończyła się niepowodzeniem</returns>
        public bool AddNewRoute(Route aRoute)
        {
            using (DatabaseContext context = new DatabaseContext(_connectionString))
            {
                try
                {
                    context.Routes.InsertOnSubmit(aRoute);
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
        /// Metoda pozwala na usnięcie trasy z bazy danych
        /// </summary>
        /// <param name="aRoute">obiekt trasy</param>
        /// <returns>metoda zwraca wartość logiczną:
        /// true - jeżeli operacja się powiodła
        /// false - jeżeli operacja zakończyła się niepowodzeniem</returns>
        public bool DeleteRoute(Route aRoute)
        {
            using (DatabaseContext context = new DatabaseContext(_connectionString))
            {
                try
                {
                    Route deleteRoute = context.Routes.Single(r => r.ID == aRoute.ID);

                    context.Routes.DeleteOnSubmit(deleteRoute);
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
        /// Metoda pozwala na dodanie listy punktów kontrolnych dla danej trasy
        /// </summary>
        /// <param name="aRoute">obiekt trasy</param>
        /// <param name="aControlPoints">lista punktów kontrolnych</param>
        /// <returns>metoda zwraca wartość logiczną:
        /// true - jeżeli operacja się powiodła
        /// false - jeżeli operacja zakończyła się niepowodzeniem</returns>
        public bool AddControlPointsToRoute(Route aRoute, IEnumerable<ControlPoint> aControlPoints)
        {
            using (DatabaseContext context = new DatabaseContext(_connectionString))
            {
                try
                {
                    int iterator = 0;
                    foreach (ControlPoint controlPoint in aControlPoints)
                    {
                        context.MMControlPointRoutes.InsertOnSubmit(
                            new MMControlPointRoute
                            {
                                IDControlPoint = controlPoint.ID,
                                IDRoute = aRoute.ID,
                                Order = iterator
                            });
                        iterator++;
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
    }
}
