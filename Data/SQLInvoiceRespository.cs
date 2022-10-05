using CS3750_PlanetExpressLMS.Models;
using System.Collections.Generic;
using System.Linq;

namespace CS3750_PlanetExpressLMS.Data
{
    public class SQLInvoiceRepository : IInvoiceRepository
    {
        public readonly CS3750_PlanetExpressLMSContext context;
        public SQLInvoiceRepository(CS3750_PlanetExpressLMSContext context)
        {
            this.context = context;
        }

        // Create method to add a new invoice
        public Invoice Add(Invoice newInvoice)
        {
            context.Invoice.Add(newInvoice);
            context.SaveChanges();
            return newInvoice;
        }

        public IEnumerable<Invoice> GetAllInvoices()
        {
            return context.Invoice;
        }
        public List<Invoice> GetInvoices(int id)
        {
            var userInvoice = GetAllInvoices();
            userInvoice = userInvoice.Where(c => c.ID == id);
            return userInvoice.ToList<Invoice>();
        }



    }
}
