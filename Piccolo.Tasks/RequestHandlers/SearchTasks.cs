using Piccolo.Tasks.Repositories;
using Piccolo.Tasks.Validators;
using Piccolo.Tasks.ViewModels;
using Piccolo.Validation;

namespace Piccolo.Tasks.RequestHandlers
{
	[Route("/tasks/search")]
	public class SearchTasks : IGet<TaskCollection>
	{
		private readonly ITaskRepository _taskRepository;

		public SearchTasks(ITaskRepository taskRepository)
		{
			_taskRepository = taskRepository;
		}

		[Optional]
		public int? PageNumber { get; set; }

		[Optional]
		[ValidateWith(typeof(PageSizeValidator))]
		public int? PageSize { get; set; }

		[Optional]
		public string Term { get; set; }

		public HttpResponseMessage<TaskCollection> Get()
		{
			var searchResults = _taskRepository.Search(Term ?? "", PageNumber ?? 1, PageSize ?? 10);

			return Response.Success.Ok(searchResults);
		}
	}
}