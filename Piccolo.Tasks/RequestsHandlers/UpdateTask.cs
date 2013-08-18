using Piccolo.Tasks.Models;
using Piccolo.Tasks.Repositories;
using Piccolo.Tasks.ViewModels;

namespace Piccolo.Tasks.RequestsHandlers
{
	[Route("/tasks/{id}")]
	public class UpdateTask : IPut<UpdateTaskParameters, Task>
	{
		private readonly ITaskRepository _taskRepository;

		public UpdateTask(ITaskRepository taskRepository)
		{
			_taskRepository = taskRepository;
		}

		public int Id { get; set; }

		public HttpResponseMessage<Task> Put(UpdateTaskParameters parameters)
		{
			var task = new Task {Title = parameters.Title, IsCompleted = parameters.IsCompleted};
			_taskRepository.Update(task);

			return Response.Success.Ok(task);
		}
	}
}