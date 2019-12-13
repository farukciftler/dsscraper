using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;
using HtmlAgilityPack;
using System.IO;

namespace DSScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Lütfen kullanıcı adınızı giriniz: ");
            string username = Console.ReadLine();
            Console.WriteLine("Lütfen kullanıcı id'nizi giriniz: ");
            string userid = Console.ReadLine();
            Console.WriteLine("Lütfen başlangıç tanım numarasını giriniz: ");
            string baslangictanim = Console.ReadLine();
            
            int baslangic = (System.Convert.ToInt32(baslangictanim) / 20) + 1;
            string filename = username +"_"+baslangictanim+ ".txt";
            for (int i = baslangic; i < 9999; i++)
            {

                GetHtmlAsync(i, filename, userid);
                Thread.Sleep(10000);

            }
            Console.ReadLine();
            
        }
        private static void write(string filepath, string text)
        {

            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(filepath, true))
            {
                file.WriteLine(text);
            }
        }


        private static async void GetHtmlAsync(int page, string filename, string userid)
        {

            string currentdirectory = System.IO.Directory.GetCurrentDirectory();
            string path = currentdirectory + "/"  +filename;
           
            int point = 1;

            Encoding iso = Encoding.GetEncoding("iso-8859-1");
            HtmlWeb web = new HtmlWeb()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = iso,
            };


            string urlpage = $"{page}";
            var url = "https://dunyasozluk.com/author/entries/"+ userid +"/all?page="+urlpage;



            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            HtmlDocument htmlDocument = web.Load(url);

            htmlDocument.LoadHtml(html);

            var Entries = htmlDocument.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("itemprop", "")
                .Equals("comment")).ToList();
            if(Entries.Count == 0)
            {

                System.Environment.Exit(0);
            }


            foreach (var Entry in Entries)
            {
                
                var textRoad = Entry.ChildNodes[5];
                var textRoad_2 = textRoad.ChildNodes[1];
                var text = textRoad_2.ChildNodes[1].InnerText;


                var timeRoad = Entry.ChildNodes[7];
                var timeRoad_2 = timeRoad.ChildNodes[3];
                var time = timeRoad_2.ChildNodes[1].InnerText;

                Console.WriteLine("---------------------------------------------");
                string id = ((page * 20 - 20) + point).ToString();
                string header = Entry.ChildNodes[3].InnerText;
                Console.WriteLine(id);
                Console.WriteLine(header);
                Console.WriteLine(text);
                Console.WriteLine(time);
                if(header == "")
                {
                    System.Environment.Exit(0);
                }
                write(path, "");
                write(path, "Tanım Numarası: "+id);
                write(path, "Başlık: " + header);
                write(path, "");
                write(path, "Tanım: " + text);
                write(path, "");
                write(path, "Zaman: " +time);
                write(path, "");
                write(path, "-----------------------------------------------");
                point++;
            }
            
           
        }




    }
}
