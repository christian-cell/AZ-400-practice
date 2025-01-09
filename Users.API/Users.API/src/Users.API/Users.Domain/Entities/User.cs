using User.Repository.Entities;

namespace User.Domain.Entities
{
    public class User: EntityBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DocumentNumber { get; set; }
        public string Email { get; set; }
        
        public User(string firstName, string lastName, string documentNumber, string email)
        {
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            DocumentNumber = documentNumber ?? throw new ArgumentNullException(nameof(documentNumber));
            Email = email ?? throw new ArgumentNullException(nameof(email));
        }
        
        
    }
};

