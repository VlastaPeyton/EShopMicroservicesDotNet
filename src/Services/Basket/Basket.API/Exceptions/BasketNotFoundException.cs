using BuildingBlocks.Exceptions;

namespace Basket.API.Exceptions
{
    public class BasketNotFoundException : NotFoundException
    { // NotFoundException defined in BuildingBlocks
        public BasketNotFoundException(string userName) : base("Basket", userName) { }
    }
}
