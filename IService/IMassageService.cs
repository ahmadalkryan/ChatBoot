using Application.Dtos.MessageDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface IMassageService
    {

        Task<string> GetAIResponseAsync(CrMessageDto crMessageDto);

        Task<MessageDto>CreateMessage(CrMessageDto crMessageDto);

        Task<List<MessageDto>> GetMessagesByConversationId(int conversationId);
    }
}
