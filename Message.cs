using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public  class Message
    {
        public int Id { get; set; }
        public string Content { get; set; }

        public DateTime SentAt { get; set; }

        public Chat _chat { get; set; }

        public int chatId { get; set; }

    }
}
