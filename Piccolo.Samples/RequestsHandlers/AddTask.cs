using Piccolo.Samples.Models;
using Piccolo.Samples.Repositories;

namespace Piccolo.Samples.RequestsHandlers
{
	[Route("/tasks")]
	public class AddTask : IPost<Task>
	{
		private readonly ITaskRepository _taskRepository;

		public AddTask(ITaskRepository taskRepository)
		{
			_taskRepository = taskRepository;
		}

		public HttpResponseMessage<dynamic> Post(Task task)
		{
			_taskRepository.Add(task);

			return Response.Success.Created();
		}
	}
}