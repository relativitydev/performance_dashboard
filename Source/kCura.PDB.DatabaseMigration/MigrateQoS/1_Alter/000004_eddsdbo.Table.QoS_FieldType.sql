USE EDDSQoS
GO

IF NOT EXISTS ( SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_FieldType' AND TABLE_SCHEMA = 'EDDSDBO' )
BEGIN
	--This is an enumeration of all the field types in the Field table
	CREATE TABLE EDDSQoS.eddsdbo.QoS_FieldType
		(
			KFID INT IDENTITY(1, 1) ,
			PRIMARY KEY ( KFID ) ,
			FieldType VARCHAR(10) ,
			FieldTypeID INT,
		)
	INSERT INTO EDDSQoS.eddsdbo.QoS_FieldType
	VALUES  ( 'Empty', -1 ),
			( 'Varchar', 0 ),
			( 'Integer', 1 ),
			( 'Date', 2 ),
			( 'Boolean', 3 ),
			( 'Text', 4 ),
			( 'Code', 5 ),
			( 'Decimal', 6 ),
			( 'Currency', 7 ),
			( 'MultiCode', 8 ),
			( 'File', 9 ),
			( 'Object', 10 ),
			( 'User', 11 ),
			( 'LayoutText', 12 ),
			( 'Objects', 13 )
END