﻿#pragma checksum "D:\Document\プログラミング\windows phone app\Projects\TakeMeThere_まとめフォルダ\20140214_TakeMeThere_ver1.1.2.0\TakeMeThere\DetailPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C6A347DBD77BF7B6F5875670FCAD1E46"
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
    
    
    public partial class DetailPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal Microsoft.Phone.Controls.Panorama Panorama;
        
        internal Microsoft.Phone.Controls.PhoneTextBox TextBox_Name;
        
        internal Microsoft.Phone.Controls.PhoneTextBox TextBox_Latitude;
        
        internal Microsoft.Phone.Controls.PhoneTextBox TextBox_Longitude;
        
        internal Microsoft.Phone.Controls.PhoneTextBox TextBox_Address;
        
        internal Microsoft.Phone.Controls.PhoneTextBox TextBox_PhoneNum;
        
        internal System.Windows.Controls.Grid Grid_PhoneCall;
        
        internal System.Windows.Controls.Image Image_Phone;
        
        internal System.Windows.Controls.Image Image_BaseCircle;
        
        internal System.Windows.Controls.ScrollViewer ScrollViewer_Note;
        
        internal Microsoft.Phone.Controls.PhoneTextBox TextBox_Note;
        
        internal System.Windows.Controls.CheckBox CheckBox_IsTarget;
        
        internal System.Windows.Controls.CheckBox CheckBox_IsVisible;
        
        internal System.Windows.Controls.TextBlock TextBlock_CreateDate;
        
        internal System.Windows.Controls.TextBlock TextBlock_CreateDayOfWeek;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton SaveButton;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton DownloadButton;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/TakeMeThere;component/DetailPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.Panorama = ((Microsoft.Phone.Controls.Panorama)(this.FindName("Panorama")));
            this.TextBox_Name = ((Microsoft.Phone.Controls.PhoneTextBox)(this.FindName("TextBox_Name")));
            this.TextBox_Latitude = ((Microsoft.Phone.Controls.PhoneTextBox)(this.FindName("TextBox_Latitude")));
            this.TextBox_Longitude = ((Microsoft.Phone.Controls.PhoneTextBox)(this.FindName("TextBox_Longitude")));
            this.TextBox_Address = ((Microsoft.Phone.Controls.PhoneTextBox)(this.FindName("TextBox_Address")));
            this.TextBox_PhoneNum = ((Microsoft.Phone.Controls.PhoneTextBox)(this.FindName("TextBox_PhoneNum")));
            this.Grid_PhoneCall = ((System.Windows.Controls.Grid)(this.FindName("Grid_PhoneCall")));
            this.Image_Phone = ((System.Windows.Controls.Image)(this.FindName("Image_Phone")));
            this.Image_BaseCircle = ((System.Windows.Controls.Image)(this.FindName("Image_BaseCircle")));
            this.ScrollViewer_Note = ((System.Windows.Controls.ScrollViewer)(this.FindName("ScrollViewer_Note")));
            this.TextBox_Note = ((Microsoft.Phone.Controls.PhoneTextBox)(this.FindName("TextBox_Note")));
            this.CheckBox_IsTarget = ((System.Windows.Controls.CheckBox)(this.FindName("CheckBox_IsTarget")));
            this.CheckBox_IsVisible = ((System.Windows.Controls.CheckBox)(this.FindName("CheckBox_IsVisible")));
            this.TextBlock_CreateDate = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_CreateDate")));
            this.TextBlock_CreateDayOfWeek = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlock_CreateDayOfWeek")));
            this.SaveButton = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("SaveButton")));
            this.DownloadButton = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("DownloadButton")));
        }
    }
}

