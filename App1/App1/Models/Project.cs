using App1.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App1.Models
{
    public class Project
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        [Display(Name = "Название")]
        public string Name { get; set; }
        [Display(Name = "Описание")]
        public string Description { get; set; }
        [Display(Name = "Дата сдачи")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime DeadLine { get; set; }
        [Display(Name = "Заказчик")]
        [ForeignKey("Customer")]
        public int? Customer_Id { get; set; }
        public virtual Customer Customer { get; set; }
        [Display(Name = "Главный архитектор")]
        [ForeignKey("Worker")]
        public int? Worker_Id { get; set; }
        public virtual Worker Worker { get; set; }
        public virtual ICollection<Worker> Workers { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
        public Project()
        {
            Workers = new List<Worker>();
            Tasks = new List<Task>();
        }

    }
}