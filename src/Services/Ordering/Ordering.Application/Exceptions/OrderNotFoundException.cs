
using BuildingBlocks.Exceptions;

namespace Ordering.Application.Exceptions
{
    public class OrderNotFoundException : NotFoundException // From BuildingBlocks
    {
        public OrderNotFoundException(Guid id) : base("Order", id) { }
    }
}
