using System.Collections.Generic;
using Piccolo.Tasks.Models;
using Piccolo.Tasks.Repositories;

namespace Piccolo.Tasks.RequestsHandlers
{
	[Route("/tasks/search")]
	public class SearchTasks : IGet<IList<Task>>
	{
		private readonly ITaskRepository _taskRepository;

		public SearchTasks(ITaskRepository taskRepository)
		{
			_taskRepository = taskRepository;
		}

		public HttpResponseMessage<IList<Task>> Get()
		{
			var tasks = _taskRepository.Get(1, 10);

			return Response.Success.Ok(tasks);
		}
	}
}