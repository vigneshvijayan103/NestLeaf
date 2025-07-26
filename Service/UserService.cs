using BCrypt.Net;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using NestLeaf.Dto;
using NestLeaf.Models;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;




namespace NestLeaf.Services
{
    public class UserService : IUserService
    {
        private readonly NestLeafDbContext _context;
        private readonly TokenService _tokenService;
        private readonly IDbConnection _connection;
        public UserService(NestLeafDbContext context, TokenService tokenService, IDbConnection connection)
        {
            _context = context;
            _tokenService = tokenService;
            _connection = connection;
        }



        public async Task<UserRegisterDto> RegisterUser(UserRegisterDto dto)
        {
            bool exists = await _context.Users.AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email || u.PhoneNumber == dto.PhoneNumber);


            if (exists)
            {
                return null;
            }


            var hashpassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);



            var newUser = new User
            {
                Name = dto.Username,
                Username = dto.Username,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Password = hashpassword

            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return new UserRegisterDto
            {
                Name=dto.Name,
                Username = dto.Username,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber

            };
        }


        public async Task<string> UserLogin(UserLoginDto logindto)
        {
            var user =  _context.Users.FromSqlRaw("Exec LoginUser @p0", logindto.Username).AsEnumerable().FirstOrDefault();

          

             if (user == null || !BCrypt.Net.BCrypt.Verify(logindto.Password, user.Password))
            {
                return null;
            }

            if (user.IsBlocked)
            {
                return "blocked";
            }
      

            var token = _tokenService.createToken(user);
            return token;
            return user.Name;
        }


        public async Task<List<UserAdminDto>> GetAllUser()
        {
            var users = await _connection.QueryAsync<UserAdminDto>(
                "GetAllUsers",
                commandType: CommandType.StoredProcedure
            );

            return users.ToList();
        }

        public async Task<UserAdminDto> GetById(int id)
        {
            var user = await _connection.QueryFirstOrDefaultAsync<UserAdminDto>(
                "GetUserById",
                new { UserId = id },
                commandType: CommandType.StoredProcedure
            );

            return user;
        }

 

        public async Task<bool> DeleteById(int id)
        {
            var rowsAffected = await _connection.ExecuteAsync(
                "SoftDeleteUser",
                new { UserId = id },
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected > 0;
        }

        public async Task<int> BlockUnBlockUser(int id)
        {
            var result = await _connection.QueryFirstOrDefaultAsync<int>(
           "BlockUser",
           new { UserId = id },
           commandType: CommandType.StoredProcedure
       );

            return result;
        }

     








    }
}

