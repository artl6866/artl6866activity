using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using belt.Models;

namespace belt.Models
{
    public class User
    {
        [Key]
        public int UserId {get;set;}
        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Must be letters only")]
        [MinLength(2, ErrorMessage = "Must be at least 2 characters")]
        public string FirstName {get;set;}
        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Must be letters only")]
        [MinLength(2, ErrorMessage = "Must be at least 2 characters")]
        public string LastName {get;set;}
        [Required]
        [EmailAddress]
        public string Email {get;set;}
        [Required]
        [MinLength(8, ErrorMessage="Password must be 8 characters or longer!")]
        [DataType(DataType.Password)]
        public string Password {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
        public List <Activity> Activities {get;set;}
        public List <Participant> Participants {get;set;}
        // Will not be mapped to your users table!
        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string Confirm {get;set;}
        
    }
    public class LoginUser
    {
        [Required]
        [EmailAddress]
        public string LoginEmail {get;set;}
        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage="Password must be 8 characters or longer!")]
        public string LoginPassword {get;set;}
    }
}
