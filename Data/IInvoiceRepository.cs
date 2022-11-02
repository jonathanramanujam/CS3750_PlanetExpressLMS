using CS3750_PlanetExpressLMS.Models;
using System.Collections.Generic;

namespace CS3750_PlanetExpressLMS.Data
{
    public interface IInvoiceRepository
    {
        Invoice Add(Invoice newInvoice);
        IEnumerable<Invoice> GetAllInvoices();
        List<Invoice> GetInvoices(int id);
    }
}
