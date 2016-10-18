GO
/****** Object:  StoredProcedure [dbo].[SierraApi_AddError]    Script Date: 10/18/2016 13:23:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SierraApi_AddError]
(
	@Message varchar(max),
	@Application varchar(50),
	@OptionalNote varchar(max) = null
)
AS
BEGIN
	INSERT INTO SierraApiErrorLog
	VALUES (GetDate(), @Message, @Application, @OptionalNote)
END