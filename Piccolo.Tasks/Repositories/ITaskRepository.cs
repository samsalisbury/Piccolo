using System.Collections.Generic;
using Piccolo.Tasks.Models;

namespace Piccolo.Tasks.Repositories
{
	public interface ITaskRepository
	{
		void Add(Task task);
		Task Get(int id);
		IList<Task> Get(int page, int pageSize);
		void Update(Task task);
		void Delete(int id);
	}
}