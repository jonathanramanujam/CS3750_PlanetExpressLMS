using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CS3750_PlanetExpressLMS.Data;
using CS3750_PlanetExpressLMS.Models;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Security.Cryptography.Xml;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

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

        /*
         Future tasks for this page:
            Confirmation message once user successfully submits a payment
            Warning msg when student tries to make a payment when balance is paid off
            Warning msg whenever they input wrong thing (it just refreshes page for now)
            'Clean up' code by abstracting out code into their appropriate functions
            How to deal when user made at least one payment, then drops a course. What should we do then? 
         */
        #region BindProperties
        [BindProperty]
        public User user { get; set; }

        [BindProperty]
        public Payment payment { get; set; }

        [BindProperty]
        public List<Invoice> invoices { get; set; }

        [BindProperty]
        public Invoice oldInvoice { get; set; }

        [BindProperty]
        public List<Course> courses { get; set; }

        [BindProperty]
        public int creditHours { get; set; }

        [BindProperty]
        public decimal balance { get; set; }

        [BindProperty]
        public string cardNumber { get; set; }

        [BindProperty]
        public string cvv { get; set; }

        [BindProperty]
        public string firstName { get; set; }

        [BindProperty]
        public string lastName { get; set; }

        [BindProperty]
        public DateTime expDate { get; set; }

        [BindProperty]
        public string amountPaid { get; set; }

        [BindProperty]
        public Invoice newInvoice { get; set; }

        [BindProperty]
        public string errorMessage { get; set; }
        #endregion

        public async Task<IActionResult> OnGet()
        {
            // Access the current session
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);

            // Make sure a user is logged in
            user = session.GetUser();

            if (user == null) 
            {
                return RedirectToPage("Login");
            }

            // Get courses from session
            courses = session.GetCourses();
            
            if (courses != null)
            {
                invoices = session.GetInvoices();

                if (invoices == null)
                {
                    // Get existing invoices
                    invoices = invoiceRepository.GetInvoices(user.ID);

                    // save invoices to session
                    session.SetInvoices(invoices);
                }

                foreach (Course course in courses)
                {
                    creditHours += course.CreditHours;
                }

                if (invoices.Count() != 0)
                {
                    oldInvoice = invoices.LastOrDefault(Invoice => Invoice.ID == user.ID);
                    balance = oldInvoice.FullBalance - oldInvoice.AmountPaid;
                }
                else
                {

                    balance = creditHours * 100;
                }
                
            }
            // Otherwise, return the page
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Access the current session
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);

            // Make sure a user is logged in
            user = session.GetUser();

            if (user == null)
            {
                return RedirectToPage("Login");
            }

            // might create duplicate payment info, but different paymentID

            // Moved this up so I can pass in balance to UI
            invoices = session.GetInvoices();
            oldInvoice = invoices.LastOrDefault(Invoice => Invoice.ID == user.ID);

            // Moved this up so credit hours were posted correctly on UI 
            courses = session.GetCourses();

            foreach (Course course in courses)
            {
                creditHours += course.CreditHours;
            }

            // If user input is invalid, return page
            if(invoices.Count() != 0)
            {
                // why is old balance passed? 
                if (!validPayment(oldInvoice.FullBalance))
                {
                    return refreshPage();
                }
            }
            else
            {
                if (!validPayment(creditHours * 100))
                {
                    return refreshPage();
                }
            }


            Payment newPayment = new Payment();

            firstName = Request.Form["txtFirstName"];

            newPayment.FirstName = firstName;

            lastName = Request.Form["txtLastName"];

            newPayment.LastName = lastName;

            cardNumber = Request.Form["txtCardNumber"];

            newPayment.CardNumber = cardNumber;

            cvv = Request.Form["txtCvv"];

            newPayment.Cvv = cvv;

            newPayment.ID = user.ID;

            expDate = Convert.ToDateTime(Request.Form["txtExpDate"]);

            newPayment.ExpDate = expDate;

            Invoice newInvoice = new Invoice();

            amountPaid = Request.Form["txtAmount"];

            newInvoice.AmountPaid = Decimal.Parse(amountPaid);

            if (invoices.Count() != 0)
            {
                oldInvoice = invoices.LastOrDefault(Invoice => Invoice.ID == user.ID);
                newInvoice.FullBalance = oldInvoice.FullBalance - oldInvoice.AmountPaid;
            }
            else
            {
                // bug might be here
                //balance = creditHours * 100;
                //balance = creditHours * 100 - newInvoice.AmountPaid;
                newInvoice.FullBalance = creditHours * 100 - newInvoice.AmountPaid;
            }

            newInvoice.ID = user.ID;
            newInvoice.PaymentDate = System.DateTime.Today;



            // Code for api
            HttpClient client = new HttpClient();
            
            //string key = "Bearer sk_test_51Lk9RZAUFqfgks1NFzsod5WiLQApGnMFPV8MMdpd1QUY7n27UugEMxoyUk6mMAEnBDW6WYJVH0owdzs3S3jCiTNN005kOXfcj0";
            string url = "https://api.stripe.com/v1/tokens";

            // token
            client.BaseAddress = new Uri(url);

            client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", "sk_test_51Lk9RZAUFqfgks1NFzsod5WiLQApGnMFPV8MMdpd1QUY7n27UugEMxoyUk6mMAEnBDW6WYJVH0owdzs3S3jCiTNN005kOXfcj0");


            var cardContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("card[number]", cardNumber.ToString()),
                    new KeyValuePair<string, string>("card[exp_month]", newPayment.ExpDate.Month.ToString()),
                    new KeyValuePair<string, string>("card[exp_year]", newPayment.ExpDate.Year.ToString()),
                    new KeyValuePair<string, string>("card[cvc]", cvv),
                }
            );


            var response = await client.PostAsync(url, cardContent);
            var rRes = await response.Content.ReadAsStringAsync();
            
            var token = JObject.Parse(rRes)["id"];

            // payment

            url = "https://api.stripe.com/v1/charges";
            amountPaid = (Convert.ToInt32(amountPaid) * 100).ToString();


            var chargeContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("amount", amountPaid),
                    new KeyValuePair<string, string>("currency", "usd"),
                    new KeyValuePair<string, string>("source",token.ToString()),
                    new KeyValuePair<string, string>("description", "Tuition Payment"),
                }
            );

            response = await client.PostAsync(url, chargeContent);
            rRes = await response.Content.ReadAsStringAsync();

            var paymentReceipt = JObject.Parse(rRes)["receipt_url"];

            newInvoice.PaymentReceipt = paymentReceipt.ToString();

            invoiceRepository.Add(newInvoice);

            paymentRepository.Add(newPayment);

            // Change amount owed and credits displayed

            balance = newInvoice.FullBalance - newInvoice.AmountPaid;

            // Update invoices locally and in the session
            invoices = invoiceRepository.GetInvoices(user.ID);

            session.SetInvoices(invoices);

            return Page();

        } // End of On Post

        #region Valid Payment
        /// <summary>
        /// Verifies if values entered by user is valid
        /// </summary>
        /// <returns></returns>
        public bool validPayment(decimal balance)
        {
            // Length of credit card should be 16
            int CCNumLen = 16;

            // Length of CVV should be 3
            int CCVNumLen = 3;

            // Credit Card Number 
            string sCCN = Request.Form["txtCardNumber"];

            // CVV Number
            string sCVV = Request.Form["txtCvv"];

            // Amount user is paying
            string sAmount = Request.Form["txtAmount"];

            // User's CC expiration date
            DateTime exp = Convert.ToDateTime(Request.Form["txtExpDate"]);

            // Today's date
            DateTime today = DateTime.Today;

            // If user used an interger or decimal, save it to this var
            decimal dAmount;

            // User can't make payment if their balance is zero
            if (balance == 0)
            {
                return false;
            }

            // Checks if Credit card and CVV number have the correct # of digits
            if ((CCNumLen != sCCN.Length) || (CCVNumLen != sCVV.Length))
            {
                return false;
            }

            // Checks if all characters in CC & CVV # are digits
            if (!Regex.IsMatch(sCCN, @"^\d+$") || !Regex.IsMatch(sCVV, @"^\d+$"))
            {
                return false;
            }

            // Checks if user input a decimal or an integer
            if (!decimal.TryParse(sAmount, out dAmount) && !Regex.IsMatch(sAmount, @"^\d+$"))
            {
                return false;
            }
            else
            {
                // If its an integer, convert it to a decimal
                dAmount = Decimal.Parse(sAmount);

                // Bankers Rounding
                dAmount = decimal.Round(dAmount, 2, MidpointRounding.AwayFromZero);
            }

            // Makes sure user makes payments that is at least a penny
            if(dAmount < (decimal)0.01)
            {
                return false;
            }

            // Checks if payment amount is less than full balance
            if(dAmount > balance)
            {
                return false;
            }

            // Checks if if expiration date hasn't passed
            if (today > exp)
            {
                return false;
            }

            // Only return true if it passed all test cases
            return true;
        }
        #endregion

        /// <summary>
        /// Sets values for page refresh
        /// </summary>
        /// <returns></returns>
        public PageResult refreshPage()
        {
            // Access the current session
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);

            courses = session.GetCourses();
            invoices = session.GetInvoices();

            if (invoices.Count() != 0)
            {
                oldInvoice = invoices.LastOrDefault(Invoice => Invoice.ID == user.ID);
                balance = oldInvoice.FullBalance - oldInvoice.AmountPaid;
            }
            else
            {
                balance = creditHours * 100;
            }
            return Page();
        }
    } // End of class
}
