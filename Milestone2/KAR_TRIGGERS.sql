--Update “numCheckins” to checkins on that business
CREATE OR REPLACE FUNCTION updatecheckin() RETURNS trigger AS '
BEGIN 
	UPDATE Business SET numCheckins = SQ.total 
	FROM (SELECT businessId, SUM (checkInCount) AS total FROM Checkin GROUP BY businessId) AS SQ 
	WHERE SQ.businessId = id;
	RETURN NEW;
END
' LANGUAGE plpgsql;  

CREATE TRIGGER checkins
AFTER UPDATE OR INSERT ON Checkin
FOR EACH STATEMENT
EXECUTE PROCEDURE updatecheckin();
 
DROP TRIGGER checkins on Checkin;

--Update "reviewcount" to reviews on that business
CREATE OR REPLACE FUNCTION updatereviews() RETURNS trigger AS '
BEGIN 
	UPDATE Business SET reviewCount = SQ.total, reviewRating = SQ.average 
	FROM (SELECT businessId, COUNT (*) AS total, AVG(stars) AS average FROM Review GROUP BY businessId) AS SQ 
	WHERE SQ.businessId = id;
	RETURN NEW;
END
' LANGUAGE plpgsql;  

CREATE TRIGGER reviewadd
AFTER INSERT ON review
FOR EACH STATEMENT
EXECUTE PROCEDURE updatereviews();

DROP TRIGGER reviewadd on reviews;

--test statements
update checkin set checkincount = 75 where businessId = 'dwQEZBFen2GdihLLfWeexA' and dayoftheweek = 'friday';
select * from business where id = 'dwQEZBFen2GdihLLfWeexA';
insert into review values ('unEY79t6hHECP9Yd58R1dg', 'dwQEZBFen2GdihLLfWeexA', 'xG6qtg9y3Hq_zw5g', '2014-06-09', '3', 1, 0, 1, 'blablabla');
