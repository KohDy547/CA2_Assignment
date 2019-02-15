using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using CA2_Assignment.Configurations;
using CA2_Assignment.Models;
using CA2_Assignment.Models.CscModels;
using CA2_Assignment.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace CA2_Assignment.Repositories.CscRepositories
{
    public interface ITalentsRepository
    {
        Task<Response> Post(TalentView inputTalent);
        Task<Response> GetAll();
        Task<Response> Get(string inputId);
        Task<Response> Put(string inputId, Talent inputTalent);
        Task<Response> Delete(string inputId);

        Task<Response> PostAsync(TalentView inputTalent);
        Task<Response> GetAllAsync();
        Task<Response> GetAsync(string inputId);
        Task<Response> PutAsync(string inputId, Talent inputTalent);
        Task<Response> DeleteAsync(string inputId);
    }

    public class TalentsRepository : ITalentsRepository
    {
        private readonly IHostingEnvironment _IHostingEnvironment;
        private readonly App_SharedSettings _App_SharedSettings;
        private readonly Csc_AwsS3Settings _Csc_AwsS3Settings;
        private readonly IAwsService _IAwsService;
        private readonly AmazonS3Client _AmazonS3Client;

        public TalentsRepository(
            IHostingEnvironment IHostingEnvironment,
            IOptions<App_SharedSettings> App_SharedSettings,
            IOptions<Csc_AwsS3Settings> Csc_AwsS3Settings,
            IAwsService IAwsService)
        {
            _IHostingEnvironment = IHostingEnvironment;
            _App_SharedSettings = App_SharedSettings.Value;
            _Csc_AwsS3Settings = Csc_AwsS3Settings.Value;
            _IAwsService = IAwsService;

            string awsCredentialsPath = _IHostingEnvironment.ContentRootPath + _App_SharedSettings.Aws_CredentialsPath;
            _AmazonS3Client = new AmazonS3Client(
                new StoredProfileAWSCredentials(
                    _Csc_AwsS3Settings.Profile,
                    awsCredentialsPath),
                RegionEndpoint.GetBySystemName(_Csc_AwsS3Settings.Region));
        }

        public Task<Response> Post(TalentView inputTalent)
        {
            try
            {
                Response returnedResponse;

                returnedResponse = GetAll().Result;
                if (returnedResponse.HasError) return Task.FromResult<Response>(returnedResponse);

                Talent talentInfo = inputTalent.GetBase();
                talentInfo.Id = Guid.NewGuid().ToString();
                talentInfo.ShortName = talentInfo.Name.Replace(" ", string.Empty);
                
                Talent[] talentsAr = (Talent[])returnedResponse.Payload;
                List<Talent> talentsList = talentsAr.ToList();
                talentsList.Add(talentInfo);
                string newTalents = JsonConvert.SerializeObject(talentsList.ToArray(), Formatting.Indented);

                returnedResponse = _IAwsService.awsS3_UploadTextAsync(
                    _AmazonS3Client,
                    _Csc_AwsS3Settings.Bucket,
                    _Csc_AwsS3Settings.Talents_FileKey,
                    newTalents).Result;
                if (returnedResponse.HasError) return Task.FromResult<Response>(returnedResponse);

                IFormFile photo = inputTalent.Photo;
                using (Stream stream = photo.OpenReadStream())
                {
                    returnedResponse = _IAwsService.awsS3_UploadStreamAsync(
                        _AmazonS3Client,
                        _Csc_AwsS3Settings.Bucket,
                        "Talent_Photos/" + talentInfo.Id + ".jpg",
                        stream,
                        true).Result;
                }
                if (returnedResponse.HasError) return Task.FromResult<Response>(returnedResponse);

                return Task.FromResult<Response>(new Response
                {
                    HttpStatus = HttpStatusCode.OK
                });
            }
            catch (Exception e)
            {
                return Task.FromResult<Response>(new Response
                {
                    HttpStatus = HttpStatusCode.InternalServerError,
                    ExceptionPayload = e
                });
            }
        }
        public Task<Response> GetAll()
        {
            try
            {
                Response returnedResponse = _IAwsService.awsS3_ReadFileAsync(
                    _AmazonS3Client,
                    _Csc_AwsS3Settings.Bucket,
                    _Csc_AwsS3Settings.Talents_FileKey
                    ).Result;
                if (returnedResponse.HasError)
                {
                    return Task.FromResult<Response>(returnedResponse);
                }

                string returnedPayload = (string)returnedResponse.Payload;
                Talent[] responseContent = JsonConvert.DeserializeObject<Talent[]>(returnedPayload);

                return Task.FromResult<Response>(new Response
                {
                    HttpStatus = HttpStatusCode.OK,
                    Payload = (Talent[])responseContent
                });
            }
            catch (Exception e)
            {
                return Task.FromResult<Response>(new Response
                {
                    HttpStatus = HttpStatusCode.InternalServerError,
                    ExceptionPayload = e
                });
            }
        }
        public Task<Response> Get(string inputId)
        {
            try
            {
                Response returnedResponse;

                returnedResponse = GetAll().Result;
                if (returnedResponse.HasError) return Task.FromResult<Response>(returnedResponse);

                Talent[] returnedPayload = (Talent[])returnedResponse.Payload;
                Talent responseContent = returnedPayload.First(t => t.Id == inputId);

                return Task.FromResult<Response>(new Response
                {
                    HttpStatus = HttpStatusCode.OK,
                    Payload = (Talent)responseContent
                });
            }
            catch (Exception e)
            {
                return Task.FromResult<Response>(new Response
                {
                    HttpStatus = HttpStatusCode.InternalServerError,
                    ExceptionPayload = e
                });
            }
        }
        public Task<Response> Put(string inputId, Talent inputTalent)
        {
            try
            {
                Response returnedResponse;

                returnedResponse = GetAll().Result;
                if (returnedResponse.HasError) return Task.FromResult<Response>(returnedResponse);

                PropertyInfo[] targetProp = typeof(Talent)
                .GetProperties()
                .Where(
                    p => p.GetValue(inputTalent, null) != null
                )
                .ToArray();

                Talent[] talents = (Talent[])returnedResponse.Payload;
                Talent[] newTalentsRaw = talents
                    .Where(x => x.Id == inputId)
                    .Select(x =>
                    {
                        foreach (PropertyInfo property in targetProp)
                        {
                            property.SetValue(x, property.GetValue(inputTalent, null));
                        }
                        return x;
                    })
                    .ToArray();
                string newTalents = JsonConvert.SerializeObject(newTalentsRaw, Formatting.Indented);

                returnedResponse = _IAwsService.awsS3_UploadTextAsync(
                    _AmazonS3Client,
                    _Csc_AwsS3Settings.Bucket,
                    _Csc_AwsS3Settings.Talents_FileKey,
                    newTalents).Result;
                if (returnedResponse.HasError) return Task.FromResult<Response>(returnedResponse);

                return Task.FromResult<Response>(new Response
                {
                    HttpStatus = HttpStatusCode.OK
                });
            }
            catch (Exception e)
            {
                return Task.FromResult<Response>(new Response
                {
                    HttpStatus = HttpStatusCode.InternalServerError,
                    ExceptionPayload = e
                });
            }
        }
        public Task<Response> Delete(string inputId)
        {
            try
            {
                Response returnedResponse;

                returnedResponse = GetAll().Result;
                if (returnedResponse.HasError) return Task.FromResult<Response>(returnedResponse);

                Talent[] returnedPayload = (Talent[])returnedResponse.Payload;
                Talent[] newTalentsRaw = returnedPayload.Where(t => t.Id != inputId).ToArray();
                string newTalents = JsonConvert.SerializeObject(newTalentsRaw, Formatting.Indented);

                returnedResponse = _IAwsService.awsS3_UploadTextAsync(
                    _AmazonS3Client,
                    _Csc_AwsS3Settings.Bucket,
                    _Csc_AwsS3Settings.Talents_FileKey,
                    newTalents).Result;
                if (returnedResponse.HasError) return Task.FromResult<Response>(returnedResponse);

                returnedResponse = _IAwsService.awsS3_DeleteFileAsync(
                    _AmazonS3Client,
                    _Csc_AwsS3Settings.Bucket,
                    "Talent_Photos/" + _Csc_AwsS3Settings.Talents_FileKey).Result;
                if (returnedResponse.HasError) return Task.FromResult<Response>(returnedResponse);

                return Task.FromResult<Response>(new Response
                {
                    HttpStatus = HttpStatusCode.OK
                });
            }
            catch (Exception e)
            {
                return Task.FromResult<Response>(new Response
                {
                    HttpStatus = HttpStatusCode.InternalServerError,
                    ExceptionPayload = e
                });
            }
        }

        public async Task<Response> PostAsync(TalentView inputTalent)
        {
            try
            {
                Response returnedResponse;

                returnedResponse = GetAll().Result;
                if (returnedResponse.HasError) return await Task.FromResult<Response>(returnedResponse);

                Talent talentInfo = inputTalent.GetBase();
                talentInfo.Id = Guid.NewGuid().ToString();
                talentInfo.ShortName = talentInfo.Name.Replace(" ", string.Empty);

                Talent[] talentsAr = (Talent[])returnedResponse.Payload;
                List<Talent> talentsList = talentsAr.ToList();
                talentsList.Add(talentInfo);
                string newTalents = JsonConvert.SerializeObject(talentsList.ToArray(), Formatting.Indented);

                returnedResponse = _IAwsService.awsS3_UploadTextAsync(
                    _AmazonS3Client,
                    _Csc_AwsS3Settings.Bucket,
                    _Csc_AwsS3Settings.Talents_FileKey,
                    newTalents).Result;
                if (returnedResponse.HasError) return await Task.FromResult<Response>(returnedResponse);

                IFormFile photo = inputTalent.Photo;
                using (Stream stream = photo.OpenReadStream())
                {
                    returnedResponse = _IAwsService.awsS3_UploadStreamAsync(
                        _AmazonS3Client,
                        _Csc_AwsS3Settings.Bucket,
                        "Talent_Photos/" + talentInfo.Id + ".jpg",
                        stream,
                        true).Result;
                }
                if (returnedResponse.HasError) return await Task.FromResult<Response>(returnedResponse);

                return await Task.FromResult<Response>(new Response
                {
                    HttpStatus = HttpStatusCode.OK
                });
            }
            catch (Exception e)
            {
                return await Task.FromResult<Response>(new Response
                {
                    HttpStatus = HttpStatusCode.InternalServerError,
                    ExceptionPayload = e
                });
            }
        }
        public async Task<Response> GetAllAsync()
        {
            try
            {
                Response returnedResponse = _IAwsService.awsS3_ReadFileAsync(
                    _AmazonS3Client,
                    _Csc_AwsS3Settings.Bucket,
                    _Csc_AwsS3Settings.Talents_FileKey
                    ).Result;
                if (returnedResponse.HasError)
                {
                    return await Task.FromResult<Response>(returnedResponse);
                }

                string returnedPayload = (string)returnedResponse.Payload;
                Talent[] responseContent = JsonConvert.DeserializeObject<Talent[]>(returnedPayload);

                return await Task.FromResult<Response>(new Response
                {
                    HttpStatus = HttpStatusCode.OK,
                    Payload = (Talent[])responseContent
                });
            }
            catch (Exception e)
            {
                return await Task.FromResult<Response>(new Response
                {
                    HttpStatus = HttpStatusCode.InternalServerError,
                    ExceptionPayload = e
                });
            }
        }
        public async Task<Response> GetAsync(string inputId)
        {
            try
            {
                Response returnedResponse;

                returnedResponse = GetAll().Result;
                if (returnedResponse.HasError) return await Task.FromResult<Response>(returnedResponse);

                Talent[] returnedPayload = (Talent[])returnedResponse.Payload;
                Talent responseContent = returnedPayload.First(t => t.Id == inputId);

                return await Task.FromResult<Response>(new Response
                {
                    HttpStatus = HttpStatusCode.OK,
                    Payload = (Talent)responseContent
                });
            }
            catch (Exception e)
            {
                return await Task.FromResult<Response>(new Response
                {
                    HttpStatus = HttpStatusCode.InternalServerError,
                    ExceptionPayload = e
                });
            }
        }
        public async Task<Response> PutAsync(string inputId, Talent inputTalent)
        {
            try
            {
                Response returnedResponse;

                returnedResponse = GetAll().Result;
                if (returnedResponse.HasError) return await Task.FromResult<Response>(returnedResponse);

                PropertyInfo[] targetProp = typeof(Talent)
                .GetProperties()
                .Where(
                    p => p.GetValue(inputTalent, null) != null
                )
                .ToArray();

                Talent[] talents = (Talent[])returnedResponse.Payload;
                Talent[] newTalentsRaw = talents
                    .Where(x => x.Id == inputId)
                    .Select(x =>
                    {
                        foreach (PropertyInfo property in targetProp)
                        {
                            property.SetValue(x, property.GetValue(inputTalent, null));
                        }
                        return x;
                    })
                    .ToArray();
                string newTalents = JsonConvert.SerializeObject(newTalentsRaw, Formatting.Indented);

                returnedResponse = _IAwsService.awsS3_UploadTextAsync(
                    _AmazonS3Client,
                    _Csc_AwsS3Settings.Bucket,
                    _Csc_AwsS3Settings.Talents_FileKey,
                    newTalents).Result;
                if (returnedResponse.HasError) return await Task.FromResult<Response>(returnedResponse);

                return await Task.FromResult<Response>(new Response
                {
                    HttpStatus = HttpStatusCode.OK
                });
            }
            catch (Exception e)
            {
                return await Task.FromResult<Response>(new Response
                {
                    HttpStatus = HttpStatusCode.InternalServerError,
                    ExceptionPayload = e
                });
            }
        }
        public async Task<Response> DeleteAsync(string inputId)
        {
            try
            {
                Response returnedResponse;

                returnedResponse = GetAll().Result;
                if (returnedResponse.HasError) return await Task.FromResult<Response>(returnedResponse);

                Talent[] returnedPayload = (Talent[])returnedResponse.Payload;
                Talent[] newTalentsRaw = returnedPayload.Where(t => t.Id != inputId).ToArray();
                string newTalents = JsonConvert.SerializeObject(newTalentsRaw, Formatting.Indented);

                returnedResponse = _IAwsService.awsS3_UploadTextAsync(
                    _AmazonS3Client,
                    _Csc_AwsS3Settings.Bucket,
                    _Csc_AwsS3Settings.Talents_FileKey,
                    newTalents).Result;
                if (returnedResponse.HasError) return await Task.FromResult<Response>(returnedResponse);

                returnedResponse = _IAwsService.awsS3_DeleteFileAsync(
                    _AmazonS3Client,
                    _Csc_AwsS3Settings.Bucket,
                    "Talent_Photos/" + inputId + ".jpg").Result;
                if (returnedResponse.HasError) return await Task.FromResult<Response>(returnedResponse);

                return await Task.FromResult<Response>(new Response
                {
                    HttpStatus = HttpStatusCode.OK
                });
            }
            catch (Exception e)
            {
                return await Task.FromResult<Response>(new Response
                {
                    HttpStatus = HttpStatusCode.InternalServerError,
                    ExceptionPayload = e
                });
            }
        }
    }
}
