DROP PROCEDURE IF EXISTS `SP_Get_Employees_By_Current_User`;
DELIMITER $$
CREATE PROCEDURE `SP_Get_Employees_By_Current_User`(
in currentUserId INT
) 
begin
    declare deptId,  isAllBrandCheck int default 0;
   	declare brandIds, deptIds, employeeIds varchar(200) default '';
   
	select u.DeptId , group_concat(b.BrandId)
	into 
		deptId, brandIds
	from users u  
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

	set deptIds = concat(deptIds,',',  deptId);
	 
	 
	
	select  exists 
		(select * from brands b  where IsAllBrand = 1 and Id in (brandIds))
	into isAllBrandCheck;
	
	if (isAllBrandCheck = 0) then
		select group_concat(distinct (EmployeeId)) 
		into employeeIds 
		from brandemployees where FIND_IN_SET(BrandId, brandIds)>0;
	end if;
	 
	select u.Id, u.EmployeeCode , u.Name , u.DeptId as 'DepartmentId'
	from Users u 
	inner join brandemployees be
	on be.EmployeeId = u.Id		 
	where 
		    (isAllBrandCheck = 1 or FIND_IN_SET(u.Id  , employeeIds)>0)
		and FIND_IN_SET(u.DeptId  , deptIds)>0
		and u.IsDeleted = 0
	-- group by u.Id, u.EmployeeCode , u.Name , u.DeptId as 'DepartmentId'
	;
 
END$$
DELIMITER


call SP_Get_Employees_By_Current_User(8);


	select  group_concat(rd.DepartmentId)	
	from roles r  
	inner join userroles ur 
	on ur.RoleId = r.Id
	inner join roledepartments rd
	on rd.RoleId = r.Id 
	where ur.UserId = 7
	group by ur.UserId;
	

select * from userroles u where UserId =7
-- role 4
select * from roledepartments r where RoleId =4
-- dept 2 4