using Application.Dtos.ChatDto;
using Application;
using Application.IService;
using AutoMapper;
using Domain;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.IRepository;

namespace Infrastructure.Service
{
    public class ChatService : IChatService
    {
        private readonly IAppRepository<Chat> _app;
          private readonly IMapper _mapper;


       public ChatService(IAppRepository<Chat> app, IMapper mapper)
        {
            _app = app;
            _mapper = mapper;
        }

        public   async Task<ChatDto> ChatDtoCreateChatc(CrChatDto crChatDto)
        {
            var chat = _mapper.Map<Chat>(crChatDto);
            await _app.AddAsync(chat);

           return _mapper.Map<ChatDto>(chat);
        }

       public async Task<IEnumerable<ChatDto>> GetAllChats(int userId)
        {
           var chats = await _app.GetAllAsync();
            var res= chats.Where(c => c.userId == userId).ToList();
            return _mapper.Map<IEnumerable<ChatDto>>(res);
        }
    }
}
