# Fine me a way 🚔🎥🏎️⏩️💸

Avoid your next speeding fine, help reduce government administrative expenditure

Govhack Canberra 2017 entry:
https://2017.hackerspace.govhack.org/project/fine-away

Simply type in where you want to go and we'll give you directions with the least probability of getting a speeding ticket based on the amount of fines handed out for particular roads!

# License

MIT License

# Installation

 * install Java
 * git clone
 * get the area of your choice in OSM PBF (Open Street Map protobuff) format, we're (obviously) using Canberra 
 * update the cameras.geojson if not Canberra (defaults to canberra traffic camera dataset)
 * run `mvn clean install assembly:single`
 * run `java -jar target/avoid-camera-route-Canberra-1.0-SNAPSHOT-jar-with-dependencies.jar osmreader.osm=Resources/canberra.osm.pbf config=config.properties`
 * wait for the server to load (should be very quick) You should see `Started server at HTTP :8989`
 * open http://localhost:8989
 * change the weighting in the URL to 'avoidcamera' instead of fastest (or another type of weighting) (this is not default!)
 
 # Under the hood
 
 We're using the Graphhopper routing engine to weight the closest edges to a lat, long where a speeding fine has been issued.
 The higher the total fine amount issued, the higher the weighting is for that road segment.
 Thus, this is an optimization problem where the map tries to reduce the weighting from point A to B.
 Using an A\* implementation. Yay for graph theory. That was a nice refresher (and stressful one at that)
