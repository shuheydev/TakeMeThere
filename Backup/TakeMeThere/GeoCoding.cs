using System;
using System.Threading;
using System.Device.Location;
using System.Net;
using System.Globalization;
using Microsoft.Phone.Net.NetworkInformation;
using System.Windows.Threading;



namespace TakeMeThere
{
    //自作イベント用のイベント引数として使うクラス
    //GeoCodingの結果を取得（もしくは失敗）したら発行される。
    public class DownloadGeoCodeResultCompletedEventArgs : EventArgs
    {
        public string Result;
        public GeoCoordinate Location;
        public string Status;

        public DownloadGeoCodeResultCompletedEventArgs()
        {
            this.Result = "";
            this.Location = new GeoCoordinate();
            this.Status = "";
        }
    }




    class GeoCoding
    {
        //自作イベントの宣言、実装
        public delegate void DownloadGeoCodeResultCompletedEventHandler(object sender, DownloadGeoCodeResultCompletedEventArgs e);
        public event DownloadGeoCodeResultCompletedEventHandler DownloadGeoCodeResultCompleted;
        protected virtual void OnDownloadStringCompleted(DownloadGeoCodeResultCompletedEventArgs e)//このメソッドでイベント発行。
        {
            if (DownloadGeoCodeResultCompleted != null)
            {
                DownloadGeoCodeResultCompleted(this, e);
            }
        }

        private int limitTime=1000;

        private GeoCoordinate Location;

        DispatcherTimer WebClientTimeout;//=new DispatcherTimer();

        public GeoCoding()
        {
            Location = new GeoCoordinate();

            //なくてもエラーにはならないが、ログイン画面のhtmlを取得し終えるまで待たなければならなくなる。
            WebClientTimeout = new DispatcherTimer();
            WebClientTimeout.Interval = TimeSpan.FromMilliseconds(limitTime);
            WebClientTimeout.Tick += WebClientTimeout_Tick;
        }



        public void GetAddressFromGeoCoordinate(GeoCoordinate location)
        {
            this.Location = location;

            DownloadGeoCodeResultCompletedEventArgs completedEvent = new DownloadGeoCodeResultCompletedEventArgs();


            if (location.IsUnknown == true)
            {
                completedEvent.Status = "LocationUnknown";
                OnDownloadStringCompleted(completedEvent);//イベントを発行する。
                return;
            }

            if (DeviceNetworkInformation.IsNetworkAvailable == false)
            {
                completedEvent.Status = "NoNetWork";
                OnDownloadStringCompleted(completedEvent);//イベントを発行する。
                return;
            }

            CultureInfo cc = Thread.CurrentThread.CurrentCulture;

            // URI で識別されるリソースとのデータの送受信用の共通クラス 
            WebClient downloadClient = new WebClient();

            // URL
            Uri requestURL = new Uri(string.Format("http://maps.googleapis.com/maps/api/geocode/xml?latlng={0},{1}&language={2}&sensor=false", location.Latitude, location.Longitude, cc.ToString()));
            // ジオコーティング

            //WebClientTimeout.Start();
            downloadClient.DownloadStringCompleted += downloadClient_DownloadStringCompleted;
            downloadClient.DownloadStringAsync(requestURL);

        }

        void WebClientTimeout_Tick(object sender, EventArgs e)
        {
            DownloadGeoCodeResultCompletedEventArgs completedEvent = new DownloadGeoCodeResultCompletedEventArgs();
            completedEvent.Status = "TimeOut";
            completedEvent.Location = this.Location;

            OnDownloadStringCompleted(completedEvent);//イベントを発行する。

            //WebClientTimeout.Stop();
        }

        void downloadClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            //WebClientTimeout.Stop();

            DownloadGeoCodeResultCompletedEventArgs completedEvent = new DownloadGeoCodeResultCompletedEventArgs();

            if (e.Error != null)
            {
                completedEvent.Status = "Error";
                completedEvent.Location = this.Location;
            }
            else
            {
                completedEvent.Status = "Completed";
                completedEvent.Result = e.Result;
                completedEvent.Location = this.Location;
            }

            OnDownloadStringCompleted(completedEvent);//イベントを発行する。
        }



        public void GetGeoCoordintateFromAddress(string address)
        {
            DownloadGeoCodeResultCompletedEventArgs completedEvent = new DownloadGeoCodeResultCompletedEventArgs();

            if (DeviceNetworkInformation.IsNetworkAvailable == false)
            {
                completedEvent.Status = "NoNetWork";
                OnDownloadStringCompleted(completedEvent);//イベントを発行する。
                return;
            }

            CultureInfo cc = Thread.CurrentThread.CurrentCulture;
            // URI で識別されるリソースとのデータの送受信用の共通クラス 
            WebClient downloadClient = new WebClient();
            string encodedAddress = HttpUtility.UrlEncode(address);
            System.Diagnostics.Debug.WriteLine(encodedAddress);
            // URL
            Uri requestURL = new Uri(string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&language={1}&sensor=false",address, cc.ToString()));
            /*
             * # 住所から緯度・経度を取得

                 https://maps.google.co.jp/maps?hl=ja&q=%E6%9C%AD%E5%B9%8C&lr=lang_ja&ie=UTF-8&ei=1D9UUruaEOKFiQewxICgDg&ved=0CAoQ_AUoAg
              */
            // ジオコーティング
            //string encodedRequestURL = HttpUtility.UrlEncode(string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&language={1}&sensor=false",address, cc.ToString()));
            System.Diagnostics.Debug.WriteLine(requestURL);
            downloadClient.DownloadStringCompleted += downloadClient_DownloadStringCompleted;
            downloadClient.DownloadStringAsync(requestURL);


        }



    }
}
