using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.UserDto
{
    public class LoginResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }

        public DateTime Expires { get; set; }

        public string Role { get; set; }
    }
}
