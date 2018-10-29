using Microsoft.Phone.Controls;
using System;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Input;


namespace TakeMeThere
{
    public partial class SettingPage : PhoneApplicationPage
    {
        public SettingPage()
        {
            InitializeComponent();

            Grid_1.Visibility = Visibility.Collapsed;
            Grid_2.Visibility = Visibility.Collapsed;
            Grid_3.Visibility = Visibility.Collapsed;
            Grid_4.Visibility = Visibility.Collapsed;
            Grid_5.Visibility = Visibility.Collapsed;
            Grid_6.Visibility = Visibility.Collapsed;

            Grid_Animation.Visibility = Visibility.Collapsed;
        }

        string avgSpeedBefore;
        string avgSpeedAfter;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Slider_TimeSpanRefreshCompass_main.Value = (double)IsolatedStorageSettings.ApplicationSettings["TimeSpanRefreshCompass_main"];
            Slider_FactorToCorrectDirection_main.Value = (double)IsolatedStorageSettings.ApplicationSettings["FactorToCorrectDirection_main"];
            Slider_TimeSpanUpdateCompass_sensor.Value = (double)IsolatedStorageSettings.ApplicationSettings["TimeSpanUpdateCompass_sensor"];
            Slider_DistanceUpdateGPS_sensor.Value = (double)IsolatedStorageSettings.ApplicationSettings["DistanceUpdateGPS_sensor"];
            Slider_TimeSpanRefreshCompass_map.Value = (double)IsolatedStorageSettings.ApplicationSettings["TimeSpanRefreshCompass_map"];
            Slider_FactorToCorrectDirection_map.Value = (double)IsolatedStorageSettings.ApplicationSettings["FactorToCorrectDirection_map"];
            Slider_Performance_Setting.Value = (double)IsolatedStorageSettings.ApplicationSettings["Performance_Level"];
            tb1.Text = Slider_TimeSpanRefreshCompass_main.Value.ToString();
            tb2.Text = Slider_FactorToCorrectDirection_main.Value.ToString();
            tb3.Text = Slider_TimeSpanUpdateCompass_sensor.Value.ToString();
            tb4.Text = Slider_DistanceUpdateGPS_sensor.Value.ToString();
            tb5.Text = Slider_TimeSpanRefreshCompass_map.Value.ToString();
            tb6.Text = Slider_FactorToCorrectDirection_map.Value.ToString();

            switch ((int)Slider_Performance_Setting.Value)
            {
                case 1:
                    tb7.Text ="Low";
                    break;
                case 2:
                    tb7.Text = "Midium";
                    break;
                case 3:
                    tb7.Text = "High";
                    break;
            }

            TextBox_PreferredEmail.Text = (string)IsolatedStorageSettings.ApplicationSettings["PreferredEmail"];

            ToggleSwitch_MapAnimation.IsChecked = (bool)IsolatedStorageSettings.ApplicationSettings["MapAnimation"];
            ToggleSwitch_LocationService.IsChecked = (bool)IsolatedStorageSettings.ApplicationSettings["LocationService"];

            if ((string)IsolatedStorageSettings.ApplicationSettings["Map"] == "gMap")
                RadioButton_gMap.IsChecked = true;
            else
                RadioButton_bMap.IsChecked = true;

            if ((string)IsolatedStorageSettings.ApplicationSettings["Unit"] == "International")
                RadioButton_International.IsChecked = true;
            else
                RadioButton_Imperial.IsChecked = true;


            double avgSpeed=(double)IsolatedStorageSettings.ApplicationSettings["AverageSpeed"];
            var _avgSpeedString = Utility.ConvertSpeedWithUnit(avgSpeed);
            TextBox_AverageSpeed.Text = _avgSpeedString.Substring(0,_avgSpeedString.IndexOf(" "));//(avgSpeed*3600/1000).ToString("F1");

            if ((string)IsolatedStorageSettings.ApplicationSettings["Unit"] == "International")
            {
                TextBlock_AverageSpeed.Text = "Average Speed(km/h)";
            }
            else
            {
                TextBlock_AverageSpeed.Text = "Average Speed(mile/h)";
            }

            avgSpeedBefore = TextBox_AverageSpeed.Text;



            base.OnNavigatedTo(e);
        }


        //メインページから離脱した時の処理。
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings["TimeSpanRefreshCompass_main"] = Slider_TimeSpanRefreshCompass_main.Value;
            IsolatedStorageSettings.ApplicationSettings["FactorToCorrectDirection_main"] = Slider_FactorToCorrectDirection_main.Value;
            IsolatedStorageSettings.ApplicationSettings["TimeSpanUpdateCompass_sensor"] = Slider_TimeSpanUpdateCompass_sensor.Value;
            IsolatedStorageSettings.ApplicationSettings["DistanceUpdateGPS_sensor"] = Slider_DistanceUpdateGPS_sensor.Value;
            IsolatedStorageSettings.ApplicationSettings["TimeSpanRefreshCompass_map"] = Slider_TimeSpanRefreshCompass_map.Value;
            IsolatedStorageSettings.ApplicationSettings["FactorToCorrectDirection_map"] = Slider_FactorToCorrectDirection_map.Value;
            IsolatedStorageSettings.ApplicationSettings["PreferredEmail"] = TextBox_PreferredEmail.Text;
            IsolatedStorageSettings.ApplicationSettings["Performance_Level"] = Slider_Performance_Setting.Value;

            double avgSpeed;
            if (TextBox_AverageSpeed.Text == "")
                avgSpeed = 0;
            else
                avgSpeed = Convert.ToDouble(TextBox_AverageSpeed.Text);

            var systemOfUnit = (string)IsolatedStorageSettings.ApplicationSettings["Unit"];
            if (systemOfUnit == "International")
                IsolatedStorageSettings.ApplicationSettings["AverageSpeed"] = avgSpeed * 1000 / 3600;//km/hをm/sへ変換
            else
                IsolatedStorageSettings.ApplicationSettings["AverageSpeed"] = avgSpeed * 1609.344 / 3600;//mile/hをm/sへ変換



            avgSpeedAfter = TextBox_AverageSpeed.Text;
            if (avgSpeedBefore.CompareTo(avgSpeedAfter) != 0)
            {
                IsolatedStorageSettings.ApplicationSettings["AverageSpeedChanged"] = true;
            }

            IsolatedStorageSettings.ApplicationSettings.Save();
            base.OnNavigatedFrom(e);
        }

        #region アプリケーションバー
        private void Appbar_MapButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MapPage_gMap.xaml", UriKind.Relative));
        }

        private void Appbar_CompassButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
        #endregion
    

        //ValueChangedイベントではスライダーが動かなくなる不具合があるので、MouseMoveイベントで代用している。パーフェクトだ。
        private void Slider_TimeSpanRefreshCompass_main_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var mySlider = sender as Slider;
            mySlider.Value =(int)( e.GetPosition(mySlider).X / mySlider.ActualWidth * (mySlider.Maximum - mySlider.Minimum));
            tb1.Text = mySlider.Value.ToString();
        }

        private void Slider_FactorToCorrectDirection_main_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var mySlider = sender as Slider;
            mySlider.Value = (int)(e.GetPosition(mySlider).X / mySlider.ActualWidth * (mySlider.Maximum - mySlider.Minimum));
            tb2.Text = mySlider.Value.ToString();
        }

        private void Slider_TimeSpanUpdateCompass_sensor_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var mySlider = sender as Slider;
            mySlider.Value = (int)(e.GetPosition(mySlider).X / mySlider.ActualWidth * (mySlider.Maximum - mySlider.Minimum));
            tb3.Text = mySlider.Value.ToString();
        }

        private void Slider_DistanceUpdateGPS_sensor_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var mySlider = sender as Slider;
            mySlider.Value = (int)(e.GetPosition(mySlider).X / mySlider.ActualWidth * (mySlider.Maximum - mySlider.Minimum));
            tb4.Text = mySlider.Value.ToString();
        }

        private void Slider_TimeSpanRefreshCompass_map_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var mySlider = sender as Slider;
            mySlider.Value = (int)(e.GetPosition(mySlider).X / mySlider.ActualWidth * (mySlider.Maximum - mySlider.Minimum));
            tb5.Text = mySlider.Value.ToString();
        }

        private void Slider_FactorToCorrectDirection_map_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var mySlider = sender as Slider;
            mySlider.Value = (int)(e.GetPosition(mySlider).X / mySlider.ActualWidth * (mySlider.Maximum - mySlider.Minimum));
            tb6.Text = mySlider.Value.ToString();
        }


        private void RadioButton_gMap_Checked(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings["Map"] = "gMap";
        }

        private void RadioButton_bMap_Checked(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings["Map"] = "bMap";
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                e.Handled = true;
                this.Focus();
            }
        }

        private void Slider_Performance_Setting_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            try
            {
                Slider slider = sender as Slider;
                int n = (int)(slider.Value);

                switch (n)
                {
                    case 0:
                        break;
                    case 1:
                        {
                            tb7.Text = "Low";

                            Slider_TimeSpanRefreshCompass_main.Value = 1000;
                            Slider_FactorToCorrectDirection_main.Value = 1;
                            Slider_TimeSpanUpdateCompass_sensor.Value = 1000;
                            Slider_DistanceUpdateGPS_sensor.Value = 10;
                            Slider_TimeSpanRefreshCompass_map.Value = 1000;
                            Slider_FactorToCorrectDirection_map.Value = 1;

                            break;
                        }
                    case 2:
                        tb7.Text = "Midium";

                            Slider_TimeSpanRefreshCompass_main.Value = 200;
                            Slider_FactorToCorrectDirection_main.Value = 5;
                            Slider_TimeSpanUpdateCompass_sensor.Value = 200;
                            Slider_DistanceUpdateGPS_sensor.Value = 5;
                            Slider_TimeSpanRefreshCompass_map.Value = 200;
                            Slider_FactorToCorrectDirection_map.Value = 5;
                        break;
                    case 3:
                        tb7.Text = "High";
                            Slider_TimeSpanRefreshCompass_main.Value = 66;
                            Slider_FactorToCorrectDirection_main.Value = 6;
                            Slider_TimeSpanUpdateCompass_sensor.Value = 300;
                            Slider_DistanceUpdateGPS_sensor.Value = 5;
                            Slider_TimeSpanRefreshCompass_map.Value = 66;
                            Slider_FactorToCorrectDirection_map.Value = 300;
                        break;

                }
                tb1.Text = Slider_TimeSpanRefreshCompass_main.Value.ToString();
                tb2.Text = Slider_FactorToCorrectDirection_main.Value.ToString();
                tb3.Text = Slider_TimeSpanUpdateCompass_sensor.Value.ToString();
                tb4.Text = Slider_DistanceUpdateGPS_sensor.Value.ToString();
                tb5.Text = Slider_TimeSpanRefreshCompass_map.Value.ToString();
                tb6.Text = Slider_FactorToCorrectDirection_map.Value.ToString();
            }
            catch
            { }


            /*

             */
        }

        private void ToggleSwitch_MapAnimation_Changed(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings["MapAnimation"] = ToggleSwitch_MapAnimation.IsChecked;
            System.Diagnostics.Debug.WriteLine(ToggleSwitch_MapAnimation.IsChecked);
        }

        private void ToggleSwitch_LocationService_Changed(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings["LocationService"] = ToggleSwitch_LocationService.IsChecked;
            System.Diagnostics.Debug.WriteLine(ToggleSwitch_LocationService.IsChecked);
        }

        private void RadioButton_Imperial_Checked(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings["Unit"] = "Imperial";
            TextBlock_AverageSpeed.Text = "Average Speed(mile/h)";

        }

        private void RadioButton_International_Checked(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings["Unit"] = "International";
            TextBlock_AverageSpeed.Text = "Average Speed(km/h)";
        }




    }
}