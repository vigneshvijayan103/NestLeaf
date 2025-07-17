using NestLeaf.Dto;
using NestLeaf.Models;

namespace NestLeaf.Services

{
    public interface IUserService
    {
        Task<UserRegisterDto> RegisterUser(UserRegisterDto register);
        Task<string> UserLogin(UserLoginDto logindto);
        Task<List<UserAdminDto>> GetAllUser();

        Task<UserAdminDto> GetById(int id);
        

        Task <bool> DeleteById(int id);
        Task<bool> BlockUser(int id);
        Task<bool> Unblock(int id);
    }
}
        

