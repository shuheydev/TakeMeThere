using Microsoft.Devices;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Net.NetworkInformation;
using System;
using System.ComponentModel;
using System.Device.Location;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Xml.Linq;

namespace TakeMeThere
{
    public class TwoPoint//二地点間の直線距離を表示するために使用するデータ格納用クラス
    {
        public PushPinModel Point1;
        public PushPinModel Point2;

        //コンストラクタ
        public TwoPoint()
        {
            Point1 = new PushPinModel();
            Point2 = new PushPinModel();
        }

        public void Add(PushPinModel pin)
        {
            if (Point1.Location.IsUnknown == true)
                Point1 = pin;
            else if (Point2.Location.IsUnknown == true)
                Point2 = pin;
            else
            {
                Point1 = Point2;
                Point2 = pin;
            }
        }

        public void Clear()
        {
            Point1 = new PushPinModel();
            Point2 = new PushPinModel();
        }

        //2点間の距離を計算
        public double Distance
        {
            get
            {
                //return Utility.CalcDistanceTo_Hubeny(this.Point1, this.Point2);
                double distance;
                if (this.Point1.Location.IsUnknown == true || this.Point2.Location.IsUnknown == true)
                    distance = 0;
                else
                    distance = Utility.CalcDistanceTo_Hubeny(Point1.Location, Point2.Location);
                return distance;
            }

        }
    }


    public partial class MapPage_gMap : PhoneApplicationPage
    {
        public string CandidateColor_Hex = "99825A2C";

        IsolatedStorageSettings isolateStore = IsolatedStorageSettings.ApplicationSettings;

        DispatcherTimer refreshCompassTimer = new DispatcherTimer();

        double TimeSpanRefreshCompass_map;// 20;//ms。回転の機敏さ。値が小さいほど機敏に回る。20は十分に機敏。
        double FactorToCorrectDirection_map;//動きの滑らかさ。100なら100回かけて徐々に収束していく。10で十分なめらか。
        //(1/FactorToCorrectDirection)*TimeSpanRefreshCompassは回転にかかる時間を示すことになる。ms。200msを目安に。

        string MapKind;

        MySensors Sensor;// = new MySensors();
        ViewModel PushPinView;// = new ViewModel();
        //AppDB DB;// = new AppDB();
        
        Pushpin candidatePin = new Pushpin();



        //現在地と目的地を結ぶ線
        private MapPolyline LineToTarget;

        private MapPolyline LineTwoPoint;

        private string LineColor_Hex="55E51400";
        private string LineColor_Hex_2 = "551BA1E2";

        PushPinModel Target=null;
        double ETA = double.NaN;
        double Distance = double.NaN;

        TwoPoint TP;

        bool PermissionOfLocationService;


        App MyApp;
        public MapPage_gMap()
        {
            InitializeComponent();


            MyApp = Application.Current as App;


            //現在地とターゲットを結ぶ直線の初期化
            LineToTarget = new MapPolyline()
            {
                Stroke = new SolidColorBrush(Utility.GetColorFromHexString(LineColor_Hex)),
                StrokeThickness = 3,
                Locations = new LocationCollection()
            };

            //タップした二点間を結ぶ直線の初期化
            LineTwoPoint = new MapPolyline()
            {
                Stroke = new SolidColorBrush(Utility.GetColorFromHexString(LineColor_Hex_2)),
                StrokeThickness = 3,
                Locations = new LocationCollection()
            };


        }


        //ページに移動してきた時の処理。
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Grid_SearchBox.Visibility = Visibility.Collapsed;

            PermissionOfLocationService = (bool)IsolatedStorageSettings.ApplicationSettings["LocationService"];
            if (Sensor == null)
                Sensor = new MySensors();
            //if (DB == null)
            //    DB = new AppDB();

            Sensor.UserPermission = PermissionOfLocationService;

            //現在地マークの初期化
            CurrentMark.PositionOrigin = PositionOrigin.Center;
            CurrentMark.Visibility = Visibility.Collapsed;

            //候補地ピンの初期化。配置はしない。         
            candidatePin.Content = new TextBlock() { Text = "", Margin = new Thickness(3) };
            var candidate_color = Utility.GetColorFromHexString(CandidateColor_Hex);
            candidatePin.Background = new SolidColorBrush(candidate_color);




            LineTwoPoint.Locations.Clear();
            TP = new TwoPoint();
            Grid_TwoPoint_Distance.Visibility = Visibility.Collapsed;
            TP.Clear();
            refreshTwoPointLine();

            MapKind = (string)IsolatedStorageSettings.ApplicationSettings["Map"];
            if (MapKind == "gMap")
                MapTileLayer_gMap.Width = 480;
            else
                MapTileLayer_gMap.Width = 0;

            if ((bool)isolateStore["MapPageLockPin"] == true)
            {
                TextBlock_LockPin.Text = "Lock";
                MapItemsControl_PushPins.IsEnabled = false;
            }
            else
            {
                TextBlock_LockPin.Text = "Unlock";
                MapItemsControl_PushPins.IsEnabled = true;
            }

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

            DeviceNetworkInformation.NetworkAvailabilityChanged += new EventHandler<NetworkNotificationEventArgs>(DeviceNetworkInformation_NetworkAvailabilityChanged);
            TimeSpanRefreshCompass_map = (double)IsolatedStorageSettings.ApplicationSettings["TimeSpanRefreshCompass_map"];
            FactorToCorrectDirection_map = (double)IsolatedStorageSettings.ApplicationSettings["FactorToCorrectDirection_map"];

            Sensor.GpsMovementThreshold = (double)IsolatedStorageSettings.ApplicationSettings["DistanceUpdateGPS_sensor"];
            Sensor.UpdateCompassTimeSpan = (double)IsolatedStorageSettings.ApplicationSettings["TimeSpanUpdateCompass_sensor"];

            Sensor.GPSDataChanged += Sensor_GPSDataChanged;
            Sensor.GPSStatusChanged += Sensor_GPSStatusChanged;
            Sensor.CompassDataChanged += Sensor_CompassDataChanged;
            Sensor.Start();

            //PushPinView = DB.LoadInfoFromXML();
            PushPinView = MyApp.PushPinView;
            Target = PushPinView.GetTarget();

            //プロパティ変更イベントハンドラを設定
            foreach (var item in PushPinView.PushPins)
            {
                item.PropertyChanged += new PropertyChangedEventHandler(Pin_PropertyChanged);
            }
            DataContext = PushPinView;


            var centerLocation = new GeoCoordinate();
            double currentZoomLevel;

            if (NavigationContext.QueryString.Count > 0 && e.NavigationMode == NavigationMode.New)
            {
                //System.Diagnostics.Debug.WriteLine(NavigationContext.QueryString.ToString());
                centerLocation.Latitude = Convert.ToDouble(NavigationContext.QueryString["Latitude"]);
                centerLocation.Longitude = Convert.ToDouble(NavigationContext.QueryString["Longitude"]);
                currentZoomLevel = MyMap.ZoomLevel;
            }
            else
            {
                centerLocation.Latitude = (double)isolateStore["CenterLatitude"];
                centerLocation.Longitude = (double)isolateStore["CenterLongitude"];
                currentZoomLevel = (double)isolateStore["ZoomLevel"];
            }




            MyMap.SetView(centerLocation, currentZoomLevel);


            initTimer();

            initDisplay();

            refreshLine();
            refreshTwoPointLine();

            base.OnNavigatedTo(e);
        }
        private void initTimer()
        {
            refreshCompassTimer.Interval = TimeSpan.FromMilliseconds(TimeSpanRefreshCompass_map);
            refreshCompassTimer.Tick += refreshTimer_Tick;
            refreshCompassTimer.Start();
        }

        private void initDisplay()
        {
            if (Target != null)
            {
                //refreshLine();
                TextBlock_TargetName.Text = Target.Name;
                //refreshDistanceTextBlock();
                if (PermissionOfLocationService == false)
                {
                    TextBlock_Distance.Text = "";
                    TextBlock_Distance_Unit.Text = "";
                    TextBlock_ETA.Text = "";
                    TextBlock_ETA_Unit.Text = "";

                }
                TextBlock_AvgSpeed.Text = Utility.ConvertSpeedWithUnit(Sensor.AvgSpeed);//(Sensor.AvgSpeed * 3600 / 1000).ToString("F1");

            }
            else
            {
                TextBlock_TargetName.Text = "";
                //TextBlock_Distance.Text = "";
                //TextBlock_Distance_Unit.Text = "";
                TextBlock_ETA.Text = "";
                TextBlock_ETA_Unit.Text = "";
                TextBlock_AvgSpeed.Text = Utility.ConvertSpeedWithUnit(Sensor.AvgSpeed);//(Sensor.AvgSpeed * 3600 / 1000).ToString("F1");
            }

        }



        //メインページから離脱した時の処理。
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Sensor.Stop();
            Sensor.GPSDataChanged -= Sensor_GPSDataChanged;
            Sensor.GPSStatusChanged -= Sensor_GPSStatusChanged;
            Sensor.CompassDataChanged -= Sensor_CompassDataChanged;

            turnOffTimer();

            //DB.SaveInfoToIsoStrage(PushPinView);

            isolateStore["ZoomLevel"] = MyMap.ZoomLevel;
            isolateStore["CenterLatitude"] = MyMap.Center.Latitude;
            isolateStore["CenterLongitude"] = MyMap.Center.Longitude;
            isolateStore["AverageSpeed"] = Sensor.AvgSpeed;

            base.OnNavigatedFrom(e);
        }
        private void turnOffTimer()
        {
            refreshCompassTimer.Stop();
            refreshCompassTimer.Tick -= refreshTimer_Tick;
        }


        private void Sensor_CompassDataChanged(object sender, CompassDataChangedEventArgs e)
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
            //IsCompassDataValid.Text = sensor.IsCompassDataValid.ToString();
        }

        private void Sensor_GPSStatusChanged(object sender, GPSStatusChangedEventArgs e)
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

        void Sensor_GPSDataChanged(object sender, GPSDataChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {

                if (Sensor.Location.IsUnknown == false)
                {
                    CurrentMark.Location = Sensor.Location;
                    if (CurrentMark.Visibility == Visibility.Collapsed)
                        CurrentMark.Visibility = Visibility.Visible;
                }
                else
                {
                    return;
                }

                if (Target != null)
                {
                    Distance = Utility.CalcDistanceTo_Hubeny(Sensor.Location, Target.Location);
                    ETA = Utility.CalcETA(Distance, Sensor.AvgSpeed);
                }


                refreshDistanceTextBlock();
                refreshLine();
            
            });

        }




        void DeviceNetworkInformation_NetworkAvailabilityChanged(object sender, NetworkNotificationEventArgs e)
        {
            if (DeviceNetworkInformation.IsNetworkAvailable == true)
            {
                LineToTarget.Stroke = new SolidColorBrush(Utility.GetColorFromHexString(LineColor_Hex));
            }
            else if (DeviceNetworkInformation.IsNetworkAvailable == false)
            {
                LineToTarget.Stroke = new SolidColorBrush(Utility.GetColorFromHexString("FFFFFFFF"));

            }
        }



        #region 画面情報の再描画関連



        private void refreshLine()
        {
            if (Sensor.Location.IsUnknown == true || Target == null || PermissionOfLocationService==false)
            {
                if (MyMap.Children.Contains(LineToTarget) == true)
                {
                    MyMap.Children.Remove(LineToTarget);
                }
                return;
            }

            LineToTarget.Locations.Clear();
            LineToTarget.Locations.Add(Sensor.Location);
            LineToTarget.Locations.Add(Target.Location);
            if (MyMap.Children.Contains(LineToTarget) == false)
                MyMap.Children.Add(LineToTarget);

        }

        private void refreshTwoPointLine()//タップした二つのピンを直線で結ぶ
        {
            if (TP.Point2.Location.IsUnknown == true)//二地点がタップされていなければ、線を引かない。
            {
                MyMap.Children.Remove(LineTwoPoint);
                Grid_TwoPoint_Distance.Visibility = Visibility.Collapsed;
                return;
            }
            if (TP.Point1.Location == TP.Point2.Location)//同じ地点が二回選択されても線は引かない。
            {
                MyMap.Children.Remove(LineTwoPoint);
                Grid_TwoPoint_Distance.Visibility = Visibility.Collapsed;
                return;
            }

            //距離を表示するグリッドをvisibleにする
            Grid_TwoPoint_Distance.Visibility = Visibility.Visible;
            var _distanceWithUnit = Utility.ConvertDistanceWithUnit(TP.Distance);
            TextBlock_TwoPoint_Distance.Text = _distanceWithUnit.Substring(0, _distanceWithUnit.IndexOf(" "));
            TextBlock_TwoPoint_Distance_Unit.Text = _distanceWithUnit.Substring(_distanceWithUnit.IndexOf(" "));

            //二地点の位置情報をセット
            LineTwoPoint.Locations.Clear();
            LineTwoPoint.Locations.Add(TP.Point1.Location);
            LineTwoPoint.Locations.Add(TP.Point2.Location);

            //マップに追加。
            if (MyMap.Children.Contains(LineTwoPoint))
                MyMap.Children.Remove(LineTwoPoint);
            MyMap.Children.Add(LineTwoPoint);

        }


        private void refreshDistanceTextBlock()
        {
            TextBlock_AvgSpeed.Text = Utility.ConvertSpeedWithUnit(Sensor.AvgSpeed);//(Sensor.AvgSpeed * 3600 / 1000).ToString("F1");

            if (Sensor.Location.IsUnknown == true || Target == null)
            {
                TextBlock_Distance.Text = "";
                TextBlock_Distance_Unit.Text = "";
                TextBlock_ETA.Text = "";
                TextBlock_ETA_Unit.Text = "";
                return;
            }





            if (double.IsNaN(Distance)==true)
            {
                TextBlock_Distance.Text = "";
            }
            else
            {
                var _distanceWithUnit = Utility.ConvertDistanceWithUnit(Distance);
                TextBlock_Distance.Text = _distanceWithUnit.Substring(0, _distanceWithUnit.IndexOf(" "));
                TextBlock_Distance_Unit.Text = _distanceWithUnit.Substring(_distanceWithUnit.IndexOf(" "));
            }

            if (double.IsNaN(ETA) == true)
            {
                TextBlock_ETA.Text = "";
            }
            else
            {
                var _etaWithUnit = Utility.ConvertETAWithUnit(ETA);
                TextBlock_ETA.Text = _etaWithUnit.Substring(0, _etaWithUnit.IndexOf(" "));
                TextBlock_ETA_Unit.Text = _etaWithUnit.Substring(_etaWithUnit.IndexOf(" "));
                //TextBlock_AvgSpeed.Text =(Sensor.AvgSpeed*3600/1000).ToString("F1");
            }
        }



        

        //現在地マークを回転させる処理
        double CorrectDirection;

        void refreshTimer_Tick(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                //コンパスをなめらかに動かすための処理。
                double sub_direction = Sensor.TrueHeading - CorrectDirection;

                if (sub_direction < -180)
                    sub_direction += 360;
                if (180 < sub_direction)
                    sub_direction -= 360;

                CorrectDirection = sub_direction * 1/FactorToCorrectDirection_map + CorrectDirection;

                if (360 <= CorrectDirection)
                    CorrectDirection -= 360;
                if (CorrectDirection < 0)
                    CorrectDirection += 360;

                CurrentMarkRotate.Angle = CorrectDirection;
            });

        }

        #endregion




        #region ピンのドラッグアンドドロップ操作

        Point PushPin_Point;// = new Point();
        GeoCoordinate PushPin_GeoCoordinate;// = new GeoCoordinate();
        PushPinModel DraggingPin;
        string PinName;
        private void Pushpin_OnDragStarted(object sender, DragStartedGestureEventArgs e)
        {

            MyMap.IsEnabled = false; // prevents the map from dragging w/ pushpin}
    
            #region 変更前のコード
            /*
            Pushpin draggingPin = sender as Pushpin;

            //MessageBox.Show(draggingPin.Parent.ToString());

            Point p = MyMap.LocationToViewportPoint(draggingPin.Location);
            PushPin_Point.X = p.X;
            PushPin_Point.Y = p.Y;

            var pins = from pin in PushPinViewModel.PushPins
                       where pin.Location == (sender as Pushpin).Location
                       select pin;
            if (pins.Count() != 0)
            {
                pin = pins.First();
            }
             */
            #endregion
          
            //これをnewしておくと、ピンがかさなっていても一緒に動かない。
            //ただし、移動量は相変わらず重なっているピンの数に比例して2倍、3倍となる。
            PushPin_Point = new Point();
            PushPin_GeoCoordinate = new GeoCoordinate();
            DraggingPin = new PushPinModel();
         
            DraggingPin = (sender as Pushpin).DataContext as PushPinModel;
            //DraggingPin.Selected = true;
            PinName = DraggingPin.Name;
            Point p = MyMap.LocationToViewportPoint(DraggingPin.Location);
            PushPin_Point.X = p.X+80;
            PushPin_Point.Y = p.Y-120;

        }

        private void Pushpin_OnDragDelta(object sender, DragDeltaGestureEventArgs e)
        {
            #region 変更前のコード
            /*
                PushPin_Point.X += e.HorizontalChange;
                PushPin_Point.Y += e.VerticalChange;

                PushPin_GeoCoordinate = MyMap.ViewportPointToLocation(PushPin_Point);

                Pushpin draggingPin = sender as Pushpin;
                draggingPin.Location = PushPin_GeoCoordinate;


                (draggingPin.Content as TextBlock).Text = PushPin_GeoCoordinate.ToString();
        
             */
            #endregion
          
            PushPin_Point.X += e.HorizontalChange;
            PushPin_Point.Y += e.VerticalChange;

            PushPin_GeoCoordinate = MyMap.ViewportPointToLocation(PushPin_Point);

            DraggingPin.Location = PushPin_GeoCoordinate;
            DraggingPin.Name = (sender as Pushpin).Location.ToString();//表示の変更をしないと、移動途中が描画されないので経度緯度を表示させることに。

        }


        private void Pushpin_OnDragCompleted(object sender, DragCompletedGestureEventArgs e)
        {      
            DraggingPin.Location = PushPin_GeoCoordinate;

            DraggingPin.Name = PinName;//移動中に表示させていた経緯度を消す。
            MyMap.IsEnabled = true;
            
           //DB.SaveInfoToIsoStrage(PushPinView);
        }

        #endregion





        private bool pin_tap_flag = false;
        private bool pin_hold_flag = false;

        #region マップのタッチイベントハンドラ

        //マップをタップ
        private void MyMap_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (pin_tap_flag == false)
            {


                PushPinView.ClearSelected();
                TP.Clear();
                refreshTwoPointLine();

                //候補地ピンの座標を設定
                candidatePin.Location = MyMap.ViewportPointToLocation(e.GetPosition(MyMap));

                //MyMap.SetView(candidatePin.Location, MyMap.ZoomLevel);

                if (MyMap.Children.Contains(candidatePin) == false)
                {
                    MyMap.Children.Add(candidatePin);
                }
            }

            pin_tap_flag = false;
        }

        private void AddNewPin(GeoCoordinate location)
        {
            PushPinModel pin = new PushPinModel();
            pin.Location = location; //new GeoCoordinate(holdLocation.Latitude, holdLocation.Longitude),
            pin.Name = "New Place";
            pin.Color = new SolidColorBrush(Utility.GetColorFromHexString(PushPinView.DefaultColor_Hex));
            pin.Note = "";
            pin.IsEnabled = true;
            pin.Selected = false;
            pin.Target = false;
            pin.Visibility = System.Windows.Visibility.Visible;
            pin.CreateDate = DateTime.Now;

            pin.PropertyChanged += Pin_PropertyChanged;

            PushPinView.PushPins.Add(pin);

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

            Dispatcher.BeginInvoke(() =>
            {
                //var holdLocation = MyMap.ViewportPointToLocation(e.GetPosition(MyMap));

                GeoCoding geoCode = new GeoCoding();
                geoCode.DownloadGeoCodeResultCompleted += geoCode_DownloadGeoCodeResultCompleted;
                geoCode.GetAddressFromGeoCoordinate(location);
            });
        }

        //マップをホールド。ピンの追加
        private void MyMap_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (pin_hold_flag == false)
            {



                var holdLocation = MyMap.ViewportPointToLocation(e.GetPosition(MyMap));

                AddNewPin(holdLocation);

                var vc = VibrateController.Default;
                vc.Start(TimeSpan.FromMilliseconds(50));

                //DB.SaveInfoToIsoStrage(PushPinView);
            }
            pin_hold_flag = false;

        }

        void geoCode_DownloadGeoCodeResultCompleted(object sender, DownloadGeoCodeResultCompletedEventArgs e)
        {
            /*
            PushPinModel pin = new PushPinModel();
            pin.Location = e.Location; //new GeoCoordinate(holdLocation.Latitude, holdLocation.Longitude),
            pin.Name = "New Place";
            pin.Color = new SolidColorBrush(Utility.GetColorFromHexString(PushPinView.DefaultColor_Hex));
            pin.Note = "";
            pin.IsEnabled = true;
            pin.Selected = false;
            pin.Target = false;
            pin.Visibility = System.Windows.Visibility.Visible;
            pin.CreateDate = DateTime.Now;
            */

            PushPinModel pin;
            pin = PushPinView.GetPinByLocation(e.Location);
            if (pin == null)
                return;
            

            if (e.Status != "Completed")
            {
                pin.Address = "";
            }
            else if(e.Status=="Completed")
            {
                try
                {
                    var xmlDoc = XElement.Parse(e.Result);
                    var ns = xmlDoc.Name.Namespace;
                    var places = from place in xmlDoc.Descendants(ns + "Placemark")
                                 select place;

                    if (places.Count() == 0)
                    {
                        pin.Address = "";
                    }
                    else
                    {
                        var firstElement = places.First();
                        string address = (string)firstElement.Element(ns + "address").Value;

                        pin.Address = address;

                    }
                }
                catch
                {
                    //notsupportedexceptionだったかな？ログイン画面に飛ばされるWifiに接続した状態だと、htmlを取得してしまうのでxmlとして解析できない。
                    pin.Address = "";
                }
            }
            else if (e.Status == "TimeOut")
            {
                pin.Address = "";
            }
            /*
            pin.PropertyChanged += Pin_PropertyChanged;

            PushPinView.PushPins.Add(pin);
            DB.SaveInfoToIsoStrage(PushPinView);

            var vc = VibrateController.Default;
            vc.Start(TimeSpan.FromMilliseconds(50));
             */
        }





        #endregion



        #region ピンのタッチイベントハンドラー
        private void Pushpin_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            pin_tap_flag = true;

            var tappedPin = (sender as Pushpin).DataContext as PushPinModel;

            TP.Add(tappedPin);
            //System.Diagnostics.Debug.WriteLine("1:"+TP.Point1.Location+" 2:"+TP.Point2.Location+" d:"+TP.Distance);
            refreshTwoPointLine();

            PushPinView.SetSelected(tappedPin);


        }

        private void Pushpin_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            pin_hold_flag = true;

            var holdedPin = (sender as Pushpin).DataContext as PushPinModel;

            var result = MessageBox.Show("Are you sure to delete ''" +holdedPin.Name + "'' ?", "Delete", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {

                if (holdedPin.Target == true)
                {
                    Target = null;

                    refreshLine();
                    refreshDistanceTextBlock();
                    TextBlock_TargetName.Text = "";
                }

                TP.Clear();
                refreshTwoPointLine();
                
                PushPinView.PushPins.Remove(holdedPin);

                //DB.SaveInfoToIsoStrage(PushPinView);
            }
        }

        private void Pushpin_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MyMap.IsEnabled = false;

            var doubleTappedPin = (sender as Pushpin).DataContext as PushPinModel;

            PushPinView.SetTarget(doubleTappedPin);
            Target=PushPinView.GetTarget();

            refreshLine();
            refreshDistanceTextBlock();
            TextBlock_TargetName.Text = Target.Name;
            
            var vc = VibrateController.Default;
            vc.Start(TimeSpan.FromMilliseconds(50));

            System.Threading.Thread.Sleep(500);

            MyMap.IsEnabled = true;

        }

        void Pin_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            
            var pin = sender as PushPinModel;

            if (e.PropertyName == "Target")
            {
                Target = pin;

                if (Target.Location.IsUnknown == false)
                {
                    Distance = Utility.CalcDistanceTo_Hubeny(Sensor.Location, Target.Location);
                    ETA = Utility.CalcETA(Distance, Sensor.AvgSpeed);
                }
                TextBlock_TargetName.Text = Target.Name;
                refreshDistanceTextBlock();
                refreshLine();
            }
            if (e.PropertyName == "Location")
            {
                if (pin.Target == true)
                {
                    Target = pin;
                    if (Target.Location.IsUnknown == false)
                    {
                        Distance = Utility.CalcDistanceTo_Hubeny(Sensor.Location, Target.Location);
                        ETA = Utility.CalcETA(Distance, Sensor.AvgSpeed);
                    }


                    refreshDistanceTextBlock();
                    refreshLine();

                }
                refreshTwoPointLine();
            }
             
        }



        #endregion


        #region 画面左のツールバーの処理

        private void Image_ZoomUp_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //var util = new Utility();
            MyMap.ZoomLevel++;
        }

        private void Image_ZoomDown_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //var util = new Utility();
            MyMap.ZoomLevel--;
        }

        private void Image_MoveToCurrent_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //現在地を中心に地図を表示
            if(Sensor.Location.IsUnknown==false)
                MyMap.SetView(Sensor.Location, MyMap.ZoomLevel);
        }

        private void Image_MoveToTarget_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (Target != null)
                MyMap.SetView(Target.Location, MyMap.ZoomLevel);
            else
                return;//MyMap.SetView(Sensor.Location, MyMap.ZoomLevel);
        }

        private void Grid_Info_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var selectedItem = PushPinView.GetSelcted();
            if (selectedItem != null)
            {
                NavigationService.Navigate(new Uri("/DetailPage.xaml?EditingType=Modify", UriKind.Relative));
            }
            else
            {
                NavigationService.Navigate(new Uri("/DetailPage.xaml?EditingType=New" + "&Latitude=" + candidatePin.Location.Latitude.ToString() + "&Longitude=" + candidatePin.Location.Longitude.ToString(), UriKind.Relative));
            }
        }

        private void Grid_VisibilitySwitch_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var tileVisibility = (bool)isolateStore["MapPageTileVisibility"];
            if (tileVisibility == true)
            {
                gridCollapsed();

                isolateStore["MapPageTileVisibility"] = false;
            }
            else
            {
                gridVisible();

                isolateStore["MapPageTileVisibility"] = true;
            }
        }
        private void gridVisible()
        {
            Grid_Distance.Visibility = Visibility.Visible;
            Grid_GoCurrentLocation.Visibility = Visibility.Visible;
            Grid_GoTargetLocation.Visibility = Visibility.Visible;
            Grid_Info.Visibility = Visibility.Visible;
            Grid_TargetName.Visibility = Visibility.Visible;
            Grid_ZoomDown.Visibility = Visibility.Visible;
            Grid_ZoomUp.Visibility = Visibility.Visible;
            //Grid_LockPin.Visibility = Visibility.Visible;
            Grid_EstimateTimeArrival.Visibility = Visibility.Visible;
        }
        private void gridCollapsed()
        {
            Grid_Distance.Visibility = Visibility.Collapsed;
            Grid_GoCurrentLocation.Visibility = Visibility.Collapsed;
            Grid_GoTargetLocation.Visibility = Visibility.Collapsed;
            Grid_Info.Visibility = Visibility.Collapsed;
            Grid_TargetName.Visibility = Visibility.Collapsed;
            Grid_ZoomDown.Visibility = Visibility.Collapsed;
            Grid_ZoomUp.Visibility = Visibility.Collapsed;
            //Grid_LockPin.Visibility = Visibility.Collapsed;
            Grid_EstimateTimeArrival.Visibility = Visibility.Collapsed;
        }

        private void Grid_LockPin_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var pinLock = (bool)isolateStore["MapPageLockPin"];
            if (pinLock == true)
            {
                MapItemsControl_PushPins.IsEnabled = true;
                isolateStore["MapPageLockPin"] = false;
                TextBlock_LockPin.Text = "Unlock";
            }
            else
            {
                MapItemsControl_PushPins.IsEnabled = false;
                isolateStore["MapPageLockPin"] = true;
                TextBlock_LockPin.Text = "Lock";
            }

            var vc = VibrateController.Default;
            vc.Start(TimeSpan.FromMilliseconds(50));
        }

        private void Grid_Search_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            gridCollapsed();

            Grid_SearchBox.Visibility = Visibility.Visible;
            TextBox_SearchBox.Focus();
            TextBox_SearchBox.SelectAll();
            TextBox_SearchBox.Background = new SolidColorBrush(Colors.White);
            
        }

        #region 場所検索ボックス関連
        private void TextBox_Search_LostFocus(object sender, RoutedEventArgs e)
        {
            Grid_SearchBox.Visibility = Visibility.Collapsed;
            gridVisible();
        }

        private void TextBox_Search_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                e.Handled = true;
                this.Focus();

                GeoCoding geoCode = new GeoCoding();
                geoCode.DownloadGeoCodeResultCompleted += geoCode_DownloadReverseGeoCodeResultCompleted;
                geoCode.GetGeoCoordintateFromAddress(TextBox_SearchBox.Text);
            }
        }
        private void geoCode_DownloadReverseGeoCodeResultCompleted(object sender, DownloadGeoCodeResultCompletedEventArgs e)
        {
            if (e.Status != "Completed")
                return;

            var xmlDoc = XElement.Parse(e.Result);
            var ns = xmlDoc.Name.Namespace;

            var locations = from location in xmlDoc.Descendants(ns + "location")
                         select location;

            if (locations.Count() == 0)
            {
                return;
            }
            var firstElement = locations.First();
            //string[] geocoordinate = ((string)firstElement.Element(ns + "Point").Element(ns + "coordinates").Value).Split(',');
            string[] geocoordinate =new string[]{firstElement.Element(ns + "lat").Value,firstElement.Element(ns + "lng").Value};

           

            var searchLocation = new GeoCoordinate(Convert.ToDouble(geocoordinate[0]), Convert.ToDouble(geocoordinate[1]));

            PushPinView.ClearSelected();

            //候補地ピンの座標を設定
            candidatePin.Location = searchLocation;

            MyMap.SetView(candidatePin.Location, MyMap.ZoomLevel);

            if (MyMap.Children.Contains(candidatePin) == false)
            {
                MyMap.Children.Add(candidatePin);
            }
        }

        #endregion




        #endregion

        #region アプリケーションバー
        private void Appbar_ListButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/ListPage.xaml", UriKind.Relative));

        }
        private void Appbar_CompassButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void AppbarMenu_Settings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingPage.xaml", UriKind.Relative));
        }

        private void AppbarMenu_SensorInfo_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SensorDataPage.xaml", UriKind.Relative));
        }

        #endregion



        private void Grid_TargetName_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PushPinView.SetSelected(Target);

            NavigationService.Navigate(new Uri("/DetailPage.xaml?EditingType=Modify", UriKind.Relative));
        }

        private void Grid_GoCurrentLocation_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (Sensor.Location.IsUnknown == true)
                return;

            var currentLocation = Sensor.Location;

            AddNewPin(currentLocation);

            var vc = VibrateController.Default;
            vc.Start(TimeSpan.FromMilliseconds(50));


            GeoCoding geoCode = new GeoCoding();
            geoCode.DownloadGeoCodeResultCompleted += geoCode_DownloadGeoCodeResultCompleted;
            geoCode.GetAddressFromGeoCoordinate(currentLocation);
        }




        //GameクラスのExitメソッドを使って強制終了する。
        private void AppbarMenu_ExitApp_Click(object sender, EventArgs e)
        {
            var game = new Microsoft.Xna.Framework.Game();

            game.Exit();

        }




    }
}