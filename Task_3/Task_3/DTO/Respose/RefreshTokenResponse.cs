using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task_3.Dto;

namespace Task_3.DTO.Respose
{
    public class RefreshTokenResponse
    {
        public string login { get; set; }
        public string error { get; set; }
    }
}
