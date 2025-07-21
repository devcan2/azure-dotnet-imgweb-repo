using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Web.Pages
{
    public class IndexModel : PageModel
    {
        private HttpClient _httpClient;
        private Options _options;

        public IndexModel(HttpClient httpClient, Options options)
        {
            _httpClient = httpClient;
            _options = options;
        }

        [BindProperty]
        public List<string> ImageList { get; private set; }

        [BindProperty]
        public IFormFile Upload { get; set; }

        public async Task OnGetAsync()
        {
            var imagesUrl = _options.ApiUrl;

            string imagesJson = await _httpClient.GetStringAsync(imagesUrl);

            IEnumerable<string> imagesList = JsonConvert.DeserializeObject<IEnumerable<string>>(imagesJson);

            this.ImageList = imagesList.ToList<string>();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var imageName = Request.Form["ImageName"];
            if (Upload != null && Upload.Length > 0 && !string.IsNullOrWhiteSpace(imageName))
            {
                var imagesUrl = _options.ApiUrl;
                using (var content = new MultipartFormDataContent())
                {
                    var imageContent = new StreamContent(Upload.OpenReadStream());
                    imageContent.Headers.ContentType = new MediaTypeHeaderValue(Upload.ContentType);
                    content.Add(imageContent, "file", Upload.FileName);
                    content.Add(new StringContent(imageName), "ImageName");
                    var response = await _httpClient.PostAsync(imagesUrl, content);
                }
            }
            return RedirectToPage("/Index");
        }
    }
}