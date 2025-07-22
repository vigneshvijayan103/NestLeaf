using Microsoft.AspNetCore.Mvc;
using NestLeaf.Dto;
using NestLeaf.Models;

namespace NestLeaf.Service
{
    public interface IAddressService
    {
        Task<bool> Addaddress(AddressDto dto, int userId);
        Task<List<AddressDto>> GetAddresses(int userId);
        Task<bool> UpdateAddress(int id, UpdateAddressDto dto, int userId);
        Task<bool> DeleteAddress(int id, int userId);
    }
}
