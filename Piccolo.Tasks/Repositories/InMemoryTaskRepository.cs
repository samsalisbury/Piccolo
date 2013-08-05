using System.Collections.Generic;
using System.Linq;
using Piccolo.Tasks.Models;

namespace Piccolo.Tasks.Repositories
{
	// Created for demo purposes. Ignore obvious concurrency issues, etc.
	public class InMemoryTaskRepository : ITaskRepository
	{
		private readonly List<Task> _tasks;

		public InMemoryTaskRepository()
		{
			_tasks = new List<Task>();
			_tasks.Add(new Task {Id = 1, Title = "Create project", IsCompleted = true});
			_tasks.Add(new Task {Id = 2, Title = "Commit", IsCompleted = true});
			_tasks.Add(new Task {Id = 3, Title = "Push to GitHug", IsCompleted = true});
			_tasks.Add(new Task {Id = 4, Title = "Write awesome docs!", IsCompleted = false});
			_tasks.Add(new Task {Id = 5, Title = "Push NuGet package", IsCompleted = false});
		}

		public void Add(Task task)
		{
			task.Id = _tasks.Max(x => x.Id) + 1;
			_tasks.Add(task);
		}

		public Task Get(int id)
		{
			return _tasks.SingleOrDefault(x => x.Id == id);
		}

		public IList<Task> Get(int page, int pageSize)
		{
			return _tasks.Skip((page - 1)*pageSize).Take(pageSize).ToList();
		}

		public void Update(Task task)
		{
			var _ = Get(task.Id);
			_.Title = task.Title;
			_.IsCompleted = task.IsCompleted;
		}

		public void Delete(int id)
		{
			_tasks.RemoveAll(x => x.Id == id);
		}
	}
}