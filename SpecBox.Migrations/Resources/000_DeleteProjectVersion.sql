CREATE OR REPLACE PROCEDURE public."DeleteProjectVersion"(
	IN "v_VersionId" uuid)
LANGUAGE 'plpgsql'
AS $BODY$
BEGIN
	BEGIN
		DELETE FROM public."Assertion" WHERE "AssertionGroupId" IN 
			(SELECT ag."Id" FROM public."AssertionGroup" as ag 
				JOIN public."Feature" as ft on ag."FeatureId" = ft."Id"
			WHERE ft."ProjectId" = "v_VersionId");

		DELETE FROM public."AssertionGroup" WHERE "FeatureId" IN 
			(SELECT "Id" FROM public."Feature" WHERE "ProjectId" = "v_VersionId");

		DELETE FROM public."AssertionsStat" WHERE "ProjectId" = "v_VersionId";

		DELETE FROM public."AttributeGroupOrder" WHERE "AttributeId" IN
			(SELECT "Id" FROM public."Attribute" WHERE "ProjectId" = "v_VersionId");

		DELETE FROM public."AttributeValue" WHERE "AttributeId" IN
			(SELECT "Id" FROM public."Attribute" WHERE "ProjectId" = "v_VersionId");
		
		DELETE FROM public."Attribute" WHERE "ProjectId" = "v_VersionId";

		DELETE FROM public."AutotestsStat" WHERE "ProjectId" = "v_VersionId";

		DELETE FROM public."ExportFeatureAttribute" WHERE "ExportId" IN
			(SELECT "Id" FROM public."Export" WHERE "ProjectId" = "v_VersionId");

		DELETE FROM public."ExportFeature" WHERE "ExportId" IN
			(SELECT "Id" FROM public."Export" WHERE "ProjectId" = "v_VersionId");

		DELETE FROM public."ExportAssertion" WHERE "ExportId" IN
			(SELECT "Id" FROM public."Export" WHERE "ProjectId" = "v_VersionId");

		DELETE FROM public."Export" WHERE "ProjectId" = "v_VersionId";

		DELETE FROM public."TreeNode" WHERE "TreeId" IN 
			(SELECT "Id" FROM public."Tree" WHERE "ProjectId" = "v_VersionId");

		DELETE FROM public."Tree" WHERE "ProjectId" = "v_VersionId";

		DELETE FROM public."TestResult" WHERE "TestRunId" IN 
			(SELECT "Id" FROM public."TestRun" WHERE "ProjectId" = "v_VersionId");

		DELETE FROM public."TestRun" WHERE "ProjectId" = "v_VersionId";

		DELETE FROM public."FeatureAttributeValue" WHERE "FeatureId" IN 
			(SELECT "Id" FROM "Feature" WHERE "ProjectId" = "v_VersionId");

		DELETE FROM public."Feature" WHERE "ProjectId" = "v_VersionId";

		DELETE FROM public."Project" WHERE "Id" = "v_VersionId";
		
		EXCEPTION
	        WHEN OTHERS THEN
	            ROLLBACK;
	            RAISE EXCEPTION 'Error deleting project: %', SQLERRM;
	    END;
	COMMIT;
END;
$BODY$;
ALTER PROCEDURE public."DeleteProjectVersion"(uuid)
    OWNER TO postgres;
