﻿#pragma checksum "F:\Document\プログラミング\windows phone app\Projects\TakeMeThere\TakeMeThere\DestinationInformationPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C5470FDF00C26544E2766F4577A50AC9"
//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.296
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
    
    
    public partial class DestinationInfomationPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.TextBox TextBox_Name;
        
        internal System.Windows.Controls.TextBox TextBox_Latitude;
        
        internal System.Windows.Controls.TextBox TextBox_Longitude;
        
        internal System.Windows.Controls.CheckBox CheckBox_IsTarget;
        
        internal System.Windows.Controls.TextBox TextBox_Note;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton EditButton;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton SaveButton;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/TakeMeThere;component/DestinationInformationPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.TextBox_Name = ((System.Windows.Controls.TextBox)(this.FindName("TextBox_Name")));
            this.TextBox_Latitude = ((System.Windows.Controls.TextBox)(this.FindName("TextBox_Latitude")));
            this.TextBox_Longitude = ((System.Windows.Controls.TextBox)(this.FindName("TextBox_Longitude")));
            this.CheckBox_IsTarget = ((System.Windows.Controls.CheckBox)(this.FindName("CheckBox_IsTarget")));
            this.TextBox_Note = ((System.Windows.Controls.TextBox)(this.FindName("TextBox_Note")));
            this.EditButton = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("EditButton")));
            this.SaveButton = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("SaveButton")));
        }
    }
}
