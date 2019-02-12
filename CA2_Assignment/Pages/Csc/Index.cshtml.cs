using CA2_Assignment.Configurations;
using CA2_Assignment.Models;
using CA2_Assignment.Models.CscModels;
using CA2_Assignment.Repositories.CscRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace CA2_Assignment.Pages.Csc.Talents
{
    public class IndexModel : PageModel
    {
        [TempData]
        public string SearchedQuery { get; set; }
        public bool Searching => !string.IsNullOrEmpty(SearchedQuery);

        [TempData]
        public string Message { get; set; }
        public bool ShowMessage => !string.IsNullOrEmpty(Message);

        public int LoaderFade = 800;
        public int ContentFade = 500;
        public string S3ImgBaseUrl;
        public Talent[] Talents = new Talent[0];

        private readonly ITalentsRepository _ITalentsRepository;
        private readonly Csc_AwsS3Settings _Csc_AwsS3Settings;
        public IndexModel(ITalentsRepository ITalentsRepository, IOptions<Csc_AwsS3Settings> Csc_AwsS3Settings)
        {
            _ITalentsRepository = ITalentsRepository;
            _Csc_AwsS3Settings = Csc_AwsS3Settings.Value;

            S3ImgBaseUrl = _Csc_AwsS3Settings.Talents_ImgBaseUrl;
        }

        public void OnGet()
        {
            try
            {
                Response returnedResponse = _ITalentsRepository.GetAllAsync().Result;
                if (returnedResponse.HasError) Message = "alert alert-danger|Failed to load talents.";
                Talents = (Talent[])returnedResponse.Payload;

                if (Searching)
                {
                    string query = SearchedQuery;
                    string queryLower = query.ToLower();
                    try
                    {
                        Talents = Talents.Where(x =>
                        x.Name.ToLower().Contains(queryLower) ||
                        x.Bio.ToLower().Contains(queryLower))
                        .ToArray();
                        if (Talents.Length == 0) Message = "alert alert-warning|'" + query + "' returned no result.";
                        else Message = "alert alert-success|'" + query + "' returned " + Talents.Length + " result" + 
                            ((Talents.Length > 1)? "s.": ".");
                    }
                    catch (Exception)
                    {
                        Message = "alert alert-danger|Search failed.";
                    }
                }
                else
                {
                    Random rnd = new Random();
                    Talents = Talents.OrderBy(x => rnd.Next()).ToArray();
                }
            }
            catch (Exception)
            {
                Message = "alert alert-danger|Failed to load talents.";
            }
        }
        public IActionResult OnPost()
        {
            string inputQuery = Request.Form["txt_searchTalent"];
            SearchedQuery = inputQuery;
            return RedirectToPage("Index");
        }
    }
}