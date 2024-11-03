using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Test_menu.Models
{
    public class Resource
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [DefaultValue(0)]
        public int Quantity { get; set; }


        public ICollection<Booking> Bookings { get; set; }

    }
}
