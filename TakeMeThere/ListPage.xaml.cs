using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using System.Windows.Navigation;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using System.Globalization;
using System.Threading;
using System.Windows.Media.Imaging;

namespace TakeMeThere
{
    public partial class ListPage : PhoneApplicationPage
    {
        ViewModel PushPinView;// = new ViewModel();
        //AppDB DB;// = new AppDB();
        MySensors Sensor;// = new MySensors();

        string PreferredEmail;
        string SortBy;

        bool calcOnceFlag;

        bool PermissionOfLocationService;

        App MyApp;

        string SearchKeyword = "";

        public ListPage()
        {
            InitializeComponent();

            MyApp = Application.Current as App;
            
        }



        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            PermissionOfLocationService = (bool)IsolatedStorageSettings.ApplicationSettings["LocationService"];
            if (Sensor == null)
                Sensor = new MySensors();
            //if (DB == null)
            //    DB = new AppDB();

            Sensor.UserPermission = PermissionOfLocationService;

            calcOnceFlag = false;

            //いろいろ非表示に
            //Grid_SearchBox.Visibility = Visibility.Collapsed;//検索ボックスは非表示に
            //Grid_SortOption.Visibility = Visibility.Collapsed;
            //Button_ResetSearch.Visibility = Visibility.Collapsed;

            PreferredEmail = (string)IsolatedStorageSettings.ApplicationSettings["PreferredEmail"];
            SortBy = (string)IsolatedStorageSettings.ApplicationSettings["Sort"];
            SearchKeyword=(string)IsolatedStorageSettings.ApplicationSettings["SearchKeyword"];


            //PushPinView = DB.LoadInfoFromXML();//データ読み込み
            PushPinView = MyApp.PushPinView;

            if (PushPinView.PushPins.Count == 0)
                TextBlock_NoItemSign.Visibility = Visibility.Visible;
            else
                TextBlock_NoItemSign.Visibility = Visibility.Collapsed;

            //DataContext = Source;
            MyCollection.Source = PushPinView.PushPins;//collectionViewSourceに目的地情報をセット
            MyCollection.View.Filter = null;
            //MyCollection.View.SortDescriptions.Clear();

            initList();

            TextBox_SearchBox.Text = SearchKeyword;

            setOpacityAndColorForList();

            //センサーインスタンスの初期化
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
            //Sensor.AvgSpeed = (double)IsolatedStorageSettings.ApplicationSettings["AverageSpeed"];
            Sensor.GpsMovementThreshold = (double)IsolatedStorageSettings.ApplicationSettings["DistanceUpdateGPS_sensor"];
            Sensor.UpdateCompassTimeSpan = (double)IsolatedStorageSettings.ApplicationSettings["TimeSpanUpdateCompass_sensor"];
            Sensor.GPSDataChanged += Sensor_GPSDataChanged;
            Sensor.Start();

            TextBox_SearchBox.Text = SearchKeyword;

            base.OnNavigatedTo(e);
        }

        private void initList()
        {
            initSortOption();

            SortCollection();
            filteringByKeyword();
        }



        //メインページから離脱した時の処理。
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Sensor.Stop();
            //Sensor.GPSStatusChanged -= Sensor_GPSStatusChanged;

            //DB.SaveInfoToIsoStrage(PushPinView);

            IsolatedStorageSettings.ApplicationSettings["Sort"] = SortBy;
            IsolatedStorageSettings.ApplicationSettings["SearchKeyword"]=SearchKeyword;

            base.OnNavigatedFrom(e);
        }

        private void setOpacityAndColorForList()
        {
            foreach (var item in PushPinView.PushPins)
            {
                if (item.Visibility == Visibility.Visible)
                    item.ListItemOpacity = 1.0;
                else
                    item.ListItemOpacity = 0.5;
                item.ListColor = new SolidColorBrush(Utility.ConvertToFullOpacityColor(item.Color.Color));
            }
        }



        void Sensor_GPSDataChanged(object sender, GPSDataChangedEventArgs e)
        {
            if (Sensor.Location.IsUnknown == true)
                return;
            if (calcOnceFlag == true)
                return;

            Dispatcher.BeginInvoke(() => {
                setDistanceAndETA();
            });
            calcOnceFlag = true;
        }
        private void setDistanceAndETA()
        {
            foreach (var item in PushPinView.PushPins)
            {
                item.Distance = Utility.CalcDistanceTo_Hubeny(Sensor.Location, item.Location);
                item.DistanceString = Utility.ConvertDistanceWithUnit(item.Distance);

                var eta = Utility.CalcETA(item.Distance, Sensor.AvgSpeed);
                if (double.IsNaN(eta) == true)
                {
                    //item.ETAString = "ETA: --";
                }
                else
                {
                    item.ETAString = Utility.ConvertETAWithUnit(eta);
                }
            }

            if (SortBy == "Distance")//Distanceが計算できたらソートするようにしている。
            {
                SortCollection();
            }
        }


        #region リストボックスのイベント

        private void ListBox_Destination_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var selectedItem = ListBox_Destination.SelectedItem as PushPinModel;
            PushPinView.SetSelected(selectedItem);


        }

        private void StackPanel_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var selectedItem = (sender as StackPanel).DataContext as PushPinModel;
            PushPinView.SetSelected(selectedItem);
            NavigationService.Navigate(new Uri("/DetailPage.xaml?EditingType=Modify", UriKind.Relative));
        }

        
        private void Grid_GoToPlace_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var parentUI = (sender as Grid).Parent as StackPanel;

            var tappedItem = parentUI.DataContext as PushPinModel;
            NavigationService.Navigate(new Uri("/MapPage_gMap.xaml?" + "Latitude=" + tappedItem.Location.Latitude.ToString() + "&Longitude=" + tappedItem.Location.Longitude.ToString(), UriKind.Relative));
        }
        

        #endregion

        #region アプリケーションバー
        private void Appbar_MapButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MapPage_gMap.xaml", UriKind.Relative));
        }

        private void Appbar_CompassButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
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
        private void ContextMenuDelete_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var holdedItem = (sender as MenuItem).DataContext as PushPinModel;

            var result = MessageBox.Show("Are you sure to delete ''" + holdedItem.Name + "'' ?", "Delete", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                PushPinView.PushPins.Remove(holdedItem);
            }

            if (PushPinView.PushPins.Count == 0)
                TextBlock_NoItemSign.Visibility = Visibility.Visible;
            else
                TextBlock_NoItemSign.Visibility = Visibility.Collapsed;
        }

        private void ContextMenuItemEmail_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var holdedItem = (sender as MenuItem).DataContext as PushPinModel;

            CultureInfo cc = Thread.CurrentThread.CurrentCulture;
            string body = "" + System.Environment.NewLine + System.Environment.NewLine
                + "Name: " + holdedItem.Name + System.Environment.NewLine
                + "Location: " + holdedItem.Location.ToString() + System.Environment.NewLine
                + "Address: " + holdedItem.Address + System.Environment.NewLine
                + "Link: " + string.Format("http://maps.google.com/maps?q={0},{1}&hl={2}", holdedItem.Location.Latitude, holdedItem.Location.Longitude, cc.ToString())+System.Environment.NewLine
                + "Phone: " + holdedItem.PhoneNum + System.Environment.NewLine
                + "Note:" + holdedItem.Note + System.Environment.NewLine;


            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = holdedItem.Name;
            emailComposeTask.Body = body;
            emailComposeTask.To = PreferredEmail;
            emailComposeTask.Show();
        }

        private void ContextMenuItemSMS_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var holdedItem = (sender as MenuItem).DataContext as PushPinModel;

            string body = "" + System.Environment.NewLine + System.Environment.NewLine
                + "Name: " + holdedItem.Name + System.Environment.NewLine
                + "Location: " + holdedItem.Location.ToString() + System.Environment.NewLine
                + "Address: " + holdedItem.Address + System.Environment.NewLine
                + "Phone: " + holdedItem.PhoneNum + System.Environment.NewLine
                + "Note:" + holdedItem.Note + System.Environment.NewLine;


            SmsComposeTask smsComposeTask = new SmsComposeTask();
            smsComposeTask.To = "";
            smsComposeTask.Body = body;
            smsComposeTask.Show();
        }

        private void ContextMenuItemShare_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var holdedItem = (sender as MenuItem).DataContext as PushPinModel;

            CultureInfo cc = Thread.CurrentThread.CurrentCulture;
            string body = "" + System.Environment.NewLine + System.Environment.NewLine
                + "Name: " + holdedItem.Name + System.Environment.NewLine
                + "Location: " + holdedItem.Location.ToString() + System.Environment.NewLine
                + "Address: " + holdedItem.Address + System.Environment.NewLine
                + "Link: " + string.Format("http://maps.google.com/maps?q={0},{1}&hl={2}", holdedItem.Location.Latitude, holdedItem.Location.Longitude, cc.ToString())
                + "Phone: " + holdedItem.PhoneNum + System.Environment.NewLine
                + "Note:" + holdedItem.Note + System.Environment.NewLine;

            ShareStatusTask shareStatusTask = new ShareStatusTask();
            shareStatusTask.Status = body;
            shareStatusTask.Show();
        }

        private void ContextMenuItemCall_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var holdedItem = (sender as MenuItem).DataContext as PushPinModel;

            string callNum = (holdedItem.PhoneNum).Replace("-", "");

            PhoneCallTask phoneCallTask = new PhoneCallTask();
            phoneCallTask.PhoneNumber = callNum;
            phoneCallTask.DisplayName = holdedItem.Name;
            phoneCallTask.Show();
        }


        #endregion



        #region 検索ボックス



        private void TextBox_Search_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                e.Handled = true;
                this.Focus();


                
                SearchKeyword = TextBox_SearchBox.Text;

                filteringByKeyword();

                Pivot_ListPage.SelectedItem = PivotItem1;
            }

        }

        private void filteringByKeyword()
        {
            CompareInfo compInfo = System.Globalization.CultureInfo.CurrentCulture.CompareInfo;

            if (SearchKeyword != "")
            {
                //PivotItem1.Header = SearchKeyword;
                MyCollection.View.Filter = delegate(object o)
                {
                    PushPinModel pin = (o as PushPinModel);

                    string compString = pin.Name + pin.Address + pin.Note + pin.PhoneNum;

                    if (compInfo.IndexOf(compString, SearchKeyword, 0, CompareOptions.IgnoreCase) != -1)
                        return true;
                    else if (compInfo.IndexOf(compString, SearchKeyword, 0, CompareOptions.IgnoreKanaType) != -1)
                        return true;
                    return false;
                };
                //Button_ResetSearch.Visibility = Visibility.Visible;
            }
            else
            {
                //PivotItem1.Header = "List";
                MyCollection.View.Filter = null;
                //Button_ResetSearch.Visibility = Visibility.Collapsed;
            }


        }

        private void TextBox_Search_LostFocus(object sender, RoutedEventArgs e)
        {
            //Grid_SearchBox.Visibility = Visibility.Collapsed;
        }

        #endregion


        #region 並べ替え
        private void AppbarMenuItem_Sort_Click(object sender, EventArgs e)
        {
            //Grid_SortOption.Visibility = Visibility.Visible;

        }

        private void Button_SortOption_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Grid_SortOption.Visibility = Visibility.Collapsed;
            SortBy = tempCheckedSortOption;
            SortCollection();
            Pivot_ListPage.SelectedItem = PivotItem1;
        }

        string tempCheckedSortOption = "";
        private void RadioButton_SortOption_Checked(object sender, RoutedEventArgs e)
        {
            var checkedItem = sender as RadioButton;

            tempCheckedSortOption = checkedItem.Content.ToString();


            //SortCollection();
            //System.Diagnostics.Debug.WriteLine(SortBy);
        }


        private void SortCollection()
        {
            MyCollection.View.SortDescriptions.Clear();

            switch (SortBy)
            {
                case "Name":
                    {
                        MyCollection.View.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

                        break;
                    }
                case "Location":
                    {
                        MyCollection.View.SortDescriptions.Add(new SortDescription("Location.Latitude", ListSortDirection.Descending));//メンバ変数にもアクセスできる！普通に.を使えばいい。うひょー。
                        MyCollection.View.SortDescriptions.Add(new SortDescription("Location.Longitude", ListSortDirection.Descending));

                        break;
                    }
                case "Distance":
                    {
                        if (Sensor.Location.IsUnknown == true)
                        {
                            //RadioButton_SortOption_Name.IsChecked = true;
                            break;
                        }
                        MyCollection.View.SortDescriptions.Add(new SortDescription("Distance", ListSortDirection.Ascending));
                        break;
                    }
                case "Address":
                    {
                        MyCollection.View.SortDescriptions.Add(new SortDescription("Address", ListSortDirection.Descending));
                        break;
                    }
            }
        }
        #endregion



        //GameクラスのExitメソッドを使って強制終了する。
        private void AppbarMenu_ExitApp_Click(object sender, EventArgs e)
        {
            var game = new Microsoft.Xna.Framework.Game();

            game.Exit();

        }

        private void TextBox_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }


        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            initSortOption();

            var pivot = sender as Pivot;

            switch (pivot.SelectedIndex)
            {
                case 0:
                    ApplicationBar.IsVisible = true;
                    break;
                case 1:
                    ApplicationBar.IsVisible = false;
                    break;
            }
        }

        private void initSortOption()
        {
            //ラジオボタンにチェックを入れるだけ
            switch (SortBy)
            {
                case "Name":
                    {
                        RadioButton_SortOption_Name.IsChecked = true;
                        break;
                    }
                case "Location":
                    {
                        RadioButton_SortOption_Location.IsChecked = true;
                        break;
                    }
                case "Distance":
                    {
                        RadioButton_SortOption_Distance.IsChecked = true;
                        break;
                    }
                case "Address":
                    {
                        RadioButton_SortOption_Address.IsChecked = true;
                        break;
                    }
            }
        }



    }
}