// -----------------------------------------------------------------------
// <copyright file="WearService.cs" company="Shameem">
// Copyright (c) Shameem Ahamed. All rights reserved.
// </copyright>
// <author>Shameem Ahmed</author>
// <date>16/05/2016</date>
// -----------------------------------------------------------------------

namespace WearService_Device.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Android.App;
    using Android.Gms.Common;
    using Android.Gms.Common.Apis;
    using Android.Gms.Wearable;
    using Android.Locations;
    using Android.OS;
    using Android.Runtime;
    using Android.Util;

    /// <summary>
    ///     Service for wearable
    /// </summary>
    [Service(Enabled = true, Name = "com.shameem.apps.WearService"),
     IntentFilter(new[] { "com.google.android.gms.wearable.BIND_LISTENER" })]
    public class WearService : WearableListenerService, ILocationListener, IGoogleApiClientConnectionCallbacks,
        IGoogleApiClientOnConnectionFailedListener
    {
        /// <summary>
        ///     The tag
        /// </summary>
        public const string Tag = "WEARSERVICE-DEVICE";

        /// <summary>
        ///     The path
        /// </summary>
        private readonly string path = "/wearService";

        /// <summary>
        ///     The client
        /// </summary>
        private IGoogleApiClient client;

        /// <summary>
        ///     The current location
        /// </summary>
        private Location currentLocation;

        /// <summary>
        ///     The location manager
        /// </summary>
        private LocationManager locationManager;

        /// <summary>
        ///     The location provider
        /// </summary>
        private string locationProvider;

        /// <summary>
        ///     Initializes a new instance of the <see cref="WearService" /> class.
        /// </summary>
        public WearService()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WearService" /> class.
        /// </summary>
        /// <param name="javaReference">The java reference.</param>
        /// <param name="transfer">The transfer.</param>
        protected WearService(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        /// <summary>
        ///     Occurs when [data received].
        /// </summary>
        public event Action<DataMap> DataReceived = delegate { };

        /// <summary>
        ///     Occurs when [message received].
        /// </summary>
        public event Action<string> MessageReceived = delegate { };

        /// <summary>
        ///     Connects the devices.
        /// </summary>
        public void ConnectDevices()
        {
            Log.Info(Tag, "Wear Service Connect Device");
            if (this.client != null && !this.client.IsConnected)
            {
                this.client.Connect();

                this.InitializeLocationManager();
                this.locationManager.RequestSingleUpdate(this.locationProvider, this, null);
            }
        }

        /// <summary>
        ///     Disconnects the devices.
        /// </summary>
        public void DisconnectDevices()
        {
            if (this.client != null && this.client.IsConnected)
            {
                this.client.Disconnect();
            }
        }

        /// <summary>
        ///     Called when [connected].
        /// </summary>
        /// <param name="connectionHint">The connection hint.</param>
        public void OnConnected(Bundle connectionHint)
        {
        }

        /// <summary>
        ///     Called when [connection failed].
        /// </summary>
        /// <param name="result">The result.</param>
        public void OnConnectionFailed(ConnectionResult result)
        {
            Log.Error(Tag, "Wear Service OnConnectionFailed - " + result);
        }

        /// <summary>
        ///     Called when [connection suspended].
        /// </summary>
        /// <param name="cause">The cause.</param>
        public void OnConnectionSuspended(int cause)
        {
            Log.Info(Tag, "Wear Service OnConnectionSuspended - " + cause);
        }

        /// <summary>
        ///     Called when [data changed].
        /// </summary>
        /// <param name="dataEvents">The data events.</param>
        public override void OnDataChanged(DataEventBuffer dataEvents)
        {
            Log.Info(Tag, "Wear Service On Data Changed");
            this.client = new GoogleApiClientBuilder(this)
                .AddApi(WearableClass.Api)
                .Build();

            this.ConnectDevices();

            Log.Info(Tag, "Wear Service Data changed ({0} data events", dataEvents.Count());

            foreach (var dataEvent in dataEvents)
            {
                if (dataEvent.Type == DataEvent.TypeChanged && dataEvent.DataItem.Uri.Path == this.path)
                {
                    this.DataReceived(DataMapItem.FromDataItem(dataEvent.DataItem).DataMap);
                }
            }
        }

        /// <summary>
        ///     Called when [location changed].
        /// </summary>
        /// <param name="location">The location.</param>
        public void OnLocationChanged(Location location)
        {
            Log.Info(Tag, "Wear Service On LOCATION DETECTED");
            this.currentLocation = location;
            if (this.currentLocation != null)
            {
                this.locationManager.RemoveUpdates(this);
            }
        }

        /// <summary>
        ///     Called when [message received].
        /// </summary>
        /// <param name="messageEvent">The message event.</param>
        public override void OnMessageReceived(IMessageEvent messageEvent)
        {
            Log.Info(Tag, "Wear Service On Message Received");

            this.client = new GoogleApiClientBuilder(this)
                .AddApi(WearableClass.Api)
                .Build();

            this.ConnectDevices();

            var message = Encoding.Default.GetString(messageEvent.GetData());
            Console.WriteLine("Communicator: Message received \"{0}\"", message);
            Log.Info(Tag, "Wear Service On Message Received \"{0}\"", message);
            switch (message)
            {
                case "TASK 1":

                    break;
                case "TASK 2":
                    break;
                case "TASK 3":
                    if (this.IsGpsEnabled())
                    {
                        Log.Info(Tag, "Wear Service On GPS - TRUE");

                        if (this.locationManager != null && !string.IsNullOrEmpty(this.locationProvider))
                        {
                            var lastLocation = this.locationManager.GetLastKnownLocation(this.locationProvider);

                            if (lastLocation != null)
                            {
                            }
                        }
                        else
                        {
                            Log.Info(Tag, "Wear Service On locationManager - NULL");
                        }
                    }
                    else
                    {
                        Log.Info(Tag, "Wear Service On GPS - FALSE");
                    }

                    break;
                default:
                    break;
            }

            this.MessageReceived(message);
        }

        /// <summary>
        ///     Called when [peer disconnected].
        /// </summary>
        /// <param name="peer">The peer.</param>
        public override void OnPeerDisconnected(INode peer)
        {
            base.OnPeerDisconnected(peer);

            if (this.locationManager != null)
            {
                this.locationManager.RemoveUpdates(this);
            }
        }

        /// <summary>
        ///     Called when [provider disabled].
        /// </summary>
        /// <param name="provider">The provider.</param>
        public void OnProviderDisabled(string provider)
        {
            // throw new NotImplementedException();
        }

        /// <summary>
        ///     Called when [provider enabled].
        /// </summary>
        /// <param name="provider">The provider.</param>
        public void OnProviderEnabled(string provider)
        {
            // throw new NotImplementedException();
        }

        /// <summary>
        ///     Called when [status changed].
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="status">The status.</param>
        /// <param name="extras">The extras.</param>
        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            // throw new NotImplementedException();
        }

        /// <summary>
        ///     Sends the data.
        /// </summary>
        /// <param name="dataMap">The data map.</param>
        public void SendData(DataMap dataMap)
        {
            Log.Info(Tag, "Wear Service On Send Data");
            Task.Run(() =>
            {
                var request = PutDataMapRequest.Create(this.path);
                request.DataMap.PutAll(dataMap);
                if (this.client != null)
                {
                    var result = WearableClass.DataApi.PutDataItem(this.client, request.AsPutDataRequest()).Await();
                    var success = result.JavaCast<IDataApiDataItemResult>().Status.IsSuccess ? "Ok." : "Failed!";
                }
                else
                {
                    Log.Error(Tag, "Wear Service Send Data - Client is NULL");
                }
            });
        }

        /// <summary>
        ///     Initializes the location manager.
        /// </summary>
        private void InitializeLocationManager()
        {
            Log.Info(Tag, "Wear Service InitializeLocationManager");
            this.locationManager = (LocationManager)this.GetSystemService(LocationService);
            var locationCriteria = new Criteria();

            locationCriteria.Accuracy = Accuracy.Coarse;
            locationCriteria.PowerRequirement = Power.Medium;

            this.locationProvider = this.locationManager.GetBestProvider(locationCriteria, true);
        }

        /// <summary>
        ///     Determines whether [is GPS enabled].
        /// </summary>
        /// <returns>Returns true or false</returns>
        private bool IsGpsEnabled()
        {
            this.locationManager = (LocationManager)this.GetSystemService(LocationService);

            if (!this.locationManager.IsProviderEnabled(LocationManager.NetworkProvider))
            {
                Log.Info(Tag, "Wear Service IsGpsEnabled - False");
                return false;
            }

            Log.Info(Tag, "Wear Service IsGpsEnabled - True");
            return true;
        }

        /// <summary>
        ///     Nodes this instance.
        /// </summary>
        /// <returns>The Nodes</returns>
        private IList<INode> Nodes()
        {
            Log.Info(Tag, "Wear Service On Nodes Start");
            var result = WearableClass.NodeApi.GetConnectedNodes(this.client).Await();
            Log.Info(Tag, "Wear Service On Nodes End");
            return result.JavaCast<INodeApiGetConnectedNodesResult>().Nodes;
        }
    }
}