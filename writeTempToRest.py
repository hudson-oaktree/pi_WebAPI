#!/usr/bin/env python
import urllib2
import urllib
from  time import localtime, strftime

def getF():
        tempfile=open("/sys/bus/w1/devices/28-0000068bde7b/w1_slave")
        s=tempfile.read()
        tempfile.close()
        tempdata=s.split("\n")[1].split(" ")[9]
        temp=float(tempdata[2:])
        c=temp/1000
        f=(c*1.8)+32
        return f


tempF =str(getF())
print tempF
query_args = {"Id":0,"tds":(strftime("%Y-%m-%dT%H:%M:%S",localtime())),"tempF":tempF}
print query_args
url = 'http://XXXXXXX.azurewebsites.net/api/housetemps'
data = urllib.urlencode(query_args)
request = urllib2.Request(url, data)
response = urllib2.urlopen(request).read()
print response
