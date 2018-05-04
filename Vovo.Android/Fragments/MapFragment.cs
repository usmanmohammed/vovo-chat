using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Vovo.Android.Activities;
using Vovo.Models;

namespace Vovo.Android.Fragments
{
    public class MapFragment : Fragment, IOnMapReadyCallback
    {
        private GoogleMap map;
        private MapView mapView;
        private MainActivity mainActivity;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.map_layout, container, false);
            mainActivity = this.Activity as MainActivity;

            mapView = view.FindViewById<MapView>(Resource.Id.map);
            mapView.OnCreate(savedInstanceState);
            mapView.GetMapAsync(this);

            mapView.OnResume();

            return view;
        }

        public async override void OnResume()
        {
            mapView.OnResume();
            base.OnResume();

            await mainActivity.GetFriendsAsync();
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;
            if (map != null)
            {
                map.MapType = GoogleMap.MapTypeNormal;
                map.UiSettings.ZoomControlsEnabled = true;
                map.UiSettings.CompassEnabled = true;
                map.UiSettings.MyLocationButtonEnabled = true;
                map.UiSettings.ZoomGesturesEnabled = true;
                map.UiSettings.SetAllGesturesEnabled(true);

                foreach (var friend in mainActivity.FriendsList)
                {
                    double lat = double.Parse(friend.Friend.Location.Split(':')[0]);
                    double lng = double.Parse(friend.Friend.Location.Split(':')[1]);
                    MarkerOptions marker = new MarkerOptions();
                    marker.SetPosition(new LatLng(lat, lng));
                    marker.SetTitle(friend.Friend.FullName);
                    marker.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueRed));
                    map.AddMarker(marker);
                }

                if (mainActivity.FriendsList.Count > 0)
                {
                    double _lat = double.Parse(mainActivity.FriendsList.Last().Friend.Location.Split(':')[0]);
                    double _lng = double.Parse(mainActivity.FriendsList.Last().Friend.Location.Split(':')[1]);
                    map.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(_lat, _lng), 10));
                }

                //Load Friends Markers
            }
        }

        public override void OnDestroy()
        {
            mapView.OnDestroy();
            base.OnDestroy();
        }

        public override void OnPause()
        {
            mapView.OnPause();
            base.OnPause();
        }

        public override void OnLowMemory()
        {
            mapView.OnLowMemory();
            base.OnLowMemory();
        }
    }
}