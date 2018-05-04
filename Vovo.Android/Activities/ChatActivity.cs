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

using SupportV7 = Android.Support.V7.App;
using SupportFragment = Android.Support.V7.Widget;
using Android.Util;
using Android.Graphics.Drawables;
using Android.Graphics;
using Vovo.Models;
using Vovo.Android.Adapters;
using System.Net.Http;
using Vovo.Android.Models;
using Newtonsoft.Json;
using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json.Linq;
using WebSocket4Net;
using System.Threading.Tasks;
using System.Threading;

namespace Vovo.Android.Activities
{
    [Activity(Label = "Chat", Theme = "@style/Theme.AppTheme")]
    public class ChatActivity : SupportV7.AppCompatActivity
    {
        private ListView chatListView;
        private AlertDialog alertDialog;
        private List<User> users;
        HttpClient client;
        private UserFriend chatParty;
        private List<Vovo.Models.Message> messages;
        private User activeUser;
        private Chat activeChat;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.AddFlags(global::Android.Views.WindowManagerFlags.DrawsSystemBarBackgrounds);
            activeUser = JsonConvert.DeserializeObject<User>(Intent.GetStringExtra("User"));
            chatParty = JsonConvert.DeserializeObject<UserFriend>(Intent.GetStringExtra("UserFriend"));

            client = new HttpClient()
            {
                Timeout = new TimeSpan(0, 0, 30)
            };

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.chat);

            var toolbar = FindViewById<SupportFragment.Toolbar>(Resource.Id.toolbar);

            SetSupportActionBar(toolbar);
            SupportActionBar.Title = chatParty.Friend.FullName;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_arrow_back_white_24dp);

            chatListView = FindViewById<ListView>(Resource.Id.chat_thread);

            (FindViewById<Button>(Resource.Id.btnSend)).Click += SendMessage;

            //Create New Chat Instance
            RunOnUiThread(async () => await LoadItems());

            new Timer(state =>
            {
                RunOnUiThread(async () => await LoadItems());
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
        }

        private async Task LoadItems()
        {
            try
            {
                activeChat = new Chat
                {
                    SenderUserID = chatParty.UserUserID,
                    ReceiverUserID = chatParty.FriendUserID,
                    DateCreated = DateTime.Now,
                };

                var serializedChat = JsonConvert.SerializeObject(activeChat);
                var content = new StringContent(serializedChat, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(new Uri($"{RestService.BaseAddress}/api/users/creategetchat"), content);
                if (response.IsSuccessStatusCode)
                {
                    activeChat = JsonConvert.DeserializeObject<Chat>(await response.Content.ReadAsStringAsync());
                    messages = new List<Vovo.Models.Message>(activeChat.Messages ?? new List<Vovo.Models.Message>());
                    chatListView.Adapter = new ChatThreadAdapter(this, messages, activeUser);
                }
                else
                {
                    activeChat = new Chat();
                    //Login Failed
                    alertDialog = new AlertDialog.Builder(this).Create();
                    alertDialog.SetMessage("An error has occured. Please try again.");
                    alertDialog.SetIcon(Resource.Drawable.ic_info_black_24dp);
                    alertDialog.SetButton("OK", (s, a) => { });
                    alertDialog.Show();
                }

            }
            catch (Exception)
            {
            }
        }

        private void SendMessage(object sender, EventArgs e)
        {
            RunOnUiThread(async () =>
            {
                try
                {
                    var content = (FindViewById<EditText>(Resource.Id.messageText)).Text;
                    var message = new Vovo.Models.Message
                    {
                        ChatID = activeChat.ChatID,
                        UserID = activeUser.UserID,
                        Content = content,
                        DateCreated = DateTime.Now,
                        IsDelivered = true,
                    };

                    var serializedMessage = JsonConvert.SerializeObject(message);
                    var stringContent = new StringContent(serializedMessage, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(new Uri($"{RestService.BaseAddress}/api/users/postmessage"), stringContent);
                    if (response.IsSuccessStatusCode)
                    {
                        message = JsonConvert.DeserializeObject<Vovo.Models.Message>(await response.Content.ReadAsStringAsync());
                        messages.Add(message);

                        (FindViewById<ListView>(Resource.Id.chat_thread).Adapter as ChatThreadAdapter).NotifyDataSetChanged();
                        (FindViewById<EditText>(Resource.Id.messageText)).Text = string.Empty;
                    }
                    else
                    {
                        activeChat = new Chat();
                        //Login Failed
                        alertDialog = new AlertDialog.Builder(this).Create();
                        alertDialog.SetMessage("An error has occured. Please try again.");
                        alertDialog.SetIcon(Resource.Drawable.ic_info_black_24dp);
                        alertDialog.SetButton("OK", (s, a) => { });
                        alertDialog.Show();
                    }
                }
                catch (Exception)
                {
                }
            });
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == 16908332)
            {
                this.Finish();
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}