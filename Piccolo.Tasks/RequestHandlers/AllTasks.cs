using Piccolo.Tasks.Repositories;
using Piccolo.Tasks.ViewModels;

namespace Piccolo.Tasks.RequestHandlers
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
		public int? PageNumber { get; set; }

		[Optional]
		public int? PageSize { get; set; }

		public HttpResponseMessage<TaskCollection> Get()
		{
			var tasks = _taskRepository.GetAll(PageNumber ?? 1, PageSize ?? 10);
			var totalCount = _taskRepository.Count();

			return Response.Success.Ok(new TaskCollection(tasks, totalCount));
		}
	}
}