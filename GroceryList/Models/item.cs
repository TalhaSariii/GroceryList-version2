using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;

namespace GroceryList.Models
{
    public class Item
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ?Name { get; set; }
        public string ?Type { get; set; }
        public int Amount { get; set; }
        public double Price { get; set; }
        public string? ShopName { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }

        public bool IsEditing { get; set; } 


        public Item()
        {
            CreateDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
            IsDeleted = false;
            IsActive = true;
            
        }
    }
}
