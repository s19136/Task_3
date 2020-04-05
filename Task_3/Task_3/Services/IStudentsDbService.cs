using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task_3.DTO.Request;

namespace Task_3.Services
{
    public interface IStudentServiceDb
    {
        string EnrollStudent(EnrollStudentRequest req);
        string Promote(PromoteStudentRequest req);
    }
}
