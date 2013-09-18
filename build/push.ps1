framework "4.0"

properties {
	$build_dir = split-path $psake.build_script_file
	$nuget = "$build_dir\NuGet.exe"
	$package_file = get-childItem *.nupkg | select-object -first 1
	$api_key = $null
}

task default -depends package

formatTaskName {
	param($taskName)
	write-host
	write-host (("-"*30) + "[ " + $taskName.ToUpper() + " ]" + ("-"*30)) -fore Green
	write-host
}

task package {
	try {
		& $nuget push $package_file -ApiKey $api_key
	}
	catch {
		write-host $_.Exception.Message
		write-host "##teamcity[buildStatus text='package task failed - see build log for details' status='FAILURE']"
		throw ("************ package task failed **************")
	}
}