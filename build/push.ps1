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
	& $nuget push $package_file -ApiKey $api_key
}