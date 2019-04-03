properties {
    $root = ""       
    $buildid = 0

    $buildserver = 'https://teamcity.kcura.corp'
    $database = 'TCBuildVersion'
    $proget = 'https://proget.kcura.corp/feeds/NuGet/'
}

task default -depends sendemail


task sendemail {

Add-Type -TypeDefinition @"
public struct Change 
{
    public string id;
    public string user;
    public string email;
    public string date;
    public string node;
    public string desc;
    public int mods;
    public int adds;
    public int dels;
}
"@

Add-Type -TypeDefinition @"
public struct Build 
{
    public int id;
    public string status;
    public string statusColor;
    public string statusText;
    public string canceledBy;
    public string canceledReason;
    public string product;
    public string project;
    public string config;
    public string branch;
    public string buildType;
    public string agent;
    public string version;
    public string triggeredBy;
    public System.DateTime triggeredTime;
    public System.DateTime startTime;  
    public System.DateTime endTime;
    public string changes;
    public string changers;
    public int changesCount;
    public string messages;
    public int dependencyID;
    public string dependencies;
    public string coverage;
}
"@

Function GetDate([String] $datestring) {

    if ($datestring -eq $null -or $datestring -eq '') {
        return Get-Date
    }

    $date = Get-Date -Year $datestring.Substring(0,4) -Month $datestring.Substring(4,2) -Day $datestring.Substring(6,2) -Hour $datestring.Substring(9,2) -Minute $datestring.Substring(11,2) -Second $datestring.Substring(13,2)
    
    return $date
}

Function GetTime([DateTime] $startdate, [DateTime] $enddate) {

    $ts = New-TimeSpan -Start $startdate -End $enddate

    $return = ''

    if($ts.TotalHours -gt 1) {
        $return += [String]($ts.Hours) + 'h:'
    }

    if($ts.TotalMinutes -gt 1) {
        $return += [String]($ts.Minutes) + 'm:'
    }

    $return += [String]($ts.Seconds) + 's'

    
    return $return
}

Function GetAuth() {
    $username = 'littleboss'
    $password = 'Idontcareifyouknowthispassword'

    return [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(("{0}:{1}" -f $username,$password)))
}

Function GetBuildInfo($id) {

    $base64AuthInfo = GetAuth
    $statusraw = Invoke-RestMethod -Headers @{Authorization=("Basic {0}" -f $base64AuthInfo)} "$buildserver/app/rest/builds/id:$id"

    $bld = New-Object Build
    $bld.id = $id
        
    $x = [xml]$statusraw.InnerXml

    $bld.status = $x.build.status
    $bld.statusText = $x.build.statusText
    $bld.product = $x.build.buildType.projectName.split(':', [StringSplitOptions]::RemoveEmptyEntries)[0].trim()
    $bld.project = $x.build.buildType.projectName.split(':', [StringSplitOptions]::RemoveEmptyEntries)[1].trim()
    $bld.config = $x.build.buildType.name
    $bld.branch = $x.build.branchName
    $bld.buildtype =  ($x.build.properties.property | ? {$_.name -eq 'buildType'} | select Value).value
    $bld.agent = $x.build.agent.name
    $bld.version = $x.build.number
    
    $bld.triggeredBy = $x.build.triggered.type
    $bld.triggeredtime = GetDate($x.build.triggered.date)
    $bld.startTime = GetDate($x.build.startDate)
    $bld.endTime = GetDate($x.build.finishDate)

    if($bld.triggeredBy -eq 'user') {
        $bld.triggeredBy = $x.build.triggered.user.username
    }
    elseif($bld.triggeredBy -eq 'buildType') {
        $bld.triggeredBy = $x.build.triggered.buildType.name
    }
    else {
        $bld.triggeredBy = $x.build.triggered.details
    }

    if($bld.status -eq 'SUCCESS') {
        $bld.statusColor = 'green'
    } 
    else {
        $bld.status = 'FAILURE'

        $bld.canceledBy = $x.build.canceledInfo.user.username
        $bld.canceledReason = $x.build.canceledInfo.text

        $bld.statusColor = 'red'  
    }

    $bld.dependencyID = $x.build.'snapshot-dependencies'.build.id

    return $bld
}

Function GetBuildChanges([Build] $bld) {
    $base64AuthInfo = GetAuth
    $id = $bld.id
    $changesraw = Invoke-RestMethod -Headers @{Authorization=("Basic {0}" -f $base64AuthInfo)} "$buildserver/app/rest/changes?locator=build:$id"

    $x = [xml]$changesraw.InnerXml

    $changes = @()
    $changesLimit = 5
    $bld.changesCount = 0

    Set-Location $root
    foreach ($node in $x.SelectNodes('/changes/change')){

        $bld.changesCount++

        if ($bld.changesCount -le $changesLimit) {
            $hg = (git --no-pager log $node.version -n 1 --pretty=format:"%aN|%aE|%ad|%H|%s" --date=format:"%m/%d/%Y %H:%M:%S" --name-status)
            
            $chng = New-Object Change
            $chng.id = $node.id
            $chng.user = $hg.split('|')[0]
            $chng.email = $hg.split('|')[1].replace(" + '@kcura.com'", "@kcura.com")
            $chng.date = $hg.split('|')[2]
            $chng.node = $hg.split('|')[3]            
            $chng.desc = $hg.split('|')[4]

            for($i = 5; $i -lt $hg.split('|').count; $i++){
                
                switch -wildcard ($hg.split('|')[$i][0]){
                    "A*" {$chng.adds++}
                    "M*" {$chng.mods++}
                    "D*" {$chng.dels++}
                }
            }
        }
        else {
            $hg = (git --no-pager log $node.version -n 1 --pretty=format:"%aE")

            $chng = New-Object Change
            $chng.email = $hg.split('|')[0].replace(" + '@kcura.com'", "@kcura.com")       
        }    

        $changes += $chng
    }

    $bld.changers = ''
    foreach ($chng in $changes) {
        if ($bld.changers -eq '') {
            $bld.changers = "'" + $chng.email + "'"
        }
        else {
            $bld.changers += ", '" + $chng.email + "'"
        }
    }

    if($bld.changers -eq ''){
        $bld.changers = "''"
    }

    $bld.changes = ''

    $urlFiles = "$buildserver/viewModification.html?modId=####&tab=vcsModificationFiles"
    $urlExtra = "$buildserver/viewLog.html?buildId=$buildid&tab=buildChangesDiv"

    $bld.changesCount = 0

    foreach ($chng in $changes) {
        $bld.changesCount++

        if ($bld.changesCount -gt $changesLimit) {
            continue
        }

        $bld.changes += '<table style="width: 100%; font-family: Tahoma; font-size: 10pt; border-bottom-style: dashed; border-width: 1px; border-color: #C0C0C0">
                            <tr>
                                <td style="padding-bottom: 0px;">
                                <table style="padding: 0px;">
                            <tr>
                         <td style="padding-bottom: 0px;">
                            <b>[' + $chng.user + ']</b>
                         </td>
                         <td style="font-size: 10pt; padding: 0px;">
                            (' + $chng.date + ')
                         </td>
                         <td rowspan="2" style="padding: 0px;">
                            <a href="' + $urlFiles.Replace('####', $chng.id) + '">'

        if($chng.mods -gt 0) {
            $bld.changes += '<span style="color: Orange;"> [Modified (' + $chng.mods + ')] </span>'
        }

        if($chng.adds -gt 0) {
            $bld.changes += '<span style="color: Green;"> [Added (' + $chng.adds + ')] </span>'
        }

        if($chng.dels -gt 0) {
            $bld.changes += '<span style="color: Red;"> [Deleted (' + $chng.dels + ')] </span>'
        }

        $bld.changes += '</a>
                         </td>
                         </tr>
                         <tr>
                            <td colspan="2" style="color: #0099FF; font-size: 10pt; padding: 0px;">' + $chng.node + '</td>
                         </tr>
                         </table>
                         </td>
                         </tr>
                         <tr>
                            <td style="padding-left: 23px; padding-top: 0px;">
                                <u>' + $chng.desc + '</u>
                            </td>
                         </tr>
                         </table>'
    }

    if ($bld.changesCount -gt $changesLimit) {
        $bld.changes += "<a href='$urlExtra'>And "

        if ($bld.changesCount -eq 100) {
            $bld.changes += "100+"
        }
        else {
            $bld.changes += ($bld.changesCount - $changesLimit)
        }

        $bld.changes += " others</a>"
    }
    elseif ($bld.changesCount -eq 0) {
        $bld.changes = "No changes in current build"
    }

    return $bld
}

Function GetBuildMessages([Build] $bld) {
    $base64AuthInfo = GetAuth
    $id = $bld.id
    $importantMessage = Invoke-RestMethod -Headers @{Authorization=("Basic {0}" -f $base64AuthInfo)} "$buildserver/viewLog.html?buildId=$id&tab=buildLog&filter=important&hideBlocks=true&state=&expand=all#_"

    $importantMessage = $importantMessage.Substring($importantMessage.IndexOf('<div class="log rTree" id="buildLog">'), $importantMessage.IndexOf('<!-- END EXTENSION CONTENT jetbrains.buildServer.controllers.viewLog.BuildLogTab -->', $importantMessage.IndexOf('<div class="log rTree" id="buildLog">')) - $importantMessage.IndexOf('<div class="log rTree" id="buildLog">'));

    while ($importantMessage.IndexOf("<script ") -gt 0)
    {
	    $importantMessage = $importantMessage.Remove($importantMessage.IndexOf("<script "), $importantMessage.IndexOf("</script>") - $importantMessage.IndexOf("<script ") + 9);
    }
    if ($importantMessage.IndexOf('<div id="buildLogProgress"') -gt 0)
    {
	    $importantMessage = $importantMessage.Remove($importantMessage.IndexOf('<div id="buildLogProgress"'), $importantMessage.IndexOf("</div>", $importantMessage.IndexOf('<div id="buildLogProgress"')) - $importantMessage.IndexOf('<div id="buildLogProgress"') + 6);
    }

    $importantMessage = $importantMessage.Replace("<br class='hidden'>", "")

    $IMs = New-Object System.Collections.ArrayList

    foreach ($x in ([xml]$importantMessage).SelectNodes('/div/div/div/i')){
        if ($x.Attributes["class"].Value -eq "mark error_msg  status_warn")
	    {
		    [void]$IMs.Add([System.String]::Format("<span style='color:Orange'>{0}</span><br/>", $x.InnerText))
	    }
	    elseif ($x.Attributes["class"].Value -eq "mark error_msg  status_err")
	    {
		    [void]$IMs.Add([System.String]::Format("<span style='color:RED'>{0}</span><br/>", $x.InnerText))
	    }
    }

    if($IMs.Count -gt 100) {
        $IMs.RemoveRange(50, $IMs.Count - 100)
        $IMs.Insert(50, "<br/>...<br/>")
    }

    foreach ($m in $IMs) {
        $bld.messages += $m
    }

    return $bld
}

Function GetBuildDependencies([Build] $bld, [int] $id) {

    if($bld.id -eq $id) {
        if($bld.dependencyID -ne 0) {
            $bld.dependencies += '<div align="center" style="border: 2px solid Red;">
                                    <a href="' + $buildserver + '/viewLog.html?buildId=' + $bld.id + '">
                                        ' + $bld.product + ' :: ' + $bld.project + ' :: ' + $bld.config + '<br/>
                                        ' + $bld.version + '
                                
                                    </a>
                                </div>'

           $bld = GetBuildDependencies $bld $bld.dependencyID
        }        
    }
    else {
        $bldTmp = GetBuildInfo $id

        $bld.dependencies += '<div align="center" style="font-size: large;">
                                &uarr;
                            </div>
                            <div align="center" style="border: 2px solid Black;">
                                <a href="' + $buildserver + '/viewLog.html?buildId=' + $bldTmp.id + '">
                                    ' + $bldTmp.product + ' :: ' + $bldTmp.project + ' :: ' + $bldTmp.config + '<br/>
                                    [' + $bldTmp.branch + '] ' + $bldTmp.agent + ' (' + $bldTmp.buildType + ') ' + $bldTmp.version + '
                                </a>
                            </div>'

        if($bldTmp.dependencyID -ne 0) {
            $bld = GetBuildDependencies $bld $bldTmp.dependencyID
        }
        else {
            $bld.branch = $bldTmp.branch
            $bld.version = $bldTmp.version
        }
    }      

    return $bld
}

Function GetStatistics([Build] $bld, [int] $id) {
    $base64AuthInfo = GetAuth
    $statusraw = Invoke-RestMethod -Headers @{Authorization=("Basic {0}" -f $base64AuthInfo)} "$buildserver/app/rest/builds/id:$id/statistics"
    
    $x = [xml]$statusraw.InnerXml

    $ccc =  ($x.properties.property | ? {$_.name -eq 'CodeCoverageAbsCCovered'} | select Value).value
    $ccct = ($x.properties.property | ? {$_.name -eq 'CodeCoverageAbsCTotal'} | select Value).value
    $ccm =  ($x.properties.property | ? {$_.name -eq 'CodeCoverageAbsMCovered'} | select Value).value
    $ccmt = ($x.properties.property | ? {$_.name -eq 'CodeCoverageAbsMTotal'} | select Value).value
    $ccb =  ($x.properties.property | ? {$_.name -eq 'CodeCoverageAbsBCovered'} | select Value).value
    $ccbt = ($x.properties.property | ? {$_.name -eq 'CodeCoverageAbsBTotal'} | select Value).value
    $ccl =  ($x.properties.property | ? {$_.name -eq 'CodeCoverageAbsLCovered'} | select Value).value
    $cclt = ($x.properties.property | ? {$_.name -eq 'CodeCoverageAbsLTotal'} | select Value).value
    
    $bld.coverage = '<table>
							<tr>
								<td>Classes</td>
								<td>:</td>
								<td>'+ "{0:N2}" -f (($ccc / $ccct) * 100) +'%</td>	
                                <td>('+ [int]$ccc + '/' + [int]$ccct + ')</td>								
							</tr>
							<tr>
								<td colspan="4" style="padding: 0px; border: 1px solid #00CC00;"><table style="width: '+ (($ccc / $ccct) * 100) +'%; border-collapse:collapse;"><tr style="line-height: 10px; border-collapse:collapse;"><td style="background-color:#00CC00; border-collapse:collapse;"></td></tr></table> </td>
							</tr>
							<tr>
								<td>Methods</td>
								<td>:</td>
								<td>'+ "{0:N2}" -f (($ccm / $ccmt) * 100) +'%</td>
                                <td>('+ [int]$ccm + '/' + [int]$ccmt + ')</td>
							</tr>
							<tr>
								<td colspan="4" style="padding: 0px; border: 1px solid #00CC00;"><table style="width: '+ (($ccm / $ccmt) * 100) +'%; border-collapse:collapse;"><tr style="line-height: 10px; border-collapse:collapse;"><td style="background-color:#00CC00; border-collapse:collapse;"></td></tr></table> </td>
							</tr>
							<tr>
								<td>Blocks</td>
								<td>:</td>
								<td>'+ "{0:N2}" -f (($ccb / $ccbt) * 100) +'%</td>
                                <td>('+ [int]$ccb + '/' + [int]$ccbt + ')</td>
							</tr>
							<tr>
								<td colspan="4" style="padding: 0px; border: 1px solid #00CC00;"><table style="width: '+ (($ccb / $ccbt) * 100) +'%; border-collapse:collapse;"><tr style="line-height: 10px; border-collapse:collapse;"><td style="background-color:#00CC00; border-collapse:collapse;"></td></tr></table> </td>
							</tr>
							<tr>
								<td>Lines</td>
								<td>:</td>
								<td>'+ "{0:N2}" -f (($ccl / $cclt) * 100) +'%</td>
                                <td>('+ [int]$ccl + '/' + [int]$cclt + ')</td>
							</tr>
							<tr>
								<td colspan="4" style="padding: 0px; border: 1px solid #00CC00;"><table style="width: '+ (($ccl / $cclt) * 100) +'%; border-collapse:collapse;"><tr style="line-height: 10px; border-collapse:collapse;"><td style="background-color:#00CC00; border-collapse:collapse;"></td></tr></table> </td>
							</tr>
						</table>'


    return $bld
}


Function GetSQL($product, $project, $branch, $status, $triggeredBy, $changesCount, $changers) {
    return "
SET NOCOUNT ON
	--Declare variables used by SQL passed in from NANT script
	DECLARE @projectName AS VARCHAR(255) = '$project'
	DECLARE @branchName AS VARCHAR(255) = '$branch'
	DECLARE @productName AS VARCHAR(255) = '$product'
	DECLARE @branchID as INT
	
    IF(NOT EXISTS(SELECT TOP 1 1 FROM TCBranches 
	        		             WHERE ProjectName = @projectName 
	                               AND BranchName = @branchName 
	                               AND ProductName = @productName))
	BEGIN
		INSERT INTO TCBranches(ProductName, ProjectName, BranchName) VALUES (@productName, @projectName, @branchName)
	END


	SET @branchID = (SELECT TOP 1 [ID] 
	                 FROM TCBranches
					 WHERE ProjectName = @projectName 
	                   AND BranchName = @branchName 
	                   AND ProductName = @productName)
	
	--Creates temp table to store all emails, used with DISTINCT so each email appears only once.
	CREATE TABLE #tmpEmails (Email VARCHAR (255))
	
	--Get email addresses of people subscribed to either Successful or Failure builds
	INSERT INTO #tmpEmails
	SELECT tcue.UserEmail
	FROM TCUserSubscription tcus
	INNER JOIN TCUserEmail tcue ON tcus.UserEmailID = tcue.[ID]
	INNER JOIN TCBranches tcb ON tcus.ProjectBranchID = tcb.[ID]
	WHERE tcb.[ID] = @branchID
	  AND tcus.Send${status} = 1
	  AND tcus.Send${status}Only1 = 0
	  
	--Get email addresses of people subscribed to either Successful or Failure builds after a failed or succeeded build respectively
	INSERT INTO #tmpEmails
	SELECT tcue.UserEmail
	FROM TCUserSubscription tcus
	INNER JOIN TCUserEmail tcue ON tcus.UserEmailID = tcue.[ID]
	INNER JOIN TCBranches tcb ON tcus.ProjectBranchID = tcb.[ID]
	WHERE tcb.[ID] = @branchID
	  AND tcus.Send${status}Only1 = 1
	  AND tcb.[Status] = CASE WHEN '$status' = 'Success' THEN 0 ELSE 1 END 
	
	--Get email addresses of people subscribed to builds they have triggered
	INSERT INTO #tmpEmails
	SELECT tcue.UserEmail
	FROM TCUserSubscription tcus
	INNER JOIN TCUserEmail tcue ON tcus.UserEmailID = tcue.[ID]
	INNER JOIN TCBranches tcb ON tcus.ProjectBranchID = tcb.[ID]
	WHERE tcb.[ID] = @branchID
      AND tcus.SendTrigger = 1 
	  AND tcue.UserEmail like '${triggeredBy}%'
		
	--If there are any changes, get email addresses of people who subscribed to builds that contain their changes
	IF($changesCount > 0)
	BEGIN
		INSERT INTO #tmpEmails
		SELECT tcue.UserEmail
		FROM TCUserSubscription tcus
		INNER JOIN TCUserEmail tcue ON tcus.UserEmailID = tcue.[ID]
		INNER JOIN TCBranches tcb ON tcus.ProjectBranchID = tcb.[ID]
		WHERE tcb.[ID] = @branchID
		  AND tcus.SendChanges = 1 
		  AND tcue.UserEmail IN ($changers)
	END
	
	--Send failed builds to any and all changers in build
	IF('$status' <> 'SUCCESS')
	BEGIN
		INSERT INTO #tmpEmails
		SELECT tcue.UserEmail
		FROM TCUserEmail tcue
		WHERE tcue.UserEmail IN ($changers)
	END
	
	--Update branch table with build status
	UPDATE TCBranches 
	SET [Status] = CASE WHEN '$status' = 'SUCCESS' THEN 1 ELSE 0 END 
	WHERE ID = @branchID
	
	--Get distinct emails, so they only show up once
	SELECT DISTINCT Email FROM #tmpEmails
	DROP TABLE #tmpEmails
"
}

Function NuGetParseVersion($nugetString) {
    
    $parts = $nugetString.Split('.')

    $returnVal = ''

    $num = $false

    foreach ($p in $parts) {
        
        if (-not $num) {
            if ($p -match '^[1-9]') {
                $returnVal += '/'
                $num = $true
            }
            elseif ($returnVal.Length -gt 0) {
                $returnVal += '.'
            }            
        }
        elseif ($returnVal.Length -gt 0) {
            $returnVal += '.'
        }

        $returnVal += $p
    }

    return $returnVal
}

$build = GetBuildInfo $buildid

$build = GetBuildChanges $build

if($build.status -ne 'SUCCESS') {
    $build = GetBuildMessages $build
}

$build = GetBuildDependencies $build $build.id

if($build.config -eq 'CodeCoverage') {
    $build = GetStatistics $build $build.id
}



$body = '<div>
			<table style="width: 100%; font-family: Tahoma;">
				<tr>
					<td>
						<b>' + $build.config + '</b>&nbsp;<b style="color:' + $build.statusColor + '">' + $build.status + '</b>'



if($build.canceledBy -ne $null -and $build.canceledBy -ne ''){
    $body += ' (Canceled by ' + $build.canceledBy + ' | Reason: "<i>' + $build.canceledReason + '</i>")'
}

$body += '
					</td>
					<td style="text-align: right">
						<a href="' + $buildserver + '" style="text-decoration:none;"><b style="color:2AA4FC">Team</b><font color="FBBD30">City</font></a>
					</td>
				</tr>		
			</table>		
		</div>
		<div>
			<table style="font-family: Tahoma;">	

				<tr>
					<td>Triggered&nbsp;By</td>
					<td>:</td>
					<td>' + $build.triggeredby + ' @ ' + $build.startTime.ToShortDateString() + ' ' + $build.startTime.ToShortTimeString() + '</td>
				</tr>
				<tr>
					<td>Project</td>
					<td>:</td>
					<td>' + $build.product + ' :: '+ $build.project + ' :: ' + $build.config + '</td>
				</tr>
                <tr>
					<td>Branch</td>
					<td>:</td>
					<td>'+ $build.branch + '</td>
				</tr>'

if($build.buildType -ne $null -and $build.buildType -ne ''){
	$body += '			<tr>
					<td>Build&nbsp;Type</td>
					<td>:</td>
					<td>'+ $build.buildType +'</td>
				</tr>'

}

$body += '
                <tr>
					<td>Version</td>
					<td>:</td>
					<td>'+ $build.version +'</td>
				</tr>
				<tr>
					<td>Agent</td>
					<td>:</td>
					<td>' + $build.agent + '</td>
				</tr>	
				<tr>
					<td>Build&nbsp;Time</td>
					<td>:</td>
					<td>' + $build.startTime.ToShortDateString() + ' ' + $build.startTime.ToShortTimeString() + ' - ' + $build.endTime.ToShortTimeString()  + ' (<i>' + (GetTime $build.startTime $build.endTime) + '</i>)</td>
				</tr>'


if($build.status -eq 'SUCCESS') {

    if($build.config.startsWith('Build')) {
        $body += '<tr>
				    <td style="vertical-align: top">Package</td>
				    <td style="vertical-align: top">:</td>
				    <td><a href="file:\\BLD-PKGS.kcura.corp\Packages\' + $build.product + '\' + $build.branch + '\' + $build.version + '">\\BLD-PKGS.kcura.corp\Packages\' + $build.product + '\' + $build.branch + '\' + $build.version + '</a></td>
			    </tr>'
        

        foreach ($folder in [System.IO.Directory]::GetDirectories('\\BLD-PKGS.kcura.corp\Packages\' + $build.product + '\' + $build.branch + '\' + $build.version)) {
            $body += '<a style="font-size: 8pt" href="' + $folder + '">[' +  [System.IO.Path]::GetFileName($folder) + ']</a>  '
        }

        
        
        if ([System.IO.Directory]::Exists('\\BLD-PKGS.kcura.corp\Packages\' + $build.product + '\' + $build.branch + '\' + $build.version + '\NuGet')) {
            $body += '<tr>
				        <td style="vertical-align: top">NuGet</td>
				        <td style="vertical-align: top">:</td>
				        <td>'

            foreach ($file in [System.IO.Directory]::GetFiles(('\\BLD-PKGS.kcura.corp\Packages\' + $build.product + '\' + $build.branch + '\' + $build.version + '\NuGet'))) {
                $body += '<a href="' +  $proget + (NuGetParseVersion ([System.IO.Path]::GetFileNameWithoutExtension($file))) +'">' +  [System.IO.Path]::GetFileNameWithoutExtension($file) + '</a><br/>'
            }
         
            
            $body += '</td>
			        </tr>'
        }
               
    }
    elseif($build.config -eq 'CodeCoverage') {
        $body += '<tr>
				    <td>Code&nbsp;Coverage</td>
				    <td>:</td>
				    <td>' + $build.coverage + '</td>
			    </tr>'
    }
    
}
else {
    $body += '<tr>
				<td style="vertical-align: top"><font color="red">Errors</font></td>
				<td style="vertical-align: top"><font color="red">:</font></td>
				<td><font color="red">' + $build.statusText + '</font>
					<div style="font-family:Courier New, Courier, monospace; font-size: x-small; font-weight: normal; font-style: normal;">
					' + $build.messages + '
					</div>
				</td>
			</tr>
			<tr>
				<td></td>
				<td></td>
				<td><font color="red">To claim ownership of this issue, follow instructions <a href="https://einstein.kcura.com/display/DV/Team+City+build+failure+responsibility">here</a></font></td>
			</tr>'
}


$body += '<tr>
					<td>Links</td>
					<td>:</td>
					<td><a href="' + $buildserver + '/viewLog.html?buildId=' + $build.id + '">[Overview]</a>  <a href="' + $buildserver + '/viewLog.html?buildId=' + $build.id + '&tab=buildLog&filter=debug">[Build Log]</a> ' 

if([System.IO.File]::Exists('\\BLD-PKGS.kcura.corp\Packages\' + $build.product + '\' + $build.branch + '\' + $build.version + '\CoverageReports\fullcoveragereport.html')){

$body += ' <a href="file://BLD-PKGS.kcura.corp/Packages/' + $build.product + '/' + $build.branch + '/' + $build.version + '/CoverageReports/fullcoveragereport.html">[Code Coverage Report]</a>'

}

$body += '</td>
				</tr>'

if($build.dependencies -ne $null) {
$body += '  <tr>
			    <td style="vertical-align: top">Dependencies</td>
			    <td style="vertical-align: top">:</td>
			    <td style="font-size: 10pt">' + $build.dependencies + '</td>
		    </tr>'
}


$body += '  <tr>
			    <td style="vertical-align: top">Changes</td>
			    <td style="vertical-align: top">:</td>
			    <td style="font-size: 10pt">' + $build.changes + '</td>
		    </tr>
		</table>
		</div>
		<br /><br />
		<div><a href="https://storyboard.kcura.com/Relativity/External.aspx?AppID=1015532&ArtifactID=1015532&DirectTo=%25applicationPath%25%2fCustomPages%2f433751b7-9a72-4166-a0bc-c0af0133a780%2fkCura.TeamCityEmail.aspx%3fStandardsCompliance%3dtrue+&SelectedTab=1070366">Manage TeamCity Email</a></div>
		<br /><br />
		<div style="position: relative; bottom: 0px; text-align: center; width: 100%; font-family: Tahoma; font-size: small; color: #C0C0C0;">Powered by Gadgets&trade; Team</div> '


$Conn = New-Object System.Data.SqlClient.SqlConnection
$Conn.ConnectionString = "server='teamcity.kcura.corp';Database='$database';trusted_connection=true;"

$Conn.Open()

$Comm = New-Object System.Data.SqlClient.SqlCommand
$Comm.Connection = $Conn
$Comm.CommandText = GetSQL $build.product $build.project $build.branch $build.status $build.triggeredBy $build.changesCount $build.changers

$rdr = $Comm.ExecuteReader()
$emails = @()

while($rdr.Read()) {
    $emails += $rdr.GetValue($1)
}

if($emails.length -gt 0){
    $buildConfig = $build.config
    $status = $build.status
    $product = $build.product
    $branch = $build.branch
    $version = $build.version

    Send-MailMessage -From 'TeamCity@kcura.com' -To $emails -Subject "$buildConfig $status - $product [$branch] - $version" -BodyAsHtml $body -SmtpServer "smtp.kcura.corp"
}

}




