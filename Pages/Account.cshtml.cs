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
            // If user input is invalid, return page
            if (!validPayment())
            {
                return Page();
            }

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
        } // End of On Post

        #region Valid Payment
        /// <summary>
        /// Verifies if values entered by user is valid
        /// </summary>
        /// <returns></returns>
        public bool validPayment()
        {
            // Length of credit card should be 16
            int CCNumLen = 16;

            // Length of CVV should be 3
            int CCVNumLen = 3;

            // Checks if Credit card and CVV number have the correct # of digits
            if (!isCorrectStrLen(CCNumLen, Request.Form["txtCardNumber"]) 
                || !isCorrectStrLen(CCVNumLen, Request.Form["txtCvv"]))
            {
                return false;
            }



            // change Exp date to calendar object
            // payment amount isnt less than $0.01, greater than invoice balance, non integer, no more than 2 decimal values
            // Student cant make payment if balance is 0

            // Create a new invoice
            // Full balance -= paymentamount
            // add invoice to db
            // confirmation msg
            return false;
        }
        #endregion

        #region Correct String Length
        /// <summary>
        /// Checks if input is the appropriate length
        /// </summary>
        /// <returns></returns>
        public bool isCorrectStrLen(int correctStrLen, string str)
        {
            // Checks if both strings have the same length
            if(correctStrLen != str.Length){
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

    } // End of class
}
