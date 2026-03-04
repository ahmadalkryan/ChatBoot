using Application.Dtos.ChatDto;
using Application.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ArabicChatBot.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }


        
        [HttpPost]
        public async Task<IActionResult> CreateChat( CrChatDto crChatDto)
        {
            var result =  _chatService.ChatDtoCreateChatc(crChatDto);

            return Ok(new
            {
                Result = true,
                Message = "Success",
                Data = result
            });
        }

        [HttpGet]
        // return list of chatDto
        public async Task<IActionResult> GetChats(int userId)
        {
            var chats = await _chatService.GetAllChats(userId);
            return Ok(new
            {
                Result = true,
                Message = "Success",
                Data = chats
            });
        }
    }
}
