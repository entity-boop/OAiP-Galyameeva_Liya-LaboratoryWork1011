using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace laba10
{
    internal class Photographer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string FIO { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public required string Genre { get; set; }
        public DateOnly DateOfFirstPost { get; set; }
        public required string Password { get; set; }
        public string Role { get; set; } = "User";

        public Photographer() { }

        public Photographer(int id, string fio, string email, string phone, string genre, DateOnly dateoffirstpost, string password, string role)
        {
            Id = id;
            FIO = fio;
            Email = email;
            Phone = phone;
            Genre = genre;
            DateOfFirstPost = dateoffirstpost;
            Password = password;
            Role = role;
        }
    }
}
