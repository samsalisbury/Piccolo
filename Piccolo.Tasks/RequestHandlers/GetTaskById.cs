using Piccolo.Tasks.Models;
using Piccolo.Tasks.Repositories;

namespace Piccolo.Tasks.RequestHandlers
{
	[Route("/tasks/{id}")]
	public class GetTaskById : IGet<Task>
	{
		private readonly ITaskRepository _taskRepository;

		public GetTaskById(ITaskRepository taskRepository)
		{
			_taskRepository = taskRepository;
		}

		public int Id { get; set; }

		public HttpResponseMessage<Task> Get()
		{
			var task = _taskRepository.Get(Id);
			if (task == null)
				return Response.Error.NotFound<Task>();

			return Response.Success.Ok(task);
		}
	}
}