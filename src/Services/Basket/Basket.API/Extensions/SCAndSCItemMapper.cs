using Basket.API.DTOs;

namespace Basket.API.Extensions
{   
    public static class SCAndSCItemMapper
    {
        public static ReturnShoppingCartDTO ToReturnShoppingCartDTO(this ShoppingCart shoppingCart)
        {
            return new ReturnShoppingCartDTO
            {
                UserName = shoppingCart.UserName,
                TotalPrice = shoppingCart.TotalPrice,
                Items = shoppingCart.Items.Select(i => new ReturnShoppingCartItemDTO
                {   // Svaki ShoppingCartItem iz ShoppingCart pretvaram ga u ReturnShoppingCartItemDTO
                    Quantity = i.Quantity,
                    Color = i.Color,
                    Name = i.Name,
                    Price = i.Price,
                    ProductName = i.ProductName
                }).ToList()
            };
        }

        public static ShoppingCart FromSCDtoToSC(this StoreShoppingCartDTO storeSCDto)
        {
            return new ShoppingCart
            {
                UserName = storeSCDto.UserName,
                Items = storeSCDto.Items.Select(i => new ShoppingCartItem
                {
                    Quantity = i.Quantity,
                    Color = i.Color,
                    Name = i.Name,
                    Price = i.Price,
                    ProductName = i.ProductName,
                    ProductId = i.ProductId
                }).ToList()
            };
        }
    }
}
