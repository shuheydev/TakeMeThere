﻿#pragma checksum "E:\Document\プログラミング\windows phone app\Projects\TakeMeThere\TakeMeThere\Page1.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "05D7075013D254101DA2A5C29F67134E"
//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.18033
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
    
    
    public partial class Page1 : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.TextBlock Latitude;
        
        internal System.Windows.Controls.TextBlock Longitude;
        
        internal System.Windows.Controls.TextBlock Speed;
        
        internal System.Windows.Controls.TextBlock AvgSpeed;
        
        internal System.Windows.Controls.TextBlock Altitude;
        
        internal System.Windows.Controls.TextBlock VerticalAccuracy;
        
        internal System.Windows.Controls.TextBlock HorizontalAccuracy;
        
        internal System.Windows.Controls.TextBlock Course;
        
        internal System.Windows.Controls.TextBlock GpsStatus;
        
        internal System.Windows.Controls.TextBlock IsLocationUnknown;
        
        internal System.Windows.Controls.TextBlock MagneticHeading;
        
        internal System.Windows.Controls.TextBlock TrueHeading;
        
        internal System.Windows.Controls.TextBlock HeadingAccuracy;
        
        internal System.Windows.Controls.TextBlock IsCompassDataValid;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/TakeMeThere;component/Page1.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.Latitude = ((System.Windows.Controls.TextBlock)(this.FindName("Latitude")));
            this.Longitude = ((System.Windows.Controls.TextBlock)(this.FindName("Longitude")));
            this.Speed = ((System.Windows.Controls.TextBlock)(this.FindName("Speed")));
            this.AvgSpeed = ((System.Windows.Controls.TextBlock)(this.FindName("AvgSpeed")));
            this.Altitude = ((System.Windows.Controls.TextBlock)(this.FindName("Altitude")));
            this.VerticalAccuracy = ((System.Windows.Controls.TextBlock)(this.FindName("VerticalAccuracy")));
            this.HorizontalAccuracy = ((System.Windows.Controls.TextBlock)(this.FindName("HorizontalAccuracy")));
            this.Course = ((System.Windows.Controls.TextBlock)(this.FindName("Course")));
            this.GpsStatus = ((System.Windows.Controls.TextBlock)(this.FindName("GpsStatus")));
            this.IsLocationUnknown = ((System.Windows.Controls.TextBlock)(this.FindName("IsLocationUnknown")));
            this.MagneticHeading = ((System.Windows.Controls.TextBlock)(this.FindName("MagneticHeading")));
            this.TrueHeading = ((System.Windows.Controls.TextBlock)(this.FindName("TrueHeading")));
            this.HeadingAccuracy = ((System.Windows.Controls.TextBlock)(this.FindName("HeadingAccuracy")));
            this.IsCompassDataValid = ((System.Windows.Controls.TextBlock)(this.FindName("IsCompassDataValid")));
        }
    }
}

