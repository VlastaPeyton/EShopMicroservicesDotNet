using Discount.gRPC.Data;
using Discount.gRPC.Models;
using Grpc.Core;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Discount.gRPC.Services
{   
    // Business layer u kom kroz .NET kodiram sve iz discunt.proto 
    public class DiscountService (DiscountDbContext dbContext, ILogger<DiscountService> logger) : DiscountProtoService.DiscountProtoServiceBase
    {   // DiscountProtoService je definisan u discount.proto file. 
        //Moram ovako logger, jer Discount ne referencira BuildingBlocks (pa da kao Catalog i Basket  mogu samo da pozovem logger iz MediatR pipeline LoggingBehaviour automatski). 

        /*Moram da override sve metode(Endpoints) iz DiscountProtoService jer Discount je gRCP Server dok Basket je gRPC client.
         
           GetDiscountRequest, CreateDiscountRequest, UpdateDiscountRequest, DeleteDsicountRequest i CouponModel je definisan u discount.proto 
           dBContext povezuje ovaj kod sa SQLite. */

        public override async Task<CouponModel> GetDiscount (GetDiscountRequest request, ServerCallContext context)
        {   /*  Da sam u GetDiscountRequest napisao productName umesto ProductName, opet bi moglo samo ProductName, jer
            .NET pravi iz discount.proto sve u PascalCase.*/

            /* Na osnovu ProductName nadje Coupon u bazi. FirstOrDefault vrati null ako nismo nasli kupon, jer samo First 
            ce da vrati error ako nismo nasli kupon. */
            var coupon = await dbContext.Coupons.FirstOrDefaultAsync(x => x.ProductName == request.ProductName);

            // Ako nema popust za zeljeni product
            if (coupon is null) // Isto kao == null, samo sigurnije
                coupon = new Coupon { ProductName = "No Discount", Amount = 0, Description = "No Discount Desc" };// Ne navodim Id, jer ce biti default i u Coupon i u CouponModel posto nas ne zanima Id
            
            logger.LogInformation($"Discount is retrieved for ProductName : {coupon.ProductName}, Amount :{coupon.Amount}");
            
            // Mapiram iz Coupon to CouponModel jer CouponModel vraca GetDiscount 
            var couponModel = coupon.Adapt<CouponModel>();

            return couponModel;
        }

        public override async Task<CouponModel> CreateDiscount (CreateDiscountRequest request, ServerCallContext context)
        {
            /* 
              Da sam u GetDiscountRequest napisao coupon umesto Coupon, opet bi moglo samo Coupon, jer
            .NET pravi iz discount.proto sve u PascalCase. */

            var coupon = request.Coupon.Adapt<Coupon>();

            // Ako iz nekog razloga kupon je null
            if (coupon is null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid request object"));

            // Ako je kupon u redu
            dbContext.Coupons.Add(coupon);
            await dbContext.SaveChangesAsync(); // Ima commit i rollback implicitno

            logger.LogInformation($"Discount is created for ProductName: {coupon.ProductName}, Amount : {coupon.Amount}");

            // Mapiram iz Coupon to CouponModel jer CreateDiscount vraca CouponModel
            var couponModel = coupon.Adapt<CouponModel>();

            return couponModel;
        }

        public override async Task<CouponModel> UpdateDiscount (UpdateDiscountRequest request, ServerCallContext context)
        {
            /* UpdateDiscountRequest i CouponModel definisani u discount.proto
              dbContext povezuje ovaj kod sa bazom. 
              Da sam u GetDiscountRequest napisao coupon umesto Coupon, opet bi moglo samo Coupon, jer
            .NET pravi iz discount.proto sve u PascalCase. */

            var coupon = request.Coupon.Adapt<Coupon>();

            // Ako iz nekog razloga kupon je null
            if (coupon is null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid request object"));

            dbContext.Coupons.Update(coupon);
            await dbContext.SaveChangesAsync(); // Ima commit i rollback implicitno

            logger.LogInformation($"Discount is updated for ProductName: {coupon.ProductName}, Amount : {coupon.Amount}");

            // Mapiram iz Coupon to CouponModel jer Updatediscount vraca CouponModel
            var couponModel = coupon.Adapt<CouponModel>();

            return couponModel;

        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            /* 
              Da sam u GetDiscountRequest napisao coupon umesto Coupon, opet bi moglo samo Coupon, jer
            .NET pravi iz discount.proto sve u PascalCase. */

            // Na osnovu ProductName nadje Coupon u bazi
            var coupon = await dbContext.Coupons.FirstOrDefaultAsync(x => x.ProductName == request.ProductName);

            // Ako ne postoji kupon koji zelim obrisati
            if (coupon is null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid request object"));

            // Brisem kupon ako postoji
            dbContext.Coupons.Remove(coupon);
            await dbContext.SaveChangesAsync();

            logger.LogInformation($"Discount is updated for ProductName: {coupon.ProductName}, Amount : {coupon.Amount}");

            return new DeleteDiscountResponse { Success = true };

        }
    }
}
