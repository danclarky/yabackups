using System;
using YandexDisk.Client;
using YandexDisk.Client.Http;
using YandexDisk.Client.Protocol;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using Npgsql;


namespace cloud
{
    class Program
    {
        public string Login = "";
        public string Password = "";

        static void Main(string[] args)
        {
            UploadSample().GetAwaiter().GetResult();
            //Console.ReadLine();
        }
        async static Task UploadSample()
        {
            string oauthToken = "***";
            IDiskApi diskApi = new DiskHttpApi(oauthToken);

            DirectoryInfo di = new DirectoryInfo(@"path");
            FileInfo[] FileArray;
            FileArray = di.GetFiles();
            List<string> files = new List<string>();
            foreach (FileInfo FileInfo in FileArray)
            {
                if (FileInfo.CreationTime.Date == DateTime.Now.Date)
                {
                    files.Add(FileInfo.Name);
                    Console.WriteLine(FileInfo.FullName);
                    Stream d = File.Open(FileInfo.FullName, FileMode.Open);
                    var link = await diskApi.Files.GetUploadLinkAsync("/backups/" + FileInfo.Name, false);
                    await diskApi.Files.UploadAsync(link, d);
                }
            }

            DirectoryInfo di1 = new DirectoryInfo(@"path");
            FileArray = di1.GetFiles();
            foreach (FileInfo FileInfo in FileArray)
            {
                if (FileInfo.CreationTime.Date == DateTime.Now.Date)
                {
                    files.Add(FileInfo.Name);
                    Console.WriteLine(FileInfo.FullName);
                    Stream d = File.Open(FileInfo.FullName, FileMode.Open);
                    var link = await diskApi.Files.GetUploadLinkAsync("/backups/" + FileInfo.Name, false);
                    await diskApi.Files.UploadAsync(link, d);
                }
            }

            LastUploadedResourceRequest a = new LastUploadedResourceRequest();
            string find = "найдено файлов на ЯД: ";
            LastUploadedResourceList x = await diskApi.MetaInfo.GetLastUploadedInfoAsync(a);
            foreach (Resource ab in x.Items)
            {
                foreach (string filename in files)
                {
                    if (ab.Name == filename.ToString())
                    {
                        find += ab.Name + "\n";
                    }
                }
            }
            using (var conn = new NpgsqlConnection("Host=ip;Port=port;Username=admin;Password=pass;Database=Base;Command Timeout=0;"))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "insert into ПришедшиеЗаявкиДляЗапроса (ФИО) Values( '" + find + "')";
                    cmd.ExecuteNonQuery();

                }
                conn.Close();
            }

        }
        

    }
}
