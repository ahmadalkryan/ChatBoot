using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Chat
    {
        public int Id { get; set; }
       
        public string Title { get; set; }

        public DateTime CreatedAt { get; set; }= DateTime.UtcNow;

        public User _user   { get; set; }

        public int userId { get; set; }

        public List<Message> Messages { get; set; }

    }
}
