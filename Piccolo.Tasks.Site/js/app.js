'use strict';

angular.module('piccoloClient', []).
  config(['$routeProvider', function ($routeProvider) {
  	$routeProvider.
		when('/', { templateUrl: '/partials/tasks.html', controller: TaskListController }).
		when('/:taskId', { templateUrl: '/partials/task.html', controller: TaskDetailController }).
		otherwise({ redirectTo: '/' });
  }]);

function TaskListController($scope) {
	$scope.refreshTasks = function () {
		$.getJSON("http://piccolo.com/tasks").done(function (data) {
			$scope.$apply(function () {
				$scope.tasks = data;
			});
		});
	};
	
	$scope.getTask = function (id) {
		var task;
		$.ajax({
			type: "GET",
			url: "http://piccolo.com/tasks/" + id,
			async: false
		}).done(function (data) {
			task = JSON.parse(data);
		});
		return task;
	};

	$scope.createTask = function (task) {
		$.ajax({
			type: "POST",
			url: "http://piccolo.com/tasks",
			data: JSON.stringify(task)
		}).done(function () {
			$scope.refreshTasks();
		});
	};

	$scope.updateTask = function (task) {
		$.ajax({
			type: "PUT",
			url: "http://piccolo.com/tasks/" + task.Id,
			data: JSON.stringify(task)
		}).done(function () {
			$scope.refreshTasks();
		});
	};

	$scope.deleteTask = function (id) {
		$.ajax({
			type: "DELETE",
			url: "http://piccolo.com/tasks/" + id
		}).done(function () {
			$scope.refreshTasks();
		});
	};

	$scope.tasks = $scope.refreshTasks();

	$scope.addTask = function () {
		$scope.createTask({ Title: $scope.newTaskTitle, IsCompleted: false });
		$scope.newTaskTitle = "";
	};

	$scope.removeTask = function (id) {
		$scope.deleteTask(id);
	};

	$scope.toggleCompletion = function (id, isCompleted) {
		var task = $scope.getTask(id);
		task.IsCompleted = isCompleted;
		$scope.updateTask(task);
	};
}


function TaskDetailController($scope, Tasks) {
}