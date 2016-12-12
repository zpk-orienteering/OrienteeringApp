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

namespace DAL.Models
{
    /// <summary>
    /// Klasa bazowa dla wszystkich pozostałych modeli
    /// </summary>
    public class BaseModel
    {
        /// <summary>
        /// Ścieżka do pliku z lokalną bazą danych
        /// </summary>
        protected string _connectionString;

        /// <summary>
        /// Konstruktor klasy,
        /// aby utworzyć instancję klasy BaseModel
        /// należy zdefiniować ścieżkę pliku z lokalną bazą danych
        /// </summary>
        /// <param name="aConnectionString">Ścieżka do pliku z lokalną bazą danych</param>
        public BaseModel(string aConnectionString)
        {
            _connectionString = aConnectionString;
        }

        /// <summary>
        /// Metoda pozwala na ustalenie istenienia pliku bazy danych
        /// przy zdefiniowanej w connection string ścieżce, oraz
        /// sprawdzenie czy dane z bazy są możliwe do odczytu i zapisu
        /// </summary>
        /// <param name="aConntestionString">ścieżka do pliku z lokalną bazą danych aplikacji</param>
        /// <returns>
        /// true - baza danych istenieje i jest dostępna do zapisu / odczytu
        /// false - baza danych nie istenieje lub nie jest dostępna do zapisu / odczytu</returns>
        public static bool DBSelfTest(string aConntestionString)
        {
            using (DatabaseContext context = new DatabaseContext(aConntestionString))
            {
                 return context.DatabaseExists();
            }
        }
    }
}
