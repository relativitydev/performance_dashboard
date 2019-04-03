UPDATE EDDSDBO.Configuration
SET Value = @value
WHERE Section = @section AND Name = @name;