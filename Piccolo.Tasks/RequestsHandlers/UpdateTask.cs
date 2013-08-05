using Piccolo.Tasks.Models;
using Piccolo.Tasks.Repositories;

namespace Piccolo.Tasks.RequestsHandlers
{
	[Route("/tasks/{id}")]
	public class UpdateTask : IPut<Task>
	{
		private readonly ITaskRepository _taskRepository;

		public UpdateTask(ITaskRepository taskRepository)
		{
			_taskRepository = taskRepository;
		}

		public int Id { get; set; }

		public HttpResponseMessage<dynamic> Put(Task task)
		{
			_taskRepository.Update(task);

			return Response.Success.NoContent();
		}
	}
}