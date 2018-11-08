using System.Data;

namespace DbCommon
{
	public static class ConnectionFactory
	{
		public static IDbConnection GetConnection(DbCommonOptions dbCommonOptions)
		{
			switch (dbCommonOptions.DbStack)
			{
				case DbStackNames.Postgres:
					return NpgsqlWrapper.Connect(dbCommonOptions.ConnectionString);
			}
			return null;
		}
	}
}
