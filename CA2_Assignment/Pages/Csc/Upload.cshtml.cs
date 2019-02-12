using CA2_Assignment.Models;
using CA2_Assignment.Models.CscModels;
using CA2_Assignment.Repositories.CscRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Security.Claims;

namespace CA2_Assignment.Pages.Csc.Talents
{
    public class UploadModel : PageModel
    {
        [TempData]
        public string Message { get; set; }
        public bool ShowMessage => !string.IsNullOrEmpty(Message);

        public int LoaderFade = 800;
        public int ContentFade = 500;

        [BindProperty]
        public TalentView Talent { get; set; }

        private readonly IHttpContextAccessor _IHttpContextAccessor;
        private readonly ITalentsRepository _ITalentsRepository;
        public UploadModel(
            IHttpContextAccessor IHttpContextAccessor, 
            ITalentsRepository ITalentsRepository)
        {
            _IHttpContextAccessor = IHttpContextAccessor;
            _ITalentsRepository = ITalentsRepository;
        }

        public void OnGet()
        {
        }
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                string returnMessage = "alert alert-danger|Invalid input for ";
                foreach (ModelStateEntry modelState in ModelState.Values)
                {
                    foreach (ModelError modelError in modelState.Errors)
                    {
                        returnMessage += modelError.ErrorMessage + ", ";
                    }
                }
                Message = returnMessage.Remove(returnMessage.Length - 2) + ".";
                return RedirectToPage("Upload");
            }

            try
            {
                string currentUserId = _IHttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                string currentUserEmail = _IHttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                string currentUserName = currentUserEmail.Split("@")[0];
                Talent.UploadedByName = currentUserName;
                Talent.UploadedById = currentUserId;
                
                Response returnedResponse = _ITalentsRepository.PostAsync(Talent).Result;
                Message = (returnedResponse.HasError) ?
                    "alert alert-danger|Talent upload failed." :
                    "alert alert-success|Talent uploaded successfully.";
                return RedirectToPage("Index");
            }
            catch (Exception)
            {
                Message = "alert alert-danger|Talent upload failed.";
                return RedirectToPage("Upload");
            }
        }
    }
}