import csv
import json
from string import Template

fieldnames = ("CameraType","LocationDesc","SumPenAmt","Latitude", "Longitude")

with open(r'../Resources/traffic_join_output.csv', 'r') as csvfile, open(r'../Resources/cameras.json', 'w') as jsonfile:
    reader = csv.DictReader(csvfile) #, fieldnames)
    count = 0
    json.dump(json.JSONDecoder().decode("""{"type": "FeatureCollection","copyright": "This data has been transformed from the dataset ACT Transport datasets: https://www.data.act.gov.au/d/2sx9-4wg7 and https://www.data.act.gov.au/d/426s-vdu4 for a Canberra GovHack2017 entry","timestamp": "2017-07-30T02:50:01Z","features": ["""), jsonfile, indent=4)
    firstrun = True
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

        if not firstrun:
            json.dump(",\n")
        jsonTemplate = Template("""{"type": "Feature","properties": {"SumPenAmt": "$SumPenAmt", "CameraType": "$CameraType", "LocationDesc": "$LocationDesc"}, "geometry": {"type": "Point", "coordinates": [$Latitude,$Longitude]}}}""")
        jsonTemplate = jsonTemplate.substitute(SumPenAmt=row['SumPenAmt'], CameraType=row['CameraType'], LocationDesc=row['LocationDesc'], Latitude=row['Latitude'], Longitude=row['Longitude'])   
        
        json.dump(json.JSONDecoder().decode(jsonTemplate), jsonfile, indent=4)

        if (count % 1000):
            print(count)

        if (count > 1000):
            break
    
    json.dump("]}")