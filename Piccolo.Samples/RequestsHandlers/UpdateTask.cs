using Piccolo.Samples.Models;
using Piccolo.Samples.Repositories;

namespace Piccolo.Samples.RequestsHandlers
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