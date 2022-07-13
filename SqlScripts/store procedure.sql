DROP PROCEDURE IF EXISTS `SP_Filter_LeadManagementReport`;
DELIMITER $$
CREATE PROCEDURE `SP_Filter_LeadManagementReport`( 

IN dateFirstAddedFrom DATETIME,
IN dateFirstAddedTo DATETIME,

IN totalTimesExportedFrom INT,
IN totalTimesExportedTo INT,

IN dateLastExportedFrom DATETIME,
IN dateLastExportedTo DATETIME,

IN last3CampaignsUsed varchar(200),

IN dateLastOccurredFrom DATETIME,
IN dateLastOccurredTo DATETIME,

IN occurredCategories varchar(200),

IN totalOccurancePointsFrom INT,
IN totalOccurancePointsTo INT,

IN resultsCategories varchar(200),

IN totalResultsPointsFrom INT,
IN totalResultsPointsTo INT,

IN totalPointsFrom INT,
IN totalPointsTo INT,

IN exportVsPointsPercentageFrom INT,
IN exportVsPointsPercentageTo INT,

IN exportVsPointsExceptions varchar(200),

IN exportVsPointsNumberFrom INT,
IN exportVsPointsNumberTo INT,

IN sortBy varchar(200),
IN sortDirection varchar(200),

IN exportLimit INT,
in exportOffset INT
) 
BEGIN
    select lm.CustomerMobileNo 
    from LeadManagementReport lm
    where 
	    	        
	        (if(dateFirstAddedFrom is null, 1, lm.DateFirstAdded >= dateFirstAddedFrom))
	    and (if(dateFirstAddedTo is null, 1, lm.DateFirstAdded <= dateFirstAddedTo))
	    
	    and (if(totalTimesExportedFrom is null, 1, lm.TotalTimesExported >= totalTimesExportedFrom)) 
	    and (if(totalTimesExportedTo is null, 1, lm.TotalTimesExported <= totalTimesExportedTo))
	    
	    and (if(dateLastExportedFrom is null, 1, lm.DateLastExported >= dateLastExportedFrom))
	    and (if(dateLastExportedTo is null, 1, lm.DateLastExported <= dateLastExportedTo))
	    
	    and (if(last3CampaignsUsed is null, 1,  FIND_IN_SET(lm.LastUsedCampaignId, last3CampaignsUsed)>0 
	    										 or FIND_IN_SET(lm.SecondLastUsedCampaignId, last3CampaignsUsed)>0  
	     										 or FIND_IN_SET(lm.ThirdLastUsedCampaignId, last3CampaignsUsed)>0)) 
	   		
    	and (if(dateLastOccurredFrom is null, 1, lm.DateLastOccurred >= dateLastOccurredFrom))
	    and (if(dateLastOccurredTo is null, 1, lm.DateLastOccurred <= dateLastOccurredTo))
	    
	    and (if(occurredCategories is null, 1, (FIND_IN_SET(1,occurredCategories)>0 and lm.OccuranceTotalFirstScore>0) 
											   or (FIND_IN_SET(2,occurredCategories)>0 and lm.OccuranceTotalSecondScore>0)
										       or (FIND_IN_SET(3,occurredCategories)>0 and lm.OccuranceTotalThirdScore>0)
											   or (FIND_IN_SET(4,occurredCategories)>0 and lm.OccuranceTotalFourthScore>0)
											   or (FIND_IN_SET(5,occurredCategories)>0 and lm.OccuranceTotalFifthScore>0)))
											   
    	and (if(totalOccurancePointsFrom is null, 1, lm.TotalOccurancePoints >= totalOccurancePointsFrom))
	    and (if(totalOccurancePointsTo is null, 1, lm.TotalOccurancePoints <= totalOccurancePointsTo))
	    
	    and (if(resultsCategories is null, 1, (FIND_IN_SET(6,resultsCategories)>0 and lm.ResultsTotalFirstScore>0) 
	    										   or (FIND_IN_SET(7,resultsCategories)>0 and lm.ResultsTotalSecondScore>0)
	    										   or (FIND_IN_SET(8,resultsCategories)>0 and lm.ResultsTotalThirdScore>0)
	    										   or (FIND_IN_SET(9,resultsCategories)>0 and lm.ResultsTotalFourthScore>0)))
    			
    	and (if(totalResultsPointsFrom is null, 1, lm.TotalResultsPoints >= totalResultsPointsFrom))
	    and (if(totalResultsPointsTo is null, 1, lm.TotalResultsPoints <= totalResultsPointsTo))
	    
	    and (if(totalPointsFrom is null, 1, lm.TotalPoints >= totalPointsFrom))
	    and (if(totalPointsTo is null, 1, lm.TotalPoints <= totalPointsTo))
	    
	    and (if(exportVsPointsPercentageFrom is null, 1, CONVERT(TRIM(TRAILING '%' FROM lm.ExportVsPointsPercentage), SIGNED) >= exportVsPointsPercentageFrom))
	    and (if(exportVsPointsPercentageTo is null, 1, CONVERT(TRIM(TRAILING '%' FROM lm.ExportVsPointsPercentage), SIGNED) <= exportVsPointsPercentageTo))
	    
	    and (if(exportVsPointsExceptions is null, 1, FIND_IN_SET(lm.ExportVsPointsPercentage, exportVsPointsExceptions)>0))
	    										   
	    and (if(exportVsPointsNumberFrom is null, 1, lm.ExportVsPointsNumber >= exportVsPointsNumberFrom))
	    and (if(exportVsPointsNumberTo is null, 1, lm.ExportVsPointsNumber <= exportVsPointsNumberTo))
	    
	    ORDER BY
    CASE WHEN sortDirection = 'asc' THEN
        CASE 
           WHEN sortBy = 'DateFirstAdded' THEN DateFirstAdded
           WHEN sortBy = 'TotalOccurancePoints' THEN TotalOccurancePoints
           WHEN sortBy = 'TotalResultsPoints' THEN TotalResultsPoints 
           WHEN sortBy = 'TotalPoints' THEN TotalPoints
           ELSE ID 
        END
    END ASC
    , CASE WHEN sortDirection = 'desc' THEN
        CASE 
           WHEN sortBy = 'DateFirstAdded' THEN DateFirstAdded
           WHEN sortBy = 'TotalOccurancePoints' THEN TotalOccurancePoints
           WHEN sortBy = 'TotalResultsPoints' THEN TotalResultsPoints 
           WHEN sortBy = 'TotalPoints' THEN TotalPoints
           ELSE ID 
        END
    END DESC				
	   LIMIT  exportOffset, exportLimit
   ;
END$$
DELIMITER

-- 

DROP PROCEDURE IF EXISTS `SP_Filter_LeadManagementReport_CountTotal`;
DELIMITER $$
CREATE PROCEDURE `SP_Filter_LeadManagementReport_CountTotal`( 

IN dateFirstAddedFrom DATETIME,
IN dateFirstAddedTo DATETIME,

IN totalTimesExportedFrom INT,
IN totalTimesExportedTo INT,

IN dateLastExportedFrom DATETIME,
IN dateLastExportedTo DATETIME,

IN last3CampaignsUsed varchar(200),

IN dateLastOccurredFrom DATETIME,
IN dateLastOccurredTo DATETIME,

IN occurredCategories varchar(200),

IN totalOccurancePointsFrom INT,
IN totalOccurancePointsTo INT,

IN resultsCategories varchar(200),

IN totalResultsPointsFrom INT,
IN totalResultsPointsTo INT,

IN totalPointsFrom INT,
IN totalPointsTo INT,

IN exportVsPointsPercentageFrom INT,
IN exportVsPointsPercentageTo INT,

IN exportVsPointsExceptions varchar(200),

IN exportVsPointsNumberFrom INT,
IN exportVsPointsNumberTo INT,

IN sortBy varchar(200),
IN sortDirection varchar(200)
) 
BEGIN
    select count(1) 
    from LeadManagementReport lm
    where 
	    	        
	        (if(dateFirstAddedFrom is null, 1, lm.DateFirstAdded >= dateFirstAddedFrom))
	    and (if(dateFirstAddedTo is null, 1, lm.DateFirstAdded <= dateFirstAddedTo))
	    
	    and (if(totalTimesExportedFrom is null, 1, lm.TotalTimesExported >= totalTimesExportedFrom)) 
	    and (if(totalTimesExportedTo is null, 1, lm.TotalTimesExported <= totalTimesExportedTo))
	    
	    and (if(dateLastExportedFrom is null, 1, lm.DateLastExported >= dateLastExportedFrom))
	    and (if(dateLastExportedTo is null, 1, lm.DateLastExported <= dateLastExportedTo))
	    
	    and (if(last3CampaignsUsed is null, 1,  FIND_IN_SET(lm.LastUsedCampaignId, last3CampaignsUsed)>0 
	    										 or FIND_IN_SET(lm.SecondLastUsedCampaignId, last3CampaignsUsed)>0  
	     										 or FIND_IN_SET(lm.ThirdLastUsedCampaignId, last3CampaignsUsed)>0)) 
	   		
    	and (if(dateLastOccurredFrom is null, 1, lm.DateLastOccurred >= dateLastOccurredFrom))
	    and (if(dateLastOccurredTo is null, 1, lm.DateLastOccurred <= dateLastOccurredTo))
	    
	    and (if(occurredCategories is null, 1, (FIND_IN_SET(1,occurredCategories)>0 and lm.OccuranceTotalFirstScore>0) 
											   or (FIND_IN_SET(2,occurredCategories)>0 and lm.OccuranceTotalSecondScore>0)
										       or (FIND_IN_SET(3,occurredCategories)>0 and lm.OccuranceTotalThirdScore>0)
											   or (FIND_IN_SET(4,occurredCategories)>0 and lm.OccuranceTotalFourthScore>0)
											   or (FIND_IN_SET(5,occurredCategories)>0 and lm.OccuranceTotalFifthScore>0)))
											   
    	and (if(totalOccurancePointsFrom is null, 1, lm.TotalOccurancePoints >= totalOccurancePointsFrom))
	    and (if(totalOccurancePointsTo is null, 1, lm.TotalOccurancePoints <= totalOccurancePointsTo))
	    
	    and (if(resultsCategories is null, 1, (FIND_IN_SET(6,resultsCategories)>0 and lm.ResultsTotalFirstScore>0) 
	    										   or (FIND_IN_SET(7,resultsCategories)>0 and lm.ResultsTotalSecondScore>0)
	    										   or (FIND_IN_SET(8,resultsCategories)>0 and lm.ResultsTotalThirdScore>0)
	    										   or (FIND_IN_SET(9,resultsCategories)>0 and lm.ResultsTotalFourthScore>0)))
    			
    	and (if(totalResultsPointsFrom is null, 1, lm.TotalResultsPoints >= totalResultsPointsFrom))
	    and (if(totalResultsPointsTo is null, 1, lm.TotalResultsPoints <= totalResultsPointsTo))
	    
	    and (if(totalPointsFrom is null, 1, lm.TotalPoints >= totalPointsFrom))
	    and (if(totalPointsTo is null, 1, lm.TotalPoints <= totalPointsTo))
	    
	    and (if(exportVsPointsPercentageFrom is null, 1, CONVERT(TRIM(TRAILING '%' FROM lm.ExportVsPointsPercentage), SIGNED) >= exportVsPointsPercentageFrom))
	    and (if(exportVsPointsPercentageTo is null, 1, CONVERT(TRIM(TRAILING '%' FROM lm.ExportVsPointsPercentage), SIGNED) <= exportVsPointsPercentageTo))
	    
	    and (if(exportVsPointsExceptions is null, 1, FIND_IN_SET(lm.ExportVsPointsPercentage, exportVsPointsExceptions)>0))
	    										   
	    and (if(exportVsPointsNumberFrom is null, 1, lm.ExportVsPointsNumber >= exportVsPointsNumberFrom))
	    and (if(exportVsPointsNumberTo is null, 1, lm.ExportVsPointsNumber <= exportVsPointsNumberTo))
	    
	    ORDER BY
    CASE WHEN sortDirection = 'asc' THEN
        CASE 
           WHEN sortBy = 'DateFirstAdded' THEN DateFirstAdded
           WHEN sortBy = 'TotalOccurancePoints' THEN TotalOccurancePoints
           WHEN sortBy = 'TotalResultsPoints' THEN TotalResultsPoints 
           WHEN sortBy = 'TotalPoints' THEN TotalPoints
           ELSE ID 
        END
    END ASC
    , CASE WHEN sortDirection = 'desc' THEN
        CASE 
           WHEN sortBy = 'DateFirstAdded' THEN DateFirstAdded
           WHEN sortBy = 'TotalOccurancePoints' THEN TotalOccurancePoints
           WHEN sortBy = 'TotalResultsPoints' THEN TotalResultsPoints 
           WHEN sortBy = 'TotalPoints' THEN TotalPoints
           ELSE ID 
        END
    END DESC
   ;
END$$
DELIMITER

DROP PROCEDURE IF EXISTS `SP_Filter_ImportDataHistory`;
DELIMITER $$
CREATE PROCEDURE `SP_Filter_ImportDataHistory`(
IN source varchar(50),
IN importTimeFrom DATETIME,
IN importTimeTo DATETIME,
IN sortBy varchar(200),
IN sortDirection varchar(200),
IN exportLimit INT,
in exportOffset INT
) 
BEGIN
    select i.* 
    from ImportDataHistory i
    where 
	    	        
	        (if(importTimeFrom is null, 1, ImportTime >= importTimeFrom))
	    and (if(importTimeTo is null, 1, ImportTime <= importTimeTo))
	    
	    and (if(source is null, 1, i.Source LIKE CONCAT('%', source, '%')))
	    
	    ORDER BY
    CASE WHEN sortDirection = N'asc' THEN
        CASE 
	       WHEN sortBy = 'ID' THEN ID
           WHEN sortBy = 'ImportName' THEN ImportName
           WHEN sortBy = 'FileName' THEN FileName
           WHEN sortBy = 'ImportTime' THEN ImportTime
           WHEN sortBy = 'Source' THEN i.Source
           WHEN sortBy = 'TotalRows' THEN i.TotalRows
           ELSE ID 
        END
    END ASC
    , CASE WHEN sortDirection = N'desc' THEN
        CASE 
           WHEN sortBy = 'ID' THEN ID
           WHEN sortBy = 'ImportName' THEN ImportName
           WHEN sortBy = 'FileName' THEN FileName
           WHEN sortBy = 'ImportTime' THEN ImportTime
           WHEN sortBy = 'Source' THEN i.Source
           WHEN sortBy = 'TotalRows' THEN i.TotalRows
           ELSE ID 
        END
    END desc
    
	   LIMIT  exportOffset, exportLimit
   ;
END$$
DELIMITER


DROP PROCEDURE IF EXISTS `SP_Filter_ImportDataHistory_CountTotal`;
DELIMITER $$
CREATE PROCEDURE `SP_Filter_ImportDataHistory_CountTotal`(
IN source varchar(50),
IN importTimeFrom DATETIME,
IN importTimeTo DATETIME
) 
BEGIN
	select count(1) 
    from ImportDataHistory i
    where 
	    	        
	        (if(importTimeFrom is null, 1, ImportTime >= importTimeFrom))
	    and (if(importTimeTo is null, 1, ImportTime <= importTimeTo))
	    
	    and (if(source is null, 1, i.Source LIKE CONCAT('%', source, '%')));
END$$
DELIMITER

DROP PROCEDURE IF EXISTS `SP_Filter_CleanDataHistory`;
DELIMITER $$
CREATE PROCEDURE `SP_Filter_CleanDataHistory`(
IN source varchar(100),
IN cleanTimeFrom DATETIME,
IN cleanTimeTo DATETIME,
IN sortBy varchar(200),
IN sortDirection varchar(200),
IN exportLimit INT,
in exportOffset INT
) 
BEGIN
    select i.* 
    from CleanDataHistory i
    where 
	    	        
	        (if(cleanTimeFrom is null, 1, CleanTime >= cleanTimeFrom))
	    and (if(cleanTimeTo is null, 1, CleanTime <= cleanTimeTo))
	    
	    and (if(source is null, 1, i.Source LIKE CONCAT('%', source, '%')))
	    
	    ORDER BY
    CASE WHEN sortDirection = N'asc' THEN
        CASE 
	       WHEN sortBy = 'ID' THEN ID
           WHEN sortBy = 'FileName' THEN FileName           
           WHEN sortBy = 'CleanTime' THEN CleanTime
           WHEN sortBy = 'Source' THEN i.Source
           WHEN sortBy = 'TotalRows' THEN i.TotalRows
           ELSE ID 
        END
    END ASC
    , CASE WHEN sortDirection = N'desc' THEN
        CASE 
           WHEN sortBy = 'ID' THEN ID
           WHEN sortBy = 'FileName' THEN FileName           
           WHEN sortBy = 'CleanTime' THEN CleanTime
           WHEN sortBy = 'Source' THEN i.Source
           WHEN sortBy = 'TotalRows' THEN i.TotalRows
           ELSE ID 
        END
    END desc
    
	   LIMIT  exportOffset, exportLimit
   ;
END$$
DELIMITER


DROP PROCEDURE IF EXISTS `SP_Filter_CleanDataHistory_CountTotal`;
DELIMITER $$
CREATE PROCEDURE `SP_Filter_CleanDataHistory_CountTotal`(
IN source varchar(100),
IN cleanTimeFrom DATETIME,
IN cleanTimeTo DATETIME
) 
BEGIN
	select count(1) 
    from CleanDataHistory i
    where 
	    	        
	        (if(cleanTimeFrom is null, 1, CleanTime >= cleanTimeFrom))
	    and (if(cleanTimeTo is null, 1, CleanTime <= cleanTimeTo))
	    
	    and (if(source is null, 1, i.Source LIKE CONCAT('%', source, '%')));
END$$
DELIMITER