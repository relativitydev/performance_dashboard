. .\psake-common.ps1


task default -depends test


task test_initalize {

    If([System.IO.Directory]::Exists($testlog_directory)) {
        [System.IO.Directory]::Delete($testlog_directory, $true)
    }

    [System.IO.Directory]::CreateDirectory($testlog_directory)  
}

task get_testrunner -precondition { (-not [System.IO.File]::Exists($testrunner_exe)) }  {
    exec {
        & $nuget_exe @('install', 'kCura.TestRunner', '-ExcludeVersion')
    }    
    Copy-Item ([System.IO.Path]::Combine($development_scripts_directory, 'kCura.TestRunner', 'lib', 'kCura.TestRunner.exe')) $development_scripts_directory
}

task get_nunit -precondition { (-not [System.IO.File]::Exists($NUnit3)) }  {
    exec {
        & $nuget_exe @('install', 'NUnit.Console', '-Version', '3.0.1', '-ExcludeVersion')
    } 
}

task test -depends get_testrunner, get_nunit, test_initalize {
    exec {
        & $testrunner_exe @(('/source:' + $root), 
                            ('/tests:' + $inputfile), 
                            ('/out:' + $testlog_directory), 
                            ('/nunit3:' + $NUnit3))
    }
}




