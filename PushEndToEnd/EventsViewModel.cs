using PushTesting.Services;

namespace PushTesting;


public class EventsViewModel : FuncViewModel
{
	public EventsViewModel(
		BaseServices services,
		AppSqliteConnection conn
	) : base(services)
	{
		this.Load = ReactiveCommand.CreateFromTask(async () =>
		{
			var data = await conn
				.AppEvents
				.OrderByDescending(x => x.DateCreated)
				.ToListAsync();

			this.Events = data
				.Select(x => new ItemViewModel(x))
				.ToList();
		});
        this.BindBusyCommand(this.Load);

        this.Clear = ReactiveCommand.CreateFromTask(async () =>
		{
			var confirm = await this.Dialogs.Confirm("Clear All Events?");
			if (confirm)
			{
				await conn.DeleteAllAsync<AppEvent>();
				this.Load.Execute(null);
			}
		});
		this.Appearing = () => this.Load.Execute(null);
	}

	public ICommand Load { get; }
	public ICommand Clear { get; }
	[Reactive] public List<ItemViewModel> Events { get; private set; }
}

public record ItemViewModel(AppEvent e)
{ 
	public string EventName => e.EventName;
	public string Description => e.Description;
	public string DateCreated => e
		.DateCreated
		.ToLocalTime()
		.ToString("MMMM dd - hh:mm:ss tt");
}