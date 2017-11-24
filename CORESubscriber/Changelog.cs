using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace CORESubscriber
{
    internal class Changelog
    {
        internal static string ZipFile { get; set; }

        internal static string Uuid { get; set; }

        internal static string DownloadUrl { get; set; }

        internal static string DataFolder { get; set; }

        internal static void Get()
        {
            using (var client = new HttpClient())
            {
                var byteArray = Encoding.ASCII.GetBytes(Provider.User + ":" + Provider.Password);

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var result = client.GetAsync(DownloadUrl).Result;

                if (!result.IsSuccessStatusCode)
                    throw new FileNotFoundException("Statuscode when trying to download from " +
                                                    DownloadUrl + " was " + result.StatusCode);

                Uuid = DownloadUrl.Split('/')[DownloadUrl.Split('/').Length - 1];

                ZipFile = Config.DownloadFolder + Uuid;

                Uuid = Uuid.Replace(".zip", "");

                using (var fs = new FileStream(ZipFile, FileMode.Create))
                {
                    result.Content.CopyToAsync(fs);
                }
            }

            Unzip();
        }

        internal static void Unzip()
        {
            System.IO.Compression.ZipFile.ExtractToDirectory(ZipFile, Config.DownloadFolder);

            DataFolder = Config.DownloadFolder + Uuid;
        }
    }
}