DROP PROCEDURE IF EXISTS `SP_Filter_Leave_History`;
DELIMITER $$
CREATE PROCEDURE `SP_Filter_Leave_History`(
in currentUserId int,
in inputBrandId int,
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
		(select * from brands where IsAllBrand = 1 and (if(inputBrandId is null, FIND_IN_SET(Id, brandIds)>0, Id = inputBrandId)))		
	into isAllBrandCheck;	
	
	if (isAllBrandCheck = 0) then 
		select group_concat(distinct (EmployeeId)) 
		into employeeIds 
		from brandemployees where if(inputBrandId is null, FIND_IN_SET(BrandId, brandIds)>0, BrandId = inputBrandId);
	end if;
	
	select 
		s.EmployeeId,  u.Name as 'EmployeeName', u.EmployeeCode  as 'EmployeeCode', u.DeptId as 'DepartmentId', u.RankId , u.Country ,
		s.RecordType, s.RecordDetailType, s.LateAmount, s.StartDate, s.EndDate,
		s.NumberOfDays, s.NumberOfHours, s.Fine , s.CalculationAmount 
	from staffrecords s
	inner join Users u  
	on s.EmployeeId = u.Id	
	left join ranks r 
	on u.RankId = r.Id
	where 
			(FIND_IN_SET(u.DeptId  , deptIds)>0)
		and r.`Level` <= rankLevel 
		and (isAllBrandCheck >0 or FIND_IN_SET(s.EmployeeId , employeeIds)>0)
		and (if(fromTime is null, 1, s.CreationTime  >= fromTime))
		and (if(toTime is null, 1, s.CreationTime  <= toTime))
		and s.IsDeleted = 0
		;
END$$
DELIMITER

call SP_Filter_Leave_History(7,9,null,null);
SET isAllBrand = 0;
select exists  (select * from brands where IsAllBrand = 1 and Id=9) into isAllBrand;
select isAllBrand;
select * from brandemployees b where EmployeeId = 7
select * from brands where IsAllBrand = 1 and Id= 9;
delete from brandemployees where EmployeeId=8 and BrandId =2
select group_concat(distinct (EmployeeId)) from brandemployees where BrandId in (2,3,4,9)
select BrandId  from brandemployees where EmployeeId =7
-- 2,3,4,9
-- 1,3,4,5,6,7,8,9
select FIND_IN_SET(1, '10,11,12,1')

select  1 in ('10,11,12,1')
select exists  (select * from brands where IsAllBrand = 1 and Id=9)
