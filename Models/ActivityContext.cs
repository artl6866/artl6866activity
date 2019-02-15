using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using belt.Models;
using Microsoft.EntityFrameworkCore;


namespace belt.Models
{
    public class ActivityContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public ActivityContext(DbContextOptions<ActivityContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Activity> Activities { get; set; }  
        public DbSet<Participant> Participants {get;set;}  
        }
}