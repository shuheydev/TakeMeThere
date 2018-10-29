using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls;

using System.Xml.Linq;
using System.Windows.Navigation;
using System.Device.Location;
using Microsoft.Phone.Tasks;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using System.Windows.Input;

using System.Text.RegularExpressions;


namespace TakeMeThere
{

    public partial class DetailPage : PhoneApplicationPage
    {

        ViewModel PushPinView;// = new ViewModel();
        //AppDB DB;//= new AppDB();
        //MySensors Sensor;// = new MySensors();

        bool newItemFlag;
        string PreferredEmail;


        bool doNotResume = false;//レジュームさせないようにするフラグ

        PushPinModel resume;
        App MyApp;

        bool PermissionOfLocationService;

        public DetailPage()
        {

            InitializeComponent();


            PermissionOfLocationService = (bool)IsolatedStorageSettings.ApplicationSettings["LocationService"];

            //if (Sensor == null)
              //  Sensor = new MySensors();
            MyApp = Application.Current as App;

        }

        string edittype;
        DateTime TimeNow;
        //ページに移動してきた時の処理。
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //if (DB == null)
            //    DB = new AppDB();

            PreferredEmail = (string)IsolatedStorageSettings.ApplicationSettings["PreferredEmail"];

            //テーマがlightかdarkによって、電話マークの白、黒を切り替える。
            bool darkTheme = ((Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible);
            if (darkTheme == true)//darkの時は白抜きの画像
            {
                Image_Phone.Source = new BitmapImage(new Uri("/Icons/MB_0008_phone.png", UriKind.RelativeOrAbsolute));
                Image_BaseCircle.Source = new BitmapImage(new Uri("/Icons/appbar.basecircle.rest.png", UriKind.RelativeOrAbsolute));
            }
            else//lightの時は黒抜きの画像
            {
                Image_Phone.Source = new BitmapImage(new Uri("/Icons/MB_0008_phone2.png", UriKind.RelativeOrAbsolute));
                Image_BaseCircle.Source = new BitmapImage(new Uri("/Icons/appbar.base.png", UriKind.RelativeOrAbsolute));
            }

            TimeNow = DateTime.Now;

            //PushPinView = DB.LoadInfoFromXML();
            PushPinView = MyApp.PushPinView;

            edittype = NavigationContext.QueryString["EditingType"];
            if (NavigationContext.QueryString["EditingType"]=="New")//新規登録か登録済みか
                newItemFlag = true;
            else
                newItemFlag = false;

            if (resume != null)//レジュームデータから復元
            {
                TextBox_Name.Text = resume.Name;
                if (Double.IsNaN(resume.Location.Latitude)==false)
                    TextBox_Latitude.Text = resume.Location.Latitude.ToString();
                else
                    TextBox_Latitude.Text = "";
                if (Double.IsNaN(resume.Location.Longitude) ==false)
                    TextBox_Longitude.Text = resume.Location.Longitude.ToString();
                else
                    TextBox_Longitude.Text = "";

                TextBox_Address.Text = resume.Address;
                TextBox_PhoneNum.Text = resume.PhoneNum;
                TextBox_Note.Text = resume.Note;

                if (resume.Target == true)
                {
                    CheckBox_IsTarget.IsChecked = true;
                }
                else
                {
                    CheckBox_IsTarget.IsChecked = false;
                }

                if (resume.Visibility == System.Windows.Visibility.Visible)
                {
                    CheckBox_IsVisible.IsChecked = true;
                }
                else
                {
                    CheckBox_IsVisible.IsChecked = false;
                }

                TextBlock_CreateDate.Text = resume.CreateDate.ToString();
                TextBlock_CreateDayOfWeek.Text = resume.CreateDate.DayOfWeek.ToString();

                resume = null;
                doNotResume = false;

            }
            else if (newItemFlag == true)//新規登録
            {
                TextBox_Name.Text = "New Place";
                TextBox_Latitude.Text = NavigationContext.QueryString["Latitude"];
                TextBox_Longitude.Text = NavigationContext.QueryString["Longitude"];
                CheckBox_IsVisible.IsChecked = true;
                TextBlock_CreateDate.Text = TimeNow.ToString();
            }
            else//登録済みアイテムの修正
            {
                PushPinModel selctedItem = PushPinView.GetSelcted();

                if (selctedItem != null)
                {
                    TextBox_Name.Text = selctedItem.Name;
                    TextBox_Latitude.Text = selctedItem.Location.Latitude.ToString();
                    TextBox_Longitude.Text = selctedItem.Location.Longitude.ToString();
                    TextBox_Address.Text = selctedItem.Address;
                    TextBox_PhoneNum.Text = selctedItem.PhoneNum;
                    TextBox_Note.Text = selctedItem.Note;

                    if (selctedItem.Target == true)
                    {
                        CheckBox_IsTarget.IsChecked = true;
                    }
                    else
                    {
                        CheckBox_IsTarget.IsChecked = false;
                    }

                    if (selctedItem.Visibility == System.Windows.Visibility.Visible)
                    {
                        CheckBox_IsVisible.IsChecked = true;
                    }
                    else
                    {
                        CheckBox_IsVisible.IsChecked = false;
                    }
                    TextBlock_CreateDate.Text = selctedItem.CreateDate.ToString();
                    TextBlock_CreateDayOfWeek.Text = selctedItem.CreateDate.DayOfWeek.ToString();
                }
            }


            Panorama.Title = TextBox_Name.Text;

            //住所が空欄だったら住所を取得する。
            if (PermissionOfLocationService == true)
            {
                /*
                if (TextBox_Address.Text == "" || TextBox_Address.Text == "Address")
                {
                    GeoCoding geoCode = new GeoCoding();

                    geoCode.DownloadGeoCodeResultCompleted += geoCode_DownloadGeoCodeResultCompleted;
                    GeoCoordinate location = new GeoCoordinate(Convert.ToDouble(TextBox_Latitude.Text), Convert.ToDouble(TextBox_Longitude.Text));
                    geoCode.GetAddressFromGeoCoordinate(location);
                }
                 */
                if (TextBox_Latitude.Text != "" && TextBox_Longitude.Text != "" && (TextBox_Address.Text == "" || TextBox_Address.Text == "Address"))
                {
                    GeoCoding geoCode = new GeoCoding();

                    geoCode.DownloadGeoCodeResultCompleted += geoCode_DownloadGeoCodeResultCompleted;
                    GeoCoordinate location = new GeoCoordinate(Convert.ToDouble(TextBox_Latitude.Text), Convert.ToDouble(TextBox_Longitude.Text));
                    geoCode.GetAddressFromGeoCoordinate(location);
                }
                else if ((TextBox_Latitude.Text == "" || TextBox_Longitude.Text == "") && (TextBox_Address.Text != "" && TextBox_Address.Text != "Address"))
                {
                    GeoCoding geoCode = new GeoCoding();

                    geoCode.DownloadGeoCodeResultCompleted += geoCode_DownloadReverseGeoCodeResultCompleted;
                    string address = TextBox_Address.Text;
                    geoCode.GetGeoCoordintateFromAddress(address);
                }
            }

            getAddressAndLocation();

            base.OnNavigatedTo(e);
        }


        private void getAddressAndLocation()
        {
            //住所が空欄だったら住所を取得する。
            if (PermissionOfLocationService == true)
            {
                /*
                if (TextBox_Address.Text == "" || TextBox_Address.Text == "Address")
                {
                    GeoCoding geoCode = new GeoCoding();

                    geoCode.DownloadGeoCodeResultCompleted += geoCode_DownloadGeoCodeResultCompleted;
                    GeoCoordinate location = new GeoCoordinate(Convert.ToDouble(TextBox_Latitude.Text), Convert.ToDouble(TextBox_Longitude.Text));
                    geoCode.GetAddressFromGeoCoordinate(location);
                }
                 */
                if (TextBox_Latitude.Text != "" && TextBox_Longitude.Text != "" && (TextBox_Address.Text == "" || TextBox_Address.Text == "Address"))
                {
                    GeoCoding geoCode = new GeoCoding();

                    geoCode.DownloadGeoCodeResultCompleted += geoCode_DownloadGeoCodeResultCompleted;
                    GeoCoordinate location = new GeoCoordinate(Convert.ToDouble(TextBox_Latitude.Text), Convert.ToDouble(TextBox_Longitude.Text));
                    geoCode.GetAddressFromGeoCoordinate(location);
                }
                else if ((TextBox_Latitude.Text == "" || TextBox_Longitude.Text == "") && (TextBox_Address.Text != "" && TextBox_Address.Text != "Address"))
                {
                    GeoCoding geoCode = new GeoCoding();

                    geoCode.DownloadGeoCodeResultCompleted += geoCode_DownloadReverseGeoCodeResultCompleted;
                    string address = TextBox_Address.Text;
                    geoCode.GetGeoCoordintateFromAddress(address);
                }
            }
        }


        //住所から経緯度を取得
        private void geoCode_DownloadReverseGeoCodeResultCompleted(object sender, DownloadGeoCodeResultCompletedEventArgs e)
        {
            if (e.Status != "Completed")
            {

                //return;
            }
            else
            {
                try
                {
                    var xmlDoc = XElement.Parse(e.Result);
                    var ns = xmlDoc.Name.Namespace;
                    var places = from place in xmlDoc.Descendants(ns + "Placemark")
                                 select place;

                    if (places.Count() == 0)
                    {
                        TextBox_Latitude.Text = "";
                        TextBox_Longitude.Text = "";
                    }
                    else
                    {
                        var firstElement = places.First();
                        string[] geocoordinate = ((string)firstElement.Element(ns + "Point").Element(ns + "coordinates").Value).Split(',');

                        TextBox_Latitude.Text = geocoordinate[1];
                        TextBox_Longitude.Text = geocoordinate[0];
                    }
                }
                catch
                {
                    TextBox_Latitude.Text = "";
                    TextBox_Longitude.Text = "";
                }
            }

            //経緯度を取得できなかった場合に、現状を初期状態として記録
            //StrBefore = TextBox_Title.Text + TextBox_Note.Text + TextBox_Latitude.Text + TextBox_Longitude.Text + TextBox_Address.Text;
        }

        void geoCode_DownloadGeoCodeResultCompleted(object sender, DownloadGeoCodeResultCompletedEventArgs e)
        {
            if (e.Status != "Completed")
            {
                TextBox_Address.Text = "";
                return;
            }

            try
            {
                var xmlDoc = XElement.Parse(e.Result);
                var ns = xmlDoc.Name.Namespace;
                var addresses = from address in xmlDoc.Descendants(ns + "formatted_address")
                             select address;
                
                if (addresses.Count() == 0)
                {
                    TextBox_Address.Text = "";
                }
                else
                {
                    var firstElement = addresses.First();
                    //string address = (string)firstElement.Element(ns + "address").Value;

                    string address = firstElement.Value;
                    TextBox_Address.Text = address;


                    if (newItemFlag == false)
                    {
                        UIToPushPinView();
                        //DB.SaveInfoToIsoStrage(PushPinView);
                    }
                }
            }
            catch
            {
                TextBox_Address.Text = "";
            }

        }

        //メインページから離脱した時の処理。
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (doNotResume == true)
            {
                resume = null;
            }
            else
            {
                resume = new PushPinModel();
                if (TextBox_Latitude.Text != "")
                    resume.Location.Latitude = Convert.ToDouble(TextBox_Latitude.Text);
                else
                    resume.Location.Latitude = Double.NaN;
                if (TextBox_Longitude.Text != "")
                    resume.Location.Longitude = Convert.ToDouble(TextBox_Longitude.Text);
                else
                    resume.Location.Longitude = Double.NaN;
                resume.Name = TextBox_Name.Text;
                resume.Color = new SolidColorBrush(Utility.GetColorFromHexString(PushPinView.DefaultColor_Hex));
                resume.Address = TextBox_Address.Text;
                resume.PhoneNum = TextBox_PhoneNum.Text;
                resume.Note = TextBox_Note.Text;

                if (CheckBox_IsVisible.IsChecked == true)
                {
                    resume.Visibility = Visibility.Visible;
                }
                else
                {
                    resume.Visibility = Visibility.Collapsed;
                }

                if (CheckBox_IsTarget.IsChecked == true)
                {
                    resume.Target = true;
                }
                else
                {
                    resume.Target = false;
                }

                resume.CreateDate = DateTime.Parse(TextBlock_CreateDate.Text);

            }

            //DB.SaveInfoToIsoStrage(PushPinView);
            base.OnNavigatedFrom(e);
        }


        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (TextBox_Latitude.Text == "" || TextBox_Longitude.Text == "")
            {
                MessageBox.Show("Please fill Latitude and Longitude field.");
            }
            else
            {
                #region 有効な経緯度を入力するように促す処理
                var latitude=Convert.ToDouble(TextBox_Latitude.Text);
                var longitude = Convert.ToDouble(TextBox_Longitude.Text);

                int checkInputFlag=0;

                if (latitude > -90 && latitude < 90)
                {

                }
                else
                {
                    checkInputFlag = 1;//latitudeが範囲外
                }

                if (longitude > -180 && longitude < 180)
                {

                }
                else
                {
                    checkInputFlag =checkInputFlag+ 2;//longitudeが範囲外
                }


                switch (checkInputFlag)
                {
                    case 0:
                        {
                            UIToPushPinView();
                            //DB.SaveInfoToIsoStrage(PushPinView);
                            doNotResume = true;
                            NavigationService.GoBack();
                            break;
                        }
                    case 1:
                        {
                            MessageBox.Show("Latitude is out of range." + System.Environment.NewLine + "Latitude must be..." + System.Environment.NewLine + "-90 < Latitude < 90");

                            break;
                        }
                    case 2:
                        MessageBox.Show("Longitude is out of range." + System.Environment.NewLine + "Longitude must be..." + System.Environment.NewLine + "-180 < Longitude < 180");

                        break;
                    case 3:
                        MessageBox.Show("Latitude and Longitude are out of range." + System.Environment.NewLine + "Location data must be..." + System.Environment.NewLine + "-90 < Latitude < 90" + System.Environment.NewLine + "-180 < Longitude < 180");
                        break;
                }
                #endregion


            }



        }

        //UIに表示されている情報をPushPinModel→PushPinViewに格納
        private void UIToPushPinView()
        {
            if (newItemFlag == true)//新規登録
            {
                #region 新規登録
                PushPinModel newPlace = new PushPinModel();

                newPlace.Location = new GeoCoordinate(Convert.ToDouble(TextBox_Latitude.Text), Convert.ToDouble(TextBox_Longitude.Text));
                newPlace.Name = TextBox_Name.Text;
                newPlace.Color = new SolidColorBrush(Utility.GetColorFromHexString(PushPinView.DefaultColor_Hex));
                newPlace.Address = TextBox_Address.Text;
                newPlace.PhoneNum = TextBox_PhoneNum.Text;
                newPlace.Note = TextBox_Note.Text;
                newPlace.IsEnabled = true;
                newPlace.Selected = false;
                newPlace.Target = false;


                if (CheckBox_IsVisible.IsChecked == true)
                {
                    newPlace.Visibility = Visibility.Visible;
                }
                else
                {
                    newPlace.Visibility = Visibility.Collapsed;
                }

                PushPinView.PushPins.Add(newPlace);


                if (CheckBox_IsTarget.IsChecked == true)
                {

                    PushPinView.SetTarget(newPlace);
                }

                newPlace.CreateDate = TimeNow;

                bool darkTheme = ((Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible);
                if (darkTheme == true)//darkの時は白抜きの画像
                {
                    newPlace.ArrowURI = "/Icons/appbar.next.rest.png";
                    newPlace.CircleURI = "/Icons/appbar.basecircle.rest.png";
                }
                else
                {
                    newPlace.ArrowURI = "/Icons/appbar.next.rest.light.png";
                    newPlace.CircleURI = "/Icons/appbar.base.png";
                }

                #endregion

            }
            else//登録済みアイテムの修正
            {
                #region 登録済みアイテムの変更
                PushPinModel selectedItem;

                selectedItem = PushPinView.GetSelcted();
                //PushPinModel targetItem=PushPinView.GetTarget();
                bool isTargetFlag = false;

                if (selectedItem.Target == true)
                    isTargetFlag = true;

                selectedItem.Name = TextBox_Name.Text;

                /*
                //経緯度の格納
                //経緯度が両方共入力されている場合
                if (TextBox_Latitude.Text != "" && TextBox_Longitude.Text != "")
                {
                    try
                    {//有効な値かどうかを例外発行の有無で確認
                        selectedItem.Location = new GeoCoordinate(Convert.ToDouble(TextBox_Latitude.Text), Convert.ToDouble(TextBox_Longitude.Text));
                    }
                    catch
                    {
                        //formatexceptionだったと思う、が出た場合、メッセージを出して入力を促す。
                        //selectedItem.Location = GeoCoordinate.Unknown;
                        MessageBox.Show("Latitude or Longitude is out of range." + System.Environment.NewLine + "Location data must be..." + System.Environment.NewLine + "-90 < Latitude < 90" + System.Environment.NewLine + "-180 < Longitude < 180");
                    }
                }
                else
                {
                    //どちらかが空欄の場合、何もしない
                    //MessageBox.Show("");
                    //selectedItem.Location = GeoCoordinate.Unknown;
                }
                */
                //このメソッドが呼び出される前段階で、経緯度が有効かチェックされている
                selectedItem.Location = new GeoCoordinate(Convert.ToDouble(TextBox_Latitude.Text), Convert.ToDouble(TextBox_Longitude.Text));
                selectedItem.Address = TextBox_Address.Text;
                selectedItem.PhoneNum = TextBox_PhoneNum.Text;
                selectedItem.Note = TextBox_Note.Text;

                if (CheckBox_IsTarget.IsChecked == true)
                {
                    if (isTargetFlag == true)
                    {
                        //
                    }
                    else
                    {
                        PushPinView.SetTarget(selectedItem);
                    }
                }
                else
                {
                    if (isTargetFlag == true)
                    {
                        selectedItem.Target = false;
                    }
                    else
                    {
                        //
                    }

                }

                if (CheckBox_IsVisible.IsChecked == true)
                {
                    selectedItem.Visibility = Visibility.Visible;
                }
                else
                {
                    selectedItem.Visibility = Visibility.Collapsed;
                }
                #endregion
            }



        }

        
        private void TextBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //(sender as TextBox).SelectAll();

        }
         

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }


        private void Grid_PhoneCall_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PhoneCallTask phoneCallTask = new PhoneCallTask();
            phoneCallTask.PhoneNumber = TextBox_PhoneNum.Text;//(TextBox_PhoneNum.Text).Replace("-",""); 
            phoneCallTask.DisplayName = TextBox_Name.Text;
            phoneCallTask.Show();
        }

        private void TextBox_Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            Panorama.Title = TextBox_Name.Text;
        }

        private void AppbarMenuItem_Email_Click(object sender, EventArgs e)
        {
            string body = "" + System.Environment.NewLine + System.Environment.NewLine
    + "Name: " + TextBox_Name.Text + System.Environment.NewLine
    + "Location: " + TextBox_Latitude.Text+", "+TextBox_Longitude.Text + System.Environment.NewLine
    + "Address: " + TextBox_Address.Text + System.Environment.NewLine
    + "Phone: " + TextBox_PhoneNum.Text + System.Environment.NewLine
    + "Note:" + TextBox_Note.Text + System.Environment.NewLine;

            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = TextBox_Name.Text;
            emailComposeTask.Body = body;
            emailComposeTask.To = PreferredEmail;
            emailComposeTask.Show();
        }

        private void AppbarMenuItem_SMS_Click(object sender, EventArgs e)
        {
            string body = "" + System.Environment.NewLine + System.Environment.NewLine
+ "Name: " + TextBox_Name.Text + System.Environment.NewLine
+ "Location: " + TextBox_Latitude.Text + ", " + TextBox_Longitude.Text + System.Environment.NewLine
+ "Address: " + TextBox_Address.Text + System.Environment.NewLine
+ "Phone: " + TextBox_PhoneNum.Text + System.Environment.NewLine
+ "Note:" + TextBox_Note.Text + System.Environment.NewLine;

            SmsComposeTask smsComposeTask = new SmsComposeTask();
            smsComposeTask.To = "";
            smsComposeTask.Body = body;
            smsComposeTask.Show();
        }

        private void AppbarMenuItem_Share_Click(object sender, EventArgs e)
        {
            string body = "" + System.Environment.NewLine + System.Environment.NewLine
+ "Name: " + TextBox_Name.Text + System.Environment.NewLine
+ "Location: " + TextBox_Latitude.Text + ", " + TextBox_Longitude.Text + System.Environment.NewLine
+ "Address: " + TextBox_Address.Text + System.Environment.NewLine
+ "Phone: " + TextBox_PhoneNum.Text + System.Environment.NewLine
+ "Note:" + TextBox_Note.Text + System.Environment.NewLine;

            ShareStatusTask shareStatusTask = new ShareStatusTask();
            shareStatusTask.Status = body;
            shareStatusTask.Show();
        }


        //bool backkeypressFlag = false;
        //物理的な戻るボタンが押された時の処理
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            //IsolatedStorageSettings.ApplicationSettings["EditingType"] = "";

            //ページを離れるときにセーブしないようにするため
            doNotResume = true;   

            base.OnBackKeyPress(e);
        }

        //テキストボックスの長文入力時に、上から下まですべての内容をスクロールで見れるようにするため。
        private void TextBox_Note_TextInputStart(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            this.ScrollViewer_Note.UpdateLayout();
            this.ScrollViewer_Note.ScrollToVerticalOffset(this.TextBox_Note.ActualHeight);
        }


        //テキストボックスの最下部で改行したときにカーソルが見切れないようにするため。
        private void TextBox_Note_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                //e.Handled = true;
                this.ScrollViewer_Note.UpdateLayout();
                this.ScrollViewer_Note.ScrollToVerticalOffset(this.TextBox_Note.ActualHeight);
            }
        }


        private void TextBox_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            (sender as PhoneTextBox).SelectAll();

        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            getAddressAndLocation();
        }



    }
}