using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App1.Models
{
    public class WorkersListViewModel
    {
        public IEnumerable<Worker> Workers { get; set; }
        public SelectList Specialisations { get; set; }
    }
}