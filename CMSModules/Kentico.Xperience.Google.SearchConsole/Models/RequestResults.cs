﻿using System.Collections.Generic;

namespace Kentico.Xperience.Google.SearchConsole.Models
{
    /// <summary>
    /// Represents the results of a batch request to the Google APIs.
    /// </summary>
    public class RequestResults
    {
        /// <summary>
        /// A list of error messages encountered during the request.
        /// </summary>
        public List<string> Errors
        {
            get;
            set;
        } = new List<string>();


        /// <summary>
        /// The total number of requests that were successful.
        /// </summary>
        public int SuccessfulRequests
        {
            get;
            set;
        }


        /// <summary>
        /// The total number of requests that failed.
        /// </summary>
        public int FailedRequests
        {
            get;
            set;
        }
    }
}