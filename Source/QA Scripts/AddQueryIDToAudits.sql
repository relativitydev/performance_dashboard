/*
	[QueryID Audit Details Converter]
	This script will convert pre-9.2 audits to the 9.2 QueryID format (using the closest-fit IdList audit for association). Hopefully.
	It's a *really* good idea to take a backup of the workspace database before you do this in case you break the audits.
	This script does not affect audits that already have the QueryID element.
*/

/*
	STEP 1: Decide how far back you want to rewrite history...

	The further you go, the longer the updates are going to take.
	You will probably want to comment out the SELECTs below if you're going to update a large number of audits.
*/

SET NOCOUNT ON
DECLARE @hourOfReckoning DATETIME = getutcdate();
DECLARE @rowsInScope INT = (
	SELECT COUNT(*)
	FROM eddsdbo.AuditRecord WITH(NOLOCK)
	WHERE [Action] = 28
		AND [Timestamp] >= @hourOfReckoning
)
PRINT CAST(@rowsInScope as varchar(max)) + ' row(s) will be affected'

/*
	STEP 2: Add QueryIDs to all IdList audits

	This will bring back the audits within range and indicate the changes to the Details column.
	Run this before the update and make sure you're happy with it!
*/

SELECT
	ID,
	[Action],
	[Details],
	CASE WHEN CHARINDEX('&lt;QueryType&gt;IdList&lt;/QueryType&gt;', [Details], 0) != 0 THEN 'IdList'
		WHEN CHARINDEX('&lt;QueryType&gt;Count&lt;/QueryType&gt;', [Details], 0) != 0 THEN 'Count'
		WHEN CHARINDEX('&lt;QueryType&gt;Full&lt;/QueryType&gt;', [Details], 0) != 0 THEN 'Full'
		ELSE 'Unknown'
	END QueryType,
	CASE WHEN CHARINDEX('QueryID', [Details], 0) = 0
		THEN REPLACE([Details], '&lt;QueryType&gt;IdList&lt;/QueryType&gt;', '&lt;QueryType&gt;IdList&lt;/QueryType&gt;
	&lt;QueryID&gt;' + LOWER(CAST(NEWID() as varchar(36))) + '&lt;/QueryID&gt;')
		ELSE '* No change *'
	END ModifiedDetails,
	[Timestamp]
FROM eddsdbo.AuditRecord WITH(NOLOCK)
WHERE [Action] = 28
	AND [Timestamp] >= @hourOfReckoning
ORDER BY ID DESC

--This performs the update above, leaving existing rows with QueryIDs unchanged
PRINT 'Updating IdList audits...'

UPDATE eddsdbo.AuditRecord
SET Details =
	CASE WHEN CHARINDEX('QueryID', [Details], 0) = 0
		THEN REPLACE([Details], '&lt;QueryType&gt;IdList&lt;/QueryType&gt;', '&lt;QueryType&gt;IdList&lt;/QueryType&gt;
	&lt;QueryID&gt;' + LOWER(CAST(NEWID() as varchar(36))) + '&lt;/QueryID&gt;')
		ELSE [Details]
	END
WHERE [Action] = 28
	AND [Timestamp] >= @hourOfReckoning

PRINT 'Updated IdList audits.'

/* STEP 3: Add generated QueryID to Count and Full audits using the IdList audit that matches most closely in time */

/*
	This will bring back the audits within range and indicate changes to the Details column.
	It should only affect Count/Full query audits at this point as all IdList audits are already done...
	Run this before the update and make sure you're happy with it!

	IF YOU GET CONVERSION FAILED ERRORS, YOU NEED TO RUN STEP 1!
*/

--DECLARE @hourOfReckoning DATETIME = '2015-05-07 11:58:00.000';
SELECT
	ID,
	[Action],
	[Details],
	CASE WHEN CHARINDEX('&lt;QueryType&gt;IdList&lt;/QueryType&gt;', [Details], 0) != 0 THEN 'IdList'
		WHEN CHARINDEX('&lt;QueryType&gt;Count&lt;/QueryType&gt;', [Details], 0) != 0 THEN 'Count'
		WHEN CHARINDEX('&lt;QueryType&gt;Full&lt;/QueryType&gt;', [Details], 0) != 0 THEN 'Full'
		ELSE 'Unknown'
	END QueryType,
	CASE WHEN CHARINDEX('QueryID', [Details], 0) = 0 AND CHARINDEX('&lt;QueryType&gt;IdList&lt;/QueryType&gt;', [Details], 0) = 0
		THEN REPLACE([Details], '&lt;/QueryType&gt;', '&lt;/QueryType&gt;
	&lt;QueryID&gt;' + IdList.QueryID + '&lt;/QueryID&gt;')
		ELSE '* No change *'
	END FinalDetails,
	[Timestamp],
	CAST(CASE WHEN CHARINDEX('&lt;QueryType&gt;IdList&lt;/QueryType&gt;', [Details], 0) = 0 THEN IdList.QueryID
		ELSE LOWER(SUBSTRING([Details], CHARINDEX(N'&lt;QueryID&gt;', [Details]) + 15, 36))
	END as uniqueidentifier) QueryID
FROM eddsdbo.AuditRecord ar WITH(NOLOCK)
CROSS APPLY (
	SELECT TOP 1
		CASE WHEN CHARINDEX(N'&lt;QueryID&gt;', [Details]) = 0 THEN LOWER(CAST(NEWID() as varchar(36))) --It's an IdList audit, but there's no QueryID... Did you run Step 1?
			ELSE SUBSTRING([Details], CHARINDEX(N'&lt;QueryID&gt;', [Details]) + 15, 36)
		END QueryID
	FROM eddsdbo.AuditRecord WITH(NOLOCK)
	WHERE ID < ar.ID --Look at audits before this one...
		AND CHARINDEX('&lt;QueryType&gt;IdList&lt;/QueryType&gt;', [Details], 0) != 0 --Only look for IdList audits (this will be slow)
	ORDER BY ID DESC --Get the latest relevant audit we can find
) IdList
WHERE [Action] = 28
	AND [Timestamp] >= @hourOfReckoning

--This performs the update above, leaving the IdList audits unchanged.
PRINT 'Updating Count/Full audits...'

UPDATE ar
SET Details = 
	CASE WHEN CHARINDEX('QueryID', [Details], 0) = 0 AND CHARINDEX('&lt;QueryType&gt;IdList&lt;/QueryType&gt;', [Details], 0) = 0
		THEN REPLACE([Details], '&lt;/QueryType&gt;', '&lt;/QueryType&gt;
	&lt;QueryID&gt;' + IdList.QueryID + '&lt;/QueryID&gt;')
		ELSE [Details]
	END
FROM eddsdbo.AuditRecord ar WITH(UPDLOCK, ROWLOCK, READPAST)
CROSS APPLY (
	SELECT TOP 1
		CASE WHEN CHARINDEX(N'&lt;QueryID&gt;', [Details]) = 0 THEN LOWER(CAST(NEWID() as varchar(36))) --It's an IdList audit, but there's no QueryID... Did you run Step 1?
			ELSE SUBSTRING([Details], CHARINDEX(N'&lt;QueryID&gt;', [Details]) + 15, 36)
		END QueryID
	FROM eddsdbo.AuditRecord WITH(NOLOCK)
	WHERE ID < ar.ID --Look at audits before this one...
		AND CHARINDEX('&lt;QueryType&gt;IdList&lt;/QueryType&gt;', [Details], 0) != 0 --Only look for IdList audits (this will be slow)
	ORDER BY ID DESC --Get the latest relevant audit we can find
) IdList
WHERE [Action] = 28
AND [Timestamp] >= @hourOfReckoning

PRINT 'Updated Count/Full audits...'

/*
	STEP 4: Sanity check!! If you see NULL QueryIDs, we missed something. If you get a conversion failure, the QueryID we wrote to the audit was invalid. Whoops!
*/

--DECLARE @hourOfReckoning DATETIME = '2015-05-07 11:58:00.000';
SELECT
	ID,
	[Action],
	[Details],
	LOWER(SUBSTRING([Details], CHARINDEX(N'&lt;QueryID&gt;', [Details]) + 15, 36)) QueryIDString,
	CAST(SUBSTRING([Details], CHARINDEX(N'&lt;QueryID&gt;', [Details]) + 15, 36) as uniqueidentifier) QueryID
FROM eddsdbo.AuditRecord ar WITH(NOLOCK)
WHERE [Action] = 28
AND [Timestamp] >= @hourOfReckoning
ORDER BY ID DESC