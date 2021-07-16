using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace BigSchool.Models
{
    public class CoursesController : Controller
    {
        // GET: Courses
        public ActionResult Create()
        {
            //get list category
            BigSchoolContext context = new BigSchoolContext();
            Course objCourse = new Course();
            objCourse.ListCategory = context.Category.ToList();
            return View(objCourse);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Course objCourse)
        {
            BigSchoolContext context = new BigSchoolContext();
            //không xét valid LecturerId vì bằng user đăng nhập
            ModelState.Remove("LecturerId");
            if(!ModelState.IsValid)
            {
                objCourse.ListCategory = context.Category.ToList();
                return View("Create", objCourse);
            }    

            //lay login user ID
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            objCourse.LecturerId = user.Id;
            //add vào csdl
            context.Course.Add(objCourse);
            context.SaveChanges();
            //tro ve home, action index
            return RedirectToAction("Index", "Home");
        }
        
        public ActionResult Attending()
        {
            BigSchoolContext context = new BigSchoolContext();
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var listAttendances = context.Attendance.Where(p => p.Attendee == currentUser.Id).ToList();
            var courses = new List<Course>();
            foreach (Attendance temp in listAttendances)
            {
                Course objCourse = temp.Course;
                objCourse.LectureName = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(objCourse.LecturerId).Name;
                courses.Add(objCourse);
            }
            return View(courses);
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            BigSchoolContext context = new BigSchoolContext();
            Course c = context.Course.SingleOrDefault(p => p.Id == id);
            return View(c);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Course c)
        {
            BigSchoolContext context = new BigSchoolContext();
            Course dbUpdate = context.Course.SingleOrDefault(p => p.Id == c.Id);
            if (dbUpdate != null)
            {
                context.Course.AddOrUpdate(c);
                context.SaveChanges();
            }
            return RedirectToAction("Mine");
        }

        [HttpGet]
        public ActionResult Detele(int id)
        {
            BigSchoolContext context = new BigSchoolContext();
            Course c = context.Course.SingleOrDefault(p => p.Id == id);
            return View(c);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Detele(Course c)
        {
            BigSchoolContext context = new BigSchoolContext();
            Course dbDetele = context.Course.SingleOrDefault(p => p.Id == c.Id);
            if (dbDetele != null)
            {
                context.Course.Remove(dbDetele);
                context.SaveChanges();
            }
            return RedirectToAction("Mine");
        }

        public ActionResult Mine()
        {
            BigSchoolContext context = new BigSchoolContext();
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var courses = context.Course.Where(c => c.LecturerId == currentUser.Id && c.DateTime > DateTime.Now).ToList();
            foreach (Course i in courses)
            {
                i.LectureName = currentUser.Name;
            }
            return View(courses);
        }

        public ActionResult LectureIamGoing()
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            BigSchoolContext context = new BigSchoolContext();
            var listFollwee = context.Followings.Where(p => p.FollowerId == currentUser.Id).ToList();
            var listAttendances = context.Attendance.Where(p => p.Attendee == currentUser.Id).ToList();
            var courses = new List<Course>();
            foreach (var course in listAttendances)
            {
                foreach (var item in listFollwee)
                {
                    if (item.FolloweeId == course.Course.LecturerId)
                    {
                        Course objCourse = course.Course;
                        objCourse.LectureName = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(objCourse.LecturerId).Name;
                        courses.Add(objCourse);
                    }
                }
            }
            return View(courses);

        }
    }
}