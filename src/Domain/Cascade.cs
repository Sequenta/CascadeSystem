using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public class Cascade
    {
        public int CurrentLevelNumber { get; protected set; }
        public bool Completed { get; protected set; }
        public List<CascadeLevel> Levels { get; set; }

        public CascadeLevel CurrentLevel
        {
            get { return Levels.FirstOrDefault(x => x.OrderNumber == CurrentLevelNumber); }
        }

        public Cascade()
        {
            CurrentLevelNumber = 0;
            Levels = new List<CascadeLevel>();
        }

        public void AddLevel(CascadeLevel level)
        {
            Levels.Add(level);
        }

        public void SetCurrentLevel(int number)
        {
            CurrentLevelNumber = number;
            if (CurrentLevelNumber == Levels.Count)
            {
                Completed = true;
            }
        }
    }
}