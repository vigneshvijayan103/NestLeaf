﻿namespace NestLeaf.Dto
{
    public class CartItemDto
    {
     
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public decimal TotalPrice { get; set; }





    }
}
