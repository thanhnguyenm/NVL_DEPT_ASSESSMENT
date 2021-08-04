CREATE PROCEDURE [dbo].[SP_REPORT_7](@PeriodId as INT) 
AS
BEGIN
    SELECT 
			dp.DepartmentTo as DepartmentId,
			d.Code as DepartmentName,
			u.Email,
			IIF(dp.Finished=1,dp.ModifiedDate,null) as FinishedDate,
			'Cau hoi ' + cast(q.Id as Nvarchar) as QUESTION, 
            Result as Score,
			ResultComment as ScoreComment
        FROM 
			PeriodSelectedDepartments dp
			INNER JOIN Departments d ON dp.DepartmentTo = d.Id
			INNER JOIN Users u ON dp.UserId = u.Id
			LEFT JOIN PeriodAssessmentResults rs ON dp.Id = rs.PeriodSelectedDepartmentId
			LEFT JOIN PeriodQuetions pq ON pq.Id = rs.PeriodQuestionId
			LEFT JOIN AssessmentQuestions q ON q.Id = pq.AssessmentQuestionId
		WHERE dp.AssessmentPeriodId=@PeriodId
		
END;


Go
