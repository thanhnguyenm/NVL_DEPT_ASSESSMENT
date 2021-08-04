CREATE PROCEDURE [dbo].[SP_REPORT_6](@PeriodId as INT) 
AS
BEGIN
    	SELECT	d.Code as DepartmentName, AVG(CAST(rs.Result as float)) as AvgAllĐG
	FROM
		PeriodSelectedDepartments dp
		INNER JOIN Departments d ON d.Id = dp.DepartmentTo
		LEFT JOIN PeriodAssessmentResults rs ON dp.Id = rs.PeriodSelectedDepartmentId
		
	WHERE	dp.AssessmentPeriodId=@PeriodId
	GROUP BY	d.Code


END;


Go
