using CS3750_PlanetExpressLMS.Models;
using System.Collections.Generic;
using System.Linq;

namespace CS3750_PlanetExpressLMS.Data
{
    public class SQLPaymentRepository : IPaymentRepository
    {
        public readonly CS3750_PlanetExpressLMSContext context;
        public SQLPaymentRepository(CS3750_PlanetExpressLMSContext context)
        {
            this.context = context;
        }

        // Create method to add a new invoice
        public Payment Add(Payment newPayment)
        {
            context.Payment.Add(newPayment);
            context.SaveChanges();
            return newPayment;
        }
    }
}
