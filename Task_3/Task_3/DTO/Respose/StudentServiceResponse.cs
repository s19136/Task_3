using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Task_3.DTO.Respose
{
    public class StudentServiceResponse
    {
        public EnrollmentResponse studentResponse { get; set; }
        public string Error { get; set; }
    }
}
