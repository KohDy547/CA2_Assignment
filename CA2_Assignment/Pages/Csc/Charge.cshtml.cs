using CA2_Assignment.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace CA2_Assignment.Pages.CSC
{
    public class ChargeModel : PageModel
    {
        [TempData]
        public string Message { get; set; }
        public bool ShowMessage => !string.IsNullOrEmpty(Message);

        public string StripeKey = "";
        public string ChargeAmount = "";
        public string ChargeAmountString = "";

        private readonly Csc_StripeSettings _Csc_StripeSettings;
        public ChargeModel(IOptions<Csc_StripeSettings> Csc_StripeSettings)
        {
            _Csc_StripeSettings = Csc_StripeSettings.Value;

            StripeKey = _Csc_StripeSettings.PublishableKey;
            ChargeAmount = _Csc_StripeSettings.PremiumSubscriptionCost;

            double ChargeAmountDouble = double.Parse(ChargeAmount) / 100;
            ChargeAmountString = "$" + ChargeAmountDouble;
        }

        public void OnGet()
        {
        }
        public IActionResult OnPost()
        {
            return RedirectToPage("_ConfirmPayment");
        }
    }
}