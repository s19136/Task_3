using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Task_3.Dto;
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
        private readonly IConfiguration _configuration;

        public EnrollmentsController(IStudentServiceDb service, IConfiguration configuration)
        {
            _service = service;
            _configuration = configuration;
        }

        [HttpPost("login", Name = "login")]
        public IActionResult Login(LoginRequest loginRequest)
        {
            if (!_service.DBLoginSuccessful(loginRequest))
            {
                return StatusCode(403);
            }
            else
            {
                var claims = new[]
                {
                new Claim(ClaimTypes.Name, loginRequest.Login),
                new Claim(ClaimTypes.Role, "admin"),
                new Claim(ClaimTypes.Role, "employee")
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken
                (
                    issuer: "Gakko",
                    audience: "Students",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: credentials
                );

                var refreshToken = Guid.NewGuid();

                if (_service.RefreshTokenInserted(loginRequest.Login, refreshToken))
                {

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        refreshToken = refreshToken
                    });
                }
                else
                {
                    return BadRequest("Couldn't insert token in DB");
                }
            }
        }

        [HttpPost("refresh-token", Name = "refresh-token")]
        public IActionResult RefreshToken(string requestToken)
        {
            var result = _service.ExtractRefreshToken(requestToken);
            if(result.login == null)
            {
                return BadRequest(result.error);
            }
            else
            {
                var claims = new[]
                {
                new Claim(ClaimTypes.Name, result.login),
                new Claim(ClaimTypes.Role, "admin"),
                new Claim(ClaimTypes.Role, "employee")
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken
                (
                    issuer: "Gakko",
                    audience: "Students",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: credentials
                );

                var refreshToken = Guid.NewGuid();

                if (_service.RefreshTokenInserted(result.login, refreshToken))
                {

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        refreshToken = refreshToken
                    });
                }
                else
                {
                    return BadRequest("Couldn't insert token in DB");
                }
            }
        }

        [HttpPost("enroll", Name = nameof(EnrollStudent))]
        [Authorize(Roles = "employee")]
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
        [Authorize(Roles = "employee")]
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