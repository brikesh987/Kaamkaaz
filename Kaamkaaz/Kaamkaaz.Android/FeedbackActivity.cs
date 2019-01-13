namespace Kaamkaaz.Droid
{
    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Util;
    using Android.Views;
    using Android.Widget;
    using System;

    /// <summary>
    /// Defines the <see cref="FeedbackActivity" />
    /// </summary>
    [Activity(Label = "Feedback", Theme = "@style/MainTheme.Base")]
    public class FeedbackActivity : Android.Support.V7.App.AppCompatActivity
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
            SetContentView(Resource.Layout.feedback);
            try
            {
                Button sendButton = FindViewById<Button>(Resource.Id.btnSendFeedback);
                sendButton.Click += delegate { btn_Send(); };
                //Set tool bar
                var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
                SetSupportActionBar(toolbar);
                toolbar.SetNavigationIcon(Resource.Drawable.abc_ic_ab_back_material);
                toolbar.MenuItemClick += Toolbar_MenuItemClick;
                toolbar.NavigationClick += Naviagtion_Clicked;
            }
            catch (Exception ex)
            {
                Log.Error("Feedback:", $"Unexpected error :{ex.Message}");
            }
        }

        /// <summary>
        /// The btn_Send
        /// </summary>
        private void btn_Send()
        {
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
                intent.PutExtra("activity", "feedback");
                StartActivity(intent);
            }
            if (prevActivity == "register")
            {
                Intent intent = new Intent(this, typeof(RegisterActivity));
                intent.PutExtra("activity", "feedback");
                StartActivity(intent);
            }
            if (prevActivity == "messages")
            {
                Intent intent = new Intent(this, typeof(Message));
                intent.PutExtra("activity", "feedback");
                StartActivity(intent);
            }
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
                intent.PutExtra("activity", "feedback");
                StartActivity(intent);
            }
            if (e.Item.ItemId == Resource.Id.menu_feedback)
            {
                Intent intent = new Intent(this, typeof(FeedbackActivity));
                intent.PutExtra("activity", "feedback");
                StartActivity(intent);
            }
            if (e.Item.ItemId == Resource.Id.menu_register)
            {
                Intent intent = new Intent(this, typeof(RegisterActivity));
                intent.PutExtra("activity", "feedback");
                StartActivity(intent);
            }
        }

        #endregion
    }
}
