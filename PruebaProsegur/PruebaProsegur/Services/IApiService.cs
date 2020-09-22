using PruebaProsegur.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PruebaProsegur.Services
{
    public interface IApiService
    {
        Task<Response> GetUserByEmailAsync(
            string urlBase,
            string servicePrefix,
            string controller,
            string tokenType,
            string accessToken,
            string email);

        Task<Response> GetTokenAsync(
            string urlBase,
            string servicePrefix,
            string controller,
            TokenRequest request);

        Task<bool> CheckConnection(string url);
    }
}
