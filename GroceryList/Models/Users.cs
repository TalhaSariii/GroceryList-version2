using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroceryList.Models
{
    public class Users
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }

        public string  ProfilePictureURL { get; set; }

        public Users()
        {
            ProfilePictureURL = "https://t4.ftcdn.net/jpg/03/46/93/61/360_F_346936114_RaxE6OQogebgAWTalE1myseY1Hbb5qPM.jpg";
           
        }
    }
}
