﻿#pragma checksum "D:\Document\プログラミング\windows phone app\Projects\TakeMeThere_まとめフォルダ\20140214_TakeMeThere_ver1.1.2.0\TakeMeThere\SettingPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "7D551C6CE921C51959EB4BB4E5E4A220"
//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.34011
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace TakeMeThere {
    
    
    public partial class SettingPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal Microsoft.Phone.Controls.ToggleSwitch ToggleSwitch_LocationService;
        
        internal System.Windows.Controls.RadioButton RadioButton_gMap;
        
        internal System.Windows.Controls.RadioButton RadioButton_bMap;
        
        internal System.Windows.Controls.RadioButton RadioButton_Imperial;
        
        internal System.Windows.Controls.RadioButton RadioButton_International;
        
        internal System.Windows.Controls.Slider Slider_Performance_Setting;
        
        internal System.Windows.Controls.TextBlock tb7;
        
        internal System.Windows.Controls.TextBlock TextBlock_AverageSpeed;
        
        internal Microsoft.Phone.Controls.PhoneTextBox TextBox_AverageSpeed;
        
        internal Microsoft.Phone.Controls.PhoneTextBox TextBox_PreferredEmail;
        
        internal System.Windows.Controls.Grid Grid_Animation;
        
        internal Microsoft.Phone.Controls.ToggleSwitch ToggleSwitch_MapAnimation;
        
        internal System.Windows.Controls.Grid Grid_3;
        
        internal System.Windows.Controls.Slider Slider_TimeSpanUpdateCompass_sensor;
        
        internal System.Windows.Controls.TextBlock tb3;
        
        internal System.Windows.Controls.Grid Grid_4;
        
        internal System.Windows.Controls.Slider Slider_DistanceUpdateGPS_sensor;
        
        internal System.Windows.Controls.TextBlock tb4;
        
        internal System.Windows.Controls.Grid Grid_1;
        
        internal System.Windows.Controls.Slider Slider_TimeSpanRefreshCompass_main;
        
        internal System.Windows.Controls.TextBlock tb1;
        
        internal System.Windows.Controls.Grid Grid_2;
        
        internal System.Windows.Controls.Slider Slider_FactorToCorrectDirection_main;
        
        internal System.Windows.Controls.TextBlock tb2;
        
        internal System.Windows.Controls.Grid Grid_5;
        
        internal System.Windows.Controls.Slider Slider_TimeSpanRefreshCompass_map;
        
        internal System.Windows.Controls.TextBlock tb5;
        
        internal System.Windows.Controls.Grid Grid_6;
        
        internal System.Windows.Controls.Slider Slider_FactorToCorrectDirection_map;
        
        internal System.Windows.Controls.TextBlock tb6;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/TakeMeThere;component/SettingPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.ToggleSwitch_LocationService = ((Microsoft.Phone.Controls.ToggleSwitch)(this.FindName("ToggleSwitch_LocationService")));
            this.RadioButton_gMap = ((System.Windows.Controls.RadioButton)(this.FindName("RadioButton_gMap")));
            this.RadioButton_bMap = ((System.Windows.Controls.RadioButton)(this.FindName("RadioButton_bMap")));
            this.RadioButton_Imperial = ((System.Windows.Controls.RadioButton)(this.FindName("RadioButton_Imperial")));
            this.RadioButton_International = ((System.Windows.Controls.RadioButton)(this.FindName("RadioButton_International")));
            this.Slider_Performance_Setting = ((System.Windows.Controls.Slider)(this.FindName("Slider_Performance_Setting")));
            this.tb7 = ((System.Windows.Controls.TextBlock)(this.FindName("tb7")));
            this.TextBlock_AverageSpeed = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_AverageSpeed")));
            this.TextBox_AverageSpeed = ((Microsoft.Phone.Controls.PhoneTextBox)(this.FindName("TextBox_AverageSpeed")));
            this.TextBox_PreferredEmail = ((Microsoft.Phone.Controls.PhoneTextBox)(this.FindName("TextBox_PreferredEmail")));
            this.Grid_Animation = ((System.Windows.Controls.Grid)(this.FindName("Grid_Animation")));
            this.ToggleSwitch_MapAnimation = ((Microsoft.Phone.Controls.ToggleSwitch)(this.FindName("ToggleSwitch_MapAnimation")));
            this.Grid_3 = ((System.Windows.Controls.Grid)(this.FindName("Grid_3")));
            this.Slider_TimeSpanUpdateCompass_sensor = ((System.Windows.Controls.Slider)(this.FindName("Slider_TimeSpanUpdateCompass_sensor")));
            this.tb3 = ((System.Windows.Controls.TextBlock)(this.FindName("tb3")));
            this.Grid_4 = ((System.Windows.Controls.Grid)(this.FindName("Grid_4")));
            this.Slider_DistanceUpdateGPS_sensor = ((System.Windows.Controls.Slider)(this.FindName("Slider_DistanceUpdateGPS_sensor")));
            this.tb4 = ((System.Windows.Controls.TextBlock)(this.FindName("tb4")));
            this.Grid_1 = ((System.Windows.Controls.Grid)(this.FindName("Grid_1")));
            this.Slider_TimeSpanRefreshCompass_main = ((System.Windows.Controls.Slider)(this.FindName("Slider_TimeSpanRefreshCompass_main")));
            this.tb1 = ((System.Windows.Controls.TextBlock)(this.FindName("tb1")));
            this.Grid_2 = ((System.Windows.Controls.Grid)(this.FindName("Grid_2")));
            this.Slider_FactorToCorrectDirection_main = ((System.Windows.Controls.Slider)(this.FindName("Slider_FactorToCorrectDirection_main")));
            this.tb2 = ((System.Windows.Controls.TextBlock)(this.FindName("tb2")));
            this.Grid_5 = ((System.Windows.Controls.Grid)(this.FindName("Grid_5")));
            this.Slider_TimeSpanRefreshCompass_map = ((System.Windows.Controls.Slider)(this.FindName("Slider_TimeSpanRefreshCompass_map")));
            this.tb5 = ((System.Windows.Controls.TextBlock)(this.FindName("tb5")));
            this.Grid_6 = ((System.Windows.Controls.Grid)(this.FindName("Grid_6")));
            this.Slider_FactorToCorrectDirection_map = ((System.Windows.Controls.Slider)(this.FindName("Slider_FactorToCorrectDirection_map")));
            this.tb6 = ((System.Windows.Controls.TextBlock)(this.FindName("tb6")));
        }
    }
}

