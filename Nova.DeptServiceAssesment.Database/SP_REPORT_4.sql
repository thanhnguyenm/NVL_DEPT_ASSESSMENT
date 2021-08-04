CREATE PROCEDURE SP_REPORT_4(@PeriodId as INT, @DeptId as INT) 
AS
BEGIN
    Select rs.PeriodQuestionId,  AVG(Result) as Score
    FROM PeriodSelectedDepartments dp
    LEFT JOIN PeriodAssessmentResults rs ON dp.Id = rs.PeriodSelectedDepartmentId
    WHERE dp.AssessmentPeriodId=@PeriodId AND dp.DepartmentTo=@DeptId AND rs.PeriodQuestionId is not null
    GROUP BY rs.PeriodQuestionId
	ORDER BY rs.PeriodQuestionId

END;

Go
