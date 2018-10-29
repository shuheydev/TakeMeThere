using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using Mangopollo;


namespace TakeMeThere
{
    public partial class App : Application
    {
        /// <summary>
        /// Phone アプリケーションのルート フレームへの容易なアクセスを提供します。
        /// </summary>
        /// <returns>Phone アプリケーションのルート フレームです。</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Application オブジェクトのコンストラクターです。
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone 固有の初期化
            InitializePhoneApplication();

            // デバッグ中にグラフィックスのプロファイル情報を表示します。
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // 現在のフレーム レート カウンターを表示します。
                Application.Current.Host.Settings.EnableFrameRateCounter = false;

                // 各フレームで再描画されているアプリケーションの領域を表示します。
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // これにより、色付きのオーバーレイを使用して、GPU に渡されるページの領域が表示されます。
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // 注意: これはデバッグ モードのみで使用してください。ユーザーが電話を使用していないときに、ユーザーのアイドル状態の検出を無効にする
                // アプリケーションが引き続き実行され、バッテリ電源が消耗します。
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }
            

        }



        public AppDB DB { get; set; }
        public ViewModel PushPinView { get; set; }
        //public DateTime TargetPinID { get; set; }

        public class MyIsolatedStorageSettings
        {
            public double ZoomLevel { get; set; }
            public double CenterLatitude { get; set; }
            public double CenterLongitude { get; set; }
            public double TimeSpanRefreshCompass_main { get; set; }
            public double FactorToCorrectDirection_main { get; set; }


        }
        // (たとえば、[スタート] メニューから) アプリケーションが起動するときに実行されるコード
        // このコードは、アプリケーションが再アクティブ化済みの場合には実行されません
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            if (DB == null)
                DB = new AppDB();

            PushPinView = DB.LoadInfoFromXML();

            IsolatedStorageSettings isolateStore = IsolatedStorageSettings.ApplicationSettings;
            #region isoratesettingの設定。
            //Target変更フラグ用。エントリーがなければ作成する。

            if (isolateStore.Contains("ZoomLevel") == false)
            {
                isolateStore["ZoomLevel"] = (double)1;
            }



            if (isolateStore.Contains("CenterLatitude") == false)
            {
                isolateStore["CenterLatitude"] = (double)(0);
            }


            if (isolateStore.Contains("CenterLongitude") == false)
            {
                isolateStore["CenterLongitude"] = (double)(0);
            }



            if (isolateStore.Contains("TimeSpanRefreshCompass_main") == false)
            {
                isolateStore["TimeSpanRefreshCompass_main"] = (double)66;
            }



            if (isolateStore.Contains("FactorToCorrectDirection_main") == false)
            {
                isolateStore["FactorToCorrectDirection_main"] = (double)5;
            }



            if (isolateStore.Contains("TimeSpanUpdateCompass_sensor") == false)
            {
                isolateStore["TimeSpanUpdateCompass_sensor"] = (double)300;
            }




            if (isolateStore.Contains("DistanceUpdateGPS_sensor") == false)
            {
                isolateStore["DistanceUpdateGPS_sensor"] = (double)5;
            }



            if (isolateStore.Contains("TimeSpanRefreshCompass_map") == false)
            {
                isolateStore["TimeSpanRefreshCompass_map"] = (double)66;
            }

            if (isolateStore.Contains("Performance_Level") == false)
            {
                isolateStore["Performance_Level"] = (double)3;
            }

            if (isolateStore.Contains("MapAnimation") == false)
            {
                isolateStore["MapAnimation"] = true;
            }

            if (isolateStore.Contains("LocationService") == false)
            {
                isolateStore["LocationService"] = false;
            }

            if (isolateStore.Contains("FirstTimeMessagePop") == false)
            {
                isolateStore["FirstTimeMessagePop"] = false;
            }


            if (isolateStore.Contains("FactorToCorrectDirection_map") == false)
            {
                isolateStore["FactorToCorrectDirection_map"] = (double)5;
            }

            if (isolateStore.Contains("MapPageTileVisibility") == false)
            {
                isolateStore["MapPageTileVisibility"] = true;
            }

            if (isolateStore.Contains("MapPageLockPin") == false)
            {
                isolateStore["MapPageLockPin"] = false;
            }

            if (isolateStore.Contains("AverageSpeed") == false)
            {
                isolateStore["AverageSpeed"] = (double)(0);
            }
            if (isolateStore.Contains("AverageSpeedChanged") == false)
            {
                isolateStore["AverageSpeedChanged"] = false;
            }


            if (isolateStore.Contains("PreferredEmail") == false)
            {
                isolateStore["PreferredEmail"] = "";
            }

            if (isolateStore.Contains("Map") == false)
            {
                isolateStore["Map"] = "";
            }

            if (isolateStore.Contains("Unit") == false)
            {
                isolateStore["Unit"] = "International";
            }



            if (isolateStore.Contains("Sort") == false)
            {
                isolateStore["Sort"] = "Name";
            }

            if (isolateStore.Contains("SearchKeyword") == false)
            {
                isolateStore["SearchKeyword"] = "";
            }

            if (IsolatedStorageSettings.ApplicationSettings.Contains("SelectedItemID") == false)
            {
                IsolatedStorageSettings.ApplicationSettings["SelectedItemID"] = "";
            }



            isolateStore.Save();
            #endregion

        }

        // アプリケーションがアクティブになった (前面に表示された) ときに実行されるコード
        // このコードは、アプリケーションの初回起動時には実行されません
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {

        }

        // アプリケーションが非アクティブになった (バックグラウンドに送信された) ときに実行されるコード
        // このコードは、アプリケーションの終了時には実行されません
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            DB.SaveInfoToIsoStrage(PushPinView);
            updateLiveTile();
        }

        // (たとえば、ユーザーが戻るボタンを押して) アプリケーションが終了するときに実行されるコード
        // このコードは、アプリケーションが非アクティブになっているときには実行されません
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            DB.SaveInfoToIsoStrage(PushPinView);
            updateLiveTile();
        }

        // ナビゲーションに失敗した場合に実行されるコード
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            DB.SaveInfoToIsoStrage(PushPinView);

            if (System.Diagnostics.Debugger.IsAttached)
            {

                // ナビゲーションに失敗しました。デバッガーで中断します。
                System.Diagnostics.Debugger.Break();
                MessageBox.Show("App Crashed.Sorry");
            }
        }

        // ハンドルされない例外の発生時に実行されるコード
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // ハンドルされない例外が発生しました。デバッガーで中断します。
                //System.Diagnostics.Debugger.Break();
                MessageBox.Show("App Crashed.Sorry.");
            }
             


        }


        private void updateLiveTile()
        {
            ShellTile tileToUpdate = ShellTile.ActiveTiles.First();    // 2if (tileToFind == null)
            if (tileToUpdate != null)
            {
                var tile =new Mangopollo.Tiles.IconicTileData();// new StandardTileData();// 3
                
                tile.Title = "TakeMeThere";
                if (PushPinView.PushPins.Count() != 0)
                {
                    var pin = PushPinView.GetTarget();//最後に開いたnoteを取得
                    //BackgroundImage = new Uri("Background.png", UriKind.Relative),
                    if (pin != null)
                    {
                        //var content = pin.Name + System.Environment.NewLine + pin.Address + System.Environment.NewLine;


                        //tile.BackTitle = "Destination";
                        //tile.BackBackgroundImage = new Uri("BackBackground.png", UriKind.Relative);
                        //tile.BackContent = content;
                    }
                    tile.Title = "TakeMeThere";

                    tile.IconImage = new Uri("TakeMeThere_Icon_done_99.png", UriKind.Relative);
                    tile.Count = PushPinView.PushPins.Count();
                }
                else
                {
                    //BackgroundImage = new Uri("Background.png", UriKind.Relative),
                    tile.Title = "TakeMeThere";
                    tile.Count = 0;
                    tile.IconImage = new Uri("TakeMeThere_Icon_done_99.png", UriKind.Relative);
                    //tile.BackTitle = "";
                    //tile.BackBackgroundImage = new Uri("BackBackground.png", UriKind.Relative);
                    //tile.BackContent = "No Items";
                }
                
                tileToUpdate.Update(tile);

            }
        }

        /*
        private void updateLiveTile()
        {
            Version TargetVersion = new Version(7, 10, 8858);
            if (Environment.OSVersion.Version >= TargetVersion)
            {

                Type flipTileDataType = Type.GetType("Microsoft.Phone.Shell.FlipTileData,Microsoft.Phone");
                Type shellTileType = Type.GetType("Microsoft.Phone.Shell.ShellTile,Microsoft.Phone");
                var tileToUpdate = ShellTile.ActiveTiles.First();
                if (tileToUpdate != null)
                {
                    var UpdateTileData = flipTileDataType.GetConstructor(new Type[] { }).Invoke(null);

                    // Set the properties. 
                    SetProperty(UpdateTileData, "Title", "TakeMeThere");

                    //SetProperty(UpdateTileData, "SmallBackgroundImage","");
                    //SetProperty(UpdateTileData, "BackBackgroundImage", "");
                    SetProperty(UpdateTileData, "WideBackgroundImage", "/blank_background.png");
                    //SetProperty(UpdateTileData, "WideBackBackgroundImage", "")
                    if (PushPinView.PushPins.Count() != 0)
                    {
                        var pin = PushPinView.GetTarget();//目的地を取得
                        if (pin != null)
                        {
                            var content = pin.Name + System.Environment.NewLine + pin.Address + System.Environment.NewLine;
                            SetProperty(UpdateTileData, "WideBackContent", content);
                            SetProperty(UpdateTileData, "BackTitle", "Destination");
                            SetProperty(UpdateTileData, "BackContent", content);
                        }
                        SetProperty(UpdateTileData, "Count", PushPinView.PushPins.Count);
                        shellTileType.GetMethod("Update").Invoke(tileToUpdate, new Object[] { UpdateTileData });
                    }
                    else
                    {
                        SetProperty(UpdateTileData, "WideBackContent", "");
                        SetProperty(UpdateTileData, "Count", 0);
                        SetProperty(UpdateTileData, "BackTitle", "");
                        SetProperty(UpdateTileData, "BackContent", "");

                        shellTileType.GetMethod("Update").Invoke(tileToUpdate, new Object[] { UpdateTileData });
                    }
                    // Invoke the new version of ShellTile.Update.

                }
            }
        }
        private static void SetProperty(object instance, string name, object value)
        {
            var setMethod = instance.GetType().GetProperty(name).GetSetMethod();
            setMethod.Invoke(instance, new object[] { value });
        }
        */

        #region Phone アプリケーションの初期化

        // 初期化の重複を回避します
        private bool phoneApplicationInitialized = false;

        // このメソッドに新たなコードを追加しないでください
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // フレームを作成しますが、まだ RootVisual に設定しないでください。これによって、アプリケーションがレンダリングできる状態になるまで、
            // スプラッシュ スクリーンをアクティブなままにすることができます。
            //RootFrame = new PhoneApplicationFrame();
            RootFrame = new TransitionFrame();

            RootFrame.Language = System.Windows.Markup.XmlLanguage.GetLanguage(
                System.Globalization.CultureInfo.CurrentUICulture.Name);

            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // ナビゲーション エラーを処理します
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // 再初期化しないようにします
            phoneApplicationInitialized = true;
        }

        // このメソッドに新たなコードを追加しないでください
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // ルート visual を設定してアプリケーションをレンダリングできるようにします
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // このハンドラーは必要なくなったため、削除します
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}