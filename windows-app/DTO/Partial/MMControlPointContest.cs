using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL
{
    /// <summary>
    /// Rozszerzenie klasy MMControlPointContest
    /// </summary>
    public partial class MMControlPointContest
    {
        /// <summary>
        /// numer identyfikacyjny punktu zakodowany w kodzie QR
        /// cała zakodowana w kodzie informacja ma postać CH/x,
        /// kod punktu to wartość x
        /// </summary>
        public int Ident { get; set; }
    }
}
