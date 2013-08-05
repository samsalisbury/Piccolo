angular.module('piccoloServices', ['ngResource']).
    factory('Tasks', function ($resource) {
    	return $resource('http://piccolo.com/tasks', {}, {
    		list: { method: 'GET', params: { page: 1, pageSize: 10 }, isArray: true },
    		add: { method: 'POST', params: {}, isArray: false }
    	});
    });