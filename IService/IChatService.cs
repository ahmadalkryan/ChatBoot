using Application.Dtos.ChatDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface IChatService
    {
        
         Task<ChatDto> ChatDtoCreateChatc(CrChatDto crChatDto);

        Task<IEnumerable<ChatDto>> GetAllChats(int userId);
         



    }
}
