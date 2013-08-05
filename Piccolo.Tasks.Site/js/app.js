'use strict';

angular.module('piccoloClient', ['piccoloServices']).
  config(['$routeProvider', function ($routeProvider) {
  	$routeProvider.
		when('/', { templateUrl: '/partials/tasks.html', controller: TaskListController }).
		when('/:taskId', { templateUrl: '/partials/task.html', controller: TaskDetailController }).
		otherwise({ redirectTo: '/' });
  }]);

function TaskListController($scope, Tasks) {
	$scope.tasks = Tasks.query();

	$scope.addTask = function () {
		var newId = $scope.tasks[$scope.tasks.length - 1].id + 1;
		$scope.tasks.push({ id: newId, title: $scope.newTaskTitle, isCompleted: false });
		$scope.newTaskTitle = "";
	};

	$scope.toggleCompletion = function (taskId) {
		console.log("Task " + taskId + " is completed: " + $scope.tasks[taskId - 1].isCompleted);
	};
}


function TaskDetailController($scope, Tasks) {
}