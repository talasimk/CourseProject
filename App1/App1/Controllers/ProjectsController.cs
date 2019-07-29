using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using App1.Models;
using iTextSharp.text.pdf;
using RazorEngine;
using RazorEngine.Templating;
using System.Diagnostics;
using Task = App1.Models.Task;
using Microsoft.Office.Interop.Word;
using Word = Microsoft.Office.Interop.Word;

namespace App1.Controllers
{
    public class ProjectsController : Controller
    {
        private BureauContext db = new BureauContext();

        // GET: Projects
        public ViewResult Index(string sortOrder, string searchString, DateTime? date1, DateTime? date2)
        {
            
            ViewBag.DateSortParm = sortOrder == "Date" ? "Date desc" : "Date";
            var projects = db.Projects.Include(t => t.Customer)
                                       .Include(t => t.Worker);
            if (!String.IsNullOrEmpty(searchString))
            {
                projects = projects.Where(s => s.Name.ToUpper().Contains(searchString.ToUpper())
                                          || s.Customer.Name.ToUpper().Contains(searchString.ToUpper())
                                          || s.Worker.Name.ToUpper().Contains(searchString.ToUpper()));
            }
            if (date1 != null)
            {
                projects = projects.Where(s => s.DeadLine >= (DateTime)date1);
            }
            if (date2 != null)
            {
                projects = projects.Where(s => s.DeadLine <= (DateTime)date2);
            }
            switch (sortOrder)
            {
                case "Date":
                    projects = projects.OrderBy(p => p.DeadLine);
                    break;
                case "Date desc":
                    projects = projects.OrderByDescending(p => p.DeadLine);
                    break;
            }
            
            return View(projects.ToList());
        }

        // GET: Projects/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = await db.Projects.FindAsync(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            SelectList workers = new SelectList(db.Workers, "Id", "Name");
            SelectList customers = new SelectList(db.Customers, "Id", "Name");
            SelectList supposed_designer1 = new SelectList(db.Workers.Where(x => x.Specialization == "дизайнер" ).OrderBy(s => s.Tasks.Where(x => x.Status == false).Sum(x => x.Complexity)), "Id", "Name");
            
            SelectList supposed_designer2 = new SelectList(db.Workers.Where(x => x.Specialization == "дизайнер").OrderBy(s => s.Tasks.Where(x => x.Status == false).Sum(x => x.Complexity)).Skip(1), "Id", "Name");
    
            SelectList supposed_architecture1 = new SelectList(db.Workers.Where(x => x.Specialization == "архитектор").OrderBy(s => s.Tasks.Where(x => x.Status == false).Sum(x => x.Complexity)), "Id", "Name");
            SelectList supposed_architecture2 = new SelectList(db.Workers.Where(x => x.Specialization == "архитектор").OrderBy(s => s.Tasks.Where(x => x.Status == false).Sum(x => x.Complexity)).Skip(1), "Id", "Name");
            SelectList supposed_architecture3 = new SelectList(db.Workers.Where(x => x.Specialization == "архитектор").OrderBy(s => s.Tasks.Where(x => x.Status == false).Sum(x => x.Complexity)).Skip(2), "Id", "Name");
            
            SelectList supposed_draftsman = new SelectList(db.Workers.Where(x => x.Specialization == "чертежник").OrderBy(s => s.Tasks.Where(x => x.Status == false).Sum(x => x.Complexity)), "Id", "Name");
            SelectList supposed_visualizer = new SelectList(db.Workers.Where(x => x.Specialization == "визуализатор").OrderBy(s => s.Tasks.Where(x => x.Status == false).Sum(x => x.Complexity)), "Id", "Name");

            ViewBag.Workers = workers;
            ViewBag.Designer1 = supposed_designer1;
            ViewBag.Designer2 = supposed_designer2;
            ViewBag.Architecture1 = supposed_architecture1;
            ViewBag.Architecture2 = supposed_architecture2;
            ViewBag.Architecture3 = supposed_architecture3;
            ViewBag.Visualizer = supposed_visualizer;
            ViewBag.Draftsman = supposed_draftsman;
            ViewBag.Customers = customers;
            return View();
        }



        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Description,DeadLine,Customer_Id,Worker_Id")] Project project, int[] selected_workers)
        {
            if (ModelState.IsValid)
            {
                    if (selected_workers != null)
                    {
                        foreach (var p in db.Workers.Where(po => selected_workers.Contains(po.Id)))
                        {
                            project.Workers.Add(p);
                        }
                    }
                    db.Projects.Add(project);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
            }
                return View(project);
            
        }

        // GET: Projects/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = await db.Projects.FindAsync(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            SelectList workers = new SelectList(db.Workers, "Id", "Name", project.Worker_Id);
            SelectList customers = new SelectList(db.Customers, "Id", "Name", project.Customer_Id);
            ViewBag.Workers = workers;
            ViewBag.TeamWorkers = db.Workers.ToList();
            ViewBag.Customers = customers;
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Description,DeadLine,Customer_Id,Worker_Id")] Project project, int[] selectedWorkers)
        {
            if (ModelState.IsValid)
            {
                Project newProjet = db.Projects.Find(project.Id);
                newProjet.Id = project.Id;
                newProjet.Name = project.Name;
                newProjet.Description = project.Description;
                newProjet.DeadLine = project.DeadLine;
                newProjet.Customer_Id = project.Customer_Id;
                newProjet.Worker_Id = project.Worker_Id;
                newProjet.Workers.Clear();
                if (selectedWorkers != null)
                {
                    foreach (var p in db.Workers.Where(po => selectedWorkers.Contains(po.Id)))
                    {
                        newProjet.Workers.Add(p);
                    }
                }
                db.Entry(newProjet).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = await db.Projects.FindAsync(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Project project = await db.Projects.FindAsync(id);
            db.Projects.Remove(project);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public FileResult CreateDoc(int id)
        {
            Project project = db.Projects.Find(id);
            Application app = new Application();
            var document = app.Documents.Add();
            var paragraph = document.Paragraphs.Add();
            paragraph.Range.Text += "Название проекта: "  + project.Name;
            paragraph.Range.Text += "Описание проекта: " + project.Description;
            paragraph.Range.Text += "Заказчик: " +  project.Customer.Name;
            paragraph.Range.Text += "Главный архитектор: " + project.Worker.Name;
            paragraph.Range.Text += "Дата сдачи: "+ project.DeadLine.ToString() + "\n";
            paragraph.Range.Text += "Команда проекта:" ;
            foreach (Worker w in project.Workers)
            {
                paragraph.Range.Text += w.Name;
            }
            var table = document.Tables.Add(document.Paragraphs[document.Paragraphs.Count].Range, project.Tasks.Count() + 1, 8);
            Word.Table tbl = document.Tables[1];
            tbl.Range.Font.Size = 12;

            tbl.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
            tbl.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
            tbl.Columns.DistributeWidth();
            int index = 1;
            tbl.Cell(index, 1).Range.Text = "Название";
            tbl.Cell(index, 2).Range.Text = "Описание";
            tbl.Cell(index, 3).Range.Text = "Сложность";
            tbl.Cell(index, 4).Range.Text = "Работник";
            tbl.Cell(index, 5).Range.Text = "Проект";
            tbl.Cell(index, 6).Range.Text = "Начало работы";
            tbl.Cell(index, 7).Range.Text = "Дата сдачи";
            tbl.Cell(index, 8).Range.Text = "Статус";
            ++index;
            foreach (Task t in project.Tasks)
            {
                tbl.Cell(index, 1).Range.Text = t.Name;
                tbl.Cell(index, 2).Range.Text = t.Description;
                tbl.Cell(index, 3).Range.Text = t.Complexity.ToString();
                tbl.Cell(index, 4).Range.Text = t.Worker.Name;
                tbl.Cell(index, 5).Range.Text = t.Project.Name;
                tbl.Cell(index, 6).Range.Text = t.Begin_Data.ToString();
                tbl.Cell(index, 7).Range.Text = t.Deadline_Data.ToString();
                if(t.Status)
                {
                    tbl.Cell(index, 8).Range.Text = "+";
                }
                else
                {
                    tbl.Cell(index, 8).Range.Text = "-";
                }
                ++index;
            }
            app.ActiveDocument.SaveAs("D:\\document2.doc", WdSaveFormat.wdFormatDocument);
            document.Close();
            return File("D:\\document2.doc", "application/doc");

        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
