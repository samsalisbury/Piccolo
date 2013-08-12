framework "4.0"

properties {
	$build_dir = split-path $psake.build_script_file
	$project_dir = "$build_dir\.."

	$solution_file = "$project_dir\Piccolo.sln"
	$build_configuration = "Release"
	$build_output_dir = "$project_dir\build-output"
}

task default -depends compile

formatTaskName {
	param($taskName)
	write-host
	write-host (("-"*30) + "[ " + $taskName.ToUpper() + " ]" + ("-"*30)) -fore Green
	write-host
}

taskSetup {
	write-output "##teamcity[progessMessage '$($psake.context.Peek().currentTaskName.ToUpper())']"
}

task compile -depends clean {
	exec { msbuild $solution_file /m /property:"Configuration=TEST;OutputPath=$build_output_dir" /nologo }
}

task clean {
	remove-item $build_output_dir -recurse -force -errorAction SilentlyContinue
}