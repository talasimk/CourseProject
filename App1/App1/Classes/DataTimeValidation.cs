using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace App1.Classes
{
    public class DataTimeValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
          
            if (DateTime.Compare(DateTime.Now, (DateTime)value) < 0)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Некорректная дата");
            }
        }
    }
}