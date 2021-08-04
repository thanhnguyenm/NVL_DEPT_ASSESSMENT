CREATE PROCEDURE [dbo].[SP_DepartmentMatrix]

AS
    DECLARE @PivotColumnHeaders NVARCHAR(MAX)

    SELECT @PivotColumnHeaders =  
    COALESCE(@PivotColumnHeaders + ',[' +  cast(DepartmentTo as Nvarchar) + ']', '[' + cast(DepartmentTo as varchar)+ ']')
    FROM (SELECT DISTINCT DepartmentTo  From dbo.DepartmentMatrix WHERE Deleted=0) as A

    DECLARE @PivotTableSQL NVARCHAR(MAX)
	SET @PivotTableSQL = N'SELECT * FROM   
    (
        SELECT 
            DepartmentFrom, 
            Interact,
            DepartmentTo
        FROM 
            DepartmentMatrix m
        
    ) t 
    PIVOT(
        SUM(Interact) 
			    FOR DepartmentTo IN (' + @PivotColumnHeaders + '
     ) 
   ) AS PivotTable' 

   EXECUTE  (@PivotTableSQL)

RETURN
GO