CREATE PROCEDURE SP_REPORT_5(@PeriodId as INT,@DepartmentId INT) 
AS
BEGIN
    DECLARE @PivotColumnHeaders NVARCHAR(MAX);

    SELECT @PivotColumnHeaders =  
    COALESCE(@PivotColumnHeaders + ',[' +  DepartmentName + ']', '[' + DepartmentName + ']')
    FROM (
		SELECT  distinct d.Name as DepartmentName
		From PeriodSelectedDepartments dp
		INNER JOIN Departments d ON dp.DepartmentFrom = d.Id
		WHERE dp.AssessmentPeriodId=@PeriodId) as A;

    DECLARE @PivotTableSQL NVARCHAR(MAX);

	SET @PivotTableSQL = N'SELECT * FROM   
    (
        SELECT 
			d.Name as DepartmentName,
			pq.AssessmentQuestionId as Question, 
            AVG(Result) as Score,
			0 as ''Tong Binh Quan''
        FROM 
			PeriodSelectedDepartments dp
			INNER JOIN Departments d ON dp.DepartmentFrom = d.Id
			INNER JOIN AssessmentPeriods p ON p.Id=dp.AssessmentPeriodId
			LEFT JOIN PeriodAssessmentResults rs ON dp.Id = rs.PeriodSelectedDepartmentId 
			LEFT JOIN PeriodQuetions pq ON pq.Id = rs.PeriodQuestionId
		WHERE dp.AssessmentPeriodId=CAST(' + CAST(@PeriodId AS nvarchar) + ' as INT) AND dp.DepartmentTo=CAST(' + CAST(@DepartmentId AS nvarchar) + ' as INT) 
				AND rs.PeriodQuestionId is not null
		GROUP BY d.Name, pq.AssessmentQuestionId
    ) t 
    PIVOT(
        Max(Score)
			    FOR DepartmentName IN (' + @PivotColumnHeaders + '
		)
   ) AS PivotTable ' ;


    EXECUTE  (@PivotTableSQL)


END;

Go
