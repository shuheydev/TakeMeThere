﻿#pragma checksum "D:\Document\プログラミング\windows phone app\Projects\TakeMeThere_まとめフォルダ\20140214_TakeMeThere_ver1.1.2.0\TakeMeThere\MapPage_gMap.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "AA9255CFE5511E0378B611672D2E0B48"
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
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Shell;
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
    
    
    public partial class MapPage_gMap : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal Microsoft.Phone.Controls.Maps.Map MyMap;
        
        internal Microsoft.Phone.Controls.Maps.MapTileLayer MapTileLayer_gMap;
        
        internal Microsoft.Phone.Controls.Maps.Pushpin CurrentMark;
        
        internal System.Windows.Media.RotateTransform CurrentMarkRotate;
        
        internal Microsoft.Phone.Controls.Maps.MapItemsControl MapItemsControl_PushPins;
        
        internal System.Windows.Controls.Grid Grid_GPSStatus;
        
        internal System.Windows.Controls.TextBlock TextBlock_GPSStatus;
        
        internal System.Windows.Controls.Grid Grid_SearchBox;
        
        internal System.Windows.Controls.TextBox TextBox_SearchBox;
        
        internal System.Windows.Controls.Grid Grid_Search;
        
        internal System.Windows.Controls.Grid Grid_ZoomUp;
        
        internal System.Windows.Controls.Grid Grid_ZoomDown;
        
        internal System.Windows.Controls.Grid Grid_GoCurrentLocation;
        
        internal System.Windows.Controls.Grid Grid_GoTargetLocation;
        
        internal System.Windows.Controls.Grid Grid_TargetName;
        
        internal System.Windows.Controls.TextBlock TextBlock_TargetName;
        
        internal System.Windows.Controls.Grid Grid_Info;
        
        internal System.Windows.Controls.Grid Grid_VisibilitySwitch;
        
        internal System.Windows.Controls.Grid Grid_LockPin;
        
        internal System.Windows.Controls.TextBlock TextBlock_LockPin;
        
        internal System.Windows.Controls.Grid Grid_Distance;
        
        internal System.Windows.Controls.TextBlock TextBlock_Distance;
        
        internal System.Windows.Controls.TextBlock TextBlock_Distance_Unit;
        
        internal System.Windows.Controls.Grid Grid_EstimateTimeArrival;
        
        internal System.Windows.Controls.TextBlock TextBlock_ETA_Unit;
        
        internal System.Windows.Controls.TextBlock TextBlock_ETA;
        
        internal System.Windows.Controls.TextBlock TextBlock_AvgSpeed;
        
        internal System.Windows.Controls.Grid Grid_TwoPoint_Distance;
        
        internal System.Windows.Controls.TextBlock TextBlock_TwoPoint_Distance;
        
        internal System.Windows.Controls.TextBlock TextBlock_TwoPoint_Distance_Unit;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton Appbar_CompassButton;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton Appbar_ListButton;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/TakeMeThere;component/MapPage_gMap.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.MyMap = ((Microsoft.Phone.Controls.Maps.Map)(this.FindName("MyMap")));
            this.MapTileLayer_gMap = ((Microsoft.Phone.Controls.Maps.MapTileLayer)(this.FindName("MapTileLayer_gMap")));
            this.CurrentMark = ((Microsoft.Phone.Controls.Maps.Pushpin)(this.FindName("CurrentMark")));
            this.CurrentMarkRotate = ((System.Windows.Media.RotateTransform)(this.FindName("CurrentMarkRotate")));
            this.MapItemsControl_PushPins = ((Microsoft.Phone.Controls.Maps.MapItemsControl)(this.FindName("MapItemsControl_PushPins")));
            this.Grid_GPSStatus = ((System.Windows.Controls.Grid)(this.FindName("Grid_GPSStatus")));
            this.TextBlock_GPSStatus = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_GPSStatus")));
            this.Grid_SearchBox = ((System.Windows.Controls.Grid)(this.FindName("Grid_SearchBox")));
            this.TextBox_SearchBox = ((System.Windows.Controls.TextBox)(this.FindName("TextBox_SearchBox")));
            this.Grid_Search = ((System.Windows.Controls.Grid)(this.FindName("Grid_Search")));
            this.Grid_ZoomUp = ((System.Windows.Controls.Grid)(this.FindName("Grid_ZoomUp")));
            this.Grid_ZoomDown = ((System.Windows.Controls.Grid)(this.FindName("Grid_ZoomDown")));
            this.Grid_GoCurrentLocation = ((System.Windows.Controls.Grid)(this.FindName("Grid_GoCurrentLocation")));
            this.Grid_GoTargetLocation = ((System.Windows.Controls.Grid)(this.FindName("Grid_GoTargetLocation")));
            this.Grid_TargetName = ((System.Windows.Controls.Grid)(this.FindName("Grid_TargetName")));
            this.TextBlock_TargetName = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_TargetName")));
            this.Grid_Info = ((System.Windows.Controls.Grid)(this.FindName("Grid_Info")));
            this.Grid_VisibilitySwitch = ((System.Windows.Controls.Grid)(this.FindName("Grid_VisibilitySwitch")));
            this.Grid_LockPin = ((System.Windows.Controls.Grid)(this.FindName("Grid_LockPin")));
            this.TextBlock_LockPin = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_LockPin")));
            this.Grid_Distance = ((System.Windows.Controls.Grid)(this.FindName("Grid_Distance")));
            this.TextBlock_Distance = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_Distance")));
            this.TextBlock_Distance_Unit = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_Distance_Unit")));
            this.Grid_EstimateTimeArrival = ((System.Windows.Controls.Grid)(this.FindName("Grid_EstimateTimeArrival")));
            this.TextBlock_ETA_Unit = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_ETA_Unit")));
            this.TextBlock_ETA = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_ETA")));
            this.TextBlock_AvgSpeed = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_AvgSpeed")));
            this.Grid_TwoPoint_Distance = ((System.Windows.Controls.Grid)(this.FindName("Grid_TwoPoint_Distance")));
            this.TextBlock_TwoPoint_Distance = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_TwoPoint_Distance")));
            this.TextBlock_TwoPoint_Distance_Unit = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_TwoPoint_Distance_Unit")));
            this.Appbar_CompassButton = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("Appbar_CompassButton")));
            this.Appbar_ListButton = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("Appbar_ListButton")));
        }
    }
}

