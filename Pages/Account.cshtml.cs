using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CS3750_PlanetExpressLMS.Data;
using CS3750_PlanetExpressLMS.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace CS3750_PlanetExpressLMS.Pages
{
    public class AccountModel : PageModel
    {
        private readonly IUserRepository userRepository;
        private readonly ICourseRepository courseRepository;
        private readonly IInvoiceRepository invoiceRepository;
        private readonly IPaymentRepository paymentRepository;

        public AccountModel(IUserRepository userRepository, ICourseRepository courseRepository, IInvoiceRepository invoiceRepository, IPaymentRepository paymentRepository)
        {
            this.userRepository = userRepository;
            this.courseRepository = courseRepository;
            this.invoiceRepository = invoiceRepository;
            this.paymentRepository = paymentRepository;
        }

        [BindProperty]
        public User User { get; set; }

        [BindProperty]
        public Payment Payment { get; set; }

        [BindProperty]
        public List<Invoice> InvoiceList { get; set; }

        [BindProperty]
        public Invoice oldInvoice { get; set; }

        [BindProperty]
        public Course Course { get; set; }

        [BindProperty]
        public List<Course> UserCourses { get; set; }

        [BindProperty]
        public int creditHours { get; set; }

        [BindProperty]
        public string cardNumber { get; set; }

        [BindProperty]
        public string cvv { get; set; }

        [BindProperty]
        public string firstName { get; set; }

        [BindProperty]
        public string lastName { get; set; }

        [BindProperty]
        public string amountPaid { get; set; }

        [BindProperty]
        public Invoice newInvoice { get; set; }

        public async Task<IActionResult> OnGet(int? id)
        {
            // If no id was passed, return not found
            if (id == null) { return NotFound(); }

            // Look up the user based on the id
            User = userRepository.GetUser((int)id);

            // If the user does not exist, return not found
            if (User == null) { return NotFound(); }

            UserCourses = courseRepository.GetStudentCourses(User.ID);
            
            if (UserCourses != null)
            {
                InvoiceList = invoiceRepository.GetInvoices(User.ID);

                foreach (Course course in UserCourses)
                {
                    creditHours += course.CreditHours;
                }

                if (InvoiceList.Count != 0)
                {
                    oldInvoice = InvoiceList.LastOrDefault(Invoice => Invoice.ID == id);
                }
                else
                {
                    Invoice fisrtInvoice = new Invoice();

                    decimal balance = 0.00M;

                    balance = creditHours * 100;

                    fisrtInvoice.FullBalance = balance;

                    fisrtInvoice.ID = User.ID;

                    invoiceRepository.Add(fisrtInvoice);
                }
                
            }
            // Otherwise, return the page
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // might create duplicate payment info, but different paymentID

            // catch payment info
            // Add it to DB
            Payment newPayment = new Payment();

            firstName = Request.Form["txtFirstName"];

            newPayment.FirstName = firstName;

            lastName = Request.Form["txtLastName"];

            newPayment.LastName = lastName;

            cardNumber = Request.Form["txtCardNumber"];

            newPayment.CardNumber = cardNumber;

            cvv = Request.Form["txtCvv"];

            newPayment.Cvv = cvv;

            User = userRepository.GetUser(User.ID);

            newPayment.ID = User.ID;

            // make sure cc # is exactly 16, all integer numbers
            // make sure CVV is exactly 3, all integer numbers
            // change Exp date to calendar object
            // payment amount isnt less than $0.01, greater than invoice balance, non integer, no more than 2 decimal values


            // Create a new invoice
            // Full balance -= paymentamount
            // add invoice to db
            // confirmation msg

            Invoice newInvoice = new Invoice();

            // card #, CVV are integers

            newInvoice.ID = User.ID;

            amountPaid = Request.Form["txtAmount"];

            newInvoice.AmountPaid = Decimal.Parse(amountPaid);

            InvoiceList = invoiceRepository.GetInvoices(User.ID);

            oldInvoice = InvoiceList.LastOrDefault(Invoice => Invoice.ID == User.ID);

            newInvoice.FullBalance = oldInvoice.FullBalance - newInvoice.AmountPaid;

            invoiceRepository.Add(newInvoice);

            // Code for api
            HttpClient client = new HttpClient();

            // client.PostAsync(get request); token request

            // client.PostAsync(get request); payment request

            paymentRepository.Add(newPayment);

            // Change amount owed and credits displayed
            oldInvoice = newInvoice;

            UserCourses = courseRepository.GetStudentCourses(User.ID);

            foreach (Course course in UserCourses)
            {
                creditHours += course.CreditHours;
            }

            return Page();
        }
    }
}
