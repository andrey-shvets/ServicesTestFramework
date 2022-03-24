DROP PROCEDURE IF EXISTS DeployReferenceData;
CREATE PROCEDURE DeployReferenceData()
BEGIN

    CALL AddPair("New",  utc_timestamp(), "Initialized after stored procedures were added", 42,"Created with stored procedure",  @NewThirdId);

END;
CALL DeployReferenceData;
DROP PROCEDURE IF EXISTS DeployReferenceData;