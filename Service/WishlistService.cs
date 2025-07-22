using Dapper;
using NestLeaf.Dto;
using System.Data;

namespace NestLeaf.Service
{
    public class WishlistService:IwishlistService
    {
        private readonly IDbConnection _connection;

        public WishlistService(IDbConnection connection)
        {
            _connection = connection;
        }
        public async Task<bool> AddtoWishlist(int userId, int productId)
        {
            var existitem=await _connection.QueryAsync("select 1 from Wishlists where UserId=@userId and ProductId=@productId", new { UserId = userId, ProductId = productId });

            if (existitem.Any())
            {
                return false;
            }

            var result = await _connection.ExecuteAsync("AddtoWishlist", new { userId, productId }, commandType: CommandType.StoredProcedure);

            return true;
        }

       public async Task<List<wishlistDto>>GetWishList(int userID)
        {
            var getwishlist = (await _connection.QueryAsync<wishlistDto>("GetWishList", new { userID }, commandType: CommandType.StoredProcedure)).ToList();

            return getwishlist;
           
        
        }

           public async Task<bool> RemoveFromWishlist(int userId, int productId)
        {

            var rowaffected=await _connection.ExecuteAsync("RemoveFromWishlist",new { userId, productId });

            return rowaffected > 0;
        }

    }
}
