CREATE FUNCTION [dbo].[FN_NUMOFASSIGNEDDEPTS](@PeriodId as INT, @DeptId as INT) RETURNS INT
AS 
BEGIN

	DECLARE @Count int;

	SELECT	@Count = Count(*)
	FROM 
		(SELECT DISTINCT DepartmentFrom
		FROM	PeriodSelectedDepartments d
		WHERE	DepartmentTo = @DeptId AND d.AssessmentPeriodId = @PeriodId) AS A;

	RETURN @Count;
END;
