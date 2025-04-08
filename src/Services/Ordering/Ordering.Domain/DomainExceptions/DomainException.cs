

namespace Ordering.Domain.DomainExceptions
{
    public class DomainException : Exception
    {
        public DomainException(string message) : 
            base($" Domain exception {message} throw from Domain layer")
        {
        }
    }
}
