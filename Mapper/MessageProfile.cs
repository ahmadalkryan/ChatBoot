using Application.Dtos.MessageDto;
using AutoMapper;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mapper
{
    public class MessageProfile:Profile
    {
        public MessageProfile()
        {
            CreateMap<Message, MessageDto>();
            CreateMap<CrMessageDto, Message>();
                //.ForMember(dest=>dest.SentAt,opt=>opt.MapFrom(src=>DateTime.Now));

        }
    }
}
