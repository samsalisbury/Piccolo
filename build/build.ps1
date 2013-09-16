framework "4.0"

properties {
	$build_dir = split-path $psake.build_script_file
	$project_dir = "$build_dir\.."
	$build_output_dir = "$project_dir\build-output"
	
	$solution_file = "$project_dir\Piccolo.sln"
	$assembly_info_file = "$project_dir\GlobalAssemblyInfo.cs"
	
	$build_configuration = "Release"
	$version_major = 0
	$version_minor = 9
	$version_build = 0
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

task compile -depends set-version-number {
	exec { msbuild $solution_file /m /property:"Configuration=$build_configuration;OutputPath=$build_output_dir" /nologo }
}

task set-version-number -depends clean {
	$version = "$version_major.$version_minor.$version_build.0"
	write-host Setting assembly version to $version
	
	$package_version = "$version_major.$version_minor.$version_build"
	write-host "##teamcity[setParameter name='package.version' value='$package_version']"
	
	$versionPattern = 'AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
	$versionAssembly = 'AssemblyVersion("' + $version + '")';
	$versionFilePattern = 'AssemblyFileVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
	$versionAssemblyFile = 'AssemblyFileVersion("' + $version + '")';
 	
	$updatedContent = get-content $assembly_info_file | % {$_ -replace $versionFilePattern, $versionAssemblyFile } | % {$_ -replace $versionPattern, $versionAssembly }
	$updatedContent > $assembly_info_file
}

task clean {
	remove-item $build_output_dir -recurse -force -errorAction SilentlyContinue
}