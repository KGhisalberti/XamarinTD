using Newtonsoft.Json;

namespace TD2
{
	public class CreateCommentRequest
	{
		[JsonProperty("text")]
		public string Text { get; set; }
	}
}