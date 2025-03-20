namespace ecomWebApi.Models

{
    public class User
    {
        public int UserId { get; set; } = 0;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public ICollection<Address> Addresses { get; set; }
    }
}
