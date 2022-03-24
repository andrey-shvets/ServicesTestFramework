CREATE 
VIEW firstSecondDetails
AS
	SELECT first.Id as FirstId, first.Name as Name, first.Active as FirstActive, second.Id as SecondId, second.SomeText as SomeText, second.Active as SecondActive, third.Comment as Comment
	FROM thirdTable third
	INNER JOIN firstTable AS first on first.Id = third.FirstId
	INNER JOIN secondTable AS second on second.Id = third.SecondId;