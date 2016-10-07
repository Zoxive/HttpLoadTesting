using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Zoxive.HttpLoadTesting.Client.Web
{
    public static class ResourcesOrRealThing
    {
        private static readonly string CurrentDirectory = Directory.GetCurrentDirectory();
        private static readonly Assembly CurrentAssembly = typeof(ResourcesOrRealThing).GetTypeInfo().Assembly;

        private static Stream Stream(string resourceName)
        {

#if DEBUG
            var fullPath = CurrentDirectory + "/" + resourceName;

            if (File.Exists(fullPath))
            {
                return new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            }
#endif

            var embededResourceName = "Zoxive.HttpLoadTesting.Client." + resourceName.Replace('/', '.');

            return CurrentAssembly.GetManifestResourceStream(embededResourceName);
        }

        public static async Task Stream(string resourceName, HttpResponse response, string contentType)
        {
            var stream = Stream(resourceName);
            if (stream == null)
            {
                response.StatusCode = 404;
                await response.WriteAsync("Not Found");
                return;
            }

            response.ContentLength = stream.Length;
            response.ContentType = contentType;

            await stream.CopyToAsync(response.Body);

            stream.Dispose();
        }
    }
}