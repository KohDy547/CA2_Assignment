using CA2_Assignment.Configurations;
using CA2_Assignment.Models;
using CA2_Assignment.Models.CscModels;
using CA2_Assignment.Repositories.CscRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;

namespace CA2_Assignment.Pages.Csc.Talents
{
    public class TalentModel : PageModel
    {
        [TempData]
        public string Message { get; set; }
        public bool ShowMessage => !string.IsNullOrEmpty(Message);

        public int LoaderFade = 800;
        public int ContentFade = 500;
        public string S3ImgBaseUrl;
        public Talent Talent = new Talent();
        public string TalentId = "";
        public bool IsOwner = false;

        private readonly IHttpContextAccessor _IHttpContextAccessor;
        private readonly ITalentsRepository _ITalentsRepository;
        private readonly Csc_AwsS3Settings _Csc_AwsS3Settings;
        public TalentModel(
            IHttpContextAccessor IHttpContextAccessor, 
            ITalentsRepository ITalentsRepository, 
            IOptions<Csc_AwsS3Settings> Csc_AwsS3Settings)
        {
            _IHttpContextAccessor = IHttpContextAccessor;
            _ITalentsRepository = ITalentsRepository;
            _Csc_AwsS3Settings = Csc_AwsS3Settings.Value;

            S3ImgBaseUrl = _Csc_AwsS3Settings.Talents_ImgBaseUrl;
        }

        public void OnGet(string targetId)
        {
            try
            {
                Response returnedResponse = _ITalentsRepository.GetAsync((string)targetId).Result;
                if (returnedResponse.HasError) Message = "alert alert-danger|Failed to load talent.";

                Talent = (Talent)returnedResponse.Payload;
                TalentId = targetId;

                string currentUserId = _IHttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                if (Talent.UploadedById == currentUserId) IsOwner = true;
            }
            catch (Exception)
            {
                Message = "alert alert-danger|Failed to load talent.";
            }
        }

        public IActionResult OnDelete()
        {
            try
            {
                Response returnedResponse = _ITalentsRepository.DeleteAsync(TalentId).Result;
                if (returnedResponse.HasError)
                {
                    Message = "alert alert-danger|Failed to delete talent.";
                    return RedirectToPage("Upload/" + TalentId);
                }


                Message = "alert alert-success|Talent successfully deleted.";
                return RedirectToPage("Index");
            }
            catch (Exception)
            {
                Message = "alert alert-danger|Failed to delete talent.";
                return RedirectToPage("Upload/" + TalentId);
            }
        }
    }
}