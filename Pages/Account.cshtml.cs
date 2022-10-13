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
        public string amountPaid { get; set; }

        [BindProperty]
        public Invoice newInvoice { get; set; }

        [BindProperty]
        public string errorMessage { get; set; }
        #endregion

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

            // might create duplicate payment info, but different paymentID

            // Moved this up so I can pass in balance to UI
            InvoiceList = invoiceRepository.GetInvoices(User.ID);

            // Moved this up so credit hours were posted correctly on UI 
            UserCourses = courseRepository.GetStudentCourses(User.ID);

            foreach (Course course in UserCourses)
            {
                creditHours += course.CreditHours;
            }

            // If user input is invalid, return page
            if(InvoiceList.Count != 0)
            {
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

            User = userRepository.GetUser(User.ID);

            newPayment.ID = User.ID;

            Invoice newInvoice = new Invoice();

            amountPaid = Request.Form["txtAmount"];

            newInvoice.AmountPaid = Decimal.Parse(amountPaid);

            if (InvoiceList.Count != 0)
            {
                oldInvoice = InvoiceList.LastOrDefault(Invoice => Invoice.ID == User.ID);
                newInvoice.FullBalance = oldInvoice.FullBalance - oldInvoice.AmountPaid;
            }
            else
            {
                balance = creditHours * 100;
                newInvoice.FullBalance = balance;
            }

            newInvoice.ID = User.ID;
            newInvoice.PaymentDate = System.DateTime.Today;

            invoiceRepository.Add(newInvoice);

            // Code for api
            HttpClient client = new HttpClient();
            string key = "sk_test_51Lk9RZAUFqfgks1NFzsod5WiLQApGnMFPV8MMdpd1QUY7n27UugEMxoyUk6mMAEnBDW6WYJVH0owdzs3S3jCiTNN005kOXfcj0";
            string url = "https://api.stripe.com/v1/tokens/";

            // token
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Add("key", key);
            client.DefaultRequestHeaders.Add("request type", "post"); //literally what the fuck
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url); //a guess at best

            var cardContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("card[number]", cardNumber),
                    new KeyValuePair<string, string>("card[exp_month]", newPayment.ExpDate.Month.ToString()),
                    new KeyValuePair<string, string>("card[exp_year]", newPayment.ExpDate.Year.ToString()),
                    new KeyValuePair<string, string>("Cvc", cvv),
                }
            );
            request.Content = cardContent;

            client.BaseAddress = new Uri(url);

            var response = await client.PostAsync(client.BaseAddress, cardContent);

            // payment

            url = "https://api.stripe.com/v1/charge/";
            client.BaseAddress = new Uri(url);

            var chargeContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("Amount", amountPaid),
                    new KeyValuePair<string, string>("Currency", "usd"),
                    new KeyValuePair<string, string>("Souce", response.ToString()),
                    new KeyValuePair<string, string>("Description", "Tuition Payment"),
                }
            );
            url = "stripe.com/v1/charges";
            client.PostAsync(client.BaseAddress, chargeContent);

            // client.PostAsync(get request); token request

            // client.PostAsync(get request); payment request

            paymentRepository.Add(newPayment);

            // Change amount owed and credits displayed

            balance = newInvoice.FullBalance - newInvoice.AmountPaid;
            InvoiceList = invoiceRepository.GetInvoices(User.ID);
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

            // If user used an interger or decimal, save it to this var
            decimal dAmount;

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
            UserCourses = courseRepository.GetStudentCourses(User.ID);
            InvoiceList = invoiceRepository.GetInvoices(User.ID);

            if (InvoiceList.Count != 0)
            {
                oldInvoice = InvoiceList.LastOrDefault(Invoice => Invoice.ID == User.ID);
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
