using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL
{
    /// <summary>
    /// Enumerator stanu biegu
    /// </summary>
    public enum RunStatus
    {
        InProgress,
        Correct,
        WrongPath,
        Cheat
    }

    /// <summary>
    /// Rozszerzenie klasy User wygenerowanej z LINQ to SQL
    /// zawiera zmienne pomocnicze w procesie tworzenia zawodów
    /// </summary>
    public partial class User
    {
        /// <summary>
        /// Numer startowy danego uczestnika zawodów
        /// </summary>
        public int Identifier { get; set; }
        /// <summary>
        /// Ranking uczestnika zawodów,
        /// jego pozycja w rankingu
        /// liczona wg. czasów biegu
        /// </summary>
        public int? Rank { get; set; }
        /// <summary>
        /// Czas w jakim uczestnik przebiegł wyznaczoną trasę
        /// </summary>
        public TimeSpan Time { get; set; }
        /// <summary>
        /// Status zakończonego przez uczestnika biegu
        /// </summary>
        public RunStatus Status { get; set; }

        public string FullName
        {
            get { return this.Name + " " + this.Surname; }
        }
    }
}
