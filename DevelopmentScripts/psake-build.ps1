. .\psake-common.ps1


task default -depends build


task build -depends build_initalize, change_to_dot_net_451, create_settings_files, force_update_dlls, build_projects {
 
}


task build_initalize {   
    ''
    ('='*25) + ' Build Parameters ' + ('='*25)
    'version      = ' + $version 
    'server type  = ' + $server_type 
    'build type   = ' + $build_type 
    'branch       = ' + $branch 
    'build config = ' + $build_config
    'Milyli build = ' + $milyli_build
	'Integration test = ' + $integration_test
	'Change to .net 4.5.1 = ' + $changeToDotNet451
    ''

    'Time: ' + (Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
    
    'Build Type and Server Type result in sign set to ' + ($build_type -ne 'DEV' -and $server_type -ne 'local')  

    if([System.IO.Directory]::Exists($buildlogs_directory)) {Remove-Item $buildlogs_directory -Recurse}
    [System.IO.Directory]::CreateDirectory($buildlogs_directory)
}

task create_settings_files {
    .\"..\Source\CreateOverrideConfigurationFile.ps1"
}

task force_update_dlls{
 & .\ForceUpdateDLLs.ps1 -file ..\Source\Increment.cs
}

task change_to_dot_net_451{
 Write-Host 'changeToDotNet451: ' + $changeToDotNet451
 
 .\build-ChangeToDotNet451.ps1 $changeToDotNet451
}

task get_buildhelper -precondition { (-not [System.IO.File]::Exists($buildhelper_exe)) } {
    exec {
        & $nuget_exe @('install', 'kCura.BuildHelper', '-ExcludeVersion')
    }      
    Copy-Item ([System.IO.Path]::Combine($development_scripts_directory, 'kCura.BuildHelper', 'lib', 'kCura.BuildHelper.exe')) $development_scripts_directory
}

task create_build_script -depends get_buildhelper {   
    exec {
        & $buildhelper_exe @(('/source:' + $root), 
                             ('/input:' + $inputfile), 
                             ('/output:' + $targetsfile), 
                             ('/graph:' + $dependencygraph), 
                             ('/dllout:' + $internaldlls), 
                             ('/vs:14.0'), 
                             ('/sign:' + ($build_type -ne 'DEV' -and $server_type -ne 'local')), 
                             ('/signscript:' + $signScript))
    }                                                                      
}  

task restore_nuget {

    foreach($o in Get-ChildItem $source_directory){
       
       if($o.Extension -ne '.sln' -or $o.BaseName -ne 'kCura.PDD.TeamCityRapBuild') {continue}

       if ($milyli_build) { $nugetParams = @('restore', $o.FullName, '-msbuildpath', '"C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin"') }
       else { $nugetParams = @('restore', $o.FullName) }

        exec {
            & $nuget_exe $nugetParams
        } 
    }   
}                                                                             
                                                                                
task build_projects -depends restore_nuget {  
    exec {     
		if ($build_type -eq 'DEV') {
			$Injections = 'EnableInjections'
		}
		
		$intParam = ('')
		if($integration_test)
		{
			$intParam = ('/p:DefineConstants=IntegrationTest')
		}
		
		Write-Host 'Based on' $build_type 'Injection is set to' $Injections
		
        &  $msbuild_exe @(('/property:SourceRoot=' + $root),
                         ('/property:Configuration=' + $build_config),	
                         ('/property:Injections=' + $Injections),						 
                         ('/p:DeployOnBuild=true'),
                         ('/p:PublishProfile=Local'),
						 ($intParam),
                         ('/verbosity:' + $verbosity),
                         ('/nologo'),
                         ('/maxcpucount'), 
                         ('/dfl'),
                         ('/flp:LogFile=' + $logfile),
                         ('/flp2:warningsonly;LogFile=' + $logfilewarn),
                         ('/flp3:errorsonly;LogFile=' + $logfileerror),
						 ($solution_file))       
    } 
}



