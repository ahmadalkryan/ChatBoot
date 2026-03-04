using Application.Dtos.MessageDto;
using Application.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArabicChatBot.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly ILogger<MessageController> _logger;
        private readonly IMassageService _massageService;
        public MessageController(ILogger<MessageController> logger, IMassageService massageService)
        {
            _logger = logger;
            _massageService = massageService;
        }

        //return string response from openai api 
        [HttpPost]

        public async Task<IActionResult> CreateMessage([FromBody]CrMessageDto crMessageDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdMessage = await _massageService.GetAIResponseAsync(crMessageDto);
                return Ok(new
                {
                    Result = true,
                    Message = "Success",
                    Data = createdMessage
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating message");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }


        // return list of messageDto by conversationId
        [HttpGet]

        public async Task<IActionResult> GetMessagesByConversationId(int chatId)
        {
            try
            {
                var messages = await _massageService.GetMessagesByConversationId(chatId);
                return Ok(new
                {
                    Result = true,
                    Message = "Success",
                    Data = messages
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving messages");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }

        }
    }
}
