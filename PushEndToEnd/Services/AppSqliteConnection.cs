using SQLite;

namespace PushTesting.Services;


public class AppSqliteConnection : SQLiteAsyncConnection
{
	public AppSqliteConnection(IPlatform platform) : base(Path.Combine(platform.AppData.FullName, "app.db"))
	{
		var c = this.GetConnection();
		c.CreateTable<AppEvent>();
	}

	public AsyncTableQuery<AppEvent> AppEvents => this.Table<AppEvent>();
}


public class AppEvent
{
	[PrimaryKey]
	[AutoIncrement]
	public int Id { get; set; }

	public string EventName { get; set; }
	public string Description { get; set; }
	public DateTime DateCreated { get; set; }
}