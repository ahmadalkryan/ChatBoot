using Application.Dtos.UserDto;
using Application.IRepository;
using Application.IService;
using AutoMapper;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Service
{
    public class UserService : IUserService
    {
        private readonly IAppRepository<User> _appRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;
        private readonly IPasswordHash _passwordHash1;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;

        public UserService(IJwtService jwtService,IPasswordHash passwordHash1,IConfiguration configuration , IAppRepository<User> appRepostory ,IMapper mapper ,IPasswordHash passwordHash ,IHttpContextAccessor httpContextAccessor)
        {
            _appRepository = appRepostory;
            _contextAccessor = httpContextAccessor;
            _passwordHash1 = passwordHash1;
            _configuration = configuration;
            _jwtService =jwtService;
            _mapper = mapper;

        }
        public async Task<IEnumerable<UserDto>> GetAllUser()
        {
            return _mapper.Map<IEnumerable<UserDto>>(await _appRepository.GetAllAsync());
        }

        public async Task<UserDto> GetCurrentUserAsync()
        { 
            var userId = GetCurrentUserId();
            var user = await _appRepository.GetByIdAsync(userId);
           return _mapper.Map<UserDto>(user);
        }
        private int GetCurrentUserId()
        {
            var userIdClaim = _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            throw new Exception("User ID claim not found or invalid.");
        }
        public async Task<User> GetUser(int userID)
        {
           return await _appRepository.GetByIdAsync(userID);
        }

        public Task<UserDto> GetUserByIdAsync(int id)
        {
            return _mapper.Map<Task<UserDto>>(_appRepository.GetByIdAsync(id));
        }

        public async Task<LoginResponse> LoginAsync(Login loginDto)
        {
            var users = await _appRepository.GetAllAsync();
            var user = users.FirstOrDefault(x => x.Email == loginDto.Email);

            var valid = user !=null && _passwordHash1.VerifyPassword(loginDto.Password, user.Password);
            if (!valid)
            {
                throw new Exception("Invalid email or password");
            }
           var token = _jwtService.GenerateToken(user);
            return new LoginResponse
            {
                Token = token,
                Id = user.Id,
                Role = user.Role,
                Name = user.Name,
                Expires = DateTime.Now.AddMinutes(60.0)



            };


        }

        public async Task<UserDto> RegisterAsync(Register registerDto)
        {

            var check = await _appRepository.GetAllAsync();

            if (check.Any(x=>x.Email.Equals(registerDto.Email)))
            {
                throw new Exception($"Email '{registerDto.Email}' is already registered");

            }
           var user = new User
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                Password = _passwordHash1.HashPassword(registerDto.Password),
                Role = "User"
            };
            await _appRepository.AddAsync(user);
          
            return _mapper.Map<UserDto>(user);
        }
        private void ValidatePassword(string password)
        {
            if (password.Length < 6)
            {
                throw new ArgumentException("Password must be at least 6 char ");
            }
        }
    }
}



















//        private readonly  IAppRepository<User> _appRepository;
//        private readonly IMapper _mapper;
//        private readonly string _jwt;
//        private readonly ILogger<UserService> _logger;
//        private readonly IConfiguration _configuration;
//        private readonly string _jwtSecret;
//        private readonly string _jwtIssuer;
//        private readonly string _jwtAudience;





//        public UserService(
//            IAppRepository<User> appRepository,
//            IMapper mapper,
//            ILogger<UserService> logger,
//            IConfiguration configuration)
//        {
//            _appRepository = appRepository;
//            _mapper = mapper;
//            _logger = logger;
//            _configuration = configuration;

//            // إعدادات JWT من appsettings.json
//            _jwtSecret = _configuration["JwtSettings:Secret"]
//                ?? "YourSuperSecretKeyHereThatIsAtLeast32CharactersLong!";
//            _jwtIssuer = _configuration["JwtSettings:Issuer"]
//                ?? "ArabicChatBot";
//            _jwtAudience = _configuration["JwtSettings:Audience"]
//                ?? "ArabicChatBotUsers";
//        }


//        public async Task<AuthResponse> LoginAsync(LoginRequest loginRequest)
//        {
//            try
//            {
//                _logger.LogInformation($"Login attempt for email: {loginRequest.Email}");

//                // التحقق من صحة الإدخال
//                if (string.IsNullOrWhiteSpace(loginRequest.Email) ||
//                    string.IsNullOrWhiteSpace(loginRequest.Password))
//                {
//                    return CreateErrorResponse("Email and password are required");
//                }

//                // البحث عن المستخدم
//                var users = await _appRepository.GetAllAsync();
//                var user = users.FirstOrDefault(x => x.Email == loginRequest.Email);

//                if (user == null)
//                {
//                    _logger.LogWarning($"Login failed: User not found - {loginRequest.Email}");
//                    return CreateErrorResponse("Invalid email or password");
//                }

//                // التحقق من حالة الحساب
//                if (!user.IsActive)
//                {
//                    return CreateErrorResponse("Account is disabled. Please contact support.");
//                }

//                // التحقق من كلمة المرور (تشفير BCrypt)
//                if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash))
//                {
//                    _logger.LogWarning($"Login failed: Invalid password for user {user.Email}");
//                    return CreateErrorResponse("Invalid email or password");
//                }

//                // تحديث وقت آخر تسجيل دخول
//                user.LastLoginAt = DateTime.UtcNow;
//                await _appRepository.UpdateAsync(user);
//                await _appRepository.SaveChangesAsync();

//                // إنشاء JWT Token
//                var token = GenerateJwtToken(user);
//                var userDto = _mapper.Map<UserDto>(user);

//                _logger.LogInformation($"User logged in successfully: {user.Email}");

//                return new AuthResponse
//                {
//                    Success = true,
//                    Message = "Login successful",
//                    Token = token,
//                    TokenExpiration = DateTime.UtcNow.AddHours(24),
//                    User = userDto
//                };
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, $"Error during login for {loginRequest.Email}");
//                return CreateErrorResponse("An error occurred during login");
//            }
//        }




//        //public async Task<string> Login(Login login)
//        //{
//        //    var user = await _appRepository.GetAllAsync();
//        //    var us = user.Where(x=>x.Email == login.Email && x.Password == login.Password).FirstOrDefault();

//        //    if(us == null)
//        //    {
//        //        return "Invalid email or password";
//        //    }
//        //    return "Login successful";



//        //}
//        public async Task<AuthResponse> RegisterAsync(RegisterRequest registerRequest)
//        {
//            try
//            {
//                _logger.LogInformation($"Registration attempt for email: {registerRequest.Email}");

//                // التحقق من صحة البيانات
//                var validationResult = ValidateRegistration(registerRequest);
//                if (!validationResult.IsValid)
//                {
//                    return CreateErrorResponse(validationResult.ErrorMessage);
//                }

//                // التحقق من عدم وجود البريد الإلكتروني مسبقاً
//                var users = await _appRepository.GetAllAsync();
//                if (users.Any(x => x.Email == registerRequest.Email))
//                {
//                    _logger.LogWarning($"Registration failed: Email already exists - {registerRequest.Email}");
//                    return CreateErrorResponse("Email is already registered");
//                }

//                // تشفير كلمة المرور باستخدام BCrypt
//                string passwordHash = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);

//                // إنشاء مستخدم جديد
//                var user = new User
//                {
//                    Name = registerRequest.Name.Trim(),
//                    Email = registerRequest.Email.ToLower().Trim(),
//                    PasswordHash = passwordHash,
//                    Role = "User",
//                    IsActive = true,
//                    CreatedAt = DateTime.UtcNow
//                };

//                // حفظ المستخدم
//                await _appRepository.AddAsync(user);
//                await _appRepository.SaveChangesAsync();

//                // إنشاء Token تلقائياً بعد التسجيل
//                var token = GenerateJwtToken(user);
//                var userDto = _mapper.Map<UserDto>(user);

//                _logger.LogInformation($"User registered successfully: {user.Email}");

//                return new AuthResponse
//                {
//                    Success = true,
//                    Message = "Registration successful",
//                    Token = token,
//                    TokenExpiration = DateTime.UtcNow.AddHours(24),
//                    User = userDto
//                };
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, $"Error during registration for {registerRequest.Email}");
//                return CreateErrorResponse("An error occurred during registration");
//            }
//        }

//        //public async Task<UserDto> Register(Register register)
//        //{

//        //    var user = new User
//        //    {
//        //        Name = register.Name,
//        //        Email = register.Email,
//        //        Password = register.Password,
//        //        Role = "user"
//        //    };
//        //    await _appRepository.AddAsync(user);
//        //    return _mapper.Map<UserDto>(user);
//        //}


//        public async Task<UserDto> GetUserByEmailAsync(string email)
//        {
//            try
//            {
//                var users = await _appRepository.GetAllAsync();
//                var user = users.FirstOrDefault(x => x.Email == email);

//                if (user == null)
//                {
//                    return null;
//                }

//                return _mapper.Map<UserDto>(user);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, $"Error getting user by email {email}");
//                throw;
//            }
//        }








//        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
//        {
//            try
//            {
//                var users = await _appRepository.GetAllAsync();
//                return _mapper.Map<IEnumerable<UserDto>>(users);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting all users");
//                throw;
//            }
//        }

//        public async Task<AuthResponse> UpdateUserAsync(int id, UpdateUserRequest updateRequest, int currentUserId)
//        {
//            try
//            {
//                // التحقق من الصلاحيات
//                if (id != currentUserId)
//                {
//                    var currentUser = await _appRepository.GetByIdAsync(currentUserId);
//                    if (currentUser?.Role != "Admin")
//                    {
//                        return CreateErrorResponse("You don't have permission to update this user");
//                    }
//                }

//                var user = await _appRepository.GetByIdAsync(id);
//                if (user == null)
//                {
//                    return CreateErrorResponse("User not found");
//                }

//                bool hasChanges = false;

//                // تحديث الاسم
//                if (!string.IsNullOrWhiteSpace(updateRequest.Name) && user.Name != updateRequest.Name)
//                {
//                    user.Name = updateRequest.Name.Trim();
//                    hasChanges = true;
//                }

//                // تحديث كلمة المرور
//                if (!string.IsNullOrWhiteSpace(updateRequest.NewPassword))
//                {
//                    if (string.IsNullOrWhiteSpace(updateRequest.CurrentPassword))
//                    {
//                        return CreateErrorResponse("Current password is required to change password");
//                    }

//                    if (!BCrypt.Net.BCrypt.Verify(updateRequest.CurrentPassword, user.PasswordHash))
//                    {
//                        return CreateErrorResponse("Current password is incorrect");
//                    }

//                    if (updateRequest.NewPassword != updateRequest.ConfirmNewPassword)
//                    {
//                        return CreateErrorResponse("New passwords do not match");
//                    }

//                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateRequest.NewPassword);
//                    hasChanges = true;
//                }

//                if (hasChanges)
//                {
//                    user.UpdatedAt = DateTime.UtcNow;
//                    await _appRepository.UpdateAsync(user);
//                    await _appRepository.SaveChangesAsync();

//                    _logger.LogInformation($"User updated successfully: {user.Email}");
//                }

//                var updatedUserDto = _mapper.Map<UserDto>(user);
//                var token = GenerateJwtToken(user);

//                return new AuthResponse
//                {
//                    Success = true,
//                    Message = hasChanges ? "User updated successfully" : "No changes detected",
//                    Token = token,
//                    TokenExpiration = DateTime.UtcNow.AddHours(24),
//                    User = updatedUserDto
//                };
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, $"Error updating user {id}");
//                return CreateErrorResponse("An error occurred while updating user");
//            }
//        }







//        private AuthResponse CreateErrorResponse(string message)
//        {
//            return new AuthResponse
//            {
//                Success = false,
//                Message = message,
//                Token = null,
//                TokenExpiration = DateTime.MinValue,
//                User = null
//            };
//        }
//        public async Task<bool> ValidateTokenAsync(string token)
//        {
//            try
//            {
//                var tokenHandler = new JwtSecurityTokenHandler();
//                var validationParameters = new TokenValidationParameters
//                {
//                    ValidateIssuerSigningKey = true,
//                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret)),
//                    ValidateIssuer = true,
//                    ValidIssuer = _jwtIssuer,
//                    ValidateAudience = true,
//                    ValidAudience = _jwtAudience,
//                    ValidateLifetime = true,
//                    ClockSkew = TimeSpan.Zero
//                };

//                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
//                return principal != null;
//            }
//            catch
//            {
//                return false;
//            }
//        }
//        private string GenerateJwtToken(User user)
//        {
//            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
//            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

//            var claims = new[]
//            {
//                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
//                new Claim(JwtRegisteredClaimNames.Email, user.Email),
//                new Claim(ClaimTypes.Name, user.Name),
//                new Claim(ClaimTypes.Role, user.Role),
//                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
//            };

//            var token = new JwtSecurityToken(
//                issuer: _jwtIssuer,
//                audience: _jwtAudience,
//                claims: claims,
//                expires: DateTime.UtcNow.AddHours(24),
//                signingCredentials: credentials
//            );

//            return new JwtSecurityTokenHandler().WriteToken(token);
//        }
//    }
//}
