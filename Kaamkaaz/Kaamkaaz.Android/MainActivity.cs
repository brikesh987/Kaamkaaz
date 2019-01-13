

namespace Kaamkaaz.Droid
{
    using Android;
    using Android.App;
    using Android.Content;
    using Android.Content.PM;
    using Android.Gms.Common;
    using Android.Gms.Location;
    using Android.Gms.Maps;
    using Android.Gms.Maps.Model;
    using Android.OS;
    using Android.Support.V4.App;
    using Android.Text;
    using Android.Util;
    using Android.Views;
    using Android.Views.InputMethods;
    using Android.Widget;
    using Kaamkaaz.Droid.Helpers;
    using Kaamkaaz.Models;
    using Kaamkaaz.Services;
    using Nito.AsyncEx.Synchronous;
    using Plugin.Geolocator;
    using Plugin.Geolocator.Abstractions;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="MainActivity" />
    /// </summary>
    [Activity(Label = "Kaamkaaz", Icon = "@mipmap/icon", MainLauncher = true, Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Android.Support.V7.App.AppCompatActivity, IOnMapReadyCallback
    {
        #region Fields

        /// <summary>
        /// Defines the fusedLocationProviderClient
        /// </summary>
        internal FusedLocationProviderClient fusedLocationProviderClient;

        /// <summary>
        /// Defines the map
        /// </summary>
        internal GoogleMap map;

        #endregion

        #region Methods

        /// <summary>
        /// The btn_Send
        /// </summary>
        public void btn_Send()
        {
            //Disable the keyboard
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            var focus = Window.CurrentFocus;
            imm.HideSoftInputFromWindow(focus.WindowToken, 0);
            //Show progressbar on UI thread
            ShowProgressBar(true);
            //Start message processing on another thread
            System.Threading.ThreadStart processMessage = new System.Threading.ThreadStart(BroadcastMessage);
            Thread myThread = new Thread(processMessage);
            myThread.Start();
            //Stop the progress on UI thread
            ShowProgressBar(false);
            ShowDialog("Message", $"Your message has been sent to all nearby service providers.");
        }

        /// <summary>
        /// The OnCreateOptionsMenu
        /// </summary>
        /// <param name="menu">The menu<see cref="IMenu"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.toolbar_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        /// <summary>
        /// The OnMapReady
        /// </summary>
        /// <param name="googleMap">The googleMap<see cref="GoogleMap"/></param>
        public void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;
            map.MyLocationEnabled = true;
            map.SetIndoorEnabled(true);
            map.MapType = GoogleMap.MapTypeNormal;
            ConfigureUiSettings(map);
            GetLocation(map);
        }

        /// <summary>
        /// The OnOptionsItemSelected
        /// </summary>
        /// <param name="item">The item<see cref="IMenuItem"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            //TODO: redirect to new form
            var textToShow = "";
            if (item.ItemId == Resource.Id.menu_messages)
            {
                textToShow = "message was selected";
            }
            if (item.ItemId == Resource.Id.menu_feedback)
            {
                textToShow = "feedback was selected";
            }
            if (item.ItemId == Resource.Id.menu_register)
            {
                textToShow = "register was selected";
            }            
            return base.OnOptionsItemSelected(item);
        }

        /// <summary>
        /// The OnCreate
        /// </summary>
        /// <param name="savedInstanceState">The savedInstanceState<see cref="Bundle"/></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //TabLayoutResource = Resource.Layout.Tabbar;
            //ToolbarResource = Resource.Layout.Toolbar;
            fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);
            base.OnCreate(savedInstanceState);
            //global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            //LoadApplication(new App());
            SetContentView(Resource.Layout.Main);

            //Set tool bar
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            toolbar.MenuItemClick += Toolbar_MenuItemClick;
            //toolbar.NavigationClick += Toolbar_NavigationClick;
            //SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            //SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.);            

            Spinner spinner = FindViewById<Spinner>(Resource.Id.spinner);
            //Don't need an event on spinner selection
            //spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);

            //var adapter = ArrayAdapter.CreateFromResource(
            //        this, Resource.Array.planets_array, Android.Resource.Layout.SimpleSpinnerDropDownItem);
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, KaamkaazService.GetSericeTypes("India"));

            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;

            ImageButton sendButton = FindViewById<ImageButton>(Resource.Id.button1);
            sendButton.Click += delegate { btn_Send(); };

            //set character limit on the textbox
            var requestMessageText = FindViewById<EditText>(Resource.Id.edittext);
            requestMessageText.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(140) });
            requestMessageText.ClearFocus();

            RequestLocationPermission();
            if (LocationPermissionAvailable() && IsGooglePlayServicesInstalled())
            {
                SetupGoogleMap();
            }
        }

        /// <summary>
        /// The BroadcastMessage
        /// </summary>
        private void BroadcastMessage()
        {
            Spinner spinner = FindViewById<Spinner>(Resource.Id.spinner);
            var message = new BroadcastMessage();
            message.RequestedService = spinner.SelectedItem.ToString();
            var requestMessageText = FindViewById<EditText>(Resource.Id.edittext);
            message.Message = requestMessageText.Text;
            KaamkaazService.Broadcast(message);
            Task.Delay(50000);
        }

        /// <summary>
        /// The ConfigureUiSettings
        /// </summary>
        /// <param name="map">The map<see cref="GoogleMap"/></param>
        private void ConfigureUiSettings(GoogleMap map)
        {
            map.UiSettings.ZoomControlsEnabled = true;
            map.UiSettings.ZoomGesturesEnabled = true;
            map.UiSettings.MyLocationButtonEnabled = true;
            //map.UiSettings.SetAllGesturesEnabled(true);
            map.MyLocationEnabled = true;
        }

        /// <summary>
        /// Gets the last known location
        /// </summary>
        /// <param name="map"></param>
        private void GetLocation(GoogleMap map)
        {
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 102;
            if (locator.IsGeolocationAvailable)
            {
                var positionTask = locator.GetLastKnownLocationAsync();
                positionTask.WaitAndUnwrapException();
                SetPosition(positionTask, map);
            }
        }

        /// <summary>
        /// The GetLocationWithNeighbor
        /// </summary>
        /// <param name="map">The map<see cref="GoogleMap"/></param>
        private void GetLocationWithNeighbor(GoogleMap map)
        {
            LatLng location = new LatLng(23.667613, 86.151678);
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(location);
            builder.Zoom(14);
            builder.Bearing(155);
            builder.Tilt(50);

            CameraPosition cameraPosition = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

            map.MoveCamera(cameraUpdate);
            map.Clear();
            var youMarker = new MarkerOptions();
            youMarker.SetPosition(location);
            youMarker.SetTitle("Brikesh Kumar");
            youMarker.SetSnippet("Brikesh Kumar");
            map.AddMarker(youMarker);

            //Add circle
            int strokeColor = Int32.Parse("335AFF", System.Globalization.NumberStyles.HexNumber);
            int fillColor = Int32.Parse("33BAFF", System.Globalization.NumberStyles.HexNumber);
            CircleOptions circleOptions = new CircleOptions();
            circleOptions.InvokeCenter(location)
                .InvokeRadius(20)   //Radius is in meters
                .InvokeStrokeWidth(4)
                .InvokeStrokeColor(strokeColor)
                .InvokeFillColor(fillColor);

            //circleOptions.InvokeRadius(500);
            //int strokeColor = Int32.Parse("335AFF", System.Globalization.NumberStyles.HexNumber);
            //circleOptions.InvokeStrokeColor(strokeColor);
            circleOptions.Visible(true);
            map.AddCircle(circleOptions);

            //Add pins/neighbors
            var pins = GetNeighbors(location);
            map.SetInfoWindowAdapter(new WindowAdapter(LayoutInflater));
            pins.ForEach(x => map.AddMarker(x));
        }

        /// <summary>
        /// The GetNeighbors
        /// </summary>
        /// <param name="location">The location<see cref="LatLng"/></param>
        /// <returns>The <see cref="List{MarkerOptions}"/></returns>
        private List<MarkerOptions> GetNeighbors(LatLng location)
        {
            var list = new List<MarkerOptions>();
            var marker1 = new MarkerOptions();
            marker1.SetTitle("Bokaro sweets");
            //23.667613, 86.151678
            var loc1 = new LatLng(23.668, 86.151679);
            marker1.SetPosition(loc1);
            marker1.Visible(true);
            list.Add(marker1);

            var marker2 = new MarkerOptions();
            marker2.SetTitle("Sharma Furniture");
            var loc2 = new LatLng(23.669, 86.151680);
            marker2.SetPosition(loc2);
            marker2.Visible(true);
            list.Add(marker2);

            var marker3 = new MarkerOptions();
            marker3.SetTitle("Raju Electricals");
            var loc3 = new LatLng(23.66812, 86.151681);
            marker3.SetPosition(loc3);
            marker3.Visible(true);
            list.Add(marker3);

            return list;
        }

        /// <summary>
        /// The IsGooglePlayServicesInstalled
        /// </summary>
        /// <returns>The <see cref="bool"/></returns>
        private bool IsGooglePlayServicesInstalled()
        {
            var queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (queryResult == ConnectionResult.Success)
            {
                Log.Info("MainActivity", "Google Play Services is installed on this device.");
                return true;
            }

            if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
            {
                // Check if there is a way the user can resolve the issue
                var errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);
                Log.Error("MainActivity", "There is a problem with Google Play Services on this device: {0} - {1}",
                          queryResult, errorString);

                // Alternately, display the error to the user.
                ShowDialog(title: "Google Play", message: "Google play is not installed on the device");
            }

            return false;
        }

        /// <summary>
        /// The LocationPermissionAvailable
        /// </summary>
        /// <returns>The <see cref="bool"/></returns>
        private bool LocationPermissionAvailable()
        {
            bool isRequestingLocationUpdates = false;
            if (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == Permission.Granted)
            {
                //StartRequestingLocationUpdates();
                isRequestingLocationUpdates = true;
            }
            else
            {
                //TODO:
                ShowDialog(title: "Location Services", message: "The app requires permission to access location");
                //RequestLocationPermission();
            }
            return isRequestingLocationUpdates;
        }

        /// <summary>
        /// The RequestLocationPermission
        /// </summary>
        private void RequestLocationPermission()
        {
            if (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation }, 0);
            }
        }

        /// <summary>
        /// The SetPosition
        /// </summary>
        /// <param name="position">The position<see cref="Task{Position}"/></param>
        /// <param name="gmap">The gmap<see cref="GoogleMap"/></param>
        private void SetPosition(Task<Position> position, GoogleMap gmap)
        {
            if (position.Result == null)
            {
                Log.Debug("Kaamkaz GeoLocation", "Location service failed to return last known location");
                return;
            }
            LatLng location = new LatLng(position.Result.Latitude, position.Result.Longitude);
            //Add circle
            int strokeColor = Int32.Parse("335AFF", System.Globalization.NumberStyles.HexNumber);
            int fillColor = Int32.Parse("33BAFF", System.Globalization.NumberStyles.HexNumber);
            CircleOptions circleOptions = new CircleOptions();
            circleOptions.InvokeCenter(location)
                .InvokeRadius(20)   //Radius is in meters
                .InvokeStrokeWidth(4)
                .InvokeStrokeColor(strokeColor)
                .InvokeFillColor(fillColor);

            circleOptions.Visible(true);
            gmap.AddCircle(circleOptions);

            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(location);
            builder.Zoom(14);
            builder.Bearing(155);
            builder.Tilt(50);

            CameraPosition cameraPosition = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
            gmap.MoveCamera(cameraUpdate);

            gmap.Clear();
            var youMarker = new MarkerOptions();
            youMarker.SetPosition(location);
            youMarker.SetTitle("Brikesh Kumar");
            youMarker.SetSnippet("Brikesh Kumar");
            gmap.AddMarker(youMarker);
        }

        /// <summary>
        /// The SetPostionToLastLocation
        /// </summary>
        /// <returns>The <see cref="Task{Android.Locations.Location}"/></returns>
        private async Task<Android.Locations.Location> SetPostionToLastLocation()
        {
            //// This method assumes that the necessary run-time permission checks have succeeded.            
            Android.Locations.Location location = await fusedLocationProviderClient.GetLastLocationAsync();

            if (location == null)
            {
                // Seldom happens, but should code that handles this scenario
            }
            else
            {
                // Do something with the location 
                Log.Debug("Sample", "The latitude is " + location.Latitude);
            }
            //var locator = CrossGeolocator.Current;
            //var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(10));
            //map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude),
            //                             Distance.FromMiles(1));
            return location;
        }

        /// <summary>
        /// The SetupGoogleMap
        /// </summary>
        private void SetupGoogleMap()
        {
            if (map == null)
            {
                var mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.map);
                mapFragment.GetMapAsync(this);
            }
        }

        /// <summary>
        /// The ShowDialog
        /// </summary>
        /// <param name="title">The title<see cref="string"/></param>
        /// <param name="message">The message<see cref="string"/></param>
        private void ShowDialog(string title, string message)
        {
            var builder = new AlertDialog.Builder(this);

            builder.SetTitle(title)
                   .SetMessage(message)
                   //.SetPositiveButton("Yes", delegate { Console.WriteLine("Yes"); })
                   .SetNegativeButton("Ok", delegate { Console.WriteLine("No"); });

            builder.Create().Show();
        }

        /// <summary>
        /// The ShowProgressBar
        /// </summary>
        /// <param name="show">The show<see cref="bool"/></param>
        private void ShowProgressBar(bool show)
        {
            RunOnUiThread(() =>
            {
                ProgressBar probar = FindViewById<ProgressBar>(Resource.Id.progressbar);
                probar.Visibility = show ? ViewStates.Visible : ViewStates.Invisible;

            });
        }

        /// <summary>
        /// The spinner_ItemSelected
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="AdapterView.ItemSelectedEventArgs"/></param>
        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
        }

        /// <summary>
        /// The Toolbar_MenuItemClick
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="Android.Support.V7.Widget.Toolbar.MenuItemClickEventArgs"/></param>
        private void Toolbar_MenuItemClick(object sender, Android.Support.V7.Widget.Toolbar.MenuItemClickEventArgs e)
        {
            if (e.Item.ItemId == Resource.Id.menu_messages)
            {
                Intent intent = new Intent(this, typeof(MessagesActivity));
                intent.PutExtra("activity", "main");
                StartActivity(intent);
            }
            if (e.Item.ItemId == Resource.Id.menu_feedback)
            {
                Intent intent = new Intent(this, typeof(FeedbackActivity));
                intent.PutExtra("activity", "main");
                StartActivity(intent);
            }
            if (e.Item.ItemId == Resource.Id.menu_register)
            {               
                Intent intent = new Intent(this, typeof(RegisterActivity));
                intent.PutExtra("activity", "main");
                StartActivity(intent);
            }            
        }

        #endregion
    }
}
