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

        [HttpPost("enroll", Name = nameof(EnrollStudent))]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            var response = _service.EnrollStudent(request);

            if (response.studentResponse == null)
            {
                switch (response.Error)
                {
                    case "No such studies":
                        return BadRequest("No such studies");
                    case "There already is student with this index":
                        return BadRequest("There already is student with this index");
                    default:
                        return StatusCode(400);
                }
            }
            else
            {
                return CreatedAtAction(nameof(EnrollStudent), response.studentResponse);
            }
        }


        [HttpPost("promote",Name = "promote")]
        public IActionResult Promote(PromoteStudentRequest request)
        {
            var response = _service.Promote(request);

            if (response.studentResponse == null)
            {
                switch (response.Error)
                {
                    case "No such record in Enrollment":
                        return NotFound("No such record in Enrollment");
                    default:
                        return StatusCode(400);
                }
            }
            else
            {
                return CreatedAtAction(nameof(EnrollStudent), response.studentResponse);
            }

        }

    }
}