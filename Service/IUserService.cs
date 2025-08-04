using NestLeaf.Dto;
using NestLeaf.Models;

namespace NestLeaf.Services

{
    public interface IUserService
    {
        Task<UserRegisterDto> RegisterUser(UserRegisterDto register);
        Task<LoginResponseDto> UserLogin(UserLoginDto logindto);
        Task<List<UserAdminDto>> GetAllUser();

        Task<UserAdminDto> GetById(int id);
        

        Task <bool> DeleteById(int id);
        Task<int> BlockUnBlockUser(int id);

      


    }
}
        

