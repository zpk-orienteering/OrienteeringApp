using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL
{
    /// <summary>
    /// Rozszerzenie klasy Contest
    /// </summary>
    public partial class Contest
    {
        public override string ToString()
        {
            return this.Name + " (" + this.Date.ToShortDateString() + ")";
        }
    }
}
