using StudentManagement.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudentManagement.Controllers
{
    public class HomeController : Controller
    {
        string str = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        
        public List<Students> GetStudents()
        {
            List<Students> students = new List<Students>();
            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = str;
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT TOP 10 StudentID, FirstName, LastName, Major, ReportsTo FROM Students ORDER BY StudentID DESC";
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if(sdr.HasRows)
                        {
                            while (sdr.Read())
                            {
                                Students stud = new Students()
                                {
                                    StudentID = Convert.ToInt32(sdr["StudentID"]),
                                    FirstName = sdr["FirstName"].ToString(),
                                    LastName = sdr["LastName"].ToString(),
                                    ReportsTo = Convert.ToInt32(sdr["ReportsTo"] == DBNull.Value ? null : sdr["ReportsTo"])
                                };

                                students.Add(stud);

                            }
                        }
                    }
                    con.Close();
                }
            }
            return students;
        }

        public void Add(Students students)
        {
            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = str;
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "INSERT INTO Students(FirstName, LastName, Major, ReportsTo) VALUES(@FirstName, @LastName, @Major, @ReportsTo)";
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@FirstName", students.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", students.LastName);
                    cmd.Parameters.AddWithValue("@Major", students.Major);
                    cmd.Parameters.AddWithValue("@ReportsTo", students.ReportsTo);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }

        public void Update(Students students)
        {
            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = str;
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "UPDATE Students SET FirstName = @FirstName, LastName = @LastName, Major = @Major WHERE StudentID = @StudentID";
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@StudentID", students.StudentID);
                    cmd.Parameters.AddWithValue("@FirstName", students.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", students.LastName);
                    cmd.Parameters.AddWithValue("@Major", students.Major);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = str;
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "DELETE FROM Students WHERE StudentID = @StudentID";
                    cmd.Parameters.AddWithValue("@StudentID", id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetAllStudents()
        {
            return Json(GetStudents().ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddStudent()
        {
            return View();
        }

        [HttpPost]
        public JsonResult AddStudent(Students stud)
        {
            try
            {
                Add(stud);
                return Json("Records added successfully.");
            }
            catch
            {
                return Json("Records not added.");
            }
        }

        public ActionResult UpdateStudent(int? id)
        {
            return View(GetStudents().Find(e => e.StudentID == id));
        }

        [HttpPost]
        public JsonResult UpdateStudent(Students stud)
        {
            Update(stud);
            return Json("Records updated successfully.", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteStudent(int id)
        {
            Delete(id);
            return Json("Records deleted successfully.", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult StudentDetails()
        {
            return PartialView("_StudentDetails");
        }
    }
}