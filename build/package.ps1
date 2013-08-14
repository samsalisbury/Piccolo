framework "4.0"

properties {
	$build_dir = split-path $psake.build_script_file
	$project_dir = "$build_dir\.."
	$build_output_dir = "$project_dir\build-output"
	
	$nuget = "$project_dir\packages\NuGet.CommandLine.2.6.1\tools\NuGet.exe"
	$nuspec = "$project_dir\Piccolo\Piccolo.nuspec"
	$version = "0.0.0"
}

task default -depends package

formatTaskName {
	param($taskName)
	write-host
	write-host (("-"*30) + "[ " + $taskName.ToUpper() + " ]" + ("-"*30)) -fore Green
	write-host
}

task package {
	& $nuget pack $nuspec -Version $version -OutputDirectory $project_dir
}