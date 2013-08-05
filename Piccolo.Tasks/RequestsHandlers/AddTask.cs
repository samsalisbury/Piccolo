using Piccolo.Tasks.Models;
using Piccolo.Tasks.Repositories;

namespace Piccolo.Tasks.RequestsHandlers
{
	[Route("/tasks")]
	public class AddTask : IPost<Task>
	{
		private readonly ITaskRepository _taskRepository;

		public AddTask(ITaskRepository taskRepository)
		{
			_taskRepository = taskRepository;
		}

		public HttpResponseMessage<Task> Post(Task task)
		{
			_taskRepository.Add(task);

			return Response.Success.Created(task);
		}
	}
}