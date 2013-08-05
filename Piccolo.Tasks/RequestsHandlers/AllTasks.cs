using System.Collections.Generic;
using System.Web;
using Piccolo.Tasks.Models;
using Piccolo.Tasks.Repositories;

namespace Piccolo.Tasks.RequestsHandlers
{
	[Route("/tasks")]
	public class AllTasks : IGet<IList<Task>>
	{
		private readonly ITaskRepository _taskRepository;

		public AllTasks(ITaskRepository taskRepository)
		{
			_taskRepository = taskRepository;
		}

		public HttpResponseMessage<IList<Task>> Get()
		{
			var tasks = _taskRepository.Get(1, 10);

			// TODO: Implemented CORS support
			HttpContext.Current.Response.Headers["Access-Control-Allow-Origin"] = "*";

			return Response.Success.Ok(tasks);
		}
	}
}