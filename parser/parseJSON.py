import json

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
        #read each JSON abject and extract data
        while line:
            data = json.loads(line)
            outfile.write(cleanStr4SQL(data['business_id'])+'\t') #business id
            outfile.write(cleanStr4SQL(data['name'])+'\t') #name
            outfile.write(cleanStr4SQL(data['address'])+'\t') #full_address
            outfile.write(cleanStr4SQL(data['state'])+'\t') #state
            outfile.write(cleanStr4SQL(data['city'])+'\t') #city
            outfile.write(cleanStr4SQL(data['postal_code']) + '\t')  #zipcode
            outfile.write(str(data['latitude'])+'\t') #latitude
            outfile.write(str(data['longitude'])+'\t') #longitude
            outfile.write(str(data['stars'])+'\t') #stars
            outfile.write(str(data['review_count'])+'\t') #reviewcount
            outfile.write(str(data['is_open'])+'\t') #openstatus
            outfile.write(str([item for item in data['categories']])+'\t') #category list
            recursive_writer(data['attributes'], outfile) # attributes
            outfile.write('\t')
            recursive_writer(data['hours'], outfile) # hours
            outfile.write('\t')
            outfile.write('\n');

            line = f.readline()
            count_line +=1
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
        while line:
            data = json.loads(line)
            outfile.write(cleanStr4SQL(data['user_id']) + '\t')  # user id
            outfile.write(cleanStr4SQL(data['name']) + '\t')  # name
            outfile.write(str(data['yelping_since']) + '\t')  # yelping_since
            outfile.write(str(data['review_count']) + '\t')  # review_count
            outfile.write(str(data['fans']) + '\t')  # fans
            outfile.write(str(data['average_stars']) + '\t') # average_stars
            outfile.write(str(data['funny']) + '\t') # funny
            outfile.write(str(data['useful']) + '\t') # useful
            outfile.write(str(data['cool']) + '\t') # cool
            outfile.write(str([item for item in data['friends']]) + '\t') # friends list
            outfile.write('\n');

            line = f.readline()
            count_line +=1
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
        while line:
            data = json.loads(line)
            outfile.write(cleanStr4SQL(data['business_id'])+'\t') #business id
            aggregate_dates(data['time'], outfile)
            outfile.write('\n');

            line = f.readline()
            count_line +=1
    print(count_line)
    outfile.close()
    f.close()


def parseReviewData():
    #read the JSON file
    with open('.\yelp_review.JSON','r') as f:  #Assumes that the data files are available in the current directory. If not, you should set the path for the yelp data files.
        outfile = open('review.txt', 'w')
        line = f.readline()
        count_line = 0
        #read each JSON abject and extract data
        while line:
            data = json.loads(line)
            outfile.write(cleanStr4SQL(data['review_id']) + '\t')  # review_id
            outfile.write(cleanStr4SQL(data['user_id']) + '\t')  # user id
            outfile.write(cleanStr4SQL(data['business_id']) + '\t')  # business_id
            outfile.write(cleanStr4SQL(data['text']) + '\t')  # text
            outfile.write(str(data['stars']) + '\t')  # stars
            outfile.write(str(data['date']) + '\t')  # date
            outfile.write(str(data['funny']) + '\t') # funny
            outfile.write(str(data['useful']) + '\t') # useful
            outfile.write(str(data['cool']) + '\t') # cool
            outfile.write('\n');

            line = f.readline()
            count_line +=1
    print(count_line)
    outfile.close()
    f.close()

parseBusinessData()
parseUserData()
parseCheckinData()
parseReviewData()
