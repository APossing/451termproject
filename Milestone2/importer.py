import json
import psycopg2

conn = psycopg2.connect("dbname=project user=admin123@451termproject host=451termproject.postgres.database.azure.com password=Admin$123 port=5432")
cur = conn.cursor()


def executeString(str):
    try:
        cur.execute(str)
        conn.commit()
    except psycopg2.Error as e:
        print(e.pgerror)
        conn.rollback()


def cleanStr4SQL(s):
    return s.replace("'","`").replace("\n"," ")


def recursive_generator(dict_var):
    for k, v in dict_var.items():
        yield k
        if isinstance(v, dict):
            for id_val in recursive_generator(v):
                yield id_val
        else:
            yield v




def recursive_writer(dict_var, outfile):
    for k, v in dict_var.items():
        outfile.write("(")
        outfile.write(str(k))
        outfile.write(", ")
        if isinstance(v, dict):
            recursive_writer(v, outfile)
        else:
            outfile.write(str(v))

        outfile.write(")")


def aggregate_dates(dict_var, outfile):
    for k, v in dict_var.items():
        day = k
        morning = 0
        afternoon = 0
        evening = 0
        night = 0
        for key, value in v.items():
            timestr = key.split(':')
            time = int(timestr[0])

            if (time > 22 or time < 5):
                night += value
            elif (time > 16):
                evening += value
            elif(time > 10):
                afternoon += value
            elif(time > 4):
                morning += value
        outfile.write('({}, {}, {}, {}, {})'.format(day, morning, afternoon, evening, night)+'\t')



def parseBusinessData():
    #read the JSON file
    with open('.\yelp_business.JSON','r') as f:  #Assumes that the data files are available in the current directory. If not, you should set the path for the yelp data files.
        outfile = open('business.txt', 'w')
        line = f.readline()
        count_line = 0
        insertLine = insertLineDefault = "insert into Business(id,businessName,businessAddress,st,city,zipcode,latitude,longitude,stars,reviewCount,isOpen) values "
        count = 1
        insertLineAttributes = insertLineAttributesDefault = "insert into attributes(businessId, attributeName, attributeValue) values "
        attributeCount = 1
        insertLineCategories = insertLineCategoriesDefault = "insert into BelongsToCategory(businessId, categoryName) values "
        catCount = 1
        insertLineHours = insertLineHoursDefault = "insert into Hours(businessId,dayoftheweek,timeopen,timeclose) values "
        hoursCounts = 1

        #read each JSON abject and extract data
        while line:
            count += 1
            if count % 1000 == 0:
                print(count_line)
                if insertLine != insertLineDefault:
                    count = 1
                    executeString(insertLine[:-2] + ";")
                    insertLine = insertLineDefault

            data = json.loads(line)
            insertLine += '(\'' + cleanStr4SQL(data['business_id']) + '\', '
            insertLine += '\'' + cleanStr4SQL(data['name']) + '\', '
            insertLine += '\'' + cleanStr4SQL(data['address']) + '\', '
            insertLine += '\'' + cleanStr4SQL(data['state']) + '\', '
            insertLine += '\'' + cleanStr4SQL(data['city']) + '\', '
            insertLine += '\'' + cleanStr4SQL(data['postal_code']) + '\', '
            insertLine += str(data['latitude']) + ', '
            insertLine += str(data['longitude']) + ', '
            insertLine += str(data['stars']) + ', '
            insertLine += str(data['review_count']) + ', '
            insertLine += str(bool(data['is_open'])) + '), '

            for cat in [item for item in data['categories']]:
                catCount += 1
                if catCount % 1000 == 0:
                    executeString(insertLine[:-2] + ";")
                    insertLine = insertLineDefault
                    catCount = 1
                    executeString(insertLineCategories[:-2] + ";")
                    insertLineCategories = insertLineCategoriesDefault
                insertLineCategories += "(\'" + cleanStr4SQL(data['business_id']) + '\', \'' + cleanStr4SQL(cat) + '\'), '
            for k, v in (data['attributes']).items():
                if attributeCount % 1000 == 0:
                    executeString(insertLine[:-2] + ";")
                    insertLine = insertLineDefault
                    attributeCount = 1
                    executeString(insertLineAttributes[:-2] + ";")
                    insertLineAttributes = insertLineAttributesDefault
                if isinstance(v, dict) is False:
                    attributeCount += 1
                    insertLineAttributes += "(\'" + cleanStr4SQL(data['business_id']) + '\', \'' + str(k) + '\', \'' + str(v) + '\'), '
            for k,v in (data['hours']).items():
                hoursCounts +=1
                if hoursCounts % 1000 == 0:
                    executeString(insertLine[:-2] + ";")
                    insertLine = insertLineDefault
                    hoursCounts = 1
                    executeString(insertLineHours[:-2] + ";")
                    insertLineHours = insertLineHoursDefault
                insertLineHours += "(\'" + cleanStr4SQL(data['business_id']) + '\', \'' + k + '\', \'' + v.split('-')[0] +'\', \'' + v.split('-')[1] + '\'), '

            line = f.readline()
            count_line += 1
        executeString(insertLine[:-2] + ";")
        executeString(insertLineCategories[:-2] + ";")
        executeString(insertLineAttributes[:-2] + ";")
        executeString(insertLineHours[:-2] + ";")

    print(count_line)
    outfile.close()
    f.close()

def parseUserData():
    #read the JSON file
    with open('.\yelp_user.JSON','r') as f:  #Assumes that the data files are available in the current directory. If not, you should set the path for the yelp data files.
        outfile = open('user.txt', 'w')
        line = f.readline()
        count_line = 0
        #read each JSON abject and extract data
        count = 1
        insertString = 'insert into users (id,userName,dateJoined,reviewCount,fans,average_stars,funny,useful,cool) values '
        friendsCount = 1
        insertStringFriends = 'insert into Friendship (userId, friendId) values '
        while line:

            count += 1
            if count % 1000 == 0:
                print(count_line)
                count = 1
                insertString = insertString[:-2] + ";"
                # try:
                #     cur.execute(insertString)
                #     conn.commit()
                # except psycopg2.Error as e:
                #     print(e.pgerror)
                #     conn.rollback()
                #insertString = 'insert into users (id,userName,dateJoined,reviewCount,fans,average_stars,funny,useful,cool) values '


            dataString = '('

            data = json.loads(line)
            dataString += '\'' + cleanStr4SQL(data['user_id']) + '\', '
            dataString += '\'' + cleanStr4SQL(data['name']) + '\', '
            dataString += '\'' + str(data['yelping_since']) + '\', '
            dataString += str(data['review_count']) + ', '
            dataString += str(data['fans']) + ', '
            dataString += str(data['average_stars']) + ', '
            dataString += str(data['funny']) + ', '
            dataString += str(data['useful']) + ', '
            dataString += str(data['cool']) + '), '

            friends = [item for item in data['friends']]
            for friend in friends:
                if friendsCount % 1000 == 0:
                    friendsCount = 1
                    insertStringFriends = insertStringFriends[:-2] + ";"
                    try:
                        cur.execute(insertStringFriends)
                        conn.commit()
                    except psycopg2.Error as e:
                        print(e.pgerror)
                        conn.rollback()
                    insertStringFriends = 'insert into friends (id, f_user_name) values '
                friendsCount +=1
                insertStringFriends += '(\'' + cleanStr4SQL(data['user_id']) + '\', \'' + str(friend) + '\'), '
            insertString += dataString
            line = f.readline()
            count_line += 1


        # insertString = insertString[:-2] + ";"
        # try:
        #     cur.execute(insertString)
        #     conn.commit()
        # except psycopg2.Error as e:
        #     print(e.pgerror)
        #     conn.rollback()
        insertStringFriends = insertStringFriends[:-2] + ";"
        try:
            cur.execute(insertStringFriends)
            conn.commit()
        except psycopg2.Error as e:
            print(e.pgerror)
            conn.rollback()
    print(count_line)
    outfile.close()
    f.close()

def parseCheckinData():
    #read the JSON file
    with open('.\yelp_checkin.JSON','r') as f:  #Assumes that the data files are available in the current directory. If not, you should set the path for the yelp data files.
        outfile = open('checkin.txt', 'w')
        line = f.readline()
        count_line = 0
        #read each JSON abject and extract data
        insertLine = defaultInsertLine = "insert into checkin(businessId, dayOfTheWeek, timeOfDay, checkInCount) values "
        count = 1
        while line:
            data = json.loads(line)
            for k, v in data['time'].items():
                for k1,v1 in v.items():
                    if count % 1000 == 0:
                        executeString(insertLine[:-2] + ";")
                        count = 1
                        print(count_line)
                        insertLine = defaultInsertLine
                    count += 1
                    insertLine += "(\'" + cleanStr4SQL(data['business_id']) + "\', "
                    insertLine += "\'" + str(k) + "\', "
                    insertLine += "\'" + str(k1) + "\', "
                    insertLine += str(v1) + "), "

            line = f.readline()
            count_line +=1
        executeString(insertLine[:-2] + ";")
    print(count_line)
    outfile.close()
    f.close()


def parseReviewData():
    #read the JSON file
    with open('.\yelp_review.JSON','r') as f:  #Assumes that the data files are available in the current directory. If not, you should set the path for the yelp data files.
        outfile = open('review.txt', 'w')
        line = f.readline()
        count_line = 0
        count = 0
        #read each JSON abject and extract data
        insertLine = 'insert into review (reviewId,userId,businessId,reviewText,stars,dateCreated,funny,useful,cool) values '
        while line:
            count += 1
            if count % 1000 == 0:
                print(count_line)
                count = 1
                insertLine = insertLine[:-2] + ";"
                try:
                    cur.execute(insertLine)
                    conn.commit()
                except psycopg2.Error as e:
                    print(e.pgerror)
                    conn.rollback()
                insertLine = 'insert into review (reviewId,userId,businessId,reviewText,stars,dateCreated,funny,useful,cool) values '

            data = json.loads(line)
            insertLine += '(\'' + cleanStr4SQL(data['review_id']) + '\', '
            insertLine += '\'' + cleanStr4SQL(data['user_id']) + '\', '
            insertLine += '\'' + cleanStr4SQL(data['business_id']) + '\', '
            insertLine += '\'' + cleanStr4SQL(data['text']) + '\', '
            insertLine += str(data['stars']) + ', '
            insertLine += '\'' + str(data['date']) + '\', '
            insertLine += str(data['funny']) + ', '
            insertLine += str(data['useful']) + ', '
            insertLine += str(data['cool']) + '), '

            line = f.readline()
            count_line += 1
    print(count_line)
    outfile.close()
    f.close()


if __name__ == '__main__':
    parseBusinessData()
    parseUserData()
    parseCheckinData()
    parseReviewData()

