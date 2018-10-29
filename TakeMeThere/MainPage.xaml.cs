using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Device.Location;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Tasks;
using System.Windows;
using System.Globalization;
using System.Threading;


namespace TakeMeThere
{
    public partial class MainPage : PhoneApplicationPage
    {
        IsolatedStorageSettings isolateStore = IsolatedStorageSettings.ApplicationSettings;

        MySensors Sensor;//=new MySensors();
        DispatcherTimer Timer_for_Speed = new DispatcherTimer();
        DispatcherTimer Timer_for_Clock = new DispatcherTimer();
        DispatcherTimer Timer_for_Compass_Rotate = new DispatcherTimer();

        PushPinModel Target=null;
        double ETA=double.NaN;
        double Distance=double.NaN;
        double TargetDirection=double.NaN;
        ViewModel PushPinView=null;//=new ViewModel();


        //AppDB DB = new AppDB();

        double TimeSpanRefreshCompass_main;// 20;//ms。回転の機敏さ。値が小さいほど機敏に回る。20は十分に機敏。
        //(1/FactorToCorrectDirection)*TimeSpanRefreshCompassは回転にかかる時間を示すことになる。ms。200msを目安に。
        double FactorToCorrectDirection_main;// 10;//動きの滑らかさ。100なら100回かけて徐々に収束していく。10で十分なめらか。
        string PreferredEmail;

        bool PermissionOfLocationService;

        App MyApp;
        // コンストラクター
        public MainPage()
        {
            InitializeComponent();

            MyApp=Application.Current as App;
        }

        #region ページ遷移
        //メインページに移動してきた時の処理。
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine(this.Foreground);

            if ((bool)IsolatedStorageSettings.ApplicationSettings["FirstTimeMessagePop"] == false)
            {
                var result = MessageBox.Show("In order to display your location and provide optimal user experience, we need access to your current location.Your location information will not be stored or shared, and you can always disable this feature in the settings page. Do you want to enable access to location service?", "Location Service", MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    IsolatedStorageSettings.ApplicationSettings["LocationService"] = true;
                }
                IsolatedStorageSettings.ApplicationSettings["FirstTimeMessagePop"] = true;
            }

            PermissionOfLocationService = (bool)IsolatedStorageSettings.ApplicationSettings["LocationService"];

            //センサーとデータベースのインスタンス作成
            if (Sensor == null)
                Sensor = new MySensors();
            //if (DB == null)
            //    DB = new AppDB();
   

            Sensor.UserPermission = PermissionOfLocationService;


            //分離ストレージの設定情報読み込み
            TimeSpanRefreshCompass_main = (double)IsolatedStorageSettings.ApplicationSettings["TimeSpanRefreshCompass_main"];
            FactorToCorrectDirection_main = (double)IsolatedStorageSettings.ApplicationSettings["FactorToCorrectDirection_main"];
            PreferredEmail = (string)IsolatedStorageSettings.ApplicationSettings["PreferredEmail"];



            //センサー初期化
            Sensor.GpsMovementThreshold = (double)IsolatedStorageSettings.ApplicationSettings["DistanceUpdateGPS_sensor"];
            Sensor.UpdateCompassTimeSpan = (double)IsolatedStorageSettings.ApplicationSettings["TimeSpanUpdateCompass_sensor"];
            //Sensor.AvgSpeed = (double)IsolatedStorageSettings.ApplicationSettings["AverageSpeed"];
            //平均速度のリセット
            if ((bool)IsolatedStorageSettings.ApplicationSettings["AverageSpeedChanged"] == true)
            {
                Sensor.ResetAvgSpeed((double)IsolatedStorageSettings.ApplicationSettings["AverageSpeed"]);
                IsolatedStorageSettings.ApplicationSettings["AverageSpeedChanged"] = false;
            }
            else
            {
                Sensor.AvgSpeed = (double)IsolatedStorageSettings.ApplicationSettings["AverageSpeed"];
            } 
            Sensor.CompassDataChanged += sensor_CompassDataChanged;
            Sensor.GPSDataChanged += sensor_GPSDataChanged;
            Sensor.GPSStatusChanged += sensor_GPSStatusChanged;
            Sensor.Start();

            //タイマー初期化
            initTimer();

            //ピンデータの読み込み
            //PushPinView = DB.LoadInfoFromXML();
            PushPinView = MyApp.PushPinView;
            Target = PushPinView.GetTarget();




            //状態確認
            //GPS情報を受信しているか。statusがreadyかどうか。
            //されていなければ、現在地経緯度、目的地方位、目的地距離、ETA、所要時間は非表示
            //目的地は設定されているか。Targetがnullではないかどうか。
            //されていなければ、目的地名、目的地経緯度、目的地方位、目的地距離、ETA、所要時間は非表示
            initDisplay();

            base.OnNavigatedTo(e);
        }

        #region ページ遷移時の各種初期化
        private void initDisplay()
        {
            if (Target != null)
            {
                TextBlock_TargetName.Text = Target.Name;
                TextBlock_TargetLatitude.Text = Target.Location.Latitude.ToString();
                TextBlock_TargetLongitude.Text = Target.Location.Longitude.ToString();
            }
            else
            {
                TextBlock_TargetName.Text = "";
                TextBlock_TargetLatitude.Text = "";
                TextBlock_TargetLongitude.Text = "";
            }

            TextBlock_EstimateArrivalTime.Text = "";
            TextBlock_Distance.Text = "";
            TextBlock_Distance_Unit.Text = "m";
            TextBlock_ETA.Text = "";
            TextBlock_ETA_Unit.Text = "";
            Image_Arrow.Visibility = Visibility.Collapsed;
            TextBlock_EAT_Text.Visibility = Visibility.Collapsed;

            TextBlock_CurrentTime.Text = DateTime.Now.ToShortTimeString();
            TextBlock_DirectionSign.Text = "";
            TextBlock_AvgSpeed.Text = (Sensor.AvgSpeed * 3600 / 1000).ToString("F1");// Utility.ConvertSpeedWithUnit(Sensor.AvgSpeed);//(Sensor.AvgSpeed * 3600 / 1000).ToString("F1") + " km/h";
            TextBlock_CurrentLatitude.Text = "";
            TextBlock_CurrentLongitude.Text = "";
            TextBlock_GPSStatus.Text = "Disable";
            TextBlock_TrueHeading.Text = "";
            TextBlock_HorizontalAccuracy.Text = "";

            var _speedWithUnit = Utility.ConvertSpeedWithUnit(0);
            TextBlock_Speed.Text = _speedWithUnit.Substring(0, _speedWithUnit.IndexOf(" "));
            TextBlock_Speed_Unit.Text = _speedWithUnit.Substring(_speedWithUnit.IndexOf(" "));
        }


        private void initTimer()
        {
            #region タイマー初期化
            Timer_for_Speed.Interval = TimeSpan.FromMilliseconds(1000);
            Timer_for_Speed.Tick += timer_for_Speed_Tick;
            Timer_for_Speed.Start();

            Timer_for_Clock.Interval = TimeSpan.FromSeconds(1);
            Timer_for_Clock.Tick += timer_for_Clock_Tick;
            Timer_for_Clock.Start();

            Timer_for_Compass_Rotate.Interval = TimeSpan.FromMilliseconds(TimeSpanRefreshCompass_main);
            Timer_for_Compass_Rotate.Tick += Timer_for_Compass_Rotate_Tick;
            Timer_for_Compass_Rotate.Start();
            #endregion 
        }
        #endregion


        //メインページから離脱した時の処理。
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Sensor.Stop();
            Sensor.CompassDataChanged -= sensor_CompassDataChanged;
            Sensor.GPSDataChanged -= sensor_GPSDataChanged;
            Sensor.GPSStatusChanged -= sensor_GPSStatusChanged;

            turnOffTimer();
            //DB.SaveInfoToIsoStrage(PushPinView);



            isolateStore["AverageSpeed"] = Sensor.AvgSpeed;

            base.OnNavigatedFrom(e);
        }
        private void turnOffTimer()
        {       
            Timer_for_Speed.Stop();
            Timer_for_Speed.Tick -= timer_for_Speed_Tick;

            Timer_for_Clock.Stop();
            Timer_for_Clock.Tick -= timer_for_Clock_Tick;

            Timer_for_Compass_Rotate.Stop();
            Timer_for_Compass_Rotate.Tick -= Timer_for_Compass_Rotate_Tick;

        }

        #endregion


        #region 表示内容更新とそれに関するタイマー、イベント
        private void timer_for_Clock_Tick(object sender, EventArgs e)
        {
            refreshClock();
        }
        private void refreshClock()
        {
            TextBlock_CurrentTime.Text = DateTime.Now.ToShortTimeString();

            /*
            if (Sensor.GpsStatus != GeoPositionStatus.Ready)
            {
                //TextBlock_Speed.Text = "";
                return;
            }
            */

            if (double.IsNaN(ETA) == true)
            {
                //TextBlock_EstimateArrivalTime.Text = "";
            }
            else
            {
                if (Target != null && Sensor.Location.IsUnknown==false)
                {

                    TextBlock_EstimateArrivalTime.Text = (DateTime.Now + TimeSpan.FromSeconds((int)ETA)).ToShortTimeString();
                    Image_Arrow.Visibility = Visibility.Visible;
                    TextBlock_EAT_Text.Visibility = Visibility.Visible;
                }
                else
                {
                    TextBlock_EstimateArrivalTime.Text = "";
                    Image_Arrow.Visibility = Visibility.Collapsed;
                    TextBlock_EAT_Text.Visibility = Visibility.Visible;
                }
            }
        }


        Queue<double> speedRecorder_forSmoothSpeed = new Queue<double>();
        private void timer_for_Speed_Tick(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                refreshSpeed();
            });
        }
        private void refreshSpeed()
        {
            /*
            if (Sensor.GpsStatus != GeoPositionStatus.Ready)
            {
                //TextBlock_Speed.Text = "";
                return;
            }
            */

            if (double.IsNaN(Sensor.Speed) == false)//キューの長さを常に一定に保つ。
            {
                speedRecorder_forSmoothSpeed.Enqueue(Sensor.Speed);
                if (speedRecorder_forSmoothSpeed.Count == 5)
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
            if (avg < 0.027 || double.IsNaN(avg))//時速0.1kmの場合は0にする。
                avg = 0;

            var _speedWithUnit = Utility.ConvertSpeedWithUnit(avg);
            TextBlock_Speed.Text = _speedWithUnit.Substring(0, _speedWithUnit.IndexOf(" "));
            TextBlock_Speed_Unit.Text = _speedWithUnit.Substring(_speedWithUnit.IndexOf(" "));
            //TextBlock_Speed.Text = Utility.ConvertSpeedWithUnit(avg).Split(' ')[0];//(avg*3600/1000).ToString("F1");

        }

        


        private void sensor_GPSStatusChanged(object sender, GPSStatusChangedEventArgs e)
        {
            refreshGpsStatus();
        }
        private void refreshGpsStatus()
        {
            var status = Sensor.GpsStatus;

            switch (status)
            {
                case GeoPositionStatus.Disabled:
                    {
                        TextBlock_GPSStatus.Text = "Disable";
                        break;
                    }
                case GeoPositionStatus.Initializing:
                    {
                        TextBlock_GPSStatus.Text = "Initializing";
                        break;
                    }
                case GeoPositionStatus.NoData:
                    {
                        TextBlock_GPSStatus.Text = "NoData";
                        break;
                    }
                case GeoPositionStatus.Ready:
                    {
                        TextBlock_GPSStatus.Text = "Ready";
                        break;
                    }
            }
        }



        private void sensor_GPSDataChanged(object sender, GPSDataChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                    refreshGpsData();
            });
        }
        private void refreshGpsData()
        {
            if (Sensor.Location.IsUnknown == false)
            {
                TextBlock_CurrentLatitude.Text = Sensor.Latitude.ToString();
                TextBlock_CurrentLongitude.Text = Sensor.Longitude.ToString();
            }

            var _horizontalAccuracy = Utility.ConvertDistanceWithUnit(Utility.LimitAccuracy(Sensor.HorizontalAccuracy));
            TextBlock_HorizontalAccuracy.Text = "±" + _horizontalAccuracy.Substring(0,_horizontalAccuracy.IndexOf(" "));
            TextBlock_HorizontalAccuracy_Unit.Text = _horizontalAccuracy.Substring(_horizontalAccuracy.IndexOf(" "));

            TextBlock_AvgSpeed.Text = (Sensor.AvgSpeed * 3600 / 1000).ToString("F1");//Utility.ConvertSpeedWithUnit(Sensor.AvgSpeed);//(Sensor.AvgSpeed * 3600 / 1000).ToString("F1") + " km/h";


            if (Target == null)
                return;

            TextBlock_TargetName.Text = Target.Name;
            TextBlock_TargetLatitude.Text = Target.Location.Latitude.ToString();
            TextBlock_TargetLongitude.Text = Target.Location.Longitude.ToString();


            Distance = Utility.CalcDistanceTo_Hubeny(Sensor.Location, Target.Location);
            if (double.IsNaN(Distance))
            {
                //TextBlock_Distance.Text = "";
            }
            else
            {
                var _distanceWithUnit = Utility.ConvertDistanceWithUnit(Distance); 
                TextBlock_Distance.Text = _distanceWithUnit.Substring(0, _distanceWithUnit.IndexOf(" "));
                TextBlock_Distance_Unit.Text = _distanceWithUnit.Substring(_distanceWithUnit.IndexOf(" "));
            }

            ETA = Utility.CalcETA(Distance, Sensor.AvgSpeed);
            if (double.IsNaN(ETA) == true)
            {
                //TextBlock_ETA.Text = "--";
            }
            else
            {
                var _etaWithUnit = Utility.ConvertETAWithUnit(ETA); //System.Diagnostics.Debug.WriteLine(_etaWithUnit);
                TextBlock_ETA.Text = _etaWithUnit.Substring(0, _etaWithUnit.IndexOf(" "));
                TextBlock_ETA_Unit.Text = _etaWithUnit.Substring(_etaWithUnit.IndexOf(" "));
            }



            //TextBlock_Altitude.Text = sensor.Altitude.ToString();
            //IsLocationUnknown.Text = sensor.IsLocationUnknown.ToString();
            //Course.Text = sensor.Course.ToString();
            //Speed.Text = sensor.Speed.ToString();
            //TextBlock_Distance.Text = Distance.ToString();
            //VerticalAccuracy.Text = sensor.VerticalAccuracy.ToString();


        }


        private void sensor_CompassDataChanged(object sender, CompassDataChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                refreshCompassData();
            });
        }
        private void refreshCompassData()
        {
            //HeadingAccuracy.Text = sensor.HeadingAccuracy.ToString();
            //MagneticHeading.Text = sensor.MagneticHeading.ToString();
            //TextBlock_TrueHeading.Text = Sensor.TrueHeading.ToString();
            //TextBlock_DirectionSign.Text = Utility.ConvertDegreeToSign(Sensor.TrueHeading);
            //IsCompassDataValid.Text = sensor.IsCompassDataValid.ToString();

            if (Sensor.Location.IsUnknown==true)
                return;
            if (Target == null)
                return;


            var _tDir = Utility.CalcTargetDirection(Sensor.Location, Target.Location);
            if (double.IsNaN(_tDir) == true)
            {
                TargetDirection = 0;
            }
            else
            {
                TargetDirection = _tDir;
            }

        }

        #endregion


        #region コンパス更新用タイマー起動イベントハンドラ
        //磁気コンパス用
        double CorrectDirection;
        double CorrectTargetDirection;
        //経緯度の変化量から求める方角用
        double CorrectDirection2;
        double CorrectTargetDirection2;

        void Timer_for_Compass_Rotate_Tick(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                //コンパスの枠をなめらかに動かすための処理。（大きいほう。磁気方位）
                double sub_direction = Sensor.TrueHeading - CorrectDirection;

                if (sub_direction < -180)
                    sub_direction += 360;
                if (180 < sub_direction)
                    sub_direction -= 360;

                CorrectDirection = sub_direction * 1 / FactorToCorrectDirection_main + CorrectDirection;

                if (360 <= CorrectDirection)
                    CorrectDirection -= 360;
                if (CorrectDirection < 0)
                    CorrectDirection += 360;

                DialRotate.Angle = -CorrectDirection;

                TextBlock_TrueHeading.Text = (CorrectDirection).ToString("F0") + "°";
                TextBlock_DirectionSign.Text = Utility.ConvertDegreeToSign(CorrectDirection);


                //コンパスの枠をなめらかに動かすための処理。（小さいほう。経緯度の移動量から求める方位）
                if (double.IsNaN(Sensor.Course) == false)
                {
                    double sub_direction2 = Sensor.Course - CorrectDirection2;

                    if (sub_direction2 < -180)
                        sub_direction2 += 360;
                    if (180 < sub_direction2)
                        sub_direction2 -= 360;

                    CorrectDirection2 = sub_direction2 * 1 / FactorToCorrectDirection_main + CorrectDirection2;

                    if (360 <= CorrectDirection2)
                        CorrectDirection2 -= 360;
                    if (CorrectDirection2 < 0)
                        CorrectDirection2 += 360;

                    CourseDialRotate.Angle = -CorrectDirection2;
                }

                //目的地を指す針はtargetが設定されているときだけ動かす
                if (Target == null)
                    return;
                if (double.IsNaN(TargetDirection) == true)
                    return;

                //目的地針をなめらかに動かすために
                double sub_target_direction = TargetDirection - CorrectTargetDirection;
                CorrectTargetDirection = sub_target_direction * 1 / FactorToCorrectDirection_main + CorrectTargetDirection;

                TargetRotate.Angle = CorrectTargetDirection - CorrectDirection;

                //目的地針をなめらかに動かすために
                double sub_target_direction2 = TargetDirection - CorrectTargetDirection2;
                CorrectTargetDirection2 = sub_target_direction2 * 1 / FactorToCorrectDirection_main + CorrectTargetDirection2;

                CourseTargetRotate.Angle = CorrectTargetDirection2 - CorrectDirection2;

            });
        }


        #endregion




        #region アプリケーションバー
        private void Appbar_MapButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MapPage_gMap.xaml", UriKind.Relative));
        }

        private void Appbar_ListButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/ListPage.xaml?" , UriKind.Relative));
        }


        private void AppbarMenu_SensorInfo_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SensorDataPage.xaml", UriKind.Relative));
        }
        private void AppbarMenu_Settings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingPage.xaml", UriKind.Relative));
        }

        #endregion




        #region コンテキストメニュー
        private void TargetLocationContextMenuItemEmail_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (Target == null)
                return;

            string etaString;

            if (double.IsNaN(ETA) == true)
            {
                etaString = "--";
            }
            else
            {
                etaString = (DateTime.Now + TimeSpan.FromSeconds((int)ETA)).ToLongTimeString();
            }

            CultureInfo cc = Thread.CurrentThread.CurrentCulture;
            string body = "" + System.Environment.NewLine + System.Environment.NewLine
                + "To: " + System.Environment.NewLine
                + " Name: " + Target.Name + System.Environment.NewLine
                + " Location: " + Target.Location.ToString() + System.Environment.NewLine
                + " Address: " + Target.Address + System.Environment.NewLine
                + " Distance: " + Utility.ConvertDistanceWithUnit(Distance) + System.Environment.NewLine
                + " ETA: " + etaString + "(" + Utility.ConvertETAWithUnit(ETA) + ")" + System.Environment.NewLine
                + "Link: " + string.Format("http://maps.google.com/maps?q={0},{1}&hl={2}", Target.Location.Latitude, Target.Location.Longitude, cc.ToString())
                + System.Environment.NewLine
                + "From: " + System.Environment.NewLine
                + " Location: " + Sensor.Location.ToString() + System.Environment.NewLine
                + " Date/Time: " + DateTime.Now.ToString() + System.Environment.NewLine
                + "Link: " + string.Format("http://maps.google.com/maps?q={0},{1}&hl={2}", Sensor.Latitude, Sensor.Longitude, cc.ToString())
                + "Phone: " + Target.PhoneNum + System.Environment.NewLine
                + System.Environment.NewLine;


            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = Target.Name;
            emailComposeTask.Body = body;
            emailComposeTask.To = PreferredEmail;
            emailComposeTask.Show();
        }

        private void TargetLocationContextMenuItemSMS_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (Target == null)
                return;

            string etaString;

            if (double.IsNaN(ETA) == true)
            {
                etaString = "--";
            }
            else
            {
                etaString = (DateTime.Now + TimeSpan.FromSeconds((int)ETA)).ToLongTimeString();
            }

            string body = "" + System.Environment.NewLine + System.Environment.NewLine
                + "To: " + System.Environment.NewLine
                + " Name: " + Target.Name + System.Environment.NewLine
                + " Location: " + Target.Location.ToString() + System.Environment.NewLine
                + " Address: " + Target.Address + System.Environment.NewLine
                + " Distance: " + Utility.ConvertDistanceWithUnit(Distance) + System.Environment.NewLine
                + " ETA: " + etaString + "(" + Utility.ConvertETAWithUnit(ETA) + ")" + System.Environment.NewLine
                + System.Environment.NewLine
                + "From: " + System.Environment.NewLine
                + " Location: " + Sensor.Location.ToString() + System.Environment.NewLine
                + " Date/Time: " + DateTime.Now.ToString() + System.Environment.NewLine
                                + "Phone: " + Target.PhoneNum + System.Environment.NewLine
                + System.Environment.NewLine;

            SmsComposeTask smsComposeTask = new SmsComposeTask();
            smsComposeTask.To = "";
            smsComposeTask.Body = body;
            smsComposeTask.Show();
        }

        private void TargetLocationContextMenuItemShare_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (Target == null)
                return;

            string etaString;

            if (double.IsNaN(ETA) == true)
            {
                etaString = "--";
            }
            else
            {
                etaString = (DateTime.Now + TimeSpan.FromSeconds((int)ETA)).ToLongTimeString();
            }

            CultureInfo cc = Thread.CurrentThread.CurrentCulture;
            string body = "" + System.Environment.NewLine + System.Environment.NewLine
                + "To: " + System.Environment.NewLine
                + " Name: " + Target.Name + System.Environment.NewLine
                + " Location: " + Target.Location.ToString() + System.Environment.NewLine
                + " Address: " + Target.Address + System.Environment.NewLine
                + " Distance: " + Utility.ConvertDistanceWithUnit(Distance) + System.Environment.NewLine
                + " ETA: " + etaString + "(" + Utility.ConvertETAWithUnit(ETA) + ")" + System.Environment.NewLine
                + "Link: " + string.Format("http://maps.google.com/maps?q={0},{1}&hl={2}", Target.Location.Latitude, Target.Location.Longitude, cc.ToString())
                + System.Environment.NewLine
                + "From: " + System.Environment.NewLine
                + " Location: " + Sensor.Location.ToString() + System.Environment.NewLine
                + " Date/Time: " + DateTime.Now.ToString() + System.Environment.NewLine
                + "Link: " + string.Format("http://maps.google.com/maps?q={0},{1}&hl={2}", Sensor.Latitude, Sensor.Longitude, cc.ToString())
                                + "Phone: " + Target.PhoneNum + System.Environment.NewLine
                + System.Environment.NewLine;


            ShareStatusTask shareStatusTask = new ShareStatusTask();
            shareStatusTask.Status = body;
            shareStatusTask.Show();
        }




        private void CurrentLocationContextMenuItemEmail_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            /*
            if (Sensor.GpsStatus != GeoPositionStatus.Ready)
                return;
            */
            if (Sensor.Location.IsUnknown)
                return;

            CultureInfo cc = Thread.CurrentThread.CurrentCulture;
            string body = "" + System.Environment.NewLine + System.Environment.NewLine
                + "I'm Here Now: " + System.Environment.NewLine
                + " Location: " + Sensor.Location.ToString() + System.Environment.NewLine
                + " Date/Time: " + DateTime.Now.ToString() + System.Environment.NewLine
                + "Link: " + string.Format("http://maps.google.com/maps?q={0},{1}&hl={2}", Sensor.Latitude, Sensor.Longitude, cc.ToString())
                + System.Environment.NewLine;


            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = "I'm Here Now.";
            emailComposeTask.Body = body;
            emailComposeTask.To = PreferredEmail;
            emailComposeTask.Show();
        }

        private void CurrentLocationContextMenuItemSMS_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            /*
            if (Sensor.GpsStatus != GeoPositionStatus.Ready)
                return;
            */
            if (Sensor.Location.IsUnknown)
                return;

            string body = "" + System.Environment.NewLine + System.Environment.NewLine
                + "I'm Here Now: " + System.Environment.NewLine
                + " Location: " + Sensor.Location.ToString() + System.Environment.NewLine
                + " Date/Time: " + DateTime.Now.ToString() + System.Environment.NewLine
                + System.Environment.NewLine;


            SmsComposeTask smsComposeTask = new SmsComposeTask();
            smsComposeTask.To = "";
            smsComposeTask.Body = body;
            smsComposeTask.Show();
        }

        private void CurrentLocationContextMenuItemShare_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            /*
            if (Sensor.GpsStatus != GeoPositionStatus.Ready)
                return;
            */
            if (Sensor.Location.IsUnknown)
                return;

            CultureInfo cc = Thread.CurrentThread.CurrentCulture;
            string body = "" + System.Environment.NewLine + System.Environment.NewLine
                + "I'm Here Now: " + System.Environment.NewLine
                + " Location: " + Sensor.Location.ToString() + System.Environment.NewLine
                + " Date/Time: " + DateTime.Now.ToString() + System.Environment.NewLine
                + "Link: " + string.Format("http://maps.google.com/maps?q={0},{1}&hl={2}", Sensor.Latitude, Sensor.Longitude, cc.ToString())
                + System.Environment.NewLine;

            ShareStatusTask shareStatusTask = new ShareStatusTask();
            shareStatusTask.Status = body;
            shareStatusTask.Show();

        }

        private void CurrentLocationContextMenuItemPintoMap_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            /*
            if (Sensor.GpsStatus != GeoPositionStatus.Ready)
                return;
            */
            if (Sensor.Location.IsUnknown)
                return;
            PushPinModel pin = new PushPinModel
            {
                Location = Sensor.Location, //new GeoCoordinate(holdLocation.Latitude, holdLocation.Longitude),
                Name = "New Place",
                Color = new SolidColorBrush(Utility.GetColorFromHexString(PushPinView.DefaultColor_Hex)),
                Note = "",
                IsEnabled = true,
                Selected = false,
                Target = false,
                Visibility = System.Windows.Visibility.Visible,
                CreateDate=DateTime.Now

            };

            bool darkTheme = ((Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible);
            if (darkTheme == true)//darkの時は白抜きの画像
            {
                pin.ArrowURI = "/Icons/appbar.next.rest.png";
                pin.CircleURI = "/Icons/appbar.basecircle.rest.png";
            }
            else
            {
                pin.ArrowURI = "/Icons/appbar.next.rest.light.png";
                pin.CircleURI = "/Icons/appbar.base.png";
            }


            PushPinView.PushPins.Add(pin);
            //DB.SaveInfoToIsoStrage(PushPinView);
        }
        #endregion



        private void Grid_GoTargetLocation_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (Target == null)
                return;

            var location = Target.Location;
            if (location.IsUnknown)
            {
                return;
            }

            NavigationService.Navigate(new Uri("/MapPage_gMap.xaml?" + "Latitude=" + location.Latitude.ToString() + "&Longitude=" + location.Longitude.ToString(), UriKind.Relative));

        }

        private void Grid_GoCurrentLocaion_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //if (Sensor.GpsStatus != GeoPositionStatus.Ready)
              //  return;

            //var location = Sensor.Location;
            if (Sensor.Location.IsUnknown)
            {
                return;
            }

            NavigationService.Navigate(new Uri("/MapPage_gMap.xaml?" + "Latitude=" + Sensor.Location.Latitude.ToString() + "&Longitude=" + Sensor.Location.Longitude.ToString(), UriKind.Relative));

        }


        private void Grid_TargetLocation_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (Target == null)
                return;

            PushPinView.SetSelected(Target);

            NavigationService.Navigate(new Uri("/DetailPage.xaml?EditingType=Modify", UriKind.Relative));
        }



        private void AppbarMenu_ApplicationInfo_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AppInfoPage.xaml", UriKind.Relative));
        }

        //GameクラスのExitメソッドを使って強制終了する。
        private void AppbarMenu_ExitApp_Click(object sender, EventArgs e)
        {
            var game = new Microsoft.Xna.Framework.Game();

            game.Exit();

        }


    }
}