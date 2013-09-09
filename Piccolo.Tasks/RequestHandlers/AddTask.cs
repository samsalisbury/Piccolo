using Piccolo.Tasks.Models;
using Piccolo.Tasks.Repositories;
using Piccolo.Tasks.ViewModels;

namespace Piccolo.Tasks.RequestHandlers
{
	[Route("/tasks")]
	public class AddTask : IPost<AddTaskParameters, Task>
	{
		private readonly ITaskRepository _taskRepository;

		public AddTask(ITaskRepository taskRepository)
		{
			_taskRepository = taskRepository;
		}

		public HttpResponseMessage<Task> Post(AddTaskParameters parameters)
		{
			var task = new Task {Title = parameters.Title};
			_taskRepository.Add(task);

			return Response.Success.Created(task);
		}
	}
}