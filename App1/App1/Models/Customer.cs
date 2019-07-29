using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace App1.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Фирма")]
        public string Name { get; set; }
        [Display(Name = "Представитель")]
        public string Representative { get; set; }
        [Display(Name = "Адрес")]
        public string Address { get; set; }
        [Display(Name = "Номер телефон")]
        [RegularExpression(@"^\+\d{2}\(\d{3}\)\d{3}-\d{2}-\d{2}$", ErrorMessage = "Некорректный номер, введите номер в формате +38(0хх)ххх-хх-хх")]
        public string Phone_Number { get; set; }
        [Display(Name = "Електронная почта")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный адрес")]
        public string Email { get; set; }

    }
}