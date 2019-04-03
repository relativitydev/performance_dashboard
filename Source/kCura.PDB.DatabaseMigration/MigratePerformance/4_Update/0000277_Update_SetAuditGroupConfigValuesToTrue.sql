--Do once on install but also every hour we need to make sure these are turned on
UPDATE EDDS.EDDSDBO.Configuration
SET Value = 'True'
WHERE Section = 'Relativity.Core'
AND (
	Name = 'AuditCountQueries'
	OR Name = 'AuditFullQueries'
	OR Name = 'AuditIdQueries'
)