using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Device.Location;
using Microsoft.Devices.Sensors;
using System.Windows.Threading;
//using Microsoft.Xna.Framework;
using System.ComponentModel;

namespace TakeMeThere
{
    //自作イベント用のイベント引数として使うクラス
    //コンパスデータが更新されたら発行される。
    public class CompassDataChangedEventArgs : EventArgs
    {
        public CompassDataChangedEventArgs()
        {
        }
    }

    //自作イベント用のイベント引数として使うクラス
    //GPSデータが更新されたら発行される。
    public class GPSDataChangedEventArgs : EventArgs
    {
        public GPSDataChangedEventArgs()
        {
        }
    }

    //自作イベント用のイベント引数として使うクラス
    //GPSの状態が更新されたら発行される。
    public class GPSStatusChangedEventArgs : EventArgs
    {
        public GPSStatusChangedEventArgs()
        {
        }
    }


    class Sensors
    {
        //自作イベントの宣言、実装
        public delegate void CompassDataChangedEventHandler(object sender, CompassDataChangedEventArgs e);
        public event CompassDataChangedEventHandler CompassDataChanged;
        protected virtual void OnCompassDataChanged(CompassDataChangedEventArgs e)//このメソッドでイベント発行。
        {
            if (CompassDataChanged != null)
            {
                CompassDataChanged(this, e);
            }
        }

        //自作イベントの宣言、実装
        public delegate void GPSDataChangedEventHandler(object sender, GPSDataChangedEventArgs e);
        public event GPSDataChangedEventHandler GPSDataChanged;
        protected virtual void OnGPSDataChanged(GPSDataChangedEventArgs e)//このメソッドでイベント発行。
        {
            if (GPSDataChanged != null)
            {
                GPSDataChanged(this, e);
            }
        }

        //自作イベントの宣言、実装
        public delegate void GPSStatusChangedEventHandler(object sender, GPSStatusChangedEventArgs e);
        public event GPSStatusChangedEventHandler GPSStatusChanged;
        protected virtual void OnGPSStatusChanged(GPSStatusChangedEventArgs e)//このメソッドでイベント発行。
        {
            if (GPSStatusChanged != null)
            {
                GPSStatusChanged(this, e);
            }
        }


        private GeoCoordinateWatcher wtc;
        private double _gpsMovementThreshold = 0;//meter
        public double GpsMovementThreshold
        {
            get { return _gpsMovementThreshold; }
            set { _gpsMovementThreshold = value; }
        }
        private Compass cmp;
        private int _updateCompassTimeSpan = 200;//ms
        public int UpdateCompassTimeSpan
        {
            get { return _updateCompassTimeSpan; }
            set { _updateCompassTimeSpan = value; }
        }

        private double _altitude;
        private double _course;
        private double _horizontalAccuracy;
        private double _latitude;
        private double _longitude;
        private double _speed;
        private double _verticalAccuracy;
        private double _avgSpeed;
        private GeoPositionStatus _gpsStatus;
        private bool _isLocationUnknown = false;
        private DateTime _timeStamp;

        private double _magneticHeading;
        private double _trueHeading;
        private double _headingAccuracy;
        private bool _isCompassDataValid = false;
        //Vector3 _rawMagnetometerReading;
        private bool calibrating = false;

        #region Compassパラメータ
        public double MagneticHeading
        {
            get
            {
                return _magneticHeading;
            }
            set
            { _magneticHeading = value; }
        }
        public double TrueHeading
        {
            get
            { return _trueHeading; }
            set
            {
                _trueHeading = value;
            }
        }
        public double HeadingAccuracy
        {
            get
            { return _headingAccuracy; }
            set
            {
                _headingAccuracy = value;
            }
        }
        public bool IsCompassDataValid
        {
            get
            { return _isCompassDataValid; }
            set
            { _isCompassDataValid = value; }
        }


        #endregion

        #region GPSパラメータ
        public double Altitude
        {
            get
            { return _altitude; }
            set
            { _altitude = value; }
        }
        public double Course
        {
            get
            { return _course; }
            set
            { _course = value; }
        }
        public double HorizontalAccuracy
        {
            get
            { return _horizontalAccuracy; }
            set
            { _horizontalAccuracy = value; }
        }
        public double Latitude
        {
            get
            { return _latitude; }
            set
            {
                _latitude = value;
            }
        }
        public double Longitude
        {
            get
            { return _longitude; }
            set
            { _longitude = value; }
        }
        public double Speed
        {
            get
            { return _speed; }
            set
            {
                _speed = value;
            }
        }
        public double VerticalAccuracy
        {
            get
            { return _verticalAccuracy; }
            set
            { _verticalAccuracy = value; }
        }
        public double AvgSpeed
        {
            get
            { return _avgSpeed; }
            set
            {
                _avgSpeed = value;
            }
        }
        public GeoPositionStatus GpsStatus
        {
            get
            { return _gpsStatus; }
            set
            { _gpsStatus = value; }
        }
        public bool IsLocationUnknown
        {
            get
            { return _isLocationUnknown; }
            set
            { _isLocationUnknown = value; }
        }
        public DateTime TimeStamp
        {
            get
            {
                return _timeStamp;
            }
            set
            {
                _timeStamp = value;
            }
        }

        #endregion



        public Sensors()
        {


            if (wtc == null)
            {
                //System.Diagnostics.Debug.WriteLine("wtc:"+GpsMovementThreshold);
                wtc = new GeoCoordinateWatcher(GeoPositionAccuracy.High);//using high accuracy
                wtc.MovementThreshold = GpsMovementThreshold;
                wtc.PositionChanged += wtc_PositionChanged; //+= new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(wtc_PositionChanged);
                wtc.StatusChanged += wtc_StatusChanged;
            }

            if (!Compass.IsSupported)
            {
                //Do something if the device does not support the compass sensor. 
            }
            else
            {
                if (cmp == null)
                {
                    //System.Diagnostics.Debug.WriteLine("cmp:"+UpdateCompassTimeSpan);
                    cmp = new Compass();
                    cmp.TimeBetweenUpdates = TimeSpan.FromMilliseconds(UpdateCompassTimeSpan);
                    cmp.CurrentValueChanged += cmp_CurrentValueChanged;
                    cmp.Calibrate += cmp_Calibrate;
                }
            }

        }


        public void Start()
        {
            wtc.Start();

            try
            {
                cmp.Start();
            }
            catch (InvalidOperationException)
            {
                //Do something
            }
        }
        public void Stop()
        {
            if (wtc != null)
                wtc.Stop();
            if (cmp != null)
                cmp.Stop();
        }



        void cmp_Calibrate(object sender, CalibrationEventArgs e)
        {
            //コンパスの方位精度が +/- 20°を超えていることがシステムによって検出された場合に発生
        }

        void cmp_CurrentValueChanged(object sender, SensorReadingEventArgs<CompassReading> e)
        {
            IsCompassDataValid = cmp.IsDataValid;
            TrueHeading = e.SensorReading.TrueHeading;
            MagneticHeading = e.SensorReading.MagneticHeading;
            HeadingAccuracy = e.SensorReading.HeadingAccuracy;
            //_rawMagnetometerReading = e.SensorReading.MagnetometerReading;

            CompassDataChangedEventArgs changedEvent = new CompassDataChangedEventArgs();
            OnCompassDataChanged(changedEvent);//イベントを発行する。
        }



        private Queue<double> speedRecorder_forCalcAvgSpeed = new Queue<double>();

        private void wtc_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            var gpsdata = e.Position.Location;

            Latitude = gpsdata.Latitude;
            Longitude = gpsdata.Longitude;
            Speed = gpsdata.Speed;
            if (double.IsNaN(Speed) == false)//キューの長さを常に10に保つ。
            {
                speedRecorder_forCalcAvgSpeed.Enqueue(Speed);
                if (speedRecorder_forCalcAvgSpeed.Count == 10)
                {
                    speedRecorder_forCalcAvgSpeed.Dequeue();
                }
            }

            Course = gpsdata.Course;
            Altitude = gpsdata.Altitude;
            HorizontalAccuracy = gpsdata.HorizontalAccuracy;
            VerticalAccuracy = gpsdata.VerticalAccuracy;
            AvgSpeed = calcAvgSpeed();
            IsLocationUnknown = gpsdata.IsUnknown;
            //System.Diagnostics.Debug.WriteLine(Speed);

            GPSDataChangedEventArgs changedEvent = new GPSDataChangedEventArgs();
            OnGPSDataChanged(changedEvent);//イベントを発行する。
        }
        private double calcAvgSpeed()
        {
            double sum = 0;
            int num = speedRecorder_forCalcAvgSpeed.Count;
            for (var i = 0; i < num; i++)
            {
                sum = sum + speedRecorder_forCalcAvgSpeed.ElementAt(i);
            }

            var avg = (sum + AvgSpeed) / (num + 1);

            return avg;
        }

        void wtc_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            GpsStatus = e.Status;

            GPSStatusChangedEventArgs changedEvent = new GPSStatusChangedEventArgs();
            OnGPSStatusChanged(changedEvent);//イベントを発行する。
        }


    }
}
