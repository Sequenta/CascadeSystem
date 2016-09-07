namespace Domain
{
    public class Workplace
    {
        protected Workplace()
        {
        }

        public Workplace(User user, string name = "")
        {
            Owner = user;
            Name = !string.IsNullOrEmpty(name) ? name : Owner.Email;
        }

        /// <summary>
        /// Название рабочего места
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Владелец рабочего места
        /// </summary>
        public User Owner { get; set; }

        public override string ToString()
        {
            return $"Рабочее место: {Name}";
        }
    }
}