CREATE PROCEDURE SP_REPORT_3(@PeriodId as INT) 
AS
BEGIN
    DECLARE @PivotColumnHeaders NVARCHAR(MAX);
    SELECT @PivotColumnHeaders =  
    COALESCE(@PivotColumnHeaders + ',[' +  cast(Id as Nvarchar) + ']', '[' + cast(Id as varchar)+ ']')
    FROM (
		SELECT  'QUES_' + cast(q.Id as Nvarchar) as Id
		From AssessmentQuestions q
		INNER JOIN PeriodQuetions pq ON q.Id=pq.AssessmentQuestionId
		WHERE pq.AssessmentPeriodId=@PeriodId) as A;

    DECLARE @PivotTableSQL NVARCHAR(MAX);

	SET @PivotTableSQL = N'SELECT * FROM   
    (
        SELECT 
			dp.DepartmentTo as DepartmentId1,
			d.Name as DepartmentName1,
			dp.DepartmentFrom as DepartmentId2,
			d1.Name as DepartmentName2,
			''QUES_'' + cast(q.Id as Nvarchar) as Id, 
            ROUND(AVG(Cast(Result as float)),1) as Score,
			0.0 as ''Tong Diem'',
			0.0 as ''Tong Binh Quan''
        FROM 
			PeriodSelectedDepartments dp
			INNER JOIN Departments d ON dp.DepartmentTo = d.Id
			INNER JOIN Departments d1 ON dp.DepartmentFrom = d1.Id
			LEFT JOIN PeriodAssessmentResults rs ON dp.Id = rs.PeriodSelectedDepartmentId
			LEFT JOIN PeriodQuetions pq ON pq.Id = rs.PeriodQuestionId
			LEFT JOIN AssessmentQuestions q ON q.Id = pq.AssessmentQuestionId
		WHERE dp.AssessmentPeriodId=CAST(' + CAST(@PeriodId AS nvarchar) + ' as INT)
		GROUP BY dp.DepartmentTo, q.Id, d.Name, dp.DepartmentFrom, d1.Name
    ) t 
    PIVOT(
        SUM(Score)
			    FOR Id IN (' + @PivotColumnHeaders + '
     ) 
   ) AS PivotTable' ;


    EXECUTE  (@PivotTableSQL)

END;

Go
