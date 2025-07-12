using Catalog.API.DTOs;

namespace Catalog.API.Extensions
{
    public static class ProductMapper
    {
        public static ProductResultDTO ToGetProductByIdResultDTO(this Product product)
        {
            return new ProductResultDTO
            {
                Name = product.Name,
                Description = product.Description,
                ImageFile = product.ImageFile,
                Price = product.Price,
                Category = product.Category,
            };
        }
    }
}
