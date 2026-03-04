using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class User
    {
        public User()
        {
            Chats = new HashSet<Chat>();
        }
        public int Id { get; set; }
      public  string Name { get; set; }
       public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } //admin, user;

        public ICollection<Chat> Chats { get; set; }
    }
}
