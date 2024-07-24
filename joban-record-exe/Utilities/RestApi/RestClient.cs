using joban_record_exe.Models.GameDatas.Entity;
using joban_record_exe.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace joban_record_exe.Utilities.RestApi
{
    internal class RestClient
    {
        public static class Post
        {
            public static async Task RequestAsync<T>(string apiUrl, T model)
            {
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        HttpResponseMessage response = null;

                        string responseBody = null;

                        StringContent content = RestClient.modelToStringContent(model);

                        response = await client.PostAsync(apiUrl, content);
                        responseBody = await response.Content.ReadAsStringAsync();

                        string result = $"MethodType = {response.RequestMessage.Method}, StstusCode = {response.StatusCode}, StatusDesc = {response.ReasonPhrase}";

                        if (response.IsSuccessStatusCode)
                        {
                            string responseData = await response.Content.ReadAsStringAsync();

                            Console.WriteLine($"API 요청 성공. 상태 코드: {response.StatusCode}");
                            Console.WriteLine(responseBody);
                        }
                        else
                        {
                            Console.WriteLine($"API 요청 실패. 상태 코드: {response.StatusCode}");
                        }

                        Console.WriteLine(result);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            public static class Patch
            {
                public static async Task RequestAsync<T>(string apiUrl, T model)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        try
                        {
                            HttpResponseMessage response = null;

                            string responseBody = null;

                            StringContent content = RestClient.modelToStringContent(model);

                            response = await HttpClientExtensions.PatchAsync(client, apiUrl, content);
                            responseBody = await response.Content.ReadAsStringAsync();

                            string result = $"MethodType = {response.RequestMessage.Method}, StstusCode = {response.StatusCode}, StatusDesc = {response.ReasonPhrase}";

                            if (response.IsSuccessStatusCode)
                            {
                                string responseData = await response.Content.ReadAsStringAsync();

                                Console.WriteLine($"API 요청 성공. 상태 코드: {response.StatusCode}");
                                Console.WriteLine(responseBody);
                            }
                            else
                            {
                                Console.WriteLine($"API 요청 실패. 상태 코드: {response.StatusCode}");
                            }

                            Console.WriteLine(result);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }

              
            }


        }

        private static StringContent modelToStringContent<T>(T model)
        {
            string content = JsonSerializer.Serialize<T>(model);
            return new StringContent(content, System.Text.Encoding.UTF8, "application/json");
        }
    }
}
