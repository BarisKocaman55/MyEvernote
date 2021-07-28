using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.Entities
{
    [Table("EvernoteUsers")]
    public class EvernoteUser : MyEntityBase
    {
        [DisplayName("Name"), StringLength(50, ErrorMessage = "{0} field can be {1} charcter maximum")]
        public string Name { get; set; }

        [DisplayName("Surname"), StringLength(50)]
        public string Surname { get; set; }

        [DisplayName("Username"), Required(ErrorMessage = "{0} field is required"), StringLength(50, ErrorMessage = "{0} field can be {1} charcter maximum")]
        public string Username { get; set; }

        [DisplayName("Email"), Required(ErrorMessage = "{0} field is required"), StringLength(50, ErrorMessage = "{0} field can be {1} charcter maximum")]
        public string Email { get; set; }

        [DisplayName("Password"), Required(ErrorMessage = "{0} field is required"), StringLength(50, ErrorMessage = "{0} field can be {1} charcter maximum")]
        public string Password { get; set; }

        [StringLength(30), ScaffoldColumn(false), DisplayName("Profile Image")]
        public string ProfileImageFilename { get; set; }
        
        [DisplayName("Is Active")]
        public bool IsActive { get; set; }

        [Required, ScaffoldColumn(false)]
        public Guid ActivateGuid { get; set; }

        [Required, DisplayName("Is Admin")]
        public bool IsAdmin { get; set; }

        public virtual List<Note> Notes { get; set; }
        public virtual List<Comment> Comments { get; set; }
        public virtual List<Liked> Likes { get; set; }
    }
}
