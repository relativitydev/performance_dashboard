$SIGNTOOL = [System.IO.Path]::Combine(${env:ProgramFiles(x86)}, "Microsoft SDKs", "Windows", "v7.0A", "Bin", "signtool.exe")
$sites = @("http://timestamp.verisign.com/scripts/timstamp.dll",
           "http://timestamp.comodoca.com/authenticode",
           "http://www2.trustcenter.de/codesigning/timestamp")


function SignDLL($dll) {
    Write-Host "Checking" $dll

    & $SIGNTOOL verify /pa /q $dll
    $signed = $?
	
    if(-not $signed) {

		For($i =0; $i -lt 3; $i++) {
			ForEach($site in $sites){
				Write-Host "Attempting to sign" $dll "using" $site "..."
				& $SIGNTOOL sign /a /t $site /d "Relativity" /du "http://www.kcura.com" $dll
				$signed = $?
			
				if($signed) {
					Write-Host "Signed" $dll "Successfully!"
					break
				}
			}  
			
			if($signed) {
				break
			}
		}

		if(-not $signed) {
			exit 1
		}
    }
    else{
        Write-Host $dll "is already signed!"
    }
}

ForEach($arg in $args) {
	ForEach($dll in $arg.Split(';')) {
		if([System.IO.File]::Exists($dll)){
			SignDLL $dll
		}
		else{
			Write-Host "Cannot sign file" $dll "because it doesn't exist!"
			exit 1
		}
	}    
}