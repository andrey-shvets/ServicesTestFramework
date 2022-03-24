DROP PROCEDURE IF EXISTS DeployReferenceData;
CREATE PROCEDURE DeployReferenceData()
begin

INSERT INTO firstTable
 (Id, Name, SomeDate, Active) VALUES(1, 'Steven', utc_timestamp(), 1);
INSERT INTO firstTable
 (Id, Name, SomeDate, Active) VALUES(2, 'Stephen', utc_timestamp(), 0);
INSERT INTO firstTable
 (Id, Name, SomeDate, Active) VALUES(3, 'Phteven', utc_timestamp(), 1);
INSERT INTO firstTable
 (Id, Name, SomeDate, Active) VALUES(4, 'Albus Percival Wulfric Brian Dumbledore', utc_timestamp(), 1);
 
INSERT INTO secondTable
 (Id, SomeText, SomeInt, Active) VALUES(1,'Some text', 1, 1);
INSERT INTO secondTable
 (Id, SomeText, SomeInt, Active) VALUES(2, 'Some other text', 5, 1);
INSERT INTO secondTable
 (Id, SomeText, SomeInt, Active) VALUES(3, 'Once upon a time there was a little two-headed transdimensional turtle-monkey and he had a crossbow...', 10, 0);
 
INSERT INTO thirdTable
 (FirstId, SecondId, Comment) VALUES(1, 1, 'Yes');
INSERT INTO thirdTable
 (FirstId, SecondId, Comment) VALUES(2, 3, 'No');
INSERT INTO thirdTable
 (FirstId, SecondId, Comment) VALUES(3, 1, 'Yes');
INSERT INTO thirdTable
 (FirstId, SecondId, Comment) VALUES(3, 2, 'Oh, no!');
INSERT INTO thirdTable
 (FirstId, SecondId, Comment) VALUES(4, 2, 'But why?');
 
end;
CALL DeployReferenceData;
DROP PROCEDURE IF EXISTS DeployReferenceData;
