import csv
import json
from string import Template

fieldnames = ("CameraType","LocationDesc","SumPenAmt","Latitude", "Longitude")

with open('traffic_join_output.csv', 'r') as csvfile, open('cameras.json', 'w') as jsonfile:
    reader = csv.DictReader(csvfile) #, fieldnames)
    count = 0
    for row in reader:
        count+=1    
        #filter rows that have empty fieldnames
        skip = False
        for key, value in row.items():
            if key not in fieldnames:
                continue
            else:
                if value in (None, ""):
                    skip = True
        if skip:
            continue

        jsonTemplate = Template("""{"type": "Feature","properties": {"SumPenAmt": "$SumPenAmt", "CameraType": "$CameraType", "LocationDesc": "$LocationDesc", "geometry": {"type": "Point", "coordinates": [$Latitude,$Longitude]}}}""")
        jsonTemplate = jsonTemplate.substitute(SumPenAmt=row['SumPenAmt'], CameraType=row['CameraType'], LocationDesc=row['LocationDesc'], Latitude=row['Latitude'], Longitude=row['Longitude'])   
        
        json.dump(json.JSONDecoder().decode(jsonTemplate), jsonfile, indent=4)
        jsonfile.write(',\n')

        if (count % 1000):
            print(count)