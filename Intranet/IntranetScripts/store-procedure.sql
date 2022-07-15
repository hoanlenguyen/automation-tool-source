DROP PROCEDURE IF EXISTS `SP_Filter_Bank`;
DELIMITER $$
CREATE PROCEDURE `SP_Filter_Bank`(
IN keyword varchar(100),
IN status tinyint,
IN sortBy varchar(100),
IN sortDirection varchar(100),
IN exportLimit INT,
in exportOffset INT
) 
BEGIN
    select i.* 
    from Bank i
    where 
	    	        
	        (if(status is null, 1, i.Status = status))	  
	    and (if(keyword is null, 1, i.Name LIKE CONCAT('%', keyword, '%')))
	    and IsDeleted = 0
	    
	    ORDER BY
    CASE WHEN sortDirection = N'asc' THEN
        CASE 
	       WHEN sortBy = 'Id' THEN Id
           WHEN sortBy = 'Name' THEN Name           
           WHEN sortBy = 'Status' THEN Status           
           ELSE Id 
        END
    END ASC
    , CASE WHEN sortDirection = N'desc' THEN
        CASE 
           WHEN sortBy = 'Id' THEN Id
           WHEN sortBy = 'Name' THEN Name           
           WHEN sortBy = 'Status' THEN Status           
           ELSE Id
        END
    END desc
    
	   LIMIT  exportOffset, exportLimit
   ;
END$$
DELIMITER


DROP PROCEDURE IF EXISTS `SP_Filter_Bank_CountTotal`;
DELIMITER $$
CREATE PROCEDURE `SP_Filter_Bank_CountTotal`(
IN keyword varchar(100),
IN status tinyint
) 
BEGIN
	select count(1) 
    from Bank i
    where 
	     (if(status is null, 1, i.Status = status))	  
	    and (if(keyword is null, 1, i.Name LIKE CONCAT('%', keyword, '%')))
	    and IsDeleted = 0
	    ;
END$$
DELIMITER

DROP PROCEDURE IF EXISTS `SP_Filter_Brand`;
DELIMITER $$
CREATE PROCEDURE `SP_Filter_Brand`(
IN keyword varchar(100),
IN status tinyint,
IN sortBy varchar(100),
IN sortDirection varchar(100),
IN exportLimit INT,
in exportOffset INT
) 
BEGIN
    select i.* 
    from Brand i
    where 
	    	        
	        (if(status is null, 1, i.Status = status))	  
	    and (if(keyword is null, 1, i.Name LIKE CONCAT('%', keyword, '%')))
	    and IsDeleted = 0
	    
	    ORDER BY
    CASE WHEN sortDirection = N'asc' THEN
        CASE 
	       WHEN sortBy = 'Id' THEN Id
           WHEN sortBy = 'Name' THEN Name           
           WHEN sortBy = 'Status' THEN Status           
           ELSE Id 
        END
    END ASC
    , CASE WHEN sortDirection = N'desc' THEN
        CASE 
           WHEN sortBy = 'Id' THEN Id
           WHEN sortBy = 'Name' THEN Name           
           WHEN sortBy = 'Status' THEN Status           
           ELSE Id
        END
    END desc
    
	   LIMIT  exportOffset, exportLimit
   ;
END$$
DELIMITER


DROP PROCEDURE IF EXISTS `SP_Filter_Brand_CountTotal`;
DELIMITER $$
CREATE PROCEDURE `SP_Filter_Brand_CountTotal`(
IN keyword varchar(100),
IN status tinyint
) 
BEGIN
	select count(1) 
    from Brand i
    where 
	     (if(status is null, 1, i.Status = status))	  
	    and (if(keyword is null, 1, i.Name LIKE CONCAT('%', keyword, '%')))
	    and IsDeleted = 0
	    ;
END$$
DELIMITER

DROP PROCEDURE IF EXISTS `SP_Filter_Role`;
DELIMITER $$
CREATE PROCEDURE `SP_Filter_Role`(
IN keyword varchar(100),
IN status tinyint,
IN sortBy varchar(100),
IN sortDirection varchar(100),
IN exportLimit INT,
in exportOffset INT
) 
BEGIN
    select i.* 
    from UserRole i
    where 
	    	        
	        (if(status is null, 1, i.Status = status))	  
	    and (if(keyword is null, 1, i.Name LIKE CONCAT('%', keyword, '%')))
	    and IsDeleted = 0
	    
	    ORDER BY
    CASE WHEN sortDirection = N'asc' THEN
        CASE 
	       WHEN sortBy = 'Id' THEN Id
           WHEN sortBy = 'Name' THEN Name           
           WHEN sortBy = 'Status' THEN Status           
           ELSE Id 
        END
    END ASC
    , CASE WHEN sortDirection = N'desc' THEN
        CASE 
           WHEN sortBy = 'Id' THEN Id
           WHEN sortBy = 'Name' THEN Name           
           WHEN sortBy = 'Status' THEN Status           
           ELSE Id
        END
    END desc
    
	   LIMIT  exportOffset, exportLimit
   ;
END$$
DELIMITER

DROP PROCEDURE IF EXISTS `SP_Filter_Role_CountTotal`;
DELIMITER $$
CREATE PROCEDURE `SP_Filter_Role_CountTotal`(
IN keyword varchar(100),
IN status tinyint
) 
BEGIN
	select count(1) 
    from UserRole i
    where 
	     (if(status is null, 1, i.Status = status))	  
	    and (if(keyword is null, 1, i.Name LIKE CONCAT('%', keyword, '%')))
	    and IsDeleted = 0
	    ;
END$$
DELIMITER

DROP PROCEDURE IF EXISTS `SP_Filter_Department`;
DELIMITER $$
CREATE PROCEDURE `SP_Filter_Department`(
IN keyword varchar(100),
IN status tinyint,
IN sortBy varchar(100),
IN sortDirection varchar(100),
IN exportLimit INT,
in exportOffset INT
) 
BEGIN
    select i.* 
    from Department i
    where 
	    	        
	        (if(status is null, 1, i.Status = status))	  
	    and (if(keyword is null, 1, i.Name LIKE CONCAT('%', keyword, '%')))
	    and IsDeleted = 0
	    
	    ORDER BY
    CASE WHEN sortDirection = N'asc' THEN
        CASE 
	       WHEN sortBy = 'Id' THEN Id
           WHEN sortBy = 'Name' THEN Name           
           WHEN sortBy = 'Status' THEN Status           
           ELSE Id 
        END
    END ASC
    , CASE WHEN sortDirection = N'desc' THEN
        CASE 
           WHEN sortBy = 'Id' THEN Id
           WHEN sortBy = 'Name' THEN Name           
           WHEN sortBy = 'Status' THEN Status           
           ELSE Id
        END
    END desc
    
	   LIMIT  exportOffset, exportLimit
   ;
END$$
DELIMITER

DROP PROCEDURE IF EXISTS `SP_Filter_Department_CountTotal`;
DELIMITER $$
CREATE PROCEDURE `SP_Filter_Department_CountTotal`(
IN keyword varchar(100),
IN status tinyint
) 
BEGIN
	select count(1) 
    from Department i
    where 
	     (if(status is null, 1, i.Status = status))	  
	    and (if(keyword is null, 1, i.Name LIKE CONCAT('%', keyword, '%')))
	    and IsDeleted = 0
	    ;
END$$
DELIMITER

DROP PROCEDURE IF EXISTS `SP_Filter_Employee`;
DELIMITER $$
CREATE PROCEDURE `SP_Filter_Employee`(
IN keyword varchar(100),
IN status tinyint,
IN sortBy varchar(100),
IN sortDirection varchar(100),
IN exportLimit INT,
in exportOffset INT
) 
BEGIN
    select i.* 
    from Employee i
    where 
	    	        
	        (if(status is null, 1, i.Status = status))	  
	    and (if(keyword is null, 1, i.Name LIKE CONCAT('%', keyword, '%')))
	    and IsDeleted = 0
	    
	    ORDER BY
    CASE WHEN sortDirection = N'asc' THEN
        CASE 
	       WHEN sortBy = 'Id' THEN Id
           WHEN sortBy = 'Name' THEN Name           
           WHEN sortBy = 'Status' THEN Status           
           ELSE Id 
        END
    END ASC
    , CASE WHEN sortDirection = N'desc' THEN
        CASE 
           WHEN sortBy = 'Id' THEN Id
           WHEN sortBy = 'Name' THEN Name           
           WHEN sortBy = 'Status' THEN Status           
           ELSE Id
        END
    END desc
    
	   LIMIT  exportOffset, exportLimit
   ;
END$$
DELIMITER

DROP PROCEDURE IF EXISTS `SP_Filter_Employee_CountTotal`;
DELIMITER $$
CREATE PROCEDURE `SP_Filter_Employee_CountTotal`(
IN keyword varchar(100),
IN status tinyint
) 
BEGIN
	select count(1) 
    from Employee i
    where 
	     (if(status is null, 1, i.Status = status))	  
	    and (if(keyword is null, 1, i.Name LIKE CONCAT('%', keyword, '%')))
	    and IsDeleted = 0
	    ;
END$$
DELIMITER