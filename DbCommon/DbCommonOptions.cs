namespace DbCommon
{
	public static class DbStackNames
	{
		public const string Postgres = "postgres";
	}

	public class DbCommonOptions
	{
		public string DbStack { get; set; }
		public string ConnectionString { get; set; }
	}
}
