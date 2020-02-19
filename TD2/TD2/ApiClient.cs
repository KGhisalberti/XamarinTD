using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TD2 {
	public class ApiClient {
		private readonly HttpClient _client = new HttpClient();

		public async Task<HttpResponseMessage> Execute(HttpMethod method, string url, object data = null, string token = null) {
			HttpRequestMessage request = new HttpRequestMessage(method, url);

			if (data != null) {
				request.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
			}

			if (token != null) {
				request.Headers.Add("Authorization", $"Bearer {token}");
			}

			return await _client.SendAsync(request);
		}

		public async Task<T> ReadFromResponse<T>(HttpResponseMessage response) {
			string result = await response.Content.ReadAsStringAsync();

			return JsonConvert.DeserializeObject<T>(result);
		}

		public async Task<Response<ImageItem>> PostImage(byte[] bytes, string token) {
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, Urls.URI + Urls.IMAGE);
			request.Headers.Add("Authorization", $"Bearer {token}");

			MultipartFormDataContent requestContent = new MultipartFormDataContent();
			ByteArrayContent imageContent = new ByteArrayContent(bytes);
			imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

			// Le deuxième paramètre doit absolument être "file" ici sinon ça ne fonctionnera pas
			requestContent.Add(imageContent, "file", "file.jpg");

			request.Content = requestContent;

			HttpResponseMessage response = await _client.SendAsync(request);

			return await ReadFromResponse<Response<ImageItem>>(response);
		}


	}
}