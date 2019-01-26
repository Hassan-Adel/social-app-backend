using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialApp.API.Helpers
{
    //General perpose extensions class
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            //we'll add the CORS header so that the angular application doesn't compliain about it because it doesn't have the appropriate access control allow origin.
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        public static void AddPaginationHeaders(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);

            // return object names as camel cased
            var camelCaseFormatter = new JsonSerializerSettings();
            camelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();

            response.Headers.Add("Paginationr", JsonConvert.SerializeObject(paginationHeader, camelCaseFormatter)); // returns Json string

            //we'll add the CORS header so that the angular application doesn't compliain about it because it doesn't have the appropriate access control allow origin.
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }

        /*
         *  But this isn't going to give us an accurate age because depending on the time of year it is. and it depends
            if the users already had their birthday.
            Then it's only going to be right.
            Some of the time.
            So what we'll do is we'll check to see if theDateTime.AddYears(age)
            is greater than date DateTime.Today.
            And if that is the case then the user hasn't had their birthday yet this year.
            So it will take a year off their age that we're returning above.
            And then we'll return the age.
        */
        public static int CalculateAge(this DateTime theDateTime)
        {
            int age = DateTime.Today.Year - theDateTime.Year;
            if (theDateTime.AddYears(age) > DateTime.Today)
                age--;

            return age;
        }
    }
}
