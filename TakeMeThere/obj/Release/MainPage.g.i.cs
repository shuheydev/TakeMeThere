﻿#pragma checksum "D:\Document\プログラミング\windows phone app\Projects\TakeMeThere_まとめフォルダ\20140214_TakeMeThere_ver1.1.2.0\TakeMeThere\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "1800AB12E4F975A2E8225F2D31693155"
//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.34014
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
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
    
    
    public partial class MainPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.Grid Grid_Compass;
        
        internal System.Windows.Media.RotateTransform TargetRotate;
        
        internal System.Windows.Media.RotateTransform DialRotate;
        
        internal System.Windows.Controls.Grid Grid_Speed;
        
        internal System.Windows.Controls.TextBlock TextBlock_Speed;
        
        internal System.Windows.Controls.TextBlock TextBlock_Speed_Unit;
        
        internal System.Windows.Controls.Grid Grid_AvgSpeed;
        
        internal System.Windows.Controls.TextBlock TextBlock_AvgSpeed;
        
        internal System.Windows.Controls.TextBlock TextBlock_AvgSpeed_Unit;
        
        internal System.Windows.Controls.Grid Grid_Course_Compass;
        
        internal System.Windows.Media.RotateTransform CourseTargetRotate;
        
        internal System.Windows.Media.RotateTransform CourseDialRotate;
        
        internal System.Windows.Controls.Grid Grid_EstimateArrivalTime;
        
        internal System.Windows.Controls.TextBlock TextBlock_EstimateArrivalTime;
        
        internal System.Windows.Controls.TextBlock TextBlock_AvgSpeed_for_ETA;
        
        internal System.Windows.Controls.TextBlock TextBlock_CurrentTime;
        
        internal System.Windows.Controls.TextBlock TextBlock_ETA;
        
        internal System.Windows.Controls.TextBlock TextBlock_ETA_Unit;
        
        internal System.Windows.Controls.Image Image_Arrow;
        
        internal System.Windows.Controls.TextBlock TextBlock_EAT_Text;
        
        internal System.Windows.Controls.Grid Grid_Direction;
        
        internal System.Windows.Controls.TextBlock TextBlock_DirectionSign;
        
        internal System.Windows.Controls.TextBlock TextBlock_TrueHeading;
        
        internal System.Windows.Controls.Grid Grid_Distance;
        
        internal System.Windows.Controls.TextBlock TextBlock_Distance;
        
        internal System.Windows.Controls.TextBlock TextBlock_Distance_Unit;
        
        internal System.Windows.Controls.Grid Grid_TargetLocation;
        
        internal System.Windows.Controls.TextBlock TextBlock_TargetName;
        
        internal System.Windows.Controls.TextBlock TextBlock_TargetLatitude;
        
        internal System.Windows.Controls.TextBlock TextBlock_TargetLongitude;
        
        internal System.Windows.Controls.Grid Grid_GoTargetLocation;
        
        internal System.Windows.Controls.Grid Grid_GPSAccuracy;
        
        internal System.Windows.Controls.TextBlock TextBlock_HorizontalAccuracy;
        
        internal System.Windows.Controls.TextBlock TextBlock_HorizontalAccuracy_Unit;
        
        internal System.Windows.Controls.Grid Grid_GPSStatus;
        
        internal System.Windows.Controls.Image Image_GPSStatus;
        
        internal System.Windows.Controls.TextBlock TextBlock_GPSStatus;
        
        internal System.Windows.Controls.Grid Grid_CurrenLocation;
        
        internal System.Windows.Controls.TextBlock TextBlock_CurrentLatitude;
        
        internal System.Windows.Controls.TextBlock TextBlock_CurrentLongitude;
        
        internal System.Windows.Controls.Grid Grid_GoCurrentLocaion;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton Appbar_ListButton;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton Appbar_MapButton;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/TakeMeThere;component/MainPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.Grid_Compass = ((System.Windows.Controls.Grid)(this.FindName("Grid_Compass")));
            this.TargetRotate = ((System.Windows.Media.RotateTransform)(this.FindName("TargetRotate")));
            this.DialRotate = ((System.Windows.Media.RotateTransform)(this.FindName("DialRotate")));
            this.Grid_Speed = ((System.Windows.Controls.Grid)(this.FindName("Grid_Speed")));
            this.TextBlock_Speed = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_Speed")));
            this.TextBlock_Speed_Unit = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_Speed_Unit")));
            this.Grid_AvgSpeed = ((System.Windows.Controls.Grid)(this.FindName("Grid_AvgSpeed")));
            this.TextBlock_AvgSpeed = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_AvgSpeed")));
            this.TextBlock_AvgSpeed_Unit = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_AvgSpeed_Unit")));
            this.Grid_Course_Compass = ((System.Windows.Controls.Grid)(this.FindName("Grid_Course_Compass")));
            this.CourseTargetRotate = ((System.Windows.Media.RotateTransform)(this.FindName("CourseTargetRotate")));
            this.CourseDialRotate = ((System.Windows.Media.RotateTransform)(this.FindName("CourseDialRotate")));
            this.Grid_EstimateArrivalTime = ((System.Windows.Controls.Grid)(this.FindName("Grid_EstimateArrivalTime")));
            this.TextBlock_EstimateArrivalTime = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_EstimateArrivalTime")));
            this.TextBlock_AvgSpeed_for_ETA = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_AvgSpeed_for_ETA")));
            this.TextBlock_CurrentTime = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_CurrentTime")));
            this.TextBlock_ETA = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_ETA")));
            this.TextBlock_ETA_Unit = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_ETA_Unit")));
            this.Image_Arrow = ((System.Windows.Controls.Image)(this.FindName("Image_Arrow")));
            this.TextBlock_EAT_Text = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_EAT_Text")));
            this.Grid_Direction = ((System.Windows.Controls.Grid)(this.FindName("Grid_Direction")));
            this.TextBlock_DirectionSign = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_DirectionSign")));
            this.TextBlock_TrueHeading = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_TrueHeading")));
            this.Grid_Distance = ((System.Windows.Controls.Grid)(this.FindName("Grid_Distance")));
            this.TextBlock_Distance = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_Distance")));
            this.TextBlock_Distance_Unit = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_Distance_Unit")));
            this.Grid_TargetLocation = ((System.Windows.Controls.Grid)(this.FindName("Grid_TargetLocation")));
            this.TextBlock_TargetName = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_TargetName")));
            this.TextBlock_TargetLatitude = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_TargetLatitude")));
            this.TextBlock_TargetLongitude = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_TargetLongitude")));
            this.Grid_GoTargetLocation = ((System.Windows.Controls.Grid)(this.FindName("Grid_GoTargetLocation")));
            this.Grid_GPSAccuracy = ((System.Windows.Controls.Grid)(this.FindName("Grid_GPSAccuracy")));
            this.TextBlock_HorizontalAccuracy = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_HorizontalAccuracy")));
            this.TextBlock_HorizontalAccuracy_Unit = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_HorizontalAccuracy_Unit")));
            this.Grid_GPSStatus = ((System.Windows.Controls.Grid)(this.FindName("Grid_GPSStatus")));
            this.Image_GPSStatus = ((System.Windows.Controls.Image)(this.FindName("Image_GPSStatus")));
            this.TextBlock_GPSStatus = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_GPSStatus")));
            this.Grid_CurrenLocation = ((System.Windows.Controls.Grid)(this.FindName("Grid_CurrenLocation")));
            this.TextBlock_CurrentLatitude = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_CurrentLatitude")));
            this.TextBlock_CurrentLongitude = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_CurrentLongitude")));
            this.Grid_GoCurrentLocaion = ((System.Windows.Controls.Grid)(this.FindName("Grid_GoCurrentLocaion")));
            this.Appbar_ListButton = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("Appbar_ListButton")));
            this.Appbar_MapButton = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("Appbar_MapButton")));
        }
    }
}

