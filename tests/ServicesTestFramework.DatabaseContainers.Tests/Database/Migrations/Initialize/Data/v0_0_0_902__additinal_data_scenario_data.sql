DROP PROCEDURE IF EXISTS DeployReferenceData;
CREATE PROCEDURE DeployReferenceData()
BEGIN
IF '${Scenario}' = 'AdditionalData' THEN 

INSERT INTO firstTable
 (Id, Name, SomeDate, Active) VALUES(101, 'Steven', utc_timestamp(), 1);
INSERT INTO firstTable
 (Id, Name, SomeDate, Active) VALUES(102, 'Notsteven', utc_timestamp(), 0);
 
INSERT INTO secondTable
 (Id, SomeText, SomeInt, Active) VALUES(101,'Text', 0, 1);
INSERT INTO secondTable
 (Id, SomeText, SomeInt, Active) VALUES(102,'Another text', 10, 1);
INSERT INTO secondTable
 (Id, SomeText, SomeInt, Active) VALUES(103,'Text again', 100, 1);
INSERT INTO secondTable
 (Id, SomeText, SomeInt, Active) VALUES(104,'Is it text? Yes, it is.', 1000, 1);
INSERT INTO secondTable
 (Id, SomeText, SomeInt, Active) VALUES(105,'Is it a bird? Is it a plane? No, it is text.', 1001, 0);
 
INSERT INTO thirdTable
 (FirstId, SecondId, Comment) VALUES(101, 101, '11');
INSERT INTO thirdTable
 (FirstId, SecondId, Comment) VALUES(101, 102, '12');
INSERT INTO thirdTable
 (FirstId, SecondId, Comment) VALUES(101, 103, '13');
INSERT INTO thirdTable
 (FirstId, SecondId, Comment) VALUES(102, 105, '26?');
 
END IF;
END;
CALL DeployReferenceData;
DROP PROCEDURE IF EXISTS DeployReferenceData;
