using NestLeaf.Dto;
using NestLeaf.Models;
namespace NestLeaf.Service
{
    public interface ICartService 
    {
        
         Task<CartItemDto> AddtoCart(AddCartDto dto, int userid);
         Task<ViewCartDto> GetCart(int userid);
         Task<ViewCartDto> UpdateCartQuantity(AddCartDto dto, int userid);

        Task<ViewCartDto> RemoveItem( int userid,int productId);
        Task<bool> DeleteCart(int userid);

    }
}
