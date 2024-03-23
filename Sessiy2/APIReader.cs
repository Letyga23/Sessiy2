
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Globalization;
using Sessiy2;

//нужна библиотека Newtonsoft.Json, System.Net.Http.Json

namespace WebApplicationHospital
{
    internal class APIReader
    {
        private static readonly string url = "http://127.0.0.1:8082/";
        private static readonly HttpClient client = SettingHttpClient();

        public static HttpClient SettingHttpClient()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            HttpClient client = new HttpClient(handler);
            return client;
        }

        public static async Task<PersonMovement> getPersonMovement(int id)
        {
            if (!await canConnectToAPI())
                return null;

            PersonMovement personMovement = null;
            HttpResponseMessage response = await client.GetAsync(url + $"PersonMovement/{id}");
            if (response.IsSuccessStatusCode)
            {
                personMovement = await response.Content.ReadFromJsonAsync<PersonMovement>();
            }
            return personMovement;
        }

        public static async Task<List<PersonMovement>> getPersonMovements()
        {
            if (!await canConnectToAPI())
                return null;

            List<PersonMovement> personMovements = null;
            HttpResponseMessage response = await client.GetAsync(url + "PersonLocation");
            if (response.IsSuccessStatusCode)
            {
                personMovements = await response.Content.ReadFromJsonAsync<List<PersonMovement>>();
            }
            return personMovements;
        }

        public static async Task<bool> canConnectToAPI()
        {
            try
            {
                await client.GetAsync(url);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //public static async Task<bool> updatePersonMovement(PersonMovement PersonMovement)
        //{
        //    if (!await canConnectToAPI())
        //        return false;

        //    try
        //    {
        //        HttpResponseMessage response = await client.PutAsJsonAsync(url + $"/PersonMovement/Put/{PersonMovement.Id_PersonMovement}", PersonMovement);
        //        return response.IsSuccessStatusCode;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}

        //public static async Task<bool> addPersonMovement(PersonMovement PersonMovement)
        //{
        //    if (!await canConnectToAPI())
        //        return false;

        //    try
        //    {
        //        HttpResponseMessage response = await client.PostAsJsonAsync(url + $"/PersonMovement/Post", PersonMovement);
        //        return response.IsSuccessStatusCode;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}
    }
}
