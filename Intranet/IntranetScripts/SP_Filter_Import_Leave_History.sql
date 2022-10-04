DROP PROCEDURE IF EXISTS `SP_Filter_Import_Leave_History`;
DELIMITER $$
CREATE PROCEDURE `SP_Filter_Import_Leave_History`(
in currentUserId int,
in inputBrandId int,
in inputDepartmentId int,
in fromTime DATETIME,
in toTime DATETIME) 
begin
    declare deptId, rankLevel, isAllBrandCheck int default 0;
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

	if (inputDepartmentId is null) then 
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
	end if;
	
	-- set deptIds = concat(deptIds,',',  deptId);
	
	select  exists 		
		(select * from brands where IsAllBrand = 1 and (if(inputBrandId is null, FIND_IN_SET(Id, brandIds)>0, Id = inputBrandId)))		
	into isAllBrandCheck;	
	
	if (isAllBrandCheck = 0) then 
		select group_concat(distinct (EmployeeId)) 
		into employeeIds 
		from brandemployees where if(inputBrandId is null, FIND_IN_SET(BrandId, brandIds)>0, BrandId = inputBrandId);
	end if;
	
	select 
		s.EmployeeId,  u.Name as 'EmployeeName', u.EmployeeCode  as 'EmployeeCode', u.DeptId as 'DepartmentId', u.RankId , u.Country ,
		s.PaidOffs as 'SumDaysOfPaidOffs', s.PaidMCs  as 'SumDaysOfPaidMCs', s.Late as 'SumHoursOfDeduction'
	from leavehistories s
	inner join Users u  
	on s.EmployeeId = u.Id	
	left join ranks r 
	on u.RankId = r.Id
	where 
			(if(inputDepartmentId is null, (FIND_IN_SET(u.DeptId  , deptIds)>0), u.DeptId = inputDepartmentId))
		and r.`Level` <= rankLevel 
		and (isAllBrandCheck >0 or FIND_IN_SET(s.EmployeeId , employeeIds)>0)
		and (if(fromTime is null, 1, s.CreationTime  >= fromTime))
		and (if(toTime is null, 1, s.CreationTime  <= toTime))
		and s.IsDeleted = 0
		;
END$$
DELIMITER