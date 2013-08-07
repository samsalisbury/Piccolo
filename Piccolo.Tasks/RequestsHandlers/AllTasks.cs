using Piccolo.Tasks.Repositories;
using Piccolo.Tasks.ViewModels;

namespace Piccolo.Tasks.RequestsHandlers
{
	[Route("/tasks")]
	public class AllTasks : IGet<TaskCollection>
	{
		private readonly ITaskRepository _taskRepository;

		public AllTasks(ITaskRepository taskRepository)
		{
			_taskRepository = taskRepository;
		}

		[Optional]
		public int? Page { get; set; }

		[Optional]
		public int? PageSize { get; set; }

		public HttpResponseMessage<TaskCollection> Get()
		{
			var tasks = _taskRepository.Get(Page ?? 1, PageSize ?? 10);
			var totalCount = _taskRepository.Count();

			return Response.Success.Ok(new TaskCollection(tasks, totalCount));
		}
	}
}