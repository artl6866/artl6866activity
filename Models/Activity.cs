using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using belt.Models;

namespace belt.Models
{
    public class Activity
    {
        [Key]
        public int ActivityId {get;set;}
        public int UserId {get;set;}
        [Required]
        [MinLength(2, ErrorMessage = "Must be at least 2 Characters")]
        public string ActivityName {get;set;}
        [Required]
        [MinLength(2, ErrorMessage = "Must be at least 2 Characters")]
        public string Description {get;set;}
        public User Users {get;set;}
        [FutureDate]
        [Required]
        [DataType(DataType.Date)]
        public DateTime When {get;set;}
        
        [Required]
        [DataType(DataType.Time)]
        public DateTime Duration {get;set;} 
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
        public List <Participant> Participants {get;set;}
    }
    public class Participant
    {
        [Key]
        public int ParticipantId {get;set;}
        public int UserId {get;set;}
        public int ActivityId {get;set;}
        
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
        public User Users {get;set;}
        public Activity Activities {get;set;}
    }
}