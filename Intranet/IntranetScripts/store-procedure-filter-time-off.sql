DROP PROCEDURE IF EXISTS `SP_Filter_Time_Off`;
DELIMITER $$
CREATE PROCEDURE `SP_Filter_Time_Off`(
in currentUserId INT,
in fromTime DATETIME,
in toTime DATETIME,
IN sortBy varchar(200),
IN sortDirection varchar(200),
in exportLimit INT,
in exportOffset INT) 
begin
    declare deptId, rankLevel, isAllBrandCheck, totalCount INT default 0;
   	declare brandIds, deptIds, employeeIds varchar(200) default '';
   
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

	select  group_concat(rd.DepartmentId)
	into 
		deptIds
	from roles r  
	inner join userroles ur 
	on ur.RoleId = r.Id
	inner join roledepartments rd
	on rd.RoleId = r.Id 
	where ur.UserId = currentUserId
	group by ur.UserId;

	-- set deptIds = concat(deptIds,',',  deptId);
	 
	
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
			(FIND_IN_SET(u.DeptId  , deptIds)>0)
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
			(FIND_IN_SET(u.DeptId  , deptIds)>0)
		and r.`Level` <= rankLevel 
		and (isAllBrandCheck = 1 or FIND_IN_SET(u.Id, employeeIds)>0)
		and (if(fromTime is null, 1, s.CreationTime  >= fromTime))
		and (if(toTime is null, 1, s.CreationTime  <= toTime))
		and s.IsDeleted = 0
		
	ORDER BY
		CASE WHEN sortDirection = 'ASC' THEN
		    CASE 
		       WHEN sortBy = 'DeptId' THEN u.DeptId
		       WHEN sortBy = 'Name' THEN u.Name
		       WHEN sortBy = 'EmployeeCode' THEN u.EmployeeCode 
		       WHEN sortBy = 'RecordType' THEN s.RecordType
		       WHEN sortBy = 'StartDate' THEN s.StartDate
		       WHEN sortBy = 'EndDate' THEN s.EndDate
		       WHEN sortBy = 'CreationTime' THEN s.CreationTime
		       WHEN sortBy = 'CreatorUserId' THEN s.CreatorUserId
		       ELSE s.Id 
		    END
		END ASC
		, CASE WHEN sortDirection = 'DESC' THEN
		    CASE 
		       WHEN sortBy = 'DeptId' THEN u.DeptId
		       WHEN sortBy = 'Name' THEN u.Name
		       WHEN sortBy = 'EmployeeCode' THEN u.EmployeeCode 
		       WHEN sortBy = 'RecordType' THEN s.RecordType
		       WHEN sortBy = 'StartDate' THEN s.StartDate
		       WHEN sortBy = 'EndDate' THEN s.EndDate
		       WHEN sortBy = 'CreationTime' THEN s.CreationTime
		       WHEN sortBy = 'CreatorUserId' THEN s.CreatorUserId
		       ELSE s.Id 
		    END
		END DESC
   	LIMIT exportOffset, exportLimit;
END$$
DELIMITER

-- 
call SP_Filter_Time_Off(7,null,null, 20,0);
select RankId from users where EmployeeCode ='HoanLeTest08'
select RankId from users where Id=7
-- user id 7 rank 12
select * from ranks where Id= 12
-- level  id 12 s1 110

select u.DeptId ,u.RankId,  r.`Level` , group_concat(b.BrandId)	
from users u 
left join ranks r 
on u.RankId = r.Id  
left join brandemployees b 
on b.EmployeeId = u.Id 
where u.Id = 7
group by u.Id;

-- DeptId	RankId	`Level`	`group_concat(b.BrandId)`
-- 3		12		110		3,4,10,13

select  group_concat(rd.DepartmentId)	
from roles r  
inner join userroles ur 
on ur.RoleId = r.Id
inner join roledepartments rd
on rd.RoleId = r.Id 
where ur.UserId = 7
group by ur.UserId;
-- 2,3,5

select s.Id, s.EmployeeId,  u.Name as 'EmployeeName',u.EmployeeCode  as 'EmployeeCode', 
		s.StartDate , s.EndDate , s.CreationTime,
		s.RecordType ,s.RecordDetailType, u.DeptId as 'DepartmentId', u.RankId ,
		r.`Level`, u.CreatorUserId as 'CreatorUserId'
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


