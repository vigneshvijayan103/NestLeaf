using Dapper;
using Microsoft.EntityFrameworkCore;
using NestLeaf.Dto;
using NestLeaf.Models;
using System.Data;

namespace NestLeaf.Service
{
    public class CartService : ICartService

    {
        private readonly NestLeafDbContext _context;
        private readonly IDbConnection _dbconnection;

        public CartService(NestLeafDbContext context, IDbConnection connection)
        {
            _context = context;
            _dbconnection = connection;
        }
        public async Task<CartItemDto> AddtoCart(AddCartDto dto, int userid)
        {
            var stock = await _dbconnection.QueryFirstOrDefaultAsync<int?>(
                         "SELECT Quantity FROM Products WHERE Id = @ProductId",
                           new { ProductId = dto.ProductId });

            if (stock == null)
                throw new Exception("Product not found.");


            var existingQty = await _dbconnection.QueryFirstOrDefaultAsync<int?>(
                 "SELECT Quantity FROM Cart WHERE UserId = @UserId AND ProductId = @ProductId AND DeletedAt IS NULL",
                     new { UserId = userid, ProductId = dto.ProductId });

            int totalRequestedQty = dto.Quantity + (existingQty ?? 0);
            if (totalRequestedQty > stock)
                throw new Exception("Not enough stock available.");


            var cartitem = await _dbconnection.QueryFirstOrDefaultAsync<CartItemDto>(
                "InsertCart", new { userid, dto.ProductId, dto.Quantity },
                commandType: CommandType.StoredProcedure);

            return cartitem;

        }
        public async Task<ViewCartDto> GetCart(int userid)
        {
            var Getcartitem = (await _dbconnection.QueryAsync<CartItemDto>("GetCartItem", new { userid }, commandType: CommandType.StoredProcedure)).ToList();


            if (!Getcartitem.Any())
                return null;

            var result = new ViewCartDto
            {
                UserId = userid,
                items = Getcartitem,
                CartTotalPrice = Getcartitem.Sum(t => t.TotalPrice)



            };

            return result;
        }

        public async Task<ViewCartDto> UpdateCartQuantity(AddCartDto dto, int userid)
        {
            var existingcart = await _dbconnection.QueryFirstOrDefaultAsync<int?>("  select  Quantity from Cart  where ProductId = @ProductId and UserId = @UserID AND DeletedAt IS NULL", new { UserId = userid, ProductId = dto.ProductId });

            if (existingcart==null)
                return null;

            var stock = await _dbconnection.QueryFirstOrDefaultAsync<int?>(
                                    "SELECT Quantity FROM Products WHERE Id = @ProductId", new { ProductId = dto.ProductId });

            if (stock is null)
                return null;

            int newTotalQuantity = (existingcart ?? 0) + dto.Quantity;

            
            if (newTotalQuantity > stock)
                return null;



            var updatecart = (await _dbconnection.QueryAsync<CartItemDto>("UpdateCartQuantity", new
            {
                userid,
                dto.ProductId,
                dto.Quantity
            }, commandType: CommandType.StoredProcedure)).ToList();




            var result = new ViewCartDto
            {
                UserId = userid,
                items = updatecart,

                CartTotalPrice = updatecart.Sum(t => t.TotalPrice)
            };

            return result;
        }

        public async Task<ViewCartDto> RemoveItem(int userid, int productId)
        {
            var existingcart = await _dbconnection.QueryAsync("  select  1 from Cart  where ProductId = @ProductId and UserId = @UserID ", new { UserId = userid, ProductId = productId });

            if (!existingcart.Any())
                return null;

            var removeitem = (await _dbconnection.QueryAsync<CartItemDto>("RemoveCartItem", new { userid, productId }, commandType: CommandType.StoredProcedure)).ToList();



            var result = new ViewCartDto
            {
                UserId = userid,
                items = removeitem,

                CartTotalPrice = removeitem.Sum(t => t.TotalPrice)
            };

            return result;
        }

        public async Task<bool> DeleteCart(int userid)
        {
            var cartExists = await _dbconnection.QueryAsync("SELECT 1 FROM Cart WHERE UserId = @UserId ",
        new { UserId = userid });

            if (!cartExists.Any())
                return false;

            var RemoveCart = await _dbconnection.QueryAsync("DeleteCart", new { userid }, commandType: CommandType.StoredProcedure);
            return true;

        }

    }

}
