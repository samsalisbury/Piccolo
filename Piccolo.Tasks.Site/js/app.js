'use strict';

angular.module('piccoloClient', ['piccoloServices']).
  config(['$routeProvider', function ($routeProvider) {
  	$routeProvider.
		when('/', { templateUrl: '/partials/tasks.html', controller: TaskListController }).
		when('/:taskId', { templateUrl: '/partials/task.html', controller: TaskDetailController }).
		otherwise({ redirectTo: '/' });
  }]);

function TaskListController($scope, Tasks) {
	$scope.tasks = Tasks.list();

	$scope.addTask = function () {
		Tasks.add({ Title: $scope.newTaskTitle, IsCompleted: false });
	};

	$scope.toggleCompletion = function (taskId) {
		console.log("Task " + taskId + " is completed: " + $scope.tasks[taskId - 1].isCompleted);
	};
}


function TaskDetailController($scope, Tasks) {
}