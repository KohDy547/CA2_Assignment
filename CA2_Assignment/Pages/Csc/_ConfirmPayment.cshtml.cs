using CA2_Assignment.Models.CscModels;
using CA2_Ultima.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;
using System.Security.Claims;

namespace CA2_Assignment.Pages.Csc
{
    public class _ConfirmPaymentModel : PageModel
    {
        [TempData]
        public string Message { get; set; }
        public bool ShowMessage => !string.IsNullOrEmpty(Message);

        private readonly IHttpContextAccessor _IHttpContextAccessor;
        private readonly ApplicationDbContext _ApplicationDbContext;
        public _ConfirmPaymentModel(
            IHttpContextAccessor IHttpContextAccessor,
            ApplicationDbContext ApplicationDbContext)
        {
            _IHttpContextAccessor = IHttpContextAccessor;
            _ApplicationDbContext = ApplicationDbContext;
        }

        public void OnGet()
        {

        }
        public IActionResult OnPost()
        {
            try
            {
                string currentUserId = _IHttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                if (_ApplicationDbContext.PremiumMembers.ToArray().Select(x => x.Id).ToArray().Contains(currentUserId))
                {
                    Message = "alert alert-warning|Plase don't hack our app...";
                    return RedirectToPage("Index");
                }

                _ApplicationDbContext.Add(new PremiumUser { Id = currentUserId });
                _ApplicationDbContext.SaveChanges();

                Message = "alert alert-success|Now uploading talents is available to you.";
                return RedirectToPage("Index");
            }
            catch(Exception e)
            {
                Message = "alert alert-danger|Failed to update your subscription.";
                return RedirectToPage("Index");
            }
        }
    }
}