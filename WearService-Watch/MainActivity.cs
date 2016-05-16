using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.Wearable.Views;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Java.Interop;
using Android.Views.Animations;

namespace WearService_Watch
{
    using Android.Gms.Wearable;
    using Android.Util;
    using Services;

    [Activity(Label = "WearService_Watch", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        /// <summary>
        ///     The tag
        /// </summary>
        public const string Tag = "WEARSERVICE-WATCH";

        private WearService wearService;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            this.InitializeCommunicator();
            var isConnectedToDevice = this.InitializeAdapter();
            if (!isConnectedToDevice)
            {
                Log.Error(Tag, "Not Connected to Device");
                // Show Error Page
            }
        }

        /// <summary>
        /// Initializes the communicator.
        /// </summary>
        private void InitializeCommunicator()
        {
            Log.Info(Tag, "Initialize Wear Service");
            this.wearService = new WearService();
            this.wearService.ConnectDevices();
        }

        /// <summary>
        /// Initializes the adapter.
        /// </summary>
        /// <returns></returns>
        private bool InitializeAdapter()
        {
            Log.Info(Tag, "Initialize Adapter");
            var isConnected = false;

            this.wearService.SendMessage("TASK 1");
            this.wearService.SendMessage("TASK 2");
            this.wearService.SendMessage("TASK 3");

            this.wearService.MessageReceived += message => this.RunOnUiThread(() =>
            {
                Log.Info(Tag, "GridPagerMainActivity - MessageReceived");

                isConnected = true;

                this.ParseResultfromDevice(message);
            });

            this.wearService.DataReceived += dataMap => this.RunOnUiThread(() =>
            {
                Log.Info(Tag, "Data Received");
                this.ParseDatafromDevice(dataMap);
            });

            return isConnected;
        }

        private void ParseResultfromDevice(string message)
        {

        }

        private void ParseDatafromDevice(DataMap dataMap)
        {

        }
    }
}


