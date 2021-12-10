import pymysql
import json
import io
import time
from datetime import datetime
from dateutil import tz
import pytz
conn=pymysql.Connect(host='104.155.203.97', port=3306,
     user='root', passwd='xe9x83xadxe6xbcxa2xe5x9dx87', db='podcastdb', charset='utf8mb4')
cursor = conn.cursor()

with io.open("/home/shank2288s/PodcastSpider/PodcastSpider/spiders/daliyPodcast.json", 'r', encoding="utf-8") as g:
    result_data = json.load(g)
list_Total_info = []

def date_convert(time):
    if (time.split(' ')[-1] == '+0800'):
        time = time.replace('+0800', 'GMT')
        gmt = datetime.strptime(time, '%a, %d %b %Y %H:%M:%S GMT')
        #print( str(gmt.astimezone(pytz.timezone('Asia/Taipei'))).replace('+08:00', ''))
        return( str(pytz.timezone('Asia/Taipei').localize(gmt)).replace('+08:00', ''))
    if (time.split(' ')[-1] == '+0000'):
        time = time.replace('+0000', 'GMT')
    #print ("Date in GMT: {0}".format(time))
    # Hardcode from and to time zones
    from_zone =pytz.timezone('GMT')
    to_zone =pytz.timezone('Asia/Taipei')
    # gmt = datetime.gmtnow()
    gmt = datetime.strptime(time, '%a, %d %b %Y %H:%M:%S GMT')
    # Tell the datetime object that it's in GMT time zone
    gmt = gmt.replace(tzinfo=from_zone)
    # Convert time zone
    eastern_time = str(gmt.astimezone(to_zone))
    return( (eastern_time.replace('+08:00', '')))

def timeToSec(time):
    if ':' not in time:
        return time
    time = time.split(':')
    if (len(time) == 3):
        return str(int(time[0])*60*60 + int(time[1])*60 + int(time[2]))
    if (len(time) == 2):
        return str(int(time[0])*60 + int(time[1]))

def changeToId(conn, name):
    cursor.execute(f"SELECT authorId FROM authors WHERE name ='{name}'")
    data = cursor.fetchone()
    return data[0]

for data in result_data:
    list_info = []
    list_info.append(0)
    list_info.append(changeToId(conn, data['author']))
    list_info.append(data['title'])
    list_info.append(data['mp3'])
    list_info.append(data['desc'])
    list_info.append(timeToSec(data['duration']))
    list_info.append(data['img'])
    list_info.append(date_convert(data['pubDate']))
    list_Total_info.append(list_info)

cursor.executemany("insert ignore into podcastRSS values (%s,%s,%s,%s,%s,%s,%s,%s)", list_Total_info)
conn.commit()
conn.close()
g.close()
localtime = time.asctime(time.localtime(time.time()) )
print('logTime: ', localtime)
''' for testing

sqlstr="select * from podcastRSS"
cursor.execute(sqlstr)
print("author\ttitle\tmp3\tdesc\tduration\timg\tupdate")
for row in cursor:
    print(str(row[0])+"\t\n"+row[1]+"\t\n"+row[2]+"\t\n"+row[3]+"\t\n"+row[4]+"\t\n"+row[5]+"\t\n"+row[6]+"\t\n"+row[7]+"\t\n")
conn.close()

'''
