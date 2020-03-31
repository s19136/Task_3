using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Task_3.DAL;
using Task_3.Models;
using System.Data.SqlClient;

namespace Task_3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IDbService _dbService;

        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet]
        public IActionResult GetStudent()
        {
            List<Student> res = new List<Student>();
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s19136;Integrated Security=True"))
            using(var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select FirstName,LastName,BirthDate,Name,Semester " +
                    "from Studies, Enrollment, Student " +
                    "where Studies.IdStudy = Enrollment.IdStudy and Enrollment.IdEnrollment = Student.IdEnrollment";

                con.Open();
                var dr = com.ExecuteReader();
                while(dr.Read())
                {
                    var st = new Student { FirstName = dr["FirstName"].ToString(), LastName = dr["LastName"].ToString(), 
                    BirthDate = DateTime.Parse(dr["BirthDate"].ToString()), StudyName = dr["Name"].ToString(),
                    Semester = dr["Semester"].ToString()};
                    res.Add(st);
                }
            }
            return Ok(res);
        }

        [HttpGet]
        [Route("get2")]
        public IActionResult GetSemester(string id)
        {
            List<string> res = new List<string>();
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s19136;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select Semester from Student, Enrollment" +
                    " where Enrollment.IdEnrollment = Student.IdEnrollment and IndexNumber = @id";
                com.Parameters.AddWithValue("id", id);

                con.Open();
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    res.Add(dr["Semester"].ToString());
                }
            }
            return Ok(res);
        }


        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            //student.IndexNumber = $"s{new Random().Next(1, 2000)}";
            return Ok(student);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteMethod(int id)
        {
            return Ok("Delete completed");
        }

        [HttpPut("{id}")]
        public IActionResult PutMethod(int id)
        {
            return Ok("Put completed");
        }
    }
}