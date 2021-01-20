using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterApp.EFClasses
{
    public class TwitterPost
    {
        public TwitterPost()
        {

        }

        [Required, Key]
        public int Id { get; set; }

        [Required, Index]
        public long TwitterId { get; set; }

        [Required] //, ForeignKey("Author")
        public int AuthorId { get; set; }

        public virtual TwitterUser Author { get; set; }

        [Required]
        [MaxLength(250)]
        public string Text { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        public int RetweetCount { get; set; }


    }
}
