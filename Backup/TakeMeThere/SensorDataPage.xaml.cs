using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.IO.IsolatedStorage;

namespace TakeMeThere
{
    public partial class SensorDataPage : PhoneApplicationPage
    {
        MySensors Sensor;

        DispatcherTimer timer = new DispatcherTimer();

        bool PermissionOfLocationService;

        public SensorDataPage()
        {
            InitializeComponent();


        }

        //メインページに移動してきた時の処理。
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PermissionOfLocationService = (bool)IsolatedStorageSettings.ApplicationSettings["LocationService"];
            if (Sensor == null)
                Sensor = new MySensors();

            Sensor.UserPermission = PermissionOfLocationService;

            Sensor.GpsMovementThreshold = (double)IsolatedStorageSettings.ApplicationSettings["DistanceUpdateGPS_sensor"];
            Sensor.UpdateCompassTimeSpan = (double)IsolatedStorageSettings.ApplicationSettings["TimeSpanUpdateCompass_sensor"];
            Sensor.AvgSpeed = (double)IsolatedStorageSettings.ApplicationSettings["AverageSpeed"];
            Sensor.CompassDataChanged += sensor_CompassDataChanged;
            Sensor.GPSDataChanged += sensor_GPSDataChanged;
            Sensor.GPSStatusChanged += sensor_GPSStatusChanged;
                Sensor.Start();


            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += timer_Tick;
            timer.Start();

            initDisplay();

            base.OnNavigatedTo(e);
        }

        private void initDisplay()
        {
            TextBlock_Latitude.Text = "";
            TextBlock_AvgSpeed.Text = "";
            TextBlock_Altitude.Text = "";
            TextBlock_Course.Text = "";
            TextBlock_HeadingAccuracy.Text = "";
            TextBlock_HorizontalAccuracy.Text = "";
            TextBlock_IsCompassDataValid.Text = "";
            TextBlock_IsLocationUnknown.Text = "";
            TextBlock_Longitude.Text = "";
            TextBlock_MagneticHeading.Text = "";
            TextBlock_Speed.Text = "";
            TextBlock_VerticalAccuracy.Text = "";
            TextBlock_TrueHeading.Text = "";
            TextBlock_GpsStatus.Text = "Disable";
            
        }

        //メインページから離脱した時の処理。
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Sensor.Stop();
            Sensor.CompassDataChanged -= sensor_CompassDataChanged;
            Sensor.GPSDataChanged -= sensor_GPSDataChanged;
            Sensor.GPSStatusChanged -= sensor_GPSStatusChanged;

            timer.Stop();
            timer.Tick -= timer_Tick;

            base.OnNavigatedFrom(e);
        }


        Queue<double> speedRecorder_forSmoothSpeed = new Queue<double>();
        void timer_Tick(object sender, EventArgs e)
        {
            if (double.IsNaN(Sensor.Speed) == false)//キューの長さを常に10に保つ。
            {
                speedRecorder_forSmoothSpeed.Enqueue(Sensor.Speed);
                if (speedRecorder_forSmoothSpeed.Count == 10)
                {
                    speedRecorder_forSmoothSpeed.Dequeue();
                }
            }
            double sum = 0;
            int num = speedRecorder_forSmoothSpeed.Count;
            for (var i = 0; i < num; i++)
            {
                sum = sum + speedRecorder_forSmoothSpeed.ElementAt(i);
            }

            var avg = (sum) / (num);
            if (avg < 0.027 || double.IsNaN(avg))
                avg = 0;
            TextBlock_Speed.Text = avg.ToString();
        }



        void sensor_GPSStatusChanged(object sender, GPSStatusChangedEventArgs e)
        {
            TextBlock_GpsStatus.Text = Sensor.GpsStatus.ToString();
        }

        void sensor_GPSDataChanged(object sender, GPSDataChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                TextBlock_Latitude.Text = Sensor.Latitude.ToString();
                TextBlock_Longitude.Text = Sensor.Longitude.ToString();
                //TextBlock_Speed.Text = sensor.Speed.ToString();
                TextBlock_AvgSpeed.Text = Sensor.AvgSpeed.ToString();
                TextBlock_Altitude.Text = Sensor.Altitude.ToString();
                TextBlock_IsLocationUnknown.Text = Sensor.IsLocationUnknown.ToString();
                TextBlock_Course.Text = Sensor.Course.ToString();
                TextBlock_HorizontalAccuracy.Text = Sensor.HorizontalAccuracy.ToString();
                TextBlock_VerticalAccuracy.Text = Sensor.VerticalAccuracy.ToString();
            });
        }

        void sensor_CompassDataChanged(object sender, CompassDataChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                TextBlock_HeadingAccuracy.Text = Sensor.HeadingAccuracy.ToString();
                TextBlock_MagneticHeading.Text = Sensor.MagneticHeading.ToString();
                TextBlock_TrueHeading.Text = Sensor.TrueHeading.ToString();
                TextBlock_IsCompassDataValid.Text = Sensor.IsCompassDataValid.ToString();
            });
        }
    }
}
