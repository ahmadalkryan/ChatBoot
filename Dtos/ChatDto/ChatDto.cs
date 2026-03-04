using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.ChatDto
{
    public class ChatDto
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public DateTime CreatedAt { get; set; }


        public int userId { get; set; }
    }
}
