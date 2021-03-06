﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace LanguageFeatures.Models
{
    public class MyAsyncMethods
    {
        //public static Task<long?> GetPageLength()
        //{
        //    HttpClient client = new HttpClient();

        //    var httpTask = client.GetAsync("http://apress.com");

        //    // HTTP 요청이 완료되기를 기다리는 동안
        //    // 여기에서 다른 작업들을 처리할 수 있다.

        //    return httpTask.ContinueWith((Task<HttpResponseMessage> antecedent) =>
        //    {
        //        return antecedent.Result.Content.Headers.ContentLength;
        //    });
        //}

        public async static Task<long?> GetPageLength()
        {
            HttpClient client = new HttpClient();

            var httpMessage = await client.GetAsync("http://apress.com");

            // HTTP 요청이 완료되기를 기다리는 동안
            // 여기에서 다른 작업들을 처리할 수 있다.
            return httpMessage.Content.Headers.ContentLength;
        }
    }
}