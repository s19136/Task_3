using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task_3.DTO.Request;
using Task_3.DTO.Respose;
using Task_3.Services;

namespace Task_3.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private IStudentServiceDb _service;

        public EnrollmentsController(IStudentServiceDb service)
        {
            _service = service;
        }

        [HttpPost(Name = nameof(EnrollStudent))]
        [Route("enroll")]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            var response = _service.EnrollStudent(request);

            switch (response)
            {
                case "No such studies":
                    return BadRequest("No such studies");
                case "There already is student with this index":
                    return BadRequest("There already is student with this index");
                default:
                    string[] obj = response.Split(" ");
                    var res = new StudentResponse
                    {
                        IdEnrollment = obj[0],
                        IdStudy = obj[1],
                        Semester = obj[2],
                        StartDate = obj[3]
                    };
                    return CreatedAtAction(nameof(EnrollStudent), res);
            }
        }


        [HttpPost(Name = "promote")]
        [Route("promote")]
        public IActionResult Promote(PromoteStudentRequest request)
        {
            var response = _service.Promote(request);

            switch (response)
            {
                case "No such record in Enrollment":
                    return NotFound("No such record in Enrollment");
                default:
                    string[] obj = response.Split(" ");
                    var res = new StudentResponse
                    {
                        IdEnrollment = obj[0],
                        IdStudy = obj[1],
                        Semester = obj[2],
                        StartDate = obj[3]
                    };
                    return CreatedAtAction("promote",res);
            }

        }

    }
}