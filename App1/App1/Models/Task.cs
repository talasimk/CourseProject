using App1.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace App1.Models
{
    public class Task
    {
        [Key]
        public int Id { get; set; }
        
        [Display(Name = "Название")]
        public string Name { get; set; }
        [Display(Name = "Описание")]
        public string Description { get; set; }
        [ForeignKey("Worker")]
        [Display(Name = "Выполняющий сотрудник")]
        public int ?Worker_Id { get; set; }
        [ForeignKey("Project")]
        [Display(Name = "Проект")]
        public int ?Project_Id { get; set; }
        [Display(Name = "Сложность")]
        [Range(0, 10)]
        public int Complexity { get; set; }
        [Display(Name = "Статус")]
        [UIHint("Boolean")]
        public bool Status { get; set; }
        [Display(Name = "Начало выполнения")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Begin_Data { get; set; }
        [Display(Name = "Дата сдачи")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Deadline_Data { get; set; }
        [Display(Name = "Выполняющий сотрудник")]
        public virtual Worker Worker { get; set; }
        [Display(Name = "Проект")]
        public virtual Project Project { get; set; }

    }
}