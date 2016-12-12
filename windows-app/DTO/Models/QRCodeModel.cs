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
using DAL.Models;

namespace DAL.Models
{
    /// <summary>
    /// Klasa zawierająca wszystkie metody potrzebne do
    /// zarządzania kodami punktów kontrolnych w bazie danych
    /// <example>
    /// przykład pokazuje obsługę klasy QRCodeModel
    /// <code>
    /// QRCodeModel model = new QRCodeModel("C\\ProgramFiles\\Orienteering\\Database.sdf");
    /// 
    /// foreach(var item in model.GetAllControlPoints())
    /// {
    ///     // Porcess data 
    /// }
    /// </code>
    /// </example>
    /// </summary>
    public class QRCodeModel : BaseModel
    {
        #region Fields and Properties



        #endregion

        #region Constructors

        /// <summary>
        /// Kontruktor klasy QRCodeModel.
        /// Jedynym dopuszczalnym konstruktorem dla tej klasy jest
        /// konstruktor parametrowy, w którym należy podać
        /// connection string ze ścieżką do lokalnej bazy danych
        /// </summary>
        /// <param name="aConnectionString">ścieżka do lokalnej bazy danych w formacie .sdf</param>
        public QRCodeModel(string aConnectionString)
            : base(aConnectionString)
        {

        }

        #endregion

        #region Public Methodes

        /// <summary>
        /// Metoda pobiera wszystkie kody QR z bazy danych
        /// </summary>
        /// <returns>lista punktów kontrolnych / kodów QR</returns>
        public IEnumerable<ControlPoint> GetAllControlPoints()
        {
            using (DatabaseContext context = new DatabaseContext(_connectionString))
            {
                return context.ControlPoints.Where(cp => cp.IsActive).ToList();
            }
        }

        /// <summary>
        /// Metoda pozwala na dodanie nowego punktu kontrolnego / kodu QR
        /// do bazy danych
        /// </summary>
        /// <param name="aControlPoint">obiekt nowego punktu kontrolnego / kodu QR</param>
        ///<returns>metoda zwraca wartość logiczną:
        /// true - jeżeli operacja się powiodła
        /// false - jeżeli operacja zakończyła się niepowodzeniem</returns>
        public bool AddNewControlPoint(ControlPoint aControlPoint)
        {
            using (DatabaseContext context = new DatabaseContext(_connectionString))
            {
                try
                {
                    context.ControlPoints.InsertOnSubmit(aControlPoint);
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
        /// Metoda pozwala na usunięcie punktu kontrolnego / kodu QR
        /// z bazy danych
        /// </summary>
        /// <param name="aControlPoint">obiekt punktu kontrolnego / kodu QR do usunięcia</param>
        /// <returns>metoda zwraca wartość logiczną:
        /// true - jeżeli operacja się powiodła
        /// false - jeżeli operacja zakończyła się niepowodzeniem</returns>
        public bool DeleteControlPoint(ControlPoint aControlPoint)
        {
            using (DatabaseContext context = new DatabaseContext(_connectionString))
            {
                try
                {
                    ControlPoint deleteControlPoint = context.ControlPoints.Single(cp => cp.ID == aControlPoint.ID);

                    deleteControlPoint.IsActive = false;
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
        /// Metoda pozwala określić czy w bazie danych
        /// znajduje się już punkt kontrolny, który
        /// zawiera ten sam zakodowany ciąg znaków
        /// </summary>
        /// <param name="aText">ciąg znaków do zakodowania</param>
        /// <returns>true - jeżeli w bazie istnienieje już kod o tym samym zakodowanym ciągu znaków co przekazany w parametrze
        /// false - jeżeli w bazie nie ma takiego kodu</returns>
        public bool IsAnotherCodeWithSameText(string aText)
        {
            using (DatabaseContext context = new DatabaseContext(_connectionString))
            {
                foreach (ControlPoint cp in context.ControlPoints)
                {
                    if ((cp.Text == aText) && (cp.IsActive))
                        return true;
                }

                return false;
            }
        }

        #endregion

        #region Private Methodes



        #endregion
    }
}
