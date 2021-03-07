using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using TenmoClient.Data;

namespace TenmoClient
{
    public class BankingService : ILoginHandler
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly IRestClient client = new RestClient();

        public API_Account GetCurrentAccount()
        {
            RestRequest request = new RestRequest(API_BASE_URL + "account");
            IRestResponse<API_Account> response = client.Get<API_Account>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
                return null;
            }
            else if (!response.IsSuccessful)
            {
                Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode);
                return null;
            }

            return response.Data;
        }

        public List<UserInfo> GetAllUsers()
        {
            RestRequest request = new RestRequest(API_BASE_URL + "users");
            IRestResponse<List<UserInfo>> response = client.Get<List<UserInfo>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
                return null;
            }
            else if (!response.IsSuccessful)
            {
                Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode);
                return null;
            }

            return response.Data;
        }

        public API_Transfer SendTransfer(API_Transfer transfer, string endpoint)
        {
            RestRequest request = new RestRequest(API_BASE_URL + $"transfers/{endpoint}");
            request.AddJsonBody(transfer);

            IRestResponse<API_Transfer> response = client.Post<API_Transfer>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
                return null;
            }
            else if (!response.IsSuccessful)
            {
                Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode);
                return null;
            }

            return response.Data;
        }

        public List<API_Transfer> GetTransfers()
        {
            RestRequest request = new RestRequest(API_BASE_URL + "transfers");
            IRestResponse<List<API_Transfer>> response = client.Get<List<API_Transfer>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
                return null;
            }
            else if (!response.IsSuccessful)
            {
                Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode);
                return null;
            }

            return response.Data;
        }

        public API_Transfer GetTransferById(int transferId)
        {
            RestRequest request = new RestRequest(API_BASE_URL + $"transfers/{transferId}");
            IRestResponse<API_Transfer> response = client.Get<API_Transfer>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
                return null;
            }
            else if (!response.IsSuccessful)
            {
                Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode);
                return null;
            }

            return response.Data;
        }

        public void Login(API_User user)
        {
            client.Authenticator = new JwtAuthenticator(user.Token);
        }

        public void Logout()
        {
            client.Authenticator = null;
        }
    }
}
