namespace Domain
{
    public class User
    {
        protected User()
        {
        }

        public User(string email, string password, string lastName, string firstName, string patronymic)
        {
            Email = email;
            Password = Domain.Password.Create(password);
            FirstName = firstName;
            LastName = lastName;
            Patronymic = patronymic;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }

        public override string ToString()
        {
            return $"FirstName: {FirstName}, LastName: {LastName}, Patronymic: {Patronymic}, Email: {Email}, Phone: {Phone}, Password: {Password}";
        }

        public bool IsDeleted { get; set; }
    }
}