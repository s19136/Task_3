using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task_3.DAL;
using Task_3.Models;

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
        public IActionResult GetStudent(string orderBy)
        {
            return Ok(_dbService.GetStudents());
        }

        /*[HttpGet]
        public string GetStudents(string orderBy)
        {
            return $"Kowalski, Malewicz, sorting={orderBy}";

        }*/

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            student.IndexNumber = $"s{new Random().Next(1, 2000)}";
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