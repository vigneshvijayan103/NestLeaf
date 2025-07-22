using Dapper;
using Microsoft.EntityFrameworkCore;
using NestLeaf.Dto;
using NestLeaf.Models;
using System.Data;

namespace NestLeaf.Service
{
    public class AddressService:IAddressService
    {
        private readonly IDbConnection _connection;
        private readonly NestLeafDbContext _context;
        public AddressService(IDbConnection connection,NestLeafDbContext context)
        {
            _connection = connection;
            _context = context;
        }

        public async Task<bool> Addaddress(AddressDto dto,int userId)
        {

            if (dto.IsDefault)
            {
                var existingDefaults = await _context.Addresses
                    .Where(a => a.UserId == userId && a.IsDefault && a.DeletedAt == null)
                    .ToListAsync();

                foreach (var adrs in existingDefaults)
                {
                    adrs.IsDefault = false;
                    adrs.UpdatedAt = DateTime.UtcNow;
                }
            }

            var address = new Address
            {
                UserId=userId,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                AddressLine1 = dto.AddressLine1,
                AddressLine2 = dto.AddressLine2,
                City = dto.City,
                State = dto.State,
                Pincode = dto.Pincode,
                Country = dto.Country,
                IsDefault = dto.IsDefault,
                CreatedAt = DateTime.UtcNow
            };

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            return true;
        }
      
        public async Task<List<AddressDto>> GetAddresses(int userId)
        {
            var result = (await _connection.QueryAsync<AddressDto>(
                "GetUserAddresses",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure
            )).ToList();
            return result;
        }

        public async Task<bool> UpdateAddress(int id, UpdateAddressDto dto, int userId)
        {
            var address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId && a.DeletedAt == null);
            if (address == null) 
                return false;


            if (dto.IsDefault.HasValue)
            {
                if (dto.IsDefault.Value && !address.IsDefault)
                {
                    var defaults = await _context.Addresses
                        .Where(a => a.UserId == userId && a.IsDefault && a.DeletedAt == null)
                        .ToListAsync();

                    foreach (var ad in defaults)
                    {
                        ad.IsDefault = false;
                        ad.UpdatedAt = DateTime.UtcNow;
                    }

                    address.IsDefault = true;
                }
                else if (!dto.IsDefault.Value && address.IsDefault)
                {
                    address.IsDefault = false;
                }
            }

            if (!string.IsNullOrWhiteSpace(dto.FullName))
                address.FullName = dto.FullName;

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                address.PhoneNumber = dto.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(dto.AddressLine1))
                address.AddressLine1 = dto.AddressLine1;

            if (!string.IsNullOrWhiteSpace(dto.AddressLine2))
                address.AddressLine2 = dto.AddressLine2;

            if (!string.IsNullOrWhiteSpace(dto.City))
                address.City = dto.City;

            if (!string.IsNullOrWhiteSpace(dto.State))
                address.State = dto.State;

            if (!string.IsNullOrWhiteSpace(dto.Pincode))
                address.Pincode = dto.Pincode;

            if (!string.IsNullOrWhiteSpace(dto.Country))
                address.Country = dto.Country;

            address.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteAddress(int id, int userId)
        {
            var address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId && a.DeletedAt == null);
            if (address == null)
                return false;

            address.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
