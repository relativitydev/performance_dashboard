select ID 
from eddsdbo.HourSearchAuditBatches hb
where hb.HourId = @hourId and hb.ServerId = @serverId