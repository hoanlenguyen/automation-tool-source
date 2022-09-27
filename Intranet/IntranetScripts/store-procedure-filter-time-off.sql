DROP PROCEDURE IF EXISTS `SP_Filter_Time_Off`;
DELIMITER $$
CREATE PROCEDURE `SP_Filter_Time_Off`(
in currentUserId INT,
in fromTime DATETIME,
in toTime DATETIME,
in exportLimit INT,
in exportOffset INT) 
begin
    declare deptId, rankLevel, isAllBrandCheck, totalCount INT default 0;
   	declare brandIds, employeeIds varchar(200) default '';
   
	select u.DeptId , r.`Level` , group_concat(b.BrandId)
	into 
		deptId,		rankLevel,		brandIds
	from users u 
	left join ranks r 
	on u.RankId = r.Id  
	left join brandemployees b 
	on b.EmployeeId = u.Id 
	where u.Id = currentUserId
	group by u.Id;
	 
	
	select  exists 
		(select * from brands b  where IsAllBrand = 1 and Id in (brandIds))
	into isAllBrandCheck;

	if (isAllBrandCheck = 0) then 
		select group_concat(distinct (EmployeeId)) 
		into employeeIds 
		from brandemployees where FIND_IN_SET(BrandId, brandIds)>0;
	end if;
	
	select count(1)
	into totalCount
	from staffrecords s
	inner join Users u  
	on s.EmployeeId = u.Id 
	left join ranks r 
	on u.RankId = r.Id
	where 
		u.DeptId  = deptId 
		and r.`Level` <= rankLevel 
		and (isAllBrandCheck = 1 or FIND_IN_SET(u.Id, employeeIds)>0)
		and (if(fromTime is null, 1, s.CreationTime  >= fromTime))
		and (if(toTime is null, 1, s.CreationTime  <= toTime))
		and s.IsDeleted =0;
	
	
	select s.Id, s.EmployeeId,  u.Name as 'EmployeeName',u.EmployeeCode  as 'EmployeeCode', 
		s.StartDate , s.EndDate , s.CreationTime,
		s.RecordType ,s.RecordDetailType, u.DeptId as 'DepartmentId', u.RankId ,
		r.`Level`, u.CreatorUserId as 'CreatorUserId', totalCount
	from staffrecords s
	inner join Users u  
	on s.EmployeeId = u.Id
	left join ranks r 
	on u.RankId = r.Id
	where 
		u.DeptId  = deptId 
		and r.`Level` <= rankLevel 
		and (isAllBrandCheck = 1 or FIND_IN_SET(u.Id, employeeIds)>0)
		and (if(fromTime is null, 1, s.CreationTime  >= fromTime))
		and (if(toTime is null, 1, s.CreationTime  <= toTime))
		and s.IsDeleted = 0
   	LIMIT exportOffset, exportLimit;
END$$
DELIMITER

-- 
call SP_Filter_Time_Off(7,null,null, 20,0);




