CREATE PROCEDURE SP_REPORT_2(@PeriodId as INT) 
AS
BEGIN
    SELECT	d.ShortCode as DepartmentId, d.Code as DepartmentName, d.DivCode, d.DivName, 
		dbo.FN_NUMOFASSIGNEDDEPTS(@PeriodId, dp.DepartmentTo) as NumOfAssignedDepts, 
		SUM(IIF(qs.CriteriaId=1,rs.Result,0)) as TotalCriteria1, AVG(IIF(qs.CriteriaId=1,CAST(rs.Result as float),null)) as AvgCriteria1,
		SUM(IIF(qs.CriteriaId=2,rs.Result,0)) as TotalCriteria2, AVG(IIF(qs.CriteriaId=2,CAST(rs.Result as float),null)) as AvgCriteria2,
		SUM(IIF(qs.CriteriaId=3,rs.Result,0)) as TotalCriteria3, AVG(IIF(qs.CriteriaId=3,CAST(rs.Result as float),null)) as AvgCriteria3,
		AVG(rs.Result) as AvgAllĐG
	FROM
		PeriodSelectedDepartments dp
		INNER JOIN Departments d ON d.Id = dp.DepartmentTo
		LEFT JOIN PeriodAssessmentResults rs ON dp.Id = rs.PeriodSelectedDepartmentId
		LEFT JOIN AssessmentQuestions qs ON qs.Id = rs.PeriodQuestionId

	WHERE	dp.AssessmentPeriodId=@PeriodId
	GROUP BY	d.ShortCode, d.Code, d.DivCode, d.DivName, dbo.FN_NUMOFASSIGNEDDEPTS(@PeriodId, dp.DepartmentTo)



END;

Go
