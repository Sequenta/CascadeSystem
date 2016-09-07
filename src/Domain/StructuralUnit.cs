using System.Collections.Generic;

namespace Domain
{
    public class StructuralUnit
    {
        protected StructuralUnit() { }

        public StructuralUnit(string name, Workplace headWorkplace, StructuralUnit parent = null)
        {
            Name = name;
            Parent = parent;
            HeadWorkplace = headWorkplace;
            Workplaces = new List<Workplace>
            {
                headWorkplace
            };
        }

        /// <summary>
        /// Название отдела
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Родительский отдел
        /// </summary>
        /// <remarks>Если null, то это - корневой отдел</remarks>
        public StructuralUnit Parent { get; set; }

        /// <summary>
        /// Рабочие места в отделе
        /// </summary>
        public List<Workplace> Workplaces { get; set; }

        /// <summary>
        /// Рабочее место руководителя
        /// </summary>
        public Workplace HeadWorkplace { get; set; }

        public override string ToString()
        {
            return $"Отдел: {Name}";
        }

        public bool IsDeleted { get; set; }
    }
}