using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Nova.DeptServiceAssesment.Domain.ExternalModel;
using Nova.DeptServiceAssesment.Domain.Repositories;
using Nova.DeptServiceAssesment.SpaWebApp.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Helper
{
    public class UserOrgHelper : IUserOrgHelper
    {
        private readonly HttpClient _httpClient;
        private readonly AppSettings _appSettings;
        private readonly IMemoryCache _cache;
        

        private Uri BaseEndpoint;
        


        public UserOrgHelper(HttpClient httpClient, AppSettings appSettings, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _appSettings = appSettings;
            _cache = cache;
            
            BaseEndpoint = new Uri(_appSettings.OrgConfig.Api);
        }

        private Uri CreateRequestUri(string relativePath, string queryString = "")
        {
            var endpoint = new Uri(BaseEndpoint, relativePath);
            var uriBuilder = new UriBuilder(endpoint);
            uriBuilder.Query = queryString;
            return uriBuilder.Uri;

        }
        private void addHeaders()
        {
            _httpClient.DefaultRequestHeaders.Remove("userIP");
            _httpClient.DefaultRequestHeaders.Add("userIP", "192.168.1.1");
        }

        private async Task<T> GetAsyncNo<T>(Uri requestUrl)
        {
            addHeaders();
            var response = await _httpClient.GetAsync(requestUrl, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            var data2 = JsonConvert.DeserializeObject<string>(data);
            // var data1 = JsonConvert.DeserializeObject<List<OrgUserModel>>(data2);
            return JsonConvert.DeserializeObject<T>(data2);
        }

        public string FormatDepartmentCode(string unFormatedCode)
        {
            if (unFormatedCode.IndexOf("(") != -1 || unFormatedCode.IndexOf(")") != -1)
            {
                int start = unFormatedCode.LastIndexOf('(');
                int end = unFormatedCode.LastIndexOf(')');
                unFormatedCode = unFormatedCode.Substring(start, end - start + 1).Replace("(", "").Replace(")", "").Replace(" - ", "-");
            }

            return unFormatedCode;
        }

        public List<OrgDepartmentModel> GetDepartment()
        {
            var cacheEntry = _cache.GetOrCreate("Nova.Department.datacache", entry =>
            {
                var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                _appSettings.OrgConfig.ApiMethod), "code=" + _appSettings.OrgConfig.ApiCode + "&type=" + _appSettings.OrgConfig.TypePB + "");

                var task = GetAsyncNo<List<OrgDepartmentModel>>(requestUrl);
                task.Wait();

                var departments = task.Result;
                if (departments != null)
                {
                    departments.ForEach(dep =>
                    {
                        if(dep.Code.IndexOf("(") != -1 || dep.Code.IndexOf(")") != -1)
                        {
                            int start = dep.Code.LastIndexOf('(');
                            int end = dep.Code.LastIndexOf(')');
                            dep.ShortCode = dep.Code.Substring(start, end - start + 1).Replace("(","").Replace(")", "").Replace(" - ", "-");
                        }
                        else
                        {
                            dep.ShortCode = dep.Code;
                        }
                    });
                }
                
                return departments;
            });

            if(cacheEntry!=null)
                return cacheEntry;
            else
            {
                _cache.Remove("Nova.Department.datacache");
                return new List<OrgDepartmentModel>();
            }
        }

        public async Task<List<OrgDepartmentModel>> GetDepartmentAsync()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                _appSettings.OrgConfig.ApiMethod), "code=" + _appSettings.OrgConfig.ApiCode + "&type=" + _appSettings.OrgConfig.TypePB + "");

            var departments =  await GetAsyncNo<List<OrgDepartmentModel>>(requestUrl);
            
            if (departments != null)
            {
                departments.ForEach(dep =>
                {
                    if (dep.Code.IndexOf("(") != -1 || dep.Code.IndexOf(")") != -1)
                    {
                        int start = dep.Code.LastIndexOf('(');
                        int end = dep.Code.LastIndexOf(')');
                        dep.ShortCode = dep.Code.Substring(start, end - start + 1).Replace("(", "").Replace(")", "").Replace(" - ", "-");
                    }
                    else
                    {
                        dep.ShortCode = dep.Code;
                    }
                });
            }

            return departments;
        }

        public List<OrgUserModel> GetUsers()
        {
            var cacheEntry = _cache.GetOrCreate("Nova.UserORG.datacache", entry =>
            {

                var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                    _appSettings.OrgConfig.ApiMethod), "code=" + _appSettings.OrgConfig.ApiCode + "&type=" + _appSettings.OrgConfig.TypeNV + "");

                var task = GetAsyncNo<List<OrgUserModel>>(requestUrl);
                task.Wait();

                return task.Result;
            });
            return cacheEntry;
        }

        public async Task<List<OrgUserModel>> GetUsersAsync()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                    _appSettings.OrgConfig.ApiMethod), "code=" + _appSettings.OrgConfig.ApiCode + "&type=" + _appSettings.OrgConfig.TypeNV + "");

            return await GetAsyncNo<List<OrgUserModel>>(requestUrl);
        }
    }
}
