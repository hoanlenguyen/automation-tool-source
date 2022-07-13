
delete from leadmanagementreport ;
ALTER TABLE leadmanagementreport AUTO_INCREMENT=1;

select ID, CustomerMobileNo , TotalPoints  from leadmanagementreport order by ID asc limit 3;


delete from customerscore ;
ALTER TABLE customerscore AUTO_INCREMENT=1;

delete from temp_customerscore ;
ALTER TABLE temp_customerscore AUTO_INCREMENT=1;

select count(1) from temp_customerscore;

select count(1) from customerscore;

select * from temp_customerscore  limit 3;
select * from customerscore  limit 3;

INSERT IGNORE INTO CustomerScore(CustomerMobileNo, ScoreID, DateOccurred, Status)
SELECT temp_CustomerScore.CustomerMobileNo, temp_CustomerScore.ScoreID, temp_CustomerScore.DateOccurred, temp_CustomerScore.Status 
FROM temp_CustomerScore;


delete from customer  ;
ALTER TABLE customer AUTO_INCREMENT=1;
delete from customerscore  ;
ALTER TABLE customerscore AUTO_INCREMENT=1;

delete from temp_cutomer ;
ALTER TABLE temp_cutomer AUTO_INCREMENT=1;

select * from temp_cutomer limit 3;
select count(1) from temp_cutomer;


INSERT IGNORE INTO customer (DateFirstAdded, CustomerMobileNo, Status) SELECT temp_cutomer.DateFirstAdded, temp_cutomer.CustomerMobileNo, temp_cutomer.Status  FROM temp_cutomer;

select * from customer limit 3;

select count(1) from customer;
select count(1) from customerscore;

984737392
984737393
984737394


create table TempManagementReport(CustomerMobileNo VARCHAR(10) NOT NULL);

insert into TempManagementReport values ('984737392'), ('984737393'), ('984737394')

select l.* 
from leadmanagementreport  l
inner join TempManagementReport t 
on l.CustomerMobileNo = t.CustomerMobileNo


delete from leadmanagementreport ;
ALTER TABLE leadmanagementreport AUTO_INCREMENT=1;
-- select * from leadmanagementreport l 
//select * from recordcustomerexport r 



