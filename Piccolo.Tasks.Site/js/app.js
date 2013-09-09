'use strict';

angular.module('piccoloClient', []).
	config(['$routeProvider', function($routeProvider) {
		$routeProvider.
			when('/', { templateUrl: '/partials/tasks.html', controller: TaskListController }).
			when('/:taskId', { templateUrl: '/partials/task.html', controller: TaskDetailController }).
			otherwise({ redirectTo: '/' });
	}]);

function TaskListController($scope) {
	$scope.page = 1;
	$scope.pageSize = 5;

	$scope.showTasks = function() {
		$.getJSON("http://piccolo.com/tasks?pageNumber=" + $scope.page + "&pageSize=" + $scope.pageSize).done(function(data) {
			$scope.$apply(function() {
				$scope.tasks = data.tasks;
				$scope.totalCount = data.totalCount;

				$scope.totalPages = Math.ceil(data.totalCount / $scope.pageSize);
				$scope.pageNumbers = [];
				for (var pageNumber = 0; pageNumber < $scope.totalPages; pageNumber++) {
					$scope.pageNumbers.push(pageNumber + 1);
				}

				if ($scope.tasks.length == 0) {
					$scope.showPreviousPage();
				}
			});
		});
	};

	$scope.search = function() {
		$.getJSON("http://piccolo.com/tasks/search?term=" + $scope.searchTerm + "&pageNumber=" + $scope.page + "&pageSize=" + $scope.pageSize).done(function(data) {
			$scope.$apply(function() {
				$scope.tasks = data.tasks;
				$scope.totalCount = data.totalCount;

				$scope.totalPages = Math.ceil(data.totalCount / $scope.pageSize);
				$scope.pageNumbers = [];
				for (var pageNumber = 0; pageNumber < $scope.totalPages; pageNumber++) {
					$scope.pageNumbers.push(pageNumber + 1);
				}
			});
		});
	};

	$scope.getTask = function(id) {
		var task;
		$.ajax({
			type: "GET",
			url: "http://piccolo.com/tasks/" + id,
			async: false
		}).done(function(data) {
			task = data;
		});
		return task;
	};

	$scope.createTask = function(task) {
		$.ajax({
			type: "POST",
			url: "http://piccolo.com/tasks",
			data: JSON.stringify(task)
		}).done(function() {
			$scope.showTasks();
		});
	};

	$scope.updateTask = function(task) {
		$.ajax({
			type: "PUT",
			url: "http://piccolo.com/tasks/" + task.id,
			data: JSON.stringify(task)
		}).done(function() {
			$scope.showTasks();
		});
	};

	$scope.deleteTask = function(id) {
		$.ajax({
			type: "DELETE",
			url: "http://piccolo.com/tasks/" + id
		}).done(function() {
			$scope.showTasks();
		});
	};

	$scope.tasks = $scope.showTasks();

	$scope.showPreviousPage = function() {
		if ($scope.page > 1) {
			$scope.page--;
			$scope.showTasks();
		}
	};

	$scope.showNextPage = function() {
		if ($scope.page < $scope.totalPages) {
			$scope.page++;
			$scope.showTasks();
		}
	};

	$scope.showPage = function(pageNumber) {
		$scope.page = pageNumber;
		$scope.showTasks();
	};

	$scope.addTask = function() {
		$scope.createTask({ title: $scope.newTaskTitle });
		$scope.newTaskTitle = "";
	};

	$scope.removeTask = function(id) {
		$scope.deleteTask(id);
	};

	$scope.toggleCompletion = function(id, isCompleted) {
		var task = $scope.getTask(id);
		task.isCompleted = isCompleted;
		$scope.updateTask(task);
	};
}


function TaskDetailController($scope, $routeParams) {
	$scope.getTask = function(id) {
		var task;
		$.ajax({
			type: "GET",
			url: "http://piccolo.com/tasks/" + id,
			async: false
		}).done(function(data) {
			task = data;
		});
		return task;
	};

	$scope.task = $scope.getTask($routeParams.taskId);
}