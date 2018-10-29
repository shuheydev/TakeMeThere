using System;
//using System.Net;
//using System.Windows.Controls;
//using System.Windows.Documents;
//using System.Windows.Ink;
//using System.Windows.Input;
using System.Windows.Media;
//using System.Windows.Media.Animation;
//using System.Windows.Shapes;

//using System.Diagnostics;
//using Microsoft.Phone.Reactive;
using System.IO.IsolatedStorage;
//using System.Xml;
//using System.Xml.Serialization;
using System.Xml.Linq;
using System.IO;
using System.Linq;
//using System.Device.Location;
using System.Device.Location;
using System.Windows;
//using System.Windows.Application;



namespace TakeMeThere
{
    public class AppDB
    {
        private const string DefaultColor_Hex = "99A4C400";
        private const string SelectedColor_Hex = "99FA6800";
        private const string TargetColor_Hex = "99D80073";
        private const string CandidateColor_Hex = "99825A2C";

        //MySensors Sensors = new MySensors();

        private const string DBFileName = "DestinationInfos.xml";


        public AppDB()
        {
            InitDB();
        }

        //分離ストレージのDBファイルの初期化
        public void InitDB()
        {
            //分離ストレージの初期化。ファイルが存在しなければ、新しく作る。
            IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication();
            if (!isoFile.FileExists(DBFileName))
            {
                //xmlドキュメントの初期化
                XDocument XmlDoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("Destinations"));
                

                //xmlドキュメントをファイルへ書き込む
                IsolatedStorageFileStream strm = new IsolatedStorageFileStream(DBFileName, FileMode.Create, FileAccess.ReadWrite, isoFile);
                XmlDoc.Save(strm);
                strm.Dispose();//ストリームを閉じる

            }
            isoFile.Dispose();




        }


       //
        public ViewModel LoadInfoFromXML()
        {
            //目的地データの読み込み。
            IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream strm = new IsolatedStorageFileStream(DBFileName, FileMode.Open, FileAccess.Read, isoFile);
            XDocument xmlDoc = XDocument.Load(strm);
            strm.Dispose();
            isoFile.Dispose();

            var destinations = from destination in xmlDoc.Descendants("Destination")
                               orderby destination.Element("Name").Value
                                select destination;


            ViewModel PushPinView = new ViewModel();
            PushPinModel pin;//=new PushPinModel();

            bool darkTheme = ((Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible);

            foreach (var dest in destinations)
            {
                pin = new PushPinModel();
                // if (dest.Element("Latitude").Value == "NaN")
                //   pin.Location = new GeoCoordinate(34,140);
                //else 
                if (dest.Element("Latitude").Value != "NaN")
                {
                    pin.Location = new GeoCoordinate(Convert.ToDouble(dest.Element("Latitude").Value), Convert.ToDouble(dest.Element("Longitude").Value));
                    pin.Name = dest.Element("Name").Value;
                    pin.IsEnabled = Convert.ToBoolean(dest.Element("IsEnable").Value);
                    pin.Target = Convert.ToBoolean(dest.Element("Target").Value);
                    pin.Selected = Convert.ToBoolean(dest.Element("Selected").Value);
                    pin.Note = dest.Element("Note").Value;
                    pin.Color = new SolidColorBrush(Utility.GetColorFromHexString(dest.Element("Color").Value));
                    pin.Visibility = Utility.GetVisibilityFromString(dest.Element("Visibility").Value);
                    try
                    {
                        pin.CreateDate = DateTime.FromFileTime((long)Convert.ToDouble(dest.Element("CreateDate").Value));
                    }
                    catch
                    {
                        pin.CreateDate = DateTime.Parse(dest.Element("CreateDate").Value);
                    }
                    pin.Address = (dest.Descendants("Address").Count() == 0) ? "" : dest.Element("Address").Value;
                    pin.PhoneNum = (dest.Descendants("PhoneNum").Count() == 0) ? "" : dest.Element("PhoneNum").Value;

                    if (darkTheme == true)//darkの時は白抜きの画像
                    {
                        pin.ArrowURI = "/Icons/appbar.next.rest.png";
                        pin.CircleURI = "/Icons/appbar.basecircle.rest.png";
                    }
                    else
                    {
                        pin.ArrowURI = "/Icons/appbar.next.rest.light.png";
                        pin.CircleURI = "/Icons/appbar.base.png";
                    }

                    //MessageBox.Show("l "+pin.Visibility.ToString());
                    PushPinView.PushPins.Add(pin);
                }
            }

            return PushPinView;
        }




        public void SaveInfoToIsoStrage(ViewModel PushPinViewModel)
        {
            //xmlドキュメントの初期化
           XDocument xmlDoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("Destinations"));

            //コレクションのアイテムをひとつづつXMLに入れていく
            foreach (var dest in PushPinViewModel.PushPins)
            {
                if (dest.Location.IsUnknown == false)
                {
                    XElement destination = new XElement("Destination");

                    destination.Add(new XElement("Latitude", dest.Location.Latitude));
                    destination.Add(new XElement("Longitude", dest.Location.Longitude));
                    destination.Add(new XElement("Name", dest.Name));
                    destination.Add(new XElement("IsEnable", dest.IsEnabled));
                    destination.Add(new XElement("Target", dest.Target));
                    destination.Add(new XElement("Selected", dest.Selected));
                    destination.Add(new XElement("Note", dest.Note));
                    destination.Add(new XElement("Color", Utility.GetHexStringFromColor(dest.Color.Color)));
                    destination.Add(new XElement("Visibility", dest.Visibility.ToString()));
                    destination.Add(new XElement("CreateDate", dest.CreateDate.ToUniversalTime()));
                    destination.Add(new XElement("Address", dest.Address));
                    destination.Add(new XElement("PhoneNum", dest.PhoneNum));
                    //MessageBox.Show("s "+destination.Element("Visibility").Value);
                    xmlDoc.Root.Add(destination);
                }
            }


            //XMLファイルを分離ストレージに保存。
  
            //ロードとセーブでそれぞれストリームを開いて閉じること。でなければ正しく保存できない。
            IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication();

            //xmlファイルの更新。FileMode.Createとすると、ファイルが存在する場合、同名の（空の）ファイルを新規に作り直してくれる。
            //ここでもしFileMode.Openにすると、下の書き込みのときに、
            //元の内容を一文字ずつ上書きしていく形でファイルの先頭から文字列（XML）
            //が書き込まれていくようで、
            //要素が削除されて短くなっているので、
            //その後ろに以前の文がはみだしてくっついてしまう。
            //そのため、次に読み込むときにエラーが出てしまう。
            //ほんと、要注意。
            //なんでこんな仕様なの？XMLのときだけ？
            //やってることはファイルを消してまた書き込んでいるだけなので、
            //ファイルが大きくなった場合が心配。
            //データ操作のオーバーヘッドはどうなるんだろう？
            //
            IsolatedStorageFileStream strm = new IsolatedStorageFileStream(DBFileName, FileMode.Create, FileAccess.Write, isoFile);
            //xmlDoc.Save(strm);
            xmlDoc.Save(strm);
            strm.Dispose();//セーブしたら閉じる
            isoFile.Dispose();
        }



        /*
        //XMLデータの読み込み
        public XDocument LoadInfos()
        {
            //目的地データの読み込み。
            IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream strm = new IsolatedStorageFileStream(DBFileName, FileMode.Open, FileAccess.Read, isoFile);
            XDocument xmlDoc = XDocument.Load(strm);
            strm.Dispose();
            isoFile.Dispose();

            this.XmlDoc = xmlDoc;
            return xmlDoc;
        }

        //XMLデータの書き込み
        public void SaveInfos(XDocument xmlDoc)
        {
            //取得した商品情報をitems.xmlファイルのItems要素の下に追加
            //ロードとセーブでそれぞれストリームを開いて閉じること。でなければ正しく保存できない。
            IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication();

            //xmlファイルの更新。FileMode.Createとすると、ファイルが存在する場合、同名の（空の）ファイルを新規に作り直してくれる。
            //ここでもしFileMode.Openにすると、下の書き込みのときに、
            //元の内容を一文字ずつ上書きしていく形でファイルの先頭から文字列（XML）
            //が書き込まれていくようで、
            //要素が削除されて短くなっているので、
            //その後ろに以前の文がはみだしてくっついてしまう。
            //そのため、次に読み込むときにエラーが出てしまう。
            //ほんと、要注意。
            //なんでこんな仕様なの？XMLのときだけ？
            //やってることはファイルを消してまた書き込んでいるだけなので、
            //ファイルが大きくなった場合が心配。
            //データ操作のオーバーヘッドはどうなるんだろう？
            //
            IsolatedStorageFileStream strm = new IsolatedStorageFileStream(DBFileName, FileMode.Create, FileAccess.Write, isoFile);
            xmlDoc.Save(strm);

            strm.Dispose();//セーブしたら閉じる
            isoFile.Dispose();
        }


        public void SaveInfos()
        {
            //取得した商品情報をitems.xmlファイルのItems要素の下に追加
            //ロードとセーブでそれぞれストリームを開いて閉じること。でなければ正しく保存できない。
            IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication();

            //xmlファイルの更新。FileMode.Createとすると、ファイルが存在する場合、同名の（空の）ファイルを新規に作り直してくれる。
            //ここでもしFileMode.Openにすると、下の書き込みのときに、
            //元の内容を一文字ずつ上書きしていく形でファイルの先頭から文字列（XML）
            //が書き込まれていくようで、
            //要素が削除されて短くなっているので、
            //その後ろに以前の文がはみだしてくっついてしまう。
            //そのため、次に読み込むときにエラーが出てしまう。
            //ほんと、要注意。
            //なんでこんな仕様なの？XMLのときだけ？
            //やってることはファイルを消してまた書き込んでいるだけなので、
            //ファイルが大きくなった場合が心配。
            //データ操作のオーバーヘッドはどうなるんだろう？
            //
            IsolatedStorageFileStream strm = new IsolatedStorageFileStream(DBFileName, FileMode.Create, FileAccess.Write, isoFile);
            //xmlDoc.Save(strm);
            this.XmlDoc.Save(strm);
            strm.Dispose();//セーブしたら閉じる
            isoFile.Dispose();
        }

        //ターゲットに指定されている目的地がXMLの中に存在するかチェックする
        public XElement getTargetDestination()
        {
            //XDocument xmlDoc = this.LoadInfos();
            IEnumerable<XElement> destinations = from dest in this.XmlDoc.Descendants("Destination")
                                                 where dest.Element("Target").Value == "true"
                                                 select dest;

            if (destinations.Count() > 0)
            {
                return destinations.First();
            }
            else
            {
                return null;
            }

        }

        //ターゲットに指定されている目的地がXMLの中に存在するかチェックする
        public XElement getSelectedDestination()
        {
            //XDocument xmlDoc = this.LoadInfos();
            IEnumerable<XElement> destinations = from dest in this.XmlDoc.Descendants("Destination")
                                                 where dest.Element("Selected").Value == "true"
                                                 select dest;

            if (destinations.Count() > 0)
            {
                return destinations.First();
            }
            else
            {
                return null;
            }

        }

        public XElement SearchDestinationByLocation(string lat, string lon)
        {
            IEnumerable<XElement> destinations = from dest in this.XmlDoc.Descendants("Destination")
                                                 where dest.Element("Latitude").Value == lat
                                                 && dest.Element("Longitude").Value == lon
                                                 select dest;
            if (destinations.Count() > 0)
            {
                return destinations.First();
            }
            else
            {
                return null;
            }
        }


        public void resetSelected()
        {
            this.XmlDoc = this.LoadInfos();

            var selectedElements = from element in XmlDoc.Descendants("Destination")
                                   where element.Element("Selected").Value == "true"
                                   select element;

            foreach (var ele in selectedElements)
            {
                ele.Element("Selected").Value = "false";
            }

            this.SaveInfos();

        }
        */
    }



}
