using Application.Dtos.UserDto;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface IUserService
    {
        Task<UserDto> RegisterAsync(Register registerDto);
        //  Task<UserDto> UpdateUserRoleAsync(UpdateUserRoleDto updateUserRoleDto);
        Task<LoginResponse> LoginAsync(Login loginDto);
        Task<UserDto> GetCurrentUserAsync();
        Task<User> GetUser(int userID);
        Task<UserDto> GetUserByIdAsync(int id);

        //  Task<LogoutDto> Logout();
        Task<IEnumerable<UserDto>> GetAllUser();
    }

}
