angular.module('piccoloServices', ['ngResource']).
    factory('Tasks', function ($resource) {
    	return $resource('http://piccolo.com/tasks', {}, {
    		query: { method: 'GET', params: {}, isArray: true }
    	});
    });