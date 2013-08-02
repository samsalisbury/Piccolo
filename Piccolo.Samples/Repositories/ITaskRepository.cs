using System.Collections.Generic;
using Piccolo.Samples.Models;

namespace Piccolo.Samples.Repositories
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