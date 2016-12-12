using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;

namespace Orienteering.ViewModels
{
    /// <summary>
    /// Klasa reprezentująca zawodnika
    /// biorącego udział w konkretnych zawodach
    /// </summary>
    public class Player : ViewModelBase
    {

        public Player(User aUser)
        {
            User = aUser;

        }

        /// <summary>
        /// unikatowy identyfikator zawodnika,
        /// identyfikator musi być unikatowy tylko
        /// w ramach poszczególnych zawodów
        /// </summary>
        public int Identifier 
        {
            get { return User.Identifier; }
            set { User.Identifier = value; }
        }
        /// <summary>
        /// obiekt reprezentujący dane zawodnika
        /// </summary>
        public User User { get; set; }
        /// <summary>
        /// określa czy zawodnik zmieniał czas systemowy zegara w telefonie komórkowym
        /// podczas biegu
        /// </summary>
        public bool CheetingFlag { get; set; }
        /// <summary>
        /// aktualny status uczestnika w zawodach (kontynuuje bieg, ukonczyl bieg, zdyskwalifikowany)
        /// </summary>
        public string DisplayStatus
        {
            get
            {
                switch (Status)
                {
                    case RunStatus.InProgress:
                        return "W trakcie zawodów";
                    
                    case RunStatus.WrongPath:
                        return "Niepoprawna kolejność odwiedzenia punktów kontrolnych" + (CheetingFlag ? " / Próba oszustwa" : "");

                    case RunStatus.Correct:
                        return "Bieg ukończony poprawnie" + (CheetingFlag ? " / Próba oszustwa" : "");

                    case RunStatus.Cheat:
                        return "Próba oszóstwa";

                    default:
                        return "Nieokreślony";
                }
            }
        }

        public RunStatus Status
        {
            get { return User.Status; }
            set 
            { 
                User.Status = value;
                OnPropertyChanged("DisplayStatus");
            }
        }
        /// <summary>
        /// czas, w którym uczestnik rozpoczął bieg w formacie hh/mm/ss
        /// </summary>
        private DateTime _startTime;
        public DateTime StartTime
        {
            get { return _startTime; }
            set
            {
                _startTime = value;
                OnPropertyChanged("StartTime");
            }
        }
        /// <summary>
        /// czas, w którym uczestnik zakończył bieg w formacie hh/mm/ss
        /// </summary>
        private DateTime _finishTime;
        public DateTime FinishTime
        {
            get { return _finishTime; }
            set
            {
                _finishTime = value;
                OnPropertyChanged("FinishTime");
            }
        }
        /// <summary>
        /// czas biegu uczestnika zawodów
        /// </summary>
        public TimeSpan ElapsedTime
        {
            get { return User.Time; }
            set
            {
                User.Time = value;
                OnPropertyChanged("ElapsedTime");
                OnPropertyChanged("Time");
            }
        }
        /// <summary>
        /// czas biegu uczestnika w zawodach w postaci ciągu znaków
        /// </summary>
        private string _time;
        public string Time
        {
            get
            {
                if (ElapsedTime != null)
                    return ElapsedTime.ToString();
                else
                    return "---";
            }
        }
    }
}
