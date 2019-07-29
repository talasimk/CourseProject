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
using Task = App1.Models.Task;

namespace App1.Controllers
{
    public class TasksController : Controller
    {
        private BureauContext db = new BureauContext();

        // GET: Tasks
        public ActionResult Index(string sortOrder, bool? status, string searchString)
        {
            
            ViewBag.DateSortParm = sortOrder == "Date" ? "Date desc" : "Date";
            ViewBag.ComplexitySortParm = sortOrder == "Complexity" ? "Complexity desc" : "Complexity";
            var tasks = db.Tasks.Include(t => t.Worker)
                                .Include(t => t.Project);
            if (!String.IsNullOrEmpty(searchString))
            {
                tasks = tasks.Where(s => s.Name.ToUpper().Contains(searchString.ToUpper())
                                          || s.Project.Name.ToUpper().Contains(searchString.ToUpper())
                                          || s.Worker.Name.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "Date":
                    tasks = tasks.OrderBy(p => p.Deadline_Data);
                    break;
                case "Date desc":
                    tasks = tasks.OrderByDescending(p => p.Deadline_Data);
                    break;
                case "Complexity":
                    tasks = tasks.OrderBy(p => p.Complexity);
                    break;
                case "Complexity desc":
                    tasks = tasks.OrderByDescending(p => p.Complexity);
                    break;
            }
            if (status != null)
            {
                tasks = tasks.Where(p => p.Status == status);
            }
            return View( tasks.ToList());
        }

        // GET: Tasks/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = await db.Tasks.FindAsync(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            return View(task);
        }

        // GET: Tasks/Create
        public ActionResult Create()
        {
            SelectList projects = new SelectList(db.Projects, "Id", "Name");
            SelectList workers = new SelectList(db.Workers.OrderBy(s => s.Tasks.Where(x => x.Status == false).Sum(x => x.Complexity)), "Id", "Name");
            
            ViewBag.Workers = workers;
            ViewBag.Projects = projects;
            return View();
        }

        public JsonResult GetItems(int id)
        {
            Project project =  db.Projects.Find(id);
            return Json(new SelectList(project.Workers , "Id", "Name"));
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Description,Complexity,Worker_Id,Project_Id, Status,Begin_Data,Deadline_Data")] Task task)
        {
            if (ModelState.IsValid)
            {
                db.Tasks.Add(task);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(task);
        }

        // GET: Tasks/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = await db.Tasks.FindAsync(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            SelectList workers = new SelectList(db.Workers, "Id", "Name", task.Worker_Id);
            SelectList projects = new SelectList(db.Projects, "Id", "Name", task.Project_Id);
            ViewBag.Workers = workers;
            ViewBag.Projects = projects;
            return View(task);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Worker_Id,Project_Id,Name,Description,Complexity,Status,Begin_Data,Deadline_Data")] Task task)
        {
            if (ModelState.IsValid)
            {
                db.Entry(task).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(task);
        }

        // GET: Tasks/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = await db.Tasks.FindAsync(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Task task = await db.Tasks.FindAsync(id);
            db.Tasks.Remove(task);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
