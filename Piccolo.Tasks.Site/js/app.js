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

	$scope.createTask = function (task) {
		$.ajax({
			type: "POST",
			url: "http://piccolo.com/tasks",
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

	$scope.toggleCompletion = function (taskId) {
		console.log("Task " + taskId + " is completed: " + $scope.tasks[taskId - 1].isCompleted);
	};
}


function TaskDetailController($scope, Tasks) {
}