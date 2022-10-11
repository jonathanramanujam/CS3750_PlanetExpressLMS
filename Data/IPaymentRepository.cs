using CS3750_PlanetExpressLMS.Models;
using System.Collections.Generic;

namespace CS3750_PlanetExpressLMS.Data
{
    public interface IPaymentRepository
    {
        Payment Add(Payment newPayment);
    }
}
