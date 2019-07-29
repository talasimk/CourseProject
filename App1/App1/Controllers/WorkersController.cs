using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using App1.Models;
using PagedList;
using Microsoft.Office.Interop.Word;
using Task = App1.Models.Task;

namespace App1.Controllers
{
    public class WorkersController : Controller
    {
        private BureauContext db = new BureauContext();
        
        public ActionResult Index(string specialization, string sortOrder, string searchString, bool? check)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name desc" : "";
            IQueryable<Worker> workers = db.Workers;
            if (!String.IsNullOrEmpty(specialization) && !specialization.Equals("Все"))
            {
                if(check == true)
                {
                    workers = workers.Where(p => p.Specialization != specialization);
                }
                else
                    workers = workers.Where(p => p.Specialization == specialization);
            }
            if (!String.IsNullOrEmpty(searchString))
            {
                workers = workers.Where(s => s.Name.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "Name desc":
                    workers = workers.OrderBy(s => s.Name);
                    break;
            }
            SelectList specializations = new SelectList(new List<string>()
            {
                "Все",
                "дизайнер",
                "чертежник",
                "архитектор",
                "визуализатор"
            });
            ViewBag.Specializations = specializations;
            return View(workers.ToList());
        }


        // GET: Workers/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Worker worker = await db.Workers.FindAsync(id);
            if (worker == null)
            {
                return HttpNotFound();
            }

            return View(worker);
        }

        // GET: Workers/Create
        public ActionResult Create()
        {
            SelectList specializations = new SelectList(new List<string>()
            {
                "дизайнер",
                "чертежник",
                "архитектор",
                "визуализатор"
            });
            ViewBag.Specializations = specializations;
            return View();
        }

        // POST: Workers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Passport,PhoneNumber,Email,Specialization")] Worker worker)
        {
            if (ModelState.IsValid)
            {
                db.Workers.Add(worker);

                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(worker);
        }

        // GET: Workers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Worker worker = await db.Workers.FindAsync(id);
            if (worker == null)
            {
                return HttpNotFound();
            }
            SelectList specializations = new SelectList(new List<string>()
            {
                "дизайнер",
                "чертежник",
                "архитектор",
                "визуализатор"
            });
            ViewBag.Specializations = specializations;
            ViewBag.Projects = db.Projects.ToList();
            return View(worker);
        }

        // POST: Workers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Passport,PhoneNumber,Email,Specialization")] Worker worker, int[] selectedProjects)
        {
            if (ModelState.IsValid)
            {
                Worker newWorker = db.Workers.Find(worker.Id);
                newWorker.Name = worker.Name;
                newWorker.Passport = worker.Passport;
                newWorker.PhoneNumber = worker.PhoneNumber;
                newWorker.Email = worker.Email;
                newWorker.PhoneNumber = worker.PhoneNumber;
                newWorker.Specialization = worker.Specialization;
                newWorker.Projects.Clear();
                if (selectedProjects != null)
                {
                    foreach (var p in db.Projects.Where(po => selectedProjects.Contains(po.Id)))
                    {
                        newWorker.Projects.Add(p);
                    }
                }
                db.Entry(newWorker).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(worker);
        }

        // GET: Workers/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Worker worker = await db.Workers.FindAsync(id);
            if (worker == null)
            {
                return HttpNotFound();
            }
            return View(worker);
        }

        // POST: Workers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Worker worker = await db.Workers.FindAsync(id);
            db.Workers.Remove(worker);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public FileResult CreateDoc(int id)
        {
            Worker worker = db.Workers.Find(id);
            int par = 1 ;
            Application app = new Application();
            var document = app.Documents.Add();
            var paragraph = document.Paragraphs.Add();
            ++par;
            paragraph.Range.Text += "ФИО: " + worker.Name;
            paragraph.Range.Text += "Контактный телефон: " + worker.PhoneNumber;
            paragraph.Range.Text += "Паспорт: " + worker.Passport;
            paragraph.Range.Text += "Электронная почта: " + worker.Email;
            paragraph.Range.Text += "Специализация: " + worker.Specialization;
            paragraph.Range.Text += "Задания: " ;
            var a = document.Paragraphs.Add();
            if (worker.Tasks.Count > 0)
            {
                var table = document.Tables.Add(document.Paragraphs[document.Paragraphs.Count].Range, worker.Tasks.Count() + 1, 7);

                Microsoft.Office.Interop.Word.Table tbl = document.Tables[document.Tables.Count];
                tbl.Range.Font.Size = 12;
                tbl.Columns.DistributeWidth();
                tbl.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                tbl.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                tbl.Cell(1, 1).Range.Text = "Название";
                tbl.Cell(1, 2).Range.Text = "Описание";
                tbl.Cell(1, 3).Range.Text = "Сложность";
                tbl.Cell(1, 4).Range.Text = "Проект";
                tbl.Cell(1, 5).Range.Text = "Начало";
                tbl.Cell(1, 6).Range.Text = "Дата сдачи";
                tbl.Cell(1, 7).Range.Text = "Статус";
                foreach (Task t in worker.Tasks)
                {

                    int index = 2;
                    tbl.Cell(index, 1).Range.Text = t.Name;
                    tbl.Cell(index, 2).Range.Text = t.Description;
                    tbl.Cell(index, 3).Range.Text = t.Complexity.ToString();
                    tbl.Cell(index, 4).Range.Text = t.Project.Name;
                    tbl.Cell(index, 5).Range.Text = t.Begin_Data.ToString();
                    tbl.Cell(index, 6).Range.Text = t.Deadline_Data.ToString();
                    if (t.Status)
                    {
                        tbl.Cell(index, 7).Range.Text = "+";
                    }
                    else
                    {
                        tbl.Cell(index, 7).Range.Text = "-";
                    }
                    ++index;

                }
            }
            else
            {
                paragraph.Range.Text += "Сотрудник не выполнил/выполняет ни одного задания";
            }
            app.ActiveDocument.SaveAs("D:\\document3.doc", WdSaveFormat.wdFormatDocument);
            document.Close();
            return File("D:\\document3.doc", "application/doc");

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
