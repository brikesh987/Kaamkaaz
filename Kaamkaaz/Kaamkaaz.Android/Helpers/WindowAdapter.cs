using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Kaamkaaz.Droid.Helpers
{
    public class WindowAdapter : Java.Lang.Object, Android.Gms.Maps.GoogleMap.IInfoWindowAdapter
    {
        private readonly LayoutInflater inflater;        

        public WindowAdapter(LayoutInflater inflater)
        {
            this.inflater = inflater;
        }
        
        public View GetInfoContents(Marker marker)
        {
            return null;
        }

        public View GetInfoWindow(Marker marker)
        {
            View view = inflater.Inflate(Resource.Layout.info_window, null, false);
            view.FindViewById<TextView>(Resource.Id.textName).Text = marker.Title;
            view.FindViewById<TextView>(Resource.Id.textPhone).Text = marker.Snippet;
            return view;
        }
    }
}