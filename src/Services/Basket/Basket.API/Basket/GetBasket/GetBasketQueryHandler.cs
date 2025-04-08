
using Basket.API.Data;

namespace Basket.API.Basket.GetBasket
{   /* Prvo pogledaj GetBasketEndpoint.cs i onu sliku da bi razume ovo lakse, jer iz Endpoint mapira
    se ovde */

    public record GetBasketQuery(string UserName) : IQuery<GetBasketResult>;
    public record GetBasketResult(ShoppingCart Cart);
    /*Query i Result object mora imati argumente istog imeta i tipa kao Request i Response object u 
     Endpoint klasi, respektivno, kako bi mapiranje moglo da se izvrsi

     Command/Query i Result object mora imati argumente istog imeta i tipa kao Request i Response object u 
     Endpoint klasi, respektivno, kako bi mapiranje moglo da se izvrsi.  */
    public class GetBasketQueryHandler (IBasketRepository repository)
        : IQueryHandler<GetBasketQuery, GetBasketResult>
    {   /* IBasketRepository, a ne BasketRepositor, jer znamo da uvek interface ide, a u Program.cs cu da registrujem
         da prepoznaje IBasketRepository kao BasketRepository. */

        /* Mora ova metoda da se override zbog interface i mora da dodam async da bi automatski pretvorila Result object 
        u Task, jer ako nema async, moram rucno da pretvaram, plus koristicu await kod save to DB komande.  */
        public async Task<GetBasketResult> Handle(GetBasketQuery query, CancellationToken cancellationToken)
        {
            var basket = await repository.GetBasket(query.UserName, cancellationToken);
       
            return new GetBasketResult(basket);
            // Jer ShoppingCart ima konstruktor koji zahteva UserName
        }
    }
}
