using Microsoft.AspNetCore.Mvc;

namespace Zune.Net.MetaServices.Controllers
{
    [Route("/Cover/")]
    public class Cover : Controller
    {
        private readonly ILogger _logger;
        public Cover(ILogger<Cover> logger)
        {
            _logger = logger;
        }
        private static readonly Uri defaultImage = new Uri("https://hellofromseattle.com/wp-content/uploads/sites/6/2020/03/Zune-Basic.png");

        [Route("{coverId}")]
        public async Task<IActionResult> GetCover(string coverId)
        {
            var catalogImageUri = new Uri($"https://coverartarchive.org/release/{coverId}/front");
            if(coverId.Equals("default"))
            {
                _logger.LogInformation($"default albumart requested");
                catalogImageUri = defaultImage;
            }

            using var client = new HttpClient();
            var result = await client.GetAsync(catalogImageUri);
            if(!result.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Falling back to the default image with status {result.StatusCode}, failed to resolve actual artwork");
                result = await client.GetAsync(defaultImage);
            }
            return new HttpResponseMessageResult(result);
            //HttpResponseMessageResult
        }
    }
}