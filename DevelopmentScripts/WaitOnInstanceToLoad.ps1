param
(
    [String] [Parameter(Mandatory = $true)] $ServerName
)
Write-Output "Checking if Relativity Instance $ServerName is loaded and ready."
do{
    $statusCode = 500;
    try {
        $statusCode = (invoke-webrequest -uri "https://$ServerName.milyli.net/Relativity/" -TimeoutSec 5 -UseBasicParsing).statuscode
    }
	catch{
        $_.Exception.Message
        Write-Output "$ServerName is not loaded yet. Trying again in 10 seconds."
        Start-Sleep 10
    }
}while($statusCode -ne 200)

 Write-Output "$ServerName has completed loading."