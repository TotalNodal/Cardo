﻿using Microsoft.AspNetCore.SignalR;

namespace Cardo.Web.Utility
{
    public class SD
    {
        public static string CouponAPIBase { get; set; }
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }
        
    }
}
