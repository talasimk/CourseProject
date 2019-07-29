using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace App1.Models
{
    public class Worker
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display (Name="Имя сотрудника")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Паспорт")]
        public string Passport { get; set; }
        [Required]
        [RegularExpression(@"^\+\d{2}\(\d{3}\)\d{3}-\d{2}-\d{2}$", ErrorMessage = "Некорректный номер, введите номер в формате +38(0хх)ххх-хх-хх")]
        [Display(Name = "Контактный телефон")]
        public string PhoneNumber { get; set; }
        [Required]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный адрес")]
        [Display(Name = "Електронная почта")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Специализация")]
        public string Specialization { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
        public Worker()
        {
            Projects = new List<Project>();
            Tasks = new List<Task>();
        }

    }
}