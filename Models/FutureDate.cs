using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using belt.Models;

namespace belt.Models
{
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
        if(DateTime.Parse(value.ToString())< DateTime.Now)
        {
            return new ValidationResult("Please Enter Future date. ");
        }
        return ValidationResult.Success;
      } 
    }
}