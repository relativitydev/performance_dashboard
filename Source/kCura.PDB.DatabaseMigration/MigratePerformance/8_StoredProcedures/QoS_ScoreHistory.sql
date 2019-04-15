IF EXISTS (select 1 from sysobjects where [name] = 'QoS_ScoreHistory' and type = 'P')  
BEGIN
	DROP PROCEDURE EDDSDBO.QoS_ScoreHistory
END
GO

CREATE PROCEDURE EDDSDBO.QoS_ScoreHistory
	@startDate DATETIME,
	@endDate DATETIME,
	@servers nvarchar(max)
AS
BEGIN
	--This procedure returns the User Experience and System Load scores for all servers within the desired time range
	DECLARE @serversTable TABLE(ArtifactId int null);
	
	DECLARE @x XML
	SELECT @X = CAST('<A>' + REPLACE(@servers, ',', '</A><A>') + '</A>' AS XML)

	INSERT INTO @serversTable
	SELECT NULLIF(t.value('.', 'varchar(50)'), '') AS inVal
	FROM @X.nodes('/A') AS x(t)
	
	DECLARE @Where NVARCHAR(MAX) = N'';
	DECLARE @serverCount int;
	SELECT @serverCount = COUNT(*) FROM @serversTable WHERE ArtifactId is not NULL
	
	DECLARE @serversString nvarchar(max) = N''
	
	IF (@serverCount > 0)
	BEGIN
		SELECT @serversString = @serversString + CONVERT(nvarchar(20), ArtifactId) + ',' FROM @serversTable WHERE ArtifactId is not NULL
		SET @serversString = SUBSTRING(@serversString, 0, len(@serversString)) -- trim extra ',' at the end
		SET @Where = N'
		AND qr.ServerArtifactID IN (' + @serversString + ')';
	END
		
	DECLARE @SQL NVARCHAR(MAX) = N'	
	SELECT
		qr.ServerArtifactID,
		s.ServerName,
		qr.SummaryDayHour,
		(UserExperience4SLRQScore + UserExperienceSLRQScore)/2 UserExperienceScore,
		SystemLoadScore,
		IntegrityScore,
		u.UptimeScore,
		CAST(CASE WHEN sh.HourId IS NULL THEN 0
			ELSE 1
		END as bit) IsSample
	FROM EDDSDBO.QoS_Ratings qr WITH(NOLOCK)
	CROSS APPLY (
		SELECT ur.UptimeScore
		FROM eddsdbo.QoS_UptimeRatings ur WITH(NOLOCK)
		WHERE qr.SummaryDayHour = ur.SummaryDayHour
	) u
	INNER JOIN eddsdbo.[Server] s WITH(NOLOCK)
	ON qr.ServerArtifactID = s.ArtifactID
	INNER JOIN eddsdbo.[Hours] h WITH(NOLOCK)
	ON qr.SummaryDayHour = h.HourTimeStamp and h.Status != 4
	LEFT JOIN eddsdbo.QoS_SampleHistoryUX sh WITH(NOLOCK)
	ON s.ServerID = sh.ServerId
	AND h.Id = sh.HourId
	WHERE qr.SummaryDayHour >= @startDate
	AND qr.SummaryDayHour < @endDate' + ISNULL(@Where, '') + '
	AND s.ServerTypeID = 3 --SQL servers
	AND COALESCE(S.IgnoreServer, 0) != 1
	AND S.DeletedOn IS NULL
	ORDER BY qr.SummaryDayHour DESC, qr.ServerArtifactID';
	
	PRINT @SQL;
	
	EXEC sp_executesql @SQL, N'@startDate DATETIME, @endDate DATETIME',
		@startDate,
		@endDate
END