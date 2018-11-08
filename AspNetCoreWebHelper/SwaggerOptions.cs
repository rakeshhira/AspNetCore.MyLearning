namespace AspNetCoreWebCommon
{
	public class SwaggerOptions
	{
		public bool Enabled { get; set; }
		public bool IncludeXmlComments { get; set; }
		public string DocName { get; set; }
		public string DocVersion { get; set; }
		public string EndPoint { get; set; }
		public string EndPointName { get; set; }
		public string RoutePrefix { get; set; }
	}
}
