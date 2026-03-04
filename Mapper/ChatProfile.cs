using Application.Dtos.ChatDto;
using AutoMapper;
using Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mapper
{
    public class ChatProfile : Profile
    {
        public ChatProfile()
        {
            // Chat -> ChatDto
            CreateMap<Chat, ChatDto>();
            // .ForMember(dest => dest.userId, opt => opt.MapFrom(src => src.userId)); // تحديد التعيين يدوياً

            CreateMap<CrChatDto, Chat>();
            //.ForMember(dest => dest.userId, opt => opt.MapFrom(src => src.userId))
              //.ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // سيتم تعيينها افتراضياً
              //.ForMember(dest => dest._user, opt => opt.Ignore()) // تجاهل _user
              //.ForMember(dest => dest.Messages, opt => opt.Ignore()); // تجاهل Messages
        }
    }
    
}

    
