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
using System.Security.Cryptography;

namespace Vovo.Android.Models
{
    public class Cryptography
    {
        public static string ProtectPassword(string password)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] entropy = Encoding.UTF8.GetBytes("Pa$$key");
            byte[] protectedPassword = ProtectedData.Protect(bytes, entropy, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(protectedPassword);
        }

        public static string UnprotectPassword(string protectedPassword)
        {
            byte[] bytes = Convert.FromBase64String(protectedPassword);
            byte[] entropy = Encoding.UTF8.GetBytes("Pa$$key");
            byte[] password = ProtectedData.Unprotect(bytes, entropy, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(password);
        }
    }
}