CREATE TABLE Users
(
    id                  VARCHAR,
    userName            VARCHAR(50),
    dateJoined          DATE,
    latitude            INTEGER,
    longitude           INTEGER,
	reviewCount			INTEGER,
	fans				INTEGER,
	average_stars		INTEGER,
	funny				INTEGER,
	useful				INTEGER,
	cool				INTEGER,
    PRIMARY KEY         (id)
);

CREATE TABLE Business
(
    id                  VARCHAR,
    city                VARCHAR(50),
    businessName        VARCHAR(50),
	st					VARCHAR(50),
    zipcode             INTEGER,
	latitude			INTEGER,
	longitude			INTEGER,
    businessAddress     VARCHAR(100),
	reviewCount			INTEGER,
	numCheckins			INTEGER,
	reviewRating		FLOAT,
	isOpen				BOOLEAN,
	stars				FLOAT,
    priceRange          INTEGER CHECK (priceRange>0 AND priceRange<=4),
    PRIMARY KEY         (id)
);

CREATE TABLE Friendship
(
    userId              VARCHAR,
    friendId            VARCHAR,
    PRIMARY KEY (userId,friendId),
   	FOREIGN KEY (userId) REFERENCES Users(id),
	FOREIGN KEY (friendId) REFERENCES Users(id)
);

CREATE TABLE Favourites 
(
	userId              VARCHAR,
	businessId          VARCHAR,
	FOREIGN KEY (userId) REFERENCES Users(id),
	FOREIGN KEY (businessId) REFERENCES Business(id),
	PRIMARY KEY (userId,businessId)
);

CREATE TABLE Attributes
(
    businessId        		VARCHAR,
    attributeName        	VARCHAR,
	attributeValue			VARCHAR,
	PRIMARY KEY (businessId, attributeName),
    FOREIGN KEY (businessId) REFERENCES Business(id)
);

CREATE TABLE Category
(
    categoryName  VARCHAR,
    PRIMARY KEY (categoryName)
);

CREATE TABLE BelongsToCategory
(
    businessId        VARCHAR,
    categoryName        VARCHAR,
	PRIMARY KEY (businessId, categoryName),
    FOREIGN KEY (businessId) REFERENCES Business(id)
);
CREATE TABLE Hours
(
	businessId			VARCHAR,
	dayOfTheWeek		VARCHAR,
	timeOpen			VARCHAR,
	timeClose			VARCHAR,
	primary key (businessId, dayOfTheWeek),
	foreign key(businessId) references Business(id)
);


CREATE TABLE Review
(
    userId              VARCHAR,
    businessId          VARCHAR,
    reviewId            VARCHAR,
    dateCreated         DATE,
    stars               INTEGER CHECK (stars>0 AND stars<=5),
    funny               INTEGER,
    usefull             INTEGER,
    cool                INTEGER,
    reviewText          VARCHAR,
    PRIMARY KEY (userId,businessId,reviewId),
    FOREIGN KEY (userId) REFERENCES Users(id),
	FOREIGN KEY (businessId) REFERENCES Business(id)
);

CREATE TABLE Checkin
(
    businessId          VARCHAR,
    dayOfTheWeek        VARCHAR,
    timeOfDay           int,
    checkInCount        int,
    
    PRIMARY KEY (businessId,dayOfTheWeek,timeOfDay),
	FOREIGN KEY (businessId) REFERENCES Business(id)
);

