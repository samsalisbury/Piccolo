using Piccolo.Tasks.Repositories;

namespace Piccolo.Tasks.RequestsHandlers
{
	[Route("/tasks/{id}")]
	public class DeleteTaskById : IDelete<object, object>
	{
		private readonly ITaskRepository _taskRepository;

		public DeleteTaskById(ITaskRepository taskRepository)
		{
			_taskRepository = taskRepository;
		}

		public int Id { get; set; }

		public HttpResponseMessage<object> Delete(object parameters)
		{
			_taskRepository.Delete(Id);

			return Response.Success.NoContent<dynamic>();
		}
	}
}