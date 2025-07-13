

namespace Ordering.Domain.DomainExceptions
{   // Ne referencira BB i zato ne moze da se uhvati u CustomExceptionHandler + nije ovo API/Applicaiton layer pa da nam to treba
    public class DomainException : Exception 
    {
        public DomainException(string message) : 
            base($" Domain exception {message} throw from Domain layer")
        {
        }
    }
}
