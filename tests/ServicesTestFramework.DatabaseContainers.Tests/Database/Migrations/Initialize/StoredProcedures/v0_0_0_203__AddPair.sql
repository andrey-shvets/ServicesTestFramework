CREATE PROCEDURE AddPair(
	IN firstName VARCHAR(60),
    IN firstDate DATETIME,
	IN secondText VARCHAR(100),
    IN secondInt INT(10) UNSIGNED,
	IN thirdComment	VARCHAR(60),
	OUT newThirdId INT UNSIGNED
	)
BEGIN
	INSERT INTO firstTable
		(Name, SomeDate)
	VALUES
		(firstName, firstDate);
	
	SET @newFirstId := LAST_INSERT_ID();
	
	INSERT INTO secondTable
		(SomeText, SomeInt)
	VALUES
		(secondText, secondInt);
	
	SET @newSecondId := LAST_INSERT_ID();

	INSERT INTO thirdTable
		(FirstId, SecondId, Comment)
	VALUES
		(@newFirstId, @newSecondId, thirdComment);
	
	SET newThirdId := LAST_INSERT_ID();
END
