using Newtonsoft.Json;
using System.Windows.Input;

namespace TD2
{
	public class PlaceItemSummary
	{
		[JsonProperty("id")]
		public int Id { get; set; }
		
		[JsonProperty("title")]
		public string Title { get; set; }
		
		[JsonProperty("description")]
		public string Description { get; set; }
		
		[JsonProperty("image_id")]
		public int ImageId { get; set; }
		
		[JsonProperty("latitude")]
		public double Latitude { get; set; }
		
		[JsonProperty("longitude")]
		public double Longitude { get; set; }

		[JsonIgnore]
		public ICommand SelectPlaceCommand {
			get;
			internal set;
		}
	}
}