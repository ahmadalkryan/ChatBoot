using Application.Dtos.MessageDto;
using Application.IRepository;
using Application.IService;
using AutoMapper;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Service
{
    public class MessageService : IMassageService
    {
        private readonly IMapper _mapper;
        private readonly IAIService _aiService;

        private readonly IAppRepository<Message> _appRepository;

        public MessageService(IAppRepository<Message> appRepository 
            ,IMapper mapper ,IAIService aIService)
        {
            _appRepository = appRepository;
            _mapper = mapper;
            _aiService = aIService; 

        }
        public async Task<MessageDto> CreateMessage(CrMessageDto crMessageDto)
        {
            var message = _mapper.Map<Message>(crMessageDto);
            var addedMessage =  _appRepository.AddAsync(message);
            return _mapper.Map<MessageDto>(addedMessage);
        }

        public  async Task<string> GetAIResponseAsync(CrMessageDto crMessageDto)
        {
            var response =await  _aiService.GetAIResponseAsync( crMessageDto.Content);
            if(response != null)
            {
                var CrMessage  = new CrMessageDto
                {
                    chatId = crMessageDto.chatId,
                   Content = crMessageDto.Content,
                };
                 CreateMessage(CrMessage);
            }
            return response;
        }

        public async Task<List<MessageDto>> GetMessagesByConversationId(int conversationId)
        {
            var res = await _appRepository.GetAllAsync();
             res =res.Where(m => m.chatId == conversationId);
            return _mapper.Map<List<MessageDto>>(res);
        }

       
    }
}
