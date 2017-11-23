using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace CORESubscriber
{
    internal class Changelog
    {
        internal static void GetZipFile()
        {
            using (var client = new HttpClient())
            {
                var byteArray = Encoding.ASCII.GetBytes(Provider.User + ":" + Provider.Password);

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var result = client.GetAsync(Provider.OrderedChangelogDownloadUrl).Result;

                if (!result.IsSuccessStatusCode)
                    throw new FileNotFoundException("Statuscode when trying to download from " +
                                                    Provider.OrderedChangelogDownloadUrl + " was " + result.StatusCode);

                var fileName = Config.DownloadFolder + "/" +
                               Provider.OrderedChangelogDownloadUrl.Split('/')[
                                   Provider.OrderedChangelogDownloadUrl.Split('/').Length - 1];

                using (var fs = new FileStream(fileName, FileMode.Create))
                {
                    result.Content.CopyToAsync(fs);
                }

            }
        }
    }
}