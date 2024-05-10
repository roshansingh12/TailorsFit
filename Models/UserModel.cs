using System.ComponentModel.DataAnnotations;

namespace Tailors_fitv0._2.Models
{
    public class UserModel
    {
        [Key]
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Age { get; set; }
        public string gender { get; set; }
        public string address { get; set; }
        public string phone_No { get; set; }
        public string email { get; set; }
    }
}
