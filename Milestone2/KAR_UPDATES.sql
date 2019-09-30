--Update “numCheckins” to checkins on that business
UPDATE Business SET numCheckins = SQ.total 
FROM (SELECT businessId, SUM (checkInCount) AS total FROM Checkin GROUP BY businessId) AS SQ 
WHERE SQ.businessId = id;

--Update "reviewcount" to reviews on that business
UPDATE Business SET reviewCount = SQ.total, reviewRating = SQ.average 
FROM (SELECT businessId, COUNT (*) AS total, AVG(stars) AS average FROM Review GROUP BY businessId) AS SQ 
WHERE SQ.businessId = id;