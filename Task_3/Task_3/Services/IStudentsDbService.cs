using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task_3.Dto;
using Task_3.DTO.Request;
using Task_3.DTO.Respose;

namespace Task_3.Services
{
    public interface IStudentServiceDb
    {
        StudentServiceResponse EnrollStudent(EnrollStudentRequest req);
        StudentServiceResponse Promote(PromoteStudentRequest req);
        Boolean IndexExists(string index);
        Boolean DBLoginSuccessful(LoginRequest loginRequest);
        Boolean RefreshTokenInserted(string login, Guid token);
        RefreshTokenResponse ExtractRefreshToken(string token);
        string GetRole(string index);
    }
}
