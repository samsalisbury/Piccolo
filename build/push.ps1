framework "4.0"

task default -depends package

formatTaskName {
	param($taskName)
	write-host
	write-host (("-"*30) + "[ " + $taskName.ToUpper() + " ]" + ("-"*30)) -fore Green
	write-host
}

task package {
	& nuget push $package_file -ApiKey $api_key
}