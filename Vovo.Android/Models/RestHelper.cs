using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Net.Http;
using Vovo.Android.Models;

namespace Vovo.Android.Models
{
    public class RestService
    {
        public static string BaseAddress => "http://192.168.100.100";
        //public static string BaseAddress => "http://192.168.1.36";
    }
}