using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CA2_Ultima.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe;

namespace CA2_Assignment.Pages.CSC
{
    public class ChargeModel : PageModel
    {

        public void OnGet()
        {
    
        }

        public IActionResult Charge(string stripeEmail, string stripeToken)
        {
            var customers = new CustomerService();
            var charges = new ChargeService();

            var customer = customers.Create(new CustomerCreateOptions
            {
                Email = stripeEmail,
                SourceToken = stripeToken
            });

            var charge = charges.Create(new ChargeCreateOptions
            {
                Amount = 1000,
                Description = "Sample Charge",
                Currency = "usd",
                CustomerId = customer.Id
            });

            return RedirectToPage("Index");
        }
    }
}