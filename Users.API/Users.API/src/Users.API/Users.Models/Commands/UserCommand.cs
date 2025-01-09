namespace User.Models.Commands
{
    public class UserCommand
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DocumentNumber { get; set; }
        public string Email { get; set; }
        
        public UserCommand(string firstName, string lastName, string documentNumber, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            DocumentNumber = documentNumber;
            Email = email;
        }
    }
};

