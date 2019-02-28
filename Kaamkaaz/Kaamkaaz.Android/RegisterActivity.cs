namespace Kaamkaaz.Droid
{
    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Support.Design.Widget;
    using Android.Text;
    using Android.Util;
    using Android.Views;
    using Android.Views.InputMethods;
    using Android.Widget;
    using Kaamkaaz.Droid.Helpers;
    using Kaamkaaz.Models;
    using Kaamkaaz.Services;
    using System;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// Defines the <see cref="RegisterActivity" />
    /// </summary>
    [Activity(Label = "Register", Theme = "@style/MainTheme.Base")]
    public class RegisterActivity : Android.Support.V7.App.AppCompatActivity
    {
        #region Methods

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
        /// The OnOptionsItemSelected
        /// </summary>
        /// <param name="item">The item<see cref="IMenuItem"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            return base.OnOptionsItemSelected(item);
        }

        /// <summary>
        /// The OnCreate
        /// </summary>
        /// <param name="savedInstanceState">The savedInstanceState<see cref="Bundle"/></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                SetContentView(Resource.Layout.registeruser);
                FloatingActionButton sendButton = FindViewById<FloatingActionButton>(Resource.Id.btnregister);
                sendButton.Click += delegate { btn_Register(); };
                //TODO: Remove the set call. It should be done after the login once
                Cache.SetUserId(5);
                Profile userProfile = KaamkaazService.GetUser(Cache.GetUserId());
                //Set tool bar
                var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
                SetSupportActionBar(toolbar);
                toolbar.SetNavigationIcon(Resource.Drawable.abc_ic_ab_back_material);
                toolbar.MenuItemClick += Toolbar_MenuItemClick;
                toolbar.NavigationClick += Naviagtion_Clicked;

                //clear focus and set char limit
                var nameText = FindViewById<EditText>(Resource.Id.username);
                nameText.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(70) });
                nameText.FocusChange += (sender, e) =>
                {

                    if (TextUtils.IsEmpty((sender as EditText).Text))
                    {
                        nameText.SetError("Name can't be empty.", null);

                    }
                };
                nameText.ClearFocus();
                nameText.Text = userProfile.Name;
                var phoneText = FindViewById<EditText>(Resource.Id.phonenumber);
                phoneText.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(10) });
                phoneText.FocusChange += (sender, e) =>
                {

                    if (TextUtils.IsEmpty((sender as EditText).Text))
                    {
                        phoneText.SetError("Valid phone number required.", null);

                    }
                };
                phoneText.ClearFocus();
                phoneText.Text = userProfile.Phone;
                SetServicesProvided(userProfile);
            }
            catch (Exception ex)
            {
                Log.Error("Register:", $"Unexpected error :{ex.Message}");
            }
        }

        private void SetServicesProvided(Profile userProfile)
        {
            foreach(var service in userProfile.ProfileData)
            {
                //TODO: Find the service in the resource switch and make it checked
                if (service == "Auto-riksha")
                {
                    var auto = FindViewById<Switch>(Resource.Id.auto);
                    auto.Checked = true;
                }
                if (service == "Electrician")
                {
                    var auto = FindViewById<Switch>(Resource.Id.electrician);
                    auto.Checked = true;
                }
                if (service == "Plumber")
                {
                    var auto = FindViewById<Switch>(Resource.Id.plumber);
                    auto.Checked = true;
                }
                if (service == "Taxi")
                {
                    var auto = FindViewById<Switch>(Resource.Id.taxi);
                    auto.Checked = true;
                }
                if (service == "Driver")
                {
                    var auto = FindViewById<Switch>(Resource.Id.driver);
                    auto.Checked = true;
                }
                if (service == "Housemaid")
                {
                    var auto = FindViewById<Switch>(Resource.Id.maid);
                    auto.Checked = true;
                }
                if (service == "Grocery Store")
                {
                    var auto = FindViewById<Switch>(Resource.Id.grocery);
                    auto.Checked = true;
                }
                if (service == "Painter")
                {
                    var auto = FindViewById<Switch>(Resource.Id.painter);
                    auto.Checked = true;
                }
                if (service == "Caterer")
                {
                    var auto = FindViewById<Switch>(Resource.Id.caterer);
                    auto.Checked = true;
                }
                if (service == "Mechanic")
                {
                    var auto = FindViewById<Switch>(Resource.Id.mechanic);
                    auto.Checked = true;
                }
                if (service == "Tutor")
                {
                    var auto = FindViewById<Switch>(Resource.Id.tutor);
                    auto.Checked = true;
                }
                if (service == "Volunteer")
                {
                    var auto = FindViewById<Switch>(Resource.Id.volunteer);
                    auto.Checked = true;
                }
            }
        }

        /// <summary>
        /// The btn_Register
        /// </summary>
        private void btn_Register()
        {
            //Disable the keyboard
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            var focus = Window.CurrentFocus;
            imm.HideSoftInputFromWindow(focus.WindowToken, 0);
            //Show progressbar on UI thread
            ShowProgressBar(true);
            //Start message processing on another thread
            System.Threading.ThreadStart processMessage = new System.Threading.ThreadStart(SaveProfile);
            Thread myThread = new Thread(processMessage);
            myThread.Start();
            //Stop the progress on UI thread
            ShowProgressBar(false);
            ShowDialog("Message", $"Thank your! Your information is saved.");
        }

        /// <summary>
        /// The Naviagtion_Clicked
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="Android.Support.V7.Widget.Toolbar.NavigationClickEventArgs"/></param>
        private void Naviagtion_Clicked(object sender, Android.Support.V7.Widget.Toolbar.NavigationClickEventArgs e)
        {
            string prevActivity = Intent.GetStringExtra("activity");
            if (prevActivity == "main")
            {
                Intent intent = new Intent(this, typeof(MainActivity));
                intent.PutExtra("activity", "register");
                StartActivity(intent);
            }
            if (prevActivity == "feedback")
            {
                Intent intent = new Intent(this, typeof(FeedbackActivity));
                intent.PutExtra("activity", "register");
                StartActivity(intent);
            }
            if (prevActivity == "messages")
            {
                Intent intent = new Intent(this, typeof(Message));
                intent.PutExtra("activity", "register");
                StartActivity(intent);
            }
        }

        /// <summary>
        /// The SaveProfile
        /// </summary>
        private void SaveProfile()
        {
            var txtName = FindViewById<EditText>(Resource.Id.username);
            var txtPhone = FindViewById<EditText>(Resource.Id.phonenumber);
            var profile = new Profile();
            profile.Name = txtName.Text;
            profile.Phone = txtPhone.Text;
            List<string> services = new List<string>();
            var auto = FindViewById<Switch>(Resource.Id.auto);
            if (auto.Checked)
            {
                profile.ProfileData.Add(auto.Text);
            }
            var drive = FindViewById<Switch>(Resource.Id.driver);
            if (drive.Checked)
            {
                profile.ProfileData.Add(drive.Text);
            }
            var electrician = FindViewById<Switch>(Resource.Id.electrician);
            if (electrician.Checked)
            {
                profile.ProfileData.Add(electrician.Text);
            }
            var grocery = FindViewById<Switch>(Resource.Id.grocery);
            if (electrician.Checked)
            {
                profile.ProfileData.Add(grocery.Text);
            }
            var maid = FindViewById<Switch>(Resource.Id.maid);
            if (maid.Checked)
            {
                profile.ProfileData.Add(maid.Text);
            }
            var mechanic = FindViewById<Switch>(Resource.Id.mechanic);
            if (mechanic.Checked)
            {
                profile.ProfileData.Add(mechanic.Text);
            }
            var plumber = FindViewById<Switch>(Resource.Id.plumber);
            if (plumber.Checked)
            {
                profile.ProfileData.Add(plumber.Text);
            }
            var painter = FindViewById<Switch>(Resource.Id.painter);
            if (painter.Checked)
            {
                profile.ProfileData.Add(painter.Text);
            }
            var taxi = FindViewById<Switch>(Resource.Id.taxi);
            if (taxi.Checked)
            {
                profile.ProfileData.Add(taxi.Text);
            }
            var tutor = FindViewById<Switch>(Resource.Id.tutor);
            if (tutor.Checked)
            {
                profile.ProfileData.Add(tutor.Text);
            }
            var volunteer = FindViewById<Switch>(Resource.Id.volunteer);
            if (volunteer.Checked)
            {
                profile.ProfileData.Add(volunteer.Text);
            }
            var caterer = FindViewById<Switch>(Resource.Id.caterer);
            if (caterer.Checked)
            {
                profile.ProfileData.Add(caterer.Text);
            }
            profile.Id = Cache.GetUserId();
            KaamkaazService.SaveProfile(profile);
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
        /// The Toolbar_MenuItemClick
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="Android.Support.V7.Widget.Toolbar.MenuItemClickEventArgs"/></param>
        private void Toolbar_MenuItemClick(object sender, Android.Support.V7.Widget.Toolbar.MenuItemClickEventArgs e)
        {
            if (e.Item.ItemId == Resource.Id.menu_messages)
            {
                Intent intent = new Intent(this, typeof(MessagesActivity));
                intent.PutExtra("activity", "register");
                StartActivity(intent);
            }
            if (e.Item.ItemId == Resource.Id.menu_feedback)
            {
                Intent intent = new Intent(this, typeof(FeedbackActivity));
                intent.PutExtra("activity", "register");
                StartActivity(intent);
            }
            if (e.Item.ItemId == Resource.Id.menu_register)
            {
                Intent intent = new Intent(this, typeof(RegisterActivity));
                intent.PutExtra("activity", "register");
                StartActivity(intent);
            }
        }

        #endregion
    }
}
