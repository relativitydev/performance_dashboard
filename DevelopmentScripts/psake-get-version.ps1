. .\psake-common.ps1


task default -depends getversion


task getversion {
   if ($psake.build_success -eq $false) { exit 1 }
   
   $Conn = New-Object System.Data.SqlClient.SqlConnection
   $Conn.ConnectionString = "server='$server';Database='$database';user=StoryboardUser;password=Test1234!;"

   $Conn.Open()

   $Comm = New-Object System.Data.SqlClient.SqlCommand
   $Comm.Connection = $Conn
   $Comm.CommandText = "
SET NOCOUNT ON
		
--Declare variables used by SQL passed in from NANT script
DECLARE @productName AS VARCHAR(255) = '$product'
DECLARE @projectName AS VARCHAR(255) = '$project'
DECLARE @majorversion AS VARCHAR(10) = '$major_version'
DECLARE @minorversion AS VARCHAR(10) = '$minor_version'
DECLARE @patchversion AS VARCHAR(10) = '$patch_version'
DECLARE @branchID AS INT


--Insert the product/project into the table if it doesn't already exist
IF ('$server_type' <> 'local' AND NOT EXISTS(SELECT 1 FROM TCBuildSemanticVersion WHERE ProductName = @productName AND ProjectName = @projectName AND Major = @majorversion AND Minor = @minorversion AND Patch = @patchversion)) 
BEGIN
	INSERT INTO TCBuildSemanticVersion (ProductName, ProjectName, Major, Minor, Patch, Build)
	VALUES (@productName, @projectName, @majorversion, @minorversion, @patchversion, 0)
END


SET @branchID = (SELECT TOP 1 [ID]
                 FROM TCBuildSemanticVersion
				 WHERE ProductName = @productName 
				   AND ProjectName = @projectName
				   AND Major = @majorversion 
				   AND Minor = @minorversion
				   AND Patch = @patchversion)

--Select the version of the build
SELECT CAST(Major AS VARCHAR) + '.' + CAST(Minor AS VARCHAR) + '.' + CAST(Patch AS VARCHAR) + '.' + CAST(Build AS VARCHAR) AS BuildVersion
FROM TCBuildSemanticVersion
WHERE [ID] = @branchID

--Increment Patch if GOLD build
IF('$server_type' <> 'local' AND '$build_type' = 'GOLD')
    BEGIN
        UPDATE TCBuildSemanticVersion 
        SET Patch = Patch + 1,
            Build = 0 
        WHERE [ID] = @branchID
    END
ELSE
    BEGIN
    --Increment Build so next build will be a later version
    IF('$server_type' <> 'local')
        BEGIN
            UPDATE TCBuildSemanticVersion 
            SET Build = Build + 1 
            WHERE [ID] = @branchID
        END
    END

"

   $script:version = $Comm.ExecuteScalar()
         
   $Conn.Close()
  
   Write-Host "##teamcity[buildNumber '$script:version']"

   if($server_type -eq 'local'){
    [System.IO.File]::WriteAllText([System.IO.Path]::Combine($development_scripts_directory, 'version.txt'), $script:version)
   }
}





