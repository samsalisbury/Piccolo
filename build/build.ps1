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
	write-host "##teamcity[progessMessage '$($psake.context.Peek().currentTaskName.ToUpper())']"
}

task compile -depends set-version-number {
	try {
		exec { msbuild $solution_file /m /property:"Configuration=$build_configuration;OutputPath=$build_output_dir" /nologo }
	}
	catch {
		write-host $_.Exception.Message
		write-host "##teamcity[buildStatus text='compile task failed - see build log for details' status='FAILURE']"
		throw ("************ compile task failed **************")
	}
}

task set-version-number -depends clean {
	try {
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
	catch {
		write-host $_.Exception.Message
		write-host "##teamcity[buildStatus text='set-version-number task failed - see build log for details' status='FAILURE']"
		throw ("************ set-version-number task failed **************")
	}
}

task clean {
	try {
		remove-item $build_output_dir -recurse -force -errorAction SilentlyContinue
	}
	catch {
		write-host $_.Exception.Message
		write-host "##teamcity[buildStatus text='clean task failed - see build log for details' status='FAILURE']"
		throw ("************ clean task failed **************")
	}
}