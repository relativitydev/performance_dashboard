. .\psake-common.ps1


task default -depends package, sign


task package_initalize {
    $script:package_directory = [System.IO.Path]::Combine($package_root_directory, $Product, $branch, $version)
    $script:package_bin_directory = [System.IO.Path]::Combine($package_directory, 'bin')
    $script:package_rap_directory = [System.IO.Path]::Combine($package_directory, 'RAP')
    $script:package_cp_directory = [System.IO.Path]::Combine($package_directory, 'CustomPages')
    $script:package_nuget_directory = [System.IO.Path]::Combine($package_directory, 'NuGet')
    $script:package_pdb_directory = [System.IO.Path]::Combine($package_directory, 'PDBs')
    $script:package_doc_directory = [System.IO.Path]::Combine($package_directory, 'Documentation')

    if (Test-Path $package_directory) {
        Remove-Item $package_directory -Force -Recurse
    }

    [System.IO.Directory]::CreateDirectory($package_directory)
    [System.IO.Directory]::CreateDirectory($package_nuget_directory)
    [System.IO.Directory]::CreateDirectory($package_bin_directory) 
    [System.IO.Directory]::CreateDirectory($package_pdb_directory)
    [System.IO.Directory]::CreateDirectory($package_doc_directory)
}

task package -depends package_initalize { 

    Copy-Item -Path ([System.IO.Path]::Combine($nuspec_directory, '*')) -Destination $package_nuget_directory -Include '*.nupkg'
    
    if ((Test-Path $application_directory) -and ([System.IO.Directory]::GetFiles($application_directory, "*.rap", [System.IO.SearchOption]::TopDirectoryOnly).Count -gt 0)) {
        [System.IO.Directory]::CreateDirectory($package_rap_directory)
        Copy-Item -Path ([System.IO.Path]::Combine($application_directory, '*')) -Destination $package_rap_directory -Include '*.rap'
    }

    if ([System.IO.Directory]::Exists([System.IO.Path]::Combine($source_directory, 'CustomPages'))) {
        [System.IO.Directory]::CreateDirectory($package_cp_directory)
        Copy-Item -Path ([System.IO.Path]::Combine($source_directory, 'CustomPages', '*')) -Destination $package_cp_directory -Include '**' -Recurse
    }

    foreach($o in Get-ChildItem $source_directory) {
        if($o.PSIsContainer -and ([System.IO.Directory]::Exists([System.IO.Path]::Combine($o.FullName, 'bin')))) {
            Get-ChildItem -Path ([System.IO.Path]::Combine($o.FullName, 'bin')) -Recurse -Include '*.exe', '*.dll', '*.msi' | Copy-Item  -Destination $package_bin_directory
        }    
    }

	if (-not (Get-PackageProvider | Where {$_.Name -eq 'NuGet'})) {
		Install-PackageProvider -Name NuGet -MinimumVersion 2.8.5.201 -Force -confirm:$false
	}

	if (-not (Get-Module -ListAvailable -Name 7Zip4PowerShell)) {
		PowerShellGet\Install-Module -Name 7Zip4PowerShell -Force -confirm:$false
	}

	# Copy TestLibraries folder (non-C# keywords) to the package
	New-Item -ItemType Directory -Force -Path "$package_directory\$build_type $version Tests - INTERNAL USE ONLY"
	Compress-7Zip -Path "$root\FunctionalTests\" -ArchiveFileName "$package_directory\$build_type $version Tests - INTERNAL USE ONLY\$build_type $version FunctionalTests.7z" -Format SevenZip

	# Copy FunctionalTests folder to the package
	New-Item -ItemType Directory -Force -Path "$package_directory\$build_type $version Keywords - INTERNAL USE ONLY"
	Compress-7Zip -Path "$root\TestLibraries\" -ArchiveFileName "$package_directory\$build_type $version Keywords - INTERNAL USE ONLY\$build_type $version TestLibraries.7z" -Format SevenZip

    if ([System.IO.Directory]::Exists($pdb_directory)) {
        Copy-Item -Path ([System.IO.Path]::Combine($pdb_directory, '*')) -Destination $package_pdb_directory -Include '**' -Recurse
    }

    if ([System.IO.Directory]::Exists($doc_directory)) {
        Copy-Item -Path ([System.IO.Path]::Combine($doc_directory, '*')) -Destination $package_doc_directory -Include '**' -Recurse
    }
    
	New-Item $package_directory\BuildType_$build_type -Type file
	
}

task sign -precondition { ($build_type -ne 'DEV') -and ($server_type -ne 'local') } {
    foreach($o in Get-ChildItem -Path $package_directory -Recurse  -Include '*.exe', '*.dll', '*.msi') {
        exec {
            & $signscript @($o.FullName)
        }
    }
}
