using System.Collections.Generic;
using Piccolo.Tasks.Models;

namespace Piccolo.Tasks.ViewModels
{
	public class TaskCollection
	{
		public TaskCollection(IList<Task> tasks, int totalCount)
		{
			Tasks = tasks;
			TotalCount = totalCount;
		}

		public IList<Task> Tasks { get; set; }
		public int TotalCount { get; set; }
	}
}