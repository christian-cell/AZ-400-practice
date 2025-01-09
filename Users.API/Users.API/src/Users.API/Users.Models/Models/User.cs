namespace User.Models.Models
{
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DocumentNumber { get; set; }
        public string Email { get; set; }
        
        public User(string firstName, string lastName, string documentNumber, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            DocumentNumber = documentNumber;
            Email = email;
        }
    }
};

