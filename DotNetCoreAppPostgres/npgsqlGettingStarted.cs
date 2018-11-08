using System.Data;
using Npgsql;

namespace DotNetCoreAppPostgres
{
	public static class npgsqlGettingStarted
	{
		public static IDbConnection Connect(string connString)
		{
			//var connString = "Host=myserver;Username=mylogin;Password=mypass;Database=mydatabase";
			var conn = new NpgsqlConnection(connString);
			conn.Open();
			return conn;
		}
	}
}
