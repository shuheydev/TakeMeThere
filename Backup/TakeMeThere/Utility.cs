using System;
using System.Device.Location;
//using System.Linq;
using System.Windows.Media;
using System.IO.IsolatedStorage;


namespace TakeMeThere
{
    class Utility
    {



        //Hex形式のARGB値をColorオブジェクトへ変換する。矢印の色を変更するときのため。
        //参考：http://ch3cooh.hatenablog.jp/entry/20110927/1317141608
        public static Color GetColorFromHexString(string s)
        {
            byte a = System.Convert.ToByte(s.Substring(0, 2), 16);
            byte r = System.Convert.ToByte(s.Substring(2, 2), 16);
            byte g = System.Convert.ToByte(s.Substring(4, 2), 16);
            byte b = System.Convert.ToByte(s.Substring(6, 2), 16);
            
            return Color.FromArgb(a, r, g, b);
        }

        public static Color GetColorFromHexStringFullOpacity(string s)
        {
            byte a = System.Convert.ToByte("FF", 16);
            byte r = System.Convert.ToByte(s.Substring(2, 2), 16);
            byte g = System.Convert.ToByte(s.Substring(4, 2), 16);
            byte b = System.Convert.ToByte(s.Substring(6, 2), 16);

            return Color.FromArgb(a, r, g, b);
        }

        public static string GetHexStringFromColor(Color color)
        {
            byte a = color.A;
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;

            string a_Hex;
            if (a > 15)
                a_Hex = a.ToString("X");
            else
                a_Hex = "0" + a.ToString("X");

            string r_Hex;
            if (r > 15)
                r_Hex = r.ToString("X");
            else
                r_Hex = "0" + r.ToString("X");

            string g_Hex;
            if (g > 15)
                g_Hex = g.ToString("X");
            else
                g_Hex = "0" + g.ToString("X");

            string b_Hex;
            if (b > 15)
                b_Hex = b.ToString("X");
            else
                b_Hex = "0" + b.ToString("X");

            return a_Hex+r_Hex+g_Hex+b_Hex;
        }
        public static Color ConvertToFullOpacityColor(Color color)
        {
            color.A = (byte)255;
            return color;
        }

        //単位を付けて文字列として返す
        public static string ConvertDistanceWithUnit(double dist)
        {
            string SystemOfUnit = (string)IsolatedStorageSettings.ApplicationSettings["Unit"];

            if (SystemOfUnit == "International")
            {
                //目的地までの距離を表示用に変換
                if (dist >= 1000)
                {
                    //km
                    return (dist / 1000).ToString("0.0") + " km";
                }
                else
                {
                    //m
                    return dist.ToString("0") + " m";
                }
            }
            else
            {
                var yard = dist / 0.9144;
                //目的地までの距離を表示用に変換
                if (yard >= 1760)
                {
                    //mile
                    return (yard / 1760).ToString("0.0") + " mile";
                }
                else
                {
                    //yard
                    return yard.ToString("0") + " yard";
                }
            }
        }

        public static string ConvertSpeedWithUnit(double speed)
        {
            //speedはm/s
            string SystemOfUnit = (string)IsolatedStorageSettings.ApplicationSettings["Unit"];

            if (Double.IsNaN(speed))
                speed = 0;

            if (SystemOfUnit == "International")
            {
                    //km
                return (speed * 60 * 60 / 1000).ToString("0.0") + " km/h";
            }
            else
            {
                var yardSpeed=speed/0.9144; //yard/s

                return (yardSpeed * 60 * 60 / 1760).ToString("0.0") + " mile/h";
            }



        }

        public static string ConvertETAWithUnit(double eta)
        {
            string etaWithUnit = "";
            double _s = eta;
            double _m = (_s / 60);
            //var _m_frac = (eta / 60) % 1;
            double _h = (_m / 60);

            if (eta == -1)
                return "--";

            if (Double.IsPositiveInfinity(eta) || Double.IsNaN(eta))
                eta = 0;

            if (_h >= 1)
            {
                etaWithUnit = _h.ToString("F1") + " hours";
            }
            else
            {
                if (_m >= 1)
                {
                    etaWithUnit = _m.ToString("F1") + " minutes";
                }
                else
                {
                    etaWithUnit = _s.ToString("F0") + " seconds";
                }
            }

            return etaWithUnit;
        }


        public static string ConvertDegreeToSign(double degree)
        {
            string sign="N";


            if (0 <= degree && degree <= 22.5)
            {
                sign = "N";
            }
            if (22.5 < degree && degree <= 67.5)
            {
                sign = "NE";
            }
            if (67.5 < degree && degree <= 112.5)
            {
                sign = "E";
            }
            if (112.5 < degree && degree <=157.5 )
            {
                sign = "SE";
            }
            if (157.5 < degree && degree <= 202.5)
            {
                sign = "S";
            }
            if (202.5 < degree && degree <= 247.5)
            {
                sign = "SW";
            }
            if (247.5 < degree && degree <= 292.5)
            {
                sign = "W";
            }
            if (292.5 < degree && degree <= 337.5)
            {
                sign = "NW";
            }
            if (337.5 < degree && degree <= 360)
            {
                sign = "N";
            }

            return sign;
        }

        public static string ConvertDegreeToDMM(double degree)
        {
            string dmmString;
            
            int _d = (int)degree;//整数部分。度
            double _d_frac = degree % 1; 
            int _m = (int)(_d_frac*60);
            double _s = (((_d_frac*60)%1)*60);
            
            dmmString = _d + "°" + _m + "'" + _s.ToString("F3") + "''" ;

            return dmmString;
        }

        public static System.Windows.Visibility GetVisibilityFromString(string v)
        {
            System.Windows.Visibility visibility = System.Windows.Visibility.Collapsed;
            //MessageBox.Show("util "+v);
            if (v == System.Windows.Visibility.Visible.ToString())
                visibility= System.Windows.Visibility.Visible;
            if (v == System.Windows.Visibility.Collapsed.ToString())
                visibility= System.Windows.Visibility.Collapsed;

            return visibility;
        }



        #region 距離や角度の計算
        public static double CalcDistanceTo(GeoCoordinate current,GeoCoordinate target)
        {
            if (current.IsUnknown == true || target.IsUnknown== true)
            {
                return double.NaN;
            }

            return current.GetDistanceTo(target);
        }



        public static double CalcDistanceTo_Hubeny(GeoCoordinate current, GeoCoordinate target)
        {
            
            if (current.IsUnknown == true || target.IsUnknown == true)
            {
                return double.NaN;
            }

            const double equationalRadius = 6378137.000;
            const double polarRadius = 6356752.314245;

            double eSquared = (equationalRadius * equationalRadius - polarRadius * polarRadius) / (equationalRadius * equationalRadius);

            double numerator = equationalRadius * (1 - eSquared);

            //度からラジアンに変換
            var lonRad1 = current.Longitude * Math.PI / 180;
            var latRad1 = current.Latitude * Math.PI / 180;
            var lonRad2 = target.Longitude * Math.PI / 180;
            var latRad2 = target.Latitude * Math.PI / 180;
            var deltaLat = latRad1 - latRad2;
            var deltaLon = lonRad1 - lonRad2;
            var avgLat = (latRad1 + latRad2) / 2;
            var sin = Math.Sin(avgLat);
            var doubleu = Math.Sqrt(1 - eSquared * sin * sin);
            //子午線曲率半径
            var meridian = numerator / (doubleu * doubleu * doubleu);
            //卯酉線曲率半径
            var primeVertical = equationalRadius / doubleu;
            var deltaY = deltaLat * meridian;
            var deltaX = deltaLon * primeVertical * Math.Cos(avgLat);
            //return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }



        //緯度、経度から目的地の方角を計算する。
        //参考：http://hamasyou.com/blog/archives/000372
        public static double CalcTargetDirection(GeoCoordinate current,GeoCoordinate target)
        {
            if (current.IsUnknown == true || target.IsUnknown == true)
            {
                return double.NaN;
            }

            double difLatitude = deg2rad(target.Latitude - current.Latitude);
            double y = Math.Cos(deg2rad(target.Longitude)) * Math.Sin(difLatitude);
            double x = Math.Cos(deg2rad(current.Longitude)) * Math.Sin(deg2rad(target.Longitude)) - Math.Sin(deg2rad(current.Longitude)) * Math.Cos(deg2rad(target.Longitude)) * Math.Cos(difLatitude);

            double dE0 = 180 * Math.Atan2(y, x) / Math.PI;//東を0度とした場合の角度。
            if (dE0 < 0)
            {
                dE0 = dE0 + 360;//値を0から360の範囲にする。
            }

            //this.TargetDirection = (dE0 + 90) % 360;//北を0度とする角度に変換する。

            return (dE0 + 90) % 360;
        }




        //経度、緯度をラジアンに変換する関数。
        private static double deg2rad(double deg)
        {
            return (deg / 180) * Math.PI;
        }


        //緯度、経度から目的地の方角を計算する。
        //参考：http://hamasyou.com/blog/archives/000372





        public static double CalcETA(double distance,double speed)
        {
            if (speed < 0.027)
                return double.NaN;

            var _eta = (distance / speed);//秒
            //m/(m/s)
            if (double.IsNaN(_eta) == true || double.IsInfinity(_eta) == true)
            {
                _eta = double.NaN;
            }
            return _eta;
        }


        #endregion 


        public static int LimitAccuracy(double accuracy)
        {
            string SystemOfUnit = (string)IsolatedStorageSettings.ApplicationSettings["Unit"];

            if (SystemOfUnit == "International")
            {
                if (accuracy > 999)
                    accuracy = 999;

                return ((int)accuracy);
            }
            else
            {
                //単位変換はconvertDistanceWithUnitで行うので、返り値はメートルで返す。
                if (accuracy/0.9144 > 999)
                    accuracy = 999*0.9144;

                return (int)accuracy;
            }
        }

       

    }
}
