using System.Collections.Generic;
using Piccolo.Tasks.Models;
using Piccolo.Tasks.ViewModels;

namespace Piccolo.Tasks.Repositories
{
	public interface ITaskRepository
	{
		IList<Task> GetAll(int pageNumber, int pageSize);
		TaskCollection Search(string term, int pageNumber, int pageSize);
		int Count();
		Task Get(int id);
		void Add(Task task);
		void Update(Task task);
		void Delete(int id);
	}
}