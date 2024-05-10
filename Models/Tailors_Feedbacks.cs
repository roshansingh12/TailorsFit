using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Tailors_fitv0._2.Models
{
    public class Tailors_Feedbacks
    {
        [Key]
        public int comment_id { get; set; }
        [NotNull]
        public string username { get; set; }
        [Column(TypeName ="nvarchar(2000)")]
        public string comment { get; set; }
    }
}
