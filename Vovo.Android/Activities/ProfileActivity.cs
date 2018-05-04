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
using Android.Support.V7.App;
using SupportMain = Android.App;

using SupportFragment = Android.Support.V7.Widget;
using Android.Util;
using Android.Graphics.Drawables;
using Android.Graphics;
using AndroidNet = Android.Net;
using Newtonsoft.Json;
using System.Net.Http;
using Vovo.Models;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Vovo.Android.Models;
using Newtonsoft.Json.Bson;
using System.Threading.Tasks;

namespace Vovo.Android.Activities
{
    [Activity(Label = "Profile", Theme = "@style/Theme.AppTheme")]
    public class ProfileActivity : AppCompatActivity
    {
        private int pickerID = 1000;
        private ImageView profileImage;
        private HttpClient client;
        private User activeUser;
        private SupportMain.AlertDialog alertDialog;
        private Bitmap bitMapImage;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.AddFlags(global::Android.Views.WindowManagerFlags.DrawsSystemBarBackgrounds);
            activeUser = JsonConvert.DeserializeObject<User>(Intent.GetStringExtra("User"));
            client = new HttpClient { Timeout = new TimeSpan(0, 0, 30) };

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.profile);

            var toolbar = FindViewById<SupportFragment.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_arrow_back_white_24dp);

            profileImage = FindViewById<ImageView>(Resource.Id.profile_image);

            FindViewById<EditText>(Resource.Id.fullName).Text = activeUser.FullName;
            FindViewById<EditText>(Resource.Id.phoneText).Text = activeUser.PhoneNumber ?? "0800000000";
            FindViewById<TextView>(Resource.Id.emailAddress).Text = activeUser.UserID;
            FindViewById<TextView>(Resource.Id.locationCoordinatesText).Text = activeUser.Location;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.form_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == 16908332)
            {
                this.Finish();
            }

            if (item.ItemId == Resource.Id.menu_edit)
            {
                Intent = new Intent();
                Intent.SetType("image/*");
                Intent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), pickerID);
            }

            if (item.ItemId == Resource.Id.menu_save)
            {
                RunOnUiThread(async () => await SaveImage());
            }
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if ((requestCode == pickerID) && (resultCode == Result.Ok) && (data != null))
            {
                profileImage.SetImageURI(data.Data);
            }
        }

        private async Task SaveImage()
        {
            byte[] content;
            using (MemoryStream stream = new MemoryStream())
            {
                bitMapImage.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                content = stream.ToArray();
            }

            try
            {
                using (var stream = new MemoryStream())
                using (var bson = new BsonWriter(stream))
                {
                    var userImage = new UserImage
                    {
                        UserID = activeUser.UserID,
                        Content = content,
                        ContentType = ".bmp",
                        DateCreated = DateTime.Now,

                    };

                    var jsonSerializer = new JsonSerializer();
                    jsonSerializer.Serialize(bson, userImage);

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept
                        .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/bson"));

                    var byteArrayContent = new ByteArrayContent(stream.ToArray());
                    byteArrayContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/bson");

                    var result = await client.PostAsync(new Uri($"{RestService.BaseAddress}/api/users/updateuserImage"), byteArrayContent);
                    result.EnsureSuccessStatusCode();

                    if (result.IsSuccessStatusCode)
                    {
                        Toast.MakeText(this, "Image Saved to Database", ToastLength.Short).Show();
                    }
                    else
                    {
                        //Login Failed
                        alertDialog = new SupportMain.AlertDialog.Builder(this).Create();
                        alertDialog.SetMessage("Profie Image not saved. Please try again.");
                        alertDialog.SetIcon(Resource.Drawable.ic_info_black_24dp);
                        alertDialog.SetButton("OK", (_sender, args) => { });
                        alertDialog.Show();
                    }
                }
            }
            catch (Exception)
            {

            }
        }
    }
}