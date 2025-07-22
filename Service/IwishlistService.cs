using NestLeaf.Dto;

namespace NestLeaf.Service
{
    public interface IwishlistService
    {
        Task<bool> AddtoWishlist(int userId, int productId);

        Task<List<wishlistDto>> GetWishList(int userdID);

        Task<bool> RemoveFromWishlist(int userId, int productId);



    }
}
