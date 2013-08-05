using Piccolo.Tasks.Repositories;

namespace Piccolo.Tasks.RequestsHandlers
{
	[Route("/tasks/{id}")]
	public class DeleteTaskById : IDelete
	{
		private readonly ITaskRepository _taskRepository;

		public DeleteTaskById(ITaskRepository taskRepository)
		{
			_taskRepository = taskRepository;
		}

		public int Id { get; set; }

		public HttpResponseMessage<dynamic> Delete()
		{
			_taskRepository.Delete(Id);

			return Response.Success.NoContent();
		}
	}
}